using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace vpc
{
    class zmgpio : IOHdl
    {
        private static OpenLibSys.Ols MyOls;
        public void Dispose()
        {
            MyOls?.Dispose();
        }

        public bool Init()
        {
            MyOls = new OpenLibSys.Ols();
            var re = (OpenLibSys.Ols.Status)MyOls.GetStatus();
            if (re != OpenLibSys.Ols.Status.NO_ERROR)
                throw new Exception("IO初始化失败：" + re);
            return true;
        }

        public bool ReSetOutput(int portid)
        {
            if (MyOls.WriteIoPortByte != null)
            {
                byte v = 0x0F;
                MyOls.WriteIoPortByte(0xA06, v);
                return true;
            }
            else
                return false;
        }

        public bool SetOutput(int portid)
        {
            if (MyOls.WriteIoPortByte != null)
            {
                byte v = (byte)(0x0F & ~(1 << portid));
                MyOls.WriteIoPortByte(0xA06, v);
                return true;
            }
            else
                return false;
        }
    }
    class zmgpioWinIO : IOHdl
    {
        public void Dispose()
        {
            ShutdownWinIo();
        }

        public bool Init()
        {
            if (InitializeWinIo() == false)
                throw new Exception("IO初始化失败");
            uint[] ut = new uint[10];
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 5; i++)
            {
                IntPtr p = Marshal.UnsafeAddrOfPinnedArrayElement(ut, i);
                var re = GetPortVal((ushort)(0xa00 + i), p, 1);
                if (re == false)
                {
                    Program.MsgBox(i.ToString() + re);
                    return false;
                }
                sb.Append(ut[i].ToString("x")).Append("-");
            }
            Program.MsgBox(sb.ToString());
            return true;
        }

        public bool ReSetOutput(int portid)
        {
            return SetPortVal(0x0A06, 0x00, 1);
        }

        public bool SetOutput(int portid)
        {
            return SetPortVal(0x0A06, 0x00, 1);
        }
        //3 param:1 byte 2 WORD  3 DWORD
        [DllImport("WinIo64.dll", EntryPoint = "GetPortVal", CallingConvention = CallingConvention.StdCall)]
        internal static extern bool GetPortVal(ushort wPortAddr, IntPtr pdwPortVal, byte bSize);
        [DllImport("WinIo64.dll", EntryPoint = "SetPortVal", CallingConvention = CallingConvention.StdCall)]
        internal static extern bool SetPortVal(ushort wPortAddr, uint dwPortVal, byte bSize);
        [DllImport("WinIo64.dll", EntryPoint = "InitializeWinIo", CallingConvention = CallingConvention.StdCall)]
        internal static extern bool InitializeWinIo();
        [DllImport("WinIo64.dll", EntryPoint = "ShutdownWinIo", CallingConvention = CallingConvention.StdCall)]
        internal static extern void ShutdownWinIo();
        [DllImport("WinIo64.dll", EntryPoint = "InstallWinIoDriver", CallingConvention = CallingConvention.StdCall)]
        internal static extern bool InstallWinIoDriver(string pszWinIoDriverPath, bool IsDemandLoaded);


    }
}
