using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace vpc
{
    public partial class UserControl1 : UserControl
    {
        public TextBox tb;
        public int index;
        public bool cancelonly = false;
        public bool active = false;
        public string cstBtntxt;
        public UserControl1()
        {
            InitializeComponent();
            tb = new TextBox();
            tb.Dock = DockStyle.Fill;
            tb.Anchor = AnchorStyles.Left;
            tb.KeyDown += Tb_KeyDown;
        }

        private void Tb_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode== Keys.Escape)
                SetActive(false);
            else if (e.KeyCode == Keys.Enter)
            {

            }
        }

        public override string Text
        {
            get { return label1.Text; }
            set { label1.Text = value; }
        }
        public void SetActive(bool act)
        {
            active = act;
            if (act)
            {
                tableLayoutPanel1.Controls.Remove(label1);
                tb.Text = label1.Text;
                tableLayoutPanel1.Controls.Add(tb);
                button1.Text = "取消";
            }
            else
            {
                tableLayoutPanel1.Controls.Remove(tb);
                tableLayoutPanel1.Controls.Add(label1);
                tb.Clear();
                if (cancelonly)
                    button1.Text = "取消";
                else
                    button1.Text = "定位";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {//定位
            if (button1.Text == "定位")
            {
                Form1.plcHdl.WritePlcInternalAsync(PlcInterface.AddrBase + 34, (ushort)(100 + index));
            }
            else
            {
                SetActive(false);
            }
        }
    }
}
