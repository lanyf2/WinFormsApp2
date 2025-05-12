namespace vpc
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.ExitBtn = new vpc.CstImgBtn();
            this.LogInBtn = new System.Windows.Forms.Button();
            this.label20 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.UsersBtn = new vpc.CstImgBtn();
            this.LogsBtn = new vpc.CstImgBtn();
            this.HisResultsBtn = new vpc.CstImgBtn();
            this.plcBtn = new vpc.CstImgBtn();
            this.SystemSettingsBtn = new vpc.CstImgBtn();
            this.AlgSettingsBtn = new vpc.CstImgBtn();
            this.StartStopBtn = new vpc.CstImgBtn();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.cogDisplay1 = new Cognex.VisionPro.Display.CogDisplay();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.FuncCbox = new System.Windows.Forms.ComboBox();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.ProdInfoTextbox = new System.Windows.Forms.TextBox();
            this.StatusLb = new System.Windows.Forms.Label();
            this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
            this.NextImgBtn = new System.Windows.Forms.Button();
            this.TestImgBtn = new System.Windows.Forms.Button();
            this.PreImgBtn = new System.Windows.Forms.Button();
            this.LastImgBtn = new System.Windows.Forms.Button();
            this.ClearListBtn = new System.Windows.Forms.Button();
            this.PicInfoLb = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.ProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.TotalCountLb = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cogDisplay1)).BeginInit();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel6.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(232)))), ((int)(((byte)(242)))));
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel5, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel6, 0, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 62F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1848, 959);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(84)))), ((int)(((byte)(171)))));
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.panel1.Controls.Add(this.ExitBtn);
            this.panel1.Controls.Add(this.LogInBtn);
            this.panel1.Controls.Add(this.label20);
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(4, 4);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1840, 54);
            this.panel1.TabIndex = 3;
            this.panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseDown);
            // 
            // ExitBtn
            // 
            this.ExitBtn.BackColor = System.Drawing.Color.Transparent;
            this.ExitBtn.BackgroundImage = global::vpc.Properties.Resources.closebtn;
            this.ExitBtn.Dock = System.Windows.Forms.DockStyle.Right;
            this.ExitBtn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ExitBtn.FlatAppearance.BorderSize = 0;
            this.ExitBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ExitBtn.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ExitBtn.ForeColor = System.Drawing.Color.White;
            this.ExitBtn.Location = new System.Drawing.Point(1682, 0);
            this.ExitBtn.Margin = new System.Windows.Forms.Padding(0, 6, 13, 6);
            this.ExitBtn.Name = "ExitBtn";
            this.ExitBtn.Size = new System.Drawing.Size(158, 54);
            this.ExitBtn.TabIndex = 12;
            this.ExitBtn.Text = "关闭";
            this.ExitBtn.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ExitBtn.UseVisualStyleBackColor = false;
            this.ExitBtn.Click += new System.EventHandler(this.ExitBtn_Click);
            // 
            // LogInBtn
            // 
            this.LogInBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.LogInBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(84)))), ((int)(((byte)(171)))));
            this.LogInBtn.BackgroundImage = global::vpc.Properties.Resources.btnlogin;
            this.LogInBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.LogInBtn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.LogInBtn.FlatAppearance.BorderSize = 0;
            this.LogInBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.LogInBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.LogInBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.LogInBtn.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LogInBtn.ForeColor = System.Drawing.Color.White;
            this.LogInBtn.Location = new System.Drawing.Point(1567, 7);
            this.LogInBtn.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.LogInBtn.Name = "LogInBtn";
            this.LogInBtn.Size = new System.Drawing.Size(91, 42);
            this.LogInBtn.TabIndex = 8;
            this.LogInBtn.Text = "登入";
            this.LogInBtn.UseVisualStyleBackColor = false;
            this.LogInBtn.Click += new System.EventHandler(this.LogInBtn_Click);
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Font = new System.Drawing.Font("微软雅黑", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label20.ForeColor = System.Drawing.Color.White;
            this.label20.Location = new System.Drawing.Point(200, 8);
            this.label20.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(123, 36);
            this.label20.TabIndex = 2;
            this.label20.Text = "检测系统";
            this.label20.DoubleClick += new System.EventHandler(this.label20_DoubleClick);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::vpc.Properties.Resources.logofy;
            this.pictureBox1.Location = new System.Drawing.Point(2, 6);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(188, 45);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 7;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel2.Controls.Add(this.UsersBtn, 6, 0);
            this.tableLayoutPanel2.Controls.Add(this.LogsBtn, 5, 0);
            this.tableLayoutPanel2.Controls.Add(this.HisResultsBtn, 4, 0);
            this.tableLayoutPanel2.Controls.Add(this.plcBtn, 3, 0);
            this.tableLayoutPanel2.Controls.Add(this.SystemSettingsBtn, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.AlgSettingsBtn, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.StartStopBtn, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Enabled = false;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 65);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(1842, 87);
            this.tableLayoutPanel2.TabIndex = 6;
            // 
            // UsersBtn
            // 
            this.UsersBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(232)))), ((int)(((byte)(242)))));
            this.UsersBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.UsersBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.UsersBtn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.UsersBtn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(232)))), ((int)(((byte)(242)))));
            this.UsersBtn.FlatAppearance.BorderSize = 0;
            this.UsersBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.UsersBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.UsersBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.UsersBtn.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.UsersBtn.Location = new System.Drawing.Point(1570, 0);
            this.UsersBtn.Margin = new System.Windows.Forms.Padding(0);
            this.UsersBtn.Name = "UsersBtn";
            this.UsersBtn.Size = new System.Drawing.Size(262, 87);
            this.UsersBtn.TabIndex = 11;
            this.UsersBtn.Text = "用户管理";
            this.UsersBtn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.UsersBtn.UseVisualStyleBackColor = false;
            this.UsersBtn.Click += new System.EventHandler(this.UsersBtn_Click);
            this.UsersBtn.MouseEnter += new System.EventHandler(this.StartStopBtn_MouseEnter);
            this.UsersBtn.MouseLeave += new System.EventHandler(this.StartStopBtn_MouseLeave);
            // 
            // LogsBtn
            // 
            this.LogsBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(232)))), ((int)(((byte)(242)))));
            this.LogsBtn.BackgroundImage = global::vpc.Properties.Resources.btn2;
            this.LogsBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.LogsBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.LogsBtn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LogsBtn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(232)))), ((int)(((byte)(242)))));
            this.LogsBtn.FlatAppearance.BorderSize = 0;
            this.LogsBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.LogsBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.LogsBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.LogsBtn.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LogsBtn.Location = new System.Drawing.Point(1310, 0);
            this.LogsBtn.Margin = new System.Windows.Forms.Padding(0);
            this.LogsBtn.Name = "LogsBtn";
            this.LogsBtn.Size = new System.Drawing.Size(260, 87);
            this.LogsBtn.TabIndex = 10;
            this.LogsBtn.Text = "日  志";
            this.LogsBtn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.LogsBtn.UseVisualStyleBackColor = false;
            this.LogsBtn.Click += new System.EventHandler(this.LogsBtn_Click);
            this.LogsBtn.MouseEnter += new System.EventHandler(this.StartStopBtn_MouseEnter);
            this.LogsBtn.MouseLeave += new System.EventHandler(this.StartStopBtn_MouseLeave);
            // 
            // HisResultsBtn
            // 
            this.HisResultsBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(232)))), ((int)(((byte)(242)))));
            this.HisResultsBtn.BackgroundImage = global::vpc.Properties.Resources.btn3;
            this.HisResultsBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.HisResultsBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.HisResultsBtn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.HisResultsBtn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(232)))), ((int)(((byte)(242)))));
            this.HisResultsBtn.FlatAppearance.BorderSize = 0;
            this.HisResultsBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.HisResultsBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.HisResultsBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.HisResultsBtn.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.HisResultsBtn.Location = new System.Drawing.Point(1050, 0);
            this.HisResultsBtn.Margin = new System.Windows.Forms.Padding(0);
            this.HisResultsBtn.Name = "HisResultsBtn";
            this.HisResultsBtn.Size = new System.Drawing.Size(260, 87);
            this.HisResultsBtn.TabIndex = 12;
            this.HisResultsBtn.Text = "历史结果";
            this.HisResultsBtn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.HisResultsBtn.UseVisualStyleBackColor = false;
            this.HisResultsBtn.Click += new System.EventHandler(this.HisResultsBtn_Click);
            this.HisResultsBtn.MouseEnter += new System.EventHandler(this.StartStopBtn_MouseEnter);
            this.HisResultsBtn.MouseLeave += new System.EventHandler(this.StartStopBtn_MouseLeave);
            // 
            // plcBtn
            // 
            this.plcBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(232)))), ((int)(((byte)(242)))));
            this.plcBtn.BackgroundImage = global::vpc.Properties.Resources.btn2;
            this.plcBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.plcBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.plcBtn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.plcBtn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(232)))), ((int)(((byte)(242)))));
            this.plcBtn.FlatAppearance.BorderSize = 0;
            this.plcBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.plcBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.plcBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.plcBtn.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.plcBtn.Location = new System.Drawing.Point(790, 0);
            this.plcBtn.Margin = new System.Windows.Forms.Padding(0);
            this.plcBtn.Name = "plcBtn";
            this.plcBtn.Size = new System.Drawing.Size(260, 87);
            this.plcBtn.TabIndex = 14;
            this.plcBtn.Text = "PLC状态";
            this.plcBtn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.plcBtn.UseVisualStyleBackColor = false;
            this.plcBtn.Click += new System.EventHandler(this.plcBtn_Click);
            this.plcBtn.MouseEnter += new System.EventHandler(this.StartStopBtn_MouseEnter);
            this.plcBtn.MouseLeave += new System.EventHandler(this.StartStopBtn_MouseLeave);
            // 
            // SystemSettingsBtn
            // 
            this.SystemSettingsBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(232)))), ((int)(((byte)(242)))));
            this.SystemSettingsBtn.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("SystemSettingsBtn.BackgroundImage")));
            this.SystemSettingsBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.SystemSettingsBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.SystemSettingsBtn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SystemSettingsBtn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(232)))), ((int)(((byte)(242)))));
            this.SystemSettingsBtn.FlatAppearance.BorderSize = 0;
            this.SystemSettingsBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.SystemSettingsBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.SystemSettingsBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SystemSettingsBtn.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.SystemSettingsBtn.Location = new System.Drawing.Point(530, 0);
            this.SystemSettingsBtn.Margin = new System.Windows.Forms.Padding(0);
            this.SystemSettingsBtn.Name = "SystemSettingsBtn";
            this.SystemSettingsBtn.Size = new System.Drawing.Size(260, 87);
            this.SystemSettingsBtn.TabIndex = 9;
            this.SystemSettingsBtn.Text = "系统设定";
            this.SystemSettingsBtn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.SystemSettingsBtn.UseVisualStyleBackColor = false;
            this.SystemSettingsBtn.Click += new System.EventHandler(this.SystemSettingsBtn_Click);
            this.SystemSettingsBtn.MouseEnter += new System.EventHandler(this.StartStopBtn_MouseEnter);
            this.SystemSettingsBtn.MouseLeave += new System.EventHandler(this.StartStopBtn_MouseLeave);
            // 
            // AlgSettingsBtn
            // 
            this.AlgSettingsBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(232)))), ((int)(((byte)(242)))));
            this.AlgSettingsBtn.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("AlgSettingsBtn.BackgroundImage")));
            this.AlgSettingsBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.AlgSettingsBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.AlgSettingsBtn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AlgSettingsBtn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(232)))), ((int)(((byte)(242)))));
            this.AlgSettingsBtn.FlatAppearance.BorderSize = 0;
            this.AlgSettingsBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.AlgSettingsBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.AlgSettingsBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.AlgSettingsBtn.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.AlgSettingsBtn.Location = new System.Drawing.Point(270, 0);
            this.AlgSettingsBtn.Margin = new System.Windows.Forms.Padding(0);
            this.AlgSettingsBtn.Name = "AlgSettingsBtn";
            this.AlgSettingsBtn.Size = new System.Drawing.Size(260, 87);
            this.AlgSettingsBtn.TabIndex = 8;
            this.AlgSettingsBtn.Text = "算法设定";
            this.AlgSettingsBtn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.AlgSettingsBtn.UseVisualStyleBackColor = false;
            this.AlgSettingsBtn.Click += new System.EventHandler(this.AlgSettingsBtn_Click);
            this.AlgSettingsBtn.MouseEnter += new System.EventHandler(this.StartStopBtn_MouseEnter);
            this.AlgSettingsBtn.MouseLeave += new System.EventHandler(this.StartStopBtn_MouseLeave);
            // 
            // StartStopBtn
            // 
            this.StartStopBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(232)))), ((int)(((byte)(242)))));
            this.StartStopBtn.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("StartStopBtn.BackgroundImage")));
            this.StartStopBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.StartStopBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.StartStopBtn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.StartStopBtn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(232)))), ((int)(((byte)(242)))));
            this.StartStopBtn.FlatAppearance.BorderSize = 0;
            this.StartStopBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.StartStopBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.StartStopBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.StartStopBtn.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.StartStopBtn.ForeColor = System.Drawing.Color.Black;
            this.StartStopBtn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.StartStopBtn.Location = new System.Drawing.Point(10, 0);
            this.StartStopBtn.Margin = new System.Windows.Forms.Padding(0);
            this.StartStopBtn.Name = "StartStopBtn";
            this.StartStopBtn.Size = new System.Drawing.Size(260, 87);
            this.StartStopBtn.TabIndex = 6;
            this.StartStopBtn.Text = "启   动";
            this.StartStopBtn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.StartStopBtn.UseVisualStyleBackColor = false;
            this.StartStopBtn.Click += new System.EventHandler(this.StartStopBtn_Click);
            this.StartStopBtn.MouseEnter += new System.EventHandler(this.StartStopBtn_MouseEnter);
            this.StartStopBtn.MouseLeave += new System.EventHandler(this.StartStopBtn_MouseLeave);
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("tableLayoutPanel5.BackgroundImage")));
            this.tableLayoutPanel5.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.tableLayoutPanel5.ColumnCount = 6;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 44.44445F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.tableLayoutPanel5.Controls.Add(this.splitContainer1, 0, 0);
            this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel5.Location = new System.Drawing.Point(0, 152);
            this.tableLayoutPanel5.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 1;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(1848, 727);
            this.tableLayoutPanel5.TabIndex = 7;
            // 
            // splitContainer1
            // 
            this.splitContainer1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(232)))), ((int)(((byte)(242)))));
            this.tableLayoutPanel5.SetColumnSpan(this.splitContainer1, 6);
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(24, 30);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(24, 30, 30, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tableLayoutPanel4);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tableLayoutPanel3);
            this.splitContainer1.Size = new System.Drawing.Size(1794, 697);
            this.splitContainer1.SplitterDistance = 1000;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 5;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(232)))), ((int)(((byte)(242)))));
            this.tableLayoutPanel4.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Inset;
            this.tableLayoutPanel4.ColumnCount = 1;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.Controls.Add(this.cogDisplay1, 0, 0);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel4.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 1;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(1000, 697);
            this.tableLayoutPanel4.TabIndex = 7;
            // 
            // cogDisplay1
            // 
            this.cogDisplay1.ColorMapLowerClipColor = System.Drawing.Color.Black;
            this.cogDisplay1.ColorMapLowerRoiLimit = 0D;
            this.cogDisplay1.ColorMapPredefined = Cognex.VisionPro.Display.CogDisplayColorMapPredefinedConstants.None;
            this.cogDisplay1.ColorMapUpperClipColor = System.Drawing.Color.Black;
            this.cogDisplay1.ColorMapUpperRoiLimit = 1D;
            this.cogDisplay1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cogDisplay1.Location = new System.Drawing.Point(2, 2);
            this.cogDisplay1.Margin = new System.Windows.Forms.Padding(0);
            this.cogDisplay1.MouseWheelMode = Cognex.VisionPro.Display.CogDisplayMouseWheelModeConstants.Zoom1;
            this.cogDisplay1.MouseWheelSensitivity = 1D;
            this.cogDisplay1.Name = "cogDisplay1";
            this.cogDisplay1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("cogDisplay1.OcxState")));
            this.cogDisplay1.Size = new System.Drawing.Size(996, 693);
            this.cogDisplay1.TabIndex = 6;
            this.cogDisplay1.DoubleClick += new System.EventHandler(this.cogDisplay1_DoubleClick);
            this.cogDisplay1.Click += new System.EventHandler(this.cogDisplay1_Click);
            this.cogDisplay1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.cogDisplay1_MouseMove);
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(232)))), ((int)(((byte)(242)))));
            this.tableLayoutPanel3.ColumnCount = 6;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel3.Controls.Add(this.FuncCbox, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.propertyGrid1, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.ProdInfoTextbox, 2, 0);
            this.tableLayoutPanel3.Controls.Add(this.StatusLb, 4, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Enabled = false;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(4);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 62F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(789, 697);
            this.tableLayoutPanel3.TabIndex = 74;
            // 
            // FuncCbox
            // 
            this.FuncCbox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel3.SetColumnSpan(this.FuncCbox, 2);
            this.FuncCbox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.FuncCbox.Enabled = false;
            this.FuncCbox.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FuncCbox.FormattingEnabled = true;
            this.FuncCbox.Items.AddRange(new object[] {
            "1.步骤1",
            "2.步骤2"});
            this.FuncCbox.Location = new System.Drawing.Point(13, 13);
            this.FuncCbox.Margin = new System.Windows.Forms.Padding(13, 0, 13, 0);
            this.FuncCbox.Name = "FuncCbox";
            this.FuncCbox.Size = new System.Drawing.Size(236, 35);
            this.FuncCbox.TabIndex = 74;
            this.FuncCbox.SelectedIndexChanged += new System.EventHandler(this.FuncCbox_SelectedIndexChanged);
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.CategoryForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.tableLayoutPanel3.SetColumnSpan(this.propertyGrid1, 6);
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.propertyGrid1.HelpVisible = false;
            this.propertyGrid1.Location = new System.Drawing.Point(0, 62);
            this.propertyGrid1.Margin = new System.Windows.Forms.Padding(0, 0, 4, 0);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.propertyGrid1.Size = new System.Drawing.Size(785, 635);
            this.propertyGrid1.TabIndex = 76;
            this.propertyGrid1.ToolbarVisible = false;
            this.propertyGrid1.SelectedGridItemChanged += new System.Windows.Forms.SelectedGridItemChangedEventHandler(this.propertyGrid1_SelectedGridItemChanged);
            // 
            // ProdInfoTextbox
            // 
            this.ProdInfoTextbox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.ProdInfoTextbox.BackColor = System.Drawing.Color.LightGreen;
            this.tableLayoutPanel3.SetColumnSpan(this.ProdInfoTextbox, 2);
            this.ProdInfoTextbox.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ProdInfoTextbox.Location = new System.Drawing.Point(275, 14);
            this.ProdInfoTextbox.Margin = new System.Windows.Forms.Padding(13, 4, 13, 4);
            this.ProdInfoTextbox.Name = "ProdInfoTextbox";
            this.ProdInfoTextbox.Size = new System.Drawing.Size(236, 34);
            this.ProdInfoTextbox.TabIndex = 75;
            this.ProdInfoTextbox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ProdInfoTextbox.MouseEnter += new System.EventHandler(this.ProdInfoTextbox_MouseEnter);
            // 
            // StatusLb
            // 
            this.StatusLb.AutoEllipsis = true;
            this.tableLayoutPanel3.SetColumnSpan(this.StatusLb, 2);
            this.StatusLb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.StatusLb.Font = new System.Drawing.Font("微软雅黑", 10.28571F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.StatusLb.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(100)))), ((int)(((byte)(189)))));
            this.StatusLb.Location = new System.Drawing.Point(528, 0);
            this.StatusLb.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.StatusLb.Name = "StatusLb";
            this.StatusLb.Size = new System.Drawing.Size(257, 62);
            this.StatusLb.TabIndex = 80;
            this.StatusLb.Text = "待机";
            this.StatusLb.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel6
            // 
            this.tableLayoutPanel6.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("tableLayoutPanel6.BackgroundImage")));
            this.tableLayoutPanel6.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.tableLayoutPanel6.ColumnCount = 7;
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 43.32262F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8.664525F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8.664525F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 13.35476F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8.664525F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8.664525F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8.664525F));
            this.tableLayoutPanel6.Controls.Add(this.NextImgBtn, 5, 0);
            this.tableLayoutPanel6.Controls.Add(this.TestImgBtn, 6, 0);
            this.tableLayoutPanel6.Controls.Add(this.PreImgBtn, 4, 0);
            this.tableLayoutPanel6.Controls.Add(this.LastImgBtn, 2, 0);
            this.tableLayoutPanel6.Controls.Add(this.ClearListBtn, 1, 0);
            this.tableLayoutPanel6.Controls.Add(this.PicInfoLb, 0, 0);
            this.tableLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel6.Enabled = false;
            this.tableLayoutPanel6.Location = new System.Drawing.Point(0, 879);
            this.tableLayoutPanel6.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            this.tableLayoutPanel6.RowCount = 1;
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel6.Size = new System.Drawing.Size(1848, 80);
            this.tableLayoutPanel6.TabIndex = 8;
            // 
            // NextImgBtn
            // 
            this.NextImgBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(100)))), ((int)(((byte)(189)))));
            this.NextImgBtn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.NextImgBtn.FlatAppearance.BorderColor = System.Drawing.Color.Navy;
            this.NextImgBtn.FlatAppearance.BorderSize = 0;
            this.NextImgBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(100)))), ((int)(((byte)(189)))));
            this.NextImgBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(100)))), ((int)(((byte)(189)))));
            this.NextImgBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.NextImgBtn.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.NextImgBtn.ForeColor = System.Drawing.Color.White;
            this.NextImgBtn.Location = new System.Drawing.Point(1526, 24);
            this.NextImgBtn.Margin = new System.Windows.Forms.Padding(0, 24, 0, 4);
            this.NextImgBtn.Name = "NextImgBtn";
            this.NextImgBtn.Size = new System.Drawing.Size(160, 52);
            this.NextImgBtn.TabIndex = 72;
            this.NextImgBtn.Text = "下一张";
            this.NextImgBtn.UseVisualStyleBackColor = false;
            this.NextImgBtn.MouseDown += new System.Windows.Forms.MouseEventHandler(this.NextImgBtn_MouseDown);
            // 
            // TestImgBtn
            // 
            this.TestImgBtn.BackColor = System.Drawing.Color.Transparent;
            this.TestImgBtn.BackgroundImage = global::vpc.Properties.Resources.btnro;
            this.TestImgBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.TestImgBtn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TestImgBtn.FlatAppearance.BorderColor = System.Drawing.Color.Navy;
            this.TestImgBtn.FlatAppearance.BorderSize = 0;
            this.TestImgBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.TestImgBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.TestImgBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.TestImgBtn.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.TestImgBtn.ForeColor = System.Drawing.Color.White;
            this.TestImgBtn.Location = new System.Drawing.Point(1686, 24);
            this.TestImgBtn.Margin = new System.Windows.Forms.Padding(0, 24, 20, 4);
            this.TestImgBtn.Name = "TestImgBtn";
            this.TestImgBtn.Size = new System.Drawing.Size(142, 52);
            this.TestImgBtn.TabIndex = 80;
            this.TestImgBtn.Text = "取图像";
            this.TestImgBtn.UseVisualStyleBackColor = false;
            this.TestImgBtn.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TestImgBtn_MouseDown);
            // 
            // PreImgBtn
            // 
            this.PreImgBtn.BackColor = System.Drawing.Color.Transparent;
            this.PreImgBtn.BackgroundImage = global::vpc.Properties.Resources.btnlg;
            this.PreImgBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.PreImgBtn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PreImgBtn.FlatAppearance.BorderColor = System.Drawing.Color.Navy;
            this.PreImgBtn.FlatAppearance.BorderSize = 0;
            this.PreImgBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.PreImgBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.PreImgBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.PreImgBtn.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.PreImgBtn.ForeColor = System.Drawing.Color.White;
            this.PreImgBtn.Location = new System.Drawing.Point(1386, 24);
            this.PreImgBtn.Margin = new System.Windows.Forms.Padding(20, 24, 0, 4);
            this.PreImgBtn.Name = "PreImgBtn";
            this.PreImgBtn.Size = new System.Drawing.Size(140, 52);
            this.PreImgBtn.TabIndex = 71;
            this.PreImgBtn.Text = "上一张";
            this.PreImgBtn.UseVisualStyleBackColor = false;
            this.PreImgBtn.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PreImgBtn_MouseDown);
            // 
            // LastImgBtn
            // 
            this.LastImgBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.LastImgBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(177)))), ((int)(((byte)(177)))));
            this.LastImgBtn.FlatAppearance.BorderColor = System.Drawing.Color.Navy;
            this.LastImgBtn.FlatAppearance.BorderSize = 0;
            this.LastImgBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.LastImgBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.LastImgBtn.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LastImgBtn.ForeColor = System.Drawing.Color.White;
            this.LastImgBtn.Location = new System.Drawing.Point(964, 19);
            this.LastImgBtn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 0);
            this.LastImgBtn.Name = "LastImgBtn";
            this.LastImgBtn.Size = new System.Drawing.Size(152, 45);
            this.LastImgBtn.TabIndex = 73;
            this.LastImgBtn.Text = "最新图像";
            this.LastImgBtn.UseVisualStyleBackColor = false;
            this.LastImgBtn.Click += new System.EventHandler(this.LastImgBtn_Click);
            // 
            // ClearListBtn
            // 
            this.ClearListBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.ClearListBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(92)))), ((int)(((byte)(176)))));
            this.ClearListBtn.FlatAppearance.BorderColor = System.Drawing.Color.Navy;
            this.ClearListBtn.FlatAppearance.BorderSize = 0;
            this.ClearListBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.ClearListBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.ClearListBtn.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ClearListBtn.ForeColor = System.Drawing.Color.White;
            this.ClearListBtn.Location = new System.Drawing.Point(804, 19);
            this.ClearListBtn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 0);
            this.ClearListBtn.Name = "ClearListBtn";
            this.ClearListBtn.Size = new System.Drawing.Size(152, 45);
            this.ClearListBtn.TabIndex = 79;
            this.ClearListBtn.Text = "清空缓存";
            this.ClearListBtn.UseVisualStyleBackColor = false;
            this.ClearListBtn.Click += new System.EventHandler(this.ClearListBtn_Click);
            // 
            // PicInfoLb
            // 
            this.PicInfoLb.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.PicInfoLb.AutoSize = true;
            this.PicInfoLb.BackColor = System.Drawing.Color.White;
            this.PicInfoLb.Font = new System.Drawing.Font("微软雅黑", 9.07563F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.PicInfoLb.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(100)))), ((int)(((byte)(189)))));
            this.PicInfoLb.Location = new System.Drawing.Point(60, 30);
            this.PicInfoLb.Margin = new System.Windows.Forms.Padding(60, 0, 3, 0);
            this.PicInfoLb.Name = "PicInfoLb";
            this.PicInfoLb.Size = new System.Drawing.Size(30, 19);
            this.PicInfoLb.TabIndex = 81;
            this.PicInfoLb.Text = "-/-";
            this.PicInfoLb.DoubleClick += new System.EventHandler(this.PicInfoLb_DoubleClick);
            // 
            // statusStrip1
            // 
            this.statusStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(232)))), ((int)(((byte)(242)))));
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ProgressBar1,
            this.TotalCountLb,
            this.StatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 959);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 19, 0);
            this.statusStrip1.Size = new System.Drawing.Size(1848, 26);
            this.statusStrip1.TabIndex = 4;
            this.statusStrip1.Text = "statusStrip1";
            this.statusStrip1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseDown);
            // 
            // ProgressBar1
            // 
            this.ProgressBar1.Name = "ProgressBar1";
            this.ProgressBar1.Size = new System.Drawing.Size(133, 18);
            this.ProgressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            // 
            // TotalCountLb
            // 
            this.TotalCountLb.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(100)))), ((int)(((byte)(189)))));
            this.TotalCountLb.Margin = new System.Windows.Forms.Padding(20, 4, 0, 2);
            this.TotalCountLb.Name = "TotalCountLb";
            this.TotalCountLb.Size = new System.Drawing.Size(21, 20);
            this.TotalCountLb.Text = "   ";
            // 
            // StatusLabel1
            // 
            this.StatusLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(100)))), ((int)(((byte)(189)))));
            this.StatusLabel1.Name = "StatusLabel1";
            this.StatusLabel1.Size = new System.Drawing.Size(99, 20);
            this.StatusLabel1.Text = "系统初始化中";
            // 
            // timer1
            // 
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(232)))), ((int)(((byte)(242)))));
            this.ClientSize = new System.Drawing.Size(1848, 985);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.statusStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "vpc";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseDown);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel5.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cogDisplay1)).EndInit();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.tableLayoutPanel6.ResumeLayout(false);
            this.tableLayoutPanel6.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel StatusLabel1;
        private System.Windows.Forms.ToolStripProgressBar ProgressBar1;
        private vpc.CstImgBtn StartStopBtn;
        private vpc.CstImgBtn AlgSettingsBtn;
        private vpc.CstImgBtn SystemSettingsBtn;
        private vpc.CstImgBtn LogsBtn;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button LastImgBtn;
        private System.Windows.Forms.Button NextImgBtn;
        private System.Windows.Forms.Button PreImgBtn;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private vpc.CstImgBtn UsersBtn;
        private System.Windows.Forms.ComboBox FuncCbox;
        private System.Windows.Forms.TextBox ProdInfoTextbox;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button ClearListBtn;
        private System.Windows.Forms.Button TestImgBtn;
        private vpc.CstImgBtn plcBtn;
        private System.Windows.Forms.PictureBox pictureBox1;
        internal System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        internal Cognex.VisionPro.Display.CogDisplay cogDisplay1;
        private System.Windows.Forms.Button LogInBtn;
        private System.Windows.Forms.ToolStripStatusLabel TotalCountLb;
        private vpc.CstImgBtn ExitBtn;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.Label StatusLb;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel6;
        private System.Windows.Forms.Label PicInfoLb;
        private CstImgBtn HisResultsBtn;
        private System.Windows.Forms.Timer timer1;
    }
}

