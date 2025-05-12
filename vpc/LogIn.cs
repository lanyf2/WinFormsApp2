using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using vpc.DatabaseDataSetTableAdapters;

namespace vpc
{
    public partial class LogIn : Form
    {
        Timer ActiveTimer;
        public static UserInfo CurrentUser = new UserInfo();
        public LogIn()
        {
            InitializeComponent();
            if (Settings.Default.test)
                this.BackgroundImage = null;
            label5.Text = Settings.Default.标题;
            if (Settings.Default.test == false)
                textBox1.Text = string.Empty;
            this.DialogResult = DialogResult.Cancel;
            ActiveTimer = new Timer();
            ActiveTimer.Interval = 60000;
            ActiveTimer.Tick += new EventHandler(ActiveTimer_Tick);
            ActiveTimer.Enabled = true;
        }

        public static void LogOut()
        {
            CurrentUser.UserRights = UserRight.未登录;
            CurrentUser.Name = "未登录";
        }
        private void ActiveTimer_Tick(object sender, EventArgs e)
        {
            //if (IsActiveWindow == false)
            {
                ActiveTimer.Enabled = false;
                Close();
            }
        }
        bool IsActiveWindow
        {
            get
            {
                return this.Handle == GetForegroundWindow();
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {//确定
            UserInfo uinfo;
            if (comboBox1.Text.Length == 0)
                MessageBox.Show("请选择用户名");
            else if (textBox1.Text.Length == 0)
                MessageBox.Show("请输入密码");
            else
            {
                uinfo = new UserInfo();
                uinfo.Name = comboBox1.Text;
                DataRow dr = udt.FindByid(comboBox1.Text);
                uinfo.UserRights = (UserRight)dr[2];
                uinfo.psd = (string)dr[1];
                if (uinfo == null)
                {
                    MessageBox.Show("不存在用户 " + comboBox1.Text);
                    return;
                }
                if (textBox1.Text != uinfo.psd)
                {
                    MessageBox.Show("密码错误！");
                    return;
                }
                CurrentUser = uinfo;
                Program.Loginfo(string.Format("用户 {0} 登录", CurrentUser.Name));
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {//登出
            LogOut();
            Program.Loginfo(string.Format("用户{0}登出", CurrentUser.Name));
            this.DialogResult = DialogResult.No;
            this.Close();
        }

        DatabaseDataSet.usersDataTable udt;
        private void logIn_Load(object sender, EventArgs e)
        {
            try
            {
                usersTableAdapter uta = new usersTableAdapter();
                udt = uta.GetData();
                foreach (DataRow dr in udt.Rows)
                {
                    comboBox1.Items.Add(dr[0]);
                }
                if (comboBox1.Items.Count > 0)
                    comboBox1.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                Program.ErrHdl(ex);
            }
            textBox1.Select();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex >= 0)
                textBox1.Focus();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                button1.PerformClick();
                //MainForm.LeaveEvt(null, EventArgs.Empty);
            }
        }

        [DllImport("user32.dll", EntryPoint = "GetForegroundWindow")]
        public static extern IntPtr GetForegroundWindow();

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("mailto:jylee7@163.com");
            }
            catch
            {
                Program.MsgBox("mailto:jylee7@163.com");
            }
        }

        private void textBox1_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
                button1.PerformClick();
        }
    }
    public enum UserRight
    {
        超级管理员, 管理员, 用户, 未登录
    }
    public class UserInfo
    {
        static TimeSpan AutoLogoutTime = new TimeSpan(0, 15, 0);
        public UserInfo(string _name, string psd, UserRight _uRights)
        {
            Name = _name;
            UserRights = _uRights;
        }
        public UserInfo() : this("未登录", null, UserRight.未登录)
        {
        }
        public string Name
        {
            get
            {
                if (DateTime.Now - LastActiveTime > AutoLogoutTime)
                    return "自动登出";
                else
                    return name;
            }
            set
            {
                name = value;
                LastActiveTime = DateTime.Now;
            }
        }
        string name;
        UserRight userRights;
        public UserRight UserRights
        {
            get
            {
                if (DateTime.Now - LastActiveTime > AutoLogoutTime)
                    return UserRight.未登录;
                else
                    return userRights;
            }
            set
            {
                userRights = value;
            }
        }
        public string psd;
        public long CardId = 0;
        public DateTime LastActiveTime = DateTime.MinValue;
        public bool IsAdmin
        {
            get
            {
                if (Settings.Default.test)
                    return true;
                if (DateTime.Now - LastActiveTime > AutoLogoutTime)
                {
                    MessageBox.Show("登录已超时");
                    return false;
                }
                else
                {
                    LastActiveTime = DateTime.Now;
                    if (UserRight.管理员 == UserRights || UserRight.超级管理员 == UserRights)
                        return true;
                    else
                    {
                        MessageBox.Show("权限不足");
                        return false;
                    }
                }
            }
        }
        public bool IsUser
        {
            get
            {
                if (Settings.Default.test)
                    return true;
                if (DateTime.Now - LastActiveTime > AutoLogoutTime)
                {
                    MessageBox.Show("登录已超时");
                    return false;
                }
                else
                {
                    LastActiveTime = DateTime.Now;
                    if (UserRight.管理员 == UserRights || UserRight.超级管理员 == UserRights || UserRight.用户 == UserRights)
                        return true;
                    else
                    {
                        MessageBox.Show("权限不足");
                        return false;
                    }
                }
            }
        }
    }
}
