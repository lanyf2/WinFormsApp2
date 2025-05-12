using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vpc
{
    internal static class General
    {
        internal static string Compare(string stringA, string stringB)
        {
            if (stringA != stringB)
                return string.Format("{0} -> {1}\r\n", stringA, stringB);
            return null;
        }
        internal static string Compare(object objA, object objB)
        {
            if (objA == null && objB == null)
                return null;
            if (objA == objB)
                return null;
            if (objA == null)
                return "Null origin";
            if (objB == null)
                return "Null newobject";
            Type tp1 = objA.GetType();
            Type tp2 = objB.GetType();
            if (tp1 != tp2)
                return "Different Type";
            if (tp1.IsValueType || tp1.IsEnum || objA is string)
            {
                if (objA.Equals(objB) == false)
                    return string.Format("{0} -> {1}\r\n", objA, objB);
            }
            else
            {
                var properties = tp1.GetProperties();
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < properties.Length; i++)
                {
                    if (properties[i].PropertyType.IsValueType)
                    {
                        var re = Compare(properties[i].GetValue(objA), properties[i].GetValue(objB));
                        if (re != null)
                            sb.Append(properties[i].Name).Append(": ").Append(re);
                    }
                }
                return sb.ToString();
            }
            return null;
        }
    }
}
