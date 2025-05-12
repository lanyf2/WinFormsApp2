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
    public partial class FormDispImg : Form
    {
        public static FormDispImg dispForm;
        public int fmid = 0;
        public static void ShowForm(int camId)
        {
            dispForm = new FormDispImg();
            dispForm.fmid = camId;
            FormPLC.ssFlag = true;

            var cam = Cognex.VisionPro.JobManager.MtdBlocks[camId].camUsb;
            if (cam != null)
                cam.SetTrigMode(false);

            dispForm.ShowDialog();
        }
        public FormDispImg()
        {
            InitializeComponent();
        }

        private void FormDispImg_Load(object sender, EventArgs e)
        {
            cogDisplay1.ContextMenuStrip = null;
            cogDisplay1.DoubleClick += new System.EventHandler(this.cogDisplay1_DoubleClick);
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
        internal void UpdateDisplay(ICogImage re, int id)
        {
            try
            {
                if (closeFlag)
                    return;
                if (InvokeRequired)
                    BeginInvoke(new Action<ICogImage, int>(UpdateDisplay), re, id);
                else
                    cogDisplay1.Image = re;
            }
            catch
            {
            }
        }
        bool closeFlag = false;
        private void FormDispImg_FormClosing(object sender, FormClosingEventArgs e)
        {
            closeFlag = true;
            dispForm = null;
            var cam = JobManager.MtdBlocks[fmid].camUsb;
            if (cam != null)
            {
                cam.SetTrigMode(true);
            }
            for (int i = 0; i < 10; i++)
            {
                if (InvokeRequired == false)
                    Application.DoEvents();
                System.Threading.Thread.Sleep(100);
            }
            FormPLC.ssFlag = false;
        }
    }
}
