using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IAT.Imaging;
using Cognex.VisionPro;
using System.Runtime.InteropServices;
using System.Drawing;

namespace vpc
{
    internal class MVCameraHdl : CameraHdlBase
    {
        IntPtr camHdl = IntPtr.Zero;
        bool inited = false;
        const int width = 1280;
        const int widthx3 = width * 3;
        const int widthx4 = width * 4;
        const int height = 1024;
        static IntPtr ptrGlobal = System.Runtime.InteropServices.Marshal.AllocCoTaskMem(widthx3 * height);
        //1280*1024  MV-VDF 130SC

        internal override ICogImage GetImage(int discardct = 3)
        {
            if (inited)
            {
                IntPtr imgptr = new IntPtr();
                var re = CAMER_GetOutImage(camHdl, ref imgptr, 0);
                if (re == MVSTATUS.STATUS_OK)
                {
                    //CogImage24PlanarColor img = new CogImage24PlanarColor(width, height);
                    //ICogImage8PixelMemory m0, m1, m2;
                    //img.Get24PlanarColorPixelMemory(CogImageDataModeConstants.Write, 0, 0, width, height, out m0, out m1, out m2);
                    //for (int i = 0; i < height; i++)
                    //{
                    //    KSJDS.CopyMemory(m0.Scan0 + i * width, imgptr + i * 3 * width, width);
                    //    KSJDS.CopyMemory(m1.Scan0 + i * width, imgptr + (i * 3 + 1) * width, width);
                    //    KSJDS.CopyMemory(m2.Scan0 + i * width, imgptr + (i * 3 + 2) * width, width);
                    //}
                    //m0.Dispose();
                    //m1.Dispose();
                    //m2.Dispose();
                    //return img;

                    using (Bitmap bmp = new Bitmap(width, height, widthx4, System.Drawing.Imaging.PixelFormat.Format32bppArgb, imgptr))
                    {
                        CogImage24PlanarColor img = new CogImage24PlanarColor(bmp);
                        return img;
                    }
                }
            }
            return null;
        }
        internal override void Init()
        {
            var re = CAMER_Create(0, ref camHdl);
            if (re == MVSTATUS.STATUS_OK && camHdl != IntPtr.Zero)
            {
                re = CAMER_Run(camHdl, true);
                if (re == MVSTATUS.STATUS_OK)
                    inited = true;
                int paval = 2;
                CAMER_DirectShowBit(camHdl, true, ref paval);
                CAMER_SetAdjust(camHdl, ADJPROPERY.R_GAIN, 25);
                CAMER_SetAdjust(camHdl, ADJPROPERY.G_GAIN, 16);
                CAMER_SetAdjust(camHdl, ADJPROPERY.B_GAIN, 40);
                CAMER_SetPropery(camHdl, CMRCTL.SEN_FILP, 3);
            }
        }
        internal override double ExposureTime
        {
            get
            {
                if (inited && camHdl != IntPtr.Zero)
                {
                    int val = 0;
                    var re = CAMER_GetPropery(camHdl, CMRCTL.SEN_TIME, ref val);
                    if (re == MVSTATUS.STATUS_OK)
                        return val / 1000.0;
                    else
                        return -2;
                }
                return -1;
            }
            set
            {
                if (inited && camHdl != IntPtr.Zero)
                {
                    if (value >= 1)
                    {
                        var re = CAMER_SetPropery(camHdl, CMRCTL.SEN_TIME, (int)(value * 1000));
                    }
                }
            }
        }
        internal override double Gain
        {
            get
            {
                return -1;
            }
            set
            {
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
            if (camHdl != IntPtr.Zero)
            {
                CAMER_Run(camHdl, false);
                CAMER_Destroy(camHdl);

            }
            if (ptrGlobal != IntPtr.Zero)
                System.Runtime.InteropServices.Marshal.FreeCoTaskMem(ptrGlobal);
            ptrGlobal = IntPtr.Zero;
        }
        internal override bool Connected
        {
            get
            {
                return inited;
            }
        }

        [DllImport("MVCamer", EntryPoint = "CAMER_GetDeviceNumber")]
        internal static extern MVSTATUS CAMER_GetDeviceNumber(ref int pNumber);
        [DllImport("MVCamer", EntryPoint = "CAMER_SetAdjust")]
        internal static extern MVSTATUS CAMER_SetAdjust(IntPtr hCamer, ADJPROPERY Propery, int Val);
        [DllImport("MVCamer", EntryPoint = "CAMER_DirectShowBit")]
        internal static extern MVSTATUS CAMER_DirectShowBit(IntPtr hCamer, bool bSet, ref int pVal);
        [DllImport("MVCamer", EntryPoint = "CAMER_Create")]
        static extern MVSTATUS CAMER_Create(int Index, ref IntPtr hCamer);
        [DllImport("MVCamer", EntryPoint = "CAMER_Destroy")]
        static extern MVSTATUS CAMER_Destroy(IntPtr hCamer);
        [DllImport("MVCamer", EntryPoint = "CAMER_Run")]
        static extern MVSTATUS CAMER_Run(IntPtr hCamer, bool bRun);
        [DllImport("MVCamer", EntryPoint = "CAMER_SetPropery")]
        static extern MVSTATUS CAMER_SetPropery(IntPtr hCamer, CMRCTL Propery, int Val);
        [DllImport("MVCamer", EntryPoint = "CAMER_GetPropery")]
        static extern MVSTATUS CAMER_GetPropery(IntPtr hCamer, CMRCTL Propery, ref int pVal);
        [DllImport("MVCamer", EntryPoint = "CAMER_GetOutImage")]
        static extern MVSTATUS CAMER_GetOutImage(IntPtr hCamer, ref IntPtr ppBuf, int nSum);

        #region Enum
        internal enum CMRCTL
        {
            DRV_INDX = 0, // 仅读, 返回当前卡的驱动下标
            SEN_NUM = 1, // 运行时的每秒帧率统计
            SEN_TYPE = 2, // 仅读, 返回当前卡类型
            SEN_XOFF = 3, // 偏移X
            SEN_YOFF = 4, // 偏移Y
            SEN_TIME = 5, // 曝光时间
            SEN_BLACK = 6, // 模拟黑电平级别
            CTL_BLACK = 7, // 采集模式：0 - 彩色或黑白(由Sensor 决定)，1 - Byer Raw
            CTL_BIT = 8, // 0-低8 位，1-中8 位，2 高8 位，3 全位
            CTL_SMOOT = 9, // Sensor 平滑处理
            CTL_WNLEL = 10, // 数字黑电平级别
            OUT_SIZE = 11, // 输出尺寸, 高16 位输出高，低16 位输出宽
            AUT_WHIT = 12, // 自动白平衡
            MUL_BAD = 13, // 热点处理
            USER_ID = 14, // 相机用户ID 设置，只接受8 个10 进制
            OUT_SHARP = 15, // 镜下(静态)模式 0-无；1-一般滤波；4,8,16,32 递归滤波；
            TRAN_MODE = 16, // 超频
            DRV_LIST = 17, // 输出的尺寸/像素格式列表
            GAMA_LIST = 18, // 输出的Gama 列表
            COLOR_ADJ = 19, // 是否使用Gama 对图像调节
            WHITE_ADJ = 20, // 报告当前的白平衡值
            TRI_MODE = 21, // 0: 正常模式，1：触发输入模式
            SEN_FILP = 22, // 0 - 不翻转，1 - 左右翻转，2-上下左右翻转，3-上下翻转
        };
        internal enum ADJPROPERY
        {
            R_GAIN, // VGA 卡的R 分量的亮度
            G_GAIN, // VGA 卡的G 分量的亮度
            B_GAIN, // VGA 卡的B 分量的亮度
            R_CONTRAST, // VGA 卡的R 分量的对比度
            G_CONTRAST, // VGA 卡的G 分量的对比度
            B_CONTRAST, // VGA 卡的B 分量的对比度
        };
        internal enum MVSTATUS
        {
            STATUS_ERROR,
            STATUS_OK = 1,
            STATUS_NO_DEVICE_FOUND = 2,
            STATUS_DEVICE_HANDLE_INVALID = 3,
            STATUS_DEVICE_CANNOT_CONNECT = 4,
        };
        #endregion

    }
}
