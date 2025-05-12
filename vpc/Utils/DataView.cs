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
    public partial class DataView : Form
    {
        private vpc.DatabaseDataSetTableAdapters.infosTableAdapter InfosTableAdapter;
        bool LogMode;
        public DataView(bool _logmode = true)
        {
            InitializeComponent();
            LogMode = _logmode;
            if (LogMode)
            {
                label3.Visible = false;
                textBox1.Visible = false;
                logDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
            else
            {
                this.Text = "统计信息";
                InfosTableAdapter = new DatabaseDataSetTableAdapters.infosTableAdapter();
                logsBindingSource.DataMember = "infos";
                logDataGridViewTextBoxColumn.DataPropertyName = "barcode";
                logDataGridViewTextBoxColumn.HeaderText = "结果记录";
                Column1.Visible = true;
                Column1.HeaderText = "记录";
                Column2.Visible = false;
                Column2.HeaderText = "文件路径";
                Column3.Visible = false;
                Column4.Visible = false;
                userDataGridViewTextBoxColumn.Visible = false;
                logDataGridViewTextBoxColumn.Visible = false;
                logDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Column1.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Column2.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Column3.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                Column4.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                logsBindingSource.Filter = string.Format("time > '{0}' and time < '{1}'", dateTimePicker1.Text, dateTimePicker2.Text);
            }
            else
            {
                logsBindingSource.Filter = string.Format("posresult like '%{2}%'", dateTimePicker1.Text, dateTimePicker2.Text, textBox1.Text);
            }
        }

        private void DataView_Load(object sender, EventArgs e)
        {
            try
            {
                dateTimePicker1.Value = DateTime.Now - DateTime.Now.TimeOfDay;
                dateTimePicker2.Value = dateTimePicker1.Value + new TimeSpan(23, 59, 59);
                if (LogMode)
                    this.logsTableAdapter.Fill(this.databaseDataSet.logs);
                else
                    this.InfosTableAdapter.Fill(this.databaseDataSet.infos);
                dataGridView1.Sort(timeDataGridViewTextBoxColumn, ListSortDirection.Descending);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dateTimePicker1.Value = dateTimePicker1.Value - new TimeSpan(1, 0, 0, 0);
            dateTimePicker2.Value = dateTimePicker2.Value - new TimeSpan(1, 0, 0, 0);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            dateTimePicker1.Value = dateTimePicker1.Value + new TimeSpan(1, 0, 0, 0);
            dateTimePicker2.Value = dateTimePicker2.Value + new TimeSpan(1, 0, 0, 0);

        }

        private void button3_Click(object sender, EventArgs e)
        {//Export Excel
            if (dataGridView1.Rows.Count == 0)
                MessageBox.Show("无数据可导出");
            else
            {
                SaveFileDialog sf = new SaveFileDialog();
                sf.Filter = "Excel|*.xlsx";
                if (LogMode)
                    sf.FileName = dateTimePicker1.Value.ToString("日志_yyyy年MM月dd日");
                else
                    sf.FileName = dateTimePicker1.Value.ToString("统计_yyyy年MM月dd日");
                if (sf.ShowDialog() == DialogResult.OK)
                {
                    ExcelHdl.SaveExcel(dataGridView1, sf.FileName);
                    MessageBox.Show("保存成功");
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            dateTimePicker1_ValueChanged(null, EventArgs.Empty);
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (LogMode == false)
            {
                int row = dataGridView1.SelectedCells[0].RowIndex;
                int col = dataGridView1.SelectedCells[0].ColumnIndex;
                if (col == 1)
                {
                    bool flag = false;
                    string str = dataGridView1[1, row].Value as string;
                    if (string.IsNullOrEmpty(str) == false)
                    {
                        string re;
                        if (ConvertInvalidCharToHex(str, out re))
                        {
                            //dataGridView1[1, row].Value = re;
                            //Program.MsgBox(re);
                            dataGridView1[1, row].ToolTipText = re;
                        }
                    }
                }
                else
                {
                    string str = dataGridView1[2, row].Value as string;
                    if (string.IsNullOrEmpty(str) == false)
                    {
                        if (System.IO.File.Exists(str))
                        {
                            ImageDisplay id = new ImageDisplay(str);
                            id.ShowDialog();
                        }
                    }
                }
            }
        }

        internal static bool IsValidChar(char c)
        {
            if (c > 0x0100 && c < 0x2000)
                return false;
            else
                return char.IsLetterOrDigit(c) || " `~!@#$%^&*()+=-[]{};':\",.<>/?|，。（）、【】｛｝’‘”“：；？《》·！￥…".IndexOf(c) >= 0;
        }
        internal static string ConvertInvalidCharToHex(string input)
        {
            string re;
            ConvertInvalidCharToHex(input, out re);
            return re;
        }
        internal static bool ConvertInvalidCharToHex(string input, out string result)
        {
            bool hexflag = false;
            bool flag = false;
            StringBuilder sb = new StringBuilder(input.Length + 10);
            for (int i = 0; i < input.Length; i++)
            {
                if (IsValidChar(input[i]) == false)
                {
                    flag = true;
                    if (hexflag == false)
                    {
                        sb.Append("[\\x");
                        hexflag = true;
                    }
                    if (input[i] < 256)
                        sb.Append(((int)input[i]).ToString("X2")).Append('-');
                    else
                        sb.Append(((int)input[i]).ToString("X4")).Append('-');
                }
                else
                {
                    if (hexflag)
                    {
                        sb.Remove(sb.Length - 1, 1);
                        sb.Append(']');
                        hexflag = false;
                    }
                    sb.Append(input[i]);
                }
            }
            if (hexflag)
            {
                sb.Remove(sb.Length - 1, 1);
                sb.Append(']');
            }
            if (flag)
                result = sb.ToString();
            else
                result = input;
            return flag;
        }
    }
}
