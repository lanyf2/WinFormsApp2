using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;
using Dapper;
using System.Threading;

namespace vpc
{
    internal class DbHdl
    {
        internal bool ReCheckFlag = true;
        internal static IDbConnection idb;
        string y用户图号;
        string b班次;
        string pcb条码;
        string p批次号;
        string g工位号 = "光强检测";
        string c操作人 = "光强检测";
        internal string mtcFmt;
        string BuildSQLConnectionString(System.Net.IPAddress ip, string InitialCatalog, string user, string password)
        {
            if (user.IsNullOrEmpty() || password.IsNullOrEmpty())
            {
                user = InitialCatalog;
                password = InitialCatalog;
            }
            if (ip == null)
                return @"Data Source = (local);Initial Catalog ={0};Integrated Security = SSPI; Connect Timeout = 3".FormatWith(InitialCatalog);
            else
                return "Data Source = {0}; Initial Catalog = {3}; Persist Security Info = True; User ID = {1}; Password = {2}; Connect Timeout=3".FormatWith(ip, user, password, InitialCatalog);
        }
        internal bool pcbScanMode = false;
        public DbHdl(string InitStr)
        {
            //ip;Initial Catalog;工位号;userid;password
            if (InitStr.StartsWith("PCB"))
            {
                pcbScanMode = true;
                InitStr = InitStr.Substring(3);
                SelectCommandFromChanPingXinXi = "SELECT * FROM 产品信息 WHERE [PCBA码] = @mtc AND [用户图号]= @pn AND [有效标识] = 0";
            }
            var re = InitStr.Split(new char[] { ';' });
            System.Net.IPAddress ip;
            string password = null, userid = null;
            if (re[0] == "local")
            {
                ip = null;
            }
            else if (System.Net.IPAddress.TryParse(re[0], out ip))
            {
            }
            else
            {
                throw new InvalidExpressionException("IP无法解析");
            }
            if (re.Length >= 3)
            {
                g工位号 = re[2];
                if (pcbScanMode)
                {
                    UpdateCommandChanPingXinXi = "UPDATE 产品信息 SET [{0}标识] = @val WHERE [PCBA码] = @mtc AND [有效标识] = 0".FormatWith(g工位号);
                }
                else
                {
                    UpdateCommandChanPingXinXi = "UPDATE 产品信息 SET [{0}标识] = @val WHERE MTC = @mtc AND [有效标识] = 0".FormatWith(g工位号);
                    InsertCommandIntoChanPingXinXi = "INSERT INTO 产品信息 (用户图号,批次号,追溯码,MTC, PCBA码, 操作时间, [{0}标识],上传标识) VALUES (@用户图号,@picihao, @MTC, @MTC, @PCBA码,GETDATE(),@flag80,'1')".FormatWith(g工位号);
                }
            }
            if (re.Length >= 6)
            {
                mtcFmt = re[5];
            }
            if (re.Length >= 5)
            {
                password = re[4];
                userid = re[3];
            }
            else if (re.Length > 1)
            {
                password = re[1];
                userid = re[1];
            }
            if (re.Length > 1)
            {
                var str = BuildSQLConnectionString(ip, re[1], userid, password);
                idb = new System.Data.SqlClient.SqlConnection(str);
                System.Threading.ThreadPool.QueueUserWorkItem(Run);
            }
        }
        System.Collections.Concurrent.ConcurrentQueue<Cognex.VisionPro.ResultStruct> runq = new System.Collections.Concurrent.ConcurrentQueue<Cognex.VisionPro.ResultStruct>();
        AutoResetEvent wait = new AutoResetEvent(false);
        void Run(object state)
        {
            while (true)
            {
                try
                {
                    if (runq.IsEmpty)
                    {
                        wait.WaitOne(3000);
                        continue;
                    }
                    Cognex.VisionPro.ResultStruct re;
                    if (runq.TryPeek(out re))
                    {
                        WriteSync(re);

                        runq.TryDequeue(out re);
                    }
                }
                catch (Exception ex)
                {
                    Program.ErrHdl(ex);
                    try
                    {
                        Cognex.VisionPro.ResultStruct re;
                        if (System.Windows.Forms.MessageBox.Show("保存到数据库失败，是否重试？", "", System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                            runq.TryDequeue(out re);
                    }
                    catch
                    { }
                    Thread.Sleep(2000);
                }
                Thread.Sleep(2000);
            }
        }
        internal bool TryWriteSync(Cognex.VisionPro.ResultStruct re)
        {
            while (true)
            {
                try
                {
                    WriteSync(re);
                    return true;
                }
                catch (Exception ex)
                {
                    Program.ErrHdl(ex);
                    try
                    {
                        if (System.Windows.Forms.MessageBox.Show("保存到数据库失败，是否重试？", "", System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                            return false;
                    }
                    catch
                    { }
                    Thread.Sleep(2000);
                }
            }
        }
        internal void WriteSync(Cognex.VisionPro.ResultStruct re)
        {
            string mtcr = null;
            string mypn = null;
            if (re.ExtraInfo != null)
                if (re.ExtraInfo.Length >= 2 && re.ExtraInfo[0] == "MTC")
                {
                    mtcr = re.ExtraInfo[1];
                    if (re.ExtraInfo.Length >= 4 && re.ExtraInfo[2] == "零件号")
                        mypn = re.ExtraInfo[3];
                }
                else
                    for (int i = 1; i < re.ExtraInfo.Length; i += 2)
                    {
                        if (re.ExtraInfo[i - 1] == "日期6")
                        {
                            int y = 2000 + int.Parse(re.ExtraInfo[i].Substring(0, 2));
                            int m = int.Parse(re.ExtraInfo[i].Substring(2, 2));
                            int d = int.Parse(re.ExtraInfo[i].Substring(4, 2));
                            DateTime dt = new DateTime(y, m, d);
                            if (mtcFmt.IsNullOrEmpty())
                                mtcr += re.ExtraInfo[i].Substring(0, 2) + dt.DayOfYear.ToString("000");
                            else
                                mtcr += mtcFmt.FormatWith(dt, dt.DayOfYear);
                        }
                        else if (re.ExtraInfo[i - 1] == "零件号")
                            mypn = re.ExtraInfo[i];
                        else
                            mtcr += re.ExtraInfo[i];
                    }
            if (pcbScanMode && string.IsNullOrEmpty(mtcr))
                throw new Exception("缺少条码信息");

            bool createNew = false;
            if (mtcr == null)
            {
                GetSCRW();
                createNew = true;
                if (re.Passed)
                    mtcr = CheckSerialNum(mtcFmt);
            }
            else
            {
                if (GetInfos(mtcr, mypn) == false)
                {
                    if (pcbScanMode)
                        throw new Exception("数据库无法找到记录：" + mtcr);
                    createNew = true;
                    GetSCRW();
                }
                else if (pcbScanMode)
                {
                    if (string.IsNullOrEmpty(pcb条码))
                        throw new Exception("数据库查询MTC为空：" + mtcr);
                }
                CheckInfo();
            }

            c操作人 = Settings.Default.标题 + "_" + System.IO.Path.GetFileNameWithoutExtension(Settings.Default.jobs);

            var ire = idb.Execute(InsertCommandJianCeShuJu, new { tuhao = y用户图号, mtc = pcbScanMode ? pcb条码 : mtcr, gongwei = g工位号, caozuoren = c操作人, time = DateTime.Now, jieguo = re.Passed ? "2" : "3", youxiao = "0", pici = p批次号, shangchuan = "1" });
            if (ire <= 0)
            {
                throw new Exception("数据库写入错误：" + mtcr);
            }
            else if (mtcr != null)
            {
                if (createNew)
                {
                    int rer = idb.Execute(InsertCommandIntoChanPingXinXi, new { 用户图号 = y用户图号, picihao = p批次号, MTC = mtcr, PCBA码 = pcb条码, flag80 = re.Passed ? "2" : "3" });
                    if (rer == 0)
                        throw new Exception("内部错误25865：写入产品信息失败");
                }
                else if (UpdateCommandChanPingXinXi.IsNullOrEmpty() == false)
                {
                    ire = idb.Execute(UpdateCommandChanPingXinXi, new { val = re.Passed ? "2" : "3", mtc = mtcr });
                    if (ire > 1)
                        throw new Exception("内部错误：{0}_更新产品信息返回值大于1_{1}".FormatWith(mtcr, ire));
                    else if (ire == 0)
                        throw new Exception("{0}_无法更新产品信息".FormatWith(mtcr, ire));
                }
                else
                    throw new Exception("UpdateCommandChanPingXinXi is null");
            }
        }
        static string InsertCommandIntoChanPingXinXi;
        string SelectCommandFromSCRW = "SELECT * FROM 生产任务 WHERE 结束时间 is null order by ord desc";
        static string SelectCountCommandFromChanPingXinXi = "SELECT COUNT(*) FROM 产品信息 WHERE MTC = @mtc";
        string SelectCommandFromChanPingXinXi = "SELECT * FROM 产品信息 WHERE MTC = @mtc AND [有效标识] = 0";
        string InsertCommandJianCeShuJu = "INSERT INTO 检测数据 ([用户图号],[MTC],[工位号],[操作人],[操作时间],[结果],[有效标识],[批次号],[上传标识]) VALUES (@tuhao,@mtc, @gongwei,@caozuoren, @time,@jieguo, @youxiao, @pici, @shangchuan)";
        string UpdateCommandChanPingXinXi = null;
        bool GetInfos(string mymtc, string mypn)
        {
            var re0 = idb.Query(SelectCommandFromChanPingXinXi, new { mtc = mymtc, pn = mypn }).ToArray();
            if (re0 != null && re0.Length > 0)
            {
                p批次号 = re0[0].批次号;
                y用户图号 = re0[0].用户图号;
                b班次 = re0[0].班次代号;
                if (pcbScanMode)
                    pcb条码 = re0[0].MTC;
                else
                    pcb条码 = re0[0].PCBA码;
                return true;
            }
            return false;
        }
        void GetSCRW()
        {
            var re = idb.Query(SelectCommandFromSCRW).ToArray();
            if (re == null || re.Length == 0)
                throw new Exception("无法获取生产任务");
            p批次号 = re[0].批次号;
            y用户图号 = re[0].用户图号;
            b班次 = re[0].班次代号;
            pcb条码 = re[0].条码编号;
        }
        internal void WriteAsync(Cognex.VisionPro.ResultStruct re)
        {
            if (pcbScanMode == false)
            {
                runq.Enqueue(re);
                wait.Set();
            }
        }
        internal string CheckSerialNum(string fmt)
        {
            uint sr;
            if (fmt.IsNullOrEmpty())
                fmt = "{5:yyyyMMdd}{3}";
            if (Settings.Default.PrintSerialNum.IsNullOrEmpty() == false && uint.TryParse(Settings.Default.PrintSerialNum, out sr))
            {
                if (sr < 1)
                    sr = 1;
                var sriFmt = "0".PadLeft(Settings.Default.PrintSerialNum.Length, '0');

                CheckInfo();

                int maxSr = (int)Math.Pow(10, Settings.Default.PrintSerialNum.Length);
                for (uint i = sr; i < maxSr; i++)
                {
                    var mtcs = fmt.FormatWith(b班次, DateTime.Now.ToString("yy")
                        , DateTime.Now.DayOfYear.ToString("000"), Settings.Default.PrintSerialNum
                        , y用户图号, DateTime.Now);
                    var re = idb.ExecuteScalar<int>(SelectCountCommandFromChanPingXinXi, new { mtc = mtcs });
                    if (re == 0)
                        return mtcs;
                    else
                    {
                        Settings.Default.PrintSerialNum = (i + 1).ToString(sriFmt);
                    }
                }
                throw new Exception("无法生成可用追溯码");
            }
            else
                throw new Exception("标签模版未包含序列号信息");
        }
        internal bool CheckInfo()
        {
            var bcdh = Cognex.VisionPro.JobManager.barTenderPrint.GetSubString("班次代号");
            if (bcdh.IsNullOrEmpty() == false && bcdh != b班次)
                throw new Exception("班次代号与服务器设定不匹配：{0} != {1}".FormatWith(bcdh, b班次));
            var ljh = Cognex.VisionPro.JobManager.barTenderPrint.GetSubString("零件号前四位") + Cognex.VisionPro.JobManager.barTenderPrint.GetSubString("零件号后四位");
            if (string.IsNullOrEmpty(ljh))
                ljh = Cognex.VisionPro.JobManager.barTenderPrint.GetSubString("零件号");
            if (ljh.IsNullOrEmpty() == false && ljh != y用户图号)
                throw new Exception("零件号与服务器设定不匹配：{0} != {1}".FormatWith(ljh, y用户图号));
            return true;
        }
    }
    internal class DbTrace
    {
        IDbConnection idb;
        string SelectCommandFilterByTime = "SELECT * FROM LogTbl WHERE time>@time1 AND time<@time2";
        string SelectCommandFilterByMTC = "SELECT * FROM LogTbl WHERE mtc = @mtcfilter";
        string LogTblInsertCommand = "SELECT * FROM LogTbl WHERE mtc = @mtcfilter";
        internal DbTrace(string conStr, DBType dbType = DBType.SQLite)
        {
            switch (dbType)
            {
                case DBType.SQLite:
                    idb = new System.Data.SQLite.SQLiteConnection(conStr);
                    //"Data Source=Database.sqlite;Pooling=true;FailIfMissing=false"
                    break;
                default:
                    throw new NotImplementedException("未实现的数据库接口：" + dbType);
            }
        }
        internal bool SaveResult(ResultInfo ri)
        {
            if (ri != null && ri.MTC.IsNullOrEmpty() == false)
            {
                LogTblInfo lti = ri.ToLogTblInfo();
                int re = idb.Execute(LogTblInsertCommand, lti);
                if (re <= 0)
                    Program.ErrHdl("Err3854:re<=0");
                else
                    return true;
            }
            return false;
        }
        internal ResultInfo[] GetResultDetail(string MTC)
        {
            var re = idb.Query<LogTblInfo>(SelectCommandFilterByMTC, new { mtcfilter = MTC });
            return re.Select(new Func<LogTblInfo, ResultInfo>((LogTblInfo jd) => jd.ToResultInfo())).ToArray();
        }
        internal ResultInfo[] GetResults(DateTime tm0, DateTime tm1)
        {
            var re = idb.Query<LogTblInfo>(SelectCommandFilterByTime, new { time1 = tm0, time2 = tm1 }).ToArray();
            Dictionary<string, ResultInfo> dic = new Dictionary<string, ResultInfo>();
            for (int i = 0; i < re.Length; i++)
            {
                if (re[i].mtc != null)
                {
                    var v = dic.TryGetValue(re[i].mtc);
                    if (v == null)
                        dic.Add(re[i].mtc, re[i].ToResultInfo());
                    else
                        v.AddLogTblInfo(re[i]);
                }
            }
            return dic.Select(new Func<KeyValuePair<string, ResultInfo>, ResultInfo>((KeyValuePair<string, ResultInfo> jd) => jd.Value)).ToArray();
        }
    }
    public class LogTblInfo
    {
        public string mtc;
        public string info;
        public DateTime datetime;
        public ResultInfo ToResultInfo()
        {
            throw new NotImplementedException();
            //return null;
        }
    }
    public class ResultInfo
    {
        public const char FailFlag = '♂';
        public Dictionary<string, string> data;
        public DateTime datetime;
        public string MTC;
        public string GetDbInfoString()
        {
            throw new NotImplementedException();
        }
        public bool AddLogTblInfo(LogTblInfo lti)
        {
            if (lti != null && lti.mtc == MTC)
            {
                if (lti.datetime > datetime)
                    datetime = lti.datetime;
                throw new NotImplementedException();
                //return true;
            }
            return false;
        }
        public LogTblInfo ToLogTblInfo()
        {
            throw new NotImplementedException();
            //return null;
        }
    }
    public enum DBType
    {
        SQLite, SQL, MySql
    }
}
