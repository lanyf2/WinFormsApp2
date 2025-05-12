using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cognex.VisionPro;
using System.Threading;
using GxIAPINET;
using ThridLibray;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;

namespace vpc
{
    internal class DaHengHdl : CameraHdlBase
    {
        object lockobj = new object();
        int m_nWidth = 0;                   ///<图像宽度
        int m_nHeigh = 0;                   ///<图像高度
        int m_nPayloadSize = 0;                   ///<图像数据大小

        internal DaHengHdl(int index = 0)
        {
            camindex = index;
        }
        ICogImage TryReconnect()
        {
            if ((DateTime.Now - ReconnectTime).TotalSeconds > 10)
            {
                ReconnectTime = DateTime.Now;


                Init();
            }
            return null;
        }
        internal override ICogImage GetImage(int discardct = 3)
        {
            try
            {
                lock (lockobj)
                {
                    GX_VALID_BIT_LIST emValidBits = GX_VALID_BIT_LIST.GX_BIT_0_7;
                    IImageData objIImageData = null;
                    if (null == m_objIGXStream || null == m_objIGXFeatureControl)
                        TryReconnect();

                    if (null == m_objIGXStream)
                    {
                        if (Settings.Default.logTestInfo)
                            Program.Loginfo("null == m_objIGXStream");
                        return null;
                    }
                    if (null == m_objIGXFeatureControl)
                    {
                        if (Settings.Default.logTestInfo)
                            Program.Loginfo("null == m_objIGXFeatureControl");
                        return null;
                    }
                    m_objIGXStream.FlushQueue();
                    //发送软触发命令
                    m_objIGXFeatureControl.GetCommandFeature("TriggerSoftware").Execute();
                    objIImageData = m_objIGXStream.GetImage(10000);

                    if (null != objIImageData)
                    {
                        emValidBits = __GetBestValudBit(objIImageData.GetPixelFormat());
                        IntPtr ptr = objIImageData.ConvertToRGB24(emValidBits, GX_BAYER_CONVERT_TYPE_LIST.GX_RAW2RGB_NEIGHBOUR, false);

                        using (Bitmap bmp = new Bitmap(m_nWidth, m_nHeigh, m_nWidth * 3, System.Drawing.Imaging.PixelFormat.Format24bppRgb, ptr))
                        {
                            //bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
                            CogImage24PlanarColor img = new CogImage24PlanarColor(bmp);
                            if (null != objIImageData)
                            {
                                objIImageData.Destroy();
                            }
                            return img;
                        }
                    }
                    else
                        Program.MsgBox("objIImageData==null");
                }
                return null;
            }
            catch (Exception exception)
            {
                Program.ErrHdl(exception);
            }

            return TryReconnect();
        }
        DateTime ReconnectTime = DateTime.MinValue;
        internal override void Init()
        {
            try
            {
                List<IGXDeviceInfo> listGXDeviceInfo = new List<IGXDeviceInfo>();
                if (m_objIGXFactory == null)
                {
                    m_objIGXFactory = IGXFactory.GetInstance();
                    m_objIGXFactory.Init();
                }

                __CloseStream();
                __CloseDevice();
                m_objIGXFactory.UpdateDeviceList(200, listGXDeviceInfo);

                // 判断当前连接设备个数
                if (listGXDeviceInfo.Count <= 0)
                {
                    Program.MsgBox("未发现相机备!");
                    return;
                }
                //打开列表第一个设备
                if (camindex == 0 && Settings.Default.Cams?.Length > 0)
                {
                    int id;
                    if (int.TryParse(Settings.Default.Cams[0], out id) && id > 0)
                        camindex = id;
                }
                if (camindex == 0)
                {
                    m_objIGXDevice = m_objIGXFactory.OpenDeviceBySN(listGXDeviceInfo[0].GetSN(), GX_ACCESS_MODE.GX_ACCESS_EXCLUSIVE);
                }
                else
                {
                    m_objIGXDevice = m_objIGXFactory.OpenDeviceByUserID(camindex.ToString(), GX_ACCESS_MODE.GX_ACCESS_EXCLUSIVE);
                }
                m_objIGXFeatureControl = m_objIGXDevice.GetRemoteFeatureControl();

                m_nPayloadSize = (int)m_objIGXDevice.GetRemoteFeatureControl().GetIntFeature("PayloadSize").GetValue();
                m_nWidth = (int)m_objIGXDevice.GetRemoteFeatureControl().GetIntFeature("Width").GetValue();
                m_nHeigh = (int)m_objIGXDevice.GetRemoteFeatureControl().GetIntFeature("Height").GetValue();

                //打开流
                m_objIGXStream = m_objIGXDevice.OpenStream(0);
                m_objIGXStream.StartGrab();
                m_objIGXFeatureControl.GetCommandFeature("AcquisitionStart").Execute();

                __InitDevice();
            }
            catch (Exception ex)
            {
                if (ex != null && string.IsNullOrEmpty(ex.Message) == false && ex.Message.StartsWith("{Access denied:{-1005}"))
                    Program.ErrHdl("相机无法连接：相机线必须插在蓝色USB3.0的接口上");
                else
                    Program.ErrHdl(ex);
            }

        }
        internal override double ExposureTime
        {
            get
            {
                try
                {
                    if (m_objIGXFeatureControl != null)
                        return (int)Gain * 1000 + m_objIGXFeatureControl.GetFloatFeature("ExposureTime").GetValue() / 1000;
                }
                catch
                {
                    TryReconnect();
                }
                return -1;
            }
            set
            {
                if (value > 0)
                {
                    if (m_objIGXFeatureControl != null)
                    {
                        m_objIGXFeatureControl.GetFloatFeature("ExposureTime").SetValue(value % 1000 * 1000);
                        Gain = (int)(value / 1000);
                    }
                }
            }
        }
        internal override double Gain
        {
            get
            {
                if (m_objIGXFeatureControl != null)
                    return m_objIGXFeatureControl.GetFloatFeature("Gain").GetValue();
                return -1;
            }
            set
            {
                if (value >= 0)
                {
                    if (m_objIGXFeatureControl != null)
                        m_objIGXFeatureControl.GetFloatFeature("Gain").SetValue(value);
                }
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
                if (value > 0)
                {

                }
            }
        }
        internal override void Dispose()
        {
            __CloseStream();
            __CloseDevice();
        }
        internal override bool Connected
        {
            get
            {
                return true;
            }
        }

        IGXFactory m_objIGXFactory = null;                   ///<Factory对像
        IGXDevice m_objIGXDevice = null;                   ///<设备对像
        IGXStream m_objIGXStream = null;                   ///<流对像
        IGXFeatureControl m_objIGXFeatureControl = null;                   ///<远端设备属性控制器对像
        private void __CloseStream()
        {
            try
            {
                if (m_objIGXFeatureControl != null)
                    m_objIGXFeatureControl.GetCommandFeature("AcquisitionStop").Execute();

                //关闭流
                if (null != m_objIGXStream)
                {
                    m_objIGXStream.StopGrab();
                    m_objIGXStream.Close();
                    m_objIGXStream = null;
                }
            }
            catch (Exception)
            {
            }
        }
        private void __CloseDevice()
        {
            try
            {
                //关闭设备
                if (null != m_objIGXDevice)
                {
                    m_objIGXDevice.Close();
                    m_objIGXDevice = null;
                }
            }
            catch (Exception)
            {
            }
        }
        private void __InitDevice()
        {
            if (null != m_objIGXFeatureControl)
            {
                //设置采集模式连续采集
                m_objIGXFeatureControl.GetEnumFeature("AcquisitionMode").SetValue("Continuous");

                //设置触发模式为开
                m_objIGXFeatureControl.GetEnumFeature("TriggerMode").SetValue("On");

                //选择触发源为软触发
                m_objIGXFeatureControl.GetEnumFeature("TriggerSource").SetValue("Software");
            }
        }
        private int __GetStride(int nWidth, bool bIsColor)
        {
            return bIsColor ? nWidth * 3 : nWidth;
        }
        private Bitmap __UpdateBitmapForSave(byte[] byBuffer)
        {
            Bitmap m_bitmapForSave = null;                ///<bitmap对象,仅供存储图像使用
            m_bitmapForSave = new Bitmap(m_nWidth, m_nHeigh, PixelFormat.Format24bppRgb);

            //给BitmapData加锁
            BitmapData bmpData = m_bitmapForSave.LockBits(new Rectangle(0, 0, m_bitmapForSave.Width, m_bitmapForSave.Height), ImageLockMode.ReadWrite, m_bitmapForSave.PixelFormat);

            //得到一个指向Bitmap的buffer指针
            IntPtr ptrBmp = bmpData.Scan0;
            int nImageStride = __GetStride(m_nWidth, true);
            //图像宽能够被4整除直接copy
            if (nImageStride == bmpData.Stride)
            {
                Marshal.Copy(byBuffer, 0, ptrBmp, bmpData.Stride * m_bitmapForSave.Height);
            }
            else//图像宽不能够被4整除按照行copy
            {
                for (int i = 0; i < m_bitmapForSave.Height; ++i)
                {
                    Marshal.Copy(byBuffer, i * nImageStride, new IntPtr(ptrBmp.ToInt64() + i * bmpData.Stride), m_nWidth);
                }
            }
            //BitmapData解锁
            m_bitmapForSave.UnlockBits(bmpData);

            return m_bitmapForSave;
        }
        private GX_VALID_BIT_LIST __GetBestValudBit(GX_PIXEL_FORMAT_ENTRY emPixelFormatEntry)
        {
            GX_VALID_BIT_LIST emValidBits = GX_VALID_BIT_LIST.GX_BIT_0_7;
            switch (emPixelFormatEntry)
            {
                case GX_PIXEL_FORMAT_ENTRY.GX_PIXEL_FORMAT_MONO8:
                case GX_PIXEL_FORMAT_ENTRY.GX_PIXEL_FORMAT_BAYER_GR8:
                case GX_PIXEL_FORMAT_ENTRY.GX_PIXEL_FORMAT_BAYER_RG8:
                case GX_PIXEL_FORMAT_ENTRY.GX_PIXEL_FORMAT_BAYER_GB8:
                case GX_PIXEL_FORMAT_ENTRY.GX_PIXEL_FORMAT_BAYER_BG8:
                    {
                        emValidBits = GX_VALID_BIT_LIST.GX_BIT_0_7;
                        break;
                    }
                case GX_PIXEL_FORMAT_ENTRY.GX_PIXEL_FORMAT_MONO10:
                case GX_PIXEL_FORMAT_ENTRY.GX_PIXEL_FORMAT_BAYER_GR10:
                case GX_PIXEL_FORMAT_ENTRY.GX_PIXEL_FORMAT_BAYER_RG10:
                case GX_PIXEL_FORMAT_ENTRY.GX_PIXEL_FORMAT_BAYER_GB10:
                case GX_PIXEL_FORMAT_ENTRY.GX_PIXEL_FORMAT_BAYER_BG10:
                    {
                        emValidBits = GX_VALID_BIT_LIST.GX_BIT_2_9;
                        break;
                    }
                case GX_PIXEL_FORMAT_ENTRY.GX_PIXEL_FORMAT_MONO12:
                case GX_PIXEL_FORMAT_ENTRY.GX_PIXEL_FORMAT_BAYER_GR12:
                case GX_PIXEL_FORMAT_ENTRY.GX_PIXEL_FORMAT_BAYER_RG12:
                case GX_PIXEL_FORMAT_ENTRY.GX_PIXEL_FORMAT_BAYER_GB12:
                case GX_PIXEL_FORMAT_ENTRY.GX_PIXEL_FORMAT_BAYER_BG12:
                    {
                        emValidBits = GX_VALID_BIT_LIST.GX_BIT_4_11;
                        break;
                    }
                case GX_PIXEL_FORMAT_ENTRY.GX_PIXEL_FORMAT_MONO14:
                    {
                        //暂时没有这样的数据格式待升级
                        break;
                    }
                case GX_PIXEL_FORMAT_ENTRY.GX_PIXEL_FORMAT_MONO16:
                case GX_PIXEL_FORMAT_ENTRY.GX_PIXEL_FORMAT_BAYER_GR16:
                case GX_PIXEL_FORMAT_ENTRY.GX_PIXEL_FORMAT_BAYER_RG16:
                case GX_PIXEL_FORMAT_ENTRY.GX_PIXEL_FORMAT_BAYER_GB16:
                case GX_PIXEL_FORMAT_ENTRY.GX_PIXEL_FORMAT_BAYER_BG16:
                    {
                        //暂时没有这样的数据格式待升级
                        break;
                    }
                default:
                    break;
            }
            return emValidBits;
        }

    }
    internal class DaHengHdlDual : CameraHdlBase
    {
        object lockobj = new object();
        DaHengHdl hdl1;
        DaHengHdl hdl2;
        ManualResetEvent rev1 = new ManualResetEvent(false);
        ManualResetEvent rev2 = new ManualResetEvent(false);
        internal DaHengHdlDual(int index = 0)
        {
            hdl1 = new DaHengHdl(1);
            hdl2 = new DaHengHdl(2);
        }
        ICogImage TryReconnect()
        {
            if ((DateTime.Now - ReconnectTime).TotalSeconds > 10)
            {
                ReconnectTime = DateTime.Now;


                Init();
            }
            return null;
        }
        ICogImage img1;
        ICogImage img2;
        void grab1(object state)
        {
            try
            {
                img1 = hdl1.GetImage();
            }
            catch (Exception ex)
            {
                Program.ErrHdl(ex);
            }
            rev1.Set();
        }
        void grab2(object state)
        {
            try
            {
                img2 = hdl2.GetImage();
            }
            catch (Exception ex)
            {
                Program.ErrHdl(ex);
            }
            rev2.Set();
        }
        internal override ICogImage GetImage(int discardct = 3)
        {
            try
            {
                lock (lockobj)
                {
                    img1 = null;
                    img2 = null;
                    rev1.Reset();
                    rev2.Reset();
                    ThreadPool.QueueUserWorkItem(grab1);
                    ThreadPool.QueueUserWorkItem(grab2);
                    rev1.WaitOne(5000);
                    rev2.WaitOne(5000);

                    if (img1 == null)
                    {
                        if (img2 == null)
                            Program.MsgBox("相机采像失败");
                        else
                            Program.MsgBox("左相机采像失败");
                        return img2;
                    }
                    else if (img2 == null)
                    {
                        Program.MsgBox("右相机采像失败");
                        return img1;
                    }
                    else
                        return GenImg(img1, img2);
                }
            }
            catch (Exception exception)
            {
                Program.ErrHdl(exception);
            }

            return TryReconnect();
        }
        DateTime ReconnectTime = DateTime.MinValue;
        internal override void Init()
        {
            try
            {
                hdl1.Init();
                hdl2.Init();
            }
            catch (Exception ex)
            {
                if (ex != null && string.IsNullOrEmpty(ex.Message) == false && ex.Message.StartsWith("{Access denied:{-1005}"))
                    Program.ErrHdl("相机无法连接：相机线必须插在蓝色USB3.0的接口上");
                else
                    Program.ErrHdl(ex);
            }

        }
        internal override double ExposureTime
        {
            get
            {
                if (hdl1.ExposureTime != hdl2.ExposureTime)
                    hdl2.ExposureTime = hdl1.ExposureTime;
                return hdl1.ExposureTime;
            }
            set
            {
                if (value > 0)
                {
                    hdl1.ExposureTime = value;
                    hdl2.ExposureTime = value;
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
                if (value > 0)
                {

                }
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
                if (value > 0)
                {

                }
            }
        }
        internal override void Dispose()
        {
            hdl1.Dispose();
            hdl2.Dispose();
        }
        internal override bool Connected
        {
            get
            {
                return hdl1.Connected && hdl2.Connected;
            }
        }

        ICogImage GenImg(ICogImage imgl, ICogImage imgr)
        {
            if (imgl != null && imgr != null)
            {
                Cognex.VisionPro.ImageProcessing.CogCopyRegion ccr = new Cognex.VisionPro.ImageProcessing.CogCopyRegion();
                ICogImage imgdst = new CogImage24PlanarColor(imgl.Width + imgr.Width, Math.Max(imgl.Height, imgr.Height));

                CogRectangle region = new CogRectangle();
                region.X = 0;
                region.Y = 0;
                region.Width = imgl.Width;
                region.Height = imgl.Height;
                ccr.ImageAlignmentEnabled = true;

                bool sourceClipped, destinationClipped;
                ICogRegion destinationRegion;
                ccr.Execute(imgl, region, imgdst, out sourceClipped, out destinationClipped, out destinationRegion);

                region.Width = imgr.Width;
                region.Height = imgr.Height;
                ccr.DestinationImageAlignmentX = imgl.Width;
                ccr.Execute(imgr, region, imgdst, out sourceClipped, out destinationClipped, out destinationRegion);
                return imgdst;
            }
            return null;
        }

    }
}
