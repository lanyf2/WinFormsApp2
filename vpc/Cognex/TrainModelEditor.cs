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
using Cognex.VisionPro.ToolBlock;

namespace vpc
{
    public partial class TrainModelEditor : Form
    {
        CogToolBlock blockinternal;
        int blkid = 0;
        Cognex.VisionPro.PMAlign.CogPMAlignPattern ptn;

        public TrainModelEditor(Cognex.VisionPro.PMAlign.CogPMAlignPattern pn)
        {
            InitializeComponent();
            ptn = pn;
            if (ptn.TrainRegion == null)
                ptn.TrainRegion = new CogRectangleAffine();
            ICogGraphicInteractive ic = ptn.TrainRegion as ICogGraphicInteractive;
            ic.Interactive = true;
            if (ic is CogPolygon cp)
            {
                if (cp.NumVertices < 3)
                {
                    cp.AddVertex(100, 100, 0);
                    cp.AddVertex(100, 200, 0);
                    cp.AddVertex(200, 100, 0);
                }
            }
            ic.GraphicDOFEnableBase = CogGraphicDOFConstants.All;
            cogDisplay1.InteractiveGraphics.Add(ic, null, false);
        }
        public TrainModelEditor(CogToolBlock block)
        {
            InitializeComponent();
            if (block.Outputs.Contains("parentBlock") && block.Outputs.Contains("id"))
            {
                blockinternal = (CogToolBlock)block.Outputs["parentBlock"].Value;
                blkid = (int)block.Outputs["id"].Value;
            }
            else
                blockinternal = block;
            if (blockinternal.Inputs.Contains("Parm"))
            {
                object[] parm = blockinternal.Inputs["Parm"].Value as object[];
                if (parm != null && parm.Length > 2)
                {
                    string p0 = parm[0] as string;
                    var p2 = parm[2] as Cognex.VisionPro.PMAlign.CogPMAlignPattern;
                    if (p2 != null && (p0 == Block1SettingsPatCheck.NameParm || p0 == Block1SettingsModelCompare.NameParm || p0 == Block1SettingsLoc.NameParm
                         || p0 == Block1SettingsModelCompareColor.NameParm))
                    {
                        if (p2.TrainRegion == null)
                            p2.TrainRegion = new CogRectangleAffine();
                        ICogGraphicInteractive ic = p2.TrainRegion as ICogGraphicInteractive;
                        ic.Interactive = true;
                        if (ic is CogPolygon)
                        {
                            CogPolygon cp = (CogPolygon)ic;
                            if (cp.NumVertices < 3)
                            {
                                cp.AddVertex(100, 100, 0);
                                cp.AddVertex(100, 200, 0);
                                cp.AddVertex(200, 100, 0);
                            }
                        }
                        ic.GraphicDOFEnableBase = CogGraphicDOFConstants.All;
                        cogDisplay1.InteractiveGraphics.Add(ic, null, false);
                    }
                    else
                        Program.MsgBox("内部错误11");
                }
            }
            propertyGrid1.SelectedObject = blockinternal.RunStatus;
            //cogDisplay1.Image = blockinternal.Outputs[0].Value as CogImage8Grey;
            //if (cogDisplay1.Image == null)
        }

        private void TrainBtn_Click(object sender, EventArgs e)
        {
            if (cogDisplay1.Image == null)
                MessageBox.Show("未采像，无法训练");
            else
            {
                if (blockinternal == null)
                {
                    //ptn.TrainImage = (CogImage8Grey)cogDisplay1.Image;
                    try
                    {
                        CogRectangle TrainRegionEnclosingRectangle = ptn.TrainRegion.EnclosingRectangle(CogCopyShapeConstants.GeometryOnly);
                        ptn.Origin.TranslationX = TrainRegionEnclosingRectangle.CenterX;
                        ptn.Origin.TranslationY = TrainRegionEnclosingRectangle.CenterY;
                        ptn.Origin.Rotation = 0;
                        ptn.Train();
                        var info = ptn.GetInfoStrings();
                        string ifo = null;
                        if (info.Count > 0)
                            ifo = info[0];
                        MessageBox.Show("训练成功," + ifo);
                    }
                    catch (Exception ex)
                    {
                        var info = ptn.GetInfoStrings();
                        string ifo = null;
                        if (info.Count > 0)
                            ifo = info[0];
                        Program.MsgBox(ifo + "\r\n" + ex.Message);
                    }
                }
                else
                {
                    //blockinternal.Inputs[0].Value = cogDisplay1.Image;
                    if (blockinternal.Inputs.Contains("TrainModel"))
                        blockinternal.Inputs["TrainModel"].Value = true;
                    else if (blockinternal.Inputs.Contains("TrainFlag"))
                        blockinternal.Inputs["TrainFlag"].Value = blkid;
                    else
                    {
                        MessageBox.Show("内部错误，无法训练");
                        return;
                    }
                    blockinternal.Run();
                    if (ParmsInputDialogue.selectedItemGlobal != null && ParmsInputDialogue.SelectedString != null)
                        Program.Loginfo("{0}-{1}".FormatWith(ParmsInputDialogue.SelectedString, ParmsInputDialogue.GetLabel(ParmsInputDialogue.selectedItemGlobal)));
                    propertyGrid1.SelectedObject = blockinternal.RunStatus;
                    if (blockinternal.RunStatus.Exception == null)
                        if (blockinternal.RunStatus.Message == null)
                        {
                            if (blockinternal.Outputs.Contains("Info"))
                            {
                                string info = blockinternal.Outputs["Info"].Value as string;
                                if (string.IsNullOrEmpty(info) == false)
                                {
                                    MessageBox.Show(info);
                                    return;
                                }
                            }
                            MessageBox.Show("训练成功");
                        }
                        else
                            MessageBox.Show(blockinternal.RunStatus.Message);
                    else
                        MessageBox.Show(blockinternal.RunStatus.Exception.Message);
                }
            }
        }

        private void TrainModelEditor_Load(object sender, EventArgs e)
        {
            if (Form1.cogDisplayStatic.Image == null)
            {
                MessageBox.Show("未采集图像");
                TrainBtn.Enabled = false;
            }
            else if (Form1.cogDisplayStatic.Image is CogImage8Grey img)
            {
                if (blockinternal != null)
                    blockinternal.Inputs[0].Value = cogDisplay1.Image = img;
                if (ptn != null)
                    cogDisplay1.Image = ptn.TrainImage = img;
            }
            else if (Form1.cogImage8GreyStatic != null)
            {
                if (blockinternal != null)
                    blockinternal.Inputs[0].Value = cogDisplay1.Image = Form1.cogImage8GreyStatic;
                if (ptn != null)
                    cogDisplay1.Image = ptn.TrainImage = Form1.cogImage8GreyStatic;
            }
            else
            {
                MessageBox.Show("彩色图像不可用于训练");
                TrainBtn.Enabled = false;
            }
            if (cogDisplay1.Image == null)
            {
                this.Close();
                //Program.MsgBox("未采集图像");
            }
        }

        private void cogDisplay1_MouseMove(object sender, MouseEventArgs e)
        {
            CogImage8Grey img = cogDisplay1.Image as CogImage8Grey;
            if (img != null)
            {
                int x = (int)((e.X - cogDisplay1.Width / 2.0) / cogDisplay1.Zoom + img.Width / 2.0 - cogDisplay1.PanX);
                int y = (int)((e.Y - cogDisplay1.Height / 2.0) / cogDisplay1.Zoom + img.Height / 2.0 - cogDisplay1.PanY);
                if (x >= 0 && y >= 0 && x < img.Width && y < img.Height)
                    if (drawBtn.Checked)
                    {
                        if (MouseRightDown)
                            img.SetPixel(x, y, 255);
                        else if (e.Button == MouseButtons.Left)
                            img.SetPixel(x, y, 0);
                        else
                        {
                            StatusLabel1.Text = string.Format("({0},{1}) 处像素值 = {2}", x, y, img.GetPixel(x, y));
                            return;
                        }

                        cogDisplay1.Image = img;
                    }
                    else
                    {
                        byte pix = img.GetPixel(x, y);
                        StatusLabel1.Text = string.Format("({0},{1}) 处像素值 = {2}", x, y, pix);
                    }
            }
        }
        bool MouseRightDown = false;
        private void cogDisplay1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                MouseRightDown = true;
            if (cogDisplay1.Selection.Count > 0)
                if (cogDisplay1.Selection[0] is CogPolygon)
                {
                    CogPolygon cp = (CogPolygon)cogDisplay1.Selection[0];

                    ICogImage img = cogDisplay1.Image as ICogImage;
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
        }
        Point ptClicked = new Point();
        int selectedVertex = 0;
        private void drawBtn_CheckedChanged(object sender, EventArgs e)
        {

        }
        private void cogDisplay1_MouseUp(object sender, MouseEventArgs e)
        {
            MouseRightDown = false;
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
                            if (cp.NumVertices > 3)
                                cp.RemoveVertex(selectedVertex);
                        }
                        else
                            cp.AddVertex(ptClicked.X, ptClicked.Y, selectedVertex);
                    }
                }
            }
        }

        private void drawBtn_Click(object sender, EventArgs e)
        {
            Cognex.VisionPro.PMAlign.CogPMAlignEditV2 ct = new Cognex.VisionPro.PMAlign.CogPMAlignEditV2();
            Form fm = new Form();
            ct.Dock = DockStyle.Fill;
            if (ct.Subject == null)
                ct.Subject = new Cognex.VisionPro.PMAlign.CogPMAlignTool();
            if (ptn != null)
            {
                ct.Subject.Pattern = ptn;
                ct.Subject.InputImage = ct.Subject.Pattern.TrainImage;
            }
            else
            {
                var pm = blockinternal.Inputs["Parm"].Value as object[];
                if (pm != null)
                {
                    ct.Subject.Pattern = pm[2] as Cognex.VisionPro.PMAlign.CogPMAlignPattern;
                    ct.Subject.InputImage = ct.Subject.Pattern.TrainImage;
                }
            }
            fm.Controls.Add(ct);
            fm.WindowState = FormWindowState.Maximized;
            fm.ShowDialog();
        }
    }
}
