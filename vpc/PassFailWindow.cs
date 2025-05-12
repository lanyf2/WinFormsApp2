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
    public partial class PassFailWindow : Form
    {
        DateTime tm = DateTime.MinValue;
        public PassFailWindow()
        {
            InitializeComponent();
        }

        public void SetText(Color cr, string txt, int delay = 4)
        {
            ProdInfoTextbox.BackColor = cr;
            ProdInfoTextbox.Text = txt;
            if (delay > 0)
            {
                tm = DateTime.Now.AddSeconds(delay);
                timer1.Enabled = true;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (DateTime.Now > tm)
            {
                timer1.Enabled = false;
                ProdInfoTextbox.BackColor = Color.Khaki;
                ProdInfoTextbox.Text = "待机中";
            }
        }
    }
}
