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
    public partial class HMIEditForm : Form
    {
        public HMIEditForm()
        {
            InitializeComponent();
            propertyGrid1.SelectedObject = hmiDisplay1.Sts = Cognex.VisionPro.CogSerializer.LoadObjectFromFile("test.cfg") as HMISettings;
            //hmiDisplay1.Sts.img = new Cognex.VisionPro.CogImage24PlanarColor((Bitmap)Bitmap.FromFile(@"D:\work\caea\ATF01.bmp"));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Cognex.VisionPro.CogSerializer.SaveObjectToFile(hmiDisplay1.Sts, "test.cfg");
        }

        private void HMIEditForm_Load(object sender, EventArgs e)
        {
        }

        private void button2_Click(object sender, EventArgs e)
        {//增加箭头

        }

        private void button3_Click(object sender, EventArgs e)
        {//增加圆圈
            //if (hmiDisplay1.Sts.MInfoArray == null)
            //    hmiDisplay1.Sts.MInfoArray = new List<Minfo>();
            //Minfo m = new Minfo();
            //m.GraphicType = 1;
            //hmiDisplay1.Sts.MInfoArray.Add(m);
            //hmiDisplay1.AddGraphic(m);
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
        }

        private void pic1_Click(object sender, EventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            if(op.ShowDialog()== DialogResult.OK)
            {
                this.Refresh();
            }
        }
    }
}
