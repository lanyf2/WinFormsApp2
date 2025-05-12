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
    public partial class PlcModbusProgramming : Form
    {
        static bool runflag = false;
        internal static int RunDownload(ushort[] arg, string port = "COM4")
        {
            if (runflag == false)
            {
                runflag = true;
                try
                {
                    PlcModbusProgramming pp = new PlcModbusProgramming();
                    pp.ShowDialog();
                    runflag = false;
                    return pp.Result;
                }
                catch (Exception ex)
                {
                    Program.ErrHdl(ex);
                    runflag = false;
                }
            }
            return -1;
        }
        public int Result = -1;
        public PlcModbusProgramming()
        {
            InitializeComponent();
        }

        private void PlcModbusProgramming_Load(object sender, EventArgs e)
        {

        }
    }
}
