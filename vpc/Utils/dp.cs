using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Drawing;

namespace vpc
{
    public class XPropDescriptor : PropertyDescriptor
    {
        XProp theProp;
        public XPropDescriptor(XProp prop, Attribute[] attrs)
            : base(prop.Name, attrs)
        {
            theProp = prop;
        }

        public override string Category
        {
            get
            {
                if (theProp.Category != null)
                    return theProp.Category;
                else
                    return base.Category;
            }
        }

        public override string Description
        {
            get
            {
                if (theProp.Description != null)
                    return theProp.Description;
                else
                    return base.Description;
            }
        }
        public override bool CanResetValue(object component)
        {
            return false;
        }

        public override System.Type ComponentType
        {
            get { return this.GetType(); }
        }

        public override object GetValue(object component)
        {
            return theProp.Value;
        }

        public override bool IsReadOnly
        {
            get
            {
                return theProp.IsReadOnly;
                //if (theProp.Editor == null)
                //    return true;
                //else
                //    return false;
            }
        }

        public override System.Type PropertyType
        {
            get
            {
                if (theProp.Value == null)
                    return null;
                else
                    return theProp.Value.GetType();
            }
        }

        public override void ResetValue(object component)
        {
        }
        public override TypeConverter Converter
        {
            get
            {
                if (theProp.Converter != null)
                    return theProp.Converter;
                else
                    return base.Converter;
            }
        }
        public override object GetEditor(Type editorBaseType)
        {
            if (theProp.Editor != null)
                return Activator.CreateInstance(theProp.Editor);
            else
                return base.GetEditor(editorBaseType);
        }
        public override void SetValue(object component, object value)
        {
            theProp.Value = value;
        }
        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }
    }
    public class XProps : List<XProp>, ICustomTypeDescriptor
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

        public virtual PropertyDescriptorCollection GetProperties(System.Attribute[] attributes)
        {
            PropertyDescriptor[] props = new PropertyDescriptor[this.Count];
            for (int i = 0; i < this.Count; i++)
            {
                props[i] = new XPropDescriptor(this[i], attributes);
            }
            return new PropertyDescriptorCollection(props);
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
        public string Name;
        public XProps(string name = null)
        {
            Name = name;
        }
        public override string ToString()
        {
            if (string.IsNullOrEmpty(Name))
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < this.Count; i++)
                {
                    sb.Append("[" + i + "] " + this[i].ToString() + System.Environment.NewLine);
                }
                return sb.ToString();
            }
            else
                return Name;
        }
    }
    public class XProp
    {
        public string tag = null;
        public string Description = null;
        public string Category = null;
        private string theName = "";
        private object theValue = null;
        public bool IsReadOnly = false;
        TypeConverter _converter = null;
        Action<XProp> ValueChanged;
        Type _editor = null;
        public XProp(string _name, object _val, Action<XProp> valueChanged)
        {
            Name = _name;
            theValue = _val;
            if (valueChanged != null)
                ValueChanged += valueChanged;
        }
        public XProp(string _name, object _val, bool isreadonly = false)
        {
            Name = _name;
            Value = _val;
            IsReadOnly = isreadonly;
        }
        public XProp(string _name, object _val, string _category, string _desc)
        {
            Name = _name;
            Value = _val;
            Category = _category;
            Description = _desc;
        }
        public XProp(string _name, object _val, string _category, string _desc, TypeConverter converter, Type editor = null)
        {
            Name = _name;
            Value = _val;
            Category = _category;
            Description = _desc;
            _converter = converter;
            _editor = editor;
        }
        public string Name
        {
            get
            {
                return this.theName;
            }
            set
            {
                this.theName = value;
            }
        }
        public object Value
        {
            get
            {
                return this.theValue;
            }
            set
            {
                if (value != null)
                {
                    if (theValue == null || value.Equals(theValue) == false)
                    {
                        this.theValue = value;
                        ValueChangedHdl();
                        return;
                    }
                }
                this.theValue = value;
            }
        }
        public TypeConverter Converter  //类型转换器，我们在制作下拉列表时需要用到  
        {
            get
            {
                return _converter;
            }
        }
        public Type Editor
        {
            get
            {
                return _editor;
            }
        }
        public override string ToString()
        {
            return "Name: " + Name + ", Value: " + Value;
        }
        void ValueChangedHdl()
        {
            if (ValueChanged != null)
                ValueChanged(this);
        }
    }
    public class myTypeConverter : TypeConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            return base.ConvertTo(context, culture, value, destinationType);
        }
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            return TypeDescriptor.GetProperties(value, attributes);
        }
        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
    }
}
