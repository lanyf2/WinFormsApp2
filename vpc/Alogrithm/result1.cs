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
    public class R1Class : RxInterface, ICustomTypeDescriptor
    {
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
            System.ComponentModel.PropertyDescriptorCollection pdc = System.ComponentModel.TypeDescriptor.GetProperties(this);

            List<PropertyDescriptor> props = new List<PropertyDescriptor>();
            //  new PropertyDescriptor[ResultGraphics.Count + pdc.Count];
            for (int i = 0; i < pdc.Count; i++)
                props.Add(pdc[i]);
            if (img is CogImage8Grey)
                props.Remove(pdc["imgGrey"]);
            if (ModelGrey == null)
                props.Remove(pdc["ModelGrey"]);
            if (parent != null)
            {
                //if (string.IsNullOrEmpty(parent.MTC) == false)
                //    props.Add(new XPropDescriptor(new XProp("MTC", DataView.ConvertInvalidCharToHex(parent.MTC), true), attributes));
                if (string.IsNullOrEmpty(parent.DID) == false)
                    props.Add(new XPropDescriptor(new XProp("DID", DataView.ConvertInvalidCharToHex(parent.DID), true), attributes));
                if (parent.ExtraInfo != null)
                    for (int i = 0; i < parent.ExtraInfo.Length / 2; i++)
                    {
                        if (string.IsNullOrEmpty(parent.ExtraInfo[i * 2]) == false && string.IsNullOrEmpty(parent.ExtraInfo[i * 2 + 1]) == false)
                            props.Add(new XPropDescriptor(new XProp(parent.ExtraInfo[i * 2], DataView.ConvertInvalidCharToHex(parent.ExtraInfo[i * 2 + 1]), true), attributes));
                    }
            }
            if (ResultGraphics != null)
                for (int i = 0; i < ResultGraphics.Count; i++)
                {
                    var val = ResultGraphics[i];
                    string name = string.Format("工具{0}", i + 1);
                    if (val != null && val.Length > 1)
                        try
                        {
                            if (val[0] is ICogGraphicInteractive)
                            {
                                if (string.IsNullOrEmpty((val[0] as ICogGraphicInteractive).TipText) == false)
                                    name = (val[0] as ICogGraphicInteractive).TipText;
                            }
                            else if (val[0] is CogGraphicInteractiveCollection)
                            {
                                CogGraphicInteractiveCollection cca = (CogGraphicInteractiveCollection)val[0];
                                if (cca.Count > 0)
                                    if (string.IsNullOrEmpty(cca[0].TipText) == false)
                                        name = cca[0].TipText;
                            }
                            props.Add(new XPropDescriptor(new XProp(name, val[1], true), attributes));
                            if (val.Length >= 3)
                                for (int j = 2; j < val.Length; j++)
                                {
                                    if (j == 2 && val[2] is CogImage8Grey)
                                        props.Add(new XPropDescriptor(new XProp(name + "模版", val[2], null, null, new MyCogToolConverter()), attributes));
                                    else
                                        props.Add(new XPropDescriptor(new XProp(name, val[j], true), attributes));
                                }
                        }
                        catch (Exception ex)
                        {
                            props.Add(new XPropDescriptor(new XProp(name, ex.Message, true), attributes));
                        }
                }
            return new PropertyDescriptorCollection(props.ToArray());
        }
        double imgTime;
        double algTime;

        public R1Class(object[] _result, double _imgTime = 0, double _algTime = 0, ICogRecord _record = null)
        {
            tm = DateTime.Now;
            re = _result;
            imgTime = _imgTime;
            algTime = _algTime;
            record = _record;
        }

        [DisplayName("次品种类")]
        public override string Info
        {
            get
            {
                if (re != null && re.Length > 1)
                    return re[2] as string;
                return null;
            }
        }
        [DisplayName("取像耗时"), TypeConverter(typeof(MyStrConverter))]
        public double ImgTime
        {
            get
            {
                return imgTime;
            }
        }
        [DisplayName("算法耗时"), TypeConverter(typeof(MyStrConverter))]
        public double AlgTime
        {
            get
            {
                return algTime;
            }
        }
        [DisplayName("结果图像"), TypeConverter(typeof(MyCogToolConverter))]
        public override ICogImage img
        {
            get
            {
                if (re != null && re.Length > 0)
                    return re[0] as ICogImage;
                return null;
            }
        }
        [DisplayName("结果图像2"), TypeConverter(typeof(MyCogToolConverter))]
        public ICogImage imgGrey
        {
            get
            {
                if (re != null && re.Length > 3)
                    return re[4] as ICogImage;
                return null;
            }
        }
        [DisplayName("全局模版"), TypeConverter(typeof(MyCogToolConverter))]
        public ICogImage ModelGrey
        {
            get
            {
                if (re != null && re.Length > 4)
                    return re[5] as ICogImage;
                return null;
            }
        }

        public override string ToString()
        {
            return Info;
        }
    }
}
