using Cognex.VisionPro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace vpc
{
    public partial class PatternsTrainForm : Form
    {
        List<Cognex.VisionPro.PMAlign.CogPMAlignPattern> ptns;
        public PatternsTrainForm(List<Cognex.VisionPro.PMAlign.CogPMAlignPattern> ptns)
        {
            InitializeComponent();
            this.ptns = ptns;
        }

        void updateGraphic(Cognex.VisionPro.PMAlign.CogPMAlignPattern pt, Control ct)
        {
            if (ct is Cognex.VisionPro.Display.CogDisplay p)
            {
                if (pt.Trained)
                {
                    var msk = pt.GetTrainedPatternImageMask();
                    p.StaticGraphics.Clear();
                    if (msk == null)
                    {
                        p.Image = pt.GetTrainedPatternImage();
                    }
                    else
                    {
                        var im2 = pt.GetTrainedPatternImage();
                        //var tl = new Cognex.VisionPro.ImageProcessing.CogIPTwoImageMinMax();
                        //var imr = tl.Execute(msk, im2, null, null);
                        //p.Image = imr.ToBitmap();
                        p.Image = im2;
                        CogMaskGraphic graphic = ParmsInputDialogue.CreateMaskGraphic(msk);
                        p.StaticGraphics.Add(graphic, "Mask");
                    }
                    p.StaticGraphics.AddList(pt.CreateGraphicsFine(CogColorConstants.Green), null);
                    p.StaticGraphics.AddList(pt.CreateGraphicsCoarse(CogColorConstants.Yellow), null);
                }
                else
                {
                    //p.Image = Properties.Resources.logo_蓝___副本;
                    p.Image = null;
                }
            }
        }
        private void PatternsTrainForm_Load(object sender, EventArgs e)
        {
            flowLayoutPanel1.MouseDown += FlowLayoutPanel1_MouseDown;
            for (int i = 0; i < ptns.Count; i++)
            {
                var pp = CreatePictureBox(ptns[i]);
            }
            flowLayoutPanel1.AutoScroll = true;
        }

        private void FlowLayoutPanel1_MouseDown(object sender, MouseEventArgs e)
        {
            ctl = null;
        }

        private void P_Click(object sender, EventArgs e)
        {
            var p = (Control)sender;
            var ptn = (Cognex.VisionPro.PMAlign.CogPMAlignPattern)p.Tag;
            TrainModelEditor te = new TrainModelEditor(ptn);
            te.ShowDialog();
            updateGraphic(ptn, p);
        }

        private void cogDisplay1_DoubleClick(object sender, EventArgs e)
        {
            var cogDisplay = sender as Cognex.VisionPro.Display.CogDisplay;
            if (cogDisplay != null)
            {
                double rate = cogDisplay.Zoom;
                cogDisplay.AutoFit = false;
                cogDisplay.AutoFit = true;
                double nrate = cogDisplay.Zoom;
                if (Math.Abs(rate - nrate) < 0.0001)
                {

                }
            }
        }
        Control CreatePictureBox(Cognex.VisionPro.PMAlign.CogPMAlignPattern pt)
        {
            var p = new Cognex.VisionPro.Display.CogDisplay();
            p.Size = new Size(150, 150);
            flowLayoutPanel1.Controls.Add(p);
            updateGraphic(pt, p);
            //p.SizeMode = PictureBoxSizeMode.Zoom;
            //p.Click += P_Click;
            p.DoubleClick += cogDisplay1_DoubleClick;
            p.ContextMenuStrip = contextMenuStrip1;
            p.MouseDown += P_MouseDown;
            p.AutoFit = true;
            p.VerticalScrollBar = false;
            p.HorizontalScrollBar = false;
            p.Tag = pt;
            return p;
        }
        Control ctl;
        private void P_MouseDown(object sender, MouseEventArgs e)
        {
            ctl = sender as Control;
        }

        private void toolStripMenuItemAdd_Click(object sender, EventArgs e)
        {
            var pt = new Cognex.VisionPro.PMAlign.CogPMAlignPattern();
            var p = CreatePictureBox(pt);
            ptns.Add(pt);
        }

        private void toolStripMenuItemDelete_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem c && c.Tag is PictureBox p)
            {
                if (ptns.Count > 1)
                {
                    flowLayoutPanel1.Controls.Remove(p);
                    ptns.Remove((Cognex.VisionPro.PMAlign.CogPMAlignPattern)p.Tag);
                }
                else
                    Program.MsgBox("须至少保留一个模板");
            }
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            var mnu = (ContextMenuStrip)sender;
            if (ctl is Cognex.VisionPro.Display.CogDisplay p)
            {
                toolStripMenuItemDelete.Enabled = true;
                editToolStripMenuItem.Enabled = true;
                toolStripMenuItemDelete.Tag = p;
            }
            else
            {
                toolStripMenuItemDelete.Enabled = false;
                editToolStripMenuItem.Enabled = false;
                toolStripMenuItemDelete.Tag = null;
            }
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
           if(ctl != null)
            {
                var ptn = (Cognex.VisionPro.PMAlign.CogPMAlignPattern)ctl.Tag;
                TrainModelEditor te = new TrainModelEditor(ptn);
                te.ShowDialog();
                updateGraphic(ptn, ctl);
            }
        }
    }
}
