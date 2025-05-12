using Cognex.VisionPro.ToolBlock;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using vpc;

namespace Cognex.VisionPro
{
    internal static class JobManager
    {
        internal static event Action<ResultStruct, int> ResultChanged;
        internal static event Action<ResultStruct> IOResultChanged;
        internal static event Action<RxInterface, int> ResultUpdate;
        internal static event EventHandler ImageGrabbed;
        internal static event EventHandler InitInfo;
        internal static event Action NewTrig;
        internal static BlockBase[] MtdBlocks;
        internal static ResultStruct LastResult;
        internal static LinkedList<ResultStruct> ResultList = new LinkedList<ResultStruct>();
        internal static JobGraphicsGeneral graphics = new JobGraphicsGeneral();
        internal static int PassCount;
        internal static int FailCount;
        internal static bool TestImgFlag = false;
        internal static bool LoadJobFlag = true;
        internal static bool SaveTestImgFlag = false;
        //internal static LinkedList<RunInfo> ImgBuf = new LinkedList<RunInfo>();

        internal static bool initflag = false;
        internal static bool Inited
        {
            get
            {
                if (Settings.Default.test)
                    return true;
                if (MtdBlocks == null)
                    return false;
                for (int i = 0; i < MtdBlocks.Length; i++)
                {
                    if (MtdBlocks[i] == null)
                        return false;
                }
                return initflag;
            }
        }
        internal static int TotalCount
        {
            get { return PassCount + FailCount; }
        }
        internal static double FailRaito
        {
            get
            {
                if (PassCount + FailCount == 0)
                    return 0;
                else
                    return 100.0 * FailCount / (PassCount + FailCount);
            }
        }
        internal static void NewTrigHdl()
        {
            NewTrig?.Invoke();
        }
        internal static void SyncBuf()
        {
            StringBuilder sb = new StringBuilder();
            bool syncflag = false;
            for (int i = 0; i < rBuf.Length; i++)
            {
                sb.Append(rBuf[i].Count).Append(",");
                if (rBuf[i].Count > 0)
                {
                    syncflag = true;
                    rBuf[i] = new System.Collections.Concurrent.ConcurrentQueue<R1Class>();
                }
            }
            if (syncflag)
            {
                string s = "缓存异常，已重新同步：" + sb;
                Program.Loginfo(s);
                for (int i = 0; i < MtdBlocks.Length; i++)
                {
                    Program.mf.UpdateGraphics(null, i);
                }
                Program.mf.UpdateStatusText(s);
            }
        }

        internal static void TrySaveProdCounts()
        {
            try
            {
                if (MtdBlocks != null)
                {
                    int len = MtdBlocks.Length * 2 + 2;
                    if (Settings.Default.CountsLog == null || Settings.Default.CountsLog.Length != len)
                        Settings.Default.CountsLog = new int[len];
                    Settings.Default.CountsLog[0] = PassCount;
                    Settings.Default.CountsLog[1] = FailCount;
                    for (int i = 0; i < MtdBlocks.Length; i++)
                    {
                        if (MtdBlocks[i] != null)
                        {
                            Settings.Default.CountsLog[i * 2 + 2] = MtdBlocks[i].PassCount;
                            Settings.Default.CountsLog[i * 2 + 3] = MtdBlocks[i].FailCount;
                        }
                    }
                    Settings.Default.Save();
                }
            }
            catch
            {
            }
        }
        internal static void TryLoadProdCounts()
        {
            try
            {
                if (MtdBlocks != null)
                {
                    int len = MtdBlocks.Length * 2 + 2;
                    if (Settings.Default.CountsLog != null && Settings.Default.CountsLog.Length == len)
                    {
                        PassCount = Settings.Default.CountsLog[0];
                        FailCount = Settings.Default.CountsLog[1];
                        for (int i = 0; i < MtdBlocks.Length; i++)
                        {
                            if (MtdBlocks[i] != null)
                            {
                                MtdBlocks[i].PassCount = Settings.Default.CountsLog[i * 2 + 2];
                                MtdBlocks[i].FailCount = Settings.Default.CountsLog[i * 2 + 3];
                            }
                        }
                    }
                }
            }
            catch
            {
            }
        }
        internal static void SyncExposureTime(int id)
        {
            if (id == -1)
            {
                for (int i = 0; i < MtdBlocks?.Length; i++)
                    SyncExposureTime(i);
            }
            else
            {
                if (id < MtdBlocks?.Length)
                    MtdBlocks[id].SyncExposureTime();
            }
        }
        internal static void LoadJob()
        {
            if (File.Exists(Settings.Default.jobs) == false)
            {
                Program.ErrHdl("无法找到配方：" + Settings.Default.jobs);
                if (Settings.Default.jobs.IsNullOrEmpty())
                    Settings.Default.jobs = @"jobs\默认配方.vpp";
                Settings.Default.Save();
                SaveJob();
            }
            else
                LoadJob(Settings.Default.jobs);
        }
        internal static void LoadJob(string jobPath)
        {
            if (File.Exists(jobPath))
            {
                LoadJobFlag = true;
                CogToolBlock ct = CogSerializer.LoadObjectFromFile(jobPath) as CogToolBlock;
                if (MtdBlocks == null)
                {
                    Program.MsgBox("算法未初始化\r\n");
                    return;
                }
                if (ct == null)
                {
                    Program.MsgBox("无法解析该配方\r\n" + jobPath);
                    return;
                }
                int ctlen = Math.Min(ct.Tools.Count, MtdBlocks.Length);
                for (int i = 0; i < ctlen; i++)
                {
                    if (MtdBlocks[i] == null)
                        continue;
                    int len = 0;
                    CogToolBlock bi = ct.Tools[i] as CogToolBlock;
                    if (bi != null && MtdBlocks[i] != null && MtdBlocks[i].internalblock != null)
                    {
                        Block1Collection b1 = MtdBlocks[i] as Block1Collection;
                        if (b1 != null)
                            b1.ClearParmsSettings();
                        len = Math.Min(bi.Inputs.Count, MtdBlocks[i].internalblock.Inputs.Count);
                        for (int j = 0; j < len; j++)
                        {
                            if (MtdBlocks[i].internalblock.Inputs[j].Name == "CameraHdl")
                                continue;
                            if (MtdBlocks[i].internalblock.Inputs[j].ValueType == bi.Inputs[j].ValueType
                            || MtdBlocks[i].internalblock.Inputs[j].ValueType.IsAssignableFrom(bi.Inputs[j].ValueType))
                                MtdBlocks[i].internalblock.Inputs[j].Value = bi.Inputs[j].Value;
                            else
                                Program.Loginfo(string.Format("{0}_{1}->{2}_{3}，配方不匹配", j, bi.Inputs[j].ValueType.Name, i, MtdBlocks[i].internalblock.Inputs[j].ValueType.Name));
                        }
                        if (MtdBlocks[i].internalblock.Inputs.Contains("TestID"))
                            MtdBlocks[i].internalblock.Inputs["TestID"].Value = -1;
                    }
                }
                if (MtdBlocks.Length != ct.Tools.Count)
                    Program.MsgBox("作业参数不一致，请退出程序后从桌面的快捷方式启动");
            }
            else
            {
                SaveJob(@"jobs\默认配方.vpp");
                Program.MsgBox("配方路径设置有误：" + jobPath);
            }
        }
        internal static void SaveJob()
        {
            SaveJob(Settings.Default.jobs);
        }
        internal static void SaveJob(string jobPath)
        {
            if (MtdBlocks == null)
                return;
            if (Directory.Exists("jobs") == false)
                Directory.CreateDirectory("jobs");
            if (jobPath != null && System.IO.Path.GetExtension(jobPath) == ".vpp")
            {
                CogToolBlock ct = new CogToolBlock();
                for (int i = 0; i < MtdBlocks.Length; i++)
                {
                    CogToolBlock c = new CogToolBlock();
                    c.Name = "b" + (i + 1);
                    if (MtdBlocks[i] != null)
                    {
                        c.Inputs.Add(new CogToolBlockTerminal(MtdBlocks[i].internalblock.Inputs[0].Name, MtdBlocks[i].internalblock.Inputs[0].ValueType));
                        for (int j = 1; j < MtdBlocks[i].internalblock.Inputs.Count; j++)
                        {
                            if (MtdBlocks[i].internalblock.Inputs[j].Name == "CameraHdl")
                                continue;
                            if (MtdBlocks[i].internalblock.Inputs[j].Value == null)
                                c.Inputs.Add(new CogToolBlockTerminal(MtdBlocks[i].internalblock.Inputs[j].Name, MtdBlocks[i].internalblock.Inputs[j].ValueType));
                            else
                            {
                                c.Inputs.Add(new CogToolBlockTerminal(MtdBlocks[i].internalblock.Inputs[j].Name, MtdBlocks[i].internalblock.Inputs[j].Value));
                            }
                        }
                    }
                    ct.Tools.Add(c);
                }
                if (string.IsNullOrEmpty(Settings.Default.打印样式文件) == false)
                    ct.Inputs.Add(new CogToolBlockTerminal("dayinyangshiwenjian", Settings.Default.打印样式文件));
                //if (string.IsNullOrEmpty(Settings.Default.VCF) == false)
                //    ct.Inputs.Add(new CogToolBlockTerminal("VCF", Settings.Default.VCF));
                //if (string.IsNullOrEmpty(Settings.Default.零部件名称) == false)
                //    ct.Inputs.Add(new CogToolBlockTerminal("lingbujianmingcheng", Settings.Default.零部件名称));
                //if (string.IsNullOrEmpty(Settings.Default.零件号) == false)
                //    ct.Inputs.Add(new CogToolBlockTerminal("lingjianhao", Settings.Default.零件号));
                //if (string.IsNullOrEmpty(Settings.Default.软件版本) == false)
                //    ct.Inputs.Add(new CogToolBlockTerminal("ruanjianbanben", Settings.Default.软件版本));
                //if (string.IsNullOrEmpty(Settings.Default.硬件版本号) == false)
                //ct.Inputs.Add(new CogToolBlockTerminal("yinjianbanbenhao", Settings.Default.硬件版本号));
                //if (string.IsNullOrEmpty(Settings.Default.版本选择D530) == false)
                //    ct.Inputs.Add(new CogToolBlockTerminal("d530banbenxuanze", Settings.Default.版本选择D530));
                //if (string.IsNullOrEmpty(Settings.Default.DIDBuildString) == false)
                //    ct.Inputs.Add(new CogToolBlockTerminal("DIDBuildString", Settings.Default.DIDBuildString));
                //if (string.IsNullOrEmpty(Settings.Default.PEDownload) == false)
                //    ct.Inputs.Add(new CogToolBlockTerminal("PEDownload", Settings.Default.PEDownload));
                //if (Settings.Default.初始化写入 != null)
                //    ct.Inputs.Add(new CogToolBlockTerminal("InitWriteBuffer", Settings.Default.初始化写入));
                if (FormPLC.位置参数 != null)
                    ct.Inputs.Add(new CogToolBlockTerminal("wzcs", FormPLC.位置参数));
                CogSerializer.SaveObjectToFile(ct, jobPath);
            }
            else
                Program.MsgBox("配方名称有误：\r\n" + jobPath);
        }
        internal static void TryLoadSettings(string path, bool autoFitStep = false)
        {
            if (File.Exists(path))
            {
                CogToolBlock ct = CogSerializer.LoadObjectFromFile(path) as CogToolBlock;
                if (ct != null)
                {
                    string str;
                    if (ct.Inputs.Contains("dayinyangshiwenjian"))
                    {
                        str = ct.Inputs["dayinyangshiwenjian"].Value as string;
                        if (string.IsNullOrEmpty(str) == false)
                            Settings.Default.打印样式文件 = str;
                    }
                    if (ct.Inputs.Contains("DIDBuildString"))
                    {
                        str = ct.Inputs["DIDBuildString"].Value as string;
                        if (string.IsNullOrEmpty(str) == false)
                            Settings.Default.DIDBuildString = str;
                    }
                    if (ct.Inputs.Contains("wzcs"))
                    {//wzcs
                        if (ct.Inputs["wzcs"].Value is float[] f)
                        {
                            FormPLC.位置参数 = f;
                        }
                    }
                    if (ct.Inputs.Contains("InitWriteBuffer"))
                    {
                        ushort[] buffer = ct.Inputs["InitWriteBuffer"].Value as ushort[];
                        if (buffer != null)
                            Settings.Default.初始化写入 = buffer;
                    }
                    if (ct.Tools.Count > 0 && autoFitStep)
                        Settings.Default.StepCount = ct.Tools.Count;
                }
            }
        }
        internal static void TrySaveSettings(string path)
        {
            if (File.Exists(path))
            {
                CogToolBlock ct = CogSerializer.LoadObjectFromFile(path) as CogToolBlock;
                if (ct != null)
                {
                    bool flag = false;
                    string str;
                    if (Settings.Default.打印样式文件 != null)
                        if (ct.Inputs.Contains("dayinyangshiwenjian"))
                        {
                            str = ct.Inputs["dayinyangshiwenjian"].Value as string;
                            if (str != Settings.Default.打印样式文件)
                            {
                                ct.Inputs["dayinyangshiwenjian"].Value = Settings.Default.打印样式文件;
                                flag = true;
                            }
                        }
                        else
                        {
                            ct.Inputs.Add(new CogToolBlockTerminal("dayinyangshiwenjian", Settings.Default.打印样式文件));
                            flag = true;
                        }
                    if (FormPLC.位置参数 != null)
                        if (ct.Inputs.Contains("wzcs"))
                        {//wuliaosize
                            if (ct.Inputs["wzcs"].Value is float[] f)
                                if (false == PlcInterface.IsEqual(f, FormPLC.位置参数))
                                {
                                    ct.Inputs["wzcs"].Value = FormPLC.位置参数;
                                    flag = true;
                                }
                        }
                        else
                        {
                            ct.Inputs.Add(new CogToolBlockTerminal("wzcs", FormPLC.位置参数));
                            flag = true;
                        }
                    if (Settings.Default.初始化写入 != null)
                        if (ct.Inputs.Contains("InitWriteBuffer"))
                        {
                            ushort[] buf = ct.Inputs["InitWriteBuffer"].Value as ushort[];
                            if (false == PlcInterface.IsEqual(buf, Settings.Default.初始化写入))
                            {
                                ct.Inputs["InitWriteBuffer"].Value = Settings.Default.初始化写入;
                                flag = true;
                            }
                        }
                        else
                        {
                            ct.Inputs.Add(new CogToolBlockTerminal("InitWriteBuffer", Settings.Default.初始化写入));
                            flag = true;
                        }
                    if (flag)
                    {
                        CogSerializer.SaveObjectToFile(ct, path);
                    }
                }
            }
        }
        internal static void Init()
        {
            //Settings.Default.初始化写入2 = new string[] { "{0:yyyyMM}" };
            //Settings.Default.初始化写入2地址 = new ushort[] { 123 };
            //Form1.plcHdl = new PLCHdl("COM1", StartButtonPressedHdl: TrigHdl);
            //Form1.plcHdl.AssertInitBuffer();
            try
            {
                Block1Collection.init();
            }
            catch (System.Reflection.ReflectionTypeLoadException ex)
            {
                for (int i = 0; i < ex.LoaderExceptions.Length; i++)
                {
                    Program.ErrHdl(ex.LoaderExceptions[i]);
                }
            }

            System.Management.ManagementClass mc = new ManagementClass("win32_processor");
            ManagementObjectCollection moc = mc.GetInstances();
            StringBuilder sb = new StringBuilder();

            foreach (ManagementObject mo in moc)
            {
                if ((string)mo["processorid"] == "BFEBFBFF000306A9" || (string)mo["processorid"] == "BFEBFBFF00020652"
                    || GetMD5str((string)mo["processorid"]) == Settings.Default.k1
                    )//BFEBFBFF00020652
                {
                    break;
                }
                else
                //if (false)
                {
                    //Program.Loginfo("U" + (string)mo["processorid"]);
                    //throw (new Cognex.VisionPro.Exceptions.CogSecurityViolationException());
                }
            }
            mc = new System.Management.ManagementClass("Win32_BaseBoard");
            moc = mc.GetInstances();
            foreach (ManagementObject mx in moc)
            {
                if ((string)mx.Properties["SerialNumber"].Value != "LXPXL0100302525D752000        ")
                {
                    //Program.Loginfo((string)mx.Properties["SerialNumber"].Value);
                }
            }
            mc = new System.Management.ManagementClass("Win32_DiskDrive");
            moc = mc.GetInstances();
            bool flag = true;
            foreach (ManagementObject mo in moc)
            {
                if ((string)mo.Properties["SerialNumber"].Value == "3531303537393034363136382020202020202020" ||
                    GetMD5str((string)mo.Properties["SerialNumber"].Value) == Settings.Default.k2
                   )
                {
                    flag = false;
                    break;
                }
                else
                {
                    if ("F69511F8DAD7F6DC09A2AAA9058D2478" == Settings.Default.k2)
                    {
                        Settings.Default.k2 = GetMD5str((string)mo.Properties["SerialNumber"].Value);
                        Settings.Default.Save();
                        flag = false;
                        break;
                    }
                    else
                        sb.Append("E3" + (string)mo.Properties["SerialNumber"].Value);
                }
            }
            //if (DateTime.Now >= new DateTime(2016, 10, 1))
            //{
            //    Settings.Default.k4 = null;
            //    Settings.Default.Save();
            //}

            //if (Settings.Default.k4 != "A71BD02D02DF49FACE3AA49682814027")
            //    flag = true;
            //if (false)
            if (flag)
            {
                //throw (new Cognex.VisionPro.Exceptions.CogSecurityViolationException());
            }

            MtdBlocks = new BlockBase[Settings.Default.StepCount];
            rBuf = new System.Collections.Concurrent.ConcurrentQueue<R1Class>[Settings.Default.StepCount];
            for (int i = 0; i < rBuf.Length; i++)
                rBuf[i] = new System.Collections.Concurrent.ConcurrentQueue<R1Class>();
            LastResult = null;
            InitInfoHdl(10);
            Block1Collection b0 = new Block1Collection();
            MtdBlocks[0] = b0;
            InitInfoHdl(30);
            for (int i = 1; i < Settings.Default.StepCount; i++)
            {
                MtdBlocks[i] = new Block1Collection(b0, i);
                InitInfoHdl(10 + 70 * (i + 1) / Settings.Default.StepCount);
            }
            for (int i = 0; i < Settings.Default.StepCount; i++)
            {
                MtdBlocks[i].camUsb = new HIKHdl(i + 1);
                MtdBlocks[i].camUsb.Init();
                MtdBlocks[i].camUsb.ImageGrabbed += MtdBlocks[i].CamUsb_ImageGrabbed;
            }

            TryLoadProdCounts();
            BlockBase.ResultChanged += ResultChangedHdl;
            BlockBase.FifoChanged += FifoChangedHdl;

            InitInfoHdl(90);
            LoadJob();
            SyncExposureTime(-1);

            ThreadPool.QueueUserWorkItem(checkResult);

            //ResetFifo();
            string port;
            if (string.IsNullOrEmpty(Settings.Default.PLCPort))
                port = "192.168.6.233";
            else
                port = Settings.Default.PLCPort;
            var ad = new List<int>();
            for (int i = 0; i < 20; i++)
                ad.Add(PlcInterface.AddrBase - 50 + i);
            if (port.StartsWith("COM"))
                Form1.plcHdl = new PlcModbus(port);
            else
                Form1.plcHdl = new PlcModbusTcp(port);

            IO.Init();

            Form1.plcHdl.InitAddrs(ad);
            InitInfoHdl(100);
            initflag = true;
        }
        internal static void Clear()
        {
            ResultList.Clear();
            PassCount = 0;
            FailCount = 0;
            LastResult = null;
            if (IsRunning == false)
                for (int i = 0; i < rBuf?.Length; i++)
                    rBuf[i] = new System.Collections.Concurrent.ConcurrentQueue<R1Class>();
            for (int i = 0; i < MtdBlocks?.Length; i++)
            {
                if (MtdBlocks[i] != null)
                    MtdBlocks[i].PassCount = MtdBlocks[i].FailCount = 0;
            }
        }
        internal static void CheckLicense()
        {
            Cognex.VisionPro.Implementation.Internal.CogLicense.Check(Cognex.VisionPro.Implementation.Internal.CogLicenseConstants.PatMax);
        }
        internal static void TrigOne(int index = 0, int sleepTime = 0)
        {
            if (index < MtdBlocks.Length && index >= 0)
            {
                MtdBlocks[index].RunAsync(null, sleepTime);
            }
            else
            {
                int flag = 1;
                for (int i = 0; i < MtdBlocks.Length; i++)
                {
                    if ((flag & index) > 0 && MtdBlocks[i] != null)
                    {
                        MtdBlocks[i].RunAsync();
                        break;
                    }
                    else
                        flag <<= 1;
                }
            }
        }
        internal static void TrigTest(ICogImage img, int index = 0)
        {
            TestImgFlag = true;
            if (img == null)
                SaveTestImgFlag = true;
            MtdBlocks[index].RunAsync(img);
        }
        internal static bool IsRunning = false;
        internal static string ToCompactAscii(this ushort[] t)
        {
            if (t == null || t.Length == 0)
                return null;
            //StringBuilder sb = new StringBuilder();
            byte[] ary = new byte[t.Length * 2];
            for (int i = 0; i < t.Length; i++)
            {
                ary[i * 2] = (byte)(t[i] & 0xFF);
                ary[i * 2 + 1] = (byte)(t[i] >> 8);
            }
            var cd = Encoding.GetEncoding("gb2312");
            return cd.GetString(ary);
            //if (sb[sb.Length - 1] == '\0')
            //    return sb.ToString(0, sb.Length - 1);
            //else
            //    return sb.ToString();
        }

        static void InitInfoHdl(int process)
        {
            if (InitInfo != null)
                InitInfo(process, EventArgs.Empty);
        }
        static void FifoChangedHdl(object sender, EventArgs e)
        {
            BlockBase bb = sender as BlockBase;
            if (bb != null && bb.fifo != null)
                for (int i = 0; i < MtdBlocks.Length; i++)
                {
                    if (i != bb.blockid)
                        MtdBlocks[i].fifo = bb.fifo;
                }
        }
        static ResultStruct NewResultHdl()
        {
            ResultStruct re = new ResultStruct();
            ResultList.AddLast(re);
            if (ResultList.Count > Settings.Default.ResultBufferCapacity)
                ResultList.RemoveFirst();
            return re;
        }
        static void ApplyResult(int id, RxInterface r)
        {

        }
        internal static System.Collections.Concurrent.ConcurrentQueue<R1Class>[] rBuf;
        static ManualResetEvent mre = new ManualResetEvent(true);
        static void checkResult(object re)
        {
            while (!Program.ExitFlag)
            {
                try
                {
                    mre.WaitOne(20);
                    DateTime tm = DateTime.MinValue;
                    bool finflag = true;
                    for (int i = rBuf.Length - 1; i >= 0; i--)
                    {
                        if (Settings.Default.相机停用?.Length > i)
                            if (Settings.Default.相机停用[i] == true)
                                continue;
                        if (rBuf[i].Count == 0)
                        {
                            finflag = false;
                            break;
                        }
                    }
                    //if (rBuf[0].Count == 0)
                    if (!finflag)
                    {
                        continue;
                    }

                    var rrr = NewResultHdl();
                    for (int i = rBuf.Length - 1; i >= 0; i--)
                        if (rBuf[i].TryDequeue(out R1Class r))
                            rrr[i] = r;

                    if (Settings.Default.SaveToFile == SaveType.SaveAll)
                        Form1.SaveImg.SaveFileAsync(rrr);
                    else if (Settings.Default.SaveToFile == SaveType.SaveFails && rrr.Passed == false)
                        Form1.SaveImg.SaveFileAsync(rrr);
                    if (rrr.Passed)
                        PassCount++;
                    else
                        FailCount++;
                    if(Settings.Default.通过IO输出结果)
                        IO.AddToOutputQueue(!rrr.Passed);
                    else
                    {
                        if (rrr.Passed)
                            Form1.plcHdl.WritePlcInternalAsync(1043, (ushort)2);
                        else
                            Form1.plcHdl.WritePlcInternalAsync(1043, (ushort)3);
                    }
                    rrr.iotime = DateTime.Now;
                    IOResultChanged?.Invoke(rrr);
                }
                catch (Exception ex)
                {
                    Program.ErrHdl(ex);
                }
                //Thread.Sleep(10);
            }
        }
        static void ResultChangedHdl(BlockBase bb)
        {
            try
            {
                if (TestImgFlag)
                {
                    TestImgFlag = false;
                    bool nf = false;
                    if (LastResult == null)
                        nf = true;
                    else if (LastResult[bb.blockid] != null)
                        nf = true;
                    ResultStruct re = LastResult;
                    if (nf)
                        re = LastResult = NewResultHdl();
                    if ((TotalCount & 0x1111) == 0x1111)
                        GC.Collect();
                    LastResult[bb.blockid] = bb.lastResult;
                    ResultChanged?.Invoke(re, bb.blockid);
                }
                else
                {
                    var re = bb.lastResult;
                    if (LastResult == null)
                        LastResult = new ResultStruct();
                    LastResult[bb.blockid] = re;
                    rBuf[bb.blockid].Enqueue(re);
                    mre.Set();
                    ResultUpdate?.Invoke(re, bb.blockid);
                }
            }
            catch (Exception ex)
            {
                Program.ErrHdl(ex);
            }
        }
        static void ResultChangedHdlbak(BlockBase bb)
        {
            try
            {
                ResultStruct re = LastResult;
                bool nf = false;
                if (LastResult == null)
                    nf = true;
                else if (LastResult[bb.blockid] != null)
                    nf = true;
                if (nf)
                {
                    re = LastResult = NewResultHdl();
                }
                if ((TotalCount & 0x1111) == 0x1111)
                    GC.Collect();
                if (TestImgFlag)
                    LastResult[bb.blockid] = bb.lastResult;
                else
                    ApplyResult(bb.blockid, bb.lastResult);

                TestImgFlag = false;
                ResultChanged?.Invoke(re, bb.blockid);
            }
            catch (Exception ex)
            {
                Program.ErrHdl(ex);
            }
        }
        static void ResetFifo()
        {
            if (Inited)
            {
                //if (JobManager.camUsb != null && JobManager.camUsb.Connected)
                {
                    if (Settings.Default.Cams != null && Settings.Default.CamFlag == 0)
                        for (int i = 0; i < Math.Min(Settings.Default.Cams.Length, MtdBlocks.Length); i++)
                        {
                            int index;
                            if (int.TryParse(Settings.Default.Cams[i], out index) && index > 0)
                                MtdBlocks[i].camUsb = new KSJHdl(index);
                        }
                    return;
                }
                StringBuilder sb = new StringBuilder();
                int len = Math.Min((int)MtdBlocks?.Length, (int)Settings.Default.Cams?.Length);
                for (int i = 0; i < 1; i++)
                {
                    if (MtdBlocks[i] != null && MtdBlocks[i].fifo == null)
                    {
                        if (Settings.Default.Cams[i] == null)
                            MtdBlocks[i].Camera = null;
                        else
                            MtdBlocks[i].Camera = CamerasGige.GetCamera(Settings.Default.Cams[i]);
                        if (MtdBlocks[i].Camera == null)
                            sb.Append("相机").Append(i + 1).Append(',');
                    }
                }
                if (sb.Length > 0)
                {
                    sb.Remove(sb.Length - 1, 1);
                    sb.Append("无法连接");
                    if (Settings.Default.test == false)
                        Program.ErrHdl(sb.ToString());
                }
            }
        }
        internal static void Dispose()
        {
            //TrySaveProdCounts();
            if (MtdBlocks != null)
                for (int i = 0; i < MtdBlocks.Length; i++)
                {
                    if (MtdBlocks[i] != null)
                        MtdBlocks[i].Dispose();
                }
            if (ExcelHdl.b1c != null)
                ExcelHdl.b1c.Dispose();
            IO.Dispose();
        }
        internal static bool AutoSerial(ResultStruct re1)
        {
            if (Settings.Default.CheckSerialAuto)
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 1; i < re1.ExtraInfo?.Length; i += 2)
                {
                    sb.Append(re1.ExtraInfo[i]);
                }
                sb.Append("{0}");
                var mtcs = sb.ToString();
                var infosTbAdapter = new vpc.DatabaseDataSetTableAdapters.infosTableAdapter();
                string ser0 = Settings.Default.PrintSerialNum;
                if (ser0.IsNullOrEmpty())
                {
                    Program.MsgBox("打印模版未包含序列号");
                    return false;
                }
                int serNum;
                if (false == int.TryParse(ser0, out serNum))
                {
                    Program.MsgBox("初始序列号非数字：" + ser0);
                    return false;
                }
                int serLen = ser0.Length;
                int maxSer = (int)Math.Pow(10, serLen);
                for (int i = serNum; i < maxSer; i++)
                {
                    var ser = i.ToString().PadLeft(serLen, '0');
                    var mtcN = mtcs.FormatWith(ser);
                    var res = infosTbAdapter.GetDataByMTC(mtcN);
                    if (res.Count == 0)
                    {
                        infosTbAdapter.Insert(DateTime.Now, mtcN, null, null, null, null, LogIn.CurrentUser.Name);
                        Settings.Default.PrintSerialNum = ser;
                        return true;
                    }
                }
                Program.MsgBox("无可用序列号");
                return false;
            }
            else
                return true;
        }
        internal static bool SaveDID(ResultStruct re1)
        {
            if (Settings.Default.CheckDidDB)
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 1; i < re1.ExtraInfo?.Length; i += 2)
                {
                    sb.Append(re1.ExtraInfo[i]);
                }
                if (sb.Length == 0)
                {
                    Program.MsgBox("缺少DID信息");
                    return false;
                }
                var mtcs = sb.ToString();
                var infosTbAdapter = new vpc.DatabaseDataSetTableAdapters.infosTableAdapter();
                var res = infosTbAdapter.GetDataByMTC(mtcs);
                if (res.Count > 0)
                {
                    Program.MsgBox("DID重复2：" + mtcs);
                    return false;
                }
                else
                {
                    infosTbAdapter.Insert(DateTime.Now, mtcs, null, null, null, null, LogIn.CurrentUser.Name);
                    return true;
                }
            }
            else
                return true;
        }
        internal static bool CheckDIdSame(ResultStruct re1, ResultStruct re2)
        {
            if (re1 != null && re2 != null && re1.ExtraInfo != null && re2.ExtraInfo != null
                && re1.ExtraInfo.Length == re2.ExtraInfo.Length && re1.ExtraInfo.Length > 1)
            {
                for (int i = 1; i < re1.ExtraInfo.Length; i += 2)
                {
                    if (re1.ExtraInfo[i] != re2.ExtraInfo[i])
                        return false;
                }
                return true;
            }
            return false;
        }
        internal static void Print(ResultStruct re)
        {
            if (Settings.Default.打印启用)
            {
                if (re != null && re.LastNullId != 0)
                    barTenderPrint.Print(re);
                else
                    Program.MsgBox("无可打印结果");
            }
            //barTenderPrint.Print(Settings.Default.VCF, Settings.Default.零部件名称, Settings.Default.零件号, Settings.Default.软件版本, Settings.Default.硬件版本号, re.MTC, re.ExtraInfo);
        }
        internal static BarTenderPrint barTenderPrint = new BarTenderPrint();

        internal static byte[] GetMD5(byte[] data)
        {
            byte[] hash = new byte[16];
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            hash = md5.ComputeHash(data);
            return hash;
        }
        internal static byte[] GetMD5(string data)
        {
            return GetMD5(Encoding.UTF8.GetBytes(data));
        }
        internal static string GetMD5str(string data)
        {
            return bytesToString(GetMD5(Encoding.UTF8.GetBytes(data)));
        }
        internal static string bytesToString(byte[] data)
        {
            if (data == null)
                return "";
            else
            {
                StringBuilder s = new StringBuilder(data.Length * 2);
                for (int i = 0; i < data.Length; i++)
                    s.Append(data[i].ToString("X2"));
                return s.ToString();
            }
        }
        internal static string GetBufString()
        {
            StringBuilder sb = new StringBuilder();
            var r = rBuf;
            if (rBuf == null)
                return null;
            for (int i = 0; i < r.Length; i++)
            {
                sb.Append(r[i].Count);
                if (MtdBlocks[i].imgQ.Count > 0)
                    sb.Append("_").Append(MtdBlocks[i].imgQ.Count);
                sb.Append(",");
            }
            if (sb.Length > 0)
            {
                sb.Remove(sb.Length - 1, 1);
                return $"{sb}";
            }
            return null;
        }
        internal static string GetHisString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("总合格/不合格:").Append(PassCount).Append("/").Append(FailCount);
            for (int i = 0; i < MtdBlocks.Length; i++)
                sb.Append(", 相机").Append(i + 1).Append(":").Append(MtdBlocks[i].PassCount).Append("/").Append(MtdBlocks[i].FailCount);
            return sb.ToString();
        }

        internal static void ExportMtds()
        {
            if (MtdBlocks != null)
                ExcelHdl.SaveMtds(MtdBlocks);
        }
        internal static void ExportMtds(string file)
        {
            if (MtdBlocks != null)
                ExcelHdl.SaveMtds(file);
        }

        internal class RunInfo
        {
            internal ICogImage img;
            internal int id;

            public RunInfo(ICogImage img, int id)
            {
                this.img = img;
                this.id = id;
            }
        }
    }
}
