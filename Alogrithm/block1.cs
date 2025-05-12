using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using vpc;
using Cognex.VisionPro.FGGigE;
using Cognex.VisionPro.ToolBlock;
using System.Threading;
using System.Globalization;
using Cognex.VisionPro.PMAlign;

namespace Cognex.VisionPro
{
    #region Tools
    internal class Block1Settinggunzisize : Block1Settings
    {
        public const string Name = "滚子尺寸检测工具";
        public const string NameParm = "gunzisize";

        public Block1Settinggunzisize(int _index, Block1Collection _parent)
            : base(_index, _parent)
        {//
            if (IsParmValid(internalParm) == false)
            {
                var c = new CogRectangle();
                var c2 = new CogLineSegment();
                var c3 = new CogLineSegment();
                var c4 = new CogLineSegment();
                var c5 = new CogLineSegment();
                double[] p = new double[32];
                int[] p2 = new int[32];
                internalParm = new object[8] { NameParm, c, c2, c3, c4, c5, p, p2 };
            }
        }
        internal static bool IsParmValid(object[] parm)
        {
            if (parm == null)
                return false;
            if (parm.Length == 8 && parm[0] as string == NameParm && parm[1] is CogRectangle
                && parm[2] is CogLineSegment && parm[4] is CogLineSegment && parm[5] is CogLineSegment
                && parm[3] is CogLineSegment && parm[6] is double[] && parm[7] is int[])
            {
                return true;
            }
            else
                return false;
        }
        public override string ToString()
        {
            return 名称;
        }
        protected override string GetMtdStr()
        {
            return Name;
        }

        [DisplayName("名称")]
        public string 名称
        {
            set
            {
                if (internalParm != null)
                {
                    ICogGraphicInteractive ic = internalParm[1] as ICogGraphicInteractive;
                    if (ic != null)
                        ic.TipText = value;
                }
            }
            get
            {
                if (internalParm != null)
                {
                    ICogGraphicInteractive ic = internalParm[1] as ICogGraphicInteractive;
                    if (ic != null && string.IsNullOrEmpty(ic.TipText) == false)
                        return ic.TipText;
                }
                return Name;
            }
        }
        [DisplayName("检测位置"), TypeConverter(typeof(MyCogToolConverter)), Editor(typeof(MyCogToolEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public ICogRegion Region
        {
            set { internalParm[1] = value; }
            get { return internalParm[1] as ICogRegion; }
        }
        [DisplayName("倒角1位置"), TypeConverter(typeof(MyCogToolConverter)), Editor(typeof(MyCogToolEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public CogLineSegment Region2
        {
            set { internalParm[2] = value; }
            get { return internalParm[2] as CogLineSegment; }
        }
        [DisplayName("倒角2位置"), TypeConverter(typeof(MyCogToolConverter)), Editor(typeof(MyCogToolEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public CogLineSegment Region3
        {
            set { internalParm[3] = value; }
            get { return internalParm[3] as CogLineSegment; }
        }
        [DisplayName("倒角3位置"), TypeConverter(typeof(MyCogToolConverter)), Editor(typeof(MyCogToolEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public CogLineSegment Region4
        {
            set { internalParm[4] = value; }
            get { return internalParm[4] as CogLineSegment; }
        }
        [DisplayName("倒角4位置"), TypeConverter(typeof(MyCogToolConverter)), Editor(typeof(MyCogToolEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public CogLineSegment Region5
        {
            set { internalParm[5] = value; }
            get { return internalParm[5] as CogLineSegment; }
        }
        internal double[] p
        {
            set { internalParm[6] = value; }
            get { return (double[])internalParm[6]; }
        }
        internal int[] pi
        {
            set { internalParm[7] = value; }
            get { return (int[])internalParm[7]; }
        }
        internal int 绝对缺陷阈值
        {
            set { pi[0] = value; }
            get { return pi[0]; }
        }
        internal int 相对缺陷阈值差值
        {
            set { pi[1] = value; }
            get { return pi[1]; }
        }
        internal int 最小缺陷面积
        {
            set { pi[2] = value; }
            get { return pi[2]; }
        }
        internal int 边缘瑕疵滤波
        {
            set { pi[3] = value; }
            get { return pi[3]; }
        }
        internal int 相对缺陷阈值差值2
        {
            set { pi[4] = value; }
            get { return pi[4]; }
        }
        internal int 最小缺陷面积2
        {
            set { pi[5] = value; }
            get { return pi[5]; }
        }
        internal int 表面平均亮度下限
        {
            set { pi[6] = value; }
            get { return pi[6]; }
        }


        public double 标定系数
        {
            set { p[0] = value; }
            get { return p[0]; }
        }
        public double 高度
        {
            set { p[1] = value; }
            get { return p[1]; }
        }
        public double 高度允许偏差
        {
            set { p[2] = value; }
            get { return p[2]; }
        }
        public double 外径
        {
            set { p[3] = value; }
            get { return p[3]; }
        }
        public double 外径允许偏差
        {
            set { p[4] = value; }
            get { return p[4]; }
        }
        public double 倒角角度
        {
            set { p[5] = value; }
            get { return p[5]; }
        }
        public double 倒角角度允许偏差
        {
            set { p[6] = value; }
            get { return p[6]; }
        }
        public double 倒角宽度
        {
            set { p[7] = value; }
            get { return p[7]; }
        }
        public double 倒角宽度允许偏差
        {
            set { p[8] = value; }
            get { return p[8]; }
        }
        public double 倒角高度
        {
            set { p[9] = value; }
            get { return p[9]; }
        }
        public double 倒角高度允许偏差
        {
            set { p[10] = value; }
            get { return p[10]; }
        }
        public double 倒角长度
        {
            set { p[11] = value; }
            get { return p[11]; }
        }
        public double 倒角长度允许偏差
        {
            set { p[12] = value; }
            get { return p[12]; }
        }
    }
    internal class Block1Settinggunzichk : Block1Settings
    {
        public const string Name = "滚子表面缺陷检测工具";
        public const string NameParm = "gunzi";

        public Block1Settinggunzichk(int _index, Block1Collection _parent)
            : base(_index, _parent)
        {//
            if (IsParmValid(internalParm) == false)
            {
                var c = new CogRectangle();
                var c2 = new CogCircularArc();
                var c3 = new CogCircularArc();
                double[] p = new double[32];
                int[] p2 = new int[32];
                internalParm = new object[6] { NameParm, c, c2, c3, p, p2 };
            }
        }
        internal static bool IsParmValid(object[] parm)
        {
            if (parm == null)
                return false;
            if (parm.Length == 6 && parm[0] as string == NameParm && parm[1] is CogRectangle
                && parm[2] is CogCircularArc && parm[4] is double[] && parm[5] is int[]
                && parm[3] is CogCircularArc)
            {
                return true;
            }
            else
                return false;
        }
        public override string ToString()
        {
            return 名称;
        }
        protected override string GetMtdStr()
        {
            return Name;
        }

        [DisplayName("名称")]
        public string 名称
        {
            set
            {
                if (internalParm != null)
                {
                    ICogGraphicInteractive ic = internalParm[1] as ICogGraphicInteractive;
                    if (ic != null)
                        ic.TipText = value;
                }
            }
            get
            {
                if (internalParm != null)
                {
                    ICogGraphicInteractive ic = internalParm[1] as ICogGraphicInteractive;
                    if (ic != null && string.IsNullOrEmpty(ic.TipText) == false)
                        return ic.TipText;
                }
                return Name;
            }
        }
        [DisplayName("检测位置"), TypeConverter(typeof(MyCogToolConverter)), Editor(typeof(MyCogToolEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public ICogRegion Region
        {
            set { internalParm[1] = value; }
            get { return internalParm[1] as ICogRegion; }
        }
        [DisplayName("滚子上边缘位置"), TypeConverter(typeof(MyCogToolConverter)), Editor(typeof(MyCogToolEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public CogCircularArc Region2
        {
            set { internalParm[2] = value; }
            get { return internalParm[2] as CogCircularArc; }
        }
        [DisplayName("滚子下边缘位置"), TypeConverter(typeof(MyCogToolConverter)), Editor(typeof(MyCogToolEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public CogCircularArc Region3
        {
            set { internalParm[3] = value; }
            get { return internalParm[3] as CogCircularArc; }
        }
        internal double[] p
        {
            set { internalParm[4] = value; }
            get { return (double[])internalParm[4]; }
        }
        internal int[] pi
        {
            set { internalParm[5] = value; }
            get { return (int[])internalParm[5]; }
        }
        public int 绝对缺陷阈值
        {
            set { pi[0] = value; }
            get { return pi[0]; }
        }
        public int 相对缺陷阈值差值
        {
            set { pi[1] = value; }
            get { return pi[1]; }
        }
        public int 最小缺陷面积
        {
            set { pi[2] = value; }
            get { return pi[2]; }
        }
        public int 边缘瑕疵滤波
        {
            set { pi[3] = value; }
            get { return pi[3]; }
        }
        public int 相对缺陷阈值差值2
        {
            set { pi[4] = value; }
            get { return pi[4]; }
        }
        public int 最小缺陷面积2
        {
            set { pi[5] = value; }
            get { return pi[5]; }
        }
        public int 表面平均亮度下限
        {
            set { pi[6] = value; }
            get { return pi[6]; }
        }


        [DisplayName("标定系数")]
        internal double 标定系数
        {
            set { p[0] = value; }
            get { return p[0]; }
        }
        internal double 缺料长度上限
        {
            set { p[1] = value; }
            get { return p[1]; }
        }
        internal double 搜索长度
        {
            set { p[2] = value; }
            get { return p[2]; }
        }
    }
    internal class Block1Settingshanliaochk : Block1Settings
    {
        public const string Name = "焊料检测工具";
        public const string NameParm = "hanliao";

        public Block1Settingshanliaochk(int _index, Block1Collection _parent)
            : base(_index, _parent)
        {//
            if (IsParmValid(internalParm) == false)
            {
                var c = new CogLineSegment();
                var c2 = new CogLineSegment();
                var c3 = new CogLineSegment();
                double[] p = new double[32];
                internalParm = new object[6] { NameParm, c, c2, p, 0, c3 };
            }
        }
        internal static bool IsParmValid(object[] parm)
        {
            if (parm == null)
                return false;
            if (parm.Length == 6 && parm[0] as string == NameParm && parm[1] is CogLineSegment
                && parm[2] is CogLineSegment && parm[3] is double[] && parm[4] is int
                && parm[5] is CogLineSegment)
            {
                return true;
            }
            else
                return false;
        }
        public override string ToString()
        {
            return 名称;
        }
        protected override string GetMtdStr()
        {
            return Name;
        }

        [DisplayName("名称")]
        public string 名称
        {
            set
            {
                if (internalParm != null)
                {
                    ICogGraphicInteractive ic = internalParm[1] as ICogGraphicInteractive;
                    if (ic != null)
                        ic.TipText = value;
                }
            }
            get
            {
                if (internalParm != null)
                {
                    ICogGraphicInteractive ic = internalParm[1] as ICogGraphicInteractive;
                    if (ic != null && string.IsNullOrEmpty(ic.TipText) == false)
                        return ic.TipText;
                }
                return Name;
            }
        }
        [DisplayName("焊料位置"), TypeConverter(typeof(MyCogToolConverter)), Editor(typeof(MyCogToolEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public CogLineSegment Region
        {
            set { internalParm[1] = value; }
            get { return internalParm[1] as CogLineSegment; }
        }
        [DisplayName("焊料边缘1位置"), TypeConverter(typeof(MyCogToolConverter)), Editor(typeof(MyCogToolEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public CogLineSegment Region2
        {
            set { internalParm[2] = value; }
            get { return internalParm[2] as CogLineSegment; }
        }
        [DisplayName("焊料边缘2位置"), TypeConverter(typeof(MyCogToolConverter)), Editor(typeof(MyCogToolEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public CogLineSegment Region3
        {
            set { internalParm[5] = value; }
            get { return internalParm[5] as CogLineSegment; }
        }
        internal double[] p
        {
            set { internalParm[3] = value; }
            get { return (double[])internalParm[3]; }
        }
        [DisplayName("标定系数")]
        public double 标定系数
        {
            set { p[0] = value; }
            get { return p[0]; }
        }
        public double 缺料长度上限
        {
            set { p[1] = value; }
            get { return p[1]; }
        }
        public double 搜索长度
        {
            set { p[2] = value; }
            get { return p[2]; }
        }
    }
    internal class Block1Settingsgapchk : Block1Settings
    {
        public const string Name = "间隙(厚度)检测工具";
        public const string NameParm = "gapchk";

        public Block1Settingsgapchk(int _index, Block1Collection _parent)
            : base(_index, _parent)
        {//
            if (IsParmValid(internalParm) == false)
            {
                var c = new CogLineSegment();
                var c2 = new CogLineSegment();
                double[] p = new double[32];
                internalParm = new object[5] { NameParm, c, c2, p, 0 };
            }
        }
        internal static bool IsParmValid(object[] parm)
        {
            if (parm == null)
                return false;
            if (parm.Length == 5 && parm[0] as string == NameParm && parm[1] is CogLineSegment
                && parm[2] is CogLineSegment && parm[3] is double[] && parm[4] is int)
            {
                return true;
            }
            else
                return false;
        }
        public override string ToString()
        {
            return 名称;
        }
        protected override string GetMtdStr()
        {
            return Name;
        }

        [DisplayName("名称")]
        public string 名称
        {
            set
            {
                if (internalParm != null)
                {
                    ICogGraphicInteractive ic = internalParm[1] as ICogGraphicInteractive;
                    if (ic != null)
                        ic.TipText = value;
                }
            }
            get
            {
                if (internalParm != null)
                {
                    ICogGraphicInteractive ic = internalParm[1] as ICogGraphicInteractive;
                    if (ic != null && string.IsNullOrEmpty(ic.TipText) == false)
                        return ic.TipText;
                }
                return Name;
            }
        }
        [DisplayName("间隙1（基准边）位置"), TypeConverter(typeof(MyCogToolConverter)), Editor(typeof(MyCogToolEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public CogLineSegment Region
        {
            set { internalParm[1] = value; }
            get { return internalParm[1] as CogLineSegment; }
        }
        [DisplayName("间隙2（产品边）位置"), TypeConverter(typeof(MyCogToolConverter)), Editor(typeof(MyCogToolEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public CogLineSegment Region2
        {
            set { internalParm[2] = value; }
            get { return internalParm[2] as CogLineSegment; }
        }
        internal double[] p
        {
            set { internalParm[3] = value; }
            get { return (double[])internalParm[3]; }
        }
        [DisplayName("标定系数")]
        public double 标定系数
        {
            set { p[0] = value; }
            get { return p[0]; }
        }
        public double 最小间隙下限
        {
            set { p[1] = value; }
            get { return p[1]; }
        }
        public double 最小间隙上限
        {
            set { p[2] = value; }
            get { return p[2]; }
        }
        public double 搜索长度
        {
            set { p[3] = value; }
            get { return p[3]; }
        }
        public SearchMode 搜索模式
        {
            set { internalParm[4] = (int)value; }
            get { return (SearchMode)(int)internalParm[4]; }
        }
        public enum SearchMode
        {
            沿目标方向最小亮间隙 = 0, 沿目标方向最大暗厚度 = 1, 沿竖直方向最小亮间隙 = 2, 暗区厚度检测 = 3
                , 产品最高点与玻璃盘距离 = 4
        }
    }
    internal class Block1Settingsqipaochk : Block1Settings
    {
        public const string Name = "气泡检测工具";
        public const string NameParm = "qipaochk";

        public Block1Settingsqipaochk(int _index, Block1Collection _parent)
            : base(_index, _parent)
        {//luosichk,CogRectangle,标定系数,内径,内径允许偏差,对角长度,对角允许偏差
            if (IsParmValid(internalParm) == false)
            {
                var c = new CogRectangle();
                double[] p = new double[32];
                internalParm = new object[4] { NameParm, c, p, 0 };
            }
        }
        internal static bool IsParmValid(object[] parm)
        {
            if (parm == null)
                return false;
            if (parm.Length == 4 && parm[0] as string == "qipaochk" && parm[1] is CogRectangle
                && parm[2] is double[] && parm[3] is int)
            {
                return true;
            }
            else
                return false;
        }
        public override string ToString()
        {
            return 名称;
        }
        protected override string GetMtdStr()
        {
            return Name;
        }

        [DisplayName("名称")]
        public string 名称
        {
            set
            {
                if (internalParm != null)
                {
                    ICogGraphicInteractive ic = internalParm[1] as ICogGraphicInteractive;
                    if (ic != null)
                        ic.TipText = value;
                }
            }
            get
            {
                if (internalParm != null)
                {
                    ICogGraphicInteractive ic = internalParm[1] as ICogGraphicInteractive;
                    if (ic != null && string.IsNullOrEmpty(ic.TipText) == false)
                        return ic.TipText;
                }
                return Name;
            }
        }
        [DisplayName("气泡可能出现位置"), TypeConverter(typeof(MyCogToolConverter)), Editor(typeof(MyCogToolEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public ICogRegion Region
        {
            set { internalParm[1] = value; }
            get { return internalParm[1] as ICogRegion; }
        }
        internal double[] p
        {
            set { internalParm[2] = value; }
            get { return (double[])internalParm[2]; }
        }
        [DisplayName("标定系数")]
        public double 标定系数
        {
            set { p[0] = value; }
            get { return p[0]; }
        }
        public double 最小气泡尺寸
        {
            set { p[1] = value; }
            get { return p[1]; }
        }
    }
    internal class Block1Settingsluosichk : Block1Settings
    {
        public const string Name = "螺丝尺寸工具";
        public const string NameParm = "luosichk";

        public Block1Settingsluosichk(int _index, Block1Collection _parent)
            : base(_index, _parent)
        {//luosichk,CogRectangle,标定系数,内径,内径允许偏差,对角长度,对角允许偏差
            if (IsParmValid(internalParm) == false)
            {
                var c = new CogRectangle();
                double[] p = new double[32];
                internalParm = new object[4] { NameParm, c, p, new CogRectangle() };
            }
        }
        internal static bool IsParmValid(object[] parm)
        {
            if (parm == null)
                return false;
            if (parm.Length == 4 && parm[0] as string == "luosichk" && parm[1] is CogRectangle 
                && parm[2] is double[] && parm[3] is CogRectangle)
            {
                return true;
            }
            else
                return false;
        }
        public override string ToString()
        {
            return 名称;
        }
        protected override string GetMtdStr()
        {
            return Name;
        }

        [DisplayName("名称")]
        public string 名称
        {
            set
            {
                if (internalParm != null)
                {
                    ICogGraphicInteractive ic = internalParm[1] as ICogGraphicInteractive;
                    if (ic != null)
                        ic.TipText = value;
                }
            }
            get
            {
                if (internalParm != null)
                {
                    ICogGraphicInteractive ic = internalParm[1] as ICogGraphicInteractive;
                    if (ic != null && string.IsNullOrEmpty(ic.TipText) == false)
                        return ic.TipText;
                }
                return Name;
            }
        }
        [DisplayName("目标螺纹位置"), TypeConverter(typeof(MyCogToolConverter)), Editor(typeof(MyCogToolEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public ICogRegion Region
        {
            set { internalParm[1] = value; }
            get { return internalParm[1] as ICogRegion; }
        }
        [DisplayName("目标螺丝头位置"), TypeConverter(typeof(MyCogToolConverter)), Editor(typeof(MyCogToolEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public ICogRegion Region2
        {
            set { internalParm[3] = value; }
            get { return internalParm[3] as ICogRegion; }
        }
        internal double[] p
        {
            set { internalParm[2] = value; }
            get { return (double[])internalParm[2]; }
        }
        [DisplayName("标定系数")]
        public double 标定系数
        {
            set { p[0] = value; }
            get { return p[0]; }
        }
        public double 头长
        {
            set { p[1] = value; }
            get { return p[1]; }
        }
        public double 头长允许偏差
        {
            set { p[2] = value; }
            get { return p[2]; }
        }
        public double 总长
        {
            set { p[3] = value; }
            get { return (double)p[3]; }
        }
        public double 总长允许偏差
        {
            set { p[4] = value; }
            get { return p[4]; }
        }
        public double 螺纹大径
        {
            set { p[5] = value; }
            get { return p[5]; }
        }
        public double 螺纹小径
        {
            set { p[6] = value; }
            get { return p[6]; }
        }
        public double 螺纹大小径允许偏差
        {
            set { p[7] = value; }
            get { return p[7]; }
        }
    }
    internal class Block1SettingsModelCompareMult : Block1Settings
    {
        public const string Name = "找缺陷工具(多模板)";
        public const string NameParm = "ModelCompareMult";
        internal int selectPtn = 0;

        public Block1SettingsModelCompareMult(int _index, Block1Collection _parent)
            : base(_index, _parent)
        {//LinePair,CogLineSegment,角度误差
            if (IsParmValid(internalParm) == false)
            {
                ICogRegion cr = null;
                if (internalParm != null && internalParm.Length >= 2)
                    cr = internalParm[1] as ICogRegion;
                if (cr == null)
                    cr = new CogRectangle();
                internalParm = new object[13] { NameParm, cr, new List<Cognex.VisionPro.PMAlign.CogPMAlignPattern>{ new CogPMAlignPattern()}
                    , 0.6, 100, 50, ImageProcessing.CogIPTwoImageSubtractOverflowModeConstants.Absolute
                    , 0, 15.0, 5.0, 0.0, 0.0, 0.0 };
            }
        }
        internal static bool IsParmValid(object[] parm)
        {
            if (parm == null)
                return false;
            else if (parm.Length == 13 && parm[0] as string == NameParm && parm[1] is ICogRegion && parm[2] is List<Cognex.VisionPro.PMAlign.CogPMAlignPattern> && parm[3] is double
                && parm[4] is int && parm[5] is int && parm[6] is ImageProcessing.CogIPTwoImageSubtractOverflowModeConstants
                && parm[7] is int && parm[8] is double && parm[9] is double && parm[10] is double
                && parm[11] is double && parm[12] is double)
            {
                return true;
            }
            else
                return false;
        }
        public override string ToString()
        {
            return 名称;
        }
        protected override string GetMtdStr()
        {
            return Name;
        }

        internal CogPMAlignPattern pattern
        {
            get { return ptn[selectPtn]; }
        }
        [DisplayName("名称")]
        public string 名称
        {
            set
            {
                if (internalParm != null)
                {
                    ICogGraphicInteractive ic = internalParm[1] as ICogGraphicInteractive;
                    if (ic != null)
                        ic.TipText = value;
                }
            }
            get
            {
                if (internalParm != null)
                {
                    ICogGraphicInteractive ic = internalParm[1] as ICogGraphicInteractive;
                    if (ic != null && string.IsNullOrEmpty(ic.TipText) == false)
                        return ic.TipText;
                }
                return Name;
            }
        }
        [DisplayName("搜索区域"), TypeConverter(typeof(MyCogToolConverter)), Editor(typeof(MyCogToolEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public ICogRegion Region
        {
            set { internalParm[1] = value as ICogRegion; }
            get { return internalParm[1] as ICogRegion; }
        }
        [DisplayName("模板"), TypeConverter(typeof(MyCogToolsConverter)), Editor(typeof(MyCogToolEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public List<Cognex.VisionPro.PMAlign.CogPMAlignPattern> ptn
        {
            set { internalParm[2] = value; }
            get { return internalParm[2] as List<Cognex.VisionPro.PMAlign.CogPMAlignPattern>; }
        }
        [DisplayName("模版训练"), TypeConverter(typeof(MyTrainCogToolBlockConverter)), Editor(typeof(MyCogToolEditor), typeof(System.Drawing.Design.UITypeEditor))]
        internal CogToolBlock TrainModel
        {
            set { }
            get
            {
                CogToolBlock cb = parent?.internalblock.Tools[NameParm] as CogToolBlock;
                if (cb != null)
                {
                    Region.SelectedSpaceName = ".";
                    pattern.TrainRegion.SelectedSpaceName = ".";
                    cb.Inputs["Parm"].Value = internalParm;
                    if (cb.Inputs.Contains("selectPtn"))
                        cb.Inputs["selectPtn"].Value = selectPtn;
                    else
                        cb.Inputs.Add(new CogToolBlockTerminal("selectPtn", selectPtn));
                    return cb;
                }
                return null;
            }
        }
        [DisplayName("模版区域类型"), TypeConverter(typeof(MyRegionShapeConverter))]
        internal string RegionType
        {
            get { return pattern.TrainRegion.GetType().Name; }
            set
            {
                if (pattern.TrainRegion.GetType().Name != value)
                {
                    if (System.Windows.Forms.MessageBox.Show("确认切换区域类型？", null, System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                        return;
                    switch (value)
                    {
                        case "CogCircle":
                            CogCircle c = new CogCircle();
                            pattern.TrainRegion = c;
                            break;
                        case "CogRectangle":
                            pattern.TrainRegion = new CogRectangle();
                            break;
                        case "CogPolygon":
                            pattern.TrainRegion = new CogPolygon();
                            break;
                        case "CogRectangleAffine":
                            pattern.TrainRegion = new CogRectangleAffine();
                            break;
                        case "CogCircularAnnulusSection":
                            pattern.TrainRegion = new CogCircularAnnulusSection();
                            break;
                    }
                }
            }
        }
        [DisplayName("得分下限")]
        public double 得分下限
        {
            set { internalParm[3] = value; }
            get { return (double)internalParm[3]; }
        }
        [DisplayName("像素差异阈值")]
        public int 像素差异阈值
        {
            set { internalParm[4] = value; }
            get { return (int)internalParm[4]; }
        }
        [DisplayName("最小差异面积")]
        public int 最小差异面积
        {
            set { internalParm[5] = value; }
            get { return (int)internalParm[5]; }
        }
        [DisplayName("差异模式"), TypeConverter(typeof(MyEnumConverter))]
        public ImageProcessing.CogIPTwoImageSubtractOverflowModeConstants OverflowMode
        {
            set { internalParm[6] = value; }
            get { return (ImageProcessing.CogIPTwoImageSubtractOverflowModeConstants)internalParm[6]; }
        }
        [DisplayName("边缘瑕疵过滤")]
        public int ErodeLevel
        {
            set { internalParm[7] = value; }
            get { return (int)internalParm[7]; }
        }
        [DisplayName("位移偏差上限")]
        public double DisThres
        {
            set { internalParm[8] = value; }
            get { return (double)internalParm[8]; }
        }
        [DisplayName("位置旋转上限")]
        public double AngThres
        {
            set { internalParm[9] = value; }
            get { return (double)internalParm[9]; }
        }
        [DisplayName("位置旋转限制")]
        public double AngLimt
        {
            set { internalParm[10] = value; }
            get { return (double)internalParm[10]; }
        }
        [DisplayName("背景漏光判定阈值")]
        internal double HardThreshold
        {
            set { internalParm[11] = value; }
            get { return (double)internalParm[11]; }
        }
        [DisplayName("亮度一致性 、背景漏光判定面积阈值")]
        internal double AreaThres
        {
            set { internalParm[12] = value; }
            get { return (double)internalParm[12]; }
        }
    }
    internal class Block1Settingsluomuchk : Block1Settings
    {
        public const string Name = "螺母(垫片)尺寸工具";
        public const string NameParm = "luomuchk";

        public Block1Settingsluomuchk(int _index, Block1Collection _parent)
            : base(_index, _parent)
        {//luomuchk,CogCircle,标定系数,内径,内径允许偏差,对角长度,对角允许偏差
            if (IsParmValid(internalParm) == false)
            {
                var c = new CogCircularAnnulusSection();
                c.AngleSpan = Math.PI * 2;
                c.GraphicDOFEnable = CogCircularAnnulusSectionDOFConstants.Position | CogCircularAnnulusSectionDOFConstants.Radii;
                internalParm = new object[9] { NameParm, c, 0.0, 3.2, 0.2, 8.79, 0.2, 0.0, 0.0 };
            }
        }
        internal static bool IsParmValid(object[] parm)
        {
            if (parm == null)
                return false;
            if (parm.Length == 9 && parm[0] as string == "luomuchk" && parm[1] is CogCircularAnnulusSection && parm[2] is double && parm[3] is double
                && parm[4] is double && parm[5] is double && parm[6] is double && parm[7] is double && parm[8] is double)
            {
                return true;
            }
            else
                return false;
        }
        public override string ToString()
        {
            return 名称;
        }
        protected override string GetMtdStr()
        {
            return Name;
        }

        [DisplayName("名称")]
        public string 名称
        {
            set
            {
                if (internalParm != null)
                {
                    ICogGraphicInteractive ic = internalParm[1] as ICogGraphicInteractive;
                    if (ic != null)
                        ic.TipText = value;
                }
            }
            get
            {
                if (internalParm != null)
                {
                    ICogGraphicInteractive ic = internalParm[1] as ICogGraphicInteractive;
                    if (ic != null && string.IsNullOrEmpty(ic.TipText) == false)
                        return ic.TipText;
                }
                return Name;
            }
        }
        [DisplayName("目标位置"), TypeConverter(typeof(MyCogToolConverter)), Editor(typeof(MyCogToolEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public CogCircularAnnulusSection Region
        {
            set { internalParm[1] = value; }
            get { return internalParm[1] as CogCircularAnnulusSection; }
        }
        [DisplayName("标定系数")]
        public double 标定系数
        {
            set { internalParm[2] = value; }
            get { return (double)internalParm[2]; }
        }
        public double 内径
        {
            set { internalParm[3] = value; }
            get { return (double)internalParm[3]; }
        }
        [DisplayName("内径允许偏差")]
        public double 内径允许偏差
        {
            set { internalParm[4] = value; }
            get { return (double)internalParm[4]; }
        }
        [DisplayName("对角长度")]
        public double 对角长度
        {
            set { internalParm[5] = value; if (value > 0) internalParm[7] = 0.0; }
            get { return (double)internalParm[5]; }
        }
        [DisplayName("外径长度")]
        public double 外径长度
        {
            set { internalParm[7] = value; if (value > 0) internalParm[5] = 0.0; }
            get { return (double)internalParm[7]; }
        }
        [DisplayName("对角(外径)长度允许偏差")]
        public double 对角长度允许偏差
        {
            set { internalParm[6] = value; }
            get { return (double)internalParm[6]; }
        }
    }
    internal class Block1SettingsIDCmp : Block1Settings
    {
        public const string Name = "读码比较";
        public const string NameParm = "IDCmp";

        public Block1SettingsIDCmp(int _index, Block1Collection _parent)
            : base(_index, _parent)
        {//CompareGlobalModel,CogRectangleAffine,BlobThreshold,blobMinArea
            if (IsParmValid(internalParm) == false)
            {
                object[] obj = new object[5] { NameParm, new CogRectangleAffine(), 0, new int[] { }, new string[] { } };
                if (internalParm != null && internalParm.Length >= 2 && internalParm[1] is CogRectangleAffine)
                    obj[1] = internalParm[1];
                internalParm = obj;
            }
        }
        internal static bool IsParmValid(object[] parm)
        {
            if (parm == null)
                return false;
            if (parm.Length == 5 && parm[0] as string == NameParm && parm[1] is ICogRegion && parm[2] is int && parm[3] is int[] && parm[4] is string[])
                return true;
            else
                return false;
        }
        public override string ToString()
        {
            return 名称;
        }
        protected override string GetMtdStr()
        {
            return Name;
        }

        [DisplayName("名称")]
        public string 名称
        {
            set
            {
                if (internalParm != null)
                {
                    ICogGraphicInteractive ic = internalParm[1] as ICogGraphicInteractive;
                    if (ic != null)
                        ic.TipText = value;
                }
            }
            get
            {
                if (internalParm != null)
                {
                    ICogGraphicInteractive ic = internalParm[1] as ICogGraphicInteractive;
                    if (ic != null && string.IsNullOrEmpty(ic.TipText) == false)
                        return ic.TipText;
                }
                return Name;
            }
        }
        [DisplayName("读码区域"), TypeConverter(typeof(MyCogToolConverter)), Editor(typeof(MyCogToolEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public ICogRegion Region
        {
            set { internalParm[1] = value as ICogRegion; }
            get { return internalParm[1] as ICogRegion; }
        }
        [DisplayName("内容长度")]
        public int 内容长度
        {
            set { internalParm[2] = value; }
            get { return (int)internalParm[2]; }
        }
        [DisplayName("比较位置设定")]
        public int[] 比较位置设定
        {
            set { internalParm[3] = value; }
            get { return (int[])internalParm[3]; }
        }
        [DisplayName("比较内容设定")]
        public string[] 比较内容设定
        {
            set { internalParm[4] = value; }
            get { return (string[])internalParm[4]; }
        }
    }
    internal class Block1SettingsBackGroundCheck : Block1Settings
    {
        public const string Name = "背景漏光工具";
        public const string NameParm = "BackGroundCheck";

        public Block1SettingsBackGroundCheck(int _index, Block1Collection _parent)
            : base(_index, _parent)
        {//CompareGlobalModel,CogRectangleAffine,BlobThreshold,blobMinArea
            if (IsParmValid(internalParm) == false)
            {
                object[] obj = new object[7] { NameParm, new CogRectangleAffine(), 20, 40, 2, 0, 0.0 };
                if (internalParm != null && internalParm.Length >= 2 && internalParm[1] is CogRectangleAffine)
                    obj[1] = internalParm[1];
                internalParm = obj;
            }
        }
        internal static bool IsParmValid(object[] parm)
        {
            if (parm == null)
                return false;
            if (parm.Length == 7 && parm[0] as string == NameParm && parm[1] is ICogRegion
                && parm[2] is int && parm[3] is int && parm[4] is int && parm[5] is int && parm[6] is double)
                return true;
            else
                return false;
        }
        public override string ToString()
        {
            return 名称;
        }
        protected override string GetMtdStr()
        {
            return Name;
        }

        [DisplayName("名称")]
        public string 名称
        {
            set
            {
                if (internalParm != null)
                {
                    ICogGraphicInteractive ic = internalParm[1] as ICogGraphicInteractive;
                    if (ic != null)
                        ic.TipText = value;
                }
            }
            get
            {
                if (internalParm != null)
                {
                    ICogGraphicInteractive ic = internalParm[1] as ICogGraphicInteractive;
                    if (ic != null && string.IsNullOrEmpty(ic.TipText) == false)
                        return ic.TipText;
                }
                return Name;
            }
        }
        [DisplayName("搜索区域"), TypeConverter(typeof(MyCogToolConverter)), Editor(typeof(MyCogToolEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public ICogRegion Region
        {
            set { internalParm[1] = value as ICogRegion; }
            get { return internalParm[1] as ICogRegion; }
        }
        [DisplayName("背景二值化阈值")]
        public int 像素差异阈值
        {
            set { internalParm[2] = value; }
            get { return (int)internalParm[2]; }
        }
        [DisplayName("最小漏光面积")]
        public int 最小差异面积
        {
            set { internalParm[3] = value; }
            get { return (int)internalParm[3]; }
        }
        [DisplayName("边缘过滤")]
        public int 边缘过滤
        {
            set { internalParm[4] = value; }
            get { return (int)internalParm[4]; }
        }
    }
    internal class Block1SettingsChaZheng : Block1Settings
    {
        public const string Name = "插针位置工具";
        public const string NameParm = "ChaZheng";

        public Block1SettingsChaZheng(int _index, Block1Collection _parent)
            : base(_index, _parent)
        {//MQBFangXiang,CogRectangle
            if (IsParmValid(internalParm) == false)
            {
                CogRectangle cr = null;
                if (internalParm != null && internalParm.Length >= 2)
                    cr = internalParm[1] as CogRectangle;
                if (cr == null)
                    cr = new CogRectangle();
                internalParm = new object[10] { NameParm, cr, 2, 2, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
            }
        }
        internal static bool IsParmValid(object[] parm)
        {
            if (parm == null)
                return false;
            if (parm.Length == 10 && parm[0] as string == NameParm && parm[1] is CogRectangle && parm[2] is int && parm[3] is int && parm[4] is double
                && parm[5] is double && parm[6] is double && parm[7] is double && parm[8] is double && parm[9] is double)
                return true;
            else
                return false;
        }
        public override string ToString()
        {
            return 名称;
        }
        protected override string GetMtdStr()
        {
            return Name;
        }

        [DisplayName("名称")]
        public string 名称
        {
            set
            {
                if (internalParm != null)
                {
                    ICogGraphicInteractive ic = internalParm[1] as ICogGraphicInteractive;
                    if (ic != null)
                        ic.TipText = value;
                }
            }
            get
            {
                if (internalParm != null)
                {
                    ICogGraphicInteractive ic = internalParm[1] as ICogGraphicInteractive;
                    if (ic != null && string.IsNullOrEmpty(ic.TipText) == false)
                        return ic.TipText;
                }
                return Name;
            }
        }
        [DisplayName("插针位置"), TypeConverter(typeof(MyCogToolConverter)), Editor(typeof(MyCogToolEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public ICogRegion Region
        {
            set { internalParm[1] = value as ICogRegion; }
            get { return internalParm[1] as ICogRegion; }
        }
        [DisplayName("每列插针数")]
        public int RowCt
        {
            set { internalParm[2] = value; }
            get { return (int)internalParm[2]; }
        }
        [DisplayName("每行插针数")]
        public int ColCt
        {
            set { internalParm[3] = value; }
            get { return (int)internalParm[3]; }
        }
        [DisplayName("比例")]
        public double rate
        {
            set { internalParm[4] = value; }
            get { return (double)internalParm[4]; }
        }
        [DisplayName("距离一致性")]
        public double tol
        {
            set { internalParm[5] = value; }
            get { return (double)internalParm[5]; }
        }
        [DisplayName("列间距下限")]
        public double xmin
        {
            set { internalParm[6] = value; }
            get { return (double)internalParm[6]; }
        }
        [DisplayName("列间距上限")]
        public double xmax
        {
            set { internalParm[7] = value; }
            get { return (double)internalParm[7]; }
        }
        [DisplayName("行间距下限")]
        public double ymin
        {
            set { internalParm[8] = value; }
            get { return (double)internalParm[8]; }
        }
        [DisplayName("行间距上限")]
        public double ymax
        {
            set { internalParm[9] = value; }
            get { return (double)internalParm[9]; }
        }
    }
    internal class Block1SettingsPosCheck : Block1Settings
    {
        public const string Name = "位置偏移工具";
        public const string NameParm = "PosCheck";

        public Block1SettingsPosCheck(int _index, Block1Collection _parent)
            : base(_index, _parent)
        {//MQBFangXiang,CogRectangle
            if (IsParmValid(internalParm) == false)
            {
                CogRectangle cr = null;
                if (internalParm != null && internalParm.Length >= 2)
                    cr = internalParm[1] as CogRectangle;
                if (cr == null)
                    cr = new CogRectangle();
                internalParm = new object[8] { NameParm, cr, 0, 0, 20.0, 0.0, 0.0, 0.0 };
            }
        }
        internal static bool IsParmValid(object[] parm)
        {
            if (parm == null)
                return false;
            if (parm.Length == 8 && parm[0] as string == NameParm && parm[1] is CogRectangle && parm[2] is int && parm[3] is int && parm[4] is double
                && parm[5] is double && parm[6] is double && parm[7] is double)
                return true;
            else
                return false;
        }
        public override string ToString()
        {
            return 名称;
        }
        protected override string GetMtdStr()
        {
            return Name;
        }

        [DisplayName("名称")]
        public string 名称
        {
            set
            {
                if (internalParm != null)
                {
                    ICogGraphicInteractive ic = internalParm[1] as ICogGraphicInteractive;
                    if (ic != null)
                        ic.TipText = value;
                }
            }
            get
            {
                if (internalParm != null)
                {
                    ICogGraphicInteractive ic = internalParm[1] as ICogGraphicInteractive;
                    if (ic != null && string.IsNullOrEmpty(ic.TipText) == false)
                        return ic.TipText;
                }
                return Name;
            }
        }
        [DisplayName("比较工具1")]
        public int ToolA
        {
            set { internalParm[2] = value; }
            get { return (int)internalParm[2]; }
        }
        [DisplayName("比较工具2")]
        public int ToolB
        {
            set { internalParm[3] = value; }
            get { return (int)internalParm[3]; }
        }
        [DisplayName("位置最大差异")]
        public double maxDiff
        {
            set { internalParm[4] = value; }
            get { return (double)internalParm[4]; }
        }
        [DisplayName("位置X偏移")]
        public double XOffset
        {
            set { internalParm[5] = value; }
            get { return (double)internalParm[5]; }
        }
        [DisplayName("位置Y偏移")]
        public double YOffset
        {
            set { internalParm[6] = value; }
            get { return (double)internalParm[6]; }
        }
    }
    internal class Block1SettingsHistUniformity : Block1Settings
    {
        public const string Name = "亮度一致性工具";
        public const string NameParm = "HistUniformity";

        public Block1SettingsHistUniformity(int _index, Block1Collection _parent)
            : base(_index, _parent)
        {//MQBFangXiang,CogRectangle
            if (IsParmValid(internalParm) == false)
            {
                CogRectangle cr = null;
                if (internalParm != null && internalParm.Length >= 2)
                    cr = internalParm[1] as CogRectangle;
                if (cr == null)
                    cr = new CogRectangle();
                internalParm = new object[5] { NameParm, cr, new int[1], 20.0, 0.0 };
            }
        }
        internal static bool IsParmValid(object[] parm)
        {
            if (parm == null)
                return false;
            if (parm.Length == 5 && parm[0] as string == NameParm && parm[1] is CogRectangle && parm[2] is int[] && parm[3] is double && parm[4] is double)
                return true;
            else
                return false;
        }
        public override string ToString()
        {
            return 名称;
        }
        protected override string GetMtdStr()
        {
            return Name;
        }

        [DisplayName("名称")]
        public string 名称
        {
            set
            {
                if (internalParm != null)
                {
                    ICogGraphicInteractive ic = internalParm[1] as ICogGraphicInteractive;
                    if (ic != null)
                        ic.TipText = value;
                }
            }
            get
            {
                if (internalParm != null)
                {
                    ICogGraphicInteractive ic = internalParm[1] as ICogGraphicInteractive;
                    if (ic != null && string.IsNullOrEmpty(ic.TipText) == false)
                        return ic.TipText;
                }
                return Name;
            }
        }
        [DisplayName("判定工具")]
        public int[] filter
        {
            set { internalParm[2] = value; }
            get { return (int[])internalParm[2]; }
        }
        [DisplayName("亮度最大差异")]
        public double maxDiff
        {
            set { internalParm[3] = value; }
            get { return (double)internalParm[3]; }
        }
    }
    internal class Block1SettingsDongChuPian : Block1Settings
    {
        public const string Name = "动触片检测工具";
        public const string NameParm = "DongChuPian";

        public Block1SettingsDongChuPian(int _index, Block1Collection _parent)
            : base(_index, _parent)
        {
            if (IsParmValid(internalParm) == false)
            {
                ICogRegion ic = null;
                if (internalParm != null && internalParm.Length >= 2)
                    ic = internalParm[1] as CogRectangle;
                if (ic == null)
                    ic = new CogRectangle();
                CogRectangle cr3base = new CogRectangle();
                cr3base.Height = 5;
                object[] obj = new object[12] { NameParm, ic, new CogRectangle(), cr3base, new List<CogRectangle>(), 5.0, 10.0, 0.0, 0.0, 0, 0.0, 0.0 };
                if (internalParm != null && internalParm.Length >= 2 && internalParm[1] is CogRectangle)
                    obj[1] = internalParm[1];
                internalParm = obj;
            }
            else
            {
                if (internalParm.Length == 10)
                {
                    object[] obj = new object[12];
                    Array.Copy(internalParm, obj, 10);
                    obj[10] = 0.0;
                    obj[11] = 0.0;
                    internalParm = obj;
                }
            }
        }
        internal static bool IsParmValid(object[] parm)
        {
            if (parm == null)
                return false;
            if (parm.Length >= 10 && parm[0] as string == NameParm && parm[1] is CogRectangle && parm[2] is CogRectangle && parm[3] is CogRectangle
                && parm[4] is List<CogRectangle> && parm[5] is double && parm[6] is double && parm[7] is double && parm[8] is double && parm[9] is int)
            {
                if (parm.Length == 10)
                    return true;
                else if (parm.Length == 12 && parm[10] is double && parm[11] is double)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }
        public override string ToString()
        {
            return 名称;
        }
        protected override string GetMtdStr()
        {
            return Name;
        }

        [DisplayName("名称")]
        public string 名称
        {
            set
            {
                if (internalParm != null)
                {
                    ICogGraphicInteractive ic = internalParm[1] as ICogGraphicInteractive;
                    if (ic != null)
                        ic.TipText = value;
                }
            }
            get
            {
                if (internalParm != null)
                {
                    ICogGraphicInteractive ic = internalParm[1] as ICogGraphicInteractive;
                    if (ic != null && string.IsNullOrEmpty(ic.TipText) == false)
                        return ic.TipText;
                }
                return Name;
            }
        }
        [DisplayName("底座搜索模式")]
        public int 底座搜索模式
        {
            set { internalParm[9] = value; }
            get { return (int)internalParm[9]; }
        }
        [DisplayName("底座区域1"), TypeConverter(typeof(MyCogToolConverter)), Editor(typeof(MyCogToolEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public ICogRegion Region
        {
            set { internalParm[1] = value as ICogRegion; }
            get { return internalParm[1] as ICogRegion; }
        }
        [DisplayName("底座区域2"), TypeConverter(typeof(MyCogToolConverter)), Editor(typeof(MyCogToolEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public ICogRegion Region2
        {
            set { internalParm[2] = value as ICogRegion; }
            get { return internalParm[2] as ICogRegion; }
        }
        [DisplayName("底座区域3"), TypeConverter(typeof(MyCogToolConverter)), Editor(typeof(MyCogToolEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public ICogRegion Region3
        {
            set { internalParm[3] = value as ICogRegion; }
            get { return internalParm[3] as ICogRegion; }
        }
        [DisplayName("动触片区域"), TypeConverter(typeof(MyCogRegionsConverter))]
        public List<CogRectangle> Region4
        {
            set { internalParm[4] = value as List<CogRectangle>; }
            get { return internalParm[4] as List<CogRectangle>; }
        }
        [DisplayName("高度下限")]
        public double 亮度下限
        {
            set { internalParm[5] = value; }
            get { return (double)internalParm[5]; }
        }
        [DisplayName("高度上限")]
        public double 亮度上限
        {
            set { internalParm[6] = value; }
            get { return (double)internalParm[6]; }
        }
        [DisplayName("标定系数")]
        public double 统计下限
        {
            set { internalParm[7] = value; }
            get { return (double)internalParm[7]; }
        }
        [DisplayName("X偏移上限")]
        public double X偏移上限
        {
            set { internalParm[8] = value; }
            get { return (double)internalParm[8]; }
        }
        [DisplayName("高度一致性")]
        public double 高度一致性
        {
            set { internalParm[10] = value; }
            get { return (double)internalParm[10]; }
        }
    }
    internal class Block1SettingsCompareGlobalModel : Block1Settings
    {
        public const string Name = "找缺陷工具(全局定位)";
        public const string NameParm = "CompareGlobalModel";

        public Block1SettingsCompareGlobalModel(int _index, Block1Collection _parent)
            : base(_index, _parent)
        {//CompareGlobalModel,CogRectangleAffine,BlobThreshold,blobMinArea
            if (IsParmValid(internalParm) == false)
            {
                object[] obj = new object[4] { NameParm, new CogRectangleAffine(), 25, 50 };
                if (internalParm != null && internalParm.Length >= 2 && internalParm[1] is CogRectangleAffine)
                    obj[1] = internalParm[1];
                internalParm = obj;
            }
        }
        internal static bool IsParmValid(object[] parm)
        {
            if (parm == null)
                return false;
            if (parm.Length == 4 && parm[0] as string == NameParm && parm[1] is ICogRegion
                && parm[2] is int && parm[3] is int)
                return true;
            else
                return false;
        }
        public override string ToString()
        {
            return 名称;
        }
        protected override string GetMtdStr()
        {
            return Name;
        }

        [DisplayName("名称")]
        public string 名称
        {
            set
            {
                if (internalParm != null)
                {
                    ICogGraphicInteractive ic = internalParm[1] as ICogGraphicInteractive;
                    if (ic != null)
                        ic.TipText = value;
                }
            }
            get
            {
                if (internalParm != null)
                {
                    ICogGraphicInteractive ic = internalParm[1] as ICogGraphicInteractive;
                    if (ic != null && string.IsNullOrEmpty(ic.TipText) == false)
                        return ic.TipText;
                }
                return Name;
            }
        }
        [DisplayName("搜索区域"), TypeConverter(typeof(MyCogToolConverter)), Editor(typeof(MyCogToolEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public ICogRegion Region
        {
            set { internalParm[1] = value as ICogRegion; }
            get { return internalParm[1] as ICogRegion; }
        }
        [DisplayName("像素差异阈值")]
        public int 像素差异阈值
        {
            set { internalParm[2] = value; }
            get { return (int)internalParm[2]; }
        }
        [DisplayName("最小差异面积")]
        public int 最小差异面积
        {
            set { internalParm[3] = value; }
            get { return (int)internalParm[3]; }
        }
    }
    internal class Block1SettingsModelCompareColor : Block1Settings
    {
        public const string Name = "找缺陷工具(颜色)";
        public const string NameParm = "ModelCompareColor";
        const int ParmOffset = 13 - 2;

        public Block1SettingsModelCompareColor(int _index, Block1Collection _parent)
            : base(_index, _parent)
        {//LinePair,CogLineSegment,角度误差
            if (IsParmValid(internalParm) == false)
            {
                ICogRegion cr = null;
                if (internalParm != null && internalParm.Length >= 2)
                    cr = internalParm[1] as ICogRegion;
                if (cr == null)
                    cr = new CogRectangle();
                internalParm = new object[21] { NameParm, cr, new Cognex.VisionPro.PMAlign.CogPMAlignPattern(), 0.6, 90, 50, ImageProcessing.CogIPTwoImageSubtractOverflowModeConstants.Absolute,0,15.0,7.0,0,0,80
                ,0, 0, 85.0, 255.0, System.Drawing.Color.Red, -300, 50,new float[0] };
            }
            else
            {
                if (internalParm.Length == 20)
                {
                    object[] np = new object[21];
                    np[20] = new float[0];
                    Array.Copy(internalParm, np, 20);
                    internalParm = np;
                }
            }
        }
        internal static bool IsParmValid(object[] parm)
        {
            if (parm == null)
                return false;
            if (parm.Length == 20)
            {
                if (parm[ParmOffset + 4] is int)
                    parm[ParmOffset + 4] = (double)(int)parm[ParmOffset + 4];
                if (parm[ParmOffset + 5] is int)
                    parm[ParmOffset + 5] = (double)(int)parm[ParmOffset + 5];
            }
            if (parm.Length >= 20 && parm[0] as string == NameParm && parm[1] is ICogRegion && parm[2] is PMAlign.CogPMAlignPattern && parm[3] is double
                && parm[4] is int && parm[5] is int && parm[6] is ImageProcessing.CogIPTwoImageSubtractOverflowModeConstants && parm[7] is int && parm[8] is double && parm[9] is double
                && parm[10] is int && parm[11] is int && parm[12] is int
                 && parm[2 + ParmOffset] is int && parm[3 + ParmOffset] is int
                && parm[4 + ParmOffset] is double && parm[5 + ParmOffset] is double && parm[6 + ParmOffset] is System.Drawing.Color && parm[7 + ParmOffset] is int && parm[8 + ParmOffset] is int)
            {
                if (parm.Length == 20)
                    return true;
                else if (parm.Length == 21 && parm[9 + ParmOffset] is float[])
                    return true;
                else return false;
            }
            else
                return false;
        }
        public override string ToString()
        {
            return 名称;
        }
        protected override string GetMtdStr()
        {
            return Name;
        }

        internal CogPMAlignPattern pattern
        {
            get { return internalParm[2] as CogPMAlignPattern; }
        }
        [DisplayName("名称")]
        public string 名称
        {
            set
            {
                if (internalParm != null)
                {
                    ICogGraphicInteractive ic = internalParm[1] as ICogGraphicInteractive;
                    if (ic != null)
                        ic.TipText = value;
                }
            }
            get
            {
                if (internalParm != null)
                {
                    ICogGraphicInteractive ic = internalParm[1] as ICogGraphicInteractive;
                    if (ic != null && string.IsNullOrEmpty(ic.TipText) == false)
                        return ic.TipText;
                }
                return Name;
            }
        }
        [DisplayName("搜索、颜色统计区域"), TypeConverter(typeof(MyCogToolConverter)), Editor(typeof(MyCogToolEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public ICogRegion Region
        {
            set { internalParm[1] = value as ICogRegion; }
            get { return internalParm[1] as ICogRegion; }
        }
        [DisplayName("模版区域类型"), TypeConverter(typeof(MyRegionShapeConverter))]
        public string RegionType
        {
            get { return pattern.TrainRegion.GetType().Name; }
            set
            {
                if (pattern.TrainRegion.GetType().Name != value)
                {
                    if (Program.MsgBoxYesNo("确认更改区域类型？"))
                        switch (value)
                        {
                            case "CogCircle":
                                CogCircle c = new CogCircle();
                                pattern.TrainRegion = c;
                                break;
                            case "CogRectangle":
                                pattern.TrainRegion = new CogRectangle();
                                break;
                            case "CogPolygon":
                                pattern.TrainRegion = new CogPolygon();
                                break;
                            case "CogRectangleAffine":
                                pattern.TrainRegion = new CogRectangleAffine();
                                break;
                            case "CogCircularAnnulusSection":
                                pattern.TrainRegion = new CogCircularAnnulusSection();
                                break;
                        }
                }
            }
        }
        [DisplayName("模版训练"), TypeConverter(typeof(MyTrainCogToolBlockConverter)), Editor(typeof(MyCogToolEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public CogToolBlock TrainModel
        {
            set { }
            get
            {
                CogToolBlock cb = parent?.internalblock.Tools[NameParm] as CogToolBlock;
                if (cb != null)
                {
                    cb.Inputs["Parm"].Value = internalParm;
                    return cb;
                }
                return null;
            }
        }
        [DisplayName("得分下限")]
        public double 得分下限
        {
            set { internalParm[3] = value; }
            get { return (double)internalParm[3]; }
        }
        [DisplayName("像素差异阈值")]
        public int 像素差异阈值
        {
            set { internalParm[4] = value; }
            get { return (int)internalParm[4]; }
        }
        [DisplayName("最小差异面积")]
        public int 最小差异面积
        {
            set { internalParm[5] = value; }
            get { return (int)internalParm[5]; }
        }
        [DisplayName("差异模式"), TypeConverter(typeof(MyEnumConverter))]
        public ImageProcessing.CogIPTwoImageSubtractOverflowModeConstants OverflowMode
        {
            set { internalParm[6] = value; }
            get { return (ImageProcessing.CogIPTwoImageSubtractOverflowModeConstants)internalParm[6]; }
        }
        [DisplayName("边缘瑕疵过滤")]
        public int ErodeLevel
        {
            set { internalParm[7] = value; }
            get { return (int)internalParm[7]; }
        }
        [DisplayName("位移偏差上限")]
        public double DisThres
        {
            set { internalParm[8] = value; }
            get { return (double)internalParm[8]; }
        }
        [DisplayName("位置旋转上限")]
        public double AngThres
        {
            set { internalParm[9] = value; }
            get { return (double)internalParm[9]; }
        }
        [DisplayName("亮度一致性最大差异")]
        public int HistMeanDifThres
        {
            set { internalParm[10] = value; }
            get { return (int)internalParm[10]; }
        }
        [DisplayName("背景漏光判定阈值")]
        public int HardThreshold
        {
            set { internalParm[11] = value; }
            get { return (int)internalParm[11]; }
        }
        [DisplayName("亮度一致性 、背景漏光判定面积阈值")]
        internal int AreaThres
        {
            set { internalParm[12] = value; }
            get { return (int)internalParm[12]; }
        }

        [DisplayName("亮度下限")]
        public int 亮度下限
        {
            set { internalParm[2 + ParmOffset] = value; }
            get { return (int)internalParm[2 + ParmOffset]; }
        }
        [DisplayName("亮度上限")]
        public int 亮度上限
        {
            set { internalParm[3 + ParmOffset] = value; }
            get { return (int)internalParm[3 + ParmOffset]; }
        }
        [DisplayName("统计下限")]
        public double 统计下限
        {
            set { internalParm[4 + ParmOffset] = value; }
            get { return (double)internalParm[4 + ParmOffset]; }
        }
        [DisplayName("统计上限")]
        public double 统计上限
        {
            set { internalParm[5 + ParmOffset] = value; }
            get { return (double)internalParm[5 + ParmOffset]; }
        }
        [DisplayName("统计颜色")]
        public System.Drawing.Color 统计颜色
        {
            set { internalParm[6 + ParmOffset] = value; }
            get { return (System.Drawing.Color)internalParm[6 + ParmOffset]; }
        }
        [DisplayName("统计颜色")]
        public string crHsi
        {
            get
            {
                var ptf = 统计颜色.CalcHSI();
                return string.Format("HSI = {0:F1}, {1:F1}, {2:F1}", ptf.Plane0, ptf.Plane1, ptf.Plane2);
            }
        }
        [DisplayName("学习颜色"), TypeConverter(typeof(MyCogToolConverter)), Editor(typeof(MyCogToolEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public System.Drawing.Color TrainColor
        {
            set { internalParm[6 + ParmOffset] = value; }
            get { return (System.Drawing.Color)internalParm[6 + ParmOffset]; }
        }
        [DisplayName("颜色允许偏差")]
        public int 颜色允许偏差
        {
            set { internalParm[7 + ParmOffset] = value; }
            get { return (int)internalParm[7 + ParmOffset]; }
        }
        [DisplayName("颜色统计下限")]
        public int 颜色统计下限
        {
            set { internalParm[8 + ParmOffset] = value; }
            get { return (int)internalParm[8 + ParmOffset]; }
        }
        [DisplayName("色坐标范围"), Editor(typeof(WPColorToolsEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public float[] 色坐标范围
        {
            set { internalParm[9 + ParmOffset] = value; }
            get { return internalParm[9 + ParmOffset] as float[]; }
        }
    }
    internal class Block1SettingsHistColor : Block1Settings
    {
        public const string Name = "颜色亮度统计工具";
        public const string NameParm = "HistColor";
        internal class MyColorCollection : ICustomTypeDescriptor
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
                if (crc == null)
                    return System.ComponentModel.TypeDescriptor.GetProperties(this);
                System.ComponentModel.PropertyDescriptorCollection pdc = System.ComponentModel.TypeDescriptor.GetProperties(this);
                PropertyDescriptor[] props = new PropertyDescriptor[crc.Count + pdc.Count];
                for (int i = 0; i < pdc.Count; i++)
                    props[i] = pdc[i];
                for (int i = 0; i < crc.Count; i++)
                {
                    var val = crc[i];
                    if (val != null)
                        props[i + pdc.Count] = new XPropDescriptor(new XProp(string.Format("颜色{0}", i + 1), new MyColor(val, crc), null, null, new ExpandableObjectConverter()), attributes);
                }
                return new PropertyDescriptorCollection(props);
            }
            public Cognex.VisionPro.ColorSegmenter.CogColorRangeCollection crc = new ColorSegmenter.CogColorRangeCollection();
            public override string ToString()
            {
                if (crc == null)
                    return "null";
                else
                    return string.Format("{0}种颜色", crc.Count);
            }
            public int Count
            {
                get
                {
                    return crc.Count;
                }
                set
                {
                    if (crc.Count > value)
                    {
                        int ct = crc.Count - value;
                        for (int i = 0; i < ct; i++)
                            crc.RemoveAt(value);
                    }
                    else if (crc.Count < value)
                    {
                        int ct = value - crc.Count;
                        for (int i = 0; i < ct; i++)
                            crc.Add(new ColorSegmenter.CogColorRangeItem(new CogSimpleColor(CogImageColorSpaceConstants.RGB)));
                    }
                }
            }

            internal class MyColor
            {
                ColorSegmenter.CogColorRangeItem ccr;
                ColorSegmenter.CogColorRangeCollection parentCollection;
                internal MyColor(ColorSegmenter.CogColorRangeItem _ccr, ColorSegmenter.CogColorRangeCollection _parentCollection)
                {
                    ccr = _ccr;
                    parentCollection = _parentCollection;
                }
                public System.Drawing.Color color
                {
                    get { return ccr.GetColor().SystemColorValue; }
                    set
                    {
                        CogSimpleColor csc = new CogSimpleColor(CogImageColorSpaceConstants.RGB);
                        csc.Plane0 = value.R;
                        csc.Plane1 = value.G;
                        csc.Plane2 = value.B;
                        ccr.SetColor(csc);
                    }
                }
                public double rangeRHighTolerance
                {
                    get
                    {
                        return ccr.PlaneRange0.HighTolerance;
                    }
                    set
                    {
                        ccr.PlaneRange0.UpdateWithClipping(ccr.PlaneRange0.Nominal, ccr.PlaneRange0.LowTolerance, value, 0);
                    }
                }
                public double rangeRLowTolerance
                {
                    get
                    {
                        return ccr.PlaneRange0.LowTolerance;
                    }
                    set
                    {
                        ccr.PlaneRange0.UpdateWithClipping(ccr.PlaneRange0.Nominal, value, ccr.PlaneRange0.HighTolerance, 0);
                    }
                }
                public double rangeGHighTolerance
                {
                    get
                    {
                        return ccr.PlaneRange1.HighTolerance;
                    }
                    set
                    {
                        ccr.PlaneRange1.UpdateWithClipping(ccr.PlaneRange1.Nominal, ccr.PlaneRange1.LowTolerance, value, 0);
                    }
                }
                public double rangeGLowTolerance
                {
                    get
                    {
                        return ccr.PlaneRange1.LowTolerance;
                    }
                    set
                    {
                        ccr.PlaneRange1.UpdateWithClipping(ccr.PlaneRange1.Nominal, value, ccr.PlaneRange1.HighTolerance, 0);
                    }
                }
                public double rangeBHighTolerance
                {
                    get
                    {
                        return ccr.PlaneRange2.HighTolerance;
                    }
                    set
                    {
                        ccr.PlaneRange2.UpdateWithClipping(ccr.PlaneRange2.Nominal, ccr.PlaneRange2.LowTolerance, value, 0);
                    }
                }
                public double rangeBLowTolerance
                {
                    get
                    {
                        return ccr.PlaneRange2.LowTolerance;
                    }
                    set
                    {
                        ccr.PlaneRange2.UpdateWithClipping(ccr.PlaneRange2.Nominal, value, ccr.PlaneRange2.HighTolerance, 0);
                    }
                }
                public override string ToString()
                {
                    if (ccr == null)
                        return "null";
                    else
                        return color.ToString();
                }
                public void Delete()
                {
                    if (parentCollection != null)
                        parentCollection.Remove(ccr);
                }
            }
        }

        public Block1SettingsHistColor(int _index, Block1Collection _parent)
            : base(_index, _parent)
        {//Histogram,Region,亮度下限,亮度上限,统计下限,统计上限,颜色,颜色偏差,颜色统计下限
            if (IsParmValid(internalParm) == false)
            {
                ICogRegion ic = null;
                if (internalParm != null && internalParm.Length >= 2)
                    ic = internalParm[1] as ICogRegion;
                if (ic == null)
                    ic = new CogRectangle();
                object[] obj = new object[9] { NameParm, ic, 50, 150, 100, 1000, System.Drawing.Color.Red, 30, 200 };
                if (internalParm != null && internalParm.Length >= 2 && internalParm[1] is CogRectangle)
                    obj[1] = internalParm[1];
                internalParm = obj;
            }
        }
        internal static bool IsParmValid(object[] parm)
        {
            if (parm == null)
                return false;
            if (parm.Length == 9 && parm[0] as string == NameParm && parm[1] is ICogRegion && parm[2] is int && parm[3] is int
                && parm[4] is int && parm[5] is int && parm[6] is System.Drawing.Color && parm[7] is int && parm[8] is int)
                return true;
            else
                return false;
        }
        public override string ToString()
        {
            return 名称;
        }
        protected override string GetMtdStr()
        {
            return Name;
        }

        [DisplayName("名称")]
        public string 名称
        {
            set
            {
                if (internalParm != null)
                {
                    ICogGraphicInteractive ic = internalParm[1] as ICogGraphicInteractive;
                    if (ic != null)
                        ic.TipText = value;
                }
            }
            get
            {
                if (internalParm != null)
                {
                    ICogGraphicInteractive ic = internalParm[1] as ICogGraphicInteractive;
                    if (ic != null && string.IsNullOrEmpty(ic.TipText) == false)
                        return ic.TipText;
                }
                return Name;
            }
        }
        [DisplayName("检测区域"), TypeConverter(typeof(MyCogToolConverter)), Editor(typeof(MyCogToolEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public ICogRegion Region
        {
            set { internalParm[1] = value as ICogRegion; }
            get { return internalParm[1] as ICogRegion; }
        }
        [DisplayName("区域类型"), TypeConverter(typeof(MyRegionShapeConverter))]
        public string RegionType
        {
            get { return Region.GetType().Name; }
            set
            {
                if (Region.GetType().Name != value)
                {
                    if (System.Windows.Forms.MessageBox.Show("确认切换区域类型？", null, System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                        return;
                    switch (value)
                    {
                        case "CogCircle":
                            CogCircle c = new CogCircle();
                            if (Region != null)
                                c.TipText = ((ICogGraphicInteractive)Region).TipText;
                            Region = c;
                            break;
                        case "CogRectangle":
                            CogRectangle cr = new CogRectangle();
                            if (Region != null)
                                cr.TipText = ((ICogGraphicInteractive)Region).TipText;
                            Region = cr;
                            break;
                        case "CogPolygon":
                            CogPolygon cp = new CogPolygon();
                            if (Region != null)
                                cp.TipText = ((ICogGraphicInteractive)Region).TipText;
                            Region = cp;
                            break;
                        case "CogRectangleAffine":
                            CogRectangleAffine cra = new CogRectangleAffine();
                            if (Region != null)
                                cra.TipText = ((ICogGraphicInteractive)Region).TipText;
                            Region = cra;
                            break;
                        case "CogCircularAnnulusSection":
                            CogCircularAnnulusSection cca = new CogCircularAnnulusSection();
                            if (Region != null)
                                cca.TipText = ((ICogGraphicInteractive)Region).TipText;
                            Region = cca;
                            break;
                    }
                }
            }
        }
        [DisplayName("亮度下限")]
        public int 亮度下限
        {
            set { internalParm[2] = value; }
            get { return (int)internalParm[2]; }
        }
        [DisplayName("亮度上限")]
        public int 亮度上限
        {
            set { internalParm[3] = value; }
            get { return (int)internalParm[3]; }
        }
        [DisplayName("统计下限")]
        public int 统计下限
        {
            set { internalParm[4] = value; }
            get { return (int)internalParm[4]; }
        }
        [DisplayName("统计上限")]
        public int 统计上限
        {
            set { internalParm[5] = value; }
            get { return (int)internalParm[5]; }
        }
        [DisplayName("统计颜色")]
        public System.Drawing.Color 统计颜色
        {
            set { internalParm[6] = value; }
            get { return (System.Drawing.Color)internalParm[6]; }
        }
        [DisplayName("学习颜色"), TypeConverter(typeof(MyCogToolConverter)), Editor(typeof(MyCogToolEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public System.Drawing.Color TrainColor
        {
            set { internalParm[6] = value; }
            get { return (System.Drawing.Color)internalParm[6]; }
        }
        [DisplayName("颜色允许偏差")]
        public int 颜色允许偏差
        {
            set { internalParm[7] = value; }
            get { return (int)internalParm[7]; }
        }
        [DisplayName("颜色统计下限")]
        public int 颜色统计下限
        {
            set { internalParm[8] = value; }
            get { return (int)internalParm[8]; }
        }
    }
    internal class Block1SettingsHistogramCountColor : Block1Settings
    {
        public const string Name = "亮度统计工具(IRGB)";
        public const string NameParm = "HistogramCountColor";

        public Block1SettingsHistogramCountColor(int _index, Block1Collection _parent)
            : base(_index, _parent)
        {//Histogram,Region,parms{亮度下限,亮度上限,统计下限,统计上限}
            if (IsParmValid(internalParm) == false)
            {
                object[] obj = new object[3] { NameParm, new CogRectangle(),new int[16] {
                    50, 150, 100, 1000,50, 150, 100, 1000,50, 150, 100, 1000,50, 150, 100, 1000 } };

                if (internalParm != null && internalParm.Length >= 2 && internalParm[1] is CogRectangle)
                    obj[1] = internalParm[1];
                internalParm = obj;
            }
        }
        internal static bool IsParmValid(object[] parm)
        {
            if (parm == null)
                return false;
            if (parm.Length == 3 && parm[0] as string == NameParm && parm[1] is ICogRegion && parm[2] is int[]
                && ((int[])parm[2]).Length == 16)
                return true;
            else
                return false;
        }
        public override string ToString()
        {
            return 名称;
        }
        protected override string GetMtdStr()
        {
            return Name;
        }

        [DisplayName("名称")]
        public string 名称
        {
            set
            {
                if (internalParm != null)
                {
                    ICogGraphicInteractive ic = internalParm[1] as ICogGraphicInteractive;
                    if (ic != null)
                        ic.TipText = value;
                }
            }
            get
            {
                if (internalParm != null)
                {
                    ICogGraphicInteractive ic = internalParm[1] as ICogGraphicInteractive;
                    if (ic != null && string.IsNullOrEmpty(ic.TipText) == false)
                        return ic.TipText;
                }
                return Name;
            }
        }
        [DisplayName("检测区域"), TypeConverter(typeof(MyCogToolConverter)), Editor(typeof(MyCogToolEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public ICogRegion Region
        {
            set { internalParm[1] = value as ICogRegion; }
            get { return internalParm[1] as ICogRegion; }
        }
        [DisplayName("区域类型"), TypeConverter(typeof(MyRegionShapeConverter))]
        public string RegionType
        {
            get { return Region.GetType().Name; }
            set
            {
                if (Region.GetType().Name != value)
                {
                    switch (value)
                    {
                        case "CogCircle":
                            CogCircle c = new CogCircle();
                            Region = c;
                            break;
                        case "CogRectangle":
                            Region = new CogRectangle();
                            break;
                        case "CogPolygon":
                            Region = new CogPolygon();
                            break;
                        case "CogRectangleAffine":
                            Region = new CogRectangleAffine();
                            break;
                        case "CogCircularAnnulusSection":
                            Region = new CogCircularAnnulusSection();
                            break;
                    }
                }
            }
        }
        int[] parms
        {
            get { return internalParm[2] as int[]; }
        }
        [DisplayName("亮度下限")]
        public int 亮度下限
        {
            set { parms[0] = value; }
            get { return parms[0]; }
        }
        [DisplayName("亮度上限")]
        public int 亮度上限
        {
            set { parms[1] = value; }
            get { return parms[1]; }
        }
        [DisplayName("统计下限")]
        public int 统计下限
        {
            set { parms[2] = value; }
            get { return parms[2]; }
        }
        [DisplayName("统计上限")]
        public int 统计上限
        {
            set { parms[3] = value; }
            get { return parms[3]; }
        }
        [DisplayName("R下限")]
        public int R下限
        {
            set { parms[4] = value; }
            get { return parms[4]; }
        }
        [DisplayName("R上限")]
        public int R上限
        {
            set { parms[5] = value; }
            get { return parms[5]; }
        }
        [DisplayName("R统计下限")]
        public int R统计下限
        {
            set { parms[6] = value; }
            get { return parms[6]; }
        }
        [DisplayName("R统计上限")]
        public int R统计上限
        {
            set { parms[7] = value; }
            get { return parms[7]; }
        }
        [DisplayName("G下限")]
        public int G下限
        {
            set { parms[8] = value; }
            get { return parms[8]; }
        }
        [DisplayName("G上限")]
        public int G上限
        {
            set { parms[9] = value; }
            get { return parms[9]; }
        }
        [DisplayName("G统计下限")]
        public int G统计下限
        {
            set { parms[10] = value; }
            get { return parms[10]; }
        }
        [DisplayName("G统计上限")]
        public int G统计上限
        {
            set { parms[11] = value; }
            get { return parms[11]; }
        }
        [DisplayName("B下限")]
        public int B下限
        {
            set { parms[12] = value; }
            get { return parms[12]; }
        }
        [DisplayName("B上限")]
        public int B上限
        {
            set { parms[13] = value; }
            get { return parms[13]; }
        }
        [DisplayName("B统计下限")]
        public int B统计下限
        {
            set { parms[14] = value; }
            get { return parms[14]; }
        }
        [DisplayName("B统计上限")]
        public int B统计上限
        {
            set { parms[15] = value; }
            get { return parms[15]; }
        }
    }
    internal class Block1SettingsCircleCheck : Block1Settings
    {
        public const string Name = "圆度工具";
        public const string NameParm = "CircleCheck";

        public Block1SettingsCircleCheck(int _index, Block1Collection _parent)
            : base(_index, _parent)
        {//CompareGlobalModel,CogRectangleAffine,BlobThreshold,blobMinArea
            if (IsParmValid(internalParm) == false)
                internalParm = new object[3] { NameParm, new CogCircle(), 10.0 };
        }
        internal static bool IsParmValid(object[] parm)
        {
            if (parm == null)
                return false;
            if (parm.Length == 3 && parm[0] as string == NameParm && parm[1] is CogCircle
                && parm[2] is double)
                return true;
            else
                return false;
        }
        [DisplayName("目标圆位置"), TypeConverter(typeof(MyCogToolConverter)), Editor(typeof(MyCogToolEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public CogCircle Region
        {
            set { internalParm[1] = value as CogCircle; }
            get { return internalParm[1] as CogCircle; }
        }
        [DisplayName("直径差异阈值")]
        public double 直径差异阈值
        {
            set { internalParm[2] = value; }
            get { return (double)internalParm[2]; }
        }

        public override string ToString()
        {
            return 名称;
        }
        protected override string GetMtdStr()
        {
            return Name;
        }
        [DisplayName("名称")]
        public string 名称
        {
            set
            {
                if (internalParm != null)
                {
                    ICogGraphicInteractive ic = internalParm[1] as ICogGraphicInteractive;
                    if (ic != null)
                        ic.TipText = value;
                }
            }
            get
            {
                if (internalParm != null)
                {
                    ICogGraphicInteractive ic = internalParm[1] as ICogGraphicInteractive;
                    if (ic != null && string.IsNullOrEmpty(ic.TipText) == false)
                        return ic.TipText;
                }
                return Name;
            }
        }
    }
    internal class Block1SettingsLocBlobDouble : Block1Settings
    {
        public const string Name = "定位(找两个圆斑点)";
        public const string NameParm = "LocBlobDouble";

        public Block1SettingsLocBlobDouble(int _index, Block1Collection _parent)
            : base(_index, _parent)
        {//CompareGlobalModel,CogRectangleAffine,BlobThreshold,blobMinArea
            if (IsParmValid(internalParm) == false)
                internalParm = new object[4] { NameParm, new CogRectangle(), new CogRectangle(), 50 };
        }
        internal static bool IsParmValid(object[] parm)
        {
            if (parm == null)
                return false;
            if (parm.Length == 4 && parm[0] as string == NameParm && parm[1] is CogRectangle
                && parm[2] is CogRectangle && parm[3] is int)
                return true;
            else
                return false;
        }
        [DisplayName("圆斑1搜索区域"), TypeConverter(typeof(MyCogToolConverter)), Editor(typeof(MyCogToolEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public CogRectangle Region
        {
            set { internalParm[1] = value as CogRectangle; }
            get { return internalParm[1] as CogRectangle; }
        }
        [DisplayName("圆斑2搜索区域"), TypeConverter(typeof(MyCogToolConverter)), Editor(typeof(MyCogToolEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public CogRectangle Region2
        {
            set { internalParm[2] = value as CogRectangle; }
            get { return internalParm[2] as CogRectangle; }
        }
        [DisplayName("二值化阈值")]
        public int 二值化阈值
        {
            set { internalParm[3] = value; }
            get { return (int)internalParm[3]; }
        }

        public override string ToString()
        {
            return 名称;
        }
        protected override string GetMtdStr()
        {
            return Name;
        }
        [DisplayName("名称")]
        public string 名称
        {
            set
            {
                if (internalParm != null)
                {
                    ICogGraphicInteractive ic = internalParm[1] as ICogGraphicInteractive;
                    if (ic != null)
                        ic.TipText = value;
                }
            }
            get
            {
                if (internalParm != null)
                {
                    ICogGraphicInteractive ic = internalParm[1] as ICogGraphicInteractive;
                    if (ic != null && string.IsNullOrEmpty(ic.TipText) == false)
                        return ic.TipText;
                }
                return Name;
            }
        }
    }
    internal class Block1SettingsLoc : Block1Settings
    {
        public const string Name = "定位工具";
        public const string NameParm = "Loc";

        public Block1SettingsLoc(int _index, Block1Collection _parent)
            : base(_index, _parent)
        {//Loc,搜索区域CogRectangle,CogPMAlignPattern,得分下限double
            if (IsParmValid(internalParm) == false)
                internalParm = new object[6] { NameParm, new CogRectangle(), new CogPMAlignPattern(), 0.6, 0, 0.0 };
        }
        internal static bool IsParmValid(object[] parm)
        {
            if (parm == null)
                return false;
            if (parm.Length == 6 && parm[0] as string == NameParm && parm[1] is ICogRegion 
                && parm[2] is PMAlign.CogPMAlignPattern && parm[3] is double && parm[4] is int
                && parm[5] is double
                )
            {
                return true;
            }
            else
                return false;
        }
        public override string ToString()
        {
            return 名称;
        }
        protected override string GetMtdStr()
        {
            return Name;
        }

        internal CogPMAlignPattern pattern
        {
            get { return internalParm[2] as CogPMAlignPattern; }
        }
        [DisplayName("名称")]
        public string 名称
        {
            set
            {
                if (internalParm != null)
                {
                    ICogGraphicInteractive ic = internalParm[1] as ICogGraphicInteractive;
                    if (ic != null)
                        ic.TipText = value;
                }
            }
            get
            {
                if (internalParm != null)
                {
                    ICogGraphicInteractive ic = internalParm[1] as ICogGraphicInteractive;
                    if (ic != null && string.IsNullOrEmpty(ic.TipText) == false)
                        return ic.TipText;
                }
                return Name;
            }
        }
        [DisplayName("搜索区域"), TypeConverter(typeof(MyCogToolConverter)), Editor(typeof(MyCogToolEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public ICogRegion Region
        {
            set { internalParm[1] = value as ICogRegion; }
            get { return internalParm[1] as ICogRegion; }
        }
        [DisplayName("模版训练"), TypeConverter(typeof(MyTrainCogToolBlockConverter)), Editor(typeof(MyCogToolEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public CogToolBlock TrainModel
        {
            set { }
            get
            {
                if (parent != null && parent.internalblock != null && parent.internalblock.Tools.Contains(NameParm) == false)
                    return null;
                CogToolBlock cb = parent?.internalblock.Tools[NameParm] as CogToolBlock;
                if (cb != null)
                {
                    cb.Inputs["Parm"].Value = internalParm;
                    Region.SelectedSpaceName = "@";
                    ((CogPMAlignPattern)internalParm[2]).TrainRegion.SelectedSpaceName = "@";
                    return cb;
                }
                return null;
            }
        }
        [DisplayName("模版区域类型"), TypeConverter(typeof(MyRegionShapeConverter))]
        public string RegionType
        {
            get { return pattern.TrainRegion.GetType().Name; }
            set
            {
                if (pattern.TrainRegion.GetType().Name != value)
                {
                    if (System.Windows.Forms.MessageBox.Show("确认切换区域类型？", null, System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                        return;
                    switch (value)
                    {
                        case "CogCircle":
                            CogCircle c = new CogCircle();
                            pattern.TrainRegion = c;
                            break;
                        case "CogRectangle":
                            pattern.TrainRegion = new CogRectangle();
                            break;
                        case "CogPolygon":
                            pattern.TrainRegion = new CogPolygon();
                            break;
                        case "CogRectangleAffine":
                            pattern.TrainRegion = new CogRectangleAffine();
                            break;
                        case "CogCircularAnnulusSection":
                            pattern.TrainRegion = new CogCircularAnnulusSection();
                            break;
                    }
                }
            }
        }
        [DisplayName("得分下限")]
        public double 得分下限
        {
            set { internalParm[3] = value; }
            get { return (double)internalParm[3]; }
        }
        [DisplayName("边缘瑕疵过滤")]
        public int 最小差异面积
        {
            set { internalParm[4] = value; }
            get { return (int)internalParm[4]; }
        }
        [DisplayName("旋转限制")]
        public double 旋转限制
        {
            set { internalParm[5] = value; }
            get { return (double)internalParm[5]; }
        }
    }
    internal class Block1SettingsHistogramCount : Block1Settings
    {
        public const string Name = "亮度统计工具";
        public const string NameParm = "HistogramCount";

        public Block1SettingsHistogramCount(int _index, Block1Collection _parent)
            : base(_index, _parent)
        {//Histogram,Region,亮度下限,亮度上限,统计下限,统计上限
            if (IsParmValid(internalParm) == false)
            {
                ICogRegion ic = null;
                if (internalParm != null && internalParm.Length >= 2)
                    ic = internalParm[1] as ICogRegion;
                if (ic == null)
                    ic = new CogRectangle();
                internalParm = new object[6] { NameParm, ic, 50, 150, 100, 1000 };
            }
        }
        internal static bool IsParmValid(object[] parm)
        {
            if (parm == null)
                return false;
            if (parm.Length == 6 && parm[0] as string == NameParm && parm[1] is ICogRegion && parm[2] is int && parm[3] is int
                && parm[4] is int && parm[5] is int)
                return true;
            else
                return false;
        }
        public override string ToString()
        {
            return 名称;
        }
        protected override string GetMtdStr()
        {
            return Name;
        }

        [DisplayName("名称")]
        public string 名称
        {
            set
            {
                if (internalParm != null)
                {
                    ICogGraphicInteractive ic = internalParm[1] as ICogGraphicInteractive;
                    if (ic != null)
                        ic.TipText = value;
                }
            }
            get
            {
                if (internalParm != null)
                {
                    ICogGraphicInteractive ic = internalParm[1] as ICogGraphicInteractive;
                    if (ic != null && string.IsNullOrEmpty(ic.TipText) == false)
                        return ic.TipText;
                }
                return Name;
            }
        }
        [DisplayName("检测区域"), TypeConverter(typeof(MyCogToolConverter)), Editor(typeof(MyCogToolEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public ICogRegion Region
        {
            set { internalParm[1] = value as ICogRegion; }
            get { return internalParm[1] as ICogRegion; }
        }
        [DisplayName("区域类型"), TypeConverter(typeof(MyRegionShapeConverter))]
        public string RegionType
        {
            get { return Region.GetType().Name; }
            set
            {
                if (Region.GetType().Name != value)
                {
                    if (System.Windows.Forms.MessageBox.Show("确认切换区域类型？", null, System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                        return;
                    switch (value)
                    {
                        case "CogCircle":
                            CogCircle c = new CogCircle();
                            if (Region != null)
                                c.TipText = ((ICogGraphicInteractive)Region).TipText;
                            Region = c;
                            break;
                        case "CogRectangle":
                            CogRectangle cr = new CogRectangle();
                            if (Region != null)
                                cr.TipText = ((ICogGraphicInteractive)Region).TipText;
                            Region = cr;
                            break;
                        case "CogPolygon":
                            CogPolygon cp = new CogPolygon();
                            if (Region != null)
                                cp.TipText = ((ICogGraphicInteractive)Region).TipText;
                            Region = cp;
                            break;
                        case "CogRectangleAffine":
                            CogRectangleAffine cra = new CogRectangleAffine();
                            if (Region != null)
                                cra.TipText = ((ICogGraphicInteractive)Region).TipText;
                            Region = cra;
                            break;
                        case "CogCircularAnnulusSection":
                            CogCircularAnnulusSection cca = new CogCircularAnnulusSection();
                            if (Region != null)
                                cca.TipText = ((ICogGraphicInteractive)Region).TipText;
                            Region = cca;
                            break;
                    }
                }
            }
        }
        [DisplayName("亮度下限")]
        public int 亮度下限
        {
            set { internalParm[2] = value; }
            get { return (int)internalParm[2]; }
        }
        [DisplayName("亮度上限")]
        public int 亮度上限
        {
            set { internalParm[3] = value; }
            get { return (int)internalParm[3]; }
        }
        [DisplayName("统计下限")]
        public int 统计下限
        {
            set { internalParm[4] = value; }
            get { return (int)internalParm[4]; }
        }
        [DisplayName("统计上限")]
        public int 统计上限
        {
            set { internalParm[5] = value; }
            get { return (int)internalParm[5]; }
        }
    }
    internal class Block1SettingsModelCompare : Block1Settings
    {
        public const string Name = "找缺陷工具";
        public const string NameParm = "ModelCompare";

        public Block1SettingsModelCompare(int _index, Block1Collection _parent)
            : base(_index, _parent)
        {//LinePair,CogLineSegment,角度误差
            if (IsParmValid(internalParm) == false)
            {
                ICogRegion cr = null;
                if (internalParm != null && internalParm.Length >= 2)
                    cr = internalParm[1] as ICogRegion;
                if (cr == null)
                    cr = new CogRectangle();
                internalParm = new object[13] { NameParm, cr, new CogPMAlignPattern()
                    , 0.6, 100, 50, ImageProcessing.CogIPTwoImageSubtractOverflowModeConstants.Absolute
                    , 0, 15.0, 5.0, 0.0, 0.0, 0.0 };
            }
        }
        internal static bool IsParmValid(object[] parm)
        {
            if (parm == null)
                return false;
            else if (parm.Length == 13 && parm[0] as string == NameParm && parm[1] is ICogRegion && parm[2] is CogPMAlignPattern && parm[3] is double
                && parm[4] is int && parm[5] is int && parm[6] is ImageProcessing.CogIPTwoImageSubtractOverflowModeConstants 
                && parm[7] is int && parm[8] is double && parm[9] is double && parm[10] is double
                && parm[11] is double && parm[12] is double)
            {
                return true;
            }
            else
                return false;
        }
        public override string ToString()
        {
            return 名称;
        }
        protected override string GetMtdStr()
        {
            return Name;
        }

        internal CogPMAlignPattern pattern
        {
            get { return internalParm[2] as CogPMAlignPattern; }
        }
        [DisplayName("名称")]
        public string 名称
        {
            set
            {
                if (internalParm != null)
                {
                    ICogGraphicInteractive ic = internalParm[1] as ICogGraphicInteractive;
                    if (ic != null)
                        ic.TipText = value;
                }
            }
            get
            {
                if (internalParm != null)
                {
                    ICogGraphicInteractive ic = internalParm[1] as ICogGraphicInteractive;
                    if (ic != null && string.IsNullOrEmpty(ic.TipText) == false)
                        return ic.TipText;
                }
                return Name;
            }
        }
        [DisplayName("搜索区域"), TypeConverter(typeof(MyCogToolConverter)), Editor(typeof(MyCogToolEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public ICogRegion Region
        {
            set { internalParm[1] = value as ICogRegion; }
            get { return internalParm[1] as ICogRegion; }
        }
        [DisplayName("模版训练"), TypeConverter(typeof(MyTrainCogToolBlockConverter)), Editor(typeof(MyCogToolEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public CogToolBlock TrainModel
        {
            set { }
            get
            {
                CogToolBlock cb = parent?.internalblock.Tools[NameParm] as CogToolBlock;
                if (cb != null)
                {
                    Region.SelectedSpaceName = ".";
                    pattern.TrainRegion.SelectedSpaceName = ".";
                    cb.Inputs["Parm"].Value = internalParm;
                    return cb;
                }
                return null;
            }
        }
        [DisplayName("模版区域类型"), TypeConverter(typeof(MyRegionShapeConverter))]
        public string RegionType
        {
            get { return pattern.TrainRegion.GetType().Name; }
            set
            {
                if (pattern.TrainRegion.GetType().Name != value)
                {
                    if (System.Windows.Forms.MessageBox.Show("确认切换区域类型？", null, System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                        return;
                    switch (value)
                    {
                        case "CogCircle":
                            CogCircle c = new CogCircle();
                            pattern.TrainRegion = c;
                            break;
                        case "CogRectangle":
                            pattern.TrainRegion = new CogRectangle();
                            break;
                        case "CogPolygon":
                            pattern.TrainRegion = new CogPolygon();
                            break;
                        case "CogRectangleAffine":
                            pattern.TrainRegion = new CogRectangleAffine();
                            break;
                        case "CogCircularAnnulusSection":
                            pattern.TrainRegion = new CogCircularAnnulusSection();
                            break;
                    }
                }
            }
        }
        [DisplayName("得分下限")]
        public double 得分下限
        {
            set { internalParm[3] = value; }
            get { return (double)internalParm[3]; }
        }
        [DisplayName("像素差异阈值")]
        public int 像素差异阈值
        {
            set { internalParm[4] = value; }
            get { return (int)internalParm[4]; }
        }
        [DisplayName("最小差异面积")]
        public int 最小差异面积
        {
            set { internalParm[5] = value; }
            get { return (int)internalParm[5]; }
        }
        [DisplayName("差异模式"), TypeConverter(typeof(MyEnumConverter))]
        public ImageProcessing.CogIPTwoImageSubtractOverflowModeConstants OverflowMode
        {
            set { internalParm[6] = value; }
            get { return (ImageProcessing.CogIPTwoImageSubtractOverflowModeConstants)internalParm[6]; }
        }
        [DisplayName("边缘瑕疵过滤")]
        public int ErodeLevel
        {
            set { internalParm[7] = value; }
            get { return (int)internalParm[7]; }
        }
        [DisplayName("位移偏差上限")]
        public double DisThres
        {
            set { internalParm[8] = value; }
            get { return (double)internalParm[8]; }
        }
        [DisplayName("位置旋转上限")]
        public double AngThres
        {
            set { internalParm[9] = value; }
            get { return (double)internalParm[9]; }
        }
        [DisplayName("位置旋转限制")]
        public double AngLimt
        {
            set { internalParm[10] = value; }
            get { return (double)internalParm[10]; }
        }
        [DisplayName("背景漏光判定阈值")]
        internal double HardThreshold
        {
            set { internalParm[11] = value; }
            get { return (double)internalParm[11]; }
        }
        [DisplayName("亮度一致性 、背景漏光判定面积阈值")]
        internal double AreaThres
        {
            set { internalParm[12] = value; }
            get { return (double)internalParm[12]; }
        }
    }
    internal class Block1SettingsBlob : Block1Settings
    {
        public const string Name = "找斑点工具";
        public const string NameParm = "Blob";

        public Block1SettingsBlob(int _index, Block1Collection _parent)
            : base(_index, _parent)
        {//Blob,Region,分割阈值,面积下限,极性,斑点个数下限,斑点个数上限
            if (IsParmValid(internalParm) == false)
                internalParm = new object[9] { NameParm, new CogRectangle(), new CogCircle(), 50, 50, 0, 0, 0, 0 };
        }
        internal static bool IsParmValid(object[] parm)
        {
            if (parm == null)
                return false;
            if (parm.Length == 9 && parm[0] as string == "Blob" && parm[1] is ICogRegion && parm[2] is CogCircle && parm[3] is int && parm[4] is int && parm[5] is int && parm[6] is int && parm[7] is int && parm[8] is int)
                return true;
            else
                return false;
        }
        public override string ToString()
        {
            return 名称;
        }
        protected override string GetMtdStr()
        {
            return Name;
        }

        [DisplayName("名称")]
        public string 名称
        {
            set
            {
                if (internalParm != null)
                {
                    ICogGraphicInteractive ic = internalParm[1] as ICogGraphicInteractive;
                    if (ic != null)
                        ic.TipText = value;
                }
            }
            get
            {
                if (internalParm != null)
                {
                    ICogGraphicInteractive ic = internalParm[1] as ICogGraphicInteractive;
                    if (ic != null && string.IsNullOrEmpty(ic.TipText) == false)
                        return ic.TipText;
                }
                return Name;
            }
        }
        [DisplayName("检测区域"), TypeConverter(typeof(MyCogToolConverter)), Editor(typeof(MyCogToolEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public ICogGraphicInteractive Region
        {
            set { internalParm[1] = value as ICogGraphicInteractive; }
            get { return internalParm[1] as ICogGraphicInteractive; }
        }
        [DisplayName("区域类型"), TypeConverter(typeof(MyRegionShapeConverter))]
        public string RegionType
        {
            get { return Region.GetType().Name; }
            set
            {
                if (Region.GetType().Name != value)
                {
                    if (System.Windows.Forms.MessageBox.Show("确认切换区域类型？", null, System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                        return;
                    switch (value)
                    {
                        case "CogCircle":
                            CogCircle c = new CogCircle();
                            if (Region != null)
                                c.TipText = ((ICogGraphicInteractive)Region).TipText;
                            Region = c;
                            break;
                        case "CogRectangle":
                            CogRectangle cr = new CogRectangle();
                            if (Region != null)
                                cr.TipText = ((ICogGraphicInteractive)Region).TipText;
                            Region = cr;
                            break;
                        case "CogPolygon":
                            CogPolygon cp = new CogPolygon();
                            if (Region != null)
                                cp.TipText = ((ICogGraphicInteractive)Region).TipText;
                            Region = cp;
                            break;
                        case "CogRectangleAffine":
                            CogRectangleAffine cra = new CogRectangleAffine();
                            if (Region != null)
                                cra.TipText = ((ICogGraphicInteractive)Region).TipText;
                            Region = cra;
                            break;
                        case "CogCircularAnnulusSection":
                            CogCircularAnnulusSection cca = new CogCircularAnnulusSection();
                            if (Region != null)
                                cca.TipText = ((ICogGraphicInteractive)Region).TipText;
                            Region = cca;
                            break;
                        case "CogEllipse":
                            CogEllipse ce = new CogEllipse();        
                            //CogEllipticalArc ce = new CogEllipticalArc();
                            if (Region != null)
                                ce.TipText = ((ICogGraphicInteractive)Region).TipText;
                            Region = ce;
                            break;
                    }
                }
            }
        }
        [DisplayName("目标斑点"), TypeConverter(typeof(MyCogToolConverter)), Editor(typeof(MyCogToolEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public CogCircle DstCircle
        {
            set { internalParm[2] = value; }
            get { return internalParm[2] as CogCircle; }
        }
        [DisplayName("二值化阈值")]
        public int 二值化阈值
        {
            set { internalParm[3] = value; }
            get { return (int)internalParm[3]; }
        }
        [DisplayName("斑点最小面积")]
        public int 斑点最小面积
        {
            set { internalParm[4] = value; }
            get { return (int)internalParm[4]; }
        }
        [DisplayName("查找模式")]
        public BlbMode 查找模式
        {
            set { internalParm[5] = (int)value; }
            get { return (BlbMode)(int)internalParm[5]; }
        }
        [DisplayName("背景过滤二值化阈值")]
        public int bTHres
        {
            set { internalParm[6] = value; }
            get { return (int)internalParm[6]; }
        }
        [DisplayName("背景过滤腐蚀系数(临近滤波系数)")]
        public int erode
        {
            set { internalParm[7] = value; }
            get { return (int)internalParm[7]; }
        }
        [DisplayName("临近滤波系数")]
        internal int ljfix
        {
            set { internalParm[8] = value; }
            get { return (int)internalParm[8]; }
        }

        public enum BlbMode
        {
            有亮斑, 有暗斑, 无亮斑, 无暗斑, 阈值平均自适应
        }
    }
    internal class Block1SettingsHistogram : Block1Settings
    {
        public const string Name = "亮度工具";
        public const string NameParm = "Histogram";

        public Block1SettingsHistogram(int _index, Block1Collection _parent)
            : base(_index, _parent)
        {//Histogram,Region,亮度下限,亮度上限
            if (IsParmValid(internalParm) == false)
            {
                CogRectangle cr = null;
                if (internalParm != null && internalParm.Length >= 2)
                    cr = internalParm[1] as CogRectangle;
                if (cr == null)
                    cr = new CogRectangle();
                internalParm = new object[4] { NameParm, cr, 50, 150 };
            }
        }
        internal static bool IsParmValid(object[] parm)
        {
            if (parm == null)
                return false;
            if (parm.Length == 4 && parm[0] as string == "Histogram" && parm[1] is ICogRegion && parm[2] is int && parm[3] is int)
                return true;
            else
                return false;
        }
        public override string ToString()
        {
            return 名称;
        }
        protected override string GetMtdStr()
        {
            return Name;
        }

        [DisplayName("名称")]
        public string 名称
        {
            set
            {
                if (internalParm != null)
                {
                    ICogGraphicInteractive ic = internalParm[1] as ICogGraphicInteractive;
                    if (ic != null)
                        ic.TipText = value;
                }
            }
            get
            {
                if (internalParm != null)
                {
                    ICogGraphicInteractive ic = internalParm[1] as ICogGraphicInteractive;
                    if (ic != null && string.IsNullOrEmpty(ic.TipText) == false)
                        return ic.TipText;
                }
                return Name;
            }
        }
        [DisplayName("检测区域"), TypeConverter(typeof(MyCogToolConverter)), Editor(typeof(MyCogToolEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public ICogRegion Region
        {
            set { internalParm[1] = value as ICogRegion; }
            get { return internalParm[1] as ICogRegion; }
        }
        [DisplayName("区域类型"), TypeConverter(typeof(MyRegionShapeConverter))]
        public string RegionType
        {
            get { return Region.GetType().Name; }
            set
            {
                if (Region.GetType().Name != value)
                {
                    if (System.Windows.Forms.MessageBox.Show("确认切换区域类型？", null, System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                        return;
                    switch (value)
                    {
                        case "CogCircle":
                            CogCircle c = new CogCircle();
                            if (Region != null)
                                c.TipText = ((ICogGraphicInteractive)Region).TipText;
                            Region = c;
                            break;
                        case "CogRectangle":
                            CogRectangle cr = new CogRectangle();
                            if (Region != null)
                                cr.TipText = ((ICogGraphicInteractive)Region).TipText;
                            Region = cr;
                            break;
                        case "CogPolygon":
                            CogPolygon cp = new CogPolygon();
                            if (Region != null)
                                cp.TipText = ((ICogGraphicInteractive)Region).TipText;
                            Region = cp;
                            break;
                        case "CogRectangleAffine":
                            CogRectangleAffine cra = new CogRectangleAffine();
                            if (Region != null)
                                cra.TipText = ((ICogGraphicInteractive)Region).TipText;
                            Region = cra;
                            break;
                        case "CogCircularAnnulusSection":
                            CogCircularAnnulusSection cca = new CogCircularAnnulusSection();
                            if (Region != null)
                                cca.TipText = ((ICogGraphicInteractive)Region).TipText;
                            Region = cca;
                            break;
                    }
                }
            }
        }
        [DisplayName("亮度下限")]
        public int 亮度下限
        {
            set { internalParm[2] = value; }
            get { return (int)internalParm[2]; }
        }
        [DisplayName("亮度上限")]
        public int 亮度上限
        {
            set { internalParm[3] = value; }
            get { return (int)internalParm[3]; }
        }
    }
    internal class Block1SettingsLinePair : Block1Settings
    {
        public const string Name = "亮线工具";
        public const string NameParm = "LinePair";

        public Block1SettingsLinePair(int _index, Block1Collection _parent)
            : base(_index, _parent)
        {//LinePair,CogLineSegment,角度误差
            if (IsParmValid(internalParm) == false)
                internalParm = new object[6] { NameParm, new CogLineSegment(), 3.0, 1, 0.0, 0.0 };
            else if (internalParm.Length == 4)
            {
                object[] obj2 = new object[6];
                Array.Copy(internalParm, obj2, 4);
                obj2[4] = 0.0;
                obj2[5] = 0.0;
            }
        }
        internal static bool IsParmValid(object[] parm)
        {
            if (parm == null)
                return false;
            if (parm.Length >= 4 && parm[0] as string == "LinePair" && parm[1] is CogLineSegment && parm[2] is double && parm[3] is int)
            {
                if (parm.Length == 4)
                    return true;
                else if (parm.Length == 6 && parm[4] is double && parm[5] is double)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }
        public override string ToString()
        {
            return 名称;
        }
        protected override string GetMtdStr()
        {
            return Name;
        }

        [DisplayName("名称")]
        public string 名称
        {
            set
            {
                if (internalParm != null)
                {
                    ICogGraphicInteractive ic = internalParm[1] as ICogGraphicInteractive;
                    if (ic != null)
                        ic.TipText = value;
                }
            }
            get
            {
                if (internalParm != null)
                {
                    ICogGraphicInteractive ic = internalParm[1] as ICogGraphicInteractive;
                    if (ic != null && string.IsNullOrEmpty(ic.TipText) == false)
                        return ic.TipText;
                }
                return Name;
            }
        }
        [DisplayName("目标亮线"), TypeConverter(typeof(MyCogToolConverter)), Editor(typeof(MyCogToolEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public CogLineSegment Region
        {
            set { internalParm[1] = value as CogLineSegment; }
            get { return internalParm[1] as CogLineSegment; }
        }
        [DisplayName("角度误差")]
        public double 角度误差
        {
            set { internalParm[2] = value; }
            get { return (double)internalParm[2]; }
        }
        [DisplayName("边缘查找模式"), TypeConverter(typeof(边缘查找模式Converter))]
        public int 边缘查找模式
        {
            set { internalParm[3] = value; }
            get { return (int)internalParm[3]; }
        }
        [DisplayName("线宽下限")]
        public double 线宽下限
        {
            set { internalParm[4] = value; }
            get { return (double)internalParm[4]; }
        }
        [DisplayName("线宽上限")]
        public double 线宽上限
        {
            set { internalParm[5] = value; }
            get { return (double)internalParm[5]; }
        }

        internal class 边缘查找模式Converter : ExpandableObjectConverter
        {
            List<string> li = new List<string>() { "Single Edge", "Edge Pair, DarkToLight, LightToDark" };
            public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
            {
                return true;
            }
            public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                StandardValuesCollection vals = new TypeConverter.StandardValuesCollection(li);
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
            public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
            {
                if (value is int)
                    if ((int)value == 1)
                        return li[1];
                    else
                        return li[0];
                else if (destinationType == typeof(int) && value is string)
                {
                    if ((string)value == li[1])
                        return 1;
                    else
                        return 0;

                }
                return base.ConvertTo(context, culture, value, destinationType);
            }
            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                return true;
            }
            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            {
                return true;
            }
            public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
            {
                if (value is string)
                    if ((string)value == li[1])
                        return 1;
                    else
                        return 0;
                return base.ConvertFrom(context, culture, value);
            }
        }
    }
    internal class Block1SettingsUnknown : Block1Settings
    {
        public const string NameParm = "InvalidNameParm";
        public const string Name = "未知工具";
        public Block1SettingsUnknown(int _index, Block1Collection _parent)
            : base(_index, _parent)
        {

        }
        protected override string GetMtdStr()
        {
            return Name;
        }
        public override string ToString()
        {
            object[] parm = internalParm;
            if (parm != null && parm.Length > 0)
                return string.Format("{0}_{1}", Name, parm[0]);
            else
                return string.Format("{0}_Null", Name);
        }
    }
    internal class Block1SettingsPatCheck : Block1Settings
    {
        public const string Name = "特征工具";
        public const string NameParm = "PatCheck";

        public Block1SettingsPatCheck(int _index, Block1Collection _parent)
            : base(_index, _parent)
        {//LinePair,CogLineSegment,角度误差
            if (IsParmValid(internalParm) == false)
            {
                CogRectangle cr = null;
                if (internalParm != null && internalParm.Length >= 2)
                    cr = internalParm[1] as CogRectangle;
                if (cr == null)
                    cr = new CogRectangle();
                internalParm = new object[4] { NameParm, cr, new Cognex.VisionPro.PMAlign.CogPMAlignPattern(), 0.6 };
            }
        }
        internal static bool IsParmValid(object[] parm)
        {
            if (parm == null)
                return false;
            if (parm.Length == 4 && parm[0] as string == NameParm && parm[1] is ICogRegion && parm[2] is PMAlign.CogPMAlignPattern && parm[3] is double)
                return true;
            else
                return false;
        }
        public override string ToString()
        {
            return 名称;
        }
        protected override string GetMtdStr()
        {
            return Name;
        }

        [DisplayName("名称")]
        public string 名称
        {
            set
            {
                if (internalParm != null)
                {
                    ICogGraphicInteractive ic = internalParm[1] as ICogGraphicInteractive;
                    if (ic != null)
                        ic.TipText = value;
                }
            }
            get
            {
                if (internalParm != null)
                {
                    ICogGraphicInteractive ic = internalParm[1] as ICogGraphicInteractive;
                    if (ic != null && string.IsNullOrEmpty(ic.TipText) == false)
                        return ic.TipText;
                }
                return Name;
            }
        }
        [DisplayName("搜索区域"), TypeConverter(typeof(MyCogToolConverter)), Editor(typeof(MyCogToolEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public ICogRegion Region
        {
            set { internalParm[1] = value as ICogRegion; }
            get { return internalParm[1] as ICogRegion; }
        }
        [DisplayName("模版训练"), TypeConverter(typeof(MyTrainCogToolBlockConverter)), Editor(typeof(MyCogToolEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public CogToolBlock TrainModel
        {
            set { }
            get
            {
                CogToolBlock cb = parent?.internalblock.Tools[NameParm] as CogToolBlock;
                if (cb != null)
                {
                    cb.Inputs["Parm"].Value = internalParm;
                    return cb;
                }
                return null;
            }
        }
        [DisplayName("得分下限")]
        public double 得分下限
        {
            set { internalParm[3] = value; }
            get { return (double)internalParm[3]; }
        }
    }
    internal class Block1SettingsIDTool : Block1Settings
    {
        public const string Name = "条码识别";
        public const string NameParm = "IDTool";

        public Block1SettingsIDTool(int _index, Block1Collection _parent)
            : base(_index, _parent)
        {//IDTool,Region
            if (IsParmValid(internalParm) == false)
            {
                CogRectangle cr = null;
                if (internalParm != null && internalParm.Length >= 2)
                    cr = internalParm[1] as CogRectangle;
                if (cr == null)
                    cr = new CogRectangle();
                internalParm = new object[2] { NameParm, cr };
            }
        }
        internal static bool IsParmValid(object[] parm)
        {
            if (parm == null)
                return false;
            if (parm.Length == 2 && parm[0] as string == "IDTool" && parm[1] is ICogRegion)
                return true;
            else
                return false;
        }
        public override string ToString()
        {
            return 名称;
        }
        protected override string GetMtdStr()
        {
            return Name;
        }

        [DisplayName("名称")]
        public string 名称
        {
            set
            {
                if (internalParm != null)
                {
                    ICogGraphicInteractive ic = internalParm[1] as ICogGraphicInteractive;
                    if (ic != null)
                        ic.TipText = value;
                }
            }
            get
            {
                if (internalParm != null)
                {
                    ICogGraphicInteractive ic = internalParm[1] as ICogGraphicInteractive;
                    if (ic != null && string.IsNullOrEmpty(ic.TipText) == false)
                        return ic.TipText;
                }
                return Name;
            }
        }
        [DisplayName("识别区域"), TypeConverter(typeof(MyCogToolConverter)), Editor(typeof(MyCogToolEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public ICogRegion Region
        {
            set { internalParm[1] = value as ICogRegion; }
            get { return internalParm[1] as ICogRegion; }
        }
    }
    #endregion
    internal class Block1Collection : BlockBase, ICustomTypeDescriptor
    {
        internal static Dictionary<string, Type> NameDic = new Dictionary<string, Type>();
        static Block1Collection()
        {
            try
            {
                //var tps = System.Reflection.Assembly.GetCallingAssembly().GetTypes();
                //var bstp = typeof(Block1Settings);
                //foreach (var item in tps)
                //{
                //    if (item.BaseType?.Name == bstp.Name)
                //    {
                //        var fd = item.GetField("NameParm");
                //        if (fd != null)
                //        {
                //            var vv = fd.GetValue(null) as string;
                //            NameDic.Add(vv, item);
                //        }
                //        fd = item.GetField("Name");
                //        if (fd != null)
                //        {
                //            var vv = fd.GetValue(null) as string;
                //            NameDic.Add(vv, item);
                //        }
                //    }
                //}
                //NameDic.Add(Block1Settingsqipaochk.Name, typeof(Block1Settingsqipaochk));
                //NameDic.Add(Block1Settingsluosichk.Name, typeof(Block1Settingsluosichk));
                //NameDic.Add(Block1SettingsModelCompareMult.Name, typeof(Block1SettingsModelCompareMult));
                //foreach (var nm in NameDic)
                //{
                //    Block1Settings.MyMtdConverter.li.Add(nm.Key);
                //}
            }
            catch (System.Reflection.ReflectionTypeLoadException ex)
            {
                Program.ErrHdl(ex.InnerException);
                for (int i = 0; i < ex.LoaderExceptions.Length; i++)
                {
                    Program.ErrHdl(ex.LoaderExceptions[i]);
                }
            }
            catch (Exception ex)
            {
                Program.ErrHdl(ex);
            }
        }
        internal static void init()
        {
            var tps = System.Reflection.Assembly.GetCallingAssembly().GetTypes();
            var bstp = typeof(Block1Settings);
            foreach (var item in tps)
            {
                if (item.BaseType?.Name == bstp.Name)
                {
                    var fd = item.GetField("NameParm");
                    if (fd != null)
                    {
                        var vv = fd.GetValue(null) as string;
                        NameDic.Add(vv, item);
                    }
                    fd = item.GetField("Name");
                    if (fd != null)
                    {
                        var vv = fd.GetValue(null) as string;
                        NameDic.Add(vv, item);
                        //Block1Settings.MyMtdConverter.li.Add(vv);
                    }
                }
            }
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
            if (Parms.Count != list.Count)
                SyncParms();
            System.ComponentModel.PropertyDescriptorCollection pdc = System.ComponentModel.TypeDescriptor.GetProperties(this);
            PropertyDescriptor[] props = new PropertyDescriptor[list.Count + pdc.Count];
            for (int i = 0; i < pdc.Count; i++)
                props[i] = pdc[i];
            for (int i = 0; i < list.Count; i++)
            {
                var val = list[i];
                if (val != null)
                    props[i + pdc.Count] = new XPropDescriptor(new XProp(string.Format("工具{0}", i + 1), val, null, null, new ExpandableObjectConverter()), attributes);
                else
                    props[i + pdc.Count] = new XPropDescriptor(new XProp(string.Format("工具{0}", i + 1), string.Empty, null, null, new ExpandableObjectConverter()), attributes);
            }
            return new PropertyDescriptorCollection(props);
        }
        internal List<Block1Settings> list;
        string vppfilePath = "b{0}.vpp";
        internal Block1Collection(int id = 0)
        {
            blockid = id;
            vppfilePath = string.Format(vppfilePath, id);
            if (System.IO.File.Exists(vppfilePath))
                internalblock = CogSerializer.LoadObjectFromFile(vppfilePath) as CogToolBlock;
            else
            {
                string fn = "a" + blockid;
                internalblock = CogSerializer.LoadObjectFromStream(AESMtd.AESDecrypt(fn)) as CogToolBlock;
            }
            vppfilePath = null;
            list = new List<Block1Settings>();
            //if (internalblock != null && internalblock.Inputs.Contains("CameraHdl"))
            //    internalblock.Inputs["CameraHdl"].Value = photometricaHdl;
            thread.Start();
        }
        internal Block1Collection(Block1Collection b0, int id)
        {
            blockid = id;
            if (b0 != null && b0.internalblock != null)
            {
                vppfilePath = null;
                internalblock = (CogToolBlock)CogSerializer.DeepCopyObject(b0.internalblock);
                list = new List<Block1Settings>();
                //if (internalblock != null && internalblock.Inputs.Contains("CameraHdl"))
                //    internalblock.Inputs["CameraHdl"].Value = photometricaHdl;
                thread.Start();
            }
        }
        internal override void SaveMtd()
        {
            if (internalblock != null && vppfilePath != null)
            {
                internalblock.Inputs[0].Value = null;
                CogSerializer.SaveObjectToFile(internalblock, vppfilePath);
            }
        }
        internal override R1Class GenResult(object[] re, double ImageGrabTime, double AlgTime, ICogRecord record)
        {
            return new R1Class(re, ImageGrabTime, AlgTime, record);
        }
        [DisplayName("工具数")]
        public int Count
        {
            get { return Parms == null ? 0 : Parms.Count; }
            set
            {
                if (Parms.Count > value)
                {
                    Parms.RemoveRange(value, Parms.Count - value);
                    SyncParms();
                }
                else if (Parms.Count < value)
                {
                    object[][] objs = new object[value - Parms.Count][];
                    if (Parms.Count > 0)
                    {
                        object[] cpy = Parms[Parms.Count - 1];
                        for (int i = 0; i < objs.Length; i++)
                        {
                            objs[i] = CogSerializer.DeepCopyObject(cpy) as object[];
                        }
                    }
                    Parms.AddRange(objs);
                    SyncParms();
                }
            }
        }
        [DisplayName("转化模式"), TypeConverter(typeof(MyEnumConverter))]
        public ImageProcessing.CogImageConvertRunModeConstants runMode
        {
            get
            {
                return (ImageProcessing.CogImageConvertRunModeConstants)internalblock.Inputs["RunMode"].Value;
            }
            set
            {
                if (Program.MsgBoxYesNo("确认更改转化模式？"))
                    internalblock.Inputs["RunMode"].Value = value;
            }
        }
        [DisplayName("训练所有"), TypeConverter(typeof(MyTrainAllToolsCogToolBlockConverter)), Editor(typeof(MyTrainAllCogToolsEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public CogToolBlock trainAll
        {
            set { internalblock = value; }
            get { return internalblock; }
        }
        internal int Index
        {
            get { return (int)internalblock.Inputs["RunIndex"].Value; }
            set
            {
                internalblock.Inputs["RunIndex"].Value = value;
            }
        }
        [DisplayName("Parms"), Browsable(false)]
        public List<object[]> Parms
        {
            get
            {
                List<object[]> obj = internalblock.Inputs["Parms"].Value as List<object[]>;
                if (obj == null)
                {
                    obj = new List<object[]>();
                    internalblock.Inputs["Parms"].Value = obj;
                }
                return obj;
            }
        }
        internal void SyncParms()
        {
            if (Parms.Count > list.Count)
            {
                Block1Settings[] b1s = new Block1Settings[Parms.Count - list.Count];
                for (int i = 0; i < b1s.Length; i++)
                {
                    b1s[i] = Block1Settings.Create(list.Count + i, this);
                }
                list.AddRange(b1s);
            }
            else if (Parms.Count < list.Count)
            {
                list.RemoveRange(Parms.Count, list.Count - Parms.Count);
            }

        }
        public void ClearParmsSettings()
        {
            list.Clear();
        }
    }
    internal abstract class Block1Settings
    {
        internal static Block1Settings Create(int _index, Block1Collection _parent)
        {
            if (_parent != null)
            {
                object[] parm = _parent.Parms[_index];
                if (parm != null && parm.Length > 1)
                {
                    string str = parm[0] as string;
                    //Type tp = Type.GetType("Cognex.VisionPro.Block1Settings" + str);
                    //if (tp != null)
                    //{
                    //}
                    //else
                    //    Program.ErrHdl("内部错误：未知参数类型 " + str);
                    if (Block1Collection.NameDic.TryGetValue(str, out var tp))
                    {
                        var validmtd = tp.GetMethod("IsParmValid", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
                        if (validmtd != null)
                        {
                            if ((bool)validmtd.Invoke(null, new object[] { parm }))
                            {
                                if (Activator.CreateInstance(tp, new object[] { _index, _parent }) is Block1Settings re)
                                    return re;
                            }
                        }
                    }
                }
            }
            return new Block1SettingsUnknown(_index, _parent);
        }
        [DisplayName("检测模式"), TypeConverter(typeof(MyMtdConverter))]
        public string Mtd
        {
            get { return GetMtdStr(); }
            set
            {
                if (GetMtdStr() != value && parent != null && parent.list.Count > index)
                {
                    if (System.Windows.Forms.MessageBox.Show("是否确认修改检测模式？", "", System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                        return;
                    Block1Settings bb;
                    Program.Loginfo("{0}-{1}：{2} -> {3}".FormatWith(ParmsInputDialogue.SelectedString, ParmsInputDialogue.GetLabel(ParmsInputDialogue.selectedItemGlobal), GetMtdStr(), value));
                    if (Block1Collection.NameDic.TryGetValue(value, out Type tp))
                    {
                        parent.list[index] = Activator.CreateInstance(tp, new object[] { index, parent }) as Block1Settings;
                    }
                    else
                        Program.ErrHdl("无法找到" + value);
                }
            }
        }

        protected object[] _internalParm;
        protected object[] internalParm
        {
            get
            {
                if (parent != null)
                    return parent.Parms[index];
                else
                    return _internalParm;
            }
            set
            {
                if (parent != null)
                    parent.Parms[index] = value;
                else
                    _internalParm = value;
            }
        }
        protected Block1Collection parent;
        protected int index;
        internal void SetParent(Block1Collection _parent, int _index)
        {
            if (_parent != null && _index >= 0 && _index < _parent.Count)
            {
                _parent.Parms[_index] = _internalParm;
                index = _index;
                parent = _parent;
            }
        }
        internal Block1Settings(int _index, Block1Collection _parent)
        {
            parent = _parent;
            index = _index;
        }

        protected abstract string GetMtdStr();
        internal void Insert()
        {
            if (parent != null && parent.Parms.Count == parent.list.Count)
            {
                object[] parm = CogSerializer.DeepCopyObject(internalParm) as object[];
                parent.Parms.Insert(index, parm);
                parent.list.Insert(index, Block1Settings.Create(index, parent));
                for (int i = index; i < parent.list.Count; i++)
                {
                    parent.list[i].index = i;
                }
            }
        }
        internal void MoveUp()
        {
            if (parent != null && parent.Parms.Count == parent.list.Count && index > 0)
            {
                int id = index;
                object[] obj = internalParm;
                Block1Settings bs = parent.list[id];
                parent.Parms[id] = parent.Parms[id - 1];
                parent.list[id] = parent.list[id - 1];

                parent.Parms[id - 1] = obj;
                parent.list[id - 1] = bs;

                parent.list[id - 1].index = id - 1;
                parent.list[id].index = id;
            }
        }
        internal void MoveDown()
        {
            if (parent != null && parent.Parms.Count == parent.list.Count && index < parent.Parms.Count - 1)
            {
                int id = index;
                object[] obj = internalParm;
                Block1Settings bs = parent.list[id];
                parent.Parms[id] = parent.Parms[id + 1];
                parent.list[id] = parent.list[id + 1];

                parent.Parms[id + 1] = obj;
                parent.list[id + 1] = bs;

                parent.list[id + 1].index = id + 1;
                parent.list[id].index = id;
            }
        }
        internal void Remove()
        {
            if (parent != null && parent.Parms.Count == parent.list.Count)
            {
                parent.Parms.RemoveAt(index);
                parent.list.RemoveAt(index);
                for (int i = index; i < parent.list.Count; i++)
                {
                    parent.list[i].index = i;
                }
            }
        }
        internal void Append()
        {
            if (parent != null && parent.Parms.Count == parent.list.Count)
            {
                object[] parm = CogSerializer.DeepCopyObject(internalParm) as object[];
                parent.Parms.Add(parm);
                parent.list.Add(Block1Settings.Create(parent.Parms.Count - 1, parent));
            }
        }
        internal void Append(Block1Settings source)
        {
            if (parent != null && parent.Parms.Count == parent.list.Count && source != null)
            {
                object[] parm = CogSerializer.DeepCopyObject(source.internalParm) as object[];
                parent.Parms.Add(parm);
                parent.list.Add(Block1Settings.Create(parent.Parms.Count - 1, parent));
            }
        }

        internal class MyRegionShapeConverter : ExpandableObjectConverter
        {
            public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
            {
                return true;
            }
            public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                List<string> li = new List<string>() { "CogCircle", "CogRectangle", "CogPolygon", "CogRectangleAffine", "CogCircularAnnulusSection", "CogEllipse" };
                StandardValuesCollection vals = new TypeConverter.StandardValuesCollection(li);
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
        }
        internal class MyMtdConverter : ExpandableObjectConverter
        {
            internal static List<string> li = new List<string>() {
                Block1SettingsLoc.Name,Block1Settinggunzisize.Name,Block1Settinggunzichk.Name
                ,Block1Settingsluomuchk.Name,Block1Settingsluosichk.Name
                ,Block1Settingsgapchk.Name,Block1Settingshanliaochk.Name
                ,Block1SettingsHistogram.Name, Block1SettingsPatCheck.Name
                ,Block1SettingsBlob.Name,Block1Settingsqipaochk.Name
                ,Block1SettingsModelCompare.Name,Block1SettingsModelCompareMult.Name
                ,Block1SettingsHistogramCount.Name,Block1SettingsCompareGlobalModel.Name
                ,Block1SettingsHistColor.Name,Block1SettingsDongChuPian.Name
                ,Block1SettingsHistUniformity.Name
                ,Block1SettingsPosCheck.Name,Block1SettingsChaZheng.Name
                ,Block1SettingsBackGroundCheck.Name,Block1SettingsIDCmp.Name
            };
            public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
            {
                return true;
            }
            public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                StandardValuesCollection vals = new TypeConverter.StandardValuesCollection(li);
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
        }
    }
}
