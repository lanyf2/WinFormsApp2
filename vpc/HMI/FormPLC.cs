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
    public partial class FormPLC : Form
    {
        internal static float[] 位置参数;
        UserControl1 activeControl;
        UserControl1[] uc1;
        PlcInterface plc;
        bool loaded = true;
        List<Control> mnuctl = new List<Control>();
        public FormPLC(string txt, PlcInterface plc)
        {
            InitializeComponent();
            this.plc = plc;
            Text = txt;
            uc1 = new UserControl1[10];
            for (int i = 0; i < uc1.Length; i++)
            {
                uc1[i] = new UserControl1();
                uc1[i].Dock = DockStyle.Fill;
                uc1[i].Click += FormPLC_Click;
                uc1[i].index = i;
                uc1[i].label1.Click += FormPLC_Click;
                tableLayoutPanel1.Controls.Add(uc1[i], 1, i);
            }
            uc1[9].cancelonly = true;
            uc1[9].SetActive(false);
            //mnuctl.AddRange(uc1);
            //mnuctl.Add(button4);
            mnuctl.Add(button15);
            mnuctl.Add(button16);
            mnuctl.Add(button17);
            mnuctl.Add(button18);
            mnuctl.Add(button19);
            mnuctl.Add(button10);
            mnuctl.Add(button11);
            mnuctl.Add(button12);
            mnuctl.Add(button13);
            mnuctl.Add(button14);
            mnuctl.Add(button5);
            mnuctl.Add(button20);
            mnuctl.Add(button21);
            mnuctl.Add(button22);
            mnuctl.Add(button23);
            mnuctl.Add(button24);
            if (Settings.Default.StepCount > 4)
                mnuctl.Add(button25);
            else
                button25.Enabled = false;
        }

        private void FormPLC_Click(object sender, EventArgs e)
        {
            if (activeControl != null)
            {
                activeControl.SetActive(false);
            }
            if (sender is Label lb)
            {
                if (lb.Parent != null)
                    sender = lb.Parent.Parent;
            }
            if (sender is UserControl1 uc)
            {
                activeControl = uc;
                activeControl.SetActive(true);
            }
        }

        private void FormPLC_Load(object sender, EventArgs e)
        {
            loaded = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (activeControl == null || activeControl.active == false)
            {
                Program.MsgBox("未编辑位置");
                return;
            }
            if (float.TryParse(activeControl.tb.Text, out float ff))
            {
                Form1.plcHdl.WriteSync(PlcInterface.AddrBase + activeControl.index * 2, ff);
                activeControl.SetActive(false);
                if(reMem==null)
                {
                    Program.MsgBox("PLC未连接");
                    return;
                }
                if (位置参数 == null || 位置参数.Length < uc1.Length)
                {
                    位置参数 = new float[uc1.Length];
                    for (int i = 0; i < uc1.Length; i++)
                    {

                        byte[] bt = new byte[4] { (byte)reMem[i * 2], (byte)(reMem[i * 2] >> 8), (byte)reMem[i * 2 + 1], (byte)(reMem[i * 2 + 1] >> 8) };
                        var val = BitConverter.ToSingle(bt, 0);
                        位置参数[i] = val;
                    }
                }
                位置参数[activeControl.index] = ff;
                Cognex.VisionPro.JobManager.SaveJob();
            }
            else
                Program.MsgBox("位置无法解析：" + activeControl.tb.Text);

        }

        private void button5_Click(object sender, EventArgs e)
        {
            Form1.plcHdl.WritePlcInternalAsync(PlcInterface.AddrBase + 34, 98);
        }

        private void button2_Click(object sender, EventArgs e)
        {//NG
            Form1.plcHdl.WritePlcInternalAsync(PlcInterface.AddrBase + 31, 1);
        }

        private void button3_Click(object sender, EventArgs e)
        {//OK
            Form1.plcHdl.WritePlcInternalAsync(PlcInterface.AddrBase + 31, 5);
        }

        ushort[] reMem;
        public void RefreshHdl(Dictionary<int, ushort> obj)
        {
            if (loaded == false)
                return;
            var re = plc.ReadPlcInternalSync(PlcInterface.AddrBase, 40);
            reMem = re;
            try
            {
                if (Disposing == false && IsDisposed == false)
                    BeginInvoke(new Action(() => updateInterface(re)));
            }
            catch
            {
            }
        }

        void updateInterface(ushort[] re)
        {
            var d = plc.DataStore;
            for (int i = 0; i < uc1.Length; i++)
            {
                byte[] bt = new byte[4] { (byte)re[i * 2], (byte)(re[i * 2] >> 8), (byte)re[i * 2 + 1], (byte)(re[i * 2 + 1] >> 8) };
                var val = BitConverter.ToSingle(bt, 0);
                uc1[i].Text = val.ToString("0.##");
            }
            var pos = PlcInterface.GetFloat(d, PlcInterface.AddrBase - 50);
            label9.Text = $"当前位置：{pos:F1}";
            if (d.TryGetValue(PlcInterface.AddrBase - 48, out ushort d952))
                label10.Text = $"设备状态：{(PLCStatus)(d952)}";
            if (d952 == 4 || d952 == 0 || d952 == 6)
            {
                foreach (var item in mnuctl)
                    item.Enabled = true;
            }
            else
            {
                foreach (var item in mnuctl)
                    item.Enabled = false;
            }
            if (d952 == 1 && ssFlag && !ssRunningFlag)
            {
                FormDispImg.dispForm?.Close();
                setAllCam(false);
            }
            if (d.TryGetValue(PlcInterface.AddrBase - 47, out ushort d953))
                label11.Text = $"伺服错误代码：{d953}";
            if (d.TryGetValue(PlcInterface.AddrBase - 46, out ushort d954))
                label12.Text = $"伺服状态：{(ServoStatus)d954}";
            pos = PlcInterface.GetFloat(d, PlcInterface.AddrBase - 45);
            label13.Text = $"定位位置：{pos:0.##}";

            label16.Text = "上位机缓存：" + Cognex.VisionPro.JobManager.GetBufString();
            if (d.TryGetValue(PlcInterface.AddrBase - 43, out ushort d957)
                && d.TryGetValue(PlcInterface.AddrBase - 42, out ushort d958)
                && d.TryGetValue(PlcInterface.AddrBase - 41, out ushort d959)
                && d.TryGetValue(PlcInterface.AddrBase - 40, out ushort d960))
                label17.Text = $"PLC缓存{d957}/合格{d959}/不合格{d960}/总{d958}";
            var vel = PlcInterface.GetFloat(d, PlcInterface.AddrBase - 39);
            label15.Text = $"当前速度：{vel / 6:0.#} rpm";
            if (d.TryGetValue(PlcInterface.AddrBase - 37, out ushort d963))
                label18.Text = $"最小动作缓冲时间：{d963}";
            if (d.TryGetValue(PlcInterface.AddrBase - 36, out ushort d964))
                label19.Text = $"平均动作缓冲时间：{d964 / 16}";
            if (d.TryGetValue(PlcInterface.AddrBase - 35, out ushort d965))
                label19.Text = $"最后动作缓冲时间：{d965}";

            if (re[32] == 0)
                button1.Text = "皮带转动";
            else
                button1.Text = "皮带停止";

            if (re[33] == 0)
                button21.Text = "光源打开";
            else
                button21.Text = "光源关闭";
        }

        public enum ServoStatus
        {
            Disabled = 0, ErrorStop = 1, Stopping = 2,
            Standstill = 3, DiscreteMotion = 4,
            ContinuousMotion = 5, Homing = 7,
            SynchronizedMotion = 8
        }
        public enum PLCStatus
        {
            待机 = 0, 运行中 = 1, 故障 = 3, 停止 = 4, 急停 = 5, 手动运行 = 6, 伺服未就绪 = 7, PLC故障 = 8
        }

        private void button6_Click(object sender, EventArgs e)
        {
            ParmsInputDialogue pid = new ParmsInputDialogue(Text, plc);
            plc.StatusUpdated += pid.RefreshHdl;
            pid.ShowDialog();
            plc.StatusUpdated -= pid.RefreshHdl;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Form1.plcHdl.WritePlcInternalAsync(PlcInterface.AddrBase + 30, 1);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Form1.plcHdl.WritePlcInternalAsync(PlcInterface.AddrBase + 30, 0);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Form1.plcHdl.WritePlcInternalAsync(PlcInterface.AddrBase + 30, 2);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "皮带转动")
                Form1.plcHdl.WritePlcInternalAsync(PlcInterface.AddrBase + 32, 1);
            else
                Form1.plcHdl.WritePlcInternalAsync(PlcInterface.AddrBase + 32, 0);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            Form1.plcHdl.WritePlcInternalAsync(PlcInterface.AddrBase + 34, 3);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            Form1.plcHdl.WritePlcInternalAsync(PlcInterface.AddrBase + 34, 4);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            Form1.plcHdl.WritePlcInternalAsync(PlcInterface.AddrBase + 34, 1);
        }

        private void button13_Click(object sender, EventArgs e)
        {
            Form1.plcHdl.WritePlcInternalAsync(PlcInterface.AddrBase + 34, 2);
        }

        private void button14_Click(object sender, EventArgs e)
        {
            if (plc.DataStore.TryGetValue(PlcInterface.AddrBase - 48, out ushort d952) && d952 == 1)
                Form1.plcHdl.WritePlcInternalAsync(PlcInterface.AddrBase + 34, 0);
            else
                Form1.plcHdl.WritePlcInternalAsync(PlcInterface.AddrBase + 34, 0);
        }

        private void button15_Click(object sender, EventArgs e)
        {
            Form1.plcHdl.WritePlcInternalAsync(PlcInterface.AddrBase + 36, 1);
        }

        private void button16_Click(object sender, EventArgs e)
        {
            Form1.plcHdl.WritePlcInternalAsync(PlcInterface.AddrBase + 36, 2);
        }

        private void button17_Click(object sender, EventArgs e)
        {
            Form1.plcHdl.WritePlcInternalAsync(PlcInterface.AddrBase + 36, 3);

        }

        private void button18_Click(object sender, EventArgs e)
        {
            Form1.plcHdl.WritePlcInternalAsync(PlcInterface.AddrBase + 36, 4);

        }

        private void button19_Click(object sender, EventArgs e)
        {
            Form1.plcHdl.WritePlcInternalAsync(PlcInterface.AddrBase + 36, 5);
        }

        private void button21_Click(object sender, EventArgs e)
        {
            if (button21.Text == "光源打开")
                Form1.plcHdl.WritePlcInternalAsync(PlcInterface.AddrBase + 33, 1);
            else
                Form1.plcHdl.WritePlcInternalAsync(PlcInterface.AddrBase + 33, 0);
        }
        internal static bool ssFlag = false;
        internal bool ssRunningFlag = false;
        private void button20_Click(object sender, EventArgs e)
        {
            if (Cognex.VisionPro.JobManager.MtdBlocks?.Length > 0)
            {
                FormDispImg.ShowForm(0);
            }
        }
        private void button20_Clickbak(object sender, EventArgs e)
        {
            if (ssRunningFlag == false)
                setAllCam(!ssFlag);
        }
        void setAllCam(bool flag)
        {
            ssRunningFlag = true;
            try
            {
                ssFlag = true;
                for (int i = 0; i < Cognex.VisionPro.JobManager.MtdBlocks?.Length; i++)
                {
                    var cam = Cognex.VisionPro.JobManager.MtdBlocks[i].camUsb;
                    if (cam != null)
                        cam.SetTrigMode(!flag);
                }
                if (flag == false)
                    for (int i = 0; i < 10; i++)
                    {
                        if (InvokeRequired == false)
                            Application.DoEvents();
                        System.Threading.Thread.Sleep(100);
                    }
                ssFlag = flag;
            }
            catch (Exception ex)
            {
                Program.ErrHdl(ex);
            }
            ssRunningFlag = false;
        }

        private void FormPLC_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ssFlag)
            {
                setAllCam(false);
            }
        }

        private void button22_Click(object sender, EventArgs e)
        {

            if (Cognex.VisionPro.JobManager.MtdBlocks?.Length > 1)
            {
                FormDispImg.ShowForm(1);
            }
        }

        private void button23_Click(object sender, EventArgs e)
        {

            if (Cognex.VisionPro.JobManager.MtdBlocks?.Length > 2)
            {
                FormDispImg.ShowForm(2);
            }
        }

        private void button24_Click(object sender, EventArgs e)
        {

            if (Cognex.VisionPro.JobManager.MtdBlocks?.Length > 3)
            {
                FormDispImg.ShowForm(3);
            }
        }

        private void button25_Click(object sender, EventArgs e)
        {
            if (Cognex.VisionPro.JobManager.MtdBlocks?.Length > 4)
            {
                FormDispImg.ShowForm(4);
            }
        }
    }
}
