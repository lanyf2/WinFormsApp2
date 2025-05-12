using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vpc
{
    internal class ModbusProgram
    {
        static Modbus.Device.ModbusSerialMaster modbus;
        const byte slaveAddress = 1;
        static bool IsRunning = false;
        internal static int RunDownload(string arg)
        {
            try
            {
                if (IsRunning)
                    return 0;
                IsRunning = true;
                if (modbus == null)
                {
                    if (string.IsNullOrEmpty(arg) || arg.StartsWith("Modbus") == false)
                    {
                        IsRunning = false;
                        Program.MsgBox("编程参数不正确");
                        return 0;
                    }
                    string port = arg.Replace("Modbus", "COM");
                    System.IO.Ports.SerialPort sport = new System.IO.Ports.SerialPort(port);
                    sport.BaudRate = 19200;
                    sport.Open();
                    modbus = Modbus.Device.ModbusSerialMaster.CreateRtu(sport);
                    modbus.Transport.ReadTimeout = 600;
                    modbus.Transport.WriteTimeout = 600;
                    modbus.Transport.Retries = 5;
                }
                if (Settings.Default.初始化写入 != null)
                {
                    if (Settings.Default.初始化写入.Length > 3 && Settings.Default.初始化写入[0] == 65534)
                    {

                    }
                    else
                        for (int i = 0; i < Settings.Default.初始化写入.Length / 2; i++)
                        {
                            modbus.WriteSingleRegister(slaveAddress, Settings.Default.初始化写入[i * 2], Settings.Default.初始化写入[i * 2 + 1]);
                        }
                }
                modbus.WriteSingleCoil(slaveAddress, (ushort)(2000 + 11), true);
                System.Threading.Thread.Sleep(1000);
                for (int i = 1; i < 120; i++)
                {
                    //string ansi = string.Format(Cognex.VisionPro.BlockBase.WaitProgramStr, i + 1);
                    //Cognex.VisionPro.BlockBase.WaitHdl(1001, ansi);
                    var re = modbus.ReadCoils(slaveAddress, 2000 + 12, 2);
                    if (re[0])
                    {
                        IsRunning = false;
                        Cognex.VisionPro.BlockBase.InfoChangedHdl("编程失败");
                        return 1;
                    }
                    else if (re[1])
                    {
                        if (i < 3)
                        {
                            Cognex.VisionPro.BlockBase.InfoChangedHdl("编程时间异常");
                            IsRunning = false;
                            return 1;
                        }
                        IsRunning = false;
                        Cognex.VisionPro.BlockBase.InfoChangedHdl("编程成功");
                        return 0;
                    }
                }
            }
            catch (TimeoutException ex)
            {
                Program.MsgBox("编程器连接失败");
            }
            catch (Exception ex)
            {
                Program.ErrHdl(ex);
            }
            IsRunning = false;
            return -1;
        }
    }
}
