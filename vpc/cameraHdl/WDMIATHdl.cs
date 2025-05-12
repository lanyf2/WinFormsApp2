using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IAT.Imaging;
using Cognex.VisionPro;

namespace vpc
{
    internal class WDMIATHdl : CameraHdlBase
    {
        ICImagingControl icc;
        VCDAbsoluteValueProperty exposurevalue;
        VCDRangeProperty gainvalue;
        VCDRangeProperty Brightnessvalue;

        internal override ICogImage GetImage(int discardct = 3)
        {
            if (icc.DeviceValid == false)
                return null;
            icc.MemorySnapImage(50000);
            //if (discardct > 0)
            //   icc.MemorySnapImage(5000);
            var buf = icc.ImageActiveBuffer;
            if (buf == null)
                return null;
            var bmp = buf.Bitmap;
            if (bmp == null)
                return null;
            return new CogImage24PlanarColor(bmp);
        }
        internal override void Init()
        {
            //icc = new ICImagingControl();//key:ISB3200016679
            icc = new ICImagingControl();//key:ISB3200016679
            if (icc.Devices.Length > 0)
                icc.Device = icc.Devices[0].ToString();
            else
                return;
            //VCDPropertyItem exposure = icc.VCDPropertyItems.FindItem(VCDIDs.VCDID_Exposure);

            //VCDRangeProperty BrightnessRange = (VCDRangeProperty)icc.VCDPropertyItems.FindInterface(IAT.Imaging.VCDIDs.VCDID_Brightness + ":" + IAT.Imaging.VCDIDs.VCDElement_Value + ":" + IAT.Imaging.VCDIDs.VCDInterface_Range);
            VCDSwitchProperty exposureauto = (VCDSwitchProperty)icc.VCDPropertyItems.FindInterface(string.Format("{0}:{1}:{2}", VCDIDs.VCDID_Exposure, VCDIDs.VCDElement_Auto, VCDIDs.VCDInterface_Switch));
            if (exposureauto != null)
                exposureauto.Switch = false;
            exposurevalue = (VCDAbsoluteValueProperty)icc.VCDPropertyItems.FindInterface(string.Format("{0}:{1}:{2}", VCDIDs.VCDID_Exposure, VCDIDs.VCDElement_Value, VCDIDs.VCDInterface_AbsoluteValue));

            VCDSwitchProperty gainauto = (VCDSwitchProperty)icc.VCDPropertyItems.FindInterface(string.Format("{0}:{1}:{2}", VCDIDs.VCDID_Gain, VCDIDs.VCDElement_Auto, VCDIDs.VCDInterface_Switch));
            if (gainauto != null)
                gainauto.Switch = false;
            gainvalue = (VCDRangeProperty)icc.VCDPropertyItems.FindInterface(string.Format("{0}:{1}:{2}", VCDIDs.VCDID_Gain, VCDIDs.VCDElement_Value, VCDIDs.VCDInterface_Range));
            if (gainvalue != null)
                gainvalue.Value = gainvalue.RangeMin;

            VCDSwitchProperty Brightnessauto = (VCDSwitchProperty)icc.VCDPropertyItems.FindInterface(string.Format("{0}:{1}:{2}", VCDIDs.VCDID_Brightness, VCDIDs.VCDElement_Auto, VCDIDs.VCDInterface_Switch));
            if (Brightnessauto != null)
                Brightnessauto.Switch = false;
            Brightnessvalue = (VCDRangeProperty)icc.VCDPropertyItems.FindInterface(string.Format("{0}:{1}:{2}", VCDIDs.VCDID_Brightness, VCDIDs.VCDElement_Value, VCDIDs.VCDInterface_Range));

            //VCDID_Brightness //亮度
            //VCDID_Contrast //对比度
            //VCDID_Hue //色度
            //VCDID_Saturation //饱和度
            //VCDID_Gamma //伽马校正
            //VCDID_WhiteBalance //白平衡
            //VCDID_Gain //增益
            //VCDID_Exposure //曝光
            //VCDID_Zoom //镜头变倍
            //VCDID_Iris //镜头光圈
            //VCDID_Focus //镜头聚焦


        }
        internal override double ExposureTime
        {
            get
            {
                if (exposurevalue != null)
                    return exposurevalue.Value * 1000;
                return -1;
            }
            set
            {
                value = value / 1000;
                if (exposurevalue != null)
                    if (value < exposurevalue.RangeMax && value > exposurevalue.RangeMin)
                        exposurevalue.Value = value;
                    else
                        Program.MsgBox(string.Format("可设范围：{0} - {1}", exposurevalue.RangeMin * 1000, exposurevalue.RangeMax * 1000));
                //KSJDS.ExposureTime = value;
            }
        }
        internal override double Gain
        {
            get
            {
                if (gainvalue != null)
                    return gainvalue.Value;
                return -1;
            }
            set
            {
                if (gainvalue != null)
                    if (value < gainvalue.RangeMax && value > gainvalue.RangeMin)
                        gainvalue.Value = (int)value;
                    else
                        Program.MsgBox(string.Format("可设范围：{0} - {1}", gainvalue.RangeMin, gainvalue.RangeMax));
            }
        }
        internal override double Brightness
        {
            get
            {
                if (Brightnessvalue != null)
                    return Brightnessvalue.Value;
                return -1;
            }
            set
            {
                if (Brightnessvalue != null)
                    if (value < Brightnessvalue.RangeMax && value > Brightnessvalue.RangeMin)
                        Brightnessvalue.Value = (int)value;
                    else
                        Program.MsgBox(string.Format("可设范围：{0} - {1}", Brightnessvalue.RangeMin, Brightnessvalue.RangeMax));
            }
        }
        internal override void Dispose()
        {
            if (icc != null)
                icc.Dispose();
        }
        internal override bool Connected
        {
            get
            {
                if (icc == null)
                    return false;
                else
                    return icc.DeviceValid;
            }
        }
    }
}
