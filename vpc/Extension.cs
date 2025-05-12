using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vpc
{
    internal static class MyExtension
    {
        #region string
        internal static string FormatWith(this string format, params object[] parm1)
        {
            if (format == null)
                return null;
            else
                return string.Format(format, parm1);
        }
        internal static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }
        internal static string ToHexString(this ushort[] str)
        {
            if (str == null)
                return null;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < str.Length; i++)
            {
                sb.Append(str[i].ToString("X")).Append('-');
            }
            if (sb.Length > 0)
                sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }
        internal static string ToHexString(this string str)
        {
            if (str.IsNullOrEmpty())
                return null;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < str.Length; i++)
            {
                sb.Append(((ushort)str[i]).ToString("X")).Append('-');
            }
            if (sb.Length > 0)
                sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }
        internal static string ToValidFileName(this string str)
        {
            if (str.IsNullOrEmpty())
                return null;
            StringBuilder sb = new StringBuilder(str);
            foreach (char rInvalidChar in Path.GetInvalidFileNameChars())
            {
                string s = rInvalidChar.ToString();
                sb.Replace(s, "[{0}]".FormatWith(s.ToHexString()));
            }
            return sb.ToString();
        }
        #endregion
        #region Dictionary
        internal static Dictionary<Tkey, TValue> TryAdd<Tkey, TValue>(this Dictionary<Tkey, TValue> dict, Tkey key, TValue value)
        {
            if (dict != null && dict.ContainsKey(key) == false)
                dict.Add(key, value);
            return dict;
        }
        internal static Dictionary<Tkey, TValue> TryAddOrReplace<Tkey, TValue>(this Dictionary<Tkey, TValue> dict, Tkey key, TValue value)
        {
            if (dict != null)
                if (dict.ContainsKey(key) == false)
                    dict.Add(key, value);
                else
                    dict[key] = value;
            return dict;
        }
        internal static TValue TryGetValue<Tkey, TValue>(this Dictionary<Tkey, TValue> dict, Tkey key)
        {
            if (dict != null && dict.ContainsKey(key))
                return dict[key];
            return default(TValue);
        }
        #endregion
        internal static bool In<T>(this T t, params T[] c)
        {
            if (c == null)
                return false;
            return c.Any(i => i.Equals(t));
        }
        internal static bool IsBetween<T>(this T t, T lowerBound, T upperBound, bool includeLowerBound
            , bool includeUpperBound) where T : IComparable<T>
        {
            if (t == null)
                throw new ArgumentNullException("t");
            var l = t.CompareTo(lowerBound);
            var h = t.CompareTo(upperBound);
            return (includeLowerBound && l == 0) || (includeUpperBound && h == 0)
                || (l > 0 && h < 0);

        }
        #region IPEndPoint
        internal static System.Net.IPEndPoint TryParseIPEndPoint(this string AddressString)
        {
            if (AddressString.IsNullOrEmpty())
                return null;
            var sp = AddressString.Split(':');
            if (sp != null && sp.Length >= 1)
            {
                System.Net.IPAddress ip;
                ushort port = 502;
                if (sp.Length >= 2)
                    ushort.TryParse(sp[1], out port);
                if (System.Net.IPAddress.TryParse(sp[0], out ip))
                    return new System.Net.IPEndPoint(ip, port);
            }
            return null;
        }
        #endregion

        internal static System.Drawing.Color CalcRGB(this System.Drawing.PointF Yxy, double Bright)
        {

            double X = Yxy.X * (Bright / Yxy.Y);
            double Y = Bright;
            double Z = (1 - Yxy.X - Yxy.Y) * (Bright / Yxy.Y);

            double var_X = X / 100;
            double var_Y = Y / 100;
            double var_Z = Z / 100;

            double var_R = var_X * 3.2406 + var_Y * -1.5372 + var_Z * -0.4986;
            double var_G = var_X * -0.9689 + var_Y * 1.8758 + var_Z * 0.0415;
            double var_B = var_X * 0.0557 + var_Y * -0.2040 + var_Z * 1.0570;

            if (var_R > 0.0031308)
                var_R = 1.055 * (Math.Pow(var_R, (1 / 2.4))) - 0.055;
            else
                var_R = 12.92 * var_R;
            if (var_G > 0.0031308)
                var_G = 1.055 * (Math.Pow(var_G, (1 / 2.4))) - 0.055;
            else
                var_G = 12.92 * var_G;
            if (var_B > 0.0031308)
                var_B = 1.055 * (Math.Pow(var_B, (1 / 2.4))) - 0.055;
            else
                var_B = 12.92 * var_B;

            int sR = (int)(var_R * 255);
            int sG = (int)(var_G * 255);
            int sB = (int)(var_B * 255);

            if (sR > 255 || sG > 255 || sB > 255 || sR < 0 || sG < 0 || sB < 0)
                return System.Drawing.Color.FromArgb(0, 0, 0, 0);
            else
                return System.Drawing.Color.FromArgb(255, sR, sG, sB);
        }
        internal static Cognex.VisionPro.CogSimpleColor CalcHSI(this System.Drawing.Color color)
        {
            var cr = new Cognex.VisionPro.CogSimpleColor(Cognex.VisionPro.CogImageColorSpaceConstants.RGB);
            cr.Plane0 = color.R;
            cr.Plane1 = color.G;
            cr.Plane2 = color.B;
            return Cognex.VisionPro.CogImageConvert.GetHSIColor(cr);
        }
        internal static System.Drawing.PointF CalcYxy(this System.Drawing.Color color)
        {
            double var_R = (color.R / 255.0);
            double var_G = (color.G / 255.0);
            double var_B = (color.B / 255.0);

            if (var_R > 0.04045)
                var_R = Math.Pow((var_R + 0.055) / 1.055, 2.4);
            else
                var_R = var_R / 12.92;
            if (var_G > 0.04045)
                var_G = Math.Pow((var_G + 0.055) / 1.055, 2.4);
            else
                var_G = var_G / 12.92;
            if (var_B > 0.04045)
                var_B = Math.Pow((var_B + 0.055) / 1.055, 2.4);
            else
                var_B = var_B / 12.92;

            var_R = var_R * 100;
            var_G = var_G * 100;
            var_B = var_B * 100;

            double X = var_R * 0.4124 + var_G * 0.3576 + var_B * 0.1805;
            double Y = var_R * 0.2126 + var_G * 0.7152 + var_B * 0.0722;
            double Z = var_R * 0.0193 + var_G * 0.1192 + var_B * 0.9505;
            double sum = X + Y + Z;
            return new System.Drawing.PointF((float)(X / sum), (float)(Y / sum));
        }
    }
}
