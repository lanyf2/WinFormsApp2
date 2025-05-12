using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using vpc;
using Cognex.VisionPro.FGGigE;
using Cognex.VisionPro.ToolBlock;
using System.Threading;

namespace Cognex.VisionPro
{
    public abstract class BlockBase : ICloneable
    {
        const string VIDEO_FORMAT = "Generic GigEVision (Mono)";
        const string VIDEO_FORMAT_NullFormat = "Cognex NullFormat";

        internal static int HisBufferCapacity = 5;
        internal CameraHdlBase camUsb = null;
        internal CogToolBlock internalblock;
        protected Thread thread;
        DateTime dispTime = DateTime.MinValue;
        internal BlockBase()
        {
            thread = new Thread(RunThread);
        }

        internal void CamUsb_ImageGrabbed(ICogImage arg1, int arg2)
        {
            if (FormPLC.ssFlag)
            {
                if ((DateTime.Now - dispTime).TotalSeconds > 0.1)
                {
                    //Program.mf.BeginInvoke(new Action<ICogImage, int>(Program.mf.UpdateDisplay), arg1, blockid);
                    dispTime = DateTime.Now;
                    FormDispImg.dispForm?.UpdateDisplay(arg1, arg2);
                }
            }
            else
            {
                if (Program.mf.DisplayingResult == null)
                    Program.mf.BeginInvoke(new Action<RxInterface, int>(Program.mf.UpdateGraphics), null, blockid);
                if (Settings.Default.相机停用?.Length > blockid)
                    if (Settings.Default.相机停用[blockid] == true)
                        return;
                RunAsync(arg1, 0);
            }
        }

        internal int sleepBeforeAcquireImage = 0;
        internal int blockid;
        internal int PassCount = 0;
        internal int FailCount = 0;
        internal int TotalCount
        {
            get { return PassCount + FailCount; }
        }
        internal double FailRate
        {
            get
            {
                if (TotalCount == 0)
                    return 0;
                else
                    return (double)FailCount / (PassCount + FailCount);
            }
        }
        internal string FailRateStr
        {
            get
            {
                double rate = FailRate * 100;

                if (rate < 10)
                    return string.Format("{0:F2}%", rate);
                else if (rate < 100)
                    return string.Format("{0:F1}%", rate);
                else
                    return string.Format("{0:F0}%", rate);
            }
        }
        static internal EventHandler InfoChanged;
        static internal Action<BlockBase> ResultChanged;
        static internal EventHandler ImageGrabbed;
        internal ICogAcqFifo fifo;
        internal static EventHandler FifoChanged;
        internal double ImageGrabTime, AlgTime;
        static internal void InfoChangedHdl(string info)
        {
            if (InfoChanged != null)
                InfoChanged(info, EventArgs.Empty);
        }
        static internal void WaitHdl(int wait, string info = "等待检测")
        {
            if (wait > 0)
                if (InfoChanged != null)
                    for (int i = 0; i < wait; i += 1000)
                    {
                        if (wait - i >= 1000)
                        {
                            if (wait == 1001)
                                InfoChanged(info, EventArgs.Empty);
                            else
                                InfoChanged(string.Format("{1} {0}", (wait - i) / 1000, info), EventArgs.Empty);
                            Thread.Sleep(1000);
                        }
                        else
                            Thread.Sleep(wait - i);
                    }
                else
                    Thread.Sleep(wait);
        }

        DateTime reconTime = DateTime.Now;
        internal void TryReconnect()
        {
            try
            {
                if (Camera != null)
                {
                    if ((DateTime.Now - reconTime).TotalSeconds > 10)
                    {
                        reconTime = DateTime.Now;
                        Program.Loginfo(Camera.SerialNumber + " 尝试重连");
                        Camera.Disconnect(true);
                    }
                }
            }
            catch (Exception ex)
            {
                Program.Loginfo(ex.Message);
            }
        }
        internal R1Class lastResult;
        //ICogImage runImg = null;
        internal System.Collections.Concurrent.ConcurrentQueue<ICogImage> imgQ = new System.Collections.Concurrent.ConcurrentQueue<ICogImage>();
        bool runFlag = false;
        internal R1Class RunSync(ICogImage img)
        {
            int trigid;
            ImageGrabTime = AlgTime = 0;
            CogStopwatch watch = new CogStopwatch();
            watch.Start();
            if (img == null)
            {
                try
                {
                    if (CameraHdlBase.extTrigFlag == false)
                    {
                        SyncExposureTime();
                        WaitHdl(sleepBeforeAcquireImage);
                        if (fifo != null)
                        {
                            img = fifo.Acquire(out trigid);
                        }
                        else
                        {
                            img = camUsb.GetImage();
                        }
                    }
                    ImageGrabbed?.Invoke(this, EventArgs.Empty);
                }
                catch (Exceptions.CogAcqAbnormalException)
                {
                    TryReconnect();
                }
                catch (Exceptions.CogAcqTimingException)
                {
                    TryReconnect();
                }
                catch (Exception ex)
                {
                    Program.ErrHdl("Err b14257:" + ex.Message);
                }
            }
            ImageGrabTime = watch.Milliseconds - sleepBeforeAcquireImage;
            internalblock.Inputs[0].Value = img;
            watch.Reset();
            internalblock.Run();
            object[] re = new object[internalblock.Outputs.Count];
            for (int i = 0; i < re.Length; i++)
            {
                re[i] = internalblock.Outputs[i].Value;
            }
            if (img == null)
            {
                re[0] = null;
                re[1] = false;
                re[2] = "相机未连接或采像失败";
                Thread.Sleep(500);
            }
            else if (re[0] == null)
            {
                re[0] = img;
                //re[2] = internalblock.RunStatus.Exception.Message;
            }
            string errmsg = internalblock.RunStatus.Message;
            if (internalblock.RunStatus.Exception != null)
            {
                errmsg = internalblock.RunStatus.Exception.Message;
                re[1] = false;
            }
            if (!string.IsNullOrEmpty(errmsg))
                if (string.IsNullOrEmpty(re[2] as string))
                    re[2] = errmsg;
                else
                {
                    if (errmsg.IndexOf("安全性冲突") >= 0)
                    {
                        re[2] = "内部错误25，请尝试重启软件";
                        JobManager.initflag = false;
                    }
                    else
                        re[2] = string.Format("{0}_{1}", re[2], errmsg);
                    re[1] = false;
                }

            AlgTime = watch.Milliseconds;

            if ((bool)re[1])
                PassCount++;
            else
                FailCount++;
            var lastResult = GenResult(re, ImageGrabTime, AlgTime, null);
            return lastResult;
        }
        internal void RunAsync(ICogImage img = null, int sleepTime = 0)
        {
            sleepBeforeAcquireImage = sleepTime;
            //if (img == null && fifo == null)
            //    Program.ErrHdl(new InvalidOperationException("相机未连接"));
            //else
            {
                //if (runFlag)
                //    Program.Loginfo("产品可能过密：" + blockid);
                //runImg = img;
                imgQ.Enqueue(img);
                waithdl.Set();
            }
        }
        internal abstract R1Class GenResult(object[] re, double ImageGrabTime, double AlgTime, ICogRecord record);

        public object Clone()
        {
            throw new NotImplementedException();
        }
        internal void Dispose()
        {
            thread.Abort();
            if (camUsb != null)
                camUsb.Dispose();
        }
        AutoResetEvent waithdl = new AutoResetEvent(false);
        void RunThread()
        {
            while (true)
            {
                try
                {
                    waithdl.WaitOne();
                    runFlag = true;
                    while (imgQ.TryDequeue(out ICogImage img))
                    {
                        lastResult = RunSync(img);
                        //runImg = null;
                        ResultChanged?.Invoke(this);
                    }
                    runFlag = false;
                }
                catch (ThreadAbortException)
                {
                    return;
                }
                catch (Exception ex)
                {
                    Program.ErrHdl(ex);
                }
            }
        }
        internal abstract void SaveMtd();
        internal void SyncExposureTime()
        {
            if (fifo != null)
            {
                if (ExposureTime != fifo.OwnedExposureParams.Exposure)
                    fifo.OwnedExposureParams.Exposure = ExposureTime;
            }
            else
            {
                if (Math.Abs(ExposureTime3 - ExposureTime) > 0.01)
                    ExposureTime3 = ExposureTime;
            }
        }
        #region Properties
        [DisplayName("相机"), TypeConverter(typeof(MyEnumConverter))]
        public ICogFrameGrabber Camera
        {
            get
            {
                if (fifo == null)
                    return null;
                else
                    return fifo.FrameGrabber;
            }
            set
            {
                //camera = value;
                if (value != null)
                {
                    try
                    {
                        if (value is Cognex.VisionPro.FGGigE.Implementation.Internal.CogFrameGrabberGigE)
                            fifo = value.CreateAcqFifo(VIDEO_FORMAT, CogAcqFifoPixelFormatConstants.Format8Grey, 0, true);
                        //fifo = value.CreateAcqFifo(VIDEO_FORMAT, CogAcqFifoPixelFormatConstants.Format8Grey, 0, true);
                        else
                            fifo = value.CreateAcqFifo(VIDEO_FORMAT_NullFormat, CogAcqFifoPixelFormatConstants.Format8Grey, 0, true);
                        //fifo.OwnedROIParams.SetROIXYWidthHeight(0, 700, 4608, 2100);
                        fifo.OwnedROIParams.SetROIXYWidthHeight(0, 0, 4608, 9999);
                        fifo.AutoPrepareEnabled = true;
                    }
                    catch (Cognex.VisionPro.Exceptions.CogAcqHardwareInUseException)
                    {
                        fifo = null;
                        throw new Exception("相机初始化失败，相机可能被其他程序占用");//可能由于最近一次的异常退出，需重新插拔相机的网线
                    }
                }
                else
                    fifo = null;
                if (FifoChanged != null)
                    FifoChanged(this, EventArgs.Empty);
            }
        }
        [DisplayName("曝光时间设定")]
        public virtual double ExposureTime
        {
            get
            {
                if (internalblock.Inputs.Contains("ExposureTime"))
                    return (double)internalblock.Inputs["ExposureTime"].Value;
                else if (fifo == null)
                    return double.NaN;
                else
                    return fifo.OwnedExposureParams.Exposure;
            }
            set
            {
                if (internalblock.Inputs.Contains("ExposureTime"))
                    internalblock.Inputs["ExposureTime"].Value = value;
                if (fifo != null)
                    fifo.OwnedExposureParams.Exposure = value;
            }
        }
        [DisplayName("实际曝光时间")]
        public double ExposureTime3
        {
            get
            {
                return camUsb.ExposureTime;
            }
            set
            {
                camUsb.ExposureTime = value;
            }
        }
        [DisplayName("增益")]
        public double Gain
        {
            get
            {
                return camUsb.Gain;
            }
            set
            {
                camUsb.Gain = value;
            }
        }
        [DisplayName("相机名称")]
        public string CamName
        {
            get
            {
                if (camUsb != null)
                    return camUsb.CameraName;
                else return null;
            }
        }
        [DisplayName("Brightness")]
        internal double Brightness
        {
            get
            {
                return camUsb.Brightness;
            }
            set
            {
                camUsb.Brightness = value;
            }
        }
        [Browsable(false)]
        internal ICogImage LastImg
        {
            get
            {
                return lastResult.img;
            }
        }
        [Browsable(false)]
        internal bool Passed
        {
            get
            {
                return lastResult.Passed;
            }
        }
        [Browsable(false)]
        internal string SerialNumber
        {
            get
            {
                if (fifo == null || fifo.FrameGrabber == null)
                    return null;
                else
                    return fifo.FrameGrabber.SerialNumber;
            }
        }
        #endregion
    }

    #region Result Display
    public abstract class RxInterface
    {
        protected ICogRecord record;
        protected const string folder = "img";
        protected DateTime tm;
        public object Tag;
        protected object[] re;
        public abstract ICogImage img { get; }
        public abstract string Info { get; }
        [DisplayName("记录时间"), TypeConverter(typeof(MyStrConverter))]
        public DateTime LogTime { get { return tm; } }
        internal virtual bool Passed
        {
            get
            {
                if (re != null)
                    return (bool)re[1];
                return false;
            }
        }
        internal ICogRecord Record { get; }
        internal bool SaveFlag { get; set; }
        internal virtual string SaveImg(bool Failed, string path = null)
        {
            if (path == null)
            {
                path = string.Format(@"{0}\{1:M-d}_{2}\{1:HHmmss}\", folder, LogTime, Failed ? "次品" : "合格品");
                System.IO.Directory.CreateDirectory(path);
            }
            return path;
        }
        internal ResultStruct parent;
        internal List<object[]> ResultGraphics
        {
            get
            {
                if (re == null)
                    return null;
                return re[3] as List<object[]>;
            }
        }
    }
    public class ResultStruct : ICustomTypeDescriptor
    {
        public static ResultStruct EmptyResult = new ResultStruct();
        static int InternalIndex = 0;
        public int index;
        R1Class[] rarr;
        public DateTime? iotime;
        internal R1Class this[int index]
        {
            get
            {
                try
                {
                    if (index < rarr?.Length)
                        return rarr[index];
                    else
                        return null;
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                if (index < rarr?.Length)
                {
                    rarr[index] = value;
                    if (rarr[index] != null)
                        rarr[index].parent = this;
                }
            }
        }
        internal bool Failed
        {
            get
            {
                if (string.IsNullOrEmpty(ErrorInfo) == false)
                    return true;
                for (int i = 0; i < rarr?.Length; i++)
                {
                    if (Settings.Default.相机停用?.Length > i)
                        if (Settings.Default.相机停用[i] == true)
                            continue;
                    if (rarr[i] != null && false == rarr[i].Passed)
                        return true;
                }
                if (string.IsNullOrEmpty(Settings.Default.DIDBuildString) == false && DIDCheckPassed == false && LastNullId < 0)
                    return true;
                return false;
            }
        }
        internal bool Passed
        {
            get
            {
                if (string.IsNullOrEmpty(ErrorInfo) == false)
                    return false;
                for (int i = 0; i < rarr?.Length; i++)
                {
                    if (Settings.Default.相机停用?.Length > i)
                        if (Settings.Default.相机停用[i] == true)
                            continue;
                    if (rarr[i] == null || false == rarr[i].Passed)
                        return false;
                }
                return true;
            }
        }
        internal string Info
        {
            get
            {
                if (string.IsNullOrEmpty(ErrorInfo) == false)
                    return ErrorInfo;
                bool nullflag = false;
                for (int i = 0; i < rarr?.Length; i++)
                {
                    if (rarr[i] == null)
                        nullflag = true;
                    else if (!rarr[i].Passed)
                        return "次品";
                }
                if (nullflag)
                    return "检测中";
                else if (string.IsNullOrEmpty(Settings.Default.DIDBuildString) == false && DIDCheckPassed == false)
                    return "DID写入失败";
                else
                    return "合格品";
            }
        }
        internal int Count
        {
            get
            {
                if (JobManager.MtdBlocks == null)
                    return 1;
                else
                    return JobManager.MtdBlocks.Length;
            }
        }
        internal ResultStruct()
        {
            rarr = new R1Class[Count];
            index = InternalIndex++;
        }
        internal int LastFailedId
        {
            get
            {
                for (int i = rarr.Length - 1; i >= 0; i--)
                {
                    if (rarr[i] != null && rarr[i].Passed == false)
                        return i;
                }
                return rarr.Length - 1;
            }
        }
        internal int LastId
        {
            get
            {
                for (int i = rarr.Length - 1; i >= 0; i--)
                {
                    if (rarr[i] != null)
                        return i;
                }
                return 0;
            }
        }
        internal int LastNullId
        {
            get
            {
                for (int i = rarr.Length - 1; i >= 0; i--)
                {
                    if (rarr[i] == null)
                        return i;
                }
                return -1;
            }
        }
        //internal string MTC;
        internal string DID;
        internal string ErrorInfo;
        internal string[] ExtraInfo;
        internal bool DIDCheckPassed = false;
        internal void Clear()
        {
            if (rarr != null)
                rarr = new R1Class[rarr.Length];
        }

        #region ICustomTypeDescriptor 成员
        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }
        public string GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }
        public string GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }
        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }
        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }
        public PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, true);
        }
        public object GetEditor(System.Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }
        public EventDescriptorCollection GetEvents(System.Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }
        public EventDescriptorCollection GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }
        public PropertyDescriptorCollection GetProperties()
        {
            return TypeDescriptor.GetProperties(this, true);
        }
        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }
        #endregion
        public virtual PropertyDescriptorCollection GetProperties(System.Attribute[] attributes)
        {
            PropertyDescriptorCollection pdc = System.ComponentModel.TypeDescriptor.GetProperties(this);

            List<PropertyDescriptor> props = new List<PropertyDescriptor>();
            //  new PropertyDescriptor[ResultGraphics.Count + pdc.Count];
            for (int i = 0; i < pdc.Count; i++)
                props.Add(pdc[i]);
            if (iotime != null)
            {
                //props.Add(new XPropDescriptor(new XProp("IO输出时间", iotime.Value.ToString("HH:MM:ss.fff"), null, null, new MyCogToolConverter()), attributes));
            }
            double tm = 0;
            if (rarr != null)
                for (int i = 0; i < rarr.Length; i++)
                {
                    var val = rarr[i];
                    if (val == null)
                        props.Add(new XPropDescriptor(new XProp($"相机{i + 1}", "空", null, null, new MyCogToolConverter()), attributes));
                    else
                    {
                        props.Add(new XPropDescriptor(new XProp($"相机{i + 1}", val, null, null, new ExpandableObjectConverter()), attributes));
                        if (val.AlgTime > tm)
                            tm = val.AlgTime;
                    }
                }
            props.Insert(0, new XPropDescriptor(new XProp($"最长算法耗时", tm.ToString("0.#")), attributes));
            return new PropertyDescriptorCollection(props.ToArray());
        }
    }
    #endregion

    #region CamerasGige
    internal static class CamerasGige
    {
        internal static CogFrameGrabbers cameras = new CogFrameGrabbers();
        internal static ICogFrameGrabber GetCamera(string SerialNumber)
        {
            for (int i = 0; i < cameras.Count; i++)
            {
                if (cameras[i].SerialNumber == SerialNumber)
                {
                    return cameras[i];
                }
            }
            return null;
        }
        internal static void Dispose()
        {
            if (cameras != null)
                for (int i = 0; i < cameras.Count; i++)
                {
                    cameras[i].Disconnect(false);
                }
        }
    }
    #endregion

}
