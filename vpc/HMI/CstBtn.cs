using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace vpc
{
    public class CstImgBtn : Button
    {
        public Image icon;
        public CstImgBtn()
        {
            this.SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint
                | ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);
        }
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            if (icon == null || Width + 30 < Height)
                base.OnPaint(e);
            else
            {
                base.OnPaintBackground(e);
                float w = Height * 0.5f;
                float x = Height * 0.6f;
                float y = Height * 0.25f;
                e.Graphics.DrawImage(icon, x, y, w, w);
                //e.Graphics.DrawImage(Properties.Resources.pic_lo, x, y, w, w);
                using (SolidBrush b = new SolidBrush(ForeColor))
                {
                    //e.Graphics.FillPath(b, path);
                    //e.Graphics.FillPath(b, pa);
                    Brush brush = new SolidBrush(this.ForeColor);
                    //Pen penn = new Pen(brush, 3);
                    StringFormat gs = new StringFormat();
                    gs.Alignment = StringAlignment.Near; //居中
                    gs.LineAlignment = StringAlignment.Center;//垂直居中
                    e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                    int sx = (int)(x + w + y / 2);
                    e.Graphics.DrawString(this.Text, Font, brush, new Rectangle(sx, 0, Width - sx, Height), gs);
                }
            }
        }
    }
    public class CstBtn : Button
    {
        private Color enterForeColor = Color.White;
        private Color leftForeColor = Color.Black;
        private bool Isleft = true;
        public int type = 0;
        
        public bool IsLEFT { set; get; }
        public Color EnterForeColor
        {
            get { return enterForeColor; }
            set
            {
                this.enterForeColor = value;
                this.ForeColor = value;
            }
        }
        public Color LeftForeColor
        {
            get { return leftForeColor; }
            set
            {
                this.leftForeColor = value;
                this.ForeColor = value;
            }
        }
        public CstBtn()
        {
            BackgroundImage = vpc.Properties.Resources.btn;
            BackgroundImageLayout = ImageLayout.Stretch;
            BackColor = Color.Transparent;
            this.SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint
                | ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x00000020;//WS_EX_TRANSPARENT
                return cp;
            }
        }
        protected override void OnMouseEnter(EventArgs e)//鼠标进入时
        {
            BackgroundImage = vpc.Properties.Resources.btnMouseOver;
            base.OnMouseEnter(e);
            this.ForeColor = this.EnterForeColor;
        }
        protected override void OnMouseLeave(EventArgs e)//鼠标离开
        {
            BackgroundImage = vpc.Properties.Resources.btn;
            base.OnMouseLeave(e);
            this.ForeColor = this.LeftForeColor;
        }
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            //if (type == 0)
            base.OnPaintBackground(e);
            //base.OnPaint(e);
            //e.Graphics.DrawImage(BackgroundImage, e.ClipRectangle);
            Color baseColor = this.BackColor;

            using (SolidBrush b = new SolidBrush(baseColor))
            {
                //e.Graphics.FillPath(b, path);
                //e.Graphics.FillPath(b, pa);
                System.Drawing.Font fo = new System.Drawing.Font(this.Font.Name, this.Font.Size);
                Brush brush = new SolidBrush(this.ForeColor);
                Pen penn = new Pen(brush, 3);
                StringFormat gs = new StringFormat();
                gs.Alignment = StringAlignment.Center; //居中
                gs.LineAlignment = StringAlignment.Center;//垂直居中
                e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                e.Graphics.DrawString(this.Text, fo, brush, e.ClipRectangle, gs);
            }
            return;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            Rectangle rect = new Rectangle(0, 0, this.Width, this.Height);
            var path = GetRoundedRectPath(rect);
            this.Region = new Region(path);

            //var pa = RectPath(rect);
            //this.Region = new Region(pa);
        }
        private GraphicsPath RectPath(Rectangle re)
        {
            GraphicsPath path = new GraphicsPath();
            Point[] ps = new Point[4];
            ps[0] = new Point(this.Width / 5, this.Height / 5);
            ps[1] = new Point(4 * this.Width / 5, this.Height / 5);
            ps[2] = new Point(this.Width / 5, 4 * this.Height / 5);
            ps[3] = new Point(4 * this.Width / 5, 4 * this.Height / 5);
            path.AddLines(ps);
            path.CloseFigure();

            return path;
        }
        private GraphicsPath GetRoundedRectPath(Rectangle rect)
        {
            Rectangle arcRect = new Rectangle(rect.Location, new System.Drawing.Size(this.Height, this.Height));
            GraphicsPath path = new GraphicsPath();
            Point[] p = new Point[12];
            if (Isleft == true)
            {
                p[0] = new Point(2 * this.Width / 5, 0);
                p[1] = new Point(0, this.Height / 2);
                p[2] = new Point(2 * this.Width / 5, this.Height);

                p[3] = new Point(this.Width, 0);
                p[4] = new Point(4 * this.Width / 5, this.Height / 4);
                p[5] = new Point(4 * this.Width / 5, 3 * this.Height / 4);
                p[6] = new Point(this.Width, this.Height);
            }
            else
            {
                p[0] = new Point(3 * this.Width / 5, 0);
                p[1] = new Point(this.Width, this.Height / 2);
                p[2] = new Point(3 * this.Width / 5, this.Height);

                p[3] = new Point(0, 0);
                p[4] = new Point(1 * this.Width / 5, this.Height / 4);
                p[5] = new Point(1 * this.Width / 5, 3 * this.Height / 4);
                p[6] = new Point(0, this.Height);
            }
            path.AddLine(p[0], p[1]);
            path.AddLine(p[1], p[2]);
            path.AddBezier(p[6], p[5], p[4], p[3]);
            path.CloseFigure();
            return path;
        }
    }
}
