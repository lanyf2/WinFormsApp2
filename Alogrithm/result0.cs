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
    public class R0Class : RxInterface
    {
        public static R0Class Empty = new R0Class(new object[] { null, false, "无", (byte)0, 0, 0 });
        double imgTime;
        double algTime;

        public R0Class(object[] _result, double _imgTime = 0, double _algTime = 0, ICogRecord _record = null)
        {
            tm = DateTime.Now;
            re = _result;
            imgTime = _imgTime;
            algTime = _algTime;
            record = _record;
            StringBuilder sb = new StringBuilder();
            if (re == null || re.Length != 6)
            {
                re = new object[] { null, false, "传入参数错误", (byte)0, 0, 0 };
                return;
            }
           
            if (sb.Length > 0)
            {
                sb.Remove(sb.Length - 1, 1);
                sb.Append("异常");
                re[1] = false;
                re[2] = sb.ToString();
            }
            else
            {
                re[1] = true;
                re[2] = "合格";
            }
        }

        [DisplayName("次品种类")]
        public override string Info
        {
            get
            {
                if (re != null)
                    return re[2] as string;
                return null;
            }
        }

        [DisplayName("结果图像"), TypeConverter(typeof(MyCogToolConverter)), Browsable(false)]
        public override ICogImage img
        {
            get
            {
                return null;
            }
        }
    }
}
