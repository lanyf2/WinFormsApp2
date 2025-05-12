using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cognex.VisionPro;
using ThridLibray;
using System.Threading;

namespace vpc
{
    internal class DaHuaHdl : CameraHdlBase
    {
        private IDevice m_dev;
        static List<IDeviceInfo> li;
        IFloatParameter pExposureTime;
        AutoResetEvent waithdl = new AutoResetEvent(false);
        CogImage24PlanarColor ImgResult;
        object lockobj = new object();

        internal DaHuaHdl(int index = 0)
        {
            camindex = index;
        }
        ICogImage TryReconnect()
        {
            if ((DateTime.Now - ReconnectTime).TotalSeconds > 10)
            {
                ReconnectTime = DateTime.Now;
                if (m_dev != null)
                {
                    m_dev.ShutdownGrab();
                    m_dev.Close();
                    m_dev.StreamGrabber.ImageGrabbed -= OnImageGrabbed;
                    m_dev = null;
                    pExposureTime = null;
                }
                Init();
            }
            return null;
        }
        internal override ICogImage GetImage(int discardct = 3)
        {
            try
            {
                if (m_dev == null)
                    TryReconnect();
                if (m_dev == null)
                    return null;
                lock (lockobj)
                {
                    ImgResult = null;
                    int tm = (int)(CamReady - DateTime.Now).TotalMilliseconds;
                    if (tm > 0)
                        Thread.Sleep(tm);
                    waithdl.Reset();
                    m_dev.ExecuteSoftwareTrigger();
                    if (waithdl.WaitOne(3000))
                    {
                        return ImgResult;
                    }
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
        DateTime CamReady = DateTime.MinValue;
        internal override void Init()
        {
            try
            {
                li = Enumerator.EnumerateDevices();
                // 设备搜索
                if (li.Count > 0)
                {
                    // 获取搜索到的第一个设备
                    m_dev = Enumerator.GetDeviceByIndex(0);
                    // 注册链接时间

                    // 打开设备
                    if (!m_dev.Open())
                    {
                        return;
                    }

                    // 打开Software Trigger
                    m_dev.TriggerSet.Open(TriggerSourceEnum.Software);

                    // 设置图像格式
                    //using (IEnumParameter p = m_dev.ParameterCollection[ParametrizeNameSet.ImagePixelFormat])
                    //{
                    //    p.SetValue("BayerRG8");
                    //}
                    //using (IEnumParameter p = m_dev.ParameterCollection[ParametrizeNameSet.BalanceRatioSelector])
                    //using (IFloatParameter p2 = m_dev.ParameterCollection[ParametrizeNameSet.BalanceRatio])
                    //{
                    //    p.SetValue("RED");
                    //    p2.SetValue(2.18459);
                    //    p.SetValue("Blue");
                    //    p2.SetValue(1.52058);
                    //}

                    //using (IStringParameter p = m_dev.ParameterCollection[ParametrizeNameSet.DeviceUserID])
                    //{
                    //    var str = p.GetValue();
                    //}

                    pExposureTime = m_dev.ParameterCollection[ParametrizeNameSet.ExposureTime];

                    m_dev.StreamGrabber.ImageGrabbed += OnImageGrabbed;
                    if (!m_dev.GrabUsingGrabLoopThread())
                    {
                        Program.ErrHdl(@"相机打开错误2");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Program.ErrHdl(ex);
            }

        }
        internal override double ExposureTime
        {
            get
            {
                if (pExposureTime != null)
                    return pExposureTime.GetValue() / 1000;
                return -1;
            }
            set
            {
                if (value > 0 && pExposureTime != null)
                {
                    pExposureTime.SetValue(value * 1000);
                    CamReady = DateTime.Now.AddMilliseconds(value + 50);
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
            if (m_dev != null)
            {
                m_dev.ShutdownGrab();
                m_dev.Dispose();
                m_dev = null;
            }
        }
        internal override bool Connected
        {
            get
            {
                if (m_dev != null)
                    return m_dev.IsOpen;
                return false;
            }
        }
        private void OnImageGrabbed(Object sender, GrabbedEventArgs e)
        {
            ImgResult = new CogImage24PlanarColor(e.GrabResult.ToBitmap(true));
            waithdl.Set();
        }
    }
}
