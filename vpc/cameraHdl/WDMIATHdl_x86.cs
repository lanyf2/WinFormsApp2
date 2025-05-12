using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IAT.Imaging;
using Cognex.VisionPro;

namespace vpc
{
    internal class WDMIATx86Hdl : CameraHdlBase
    {
        Grabbing_an_Image.WDMIATx86Hdl icc;

        internal override ICogImage GetImage(int discardct = 3)
        {
            if (icc == null)
                return null;
            return icc.GetImage(discardct);
        }
        internal override void Init()
        {
            //icc = new ICImagingControl();//key:ISB3200016679
            icc = new Grabbing_an_Image.WDMIATx86Hdl();//key:ISB3200016679
            icc.Init();
        }
        internal override double ExposureTime
        {
            get
            {
                return icc.ExposureTime;
            }
            set
            {
                try
                {
                    icc.ExposureTime = value;
                }
                catch (Exception ex)
                {
                    Program.MsgBox(ex.Message);
                }
            }
        }
        internal override double Gain
        {
            get
            {
                return icc.Gain;
            }
            set
            {
                try
                {
                    icc.Gain = value;
                }
                catch (Exception ex)
                {
                    Program.MsgBox(ex.Message);
                }
            }
        }
        internal override double Brightness
        {
            get
            {
                return icc.Brightness;
            }
            set
            {
                try
                {
                    icc.Brightness = value;
                }
                catch (Exception ex)
                {
                    Program.MsgBox(ex.Message);
                }
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
                    return icc.Connected;
            }
        }
    }
}
