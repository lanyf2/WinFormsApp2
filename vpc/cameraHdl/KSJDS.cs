using Cognex.VisionPro;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace vpc
{
    internal class KSJDS
    {
        static bool inited = false;
        const int width = 2592;
        const int widthx3 = width * 3;
        const int height = 1944;
        static IntPtr ptrGlobal = System.Runtime.InteropServices.Marshal.AllocCoTaskMem(widthx3 * height);
        internal static CogImage24PlanarColor GetImage(int discardct = 3, int nChannel = 0)
        {
            if (inited == false)
                return null;
            IntPtr ptr = ptrGlobal;
            KSJ_API reval = 0;
            try
            {
                //if (discardct > 0)
                //    reval = CaptureRgbDataAfterEmptyFrameBuffer(nChannel, ptr);
                //else
                reval = CaptureRgbData(nChannel, ptr);
                if (reval == KSJ_API.RET_RECOVERY_SUCCESS)
                    reval = CaptureRgbData(nChannel, ptr);
                //HelperSaveToBmp(ptr, width, height, 24, "test.bmp");
            }
            catch (Exception ex)
            {
                Program.ErrHdl("KSJErr:" + ex.Message);
                return null;
            }
            if (reval != KSJ_API.RET_SUCCESS)
            {
                Program.Loginfo("KSJ CaptureRgbData Failed: " + reval);
                return null;
            }
            //HelperSaveToBmp(ptr, width, height, 24, "test.bmp");

            //IntPtr pdst = ptr2;
            //IntPtr psource = ptr + widthx3 * height;
            //for (int i = 0; i < height; i++)
            //{
            //    psource = psource - widthx3;
            //    CopyMemory(pdst, psource, widthx3);
            //    pdst = pdst + widthx3;
            //}
            using (Bitmap bmp = new Bitmap(width, height, widthx3, System.Drawing.Imaging.PixelFormat.Format24bppRgb, ptr))
            {
                //bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
                CogImage24PlanarColor img = new CogImage24PlanarColor(bmp);
                return img;
            }
        }
        //static CogImage24PlanarColor lastImg;
        //static bool convertFlag = false;
        internal static void Init()
        {
            //if (Settings.Default.test == false)
            {
                int re = KSJ_Init();
                //AutoWhiteBalance();
                int ct = DeviceGetCount();
                for (int i = 0; i < ct; i++)
                {
                    KSJ_API reval = KSJ_API.RET_SUCCESS;
                    if (Settings.Default._BayerConverter > 0)
                        reval = (KSJ_API)BayerSetMode(i, (KSJDS.KSJ_BAYERMODE)Settings.Default._BayerConverter);
                    else if (Environment.Is64BitProcess)
                        reval = (KSJ_API)BayerSetMode(i, (KSJDS.KSJ_BAYERMODE)5);
                    if (reval != KSJ_API.RET_SUCCESS)
                        Program.ErrHdl("设置BAYERMODE失败：" + reval);
                    reval = (KSJ_API)TriggerModeSet(i, KSJ_TRIGGERMODE.KSJ_TRIGGER_SOFTWARE);
                    if (reval != KSJ_API.RET_SUCCESS)
                        Program.ErrHdl("设置触发模式失败：" + reval);
                    if (System.IO.File.Exists("tbl.cc"))
                        ColorCorrectTableLoad(i, "tbl.cc");
                    int pdeviceType = 0, pnIndex = 0, val;
                    ushort pwFirmwareVersion = 0;
                    reval = (KSJ_API)DeviceGetInformation(i, ref pdeviceType, ref pnIndex, ref pwFirmwareVersion);
                    if (reval == KSJ_API.RET_SUCCESS && IndexDic.TryGetValue(pnIndex, out val) == false)
                        IndexDic.Add(pnIndex, i);
                    if (GetGAIN(i) < 10)
                        SetGAIN(i, 10);
                    inited = true;
                }
                //else
                //   ThreadPool.QueueUserWorkItem(RunAcquire);
            }
        }
        internal static Dictionary<int, int> IndexDic = new Dictionary<int, int>();

        internal static float GetExposureTime(int index)
        {
            float tm = -1;
            //if (Settings.Default.test == false)
            ExposureTimeGet(index, ref tm);
            return tm;
        }
        internal static void SetExposureTime(int index, float val)
        {
            ExposureTimeSet(index, val);
        }
        internal static int Contrast
        {
            get
            {
                int tm = 0;
                if (Settings.Default.test == false)
                    ProcessContrastGet(0, ref tm);
                return tm;
            }
            set
            {
                if (Settings.Default.test == false)
                    ProcessContrastSet(0, value);
            }
        }
        internal static int Gamma
        {
            get
            {
                int tm = 0;

                return tm;
            }
            set
            {
                KSJ_GammaSetValue(0, value);
            }
        }
        internal static int GetGAIN(int index)
        {
            int tm = 0;
            GetParam(index, KSJ_PARAM.KSJ_RED, ref tm);
            return tm;
        }
        internal static void SetGAIN(int index, int val)
        {
            int re = SetParam(index, KSJ_PARAM.KSJ_RED, val);
            re = SetParam(index, KSJ_PARAM.KSJ_BLUE, val);
            re = SetParam(index, KSJ_PARAM.KSJ_GREEN, val);
        }
        #region DllImport 
        [DllImport("KSJApi", EntryPoint = "KSJ_ColorCorrectTableLoad")]
        static extern int ColorCorrectTableLoad(int nChannel, string pszFileName);
        [DllImport("Kernel32.dll", EntryPoint = "RtlMoveMemory")]
        internal static extern void CopyMemory(IntPtr Destination, IntPtr Source, [MarshalAs(UnmanagedType.U4)] int Length);
        //        KSJ_CaptureRawDataAfterEmptyFrameBuffer
        //KSJ_CaptureRgbDataAfterEmptyFrameBuffer
        //KSJ_CaptureRgbDataExAfterEmptyFrameBuffer
        [DllImport("KSJApi", EntryPoint = "KSJ_Init")]
        private static extern int KSJ_Init();
        [DllImport("KSJApi", EntryPoint = "KSJ_GammaSetValue")]
        private static extern int KSJ_GammaSetValue(int nChannel, int nValue);
        [DllImport("KSJApi", EntryPoint = "KSJ_AWBSetRegion")]
        private static extern int AWBSetRegion(int nChannel, int nX, int nY, int nW, int nH);
        [DllImport("KSJApi", EntryPoint = "KSJ_CaptureRawData")]
        private static extern int CaptureRawData(int nChannel, IntPtr buff);
        [DllImport("KSJApi", EntryPoint = "KSJ_CaptureRgbData")]
        private static extern KSJ_API CaptureRgbData(int nChannel, IntPtr buff);
        [DllImport("KSJApi", EntryPoint = "KSJ_CaptureRgbDataAfterEmptyFrameBuffer")]
        private static extern int CaptureRgbDataAfterEmptyFrameBuffer(int nChannel, IntPtr buff);
        [DllImport("KSJApi", EntryPoint = "KSJ_ExposureTimeSet")]
        private static extern int ExposureTimeSet(int nChannel, float fExpTimeMs);
        [DllImport("KSJApi", EntryPoint = "KSJ_ExposureTimeGet")]
        private static extern int ExposureTimeGet(int nChannel, ref float fExpTimeMs);
        [DllImport("KSJApi", EntryPoint = "KSJ_ProcessContrastSet")]
        private static extern int ProcessContrastSet(int nChannel, int nValue);
        [DllImport("KSJApi", EntryPoint = "KSJ_ProcessContrastGet")]
        private static extern int ProcessContrastGet(int nChannel, ref int nValue);
        [DllImport("KSJApi", EntryPoint = "KSJ_DeviceGetCount")]
        internal static extern int DeviceGetCount();
        [DllImport("KSJApi", EntryPoint = "KSJ_UnInit")]
        internal static extern int UnInit();
        [DllImport("KSJApi", EntryPoint = "KSJ_CaptureSetFieldOfView")]
        internal static extern int CaptureSetFieldOfView(int nChannel, int nColumnStart, int
nRowStart, int nColumnSize, int nRowSize, KSJ_ADDRESSMODE ColumnAddressMode,
KSJ_ADDRESSMODE RowAddressMode);
        [DllImport("KSJApi", EntryPoint = "KSJ_CaptureGetSize")]
        internal static extern int CaptureGetSize(int nChannel, ref int pnWidth, ref int pnHeight);
        [DllImport("KSJApi", EntryPoint = "KSJ_CaptureGetSizeEx")]
        internal static extern int CaptureGetSizeEx(int nChannel, ref int pnWidth, ref int pnHeight, ref int pnBitCount);
        [DllImport("KSJApi", EntryPoint = "KSJ_HelperSaveToBmp")]
        internal static extern int HelperSaveToBmp(IntPtr pData, int nWidth, int nHeight, int nBitCount, string pszFileName);
        [DllImport("KSJApi", EntryPoint = "KSJ_BayerSetMode")]
        internal static extern int BayerSetMode(int nChannel, KSJ_BAYERMODE BayerMode);
        [DllImport("KSJApi", EntryPoint = "KSJ_BayerGetMode")]
        internal static extern int BayerGetMode(int nChannel, ref KSJ_BAYERMODE BayerMode);
        [DllImport("KSJApi", EntryPoint = "KSJ_FilterSetMode")]
        internal static extern int FilterSetMode(int nChannel, KSJ_FILTERMODE FilterrMode);
        [DllImport("KSJApi", EntryPoint = "KSJ_SetParam")]
        internal static extern int SetParam(int nChannel, KSJ_PARAM Param, int nValue);
        [DllImport("KSJApi", EntryPoint = "KSJ_GetParam")]
        internal static extern int GetParam(int nChannel, KSJ_PARAM Param, ref int nValue);
        // 得到设备信息（型号，序号，固件版本号）
        [DllImport("KSJApi.dll", EntryPoint = "KSJ_DeviceGetInformation")]
        internal static extern int DeviceGetInformation(int nChannel, ref int pDeviceType, ref int pnIndex, ref ushort pwFirmwareVersion);

        // Set Trigger Mode
        [DllImport("KSJApi.dll", EntryPoint = "KSJ_TriggerModeSet")]
        public static extern int TriggerModeSet(int nChannel, KSJ_TRIGGERMODE TriggerMode);
        #endregion
        #region Enums
        // Trigger Mode
        public enum KSJ_TRIGGERMODE
        {
            KSJ_TRIGGER_INTERNAL,
            KSJ_TRIGGER_EXTERNAL,
            KSJ_TRIGGER_SOFTWARE,
            KSJ_TRIGGER_FIXFRAMERATE
        };
        public enum KSJ_BAYERMODE
        {
            KSJ_BGGR_BGR24 = 0,
            KSJ_GRBG_BGR24,
            KSJ_RGGB_BGR24,
            KSJ_GBRG_BGR24,
            KSJ_BGGR_BGR24_FLIP,
            KSJ_GRBG_BGR24_FLIP,
            KSJ_RGGB_BGR24_FLIP,
            KSJ_GBRG_BGR24_FLIP,
            KSJ_BGGR_BGR32,
            KSJ_GRBG_BGR32,
            KSJ_RGGB_BGR32,
            KSJ_GBRG_BGR32,
            KSJ_BGGR_BGR32_FLIP,
            KSJ_GRBG_BGR32_FLIP,
            KSJ_RGGB_BGR32_FLIP,
            KSJ_GBRG_BGR32_FLIP,
            KSJ_BGGR_GRAY8,
            KSJ_GRBG_GRAY8,
            KSJ_RGGB_GRAY8,
            KSJ_GBRG_GRAY8,
            KSJ_BGGR_GRAY8_FLIP,
            KSJ_GRBG_GRAY8_FLIP,
            KSJ_RGGB_GRAY8_FLIP,
            KSJ_GBRG_GRAY8_FLIP
        };
        internal enum KSJ_FILTERMODE
        {
            KSJ_NEARESTNEIGHBOR,
            KSJ_BILINEAR,
            KSJ_SMOOTHHUE,
            KSJ_EDGESENSING,
            KSJ_LAPLACIAN,
            KSJ_FASTBILINEAR
        };
        internal enum KSJ_API
        {
            RET_SUCCESS = 1,      // Function Return Successfully.
            RET_PARAMERROR = -1,// User's Parameter Passed to API Error.
            RET_MALLOCFAIL = -2,  // Memory Allocated Fail.
            RET_NOTSUPPORT = -3,   // Function not supported by such type camera.
            RET_DEVICENOTEXIST = -4,    // Device doesn't be detected.
            RET_DEVICENOTINIT = -5,    // Device haven't be initialized
            RET_VIOLATION = -6,    // This operation is conflict to other operation.
            RET_NOPRIVILEGE = -7,  // User no privilege
            RET_FAIL = -8,  // Function Return Failed.( Normal Error, no Detail )
            RET_WRONG = -9,   // Same to RET_FAIL.
            RET_RECOVERY_SUCCESS = -10,  // Device is recovered successfully.
            RET_RECOVERY_FAIL = -11,   // Device recovered, but fail.
            RET_BADFRAME = -12,   // Bad Frame comes from sensor, should be skipped. 
            RET_INVALIDFRAME = -13,  // Invalid Frame, Transmission Error, this frame should re-get.
            RET_ZEROFRAME = -14,   // 帧存相机会返回此值，表示采集图像数据0字节，错误的帧
            RET_VERSION_ERROR = -15,   // 版本错误
            RET_TIMEOUT = -16,  // 当设置读取超时之后不进行恢复时，采集函数会返回此数值，而不会返回恢复的状态
            RET_DEVICECLOSED = -17
        }
        public enum KSJ_DEVICETYPE
        {
            KSJ_UC130C_MRNN = 0,  // Guass2
            KSJ_UC130M_MRNN,      // Guass2
            KSJ_RESERVED0,
            KSJ_UC320C_MRNN,      // Guass2
            KSJ_UC130C_MRYN,
            KSJ_UC130M_MRYN,
            KSJ_RESERVED1,
            KSJ_UC320C_MRYN,
            KSJ_UC500C_MRNN,
            KSJ_UC500M_MRNN,
            KSJ_UC500C_MRYN,
            KSJ_UC500M_MRYN,
            KSJ_UC320C_OCR,      // Not Support
            KSJ_UC900C_MRNN,     // Not Support
            KSJ_UC1000C_MRNN,    // Not Support
            KSJ_UC900C_MRYN,     // Not Support
            KSJ_UC1000C_MRYN,    // Not Support
            KSJ_UC130C_MRYY,     // Elanus2
            KSJ_UC130M_MRYY,     // Elanus2 
            KSJ_UD140C_SGNN,     // Not Support
            KSJ_UD140M_SGNN,     // Not Support
            KSJ_UC36C_MGNN,      // Not Support
            KSJ_UC36M_MGNN,      // Not Support
            KSJ_UC36C_MGYN,      // Not Support
            KSJ_UC36M_MGYN,      // Not Support
            KSJ_UC900C_MRYY,     // Elanus2
            KSJ_UC1000C_MRYY,    // Elanus2
            KSJ_UC1400C_MRYY,    // Elanus2
            KSJ_UC36C_MGYY,      // Elanus2
            KSJ_UC36M_MGYY,      // Elanus2
            KSJ_UC320C_MRYY,     // Elanus2
            KSJ_UC500C_MRYY,     // Elanus2
            KSJ_UC500M_MRYY,     // Elanus2
            KSJ_MUC130C_MRYN,    // OEM
            KSJ_MUC130M_MRYN,    // OEM
            KSJ_MUC320C_MRYN,    // OEM
            KSJ_MUC36C_MGYYO,    // Jelly2
            KSJ_MUC36M_MGYYO,    // Jelly2 
            KSJ_MUC130C_MRYY,    // Not Support
            KSJ_MUC130M_MRYY,    // Not Support
            KSJ_MUC320C_MRYY,    // Not Support
            KSJ_MUC500C_MRYY,    // Not Support
            KSJ_MUC500M_MRYYO,   // Jelly2
            KSJ_MUC900C_MRYY,    // Not Support
            KSJ_MUC1000C_MRYY,   // Not Support
            KSJ_MUC1400C_MRYY,   // Not Support
            KSJ_UD205C_SGYY,     // Elanus2
            KSJ_UD205M_SGYY,     // Elanus2
            KSJ_UD274C_SGYY,     // Elanus2
            KSJ_UD274M_SGYY,     // Elanus2
            KSJ_UD285C_SGYY,     // Elanus2
            KSJ_UD285M_SGYY,     // Elanus2
            KSJ_MU3C500C_MRYYO,  // Jelly3 
            KSJ_MU3C500M_MRYYO,  // Jelly3
            KSJ_MU3C1000C_MRYYO, // Jelly3
            KSJ_MU3C1400C_MRYYO, // Jelly3
            KSJ_MU3V130C_CGYYO,  // Jelly3
            KSJ_MU3V130M_CGYYO,  // Jelly3
            KSJ_MU3E130C_EGYYO,  // Jelly3
            KSJ_MU3E130M_EGYYO,  // Jelly3
            KSJ_MUC36C_MGYFO,    // Jelly1
            KSJ_MUC36M_MGYFO,    // Jelly1
            KSJ_MU3C120C_MGYYO,  // Jelly3
            KSJ_MU3C120M_MGYYO,  // Jelly3
            KSJ_MU3E200C_EGYYO,  // Jelly3
            KSJ_MU3E200M_EGYYO,  // Jelly3
            KSJ_MUC130C_MRYNO,   // Jelly1
            KSJ_MUC130M_MRYNO,   // Jelly1
            KSJ_MUC320C_MRYNO,   // Jelly1
            KSJ_U3C130C_MRYNO,   // Not Support
            KSJ_U3C130M_MRYNO,   // Not Support
            KSJ_U3C320C_MRYNO,   // Not Support
            KSJ_U3C500C_MRYNO,   // Guass3
            KSJ_U3C500M_MRYNO,   // Guass3
            KSJ_MU3C1401C_MRYYO,
            KSJ_MU3C1001C_MRYYO,
            KSJ_MUC131M_MRYN,    // OEM Device
            KSJ_MU3C501C_MRYYO,
            KSJ_MU3C501M_MRYYO,
            KSJ_MU3C121C_MGYYO,
            KSJ_MU3C121M_MGYYO,
            KSJ_MU3E131C_EGYYO,
            KSJ_MU3E131M_EGYYO,
            KSJ_MU3E201C_EGYYO,
            KSJ_MU3E201M_EGYYO,
            KSJ_MISSING_DEVICE,	  // Device Lost Program
            KSJ_MU3S230C_SGYYO,   // Jelly3 Sony IMX174
            KSJ_MU3S230M_SGYYO,   // Jelly3 Sony IMX174
            KSJ_MU3S640C_SRYYO,   // Jelly3 Sony IMX178
            KSJ_MU3S640M_SRYYO,   // Jelly3 Sony IMX178
            KSJ_CUD285C_SGYYO,
            KSJ_CUD285M_SGYYO,
            KSJ_MU3S231C_SGYYO,   // Jelly3 Sony IMX249
            KSJ_MU3S231M_SGYYO    // Jelly3 Sony IMX249
        };
        internal enum KSJ_ADDRESSMODE
        {
            KSJ_SKIPNONE = 0,
            KSJ_SKIP2,
            KSJ_SKIP3,
            KSJ_SKIP4,
            KSJ_SKIP8
        };
        public enum KSJ_PARAM
        {
            KSJ_EXPOSURE = 0,        // Exposure Time (ms)
            KSJ_RED,                 // Red Gain
            KSJ_GREEN,               // Green Gain
            KSJ_BLUE,                // Blue Gain
            KSJ_GAMMA,               // Gamma
            KSJ_PREVIEW_COLUMNSTART, // Preview Col Start
            KSJ_PREVIEW_ROWSTART,    // Preview Row Start
            KSJ_CAPTURE_COLUMNSTART, // Capture Col Start
            KSJ_CAPTURE_ROWSTART,    // Capture Row Start
            KSJ_HORIZONTALBLANK,     // Horizontal Blank
            KSJ_VERTICALBLANK,       // Vertical Blank
            KSJ_FLIP,                // Flip
            KSJ_BIN,                 // Binning
            KSJ_MIRROR,              // Mirror
            KSJ_CONTRAST,            // Contrast
            KSJ_BRIGHTNESS,          // Brightness
            KSJ_VGAGAIN,             // VGA Gain(CCD)
            KSJ_CLAMPLEVEL,          // Clamp Level(CCD)
            KSJ_CDSGAIN,             // CDS Gain(CCD)
            KSJ_RED_SHIFT,           // Not Use
            KSJ_GREEN_SHIFT,         // Not Use
            KSJ_BLUE_SHIFT,          // Not Use
            KSJ_COMPANDING,          // Companding
            KSJ_EXPOSURE_LINES,      // Exposure Lines
            KSJ_SATURATION,          // Saturation
            KSJRIGGERDELAY,        // Trigger Delay Step = 100uS
            KSJ_STROBEDELAY,         // Not Use
            KSJRIGGER_MODE,        // Trigger Mode
            KSJRIGGER_METHOD,      // Trigger Method
            KSJ_BLACKLEVEL,          // Black Level
            KSJ_BLACKLEVELHRESHOLD_AUTO, // Black Level Threshold Auto
            KSJ_BLACKLEVELHRESHOLD_LO,   // Black Level Low Threshold
            KSJ_BLACKLEVELHRESHOLD_HI    // Black Level High Threshold
        };
        #endregion
        internal static void Dispose()
        {
            UnInit();
            if (ptrGlobal != IntPtr.Zero)
                System.Runtime.InteropServices.Marshal.FreeCoTaskMem(ptrGlobal);
            ptrGlobal = IntPtr.Zero;
            //if (ptrGlobal2 != IntPtr.Zero)
            //    System.Runtime.InteropServices.Marshal.FreeCoTaskMem(ptrGlobal2);
            //ptrGlobal2 = IntPtr.Zero;
        }
        internal static void AutoWhiteBalance()
        {
            AWBSetRegion(0, 0, 0, width, height);
        }
    }
    internal class KSJHdl : CameraHdlBase
    {
        internal KSJHdl(int index = 0)
        {
            //if (KSJDS.DeviceGetCount() <= 1)
            //    camindex = 0;
            //else
            if (index == 0)
                camindex = 0;
            else
            {
                int val;
                if (KSJDS.IndexDic.TryGetValue(index, out val))
                    camindex = val;
                else
                    camindex = -1;
            }
        }
        int failCt = 0;
        internal override ICogImage GetImage(int discardct = 3)
        {
            if (failCt > 3)
            {
                failCt = 0;
                Init();
            }
            var re = KSJDS.GetImage(discardct, camindex);
            if (re == null)
                failCt++;
            else
                failCt = 0;
            return re;
        }
        internal override void Init()
        {
            KSJDS.Init();
        }
        internal override double ExposureTime
        {
            get
            {
                return KSJDS.GetExposureTime(camindex);
            }
            set
            {
                KSJDS.SetExposureTime(camindex, (float)value);
            }
        }
        internal override double Gain
        {
            get
            {
                return KSJDS.GetGAIN(camindex);
            }
            set
            {
                KSJDS.SetGAIN(camindex, (int)value);
            }
        }
        internal override double Brightness
        {
            get
            {
                return -1;
            }
            set
            {
            }
        }
        internal override void Dispose()
        {
            KSJDS.Dispose();
        }
        internal override bool Connected
        {
            get
            {
                return KSJDS.DeviceGetCount() > 0;
            }
        }
    }

    public abstract class CameraHdlBase
    {
        public static bool extTrigFlag = true;
        public Action<ICogImage, int> ImageGrabbed;
        internal CameraHdlBase(int index = 0)
        {
            camindex = index;
        }
        internal int camindex;
        internal virtual object GetTag() { return null; }
        internal abstract ICogImage GetImage(int discardct = 3);
        internal abstract void Init();
        internal abstract double ExposureTime { get; set; }
        internal abstract double Gain { get; set; }
        internal abstract double Brightness { get; set; }
        internal abstract void Dispose();
        internal abstract bool Connected { get; }
        internal virtual string CameraName { get { return null; } }
        internal virtual void SetTrigMode(bool mode) { }
    }
}
