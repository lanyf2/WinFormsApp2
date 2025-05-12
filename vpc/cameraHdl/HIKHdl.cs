using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cognex.VisionPro;
using System.Threading;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;
using MvCamCtrl.NET;

namespace vpc
{
    internal class HIKHdl : CameraHdlBase
    {
        internal HIKHdl(int index = 0)
        {
            camindex = index;
        }
        ICogImage TryReconnect()
        {
            if ((DateTime.Now - ReconnectTime).TotalSeconds > 10)
            {
                ReconnectTime = DateTime.Now;

                Init();
                try
                {
                    if (m_pOperator != null)
                    {
                        lock (lockobj)
                        {
                            ImgResult = null;
                            waithdl.Reset();
                            m_pOperator.TriggerSoftware();
                            if (waithdl.WaitOne(4000))
                            {
                                return ImgResult;
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    Program.ErrHdl(exception);
                }
            }
            return null;
        }
        internal override ICogImage GetImage(int discardct = 3)
        {
            try
            {
                if (m_pOperator != null)
                {
                    if (extTrigFlag)
                        return null;
                    lock (lockobj)
                    {
                        ImgResult = null;
                        waithdl.Reset();
                        m_pOperator.TriggerSoftware();
                        if (waithdl.WaitOne(4000))
                        {
                            return ImgResult;
                        }
                    }
                }
                return TryReconnect();
            }
            catch (Exception exception)
            {
                Program.ErrHdl(exception);
            }
            return TryReconnect();
        }
        DateTime ReconnectTime = DateTime.MinValue;
        MyCamera.cbOutputdelegate ImageCallback;
        ManualResetEvent waithdl = new ManualResetEvent(false);
        object lockobj = new object();
        ICogImage ImgResult;
        uint m_nBufSizeForSaveImage = 2600 * 1952 * 3 + 2048;
        byte[] m_pBufForSaveImage = new byte[2600 * 1952 * 3 + 2048];
        string DeviceInfo;

        internal override void SetTrigMode(bool mode)
        {
            if (m_pOperator != null)
            {
                if (mode)
                    m_pOperator.SetEnumValue("TriggerMode", (int)MyCamera.MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_ON);
                else
                    m_pOperator.SetEnumValue("TriggerMode", (int)MyCamera.MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_OFF);
            }
        }
        internal override void Init()
        {
            try
            {
                CameraOperator.DeviceListAcq();
                if (CameraOperator.m_pDeviceList.nDeviceNum > 0)
                {
                    int nRet = -1;
                    if (m_pOperator != null)
                    {
                        m_pOperator.StopGrabbing();
                        m_pOperator.Close();
                        m_pOperator = null;
                    }

                    CameraOperator m_pOp = new CameraOperator();

                    //获取选择的设备信息
                    MyCamera.MV_CC_DEVICE_INFO device =
                        (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(CameraOperator.m_pDeviceList.pDeviceInfo[0],
                                                                      typeof(MyCamera.MV_CC_DEVICE_INFO));
                    string si = null;
                    if (camindex > 0)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine($"总{CameraOperator.m_pDeviceList.nDeviceNum}");
                        bool flag = false;
                        for (int i = 0; i < CameraOperator.m_pDeviceList.nDeviceNum; i++)
                        {
                            MyCamera.MV_CC_DEVICE_INFO dv =
                                (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(CameraOperator.m_pDeviceList.pDeviceInfo[i],
                                                                              typeof(MyCamera.MV_CC_DEVICE_INFO));
                            if (dv.nTLayerType == MyCamera.MV_GIGE_DEVICE)
                            {
                                IntPtr bf = Marshal.UnsafeAddrOfPinnedArrayElement(dv.SpecialInfo.stGigEInfo, 0);
                                MyCamera.MV_GIGE_DEVICE_INFO gigeInfo = (MyCamera.MV_GIGE_DEVICE_INFO)Marshal.PtrToStructure(bf, typeof(MyCamera.MV_GIGE_DEVICE_INFO));
                                if (gigeInfo.chUserDefinedName == camindex.ToString())
                                {
                                    device = dv;
                                    si = gigeInfo.chSerialNumber;
                                    flag = true;
                                    break;
                                }
                                uint ip = gigeInfo.nCurrentIp;
                                var info = $"{(ip >> 24) & 255}.{(ip >> 16) & 255}.{(ip >> 8) & 255}.{(ip) & 255}({gigeInfo.chSerialNumber})";
                                sb.AppendLine($"{i}GIGE:{gigeInfo.chUserDefinedName}_{info}");
                            }
                            else
                            {
                                IntPtr bf = Marshal.UnsafeAddrOfPinnedArrayElement(dv.SpecialInfo.stUsb3VInfo, 0);
                                //MyCamera.MV_GIGE_DEVICE_INFO
                                MyCamera.MV_USB3_DEVICE_INFO uifo = (MyCamera.MV_USB3_DEVICE_INFO)Marshal.PtrToStructure(bf, typeof(MyCamera.MV_USB3_DEVICE_INFO));
                                if (uifo.chUserDefinedName == camindex.ToString())
                                {
                                    device = dv;
                                    si = uifo.chSerialNumber;
                                    flag = true;
                                    break;
                                }
                                sb.AppendLine($"{i}USB3:{uifo.chUserDefinedName}_{uifo.chSerialNumber}");
                            }
                        }
                        if (flag)
                        {
                            if (snset.Contains(si) == false)
                            {
                                flag = false;
                                return;
                            }
                        }
                        if (flag == false)
                        {
                            if (Settings.Default.test)
                                Program.MsgBox("无法找到相机：" + camindex + "\r\n" + sb.ToString());
                            return;
                        }
                    }
                    nRet = m_pOp.Open(ref device);
                    if (MyCamera.MV_OK != nRet)
                    {
                        Program.ErrHdl(GetErrInfo("HIK设备打开失败!", nRet));
                        return;
                    }
                    //MyCamera.MV_GIGE_DEVICE_INFO gigeinfo;

                    if (device.nTLayerType == MyCamera.MV_GIGE_DEVICE)
                    {
                        IntPtr buffer = Marshal.UnsafeAddrOfPinnedArrayElement(device.SpecialInfo.stGigEInfo, 0);
                        MyCamera.MV_GIGE_DEVICE_INFO usbInfo = (MyCamera.MV_GIGE_DEVICE_INFO)Marshal.PtrToStructure(buffer, typeof(MyCamera.MV_GIGE_DEVICE_INFO));
                        uint ip = usbInfo.nCurrentIp;
                        DeviceInfo = $"{(ip >> 24) & 255}.{(ip >> 16) & 255}.{(ip >> 8) & 255}.{(ip) & 255}_{usbInfo.chManufacturerName} {usbInfo.chModelName} ({usbInfo.chSerialNumber})";
                        m_nBufSizeForSaveImage = 5472 * 3648 * 3 + 2048;
                         m_pBufForSaveImage = new byte[m_nBufSizeForSaveImage];
                    }
                    else
                    {
                        IntPtr buffer = Marshal.UnsafeAddrOfPinnedArrayElement(device.SpecialInfo.stUsb3VInfo, 0);
                        MyCamera.MV_USB3_DEVICE_INFO usbInfo = (MyCamera.MV_USB3_DEVICE_INFO)Marshal.PtrToStructure(buffer, typeof(MyCamera.MV_USB3_DEVICE_INFO));
                        DeviceInfo = usbInfo.chManufacturerName + " " + usbInfo.chModelName + "(" + usbInfo.chSerialNumber + ")";
                    }
                    if (extTrigFlag)
                        m_pOp.SetEnumValue("TriggerSource", (int)MyCamera.MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_LINE0);
                    else
                        m_pOp.SetEnumValue("TriggerSource", (int)MyCamera.MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_SOFTWARE);
                    m_pOp.SetEnumValue("TriggerMode", (int)MyCamera.MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_ON);
                    m_pOp.SetEnumValue("AcquisitionMode", (int)MyCamera.MV_CAM_ACQUISITION_MODE.MV_ACQ_MODE_CONTINUOUS);
                    m_pOp.SetEnumValue("ExposureAuto", (int)MyCamera.MV_CAM_EXPOSURE_AUTO_MODE.MV_EXPOSURE_AUTO_MODE_OFF);

                    //注册回调函数
                    if (ImageCallback == null)
                        ImageCallback = new MyCamera.cbOutputdelegate(SaveImage);
                    nRet = m_pOp.RegisterImageCallBack(ImageCallback, IntPtr.Zero);
                    if (CameraOperator.CO_OK != nRet)
                    {
                        Program.ErrHdl("HIK注册回调失败!");
                        return;
                    }
                    nRet = m_pOp.RegisterExceptionCallBack(new MyCamera.cbExceptiondelegate(cbException), IntPtr.Zero);
                    if (CameraOperator.CO_OK != nRet)
                        Program.ErrHdl("HIK注册ExceptionCallBack回调失败!");
                    m_pOp.StartGrabbing();

                    m_pOperator = m_pOp;
                }
            }
            catch (Exception ex)
            {
                Program.ErrHdl(ex);
            }
        }
        private void cbException(uint nMsgType, IntPtr pUser)
        {
            if (nMsgType == MyCamera.MV_EXCEPTION_DEV_DISCONNECT)
            {
                Program.ErrHdl($"相机{camindex} ERR：MV_EXCEPTION_DEV_DISCONNECT {pUser.ToInt64()}");
            }
            else
                Program.Loginfo($"相机{camindex} ERR：{nMsgType} {pUser.ToInt64()}");
        }
        static string GetErrInfo(string csMessage, int nErrorNum)
        {
            string errorMsg;
            if (nErrorNum == 0)
            {
                errorMsg = csMessage;
            }
            else
            {
                errorMsg = csMessage + ": Error =" + String.Format("{0:X}", nErrorNum);
            }

            switch (nErrorNum)
            {
                case MyCamera.MV_E_HANDLE: errorMsg += " Error or invalid handle "; break;
                case MyCamera.MV_E_SUPPORT: errorMsg += " Not supported function "; break;
                case MyCamera.MV_E_BUFOVER: errorMsg += " Cache is full "; break;
                case MyCamera.MV_E_CALLORDER: errorMsg += " Function calling order error "; break;
                case MyCamera.MV_E_PARAMETER: errorMsg += " Incorrect parameter "; break;
                case MyCamera.MV_E_RESOURCE: errorMsg += " Applying resource failed "; break;
                case MyCamera.MV_E_NODATA: errorMsg += " No data "; break;
                case MyCamera.MV_E_PRECONDITION: errorMsg += " Precondition error, or running environment changed "; break;
                case MyCamera.MV_E_VERSION: errorMsg += " Version mismatches "; break;
                case MyCamera.MV_E_NOENOUGH_BUF: errorMsg += " Insufficient memory "; break;
                case MyCamera.MV_E_UNKNOW: errorMsg += " Unknown error "; break;
                case MyCamera.MV_E_GC_GENERIC: errorMsg += " General error "; break;
                case MyCamera.MV_E_GC_ACCESS: errorMsg += " Node accessing condition error "; break;
                case MyCamera.MV_E_ACCESS_DENIED: errorMsg += " No permission "; break;
                case MyCamera.MV_E_BUSY: errorMsg += " Device is busy, or network disconnected "; break;
                case MyCamera.MV_E_NETER: errorMsg += " Network error "; break;
                default: errorMsg += " Unknown error "; break;
            }

            return errorMsg;
        }

        private void SaveImage(IntPtr pData, ref MyCamera.MV_FRAME_OUT_INFO pFrameInfo, IntPtr pUser)
        {
            try
            {
                //if ((3 * pFrameInfo.nFrameLen + 2048) > m_nBufSizeForSaveImage)
                //{
                //    m_nBufSizeForSaveImage = 3 * pFrameInfo.nFrameLen + 2048;
                //    m_pBufForSaveImage = new byte[m_nBufSizeForSaveImage];
                //}
                IntPtr pImage = Marshal.UnsafeAddrOfPinnedArrayElement(m_pBufForSaveImage, 0);

                MyCamera.MV_SAVE_IMAGE_PARAM stSaveParam = new MyCamera.MV_SAVE_IMAGE_PARAM();
                stSaveParam.enImageType = MyCamera.MV_SAVE_IAMGE_TYPE.MV_Image_Bmp;
                stSaveParam.enPixelType = pFrameInfo.enPixelType;
                stSaveParam.pData = pData;
                stSaveParam.nDataLen = pFrameInfo.nFrameLen;
                stSaveParam.nHeight = pFrameInfo.nHeight;
                stSaveParam.nWidth = pFrameInfo.nWidth;
                stSaveParam.pImageBuffer = pImage;
                stSaveParam.nBufferSize = m_nBufSizeForSaveImage;
                stSaveParam.nImageLen = 0;
                int nRet = m_pOperator.SaveImage(ref stSaveParam);

                //int rot = Settings.Default.RotateImg;
                int id = 0;
                int rot = 0;

                if (camindex > 0)
                    id = camindex - 1;
                if (Settings.Default.图像旋转?.Length > id)
                    rot = Settings.Default.图像旋转[id];

                if (CameraOperator.CO_OK == nRet)
                {
                    System.IO.MemoryStream ms = new System.IO.MemoryStream(m_pBufForSaveImage, 0, (int)stSaveParam.nImageLen);
                    Bitmap bmp = new Bitmap(ms);
                    switch (rot)
                    {
                        case 180:
                            bmp.RotateFlip(RotateFlipType.Rotate180FlipNone);
                            break;
                        case 90:
                            bmp.RotateFlip(RotateFlipType.Rotate90FlipNone);
                            break;
                        case 270:
                            bmp.RotateFlip(RotateFlipType.Rotate270FlipNone);
                            break;
                        default:
                            if (rot > 0 && rot < 8)
                                bmp.RotateFlip((RotateFlipType)rot);
                            break;
                    }
                    if (bmp.PixelFormat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed)
                        ImgResult = new CogImage8Grey(bmp);
                    else
                        ImgResult = new CogImage24PlanarColor(bmp);
                }
                waithdl.Set();
                if (FormPLC.ssFlag)
                {
                }
                if (extTrigFlag)
                    ImageGrabbed?.Invoke(ImgResult, camindex);
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
                float expo = 0;
                if (m_pOperator == null)
                    return -2;
                int re = m_pOperator.GetFloatValue("ExposureTime", ref expo);
                if (re == CameraOperator.CO_OK)
                    return ((int)Math.Round(Gain) * 1000) + expo / 1000;
                else
                    return -1;
            }
            set
            {
                if (value > 0 && m_pOperator != null)
                {
                    var g = (int)(value / 1000);
                    var ee = (float)(((value - g * 1000) * 1000));
                    if (ee <= 0)
                        m_pOperator.SetFloatValue("ExposureTime", 977);
                    else
                        m_pOperator.SetFloatValue("ExposureTime", ee);
                    Gain = g;
                }
            }
        }
        internal override double Gain
        {
            get
            {
                if (m_pOperator == null)
                    return -2;
                float expo = 0;
                int re = m_pOperator.GetFloatValue("Gain", ref expo);
                if (re == CameraOperator.CO_OK)
                    return expo;
                else
                    return -1;
            }
            set
            {
                if (value >= 0 && m_pOperator != null)
                {
                    m_pOperator.SetFloatValue("Gain", (float)(value));
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
            try
            {
                if (m_pOperator != null)
                {
                    m_pOperator.StopGrabbing();
                    m_pOperator.Close();
                }
            }
            catch
            {
            }
        }
        internal override bool Connected
        {
            get
            {
                if (m_pOperator == null)
                    return false;
                return true;
            }
        }
        internal override string CameraName
        {
            get
            {
                if (m_pOperator == null)
                    return null;
                else
                    return DeviceInfo;
            }
        }

        CameraOperator m_pOperator;
        HashSet<string> snset = new HashSet<string>() { "00DA0323360","DA0221511"
            ,"DA0124227","DA0267083","K27294574","00DA0337306"
            ,"DA1406068","DA1142526","DA1186400","DA1406065"
        };

        class CameraOperator
        {
            internal static MyCamera.MV_CC_DEVICE_INFO_LIST m_pDeviceList;
            internal static Dictionary<int, int> IndexDic = new Dictionary<int, int>();
            internal static void DeviceListAcq()
            {
                int nRet;
                m_pDeviceList = new MyCamera.MV_CC_DEVICE_INFO_LIST();
                IndexDic.Clear();
                /*创建设备列表*/
                System.GC.Collect();
                nRet = CameraOperator.EnumDevices(MyCamera.MV_USB_DEVICE | MyCamera.MV_GIGE_DEVICE, ref m_pDeviceList);
                if (0 != nRet)
                {
                    return;
                }
                return;
                StringBuilder sb = new StringBuilder();

                //在窗体列表中显示设备名
                for (int i = 0; i < m_pDeviceList.nDeviceNum; i++)
                {
                    MyCamera.MV_CC_DEVICE_INFO device = (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(m_pDeviceList.pDeviceInfo[i], typeof(MyCamera.MV_CC_DEVICE_INFO));
                    if (device.nTLayerType == MyCamera.MV_GIGE_DEVICE)
                    {
                        IntPtr buffer = Marshal.UnsafeAddrOfPinnedArrayElement(device.SpecialInfo.stGigEInfo, 0);
                        MyCamera.MV_GIGE_DEVICE_INFO gigeInfo = (MyCamera.MV_GIGE_DEVICE_INFO)Marshal.PtrToStructure(buffer, typeof(MyCamera.MV_GIGE_DEVICE_INFO));
                        //if (gigeInfo.chUserDefinedName != "")
                        //{
                        //    cbDeviceList.Items.Add("GigE: " + gigeInfo.chUserDefinedName);
                        //}
                        //else
                        //{
                        //    cbDeviceList.Items.Add("GigE: " + gigeInfo.chManufacturerName + " " + gigeInfo.chModelName + " (" + gigeInfo.chSerialNumber + ")");
                        //}
                        sb.AppendLine($"{i}:GIGE_{gigeInfo.chUserDefinedName}_{gigeInfo.chSerialNumber}");
                        int ind;
                        if (int.TryParse(gigeInfo.chUserDefinedName, out ind))
                        {
                            //IndexDic.Add(ind, i);
                        }
                        else
                        {
                        }
                    }
                    else if (device.nTLayerType == MyCamera.MV_USB_DEVICE)
                    {
                        IntPtr buffer = Marshal.UnsafeAddrOfPinnedArrayElement(device.SpecialInfo.stUsb3VInfo, 0);
                        MyCamera.MV_USB3_DEVICE_INFO usbInfo = (MyCamera.MV_USB3_DEVICE_INFO)Marshal.PtrToStructure(buffer, typeof(MyCamera.MV_USB3_DEVICE_INFO));
                        int ind;
                        sb.AppendLine($"{i}:USB3_{usbInfo.chUserDefinedName}_{usbInfo.chSerialNumber}");
                        if (int.TryParse(usbInfo.chUserDefinedName, out ind))
                        {
                            IndexDic.Add(ind, i);
                        }
                        else
                        {
                        }
                    }
                }
                //Program.MsgBox(sb.ToString());
            }

            public const int CO_FAIL = -1;
            public const int CO_OK = 0;
            private MyCamera m_pCSI;
            //private delegate void ImageCallBack(IntPtr pData, ref MyCamera.MV_FRAME_OUT_INFO pFrameInfo, IntPtr pUser);

            public CameraOperator()
            {
                // m_pDeviceList = new MyCamera.MV_CC_DEVICE_INFO_LIST();
                m_pCSI = new MyCamera();
            }

            /****************************************************************************
             * @fn           EnumDevices
             * @brief        枚举可连接设备
             * @param        nLayerType       IN         传输层协议：1-GigE; 4-USB;可叠加
             * @param        stDeviceList     OUT        设备列表
             * @return       成功：0；错误：错误码
             ****************************************************************************/
            public static int EnumDevices(uint nLayerType, ref MyCamera.MV_CC_DEVICE_INFO_LIST stDeviceList)
            {
                return MyCamera.MV_CC_EnumDevices_NET(nLayerType, ref stDeviceList);
            }

            /****************************************************************************
             * @fn           Open
             * @brief        连接设备
             * @param        stDeviceInfo       IN       设备信息结构体
             * @return       成功：0；错误：-1
             ****************************************************************************/
            public int Open(ref MyCamera.MV_CC_DEVICE_INFO stDeviceInfo)
            {
                if (null == m_pCSI)
                {
                    m_pCSI = new MyCamera();
                    if (null == m_pCSI)
                    {
                        return CO_FAIL;
                    }
                }

                int nRet;
                nRet = m_pCSI.MV_CC_CreateDevice_NET(ref stDeviceInfo);
                if (MyCamera.MV_OK != nRet)
                {
                    return nRet;
                }

                nRet = m_pCSI.MV_CC_OpenDevice_NET();
                if (MyCamera.MV_OK != nRet)
                {
                    return nRet;
                }
                return CO_OK;
            }

            /****************************************************************************
             * @fn           Close
             * @brief        关闭设备
             * @param        none
             * @return       成功：0；错误：-1
             ****************************************************************************/
            public int Close()
            {
                int nRet;

                nRet = m_pCSI.MV_CC_CloseDevice_NET();
                if (MyCamera.MV_OK != nRet)
                {
                    return CO_FAIL;
                }

                nRet = m_pCSI.MV_CC_DestroyDevice_NET();
                if (MyCamera.MV_OK != nRet)
                {
                    return CO_FAIL;
                }
                return CO_OK;
            }

            public void TriggerSoftware()
            {
                m_pCSI.MV_CC_TriggerSoftwareExecute_NET();
            }

            /****************************************************************************
             * @fn           StartGrabbing
             * @brief        开始采集
             * @param        none
             * @return       成功：0；错误：-1
             ****************************************************************************/
            public int StartGrabbing()
            {
                int nRet;
                //开始采集
                nRet = m_pCSI.MV_CC_StartGrabbing_NET();
                if (MyCamera.MV_OK != nRet)
                {
                    return CO_FAIL;
                }
                return CO_OK;
            }

            /****************************************************************************
             * @fn           StopGrabbing
             * @brief        停止采集
             * @param        none
             * @return       成功：0；错误：-1
             ****************************************************************************/
            public int StopGrabbing()
            {
                int nRet;
                nRet = m_pCSI.MV_CC_StopGrabbing_NET();
                if (MyCamera.MV_OK != nRet)
                {
                    return CO_FAIL;
                }
                return CO_OK;
            }

            /****************************************************************************
             * @fn           RegisterImageCallBack
             * @brief        注册取流回调函数
             * @param        CallBackFunc          IN        回调函数
             * @param        pUser                 IN        用户参数
             * @return       成功：0；错误：-1
             ****************************************************************************/
            public int RegisterImageCallBack(MyCamera.cbOutputdelegate CallBackFunc, IntPtr pUser)
            {
                int nRet;
                nRet = m_pCSI.MV_CC_RegisterImageCallBack_NET(CallBackFunc, pUser);
                if (MyCamera.MV_OK != nRet)
                {
                    return CO_FAIL;
                }
                return CO_OK;
            }

            /****************************************************************************
             * @fn           RegisterExceptionCallBack
             * @brief        注册异常回调函数
             * @param        CallBackFunc          IN        回调函数
             * @param        pUser                 IN        用户参数
             * @return       成功：0；错误：-1
             ****************************************************************************/
            public int RegisterExceptionCallBack(MyCamera.cbExceptiondelegate CallBackFunc, IntPtr pUser)
            {
                int nRet;
                nRet = m_pCSI.MV_CC_RegisterExceptionCallBack_NET(CallBackFunc, pUser);
                if (MyCamera.MV_OK != nRet)
                {
                    return CO_FAIL;
                }
                return CO_OK;
            }

            /****************************************************************************
             * @fn           GetOneFrame
             * @brief        获取一帧图像数据
             * @param        pData                 IN-OUT            数据数组指针
             * @param        pnDataLen             IN                数据大小
             * @param        nDataSize             IN                数组缓存大小
             * @param        pFrameInfo            OUT               数据信息
             * @return       成功：0；错误：-1
             ****************************************************************************/
            public int GetOneFrame(IntPtr pData, ref UInt32 pnDataLen, UInt32 nDataSize, ref MyCamera.MV_FRAME_OUT_INFO pFrameInfo)
            {
                pnDataLen = 0;
                int nRet = m_pCSI.MV_CC_GetOneFrame_NET(pData, nDataSize, ref pFrameInfo);
                if (MyCamera.MV_OK != nRet)
                {
                    return CO_FAIL;
                }

                pnDataLen = (uint)(pFrameInfo.nWidth * pFrameInfo.nWidth * (((((UInt32)pFrameInfo.enPixelType) >> 16) & 0xffff) >> 3));

                return CO_OK;
            }

            /****************************************************************************
             * @fn           Display
             * @brief        显示图像
             * @param        hWnd                  IN        窗口句柄
             * @return       成功：0；错误：-1
             ****************************************************************************/
            public int Display(IntPtr hWnd)
            {
                int nRet;
                nRet = m_pCSI.MV_CC_Display_NET(hWnd);
                if (MyCamera.MV_OK != nRet)
                {
                    return CO_FAIL;
                }
                return CO_OK;
            }

            /****************************************************************************
             * @fn           GetIntValue
             * @brief        获取Int型参数值
             * @param        strKey                IN        参数键值，具体键值名称参考HikCameraNode.xls文档
             * @param        pnValue               OUT       返回值
             * @return       成功：0；错误：-1
             ****************************************************************************/
            public int GetIntValue(string strKey, ref UInt32 pnValue)
            {

                MyCamera.MVCC_INTVALUE stParam = new MyCamera.MVCC_INTVALUE();
                int nRet = m_pCSI.MV_CC_GetIntValue_NET(strKey, ref stParam);
                if (MyCamera.MV_OK != nRet)
                {
                    return CO_FAIL;
                }

                pnValue = stParam.nCurValue;

                return CO_OK;
            }

            /****************************************************************************
             * @fn           SetIntValue
             * @brief        设置Int型参数值
             * @param        strKey                IN        参数键值，具体键值名称参考HikCameraNode.xls文档
             * @param        nValue                IN        设置参数值，具体取值范围参考HikCameraNode.xls文档
             * @return       成功：0；错误：-1
             ****************************************************************************/
            public int SetIntValue(string strKey, UInt32 nValue)
            {

                int nRet = m_pCSI.MV_CC_SetIntValue_NET(strKey, nValue);
                if (MyCamera.MV_OK != nRet)
                {
                    return CO_FAIL;
                }
                return CO_OK;
            }

            /****************************************************************************
             * @fn           GetFloatValue
             * @brief        获取Float型参数值
             * @param        strKey                IN        参数键值，具体键值名称参考HikCameraNode.xls文档
             * @param        pValue                OUT       返回值
             * @return       成功：0；错误：-1
             ****************************************************************************/
            public int GetFloatValue(string strKey, ref float pfValue)
            {
                MyCamera.MVCC_FLOATVALUE stParam = new MyCamera.MVCC_FLOATVALUE();
                int nRet = m_pCSI.MV_CC_GetFloatValue_NET(strKey, ref stParam);
                if (MyCamera.MV_OK != nRet)
                {
                    return CO_FAIL;
                }

                pfValue = stParam.fCurValue;

                return CO_OK;
            }

            /****************************************************************************
             * @fn           SetFloatValue
             * @brief        设置Float型参数值
             * @param        strKey                IN        参数键值，具体键值名称参考HikCameraNode.xls文档
             * @param        fValue                IN        设置参数值，具体取值范围参考HikCameraNode.xls文档
             * @return       成功：0；错误：-1
             ****************************************************************************/
            public int SetFloatValue(string strKey, float fValue)
            {
                int nRet = m_pCSI.MV_CC_SetFloatValue_NET(strKey, fValue);
                if (MyCamera.MV_OK != nRet)
                {
                    return CO_FAIL;
                }
                return CO_OK;
            }

            /****************************************************************************
             * @fn           GetEnumValue
             * @brief        获取Enum型参数值
             * @param        strKey                IN        参数键值，具体键值名称参考HikCameraNode.xls文档
             * @param        pnValue               OUT       返回值
             * @return       成功：0；错误：-1
             ****************************************************************************/
            public int GetEnumValue(string strKey, ref UInt32 pnValue)
            {
                MyCamera.MVCC_ENUMVALUE stParam = new MyCamera.MVCC_ENUMVALUE();
                int nRet = m_pCSI.MV_CC_GetEnumValue_NET(strKey, ref stParam);
                if (MyCamera.MV_OK != nRet)
                {
                    return CO_FAIL;
                }

                pnValue = stParam.nCurValue;

                return CO_OK;
            }

            /****************************************************************************
             * @fn           SetEnumValue
             * @brief        设置Float型参数值
             * @param        strKey                IN        参数键值，具体键值名称参考HikCameraNode.xls文档
             * @param        nValue                IN        设置参数值，具体取值范围参考HikCameraNode.xls文档
             * @return       成功：0；错误：-1
             ****************************************************************************/
            public int SetEnumValue(string strKey, UInt32 nValue)
            {
                int nRet = m_pCSI.MV_CC_SetEnumValue_NET(strKey, nValue);
                if (MyCamera.MV_OK != nRet)
                {
                    return CO_FAIL;
                }
                return CO_OK;
            }

            /****************************************************************************
             * @fn           GetBoolValue
             * @brief        获取Bool型参数值
             * @param        strKey                IN        参数键值，具体键值名称参考HikCameraNode.xls文档
             * @param        pbValue               OUT       返回值
             * @return       成功：0；错误：-1
             ****************************************************************************/
            public int GetBoolValue(string strKey, ref bool pbValue)
            {
                int nRet = m_pCSI.MV_CC_GetBoolValue_NET(strKey, ref pbValue);
                if (MyCamera.MV_OK != nRet)
                {
                    return CO_FAIL;
                }

                return CO_OK;
            }

            /****************************************************************************
             * @fn           SetBoolValue
             * @brief        设置Bool型参数值
             * @param        strKey                IN        参数键值，具体键值名称参考HikCameraNode.xls文档
             * @param        bValue                IN        设置参数值，具体取值范围参考HikCameraNode.xls文档
             * @return       成功：0；错误：-1
             ****************************************************************************/
            public int SetBoolValue(string strKey, bool bValue)
            {
                int nRet = m_pCSI.MV_CC_SetBoolValue_NET(strKey, bValue);
                if (MyCamera.MV_OK != nRet)
                {
                    return CO_FAIL;
                }
                return CO_OK;
            }

            /****************************************************************************
             * @fn           GetStringValue
             * @brief        获取String型参数值
             * @param        strKey                IN        参数键值，具体键值名称参考HikCameraNode.xls文档
             * @param        strValue              OUT       返回值
             * @return       成功：0；错误：-1
             ****************************************************************************/
            public int GetStringValue(string strKey, ref string strValue)
            {
                MyCamera.MVCC_STRINGVALUE stParam = new MyCamera.MVCC_STRINGVALUE();
                int nRet = m_pCSI.MV_CC_GetStringValue_NET(strKey, ref stParam);
                if (MyCamera.MV_OK != nRet)
                {
                    return CO_FAIL;
                }

                strValue = stParam.chCurValue;

                return CO_OK;
            }

            /****************************************************************************
             * @fn           SetStringValue
             * @brief        设置String型参数值
             * @param        strKey                IN        参数键值，具体键值名称参考HikCameraNode.xls文档
             * @param        strValue              IN        设置参数值，具体取值范围参考HikCameraNode.xls文档
             * @return       成功：0；错误：-1
             ****************************************************************************/
            public int SetStringValue(string strKey, string strValue)
            {
                int nRet = m_pCSI.MV_CC_SetStringValue_NET(strKey, strValue);
                if (MyCamera.MV_OK != nRet)
                {
                    return CO_FAIL;
                }
                return CO_OK;
            }

            /****************************************************************************
             * @fn           CommandExecute
             * @brief        Command命令
             * @param        strKey                IN        参数键值，具体键值名称参考HikCameraNode.xls文档
             * @return       成功：0；错误：-1
             ****************************************************************************/
            public int CommandExecute(string strKey)
            {
                int nRet = m_pCSI.MV_CC_SetCommandValue_NET(strKey);
                if (MyCamera.MV_OK != nRet)
                {
                    return CO_FAIL;
                }
                return CO_OK;
            }

            /****************************************************************************
             * @fn           SaveImage
             * @brief        保存图片
             * @param        pSaveParam            IN        保存图片配置参数结构体
             * @return       成功：0；错误：-1
             ****************************************************************************/
            public int SaveImage(ref MyCamera.MV_SAVE_IMAGE_PARAM pSaveParam)
            {
                int nRet;
                nRet = m_pCSI.MV_CC_SaveImage_NET(ref pSaveParam);
                if (MyCamera.MV_OK != nRet)
                {
                    return CO_FAIL;
                }
                return CO_OK;
            }
        }
    }
}
