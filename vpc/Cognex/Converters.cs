using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;
using System.Reflection;
using System.IO;
using System.Windows.Forms.Design;
using Cognex.VisionPro;

namespace vpc
{
    /// <summary>
    /// 枚举转换器
    /// 用此类之前，必须保证在枚举项中定义了Description
    /// </summary>
    internal class MyEnumConverter : ExpandableObjectConverter
    {
        protected Dictionary<string, object> dic;
        public MyEnumConverter()
        {
            dic = new Dictionary<string, object>();
        }

        private void LoadDic(ITypeDescriptorContext context)
        {
            Type enumType = context.PropertyDescriptor.PropertyType;
            dic.Clear();
            FieldInfo[] fieldinfos = enumType.GetFields();
            foreach (FieldInfo field in fieldinfos)
            {
                if (field.FieldType.IsEnum)
                {
                    Object[] objs = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
                    if (objs.Length > 0)
                    {
                        dic.Add(((DescriptionAttribute)objs[0]).Description, Enum.Parse(enumType, field.Name));
                    }
                    else
                        dic.Add(field.Name, Enum.Parse(enumType, field.Name));
                }
                else if (field.DeclaringType == typeof(System.Boolean))
                {
                    dic = BoolDic;
                    return;
                }
            }
            if (enumType.FullName == "Cognex.VisionPro.ICogFrameGrabber")
            {
                foreach (ICogFrameGrabber val in CamerasGige.cameras)
                {
                    dic.Add(string.Format("{0},{1},{2}", val.Name, val.SerialNumber, val.OwnedGigEAccess == null ? "USB3.0" : val.OwnedGigEAccess.CurrentIPAddress), val);
                }
                dic.Add("Null", "Null");
            }
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;

            return base.CanConvertFrom(context, sourceType);
        }
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value is string)
            {
                //如果是枚举
                //if (context.PropertyDescriptor.PropertyType.IsEnum)
                {
                    string val = (string)value;
                    if (val == "Null")
                        return null;
                    if (string.IsNullOrEmpty(val))
                        return null;
                    if (dic.Count == 0)
                        LoadDic(context);
                    object re;
                    if (dic.TryGetValue(val, out re))
                        return re;
                    else
                    {//新增配方
                        foreach (char rInvalidChar in Path.GetInvalidFileNameChars())
                            if (val.Contains(rInvalidChar))
                            {
                                Program.MsgBox("名称不能包含 " + rInvalidChar);
                                break;
                            }
                        if (val.Length > 100)
                            Program.MsgBox("名称过长");
                        if (val.Length == 0)
                            Program.MsgBox("未输入名称");
                        return val;
                    }
                    //else if(context.PropertyDescriptor.PropertyType == typeof(System.Boolean))
                }
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return true;
        }
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (dic.Count == 0)
                LoadDic(context);
            if (value == null)
                return "Null";
            object re;
            string valuestr = value.ToString();
            foreach (var pair in dic)
            {
                if (value.Equals(pair.Value))
                    return pair.Key;
            }
            if (destinationType == typeof(string))
                if (dic.TryGetValue(valuestr, out re))
                    return re;

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            if (dic.Count == 0)
                LoadDic(context);

            StandardValuesCollection vals = new TypeConverter.StandardValuesCollection(dic.Values);

            return vals;
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return false;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }

        static readonly Dictionary<string, object> BoolDic = new Dictionary<string, object>(2) { { "是", true }, { "否", false } };
    }

    internal class MyCogRegionsConverter : ArrayConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type destinationType)
        {
            return true;
        }
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            string str = value as string;
            ushort us;
            if (context.Instance is Block1SettingsDongChuPian)
            {
                var val0 = (Block1SettingsDongChuPian)context.Instance;
                var val = val0.Region4;
                if (str != null && str.EndsWith("个区域"))
                    str = str.Substring(0, str.Length - 3);
                if (str != null && ushort.TryParse(str, out us) && val != null)
                {
                    int id = 0;
                    if (val.Count > us)
                    {
                        val.RemoveRange(us, val.Count - us);
                    }
                    else
                        while (val.Count < us)
                        {
                            val.Add(new CogRectangle());
                        }
                }
                return val;
            }
            return context.Instance;
        }
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return true;
        }
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (value is List<CogRectangle>)
            {
                var graphs = (List<CogRectangle>)value;
                return string.Format("{0}个区域", graphs.Count);
            }
            else if (value == null)
                return "Null";
            else
                return value.ToString();
        }
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            PropertyDescriptor[] array = null;
            if (value.GetType().IsArray)
            {
                int length = ((Array)value).GetLength(0);
                array = new PropertyDescriptor[length];
                Type type = value.GetType();
                Type elementType = type.GetElementType();
                for (int i = 0; i < length; i++)
                {
                    array[i] = new ArrayPropertyDescriptor(type, elementType, i);
                }
            }
            else if (value is List<CogRectangle>)
            {
                var v = (List<CogRectangle>)value;
                int length = v.Count;
                array = new PropertyDescriptor[length];
                Type type = value.GetType();
                Type elementType = typeof(CogRectangle);
                for (int i = 0; i < length; i++)
                {
                    array[i] = new ArrayPropertyDescriptor(type, elementType, i);
                }
            }
            return new PropertyDescriptorCollection(array);
        }
        private class ArrayPropertyDescriptor : SimplePropertyDescriptor
        {
            private int index;
            public ArrayPropertyDescriptor(Type arrayType, Type elementType, int index) : base(arrayType, "[" + index + "]", elementType, null)
            {
                this.index = index;
            }
            public override object GetValue(object instance)
            {
                if (instance is Array)
                {
                    Array array = (Array)instance;
                    if (array.GetLength(0) > this.index)
                    {
                        return array.GetValue(this.index);
                    }
                }
                else if (instance is List<CogRectangle>)
                {
                    var ary = (List<CogRectangle>)instance;
                    if (ary.Count > index)
                        return ary[index];
                }
                return null;
            }
            public override void SetValue(object instance, object value)
            {
                if (instance is Array)
                {
                    Array array = (Array)instance;
                    if (array.GetLength(0) > this.index)
                    {
                        array.SetValue(value, this.index);
                    }
                    else if (instance is List<CogRectangle>)
                    {
                        var ary = (List<CogRectangle>)instance;
                        if (ary.Count > index)
                            ary[index] = (CogRectangle)value;
                    }
                    this.OnValueChanged(instance, EventArgs.Empty);
                }
            }
            public override TypeConverter Converter
            {
                get
                {
                    return new MyCogToolConverter();
                }
            }
            public override object GetEditor(Type editorBaseType)
            {
                return new MyCogToolEditor();
            }
        }
    }
    internal class MyExpandableObjectConverter : ExpandableObjectConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (value == null)
                return "Null";
            return base.ConvertTo(context, culture, value, destinationType);
        }
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            //List<Block1Settings> objs = new List<Block1Settings>();
            //Block1Collection b1s = context.Instance as Block1Collection;
            //Block1Settings obj = context.PropertyDescriptor.GetValue(context.Instance) as Block1Settings;
            //if (obj != null)
            //{
            //    objs.Add(new Block1SettingsBlob(obj.index, obj.parent));
            //    objs.Add(new Block1SettingsHistogram(obj.index, obj.parent));
            //}
            List<string> objs = new List<string>();
            objs.Add(Block1SettingsBlob.Name);
            objs.Add(Block1SettingsHistogram.Name);
            StandardValuesCollection vals = new TypeConverter.StandardValuesCollection(objs);
            return vals;
        }
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;
            return base.CanConvertFrom(context, sourceType);
        }
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value is string)
            {
                string val = (string)value;
                if (val == "Null")
                    return null;
                return val;
            }
            return base.ConvertFrom(context, culture, value);
        }
    }
    internal class MySerialPortConverter : MyEnumConverter
    {
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            dic = PortsDic;
            StandardValuesCollection vals = new TypeConverter.StandardValuesCollection(dic.Values);
            return vals;
        }
        static readonly Dictionary<string, object> PortsDic = new Dictionary<string, object>(4) { { "COM1", "COM1" }, { "COM2", "COM2" }, { "COM3", "COM3" }, { "COM4", "COM4" } };
    }
    internal class MyJobConverter : MyEnumConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value is string)
            {
                //如果是枚举
                //if (context.PropertyDescriptor.PropertyType.IsEnum)
                {
                    string val = (string)value;
                    if (val == "<新配方>")
                        return null;
                    if (string.IsNullOrEmpty(val))
                        return null;
                    if (dic.Count == 0)
                        loadjobs();
                    object re;
                    if (dic.TryGetValue(val, out re))
                        return re;
                    //else if(context.PropertyDescriptor.PropertyType == typeof(System.Boolean))
                }
            }
            return base.ConvertFrom(context, culture, value);
        }
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            loadjobs();
            StandardValuesCollection vals = new TypeConverter.StandardValuesCollection(dic.Values);
            return vals;
        }
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (dic.Count == 0)
                loadjobs();
            if (value == null)
                return "<新配方>";
            object re;
            string valuestr = value.ToString();
            foreach (var pair in dic)
            {
                if (value.Equals(pair.Value))
                    return pair.Key;
            }
            if (destinationType == typeof(string))
                if (dic.TryGetValue(valuestr, out re))
                    return re;

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return false;
        }
        void loadjobs()
        {
            //if (dic == null || dic.Count == 0)
            {
                dic = new Dictionary<string, object>();
                if (Directory.Exists("jobs") == false)
                    Directory.CreateDirectory("jobs");
                DirectoryInfo di = new DirectoryInfo("jobs");
                FileInfo[] fis = di.GetFiles("*.vpp");
                for (int i = 0; i < fis.Length; i++)
                {
                    dic.Add(Path.GetFileNameWithoutExtension(fis[i].Name), fis[i].FullName);
                }
                dic.Add("<新配方>", null);
            }
        }
    }
    internal class FilePathToolEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            if (context != null && context.Instance != null)
            {
                return UITypeEditorEditStyle.Modal;
            }
            return base.GetEditStyle(context);
        }
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            IWindowsFormsEditorService editorService = null;
            if (context != null && context.Instance != null && provider != null)
            {
                editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
                if (editorService != null)
                {
                    System.Windows.Forms.OpenFileDialog fb = new System.Windows.Forms.OpenFileDialog();
                    string str = value as string;
                    if (string.IsNullOrEmpty(context.PropertyDescriptor.Description) == false)
                        fb.Filter = context.PropertyDescriptor.Description;
                    if (Directory.Exists(str))
                    {
                        DirectoryInfo di = new DirectoryInfo(str);
                        fb.InitialDirectory = di.Parent.FullName;
                    }
                    else if (Directory.Exists(context.PropertyDescriptor.Category))
                        fb.InitialDirectory = (new DirectoryInfo(context.PropertyDescriptor.Category)).FullName;
                    else
                        fb.InitialDirectory = Settings.SystemDIR;
                    if (System.Windows.Forms.DialogResult.OK == fb.ShowDialog())
                    {
                        JobManager.TryLoadSettings(fb.FileName);
                        if (fb.FileName.StartsWith(Settings.SystemDIR))
                        {
                            return fb.FileName.Substring(Settings.SystemDIR.Length + 1);
                        }
                        return fb.FileName;
                    }
                }
            }
            return value;
        }
        public override void PaintValue(PaintValueEventArgs e)
        {
            //e.Graphics.DrawRectangle(Pens.Red, e.Bounds);
            //e.Graphics.DrawEllipse(Pens.Red, e.Bounds);
        }
        public override bool GetPaintValueSupported(ITypeDescriptorContext context)
        {
            return false;
        }
    }
    internal class FolderPathToolEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            if (context != null && context.Instance != null)
            {
                return UITypeEditorEditStyle.Modal;
            }
            return base.GetEditStyle(context);
        }
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            IWindowsFormsEditorService editorService = null;
            if (context != null && context.Instance != null && provider != null)
            {
                editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
                if (editorService != null)
                {
                    System.Windows.Forms.FolderBrowserDialog fb = new System.Windows.Forms.FolderBrowserDialog();
                    string str = value as string;
                    if (Directory.Exists(str))
                    {
                        DirectoryInfo di = new DirectoryInfo(str);
                        fb.SelectedPath = di.FullName;
                    }
                    else
                        fb.SelectedPath = Settings.SystemDIR;
                    if (System.Windows.Forms.DialogResult.OK == fb.ShowDialog())
                        return fb.SelectedPath;
                }
            }
            return value;
        }
        public override void PaintValue(PaintValueEventArgs e)
        {
            //e.Graphics.DrawRectangle(Pens.Red, e.Bounds);
            //e.Graphics.DrawEllipse(Pens.Red, e.Bounds);
        }
        public override bool GetPaintValueSupported(ITypeDescriptorContext context)
        {
            return false;
        }
    }
    internal class PolygonVerticesToolEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            if (context != null && context.Instance != null)
            {
                return UITypeEditorEditStyle.Modal;
            }
            return base.GetEditStyle(context);
        }
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            //IWindowsFormsEditorService editorService = null;
            if (context != null && context.Instance != null && provider != null)
            {
                vpc.ToolEditor.CogPolygonDisplay obj = context.Instance as ToolEditor.CogPolygonDisplay;
                if (obj != null)
                {
                    if (context.PropertyDescriptor.Name == "AddVertices")
                    {
                        obj.addVertices();
                    }
                    else if (context.PropertyDescriptor.Name == "RemoveVertices")
                    {
                        obj.removeVertices();
                    }
                }
            }
            return value;
        }
    }
    internal class MaskListToolEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            if (context != null && context.Instance != null)
            {
                return UITypeEditorEditStyle.Modal;
            }
            return base.GetEditStyle(context);
        }
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            //IWindowsFormsEditorService editorService = null;
            if (context != null && context.Instance != null && provider != null)
            {
                vpc.ToolEditor.CogMaskListDisplay obj = context.Instance as ToolEditor.CogMaskListDisplay;
                if (obj != null)
                {
                    if (context.PropertyDescriptor.Name == "addRegion")
                    {
                        obj.AddRegion();
                    }
                    else if (context.PropertyDescriptor.Name == "removeRegion")
                    {
                        obj.RemoveRegion();
                    }
                    else if (context.PropertyDescriptor.Name == "clearRegion")
                    {
                        obj.ClearRegion();
                    }
                }
            }
            return value;
        }
    }

    internal class MyStrConverter : StringConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
                if (value is double)
                {
                    return ((double)value).ToString("0.000");
                }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
    internal class MyCogToolsConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return true;
        }
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (value is List<Cognex.VisionPro.PMAlign.CogPMAlignPattern> ptns)
            {
                int utCount = 0;
                for (int i = 0; i < ptns.Count; i++)
                {
                    if (ptns[i] == null || ptns[i].Trained == false)
                        utCount++;
                }
                if (utCount > 0)
                    return $"{ptns.Count}个模版,{utCount}个未训练";
                else
                    return $"{ptns.Count}个模版";
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
    internal class MyCogToolConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return true;
        }
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (value is Cognex.VisionPro.CogAcqFifoTool)
            {
                Cognex.VisionPro.CogAcqFifoTool ct = value as Cognex.VisionPro.CogAcqFifoTool;
                if (ct.Operator == null)
                    return "未初始化";
                else if (ct.Operator.FrameGrabber == null)
                    return "相机连接异常";
                else
                {
                    string ip = string.Empty;
                    string ip2 = string.Empty;
                    if (ct.Operator.FrameGrabber.OwnedGigEAccess != null)
                    {
                        ip2 = ":";
                        ip = ct.Operator.FrameGrabber.OwnedGigEAccess.CurrentIPAddress;
                    }
                    return string.Format("{0}{1}{2}-{3}", ip, ip2, ct.Operator.FrameGrabber.Name, ct.Operator.VideoFormat);
                }
            }
            else if (value is Cognex.VisionPro.PMAlign.CogPMAlignTool)
            {
                Cognex.VisionPro.PMAlign.CogPMAlignTool pm = value as Cognex.VisionPro.PMAlign.CogPMAlignTool;
                if (pm.Pattern == null || pm.Pattern.Trained == false)
                    return "模版未训练";
                else
                    return "已训练";
            }
            else if (value is Cognex.VisionPro.ToolBlock.CogToolBlock)
            {
                Cognex.VisionPro.ToolBlock.CogToolBlock pm = value as Cognex.VisionPro.ToolBlock.CogToolBlock;
                try
                {
                    if (pm.Outputs.Contains("Model"))
                    {
                        CogImage8Grey img = pm.Outputs["Model"].Value as CogImage8Grey;
                        if (img != null)
                            return "已训练";
                        else
                            return "未训练";
                    }
                    else if (pm.Inputs.Contains("Model"))
                    {
                        CogImage8Grey img = pm.Inputs["Model"].Value as CogImage8Grey;
                        if (img != null)
                            return "已训练";
                        else
                            return "未训练";
                    }
                    else
                        return "内部错误";
                }
                catch (System.Exception ex)
                {
                    return ex.Message;
                }
            }
            else if (value is Cognex.VisionPro.CogImage8Grey)
            {
                return "灰度图像";
            }
            else if (value is Cognex.VisionPro.CogImage24PlanarColor)
            {
                return "彩色图像";
            }
            else if (value is Cognex.VisionPro.ICogGraphicInteractive[])
            {
                Cognex.VisionPro.ICogGraphicInteractive[] graphs = (Cognex.VisionPro.ICogGraphicInteractive[])value;
                return string.Format("{0}个图形", graphs.Length);
            }
            else if (value is Cognex.VisionPro.CogCircularArc)
            {
                Cognex.VisionPro.CogCircularArc arc = (Cognex.VisionPro.CogCircularArc)value;
                return string.Format("(X={0:F3},Y={1:F3})R={2:F3},Ang(Start={3:F3},Span={4:F3})", arc.CenterX, arc.CenterY, arc.Radius, arc.AngleStart, arc.AngleSpan);
            }
            else if (value is Cognex.VisionPro.CogRectangle)
            {
                Cognex.VisionPro.CogRectangle arc = (Cognex.VisionPro.CogRectangle)value;
                return string.Format("(X={0:F1},Y={1:F1})宽={2:F1},高={3:F1}", arc.X, arc.Y, arc.Width, arc.Height);
            }
            else if (value is Cognex.VisionPro.CogPolygon)
            {
                Cognex.VisionPro.CogPolygon arc = (Cognex.VisionPro.CogPolygon)value;
                double x, y;
                for (int i = arc.NumVertices; i < 3; i++)
                {
                    arc.AddVertex(30 + i * 10, 50 + (i + 1) * (i + 1) * 20, -1);
                }
                arc.AreaCenter(out x, out y);
                return string.Format("多边形(X={0:F1},Y={1:F1})", x, y);
            }
            else if (value is Cognex.VisionPro.CogCircle)
            {
                Cognex.VisionPro.CogCircle arc = (Cognex.VisionPro.CogCircle)value;
                return string.Format("圆(X={0:F1},Y={1:F1},R={2:F1})", arc.CenterX, arc.CenterY, arc.Radius);
            }
            else if (value is Cognex.VisionPro.CogCircularAnnulusSection)
            {
                Cognex.VisionPro.CogCircularAnnulusSection arc = (Cognex.VisionPro.CogCircularAnnulusSection)value;
                return string.Format("圆弧(X={0:F1},Y={1:F1},R={2:F1},Ang({3:F1}-{4:F1}))", arc.CenterX, arc.CenterY, arc.Radius, arc.AngleStart * 180 / Math.PI, (arc.AngleStart + arc.AngleSpan) * 180 / Math.PI);
            }
            else if (value is Cognex.VisionPro.CogRectangleAffine)
            {
                Cognex.VisionPro.CogRectangleAffine arc = (Cognex.VisionPro.CogRectangleAffine)value;
                arc.GraphicDOFEnable = CogRectangleAffineDOFConstants.All & ~CogRectangleAffineDOFConstants.Skew;
                return string.Format("矩形(X={0:F1},Y={1:F1})宽={2:F1},高={3:F1},旋转={4:F1}", arc.CenterX, arc.CenterY, arc.SideXLength, arc.SideYLength, arc.Rotation);
            }
            else if (value is Cognex.VisionPro.CogEllipse ce)
            {
                return string.Format("椭圆(X={0:F1},Y={1:F1})rx={2:F1},ry={3:F1},旋转={4:F1}", ce.CenterX, ce.CenterY, ce.RadiusX, ce.RadiusY, ce.Rotation);
            }
            else if (value is Cognex.VisionPro.CogLineSegment)
            {
                Cognex.VisionPro.CogLineSegment arc = (Cognex.VisionPro.CogLineSegment)value;
                arc.GraphicDOFEnable = CogLineSegmentDOFConstants.All;
                return string.Format("线段(X={0:F1},Y={1:F1})->(X={2:F1},Y={3:F1}){4:F1}°", arc.StartX, arc.StartY, arc.EndX, arc.EndY, arc.Rotation * 180 / Math.PI);
            }
            else if (value is CogGraphicInteractiveCollection)
            {
                CogGraphicInteractiveCollection arc = (CogGraphicInteractiveCollection)value;
                return string.Format("{0}个屏蔽区域", arc.Count);
            }
            else if (value is Cognex.VisionPro.ColorSegmenter.CogColorRangeCollection)
            {
                Cognex.VisionPro.ColorSegmenter.CogColorRangeCollection arc = (Cognex.VisionPro.ColorSegmenter.CogColorRangeCollection)value;
                return string.Format("{0}种颜色", arc.Count);
            }
            else if (value is System.Drawing.Color)
            {
                return string.Format("训练颜色");
            }
            else if (value == null)
                return "Null";
            else
                return value.ToString();
        }
    }
    internal class MyTrainCogToolBlockConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return true;
        }
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (value is Cognex.VisionPro.ToolBlock.CogToolBlock)
            {
                Cognex.VisionPro.ToolBlock.CogToolBlock pm = value as Cognex.VisionPro.ToolBlock.CogToolBlock;
                try
                {
                    if (pm.Name == Block1SettingsModelCompare.NameParm
                        || pm.Name == Block1SettingsPatCheck.NameParm
                        || pm.Name == Block1SettingsLoc.NameParm
                        || pm.Name == Block1SettingsModelCompareMult.NameParm
                        || pm.Name == Block1SettingsModelCompareColor.NameParm)
                    {
                        if (pm.Inputs.Contains("TrainModel") && pm.Inputs.Contains("Parm"))
                        {
                            object[] parm = pm.Inputs["Parm"].Value as object[];
                            if (parm == null || parm.Length < 3)
                                return "内部错误1";
                            Cognex.VisionPro.PMAlign.CogPMAlignPattern pattern = parm[2] as Cognex.VisionPro.PMAlign.CogPMAlignPattern;
                            if (parm[2] is Cognex.VisionPro.PMAlign.CogPMAlignPattern[] ptns)
                            {
                                int id = 0;
                                if (pm.Inputs.Contains("selectPtn") && pm.Inputs["selectPtn"].Value is int ptnid)
                                    id = ptnid;
                                if (ptns.Length > id)
                                    pattern = ptns[id];
                                else
                                    pattern = null;
                            }
                            if (pattern == null)
                                return "内部错误2";
                            if (pattern.TrainRegion == null)
                                pattern.TrainRegion = new CogRectangleAffine();
                            if (pattern.Trained)
                                return "已训练";
                            else
                                return "未训练";
                        }
                        else
                            return "内部错误";
                    }
                    else
                        return "内部错误10";
                }
                catch (System.Exception ex)
                {
                    return ex.Message;
                }
            }
            else if (value == null)
                return "Null";
            else
                return value.ToString();
        }
    }
    internal class MyTrainAllToolsCogToolBlockConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return true;
        }
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (value is Cognex.VisionPro.ToolBlock.CogToolBlock)
            {
                Cognex.VisionPro.ToolBlock.CogToolBlock pm = value as Cognex.VisionPro.ToolBlock.CogToolBlock;
                try
                {
                    if (pm.Inputs.Contains("TrainAllModels"))
                    {
                        return "训练所有";
                    }
                    else
                        return "内部错误";
                }
                catch (System.Exception ex)
                {
                    return ex.Message;
                }
            }
            else if (value == null)
                return "Null";
            else
                return value.ToString();
        }
    }
    internal class MyTrainAllCogToolsEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            if (context != null && context.Instance != null)
            {
                return UITypeEditorEditStyle.Modal;
            }
            return base.GetEditStyle(context);
        }
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {

            IWindowsFormsEditorService editorService = null;
            if (context != null && context.Instance != null && provider != null)
            {
                editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
                if (editorService != null)
                {
                    if (value is Cognex.VisionPro.ToolBlock.CogToolBlock)
                    {
                        var block = value as Cognex.VisionPro.ToolBlock.CogToolBlock;
                        if (block.Inputs.Contains("TrainAllModels"))
                        {
                            if (System.Windows.Forms.MessageBox.Show("训练所有模型？", null, System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                            {
                                block.Inputs["TrainAllModels"].Value = true;
                                block.Run();
                                if (ParmsInputDialogue.SelectedString != null)
                                    Program.Loginfo("训练所有：{0}".FormatWith(ParmsInputDialogue.SelectedString));
                                if (string.IsNullOrEmpty(block.RunStatus.Message) == false)
                                    Program.MsgBox(block.RunStatus.Message);
                            }
                        }
                        else
                            Program.MsgBox("内部错误11");
                    }
                }
            }
            return value;
        }
    }

    internal class MyCogToolEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            if (context != null && context.Instance != null)
            {
                return UITypeEditorEditStyle.Modal;
            }
            return base.GetEditStyle(context);
        }
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            IWindowsFormsEditorService editorService = null;
            if (context != null && context.Instance != null && provider != null)
            {
                editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
                if (editorService != null)
                {
                    if (value is Cognex.VisionPro.CogAcqFifoTool)
                    {
                        CogAcqFifoEditV2 cav = new CogAcqFifoEditV2();
                        Cognex.VisionPro.CogAcqFifoTool fifo = value as Cognex.VisionPro.CogAcqFifoTool;
                        if (fifo != null && fifo.Operator != null)
                            if (fifo.Operator.FrameGrabber != null)
                            {
                                fifo.Operator.OwnedTriggerParams.TriggerEnabled = false;
                            }
                            else
                                fifo.Operator = null;
                        cav.Subject = fifo;
                        CogToolEditor cte = new CogToolEditor(cav);
                        cte.ShowDialog();
                    }
                    else if (value is List<Cognex.VisionPro.PMAlign.CogPMAlignPattern> ptns)
                    {
                        PatternsTrainForm cte = new PatternsTrainForm(ptns);
                        cte.ShowDialog();
                    }
                    else if (value is Cognex.VisionPro.ToolBlock.CogToolBlock)
                    {
                        Cognex.VisionPro.ToolBlock.CogToolBlock t = value as Cognex.VisionPro.ToolBlock.CogToolBlock;
                        TrainModelEditor cte = new TrainModelEditor(t);
                        cte.ShowDialog();
                    }
                    else if (value is Cognex.VisionPro.CogRectangle)
                    {
                        var arc = (Cognex.VisionPro.CogRectangle)value;
                        arc.Color = CogColorConstants.Yellow;
                        arc.LineWidthInScreenPixels = 2;
                        arc.GraphicDOFEnable = CogRectangleDOFConstants.All;
                        ToolEditor tl = new ToolEditor(arc);
                        tl.ShowDialog();
                    }
                    else if (value is Cognex.VisionPro.CogPolygon)
                    {
                        Cognex.VisionPro.CogPolygon arc = (Cognex.VisionPro.CogPolygon)value;
                        arc.Color = CogColorConstants.Yellow;
                        arc.LineWidthInScreenPixels = 2;
                        arc.GraphicDOFEnable = CogPolygonDOFConstants.All;
                        ToolEditor tl = new ToolEditor(arc);
                        tl.ShowDialog();
                    }
                    else if (value is Cognex.VisionPro.CogCircle)
                    {
                        Cognex.VisionPro.CogCircle arc = (Cognex.VisionPro.CogCircle)value;
                        arc.Color = CogColorConstants.Yellow;
                        arc.LineWidthInScreenPixels = 2;
                        arc.GraphicDOFEnable = CogCircleDOFConstants.All;
                        ToolEditor tl = new ToolEditor(arc);
                        tl.ShowDialog();
                    }
                    else if (value is Cognex.VisionPro.ICogGraphicInteractive)
                    {
                        Cognex.VisionPro.ICogGraphicInteractive arc = (Cognex.VisionPro.ICogGraphicInteractive)value;
                        arc.Color = CogColorConstants.Yellow;
                        arc.LineWidthInScreenPixels = 2;
                        arc.GraphicDOFEnableBase = CogGraphicDOFConstants.All;
                        ToolEditor tl = new ToolEditor(arc);
                        tl.ShowDialog();
                    }
                    else if (value is System.Drawing.Color)
                    {
                        System.Drawing.Color arc = (System.Drawing.Color)value;
                        ToolEditor tl = new ToolEditor(arc);
                        tl.ShowDialog();
                        //if(!arc.Equals(tl.TrainedColor))
                        //    Program.Loginfo()
                        return tl.TrainedColor;
                    }
                    else if (value is Cognex.VisionPro.CogGraphicInteractiveCollection)
                    {
                        Cognex.VisionPro.CogGraphicInteractiveCollection arc = (Cognex.VisionPro.CogGraphicInteractiveCollection)value;
                        ToolEditor tl = new ToolEditor(arc);
                        tl.ShowDialog();
                    }
                }
            }
            return value;
        }
        public override void PaintValue(PaintValueEventArgs e)
        {
            //e.Graphics.DrawRectangle(Pens.Red, e.Bounds);
            //e.Graphics.DrawEllipse(Pens.Red, e.Bounds);
        }
        public override bool GetPaintValueSupported(ITypeDescriptorContext context)
        {
            return false;
        }
    }
    internal class DateTimeEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            if (context != null && context.Instance != null)
            {
                return UITypeEditorEditStyle.Modal;
            }
            return base.GetEditStyle(context);
        }
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            IWindowsFormsEditorService editorService = null;
            if (context != null && context.Instance != null && provider != null)
            {
                editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
                if (editorService != null)
                {
                    if (value is string)
                    {
                        return DateTime.Now.ToString("yyyyMMdd");
                    }
                }
            }
            return value;
        }
    }
    internal class MyEmptyEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.None;
        }
    }
}
