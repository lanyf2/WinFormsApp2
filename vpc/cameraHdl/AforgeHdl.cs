using AForge.Video;
using AForge.Video.DirectShow;
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
    internal class AforgeHdl : CameraHdlBase
    {
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource;
        public int selectedDeviceIndex = 0;
        ManualResetEvent waithdl = new ManualResetEvent(false);
        CogImage24PlanarColor globalimg;
        object lockobj = new object();
        internal AforgeHdl(int index = 0)
        {
            selectedDeviceIndex = index;
        }
        void videoSource_NewFrame(object sender, AForge.Video.NewFrameEventArgs eventArgs)
        {
            try
            {
                using (Bitmap bmp = (Bitmap)eventArgs.Frame.Clone())
                {
                    globalimg = new CogImage24PlanarColor(bmp);
                }
                videoSource.NewFrame -= new NewFrameEventHandler(videoSource_NewFrame);
            }
            catch (Exception ex)
            {
                Program.ErrHdl(ex);
            }
            waithdl.Set();
        }
        internal override ICogImage GetImage(int discardct = 3)
        {
            if (videoSource == null)
                return null;
            lock (lockobj)
            {
                globalimg = null;
                waithdl.Reset();
                videoSource.NewFrame += new NewFrameEventHandler(videoSource_NewFrame);
                if (waithdl.WaitOne(2500))
                {
                    return globalimg;
                }
            }
            return null;
        }
        internal override void Init()
        {
            try
            {
                //枚举所有视频输入设备  
                videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                if (videoDevices.Count > 0)
                {
                    videoSource = new VideoCaptureDevice(videoDevices[selectedDeviceIndex].MonikerString);
                    //videoSource.SetCameraProperty(CameraControlProperty.Exposure, 100, CameraControlFlags.Manual);
                    videoSource.Start();
                }
            }
            catch (Exception ex)
            {
                if (selectedDeviceIndex > 0 && videoDevices.Count <= 1)
                {
                    videoSource = null;
                    Program.ErrHdl(ex);
                }
                else
                {
                    try
                    {
                        videoSource = new VideoCaptureDevice(videoDevices[1].MonikerString);
                        videoSource.Start();
                    }
                    catch (Exception exx)
                    {
                        videoSource = null;
                        Program.ErrHdl(exx);
                    }
                }
            }
        }
        internal override double ExposureTime
        {
            get
            {
                return -4;
#pragma warning disable CS0162
                if (videoSource != null)
                {
                    int val;
                    CameraControlFlags ctrl;
                    videoSource.GetCameraProperty(CameraControlProperty.Exposure, out val, out ctrl);
                    return val;
                }
                return -2;
            }
            set
            {
                return;
                if (videoSource != null)
                {
                    int val = (int)value;
                    videoSource.SetCameraProperty(CameraControlProperty.Exposure, val, CameraControlFlags.Manual);
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
            if (videoSource != null)
                videoSource.Stop();
        }
        internal override bool Connected
        {
            get
            {
                if (videoDevices == null || videoDevices.Count <= 0)
                    return false;
                return true;
            }
        }
    }
}
