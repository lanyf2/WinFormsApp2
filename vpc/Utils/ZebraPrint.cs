using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neodynamic.SDK.Printing;
using System.Threading;
using System.IO;

namespace vpc
{
    public class BarTenderPrint
    {
        static BarTender.Application barApplication;
        static Dictionary<char, ushort> codeDFXKDic = new Dictionary<char, ushort>();
        internal static BarTender.Format barFormat;
        static BarTenderPrint()
        {
            try
            {
                for (ushort i = 1; i < 10; i++)
                    codeDFXKDic.Add((char)('0' + i), i);
                for (ushort i = 10; i <= 17; i++)
                    codeDFXKDic.Add((char)('A' + i - 10), i);
                for (ushort i = 18; i <= 22; i++)
                    codeDFXKDic.Add((char)('J' + i - 18), i);
                codeDFXKDic.Add('P', 23);
                codeDFXKDic.Add('R', 24);
                codeDFXKDic.Add('S', 25);
                codeDFXKDic.Add('T', 26);
                for (ushort i = 27; i <= 31; i++)
                    codeDFXKDic.Add((char)('V' + i - 27), i);

                //barApplication = new BarTender.Application();
                //barFormat = barApplication.Formats.Open(Environment.CurrentDirectory + "\\PrintB.btw");
                //barFormat.IdenticalCopiesOfLabel = 1;
            }
            catch (Exception ex)
            {
                Program.ErrHdl(ex);
            }
        }
        HashSet<string> Substrings = new HashSet<string>();
        internal void Print(Cognex.VisionPro.ResultStruct re)
        {//string VCF, string 零部件名称, string 零件号, string 软件版本, string 硬件版本号, string MTC, 
            if (barFormat != null)
            {
                try
                {
                    if (Cognex.VisionPro.JobManager.AutoSerial(re) == false)
                        return;
                    string[] extras = re.ExtraInfo;
                    int? serNum = null;
                    if (extras != null)
                        for (int i = 0; i < extras.Length / 2; i++)
                        {
                            if (string.IsNullOrEmpty(extras[i * 2]) == false)
                            {
                                if (Substrings.Contains(extras[i * 2]))
                                {
                                    if (extras[i * 2] == "序列号")
                                    {
                                        int sertmp;
                                        if (int.TryParse(extras[i * 2 + 1], out sertmp) == false)
                                        {
                                            Program.ErrHdl("序列号非数字：" + extras[i * 2 + 1]);
                                            return;
                                        }
                                        else if (sertmp <= 0)
                                        {
                                            Program.ErrHdl("序列号错误：" + extras[i * 2 + 1]);
                                            return;
                                        }
                                        serNum = sertmp;
                                    }
                                    string sval = barFormat.GetNamedSubStringValue(extras[i * 2]);
                                    string str = extras[i * 2 + 1];
                                    if (str.Length != sval.Length || str.IndexOf('\x0') >= 0)
                                    {
                                        Program.ErrHdl("打印内容与模版不匹配：\r\n{0}\r\n{1}\r\n{2}".FormatWith(sval, str.ToHexString(), str));
                                        return;
                                    }
                                    barFormat.SetNamedSubStringValue(extras[i * 2], str);
                                    if (serNum == null && extras[i * 2] == "MTC")
                                        serNum = int.Parse(extras[i * 2 + 1].Substring(extras[i * 2 + 1].Length - 1, 1));
                                }
                                try
                                {
                                    if (extras[i * 2] == "日期")
                                    {
                                        DateTime t;
                                        if (DateTime.TryParseExact(extras[i * 2 + 1], "yyyyMMdd", new System.Globalization.CultureInfo("zh-CN"), System.Globalization.DateTimeStyles.None, out t))
                                            SetDate(t);
                                        else
                                        {
                                            Program.ErrHdl(new Exception("日期解析错误：" + extras[i * 2 + 1]));
                                            return;
                                        }
                                    }
                                    else if (extras[i * 2] == "日期6")
                                    {
                                        DateTime t;
                                        if (DateTime.TryParseExact(extras[i * 2 + 1], "yyMMdd", new System.Globalization.CultureInfo("zh-CN"), System.Globalization.DateTimeStyles.None, out t))
                                            SetDate(t);
                                        else
                                        {
                                            Program.ErrHdl(new Exception("日期解析错误：" + extras[i * 2 + 1]));
                                            return;
                                        }
                                    }
                                    else if (extras[i * 2] == "日期5")
                                    {
                                        DateTime t;
                                        int val;
                                        if (int.TryParse(extras[i * 2 + 1], out val))
                                        {
                                            t = new DateTime(2000 + val / 1000, 1, 1);
                                            t = t.AddDays(val % 1000 - 1);
                                            SetDate(t);
                                        }
                                        else
                                        {
                                            Program.ErrHdl(new Exception("日期解析错误：" + extras[i * 2 + 1]));
                                            return;
                                        }
                                    }
                                    else if (extras[i * 2] == "日期DFXK")
                                    {
                                        DateTime t;
                                        ushort y, m, d;
                                        if (extras[i * 2 + 1] == null || extras[i * 2 + 1].Length != 3 || codeDFXKDic.TryGetValue(extras[i * 2 + 1][0], out y) == false || codeDFXKDic.TryGetValue(extras[i * 2 + 1][1], out m) == false || codeDFXKDic.TryGetValue(extras[i * 2 + 1][2], out d) == false)
                                        {
                                            Program.ErrHdl(new Exception("日期解析错误：" + extras[i * 2 + 1]));
                                            return;
                                        }
                                        t = new DateTime(2000 + y, m, d);
                                        SetDate(t);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Program.ErrHdl(ex);
                                    return;
                                }
                            }
                        }
                }
                catch (Exception ex)
                {
                    Program.ErrHdl(ex);
                    return;
                }
                string snum = SerialNum;
                if (re.ExtraInfo == null && snum.IsNullOrEmpty() == false)
                    re.ExtraInfo = new string[] { "sn", snum };
                barFormat.PrintOut(true);
                if (string.IsNullOrEmpty(snum) == false && Settings.Default.PrintSerialNumMem != snum)
                {
                    Settings.Default.PrintSerialNumMem = snum;
                    //Settings.Default.Save();
                }
            }
            else
                Program.MsgBox("未加载样式文件");
        }
        string AssertValidString(string input)
        {
            if (input != null && input.Length > 0 && false == DataView.IsValidChar(input[0]))
            {
                StringBuilder sb = new StringBuilder();
                bool x4flag = false;
                for (int i = 0; i < input.Length; i++)
                    if (input[i] >= 256)
                    {
                        x4flag = true;
                        break;
                    }
                for (int i = 0; i < input.Length; i++)
                    if (x4flag)
                        sb.Append(((int)input[i]).ToString("X4"));
                    else
                        sb.Append(((int)input[i]).ToString("X2"));
                return sb.ToString();
            }
            return input;
        }
        internal void LoadBtw(string path)
        {
            string PrevSerial = null;
            if (barApplication == null)
                barApplication = new BarTender.Application();
            else
                PrevSerial = SerialNum;

            string path2 = path;
            if (File.Exists(path2) == false)
                path2 = path + ".btw";
            if (File.Exists(path2) == false)
                path2 = path + ".BTW";

            if (File.Exists(path2))
            {
                if (barFormat != null)
                    barFormat.Close(BarTender.BtSaveOptions.btDoNotSaveChanges);
                barFormat = null;
                barFormat = barApplication.Formats.Open(Path.GetFullPath(path2));
                barFormat.NumberSerializedLabels = 1;
                if (Settings.Default.PrintCopies <= 1)
                    barFormat.IdenticalCopiesOfLabel = 1; // set copies
                else
                    barFormat.IdenticalCopiesOfLabel = Settings.Default.PrintCopies; // set copies

                Substrings.Clear();
                var enu = barFormat.NamedSubStrings.GetAll(":", ",");
                var spi = enu.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < spi.Length; i++)
                {
                    int index = spi[i].IndexOf(':');
                    if (index > 0)
                        Substrings.Add(spi[i].Substring(0, index));
                }

                if ((DateTime.Now - Settings.Default.PrintSerialNumMemTime).TotalDays < 1
               && Settings.Default.PrintSerialNumMemTime.Day == DateTime.Now.Day)
                {
                    if (string.IsNullOrEmpty(PrevSerial))
                        PrevSerial = Settings.Default.PrintSerialNumMem;
                }
                else if (string.IsNullOrEmpty(SerialNum) == false)
                {
                    StringBuilder sb = new StringBuilder(SerialNum);
                    for (int i = 0; i < sb.Length - 1; i++)
                        sb[i] = '0';
                    sb[sb.Length - 1] = '1';
                    PrevSerial = sb.ToString();
                }
                if (string.IsNullOrEmpty(PrevSerial) == false && SerialNum != null)
                {
                    if (PrevSerial.Length == SerialNum.Length)
                        SerialNum = PrevSerial;
                    else if (PrevSerial.Length > SerialNum.Length)
                        SerialNum = PrevSerial.Substring(0, SerialNum.Length);
                    else
                    {
                        SerialNum = PrevSerial.PadLeft(SerialNum.Length, '0');
                    }
                }
                Settings.Default.PrintSerialNumMemTime = DateTime.Now;
                //object str = null;
                //StringBuilder sb = new StringBuilder();
                //var en = barFormat.NamedSubStrings.GetEnumerator();
                //while (en.MoveNext())
                //{
                //    str = en.Current;
                //    sb.Append(str).Append("\r\n");
                //    str = null;
                //}
                //Program.MsgBox(sb.ToString());
            }
            else
                Program.MsgBox("样式文件 \"" + path + "\" 不存在");
        }
        internal void SetCopies(int copies)
        {
            if (copies > 0 && barFormat != null)
                barFormat.IdenticalCopiesOfLabel = copies; // set copies
        }
        internal string SerialNum
        {
            get
            {
                try
                {
                    if (barFormat == null)
                        return null;
                    return barFormat.GetNamedSubStringValue("序列号");
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                try
                {
                    int val;
                    if (int.TryParse(value, out val) && val >= 0)
                    {
                        string format = barFormat.GetNamedSubStringValue("序列号");
                        if (value.Length == format.Length)
                            barFormat.SetNamedSubStringValue("序列号", value);
                        else if (value.Length > format.Length)
                            barFormat.SetNamedSubStringValue("序列号", value.Substring(value.Length - format.Length));
                        else
                        {
                            barFormat.SetNamedSubStringValue("序列号", value.PadLeft(format.Length, '0'));
                        }
                    }
                }
                catch
                {
                }
            }
        }
        void SetDate(DateTime t)
        {
            if (Substrings.Contains("年"))
                barFormat.SetNamedSubStringValue("年", t.ToString("yyyy"));
            if (Substrings.Contains("月"))
                barFormat.SetNamedSubStringValue("月", t.ToString("MM"));
            if (Substrings.Contains("日"))
                barFormat.SetNamedSubStringValue("日", t.ToString("dd"));
            if (Substrings.Contains("DayOfYear"))
                barFormat.SetNamedSubStringValue("DayOfYear", t.DayOfYear.ToString("000"));
            if (Substrings.Contains("年2"))
                barFormat.SetNamedSubStringValue("年2", t.ToString("yy"));
        }
        internal virtual string GetSubString(string subName)
        {
            if (barFormat != null)
                try
                { return barFormat.GetNamedSubStringValue(subName); }
                catch { }
            return null;
        }
    }
    internal class ZebraPrint
    {
        static PrintJob pj;
        static PrinterSettings _printerSettings;
        static ZebraPrint()
        {
            pj = new PrintJob();
            _printerSettings = new PrinterSettings();

            //_printerSettings.Communication.CommunicationType = CommunicationType.Serial;
            //_printerSettings.Communication.SerialPortName = "COM2";
            //_printerSettings.Communication.SerialPortBaudRate = 9600;

            var strs = System.Drawing.Printing.PrinterSettings.InstalledPrinters;
            if (string.IsNullOrEmpty(Settings.Default.PrinterName) == false)
                _printerSettings.PrinterName = Settings.Default.PrinterName;
            else
                _printerSettings.PrinterName = strs[0];
            _printerSettings.Communication.CommunicationType = CommunicationType.USB;
            //_printerSettings.Communication.SerialPortDataBits = int.Parse(this.txtDataBits.Text);
            //_printerSettings.Communication.SerialPortFlowControl = (System.IO.Ports.Handshake)Enum.Parse(typeof(System.IO.Ports.Handshake), this.cboFlowControl.SelectedItem.ToString());
            //_printerSettings.Communication.SerialPortParity = (System.IO.Ports.Parity)Enum.Parse(typeof(System.IO.Ports.Parity), this.cboParity.SelectedItem.ToString());
            //_printerSettings.Communication.SerialPortStopBits = (System.IO.Ports.StopBits)Enum.Parse(typeof(System.IO.Ports.StopBits), this.cboStopBits.SelectedItem.ToString());

            pj.PrinterSettings = _printerSettings;
            pj.PrintOrientation = PrintOrientation.Landscape90;
            System.Threading.ThreadPool.QueueUserWorkItem(Run);
        }
        static System.Collections.Concurrent.ConcurrentQueue<ThermalLabel> q = new System.Collections.Concurrent.ConcurrentQueue<ThermalLabel>();
        static AutoResetEvent wait = new AutoResetEvent(false);
        internal static void printAsync(string 零件名称, string 零件型号, string qrcode)
        {
            ThermalLabel lbl = new ThermalLabel();
            lbl.LoadXmlTemplate(System.IO.File.ReadAllText("aa.tl"));
            if (lbl.Items["qr"] != null)
                ((BarcodeItem)lbl.Items["qr"]).Code = qrcode;
            if (lbl.Items["text1"] != null)
                ((TextItem)lbl.Items["text1"]).Text = string.Format("零件名称\r\n{0}\r\n零件型号\r\n{1}\r\n生产日期\r\n{2:yyyyMMdd}", 零件名称, 零件型号, DateTime.Now);
            if (lbl.Items["text2"] != null)
                ((TextItem)lbl.Items["text2"]).Text = qrcode;
            q.Enqueue(lbl);
            wait.Set();
        }
        static void Run(object state)
        {
            while (true)
            {
                try
                {
                    ThermalLabel lbl;
                    if (q.TryDequeue(out lbl))
                    {
                        if (Settings.Default.打印到文件)
                            pj.ExportToImage(lbl, "test.jpg", new ImageSettings(ImageFormat.Jpeg), pj.PrinterSettings.Dpi);
                        else
                        {
                            using (PrintJob pjb = new PrintJob(_printerSettings))
                            {
                                if (Settings.Default.PrintCopies <= 1)
                                    pjb.Copies = 1; // set copies
                                else
                                    pjb.Copies = Settings.Default.PrintCopies; // set copies
                                pjb.PrintOrientation = PrintOrientation.Landscape90; //set orientation
                                pjb.ThermalLabel = lbl; // set the ThermalLabel object
                                pjb.Print(); // print the ThermalLabel object    
                                pjb.BufferFlush();
                            }
                        }
                    }
                    else
                        wait.WaitOne(3000);
                }
                catch (Exception ex)
                {
                    Program.ErrHdl(ex);
                }
            }
        }
        static void print(string 零件名称, string 零件型号, string qrcode)
        {
            //ThermalLabel lbl = new ThermalLabel(UnitType.Inch, 4, 3);
            //lbl.GapLength = 1;
            //TextItem t1 = new TextItem(0.2, 0.2, 2.5, 0.5, 零件名称);
            //BarcodeItem b1 = new BarcodeItem(0.2, 1, 5, 5, BarcodeSymbology.QRCode, qrcode + qrcode + qrcode);
            //lbl.Items.Add(t1);
            //lbl.Items.Add(b1);
            ThermalLabel lbl = new ThermalLabel();
            lbl.LoadXmlTemplate(System.IO.File.ReadAllText("aa.tl"));
            if (lbl.Items["qr"] != null)
                ((BarcodeItem)lbl.Items["qr"]).Code = qrcode;
            if (lbl.Items["text1"] != null)
                ((TextItem)lbl.Items["text1"]).Text = string.Format("零件名称\r\n{0}\r\n零件型号\r\n{1}\r\n生产日期\r\n{2:yyyyMMdd}", 零件名称, 零件型号, DateTime.Now);
            if (lbl.Items["text2"] != null)
                ((TextItem)lbl.Items["text2"]).Text = qrcode;
            pj.ThermalLabel = lbl;
            if (Settings.Default.打印到文件)
                pj.ExportToImage(qrcode + ".jpg", new ImageSettings(ImageFormat.Jpeg), pj.PrinterSettings.Dpi);
            else
                pj.Print();
        }
    }
}
