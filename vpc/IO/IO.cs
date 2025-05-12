using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;

namespace vpc
{
    public interface IOHdl
    {
        bool Init();
        bool SetOutput(int portid);
        bool ReSetOutput(int portid);
        void Dispose();
    }
    internal static class IO
    {
        static IOHdl io;
        public static uint InputStatus;
        public static void Init()
        {
            try
            {
                if (Settings.Default.test && System.IO.File.Exists("pemicro.lic"))
                    return;
                if (System.IO.File.Exists("MvIOInterfaceBox.dll"))
                    io = new IOHIKCIO();
                else
                    io = new zmgpio();

                if (io.Init() == false)
                    return;

                Thread thr = new Thread(OutputsHdlFunc);
                thr.Priority = ThreadPriority.BelowNormal;
                thr.Start();
                //Thread thr2 = new Thread(InputsHdlFunc);
                //thr2.Priority = ThreadPriority.BelowNormal;
                //thr2.Start();
                Inited = true;
                Available = true;
            }
            catch (System.Exception ex)
            {
                Program.ErrHdl(ex);
            }
        }
        internal static bool Available = false;
        internal static EventHandler RisingEdgeEvent = null;
        static ConcurrentQueue<bool> ResultQueue = new ConcurrentQueue<bool>();
        internal static bool Inited = false;
        static AutoResetEvent ResetEvt = new AutoResetEvent(false);
        internal static int QueueCount
        {
            get { return ResultQueue.Count; }
        }
        internal static void AddToOutputQueue(bool IsFailed)
        {
            if (Available)
            {
                //lock (ResultQueue)
                {
                    ResultQueue.Enqueue(IsFailed);
                }
                ResetEvt.Set();
            }
        }
        internal const int PluseWidth = 10;
        static void InputsHdlFunc()
        {
            try
            {
                uint StatusMask = 0, PreMask = 0;
                int nRet = 0;
                PreMask = StatusMask;
                while (Inited)
                {
                    byte[] byteStatus = new byte[1024];
                    //nRet = CIOControllerSDK.MV_IO_GetMainInputLevel_CS(ref byteStatus[0]);  // 用字节数组接收动态库传过来的字符串
                    //int nGPIStatus = BitConverter.ToInt32(byteStatus, 0);
                    InputStatus = byteStatus[0];
                    //if (CIOControllerSDK.MV_OK != nRet)
                    if (0 != nRet)
                    {
                        Program.MsgBox("Getting the electrical level status failed.");
                    }
                    else
                    {


                        if (StatusMask != PreMask)
                        {
                            uint rising = StatusMask & (StatusMask ^ PreMask);
                            if (rising != 0 && RisingEdgeEvent != null)
                                RisingEdgeEvent(rising, EventArgs.Empty);
                        }
                        PreMask = StatusMask;
                    }

                    Thread.Sleep(3);
                }
            }
            catch (Exception ex)
            {
                Program.ErrHdl(ex);
            }
        }
        static void OutputsHdlFunc()
        {
            try
            {
                while (Inited)
                {
                    if (ResultQueue.Count > 0)
                    {
                        bool IsFailed;
                        if (ResultQueue.TryDequeue(out IsFailed))
                        {
                            if (IsFailed)
                            {//次品
                                if (io.SetOutput(0) == false)
                                {
                                    Available = false;
                                    return;
                                }
                                Thread.Sleep(PluseWidth);
                                if (io.ReSetOutput(0) == false)
                                {
                                    Available = false;
                                    return;
                                }
                            }
                            else
                            {//合格品
                                if (io.SetOutput(1) == false)
                                {
                                    Available = false;
                                    return;
                                }
                                Thread.Sleep(PluseWidth);
                                if (io.ReSetOutput(1) == false)
                                {
                                    Available = false;
                                    return;
                                }
                            }
                            Thread.Sleep(PluseWidth);
                        }
                    }
                    if (ResultQueue.Count == 0)
                        ResetEvt.WaitOne();
                }
            }
            catch (System.Exception ex)
            {
                Program.ErrHdl(ex);
            }
            Inited = false;
        }
        internal static void Dispose()
        {
            Inited = false;
            ResetEvt.Set();
            try
            {
                if (io != null)
                    io.Dispose();
            }
            catch
            { }
        }
    }
}
