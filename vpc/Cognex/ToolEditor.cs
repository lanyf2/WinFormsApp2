using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Cognex.VisionPro;

namespace vpc
{
    internal partial class ToolEditor : Form
    {
        internal ToolEditor(ICogGraphicInteractive graph, string title = "图形编辑")
        {
            InitializeComponent();
            cogDisplay1.Image = Form1.cogDisplayStatic.Image;
            this.Text = title;
            if (graph != null)
            {
                graph.Visible = true;
                graph.Interactive = true;
                cogDisplay1.InteractiveGraphics.Add(graph, null, false);
            }
        }
        internal ToolEditor(ICogGraphicInteractive[] graph, string title = "图形编辑")
        {
            InitializeComponent();
            this.Text = title;
            if (graph == null || graph.Length == 0)
                MessageBox.Show("无可编辑图形");
            else
            {
                cogDisplay1.DrawingEnabled = false;
                cogDisplay1.Image = Form1.cogDisplayStatic.Image;
                for (int i = 0; i < graph.Length; i++)
                {
                    //graph[i].Color = CogColorConstants.Yellow;
                    graph[i].LineWidthInScreenPixels = 2;
                    if (graph[i] is Cognex.VisionPro.CogCircularArc)
                        ((Cognex.VisionPro.CogCircularArc)graph[i]).GraphicDOFEnable = CogCircularArcDOFConstants.ArcSpan | CogCircularArcDOFConstants.Position;
                    else
                        graph[i].GraphicDOFEnableBase = CogGraphicDOFConstants.All;
                    cogDisplay1.InteractiveGraphics.Add(graph[i], null, false);
                    graph[i].Visible = true;
                }
                cogDisplay1.DrawingEnabled = true;
            }
        }
        internal ToolEditor(CogGraphicInteractiveCollection graph, string title = "图形编辑")
        {
            InitializeComponent();
            this.Text = title;
            CogMaskListDisplay cm = new CogMaskListDisplay(graph);
            defaultDisplay = cm;
            propertyGrid1.SelectedObject = cm;
            cm.RegionAdded += Cm_RegionAdded;
            cm.RegionRemoved += Cm_RegionRemoved;
            this.FormClosing += ToolEditor_FormClosing;
            cogDisplay1.Image = Form1.cogDisplayStatic.Image;

            if (graph == null)
                MessageBox.Show("参数有误");
            else if (graph.Count > 0)
            {
                cogDisplay1.DrawingEnabled = false;
                //for (int i = 0; i < graph.Count; i++)
                //{
                //    cogDisplay1.InteractiveGraphics.Add(graph[i], null, false);
                //    graph[i].Visible = true;
                //}
                cogDisplay1.InteractiveGraphics.AddList(graph, null, false);
                cogDisplay1.DrawingEnabled = true;
            }
        }
        internal ToolEditor(Color color, string title = "颜色训练")
        {
            InitializeComponent();
            cogDisplay1.MouseDown -= cogDisplay1_MouseDown;
            cogDisplay1.DoubleClick -= cogDisplay1_DoubleClick;
            cogDisplay1.MouseUp += cogDisplay1_MouseUp;
            this.FormClosing += ToolEditor_FormClosing;

            TrainedColor = color;
            CogImage24PlanarColor img = Form1.cogDisplayStatic.Image as CogImage24PlanarColor;
            if (img != null)
                cogDisplay1.Image = img;
            else if (Form1.cogImage24PlanarStatic != null)
            {
                cogDisplay1.Image = img = Form1.cogImage24PlanarStatic;
            }
            else
            {
                Program.MsgBox("未采集彩色图像");
                return;
            }
            this.Text = title;
            CogRectangle cr = new CogRectangle();
            cr.Interactive = true;
            cr.Visible = true;
            cr.GraphicDOFEnable = CogRectangleDOFConstants.All;
            cogDisplay1.InteractiveGraphics.Add(cr, null, false);
            colorTraining = new ColorDisplay(cr, img);
            propertyGrid1.SelectedObject = colorTraining;
        }

        private void ToolEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                CogMaskListDisplay cm = defaultDisplay as CogMaskListDisplay;
                if (cm != null)
                {
                    cm.RegionAdded -= Cm_RegionAdded;
                    cm.RegionRemoved -= Cm_RegionRemoved;
                }
                else if (cogDisplay1.Selection.Count > 0 && colorTraining != null)
                {
                    TrainedColor = colorTraining.color;
                }
            }
            catch (Exception ex)
            {
                Program.MsgBox(ex.Message);
            }
        }

        private void Cm_RegionRemoved(object sender, EventArgs e)
        {
            ICogGraphicInteractive ic = sender as ICogGraphicInteractive;
            if (ic != null)
            {
                int id = cogDisplay1.InteractiveGraphics.FindItem(ic, Cognex.VisionPro.Display.CogDisplayZOrderConstants.Front);
                if (id >= 0)
                {
                    cogDisplay1.InteractiveGraphics.Remove(id);
                }
            }
            else if (sender is CogGraphicInteractiveCollection)
            {
                cogDisplay1.InteractiveGraphics.Clear();
            }
        }
        private void Cm_RegionAdded(object sender, EventArgs e)
        {
            if (sender is ICogGraphicInteractive)
                cogDisplay1.InteractiveGraphics.Add(sender as ICogGraphicInteractive, null, false);
        }
        object defaultDisplay;
        int selectedVertex = 0;
        Point ptClicked = new Point();
        internal System.Drawing.Color TrainedColor;
        ColorDisplay colorTraining;

        #region DisplayDefine
        public class ColorDisplay
        {
            CogRectangle ac;
            CogImage24PlanarColor img;
            [DisplayName("颜色")]
            public System.Drawing.Color color
            {
                get
                {
                    CogSimpleColor sc = new CogSimpleColor(CogImageColorSpaceConstants.RGB);
                    sc.UpdateUsingMeanColorFromImageRegion(img, ac, CogImageColorSpaceConstants.RGB);
                    return sc.SystemColorValue;
                }
            }
            public ColorDisplay(CogRectangle arc, CogImage24PlanarColor _img)
            {
                ac = arc;
                img = _img;
            }
        }
        public class CogRectangleDisplay
        {
            public CogRectangleDisplay(CogRectangle arc)
            {
                ac = arc;
            }
            CogRectangle ac;
            [DisplayName("中心X(pixel)")]
            public double centerX
            {
                get { return ac.CenterX; }
            }
            [DisplayName("中心Y(pixel)")]
            public double centerY
            {
                get { return ac.CenterY; }
            }
            [DisplayName("左上X(pixel)")]
            public double locationX
            {
                set { ac.X = value; }
                get { return ac.X; }
            }
            [DisplayName("左上Y(pixel)")]
            public double locationY
            {
                set { ac.Y = value; }
                get { return ac.Y; }
            }
            [DisplayName("宽(pixel)")]
            public double radius
            {
                set { ac.Width = value; }
                get { return ac.Width; }
            }
            [DisplayName("高(pixel)")]
            public double height
            {
                set { ac.Height = value; }
                get { return ac.Height; }
            }
        }
        public class CogPolygonDisplay
        {
            public CogPolygonDisplay(CogPolygon po)
            {
                polygon = po;
            }
            CogPolygon polygon;
            [DisplayName("顶点数")]
            public int NumVertices
            {
                get { return polygon.NumVertices; }
            }
            [DisplayName("增加顶点"), Editor(typeof(PolygonVerticesToolEditor), typeof(System.Drawing.Design.UITypeEditor))]
            public string AddVertices
            {
                set { }
                get { return "在边上双击可直接增加"; }
            }
            [DisplayName("删除顶点"), Editor(typeof(PolygonVerticesToolEditor), typeof(System.Drawing.Design.UITypeEditor))]
            public string RemoveVertices
            {
                set { }
                get { return "在顶点上双击可直接删除"; }
            }

            public void addVertices()
            {
                polygon.AddVertex(100, 100, -1);
            }
            public void removeVertices()
            {
                if (polygon.NumVertices > 3)
                    polygon.RemoveVertex(polygon.NumVertices - 1);
            }
        }
        internal class CogMaskListDisplay
        {
            CogGraphicInteractiveCollection mask;
            internal event EventHandler RegionAdded;
            internal event EventHandler RegionRemoved;
            internal CogMaskListDisplay(CogGraphicInteractiveCollection graph)
            {
                mask = graph;
            }
            [DisplayName("图形数")]
            public int NumVertices
            {
                get { return mask.Count; }
            }
            [DisplayName("增加屏蔽区域"), Editor(typeof(MaskListToolEditor), typeof(System.Drawing.Design.UITypeEditor))]
            public string addRegion
            {
                set { }
                get { return string.Empty; }
            }
            [DisplayName("删除屏蔽区域"), Editor(typeof(MaskListToolEditor), typeof(System.Drawing.Design.UITypeEditor))]
            public string removeRegion
            {
                set { }
                get
                {
                    if (selectedObj != null)
                    {
                        int id = mask.IndexOf(selectedObj);
                        return id.ToString();
                    }
                    return string.Empty;
                }
            }
            [DisplayName("清空屏蔽区域"), Editor(typeof(MaskListToolEditor), typeof(System.Drawing.Design.UITypeEditor))]
            public string clearRegion
            {
                set { }
                get { return string.Empty; }
            }
            internal ICogGraphicInteractive selectedObj = null;
            internal void AddRegion()
            {
                CogPolygon cp = new CogPolygon();
                cp.AddVertex(0, 0, -1);
                cp.AddVertex(100, 0, -1);
                cp.AddVertex(0, 100, -1);
                cp.Interactive = true;
                cp.Visible = true;
                cp.GraphicDOFEnable = CogPolygonDOFConstants.All;
                mask.Add(cp);
                if (RegionAdded != null)
                    RegionAdded(cp, EventArgs.Empty);
            }
            internal void RemoveRegion()
            {
                if (selectedObj != null)
                {
                    int id = mask.IndexOf(selectedObj);
                    if (id >= 0)
                    {
                        if (RegionRemoved != null)
                            RegionRemoved(mask[id], EventArgs.Empty);
                        mask.RemoveAt(id);
                    }
                }
            }
            internal void ClearRegion()
            {
                mask.Clear();
                if (RegionRemoved != null)
                    RegionRemoved(mask, EventArgs.Empty);
            }
        }
        #endregion

        private void cogDisplay1_MouseUp(object sender, MouseEventArgs e)
        {
            if (colorTraining != null)
            {
                //TrainedColor = colorTraining.color;
                propertyGrid1.Refresh();
            }
        }
        private void cogDisplay1_MouseDown(object sender, MouseEventArgs e)
        {
            if (cogDisplay1.Selection.Count > 0)
            {
                if (defaultDisplay is CogMaskListDisplay)
                {
                    ((CogMaskListDisplay)defaultDisplay).selectedObj = cogDisplay1.Selection[0];
                }
                if (cogDisplay1.Selection[0] is CogRectangle)
                    propertyGrid1.SelectedObject = new CogRectangleDisplay((CogRectangle)cogDisplay1.Selection[0]);
                else if (cogDisplay1.Selection[0] is CogPolygon)
                {
                    CogPolygon cp = (CogPolygon)cogDisplay1.Selection[0];
                    try
                    {
                        CogImage8Grey img = cogDisplay1.Image as CogImage8Grey;
                        if (img != null)
                        {
                            int x = (int)((e.X - cogDisplay1.Width / 2.0) / cogDisplay1.Zoom + img.Width / 2.0 - cogDisplay1.PanX);
                            int y = (int)((e.Y - cogDisplay1.Height / 2.0) / cogDisplay1.Zoom + img.Height / 2.0 - cogDisplay1.PanY);
                            if (x >= 0 && y >= 0 && x < img.Width && y < img.Height)
                            {
                                ICogTransform2D ict = cogDisplay1.GetTransform(".", "@");
                                double x1, y1;
                                ict.MapPoint(x, y, out x1, out y1);
                                ptClicked.X = (int)x1;
                                ptClicked.Y = (int)y1;
                                selectedVertex = cp.NearestVertex(x1, y1);
                            }
                        }
                    }
                    catch { }
                    propertyGrid1.SelectedObject = new CogPolygonDisplay(cp);
                }
                else
                    propertyGrid1.SelectedObject = cogDisplay1.Selection[0];
            }
            else
                propertyGrid1.SelectedObject = defaultDisplay;
        }
        private void cogDisplay1_DoubleClick(object sender, EventArgs e)
        {
            if (cogDisplay1.Selection.Count > 0)
            {
                if (cogDisplay1.Selection[0] is CogPolygon)
                {
                    CogPolygon cp = (CogPolygon)cogDisplay1.Selection[0];
                    if (selectedVertex < cp.NumVertices)
                    {
                        double vx, vy;
                        cp.GetVertex(selectedVertex, out vx, out vy);
                        if (Math.Abs(ptClicked.X - vx) + Math.Abs(ptClicked.Y - vy) < 7 / cogDisplay1.Zoom)
                        {
                            if (cp.NumVertices >= 3)
                                cp.RemoveVertex(selectedVertex);
                        }
                        else
                            cp.AddVertex(ptClicked.X, ptClicked.Y, selectedVertex);
                    }
                }
            }
        }

    }
}
