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
    public partial class UserManagement : Form
    {
        DatabaseDataSetTableAdapters.usersTableAdapter usersTableAdapter;
        DatabaseDataSet sqliteDatabaseDataSet;

        public UserManagement()
        {
            InitializeComponent();
            usersTableAdapter = new DatabaseDataSetTableAdapters.usersTableAdapter();
            sqliteDatabaseDataSet = new DatabaseDataSet();
            //textBox3.GotFocus += MainForm.GotFocusEvt;
            //textBox4.GotFocus += MainForm.GotFocusEvt;
            //textBox5.GotFocus += MainForm.GotFocusEvt;
            //comboBox1.GotFocus += MainForm.GotFocusEvt;
            //textBox3.Leave += MainForm.LeaveEvt;
            //textBox4.Leave += MainForm.LeaveEvt;
            //textBox5.Leave += MainForm.LeaveEvt;
            //comboBox1.Leave += MainForm.LeaveEvt;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void DataSta_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'sqliteDatabaseDataSet.users' table. You can move, or remove it, as needed.
            clearPsdTBox();
            comboBox2.Items.Clear();
            comboBox1.Items.Clear();

            this.usersTableAdapter.Fill(this.sqliteDatabaseDataSet.users);
            for (int i = 0; i < 3; i++)
                comboBox2.Items.Add(((UserRight)i).ToString());
            for (int i = 0; i < sqliteDatabaseDataSet.users.Rows.Count; i++)
            {
                if ((int)sqliteDatabaseDataSet.users.Rows[i][2] > (int)LogIn.CurrentUser.UserRights || (string)sqliteDatabaseDataSet.users.Rows[i][0] == (string)LogIn.CurrentUser.Name)
                    comboBox1.Items.Add(sqliteDatabaseDataSet.users.Rows[i][0]);
                else
                {
                    sqliteDatabaseDataSet.users.Rows.RemoveAt(i);
                    i--;
                }
            }
            if (LogIn.CurrentUser.UserRights == UserRight.超级管理员 || LogIn.CurrentUser.UserRights == UserRight.管理员)
                radioButton4.Enabled = true;
            else
                radioButton4.Enabled = false;
            radioButton1.Checked = true;
            radioButton_CheckedChanged(null, EventArgs.Empty);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex >= 0)
            {
                comboBox2.SelectedIndex = (int)sqliteDatabaseDataSet.users.Rows[comboBox1.SelectedIndex][2];
                textBox1.Text = ((long)sqliteDatabaseDataSet.users.Rows[comboBox1.SelectedIndex][3]).ToString("X");
                if (radioButton1.Checked)
                {
                    if ((string)comboBox1.SelectedItem == LogIn.CurrentUser.Name)
                    {
                        label3.Visible = true;
                        textBox3.Visible = true;
                    }
                    else
                    {
                        label3.Visible = false;
                        textBox3.Visible = false;
                    }
                }
            }
        }
        void clearPsdTBox()
        {
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {//修改密码
                bool fg = false;
                if (comboBox1.SelectedIndex < 0)
                    MessageBox.Show("请选择用户名。");
                else if (textBox4.Text.Length < 1)
                    MessageBox.Show("请输入新密码。");
                else if (textBox4.Text != textBox5.Text)
                    MessageBox.Show("两次输入的密码不同。");
                else if (textBox3.Visible)
                {
                    if (textBox3.Text != (string)this.sqliteDatabaseDataSet.users.Rows[comboBox1.SelectedIndex][1])
                        MessageBox.Show("原始密码不正确");
                    else
                        fg = true;
                }
                else
                    fg = true;
                if (fg)
                {
                    try
                    {
                        usersTableAdapter.UpdatePsdById(textBox4.Text, comboBox1.Text);
                        textBox3.Clear();
                        textBox4.Clear();
                        textBox5.Clear();
                        MessageBox.Show("用户 " + comboBox1.Text + " 的密码已成功修改");
                        clearPsdTBox();
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }

            }
            else if (radioButton2.Checked)
            {//添加用户
                if (textBox3.Text == "")
                    MessageBox.Show("密码不能为空。");
                else if (textBox3.Text != textBox5.Text)
                    MessageBox.Show("两次输入的密码不同。");
                else if (comboBox2.SelectedIndex < 0)
                    MessageBox.Show("未设定新用户的权限。");
                else if (comboBox2.SelectedIndex <= (int)LogIn.CurrentUser.UserRights)
                    MessageBox.Show("新用户的权限必须小于当前用户的权限。");
                else
                {
                    try
                    {
                        DataRow dr = this.sqliteDatabaseDataSet.users.NewRow();
                        dr[0] = comboBox1.Text;
                        dr[1] = textBox3.Text;
                        dr[2] = comboBox2.SelectedIndex;
                        dr[3] = 0;
                        usersTableAdapter.Insert((string)dr[0], (string)dr[1], (int)dr[2], 0);
                        comboBox1.Items.Add(comboBox1.Text);
                        this.sqliteDatabaseDataSet.users.Rows.Add(dr);
                        textBox3.Clear();
                        textBox4.Clear();
                        textBox5.Clear();
                        MessageBox.Show("用户 " + comboBox1.Text + " 已成功添加");
                    }
                    catch (System.Data.SQLite.SQLiteException ex)
                    {
                        if (ex.ErrorCode == 0x13)
                            MessageBox.Show("用户名已被使用");
                        else
                            MessageBox.Show(ex.Message);
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            else if (radioButton3.Checked)
            {
                if (comboBox1.Text == LogIn.CurrentUser.Name)
                    MessageBox.Show("不能删除自己！");
                else if (comboBox1.SelectedIndex < 0)
                    MessageBox.Show("请选择要删除的用户名！");
                else
                {
                    try
                    {
                        usersTableAdapter.DeleteId(comboBox1.SelectedItem.ToString());
                        this.sqliteDatabaseDataSet.users.Rows.RemoveAt(comboBox1.SelectedIndex);
                        MessageBox.Show("用户 " + comboBox1.SelectedItem.ToString() + " 已成功删除");
                        comboBox1.Items.RemoveAt(comboBox1.SelectedIndex);
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            else if (radioButton4.Checked)
            {
                try
                {
                    if (comboBox1.SelectedIndex < 0)
                        MessageBox.Show("错误：未选择用户");
                    else if (textBox1.Text.Length != 8)
                        MessageBox.Show("错误：卡ID格式不正确");
                    else
                    {
                        long id = Convert.ToInt64(textBox1.Text, 16);
                        usersTableAdapter.UpdateCardIDbyID(id, comboBox1.Text);
                        MessageBox.Show("修改成功");
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                textBox3.Visible = true;
                textBox4.Visible = true;
                textBox5.Visible = true;
                label3.Visible = true;
                label4.Visible = true;
                label5.Visible = true;
                comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
                comboBox2.Enabled = false;
                textBox1.Visible = false;
                label7.Visible = false;
            }
            else if (radioButton2.Checked)
            {
                textBox3.Visible = true;
                textBox4.Visible = false;
                textBox5.Visible = true;
                label3.Visible = true;
                label4.Visible = false;
                label5.Visible = true;
                comboBox1.DropDownStyle = ComboBoxStyle.DropDown;
                comboBox2.Enabled = true;
                textBox1.Visible = false;
                label7.Visible = false;
            }
            else if (radioButton3.Checked)
            {
                textBox3.Visible = false;
                textBox4.Visible = false;
                textBox5.Visible = false;
                label3.Visible = false;
                label4.Visible = false;
                label5.Visible = false;
                comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
                comboBox2.Enabled = false;
                textBox1.Visible = false;
                label7.Visible = false;
            }
            else if (radioButton4.Checked)
            {
                textBox3.Visible = false;
                textBox4.Visible = false;
                textBox5.Visible = false;
                label3.Visible = false;
                label4.Visible = false;
                label5.Visible = false;
                comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
                comboBox2.Enabled = false;
                textBox1.Visible = true;
                label7.Visible = true;
            }

        }
    }
}
