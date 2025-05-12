using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Cognex.VisionPro;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using System.Xml;

namespace vpc
{
    [Serializable]
    public class Settings : ICloneable
    {
        #region Default
        public static Settings Default = new Settings();
        public const string SettingsFilePath = @"\settings.xml";
        public const string SettingsBackupFilePath = @"\settings_backup.xml";
        public static string SystemDIR
        {
            get { return System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().ManifestModule.FullyQualifiedName); }// 系统目录名
        }
        public void Save(bool BackUp = false)
        {
            string ProfilePath = SystemDIR + SettingsFilePath;
            string ProfilePath2 = SystemDIR + SettingsBackupFilePath;
            XmlSerializer ser = new XmlSerializer(typeof(Settings));
            {
                if (BackUp && File.Exists(ProfilePath))
                {
                    File.Copy(ProfilePath, ProfilePath2, true);
                    System.Threading.Thread.Sleep(400);
                }
                TextWriter writer = new StreamWriter(ProfilePath, false);
                ser.Serialize(writer, this);
                writer.Close();
            }
        }
        static Settings Load()
        {
            string ProfilePath = SystemDIR + SettingsFilePath;
            if (File.Exists(ProfilePath))
            {
                FileStream fs = new FileStream(ProfilePath, FileMode.Open);
                if (fs != null)
                {
                    XmlSerializer ser = new XmlSerializer(typeof(Settings));
                    try
                    {
                        XmlTextReader reader = new XmlTextReader(fs);
                        Settings obj = (Settings)(ser.Deserialize(reader));
                        reader.Close();
                        if (obj != null)
                            return obj;
                    }
                    catch (Exception ex)
                    {
                        Program.ErrHdl(ex.Message);
                        fs.Close();
                    }
                }
            }
            string ProfilePath2 = SystemDIR + SettingsBackupFilePath;
            if (File.Exists(ProfilePath2))
            {
                File.Copy(ProfilePath2, ProfilePath, true);
                FileStream fs = new FileStream(ProfilePath2, FileMode.Open);
                if (fs != null)
                {
                    XmlSerializer ser = new XmlSerializer(typeof(Settings));
                    try
                    {
                        XmlTextReader reader = new XmlTextReader(fs);
                        Settings obj = (Settings)(ser.Deserialize(reader));
                        reader.Close();
                        if (obj != null)
                            return obj;
                    }
                    catch (Exception ex)
                    {
                        Program.ErrHdl(ex);
                        fs.Close();
                    }
                }
            }
            Settings st = new Settings();
            //st.EnableUpdateImage = true;
            //st.MinInterfaceUpdateInterval = 70;
            //st.LogAll = true;
            //st.SaveToFile = SaveType.NotSave;
            //st.SavePath = "Img";
            //st.ResultBufferCapacity = 100;
            st.Save();
            return st;
        }
        public static void LoadToDefault()
        {
            Default = Load();
            //Default.DebugMode = false;
            if (Default.StepCount < 1)
                Default.StepCount = 1;
            if (Default.Cams == null || Default.StepCount > Default.Cams.Length)
                Default.Cams = new string[Default.StepCount];
        }
        public object Clone()
        {
            Settings st = new Settings();
            Type tp = typeof(Settings);
            foreach (PropertyInfo pi in tp.GetProperties())
            {
                if (pi.CanWrite)
                {
                    pi.SetValue(st, pi.GetValue(this, null), null);
                }
            }
            //             st.MinInterfaceUpdateInterval = MinInterfaceUpdateInterval;
            //             st.EnableUpdateImage = EnableUpdateImage;
            //             st.LogAll = LogAll;
            //             st.SaveToFile = SaveToFile;
            //             st.SavePath = SavePath;
            //             st.ResultBufferCapacity = ResultBufferCapacity;
            //             st.DebugMode = DebugMode;
            return st;
        }
        #endregion
        [DisplayName("保存图像"), TypeConverter(typeof(MyEnumConverter))]
        public SaveType SaveToFile { get; set; }
        [DisplayName("仅保存最后一张图像")]
        internal bool SaveLastImg { get; set; }
        [DisplayName("隐藏结果图形"), TypeConverter(typeof(MyEnumConverter))]
        public bool HideGraphics { get; set; }
        [DisplayName("图像缓存队列长度")]
        public int ResultBufferCapacity { get; set; }
        //[DisplayName("调试模式"), TypeConverter(typeof(MyEnumConverter))]
        //internal bool DebugMode { get; set; }
        [DisplayName("PLC通讯串口")]
        public string PLCPort { get; set; }
        [DisplayName("打印机名字")]
        internal string PrinterName { get; set; }
        [DisplayName("选择配方"), Editor(typeof(FilePathToolEditor), typeof(System.Drawing.Design.UITypeEditor)), Description("配方文件|*.vpp"), Category("jobs")]
        public string jobs { get; set; }
        internal bool 打印到文件 { get; set; }
        [DisplayName("打印启用")]
        internal bool 打印启用 { get; set; }
        [DisplayName("强制全检")]
        internal bool 强制全检 { get; set; }
        [DisplayName("标题")]
        public string 标题 { get; set; }
        [DisplayName("待检产品缓存"), XmlIgnore]
        public string re { get { return JobManager.GetBufString(); } }

        [DisplayName("打印样式文件")]
        internal string 打印样式文件 { get; set; }
        [DisplayName("初始化写入")]
        internal ushort[] 初始化写入 { get; set; }
        [DisplayName("初始化写入2")]
        internal string[] 初始化写入2 { get; set; }
        [DisplayName("初始化写入2地址")]
        internal ushort[] 初始化写入2地址 { get; set; }
        [DisplayName("相机数"), Browsable(false)]
        public int StepCount { get; set; }
        //public string[] ExtraInfo { get; set; }
        //public ushort[] ExtraInfoAddr { get; set; }
        //public string[] ExtraInfoFormate { get; set; }
        //public double[] ExtraInfoRate { get; set; }
        internal ExtraInfoStruct[] ExtraInfos { get; set; }
        internal string ExtraInfoCheck { get; set; }
        [DisplayName("受控模式"), Browsable(false)]
        internal bool SlaveMode { get; set; }
        internal bool ShowPassFailWindow { get; set; }
        [DisplayName("检查DID重复"), Browsable(false)]
        internal bool CheckDidDB { get; set; }
        [XmlIgnore]
        public float[] 位置参数 { get { return FormPLC.位置参数; } }
        [DisplayName("检查DID重复"), XmlIgnore]
        internal bool CheckDidDBDisp
        {
            get { return CheckDidDB; }
            set
            {
                if (value != CheckDidDB)
                    if (Program.MsgBoxYesNo("确认更改检查DID重复？"))
                        CheckDidDB = value;
            }
        }
        [DisplayName("序列号防重复"), Browsable(false)]
        public bool CheckSerialAuto { get; set; }
        [DisplayName("序列号防重复"), XmlIgnore]
        internal bool CheckSerialAutoDisp
        {
            get { return CheckSerialAuto; }
            set
            {
                if (value != CheckSerialAuto)
                    if (Program.MsgBoxYesNo("确认更改序列号防重复？"))
                        CheckSerialAuto = value;
            }
        }
        internal string DIDBuildString { get; set; }
        internal ushort DIDBuildWriteAddr { get; set; }
        internal ushort DIDBuildWaitBeforeWrite { get; set; }
        internal string PEDownload { get; set; }
        internal string PEDownload2 { get; set; }
        public int[] 图像旋转 { get; set; }
        internal int[] Delays { get; set; }
        [DisplayName("默认延迟")]
        internal int DefaultDelay { get; set; }
        [DisplayName("追溯结果")]
        internal string 追溯结果 { get; set; }
        [DisplayName("打印张数")]
        internal int PrintCopies { get; set; }
        [DisplayName("打印序列号"), XmlIgnore]
        internal string PrintSerialNum
        {
            get
            {
                return JobManager.barTenderPrint.SerialNum;
            }
            set
            {
                JobManager.barTenderPrint.SerialNum = value;
            }
        }
        internal bool UDPResultUpdate { get; set; }
        internal bool ShowDataDisplayForm { get; set; }
        [DisplayName("图片保存路径")]
        internal string ImgSavePath { get; set; }
        [DisplayName("保存JPEG"), Browsable(false)]
        public bool SaveJpeg
        {
            get
            {
                { return true; }
            }
            set { }
        }
        //public int BayerTest { get; set; }
        internal string PrintSerialNumMem;
        internal bool HidePrintButton { get; set; }
        internal DateTime PrintSerialNumMemTime;
        [DisplayName("BayerConverter"), XmlIgnore]
        internal int BayerConverter
        {
            get
            {
                try
                {
                    KSJDS.KSJ_BAYERMODE mode = (KSJDS.KSJ_BAYERMODE)(-1);
                    if (KSJDS.DeviceGetCount() > 0)
                    {
                        KSJDS.BayerGetMode(0, ref mode);
                        return (int)mode;
                    }
                    else
                        return -2;

                }
                catch
                {
                    return -3;
                }
            }
            set
            {
                try
                {
                    if (value >= 0)
                    {
                        if (KSJDS.DeviceGetCount() > 0)
                        {
                            KSJDS.BayerSetMode(0, (KSJDS.KSJ_BAYERMODE)value);
                        }
                        _BayerConverter = value;
                    }
                }
                catch (Exception ex)
                {
                    Program.ErrHdl(ex);
                }
            }
        }
        internal int _BayerConverter = 0;
        public bool[] 相机停用 { get; set; }
        public bool 通过IO输出结果 { get; set; }

        [Browsable(false)]
        public int[] CountsLog { get; set; }
        [Browsable(false)]
        public int CamFlag { get; set; }
        [Browsable(false)]
        public string k1 { get; set; }
        [Browsable(false)]
        public string k2 { get; set; }
        [Browsable(false)]
        public string k4 { get; set; }

        [Browsable(false)]
        public bool DisableRecover { get; set; }
        [Browsable(false)]
        public string[] Cams { get; set; }
        [Browsable(false)]
        public bool test { get { return File.Exists("vpc1.pdb"); } }
        [XmlIgnore]
        public bool logTestInfo { get; set; }
    }
    public struct ExtraInfoStruct
    {
        public string ExtraInfo { get; set; }
        public ushort ExtraInfoAddr { get; set; }
        public ushort ReadLen { get; set; }
        public string ExtraInfoFormate { get; set; }
        public double ExtraInfoRate { get; set; }
    }
    public enum SaveType
    {
        [Description("不保存")]
        NotSave,
        [Description("保存所有")]
        SaveAll,
        [Description("保存次品")]
        SaveFails
    }
}
