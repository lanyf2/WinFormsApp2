using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Cognex.VisionPro;
using System.Threading;
using CSLC;
using System.Reflection;

namespace vpc
{
    internal partial class Form1 : Form
    {
        bool autoRun = false;
        DateTime startTime = DateTime.Now;
        internal static Image btnR;
        internal static Image btnROver;
        internal static PlcInterface plcHdl;
        internal LinkedListNode<ResultStruct> DisplayingResult;
        internal static SaveFile SaveImg = new SaveFile();
        internal static Cognex.VisionPro.Display.CogDisplay cogDisplayStatic
        {
            get
            {
                return cogDisplayGlobal[FuncCboxSelectedIndex];
            }
        }
        internal static CogImage24PlanarColor cogImage24PlanarStatic;
        internal static CogImage8Grey cogImage8GreyStatic;
        LogIn li;
        void tt()
        {
            try
            {
                var nRet = 15;
                var mind = 3.3222;
                var ss = string.Format("{1},间隙2：{0:0.###}", mind, nRet);
                var s = $"CIOController IO初始化错误：打开失败_{mind:0.##}";
                var s2 = $"CIOController IO初始化错误：打开失败_{nRet:X}";

                //float f = 123;
                //var b = BitConverter.GetBytes(f);
                //int u2 = b[2] + b[3] * 256;
                //FormPLC fm = new FormPLC("123", plcHdl);
                //fm.ShowDialog();
                //StringBuilder sb = new StringBuilder();
                //var CC = Encoding.GetEncoding("gb2312");
                //var bb = CC.GetBytes("哦a啊");
                //sb.Append(CC.GetString(new byte[] { 0xc5, 0xb6 }));
                //var str = string.Format("{0:X2}{1:00}", new ushort[] { 0xE, 0x1 }.Cast<object>().ToArray());
                //var ss = string.Format("{0:yyMM}", DateTime.Now);
                //var vv = PLCHdl.ConvertBCD(((0x20 << 8) + 0x19));
                var dt = new DateTime(1, 4, 4);
            }
            catch (Exception ex)
            {
                Program.MsgBox(ex.Message);
            }
            //var sport = new System.IO.Ports.SerialPort("COM3");
            //sport.BaudRate = 19200;
            //sport.Open();
            //var modbus = Modbus.Device.ModbusSerialMaster.CreateRtu(sport);
            //for (ushort i = 0; i < 5; i++)
            //{
            //    modbus.WriteSingleRegister(1, 5, i);
            //}
            //for (ushort i = 0; i < 5; i++)
            //{
            //    modbus.WriteSingleRegister(1, 5, i);
            //}
            //CogCircle[] cr = new CogCircle[4];
            //cr[0] = new CogCircle();
            //cr[0].SetCenterRadius(0, 0, 3);
            //cr[1] = new CogCircle();
            //cr[1].SetCenterRadius(1, 0.3, 3);
            //cr[2] = new CogCircle();
            //cr[2].SetCenterRadius(0.2, 3, 3);
            //cr[3] = new CogCircle();
            //cr[3].SetCenterRadius(1.2, 3.3, 3);
            //Array.Sort(cr, new cmpx());
            //Array.Sort(cr, new cmpy());
            //Array.Sort(cr, new cmpy());
            //CogGeneralContour cg = new CogGeneralContour();
            //cg.AddLineSegment(null, CogGeneralContourVertexConstants.FlagNone, 100, 200, CogGeneralContourVertexConstants.FlagNone, 300, 900);
            //cg.AddLineSegment(null, CogGeneralContourVertexConstants.Connected, 100, 200, CogGeneralContourVertexConstants.FlagNone, 900, 100);
            //cg.AddEllipticalArcSegment(null, CogGeneralContourVertexConstants.Connected, 100, 200, 300, 300, CogGeneralContourVertexConstants.FlagNone, 900, 900, 0.1);

            //vpc.DatabaseDataSetTableAdapters.infosTableAdapter InfosTableAdapter = new DatabaseDataSetTableAdapters.infosTableAdapter();
            //var re = InfosTableAdapter.GetDataByMTC("%");
            //string ss = "DID不匹配（写入 0000\x2016\x0902\x20zzz";
            //string s2 = DataView.ConvertInvalidCharToHex(ss);

        }
        class cmpx : IComparer<CogCircle>
        {
            public int Compare(CogCircle x, CogCircle y)
            {
                return (int)((x.CenterX - y.CenterX) * 1000);
            }
        }
        class cmpy : IComparer<CogCircle>
        {
            public int Compare(CogCircle x, CogCircle y)
            {
                return (int)((x.CenterY - y.CenterY) * 1000);
            }
        }
        internal Form1()
        {
            try
            {
                if (Settings.Default.test)
                    tt();
                if (System.IO.File.Exists("test.txt"))
                    System.IO.File.Delete("test.txt");
                InitializeComponent();
                ExitBtn.icon = Properties.Resources.close2;
                StartStopBtn.icon = Properties.Resources.icostart;
                AlgSettingsBtn.icon = Properties.Resources.icoalg;
                SystemSettingsBtn.icon = Properties.Resources.icocfg;
                plcBtn.icon = Properties.Resources.icoplc;
                HisResultsBtn.icon = Properties.Resources.icohis;
                UsersBtn.icon = Properties.Resources.icouser;
                LogsBtn.icon = Properties.Resources.icolog;
                //Type tp = typeof(PropertyGrid);
                //System.Reflection.MethodInfo mi = tp.GetMethod("GetGridEntryFromOffset", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                //FormBorderStyle = FormBorderStyle.Sizable;
                btnR = new Bitmap(StartStopBtn.BackgroundImage);
                btnR.RotateFlip(RotateFlipType.RotateNoneFlipX);
                UsersBtn.BackgroundImage = btnR;
                btnROver = new Bitmap(vpc.Properties.Resources.btnMouseOver);
                btnROver.RotateFlip(RotateFlipType.RotateNoneFlipX);

                //StatusLb.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
                //StatusLb.Dock = DockStyle.None;
                //StatusLb.AutoSize = true;
                //StatusLb.Location = new Point(propertyGrid1.Size.Width - 100, propertyGrid1.Size.Height - StatusLb.Size.Height);
                //propertyGrid1.ActiveControl.Controls.Add(StatusLb);

                FuncCbox.Items.Clear();
                cogDisplay1.ContextMenuStrip = null;
                for (int i = 0; i < Settings.Default.StepCount; i++)
                    FuncCbox.Items.Add("相机 " + (i + 1));
                //        var v = new Cognex.VisionPro3D.Cog3DMatrix3x3(1, 0, 0, 0, 1, 0, 0, 0, 0.1);
                //        var t3d = new Cognex.VisionPro3D.Cog3DTransformLinear(v, new Cognex.VisionPro3D.Cog3DVect3(1, 2, 3));
                //        var img = new Cognex.VisionPro.CogImage16Range(new CogImage16Grey(100,100), new CogImage8Grey(100, 100)
                //, t3d);
                //        img.GetTransform3D(".", "@");
                if (Settings.Default.jobs == null)
                    Settings.Default.jobs = "";
                label20.Text = string.Format("{0}({1})", Settings.Default.标题, System.IO.Path.GetFileNameWithoutExtension(Settings.Default.jobs));
                if (Settings.Default.ResultBufferCapacity <= 0)
                    Settings.Default.ResultBufferCapacity = 3;
                //numList = new Bitmap[5] { vpc.Properties.Resources.a1, vpc.Properties.Resources.a2, vpc.Properties.Resources.a3, vpc.Properties.Resources.a4, vpc.Properties.Resources.a5 };
                InitCogDisplay(Settings.Default.StepCount);
                //splitContainer1.Panel2Collapsed = true;
                JobManager.InitInfo += InitInfoHdl;
                BlockBase.ImageGrabbed += ImgGrabedHdl;
                BlockBase.InfoChanged += InfoChangedHdl;
                //cogDisplayStatic = cogDisplay1;

                //splitContainer1.Panel1.Controls.Add(recordsdisp);
                if (Settings.Default.ShowPassFailWindow)
                    this.WindowState = FormWindowState.Minimized;

                Rectangle workingArea = Screen.GetWorkingArea(this);
                this.MaximumSize = workingArea.Size;
                this.Size = workingArea.Size;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            ThreadPool.QueueUserWorkItem(Init);
        }

        #region 按钮
        private void StartStopBtn_Click(object sender, EventArgs e)
        {
            if (CameraHdlBase.extTrigFlag == false)
            {
                if (autoRun)
                {
                    StartStopBtn.Text = "启  动";
                    autoRun = !autoRun;
                }
                else
                {
                    StartStopBtn.Text = "停   止";
                    autoRun = !autoRun;
                    ThreadPool.QueueUserWorkItem(CamAUTOTest);
                }
                return;
            }
            //IO.AddToOutputQueue(false);
            if (Settings.Default.test)
                clcExe.TryInit();
            if (plcHdl == null || plcHdl.err != null)
                Program.MsgBox("PLC连接异常");
            else
            {
                if (plcHdl.DataStore.TryGetValue(PlcInterface.AddrBase - 48, out ushort d952))
                {
                    if (d952 == 0 || d952 == 4)
                        plcHdl.WritePlcInternalAsync(PlcInterface.AddrBase + 41, 1);
                    else if (d952 == 1)
                        plcHdl.WritePlcInternalAsync(PlcInterface.AddrBase + 41, 3);
                    else
                        Program.MsgBox("PLC运行状态："+(FormPLC.PLCStatus)d952);
                }
                else
                    Program.MsgBox("PLC运行状态未知");
            }
        }
        private void AlgSettingsBtn_Click(object sender, EventArgs e)
        {
            if (LogIn.CurrentUser.IsAdmin)
            {
                if (FuncCbox.SelectedIndex >= 0)
                {
                    ParmsInputDialogue pid = new ParmsInputDialogue(AlgSettingsBtn.Text, JobManager.MtdBlocks[FuncCbox.SelectedIndex]);
                    ParmsInputDialogue.SelectedString = "[{1}]{0}".FormatWith(FuncCbox.Text, System.IO.Path.GetFileNameWithoutExtension(Settings.Default.jobs));
                    if (DialogResult.OK == pid.ShowDialog())
                    {
                        JobManager.MtdBlocks[FuncCbox.SelectedIndex].SaveMtd();
                        if (JobManager.MtdBlocks[FuncCbox.SelectedIndex] != null && !string.IsNullOrEmpty(JobManager.MtdBlocks[FuncCbox.SelectedIndex].SerialNumber))
                        {
                            Settings.Default.Cams[FuncCbox.SelectedIndex] = JobManager.MtdBlocks[FuncCbox.SelectedIndex].SerialNumber;
                            Settings.Default.Save(true);
                        }
                        JobManager.SaveJob();
                        JobManager.MtdBlocks[FuncCbox.SelectedIndex].SyncExposureTime();
                        MessageBox.Show("保存成功");
                    }
                    else if (pid.changed)
                    {
                        MessageBox.Show("参数已发生变化但未保存，通过重新加载配方可还原参数设定");
                    }

                }
            }
        }
        private void SystemSettingsBtn_Click(object sender, EventArgs e)
        {
            if (LogIn.CurrentUser.IsUser)
            {
                string jobmem = Settings.Default.jobs;
                ParmsInputDialogue pid = new ParmsInputDialogue(SystemSettingsBtn.Text, Settings.Default);
                ParmsInputDialogue.SelectedString = "系统设定";
                pid.ShowDialog();
                //if (pid.ShowDialog() == DialogResult.OK)
                {
                    //Settings prevset = Settings.Default;
                    //Settings.Default = pid.SelectedObject as Settings;
                    if (pid.changed)
                    {
                        ApplySysSettings();
                        Settings.Default.Save(true);
                        FuncCbox_SelectedIndexChanged(null, EventArgs.Empty);

                        Program.MsgBox("当前配方：" + Settings.Default.jobs);
                    }
                }
            }
        }
        private void plcBtn_Click(object sender, EventArgs e)
        {
            if (LogIn.CurrentUser.IsAdmin)
            {
                FormPLC fm = new FormPLC(plcBtn.Text, plcHdl);
                plcHdl.UpdateRun += fm.RefreshHdl;
                fm.ShowDialog();
                plcHdl.UpdateRun -= fm.RefreshHdl;
                //ParmsInputDialogue pid = new ParmsInputDialogue(plcBtn.Text, plcHdl);
                //plcHdl.StatusUpdated += pid.RefreshHdl;
                //pid.ShowDialog();
                //plcHdl.StatusUpdated -= pid.RefreshHdl;
            }
        }
        private void HisResultsBtn_Click(object sender, EventArgs e)
        {
            DataView dv = new DataView(false);
            dv.ShowDialog();
        }
        private void LogsBtn_Click(object sender, EventArgs e)
        {
            DataView dv = new DataView();
            dv.ShowDialog();
        }
        private void LogInBtn_Click(object sender, EventArgs e)
        {
            if (li != null)
                li.Close();
            li = new LogIn();
            DialogResult re = li.ShowDialog();
            li = null;
            StatusLabel1.Text = string.Format("用户：{0}，权限：{1}", LogIn.CurrentUser.Name, LogIn.CurrentUser.UserRights);
            switch (re)
            {
                case DialogResult.OK:
                    break;
                case DialogResult.No:
                    break;
                case DialogResult.Cancel:
                    break;
            }
        }
        private void UsersBtn_Click(object sender, EventArgs e)
        {
            if (LogIn.CurrentUser.IsUser)
            {
                UserManagement um = new UserManagement();
                um.ShowDialog();
            }
        }
        private void ExitBtn_Click(object sender, EventArgs e)
        {
            Program.ExitFlag = true;
            Program.Loginfo("程序退出");
            string str = Settings.Default.PrintSerialNum;
            if (string.IsNullOrEmpty(str) == false && Settings.Default.PrintSerialNumMem != str && Settings.Default.打印启用)
            {
                Settings.Default.PrintSerialNumMem = str;
            }
            JobManager.TrySaveProdCounts();
            this.Close();
        }
        private void summaryBtn_Click(object sender, EventArgs e)
        {
        }

        
        private void NextImgBtn_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                NextImgBtn_Click(sender, EventArgs.Empty);
            else
            {
                if (fiMem == null || fiMem.Length < 2)
                    TestImgBtn_MouseDown(null, new MouseEventArgs(MouseButtons.Right, 0, 0, 0, 0));
                else
                {
                    fileMemId++;
                    if (fiMem.Length <= fileMemId)
                        fileMemId = 0;
                    var bmp = (Bitmap)Image.FromFile(fiMem[fileMemId].FullName);
                    if (bmp.PixelFormat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed)
                        JobManager.TrigTest(new CogImage8Grey(bmp), FuncCbox.SelectedIndex);
                    else
                        JobManager.TrigTest(new CogImage24PlanarColor(bmp), FuncCbox.SelectedIndex);
                }
            }
        }
        private void PreImgBtn_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                PreImgBtn_Click(sender, EventArgs.Empty);
            else
            {
                if (fiMem == null || fiMem.Length < 2)
                    TestImgBtn_MouseDown(null, new MouseEventArgs(MouseButtons.Right, 0, 0, 0, 0));
                else
                {
                    fileMemId--;
                    if (fileMemId < 0)
                        fileMemId = fiMem.Length - 1;
                    var bmp = (Bitmap)Image.FromFile(fiMem[fileMemId].FullName);
                    if (bmp.PixelFormat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed)
                        JobManager.TrigTest(new CogImage8Grey(bmp), FuncCbox.SelectedIndex);
                    else
                        JobManager.TrigTest(new CogImage24PlanarColor(bmp), FuncCbox.SelectedIndex);
                }
            }
        }

        private void PreImgBtn_Click(object sender, EventArgs e)
        {
            if (JobManager.ResultList.Count == 0)
            {
                DisplayingResult = null;
                MessageBox.Show("无缓存的结果");
                return;
            }
            if (DisplayingResult == null)
                DisplayingResult = JobManager.ResultList.Last;
            else if (DisplayingResult.Previous == null)
            {
                MessageBox.Show("无更早的结果");
                DisplayingResult = null;
            }
            else
                DisplayingResult = DisplayingResult.Previous;
            if (DisplayingResult == null)
                UpdateResult(null);
            else
                UpdateResult(DisplayingResult.Value);
        }
        private void NextImgBtn_Click(object sender, EventArgs e)
        {
            if (JobManager.ResultList.Count == 0)
            {
                MessageBox.Show("无缓存的结果");
                return;
            }
            if (DisplayingResult == null)
                DisplayingResult = JobManager.ResultList.First;
            else if (DisplayingResult.Next == null)
            {
                MessageBox.Show("无更多的结果");
                DisplayingResult = null;
                return;
            }
            else
                DisplayingResult = DisplayingResult.Next;
            if (DisplayingResult != null)
                UpdateResult(DisplayingResult.Value);
        }
        private void LastImgBtn_Click(object sender, EventArgs e)
        {
            DisplayingResult = null;
            UpdateResult(null);
            ParmsInputDialogue.MoveSplitterTo(propertyGrid1);

            //if (false)
            //if (CamerasGige.cameras.Count > 0)
            //    JobManager.TrigOne(FuncCbox.SelectedIndex + 1);
            //else
            //    JobManager.TrigTest(new CogImage8Grey((Bitmap)Bitmap.FromFile(filename)), FuncCbox.SelectedIndex + 1);
        }
        private void ClearListBtn_Click(object sender, EventArgs e)
        {
            if (JobManager.TotalCount > 0)
            {
                var adp = new DatabaseDataSetTableAdapters.infosTableAdapter();
                adp.Insert(DateTime.Now, JobManager.GetHisString(), null, null, null, null, null);
            }
            JobManager.Clear();
            propertyGrid1.SelectedObject = null;
            UpdateGraphics((ResultStruct)null, 0);
            UpdateGlobal();
        }
        #endregion
        void UpdateResult(ResultStruct sender)
        {
            UpdateResult(sender, FuncCbox.SelectedIndex);
        }
        void UpdateResult(ResultStruct sender, int id)
        {
            ResultStruct re;
            bool rtflag = false;
            if (sender == null)
            {
                rtflag = true;
                if (DisplayingResult != null)
                    re = DisplayingResult.Value;
                else
                {
                    re = JobManager.LastResult;
                }
            }
            else
                re = sender;
            if (rtflag)
                UpdateGraphics(re, -1);
            else
                UpdateGraphics(re, id);

            UpdateTexts(re);
            UpdateGlobal();
        }
        void UpdateGraphics(ResultStruct re, int id)
        {
            //Disable display updates (set DrawingEnabled to false).
            //Remove all graphics from the display.
            //Update the display with the new image.
            //Fit the image to the display (if required).
            //Add the results graphics produced by the last run of the tool.
            //Enable display updates.
            //if (Settings.Default.DebugMode)
            //{
            //    if (re == null)
            //        recordsdisp.Subject = null;
            //    RxInterface rx = re[FuncCbox.SelectedIndex];
            //    if (rx != null)
            //        recordsdisp.Subject = rx.Record;
            //    else
            //        recordsdisp.Subject = null;
            //}
            //else
            {
                cogImage8GreyStatic = null;
                cogImage24PlanarStatic = null;
                if (re == null)
                {
                    for (int i = 0; i < cogDisplayGlobal.Length; i++)
                        cogDisplayGlobal[i].Image = null;
                    return;
                }
                for (int i = 0; i < cogDisplayGlobal.Length; i++)
                {
                    if (re[i] == null)
                    {
                        if (id >= 0)
                            cogDisplayGlobal[i].Image = null;
                        continue;
                    }
                    var dp = cogDisplayGlobal[i];
                    dp.DrawingEnabled = false;
                    RxInterface rx = re[i];
                    if (rx != null)
                    {
                        dp.Image = rx.img;
                        if (Settings.Default.HideGraphics == false)
                            JobManager.graphics.UpdateGraphics(i, re, dp);
                        else
                            JobManager.graphics.SwitchGraphicsVisibility(dp);
                        R1Class r1 = rx as R1Class;
                        if (r1 != null && i == FuncCbox.SelectedIndex)
                        {
                            if (rx.img is CogImage8Grey)
                            {
                                cogImage8GreyStatic = rx.img as CogImage8Grey;
                                cogImage24PlanarStatic = null;
                            }
                            else
                            {
                                cogImage8GreyStatic = r1.imgGrey as CogImage8Grey;
                                cogImage24PlanarStatic = rx.img as CogImage24PlanarColor;
                            }
                        }
                    }
                    else
                        dp.Image = null;
                    dp.DrawingEnabled = true;
                }
            }

            //CogGeneralContour cg = new CogGeneralContour();
            //cg.AddLineSegment(null, CogGeneralContourVertexConstants.FlagNone, 100, 200, CogGeneralContourVertexConstants.FlagNone, 300, 900);
            //cg.AddLineSegment(null, CogGeneralContourVertexConstants.Connected, 100, 200, CogGeneralContourVertexConstants.FlagNone, 900, 100);
            //cg.AddEllipticalArcSegment(null, CogGeneralContourVertexConstants.Connected, 100, 200, 300, 300, CogGeneralContourVertexConstants.FlagNone, 900, 900,0.1);
            //cg.GraphicDOFEnable = CogGeneralContourDOFConstants.All;
            //cg.Interactive = true;
            //cogDisplay1.InteractiveGraphics.Add(cg, null, false);
        }
        void UpdateTexts(ResultStruct re)
        {
            if (re == null)
            {
                propertyGrid1.SelectedObject = null;
                ProdInfoTextbox.Text = null;
                ProdInfoTextbox.BackColor = Color.LightGreen;
                return;
            }
            propertyGrid1.SelectedObject = re[FuncCbox.SelectedIndex];
            ProdInfoTextbox.Text = re.Info;
            if (re.Failed)
            {
                ProdInfoTextbox.BackColor = Color.Red;
            }
            else
            {
                ProdInfoTextbox.BackColor = Color.LightGreen;
            }

            ParmsInputDialogue.MoveSplitterTo(propertyGrid1);
        }
        internal void UpdateStatusText(string s)
        {
            StatusLabel1.Text = s;
        }
        void UpdateGlobal()
        {
            //Cognex.VisionPro.ImageProcessing.
            TotalCountLb.Text = string.Format("合格/次品/总数：{0}/{1}/{2}，次品率：{3:F2}%，检测速度：{4:0.#} 个/分钟", JobManager.PassCount, JobManager.FailCount, JobManager.TotalCount, JobManager.FailRaito, fpsMem);
            if (JobManager.ResultList.Count == 0)
                PicInfoLb.Text = "--/--";
            else if (DisplayingResult == null || DisplayingResult.Value == null)
                PicInfoLb.Text = string.Format("{0}", JobManager.ResultList.Count);
            else
                PicInfoLb.Text = string.Format("{1}/{0}", JobManager.ResultList.Count, DisplayingResult.Value.index - JobManager.ResultList.First.Value.index + 1);
            var id = FuncCboxSelectedIndex;
            if (JobManager.MtdBlocks?[id] != null)
            {
                StatusLb.Text = $"合格/总数\r\n{JobManager.MtdBlocks[id].PassCount}/{JobManager.MtdBlocks[id].TotalCount}";
            }
            StatusLabel1.Text = $"待检产品缓存:{JobManager.GetBufString()}";
        }

        internal void UpdateDisplay(ICogImage re, int id)
        {
            if (id < cogDisplayGlobal?.Length)
                cogDisplayGlobal[id].Image = re;
        }
        internal void UpdateGraphics(RxInterface re, int id)
        {
            if (re == null)
            {
                cogDisplayGlobal[id].Image = null;
                return;
            }
            var dp = cogDisplayGlobal[id];
            dp.DrawingEnabled = false;
            dp.Image = re.img;
            if (Settings.Default.HideGraphics == false)
                JobManager.graphics.UpdateGraphics(re, dp);
            else
                JobManager.graphics.SwitchGraphicsVisibility(dp);
            dp.DrawingEnabled = true;

        }

        void ApplySysSettings()
        {
            label20.Text = string.Format("{0}({1})", Settings.Default.标题, System.IO.Path.GetFileNameWithoutExtension(Settings.Default.jobs));
            //if (Settings.Default.DebugMode)
            //{
            //    cogDisplay1.Visible = false;
            //    recordsdisp.Visible = true;
            //    JobManager.CreateRecord = true;
            //}
            //else
            {
                cogDisplay1.Visible = true;
                if (Settings.Default.HideGraphics)
                    cogDisplay1.InteractiveGraphics.Clear();
            }
            if (System.IO.File.Exists(Settings.Default.jobs))
            {
                JobManager.LoadJob();
                JobManager.TrySaveSettings(Settings.Default.jobs);
                label20.Text = string.Format("{0}({1})", Settings.Default.标题, System.IO.Path.GetFileNameWithoutExtension(Settings.Default.jobs));
            }
            JobManager.SyncExposureTime(-1);
            //JobManager.barTenderPrint.SetCopies(Settings.Default.PrintCopies);

            //summaryBtn.Visible = !Settings.Default.HidePrintButton;
        }
        public void NewTrigHdl()
        {
            if (this.InvokeRequired)
                this.BeginInvoke(new Action(NewTrigHdl));
            else
            {
                if (DisplayingResult == null)
                {
                    if (FuncCbox.SelectedIndex == 0)
                        UpdateResult(JobManager.LastResult);
                    else
                    {
                        FuncCbox.SelectedIndex = 0;
                    }
                    //cogDisplay1.Image = null;
                    //propertyGrid1.SelectedObject = null;
                    //ProdInfoTextbox.BackColor = Color.LightGreen;
                    //ProdInfoTextbox.Text = "";
                }
            }
        }

        internal int SyncCount = 0;
        double fpsMem = 0;
        int totalMem = 0;
        int test = 0;
        void AsyncTester()
        {
            for (int i = 0; i < 10; i++)
            {
                Thread.Sleep(1000);
                this.BeginInvoke(new Action(() => StatusLb.Text = string.Format("{0}_{1}", test, test2)));
                test++;
            }
        }
        int test2 = 0;
        void AsyncTester2()
        {
            for (int i = 0; i < 10; i++)
            {
                Thread.Sleep(1000);
                this.BeginInvoke(new Action(() => StatusLb.Text = string.Format("{0}_{1}", test, test2)));
                test2++;
            }
        }
        void Init(object state)
        {
            try
            {
                //TaskAsyncHelper.RunAsync(new Action(AsyncTester), null);
                //Thread.Sleep(500);
                //TaskAsyncHelper.RunAsync(new Action(AsyncTester2), null);
                JobManager.Init();
                if (Settings.Default.test == false)
                {
                    //JobManager.CheckLicense();
                }
            }
            catch (Cognex.VisionPro.Exceptions.CogSecurityViolationException ex)
            {
                Program.MsgBox("系统错误：" + ex.Source.Length);
            }
            catch (System.IO.FileNotFoundException ex)
            {
                Program.Loginfo(ex.Message);
                Program.MsgBox(ex.Message);
            }
            catch (InvalidOperationException)
            {
                Program.MsgBox("系统错误");
            }
            catch (Exception ex)
            {
                Program.ErrHdl(ex);
            }

            try
            {
                JobManager.ResultChanged += ResultChangedHdl;
                JobManager.IOResultChanged += JobManager_IOResultChanged;
                JobManager.ResultUpdate += ResultUpdateHdl;
                JobManager.NewTrig += NewTrigHdl;
                //var tps = (from t in Assembly.GetExecutingAssembly().GetTypes()
                //           where t.BaseType == null ? false : t.BaseType.Equals(typeof(Block1Settings))
                //           select t).ToDictionary(key => key.GetField("Name").GetRawConstantValue()
                //, value => value.GetField("NameParm").GetRawConstantValue());
                //for (int i = 0; i < Block1Settings.MyMtdConverter.li.Count; i++)
                //{
                //    var item = Block1Settings.MyMtdConverter.li[i];
                //    object val;
                //    if (tps.TryGetValue(item, out val))
                //    {
                //        string str = val as string;
                //        if (str != null && JobManager.MtdBlocks[0].internalblock.Tools.Contains(str))
                //            continue;
                //    }
                //    Block1Settings.MyMtdConverter.li.Remove(item);
                //    i--;
                //}
            }
            catch (Exception ex)
            {
                Program.ErrHdl("内部错误61661：" + ex.Message);
            }

            Thread.Sleep(100);
            if (Disposing == false && IsDisposed == false)
            {
                BeginInvoke(new Action(loadfin));
            }
        }

        internal static Cognex.VisionPro.Display.CogDisplay[] cogDisplayGlobal;
        void InitCogDisplay(int displayCount)
        {
            int wd = splitContainer1.Size.Height * 4 / 3;
            if (wd > 100)
                splitContainer1.SplitterDistance = wd;
            if (displayCount > 1)
            {
                cogDisplayGlobal = new Cognex.VisionPro.Display.CogDisplay[displayCount + (displayCount & 0x01)];
                cogDisplayGlobal[0] = cogDisplay1;
                tableLayoutPanel4.ColumnCount = (int)Math.Ceiling(displayCount / 2.0);
                if (displayCount > 2)
                    tableLayoutPanel4.RowCount = 2;
                if (displayCount > 6)
                    tableLayoutPanel4.ColumnCount = 4;
                else if (displayCount > 4)
                    tableLayoutPanel4.ColumnCount = 3;
                else
                    tableLayoutPanel4.ColumnCount = 2;
                for (int i = 1; i < cogDisplayGlobal.Length; i++)
                {
                    cogDisplayGlobal[i] = new Cognex.VisionPro.Display.CogDisplay();
                    cogDisplayGlobal[i].ContextMenuStrip = null;
                    cogDisplayGlobal[i].Dock = System.Windows.Forms.DockStyle.Fill;
                    cogDisplayGlobal[i].Margin = new System.Windows.Forms.Padding(0);
                    cogDisplayGlobal[i].Padding = new System.Windows.Forms.Padding(0);
                    cogDisplayGlobal[i].DoubleClick += new System.EventHandler(this.cogDisplay1_DoubleClick);
                    cogDisplayGlobal[i].Click += new System.EventHandler(this.cogDisplay1_Click);
                    cogDisplayGlobal[i].MouseMove += new System.Windows.Forms.MouseEventHandler(this.cogDisplay1_MouseMove);
                    //cogDisplayGlobal[i].BackColor = cogDisplay1.BackColor;
                }
                switch (displayCount)
                {
                    case 2:
                        tableLayoutPanel4.SetCellPosition(cogDisplayGlobal[1], new TableLayoutPanelCellPosition(1, 0));
                        tableLayoutPanel4.Controls.Add(cogDisplayGlobal[1]);
                        splitContainer1.SplitterDistance = splitContainer1.SplitterDistance + 70;
                        break;
                    case 3:
                    case 4:
                        tableLayoutPanel4.SetCellPosition(cogDisplayGlobal[1], new TableLayoutPanelCellPosition(1, 0));
                        tableLayoutPanel4.Controls.Add(cogDisplayGlobal[1]);
                        tableLayoutPanel4.SetCellPosition(cogDisplayGlobal[2], new TableLayoutPanelCellPosition(0, 1));
                        tableLayoutPanel4.Controls.Add(cogDisplayGlobal[2]);
                        tableLayoutPanel4.SetCellPosition(cogDisplayGlobal[3], new TableLayoutPanelCellPosition(1, 1));
                        tableLayoutPanel4.Controls.Add(cogDisplayGlobal[3]);
                        break;
                    case 5:
                    case 6:
                        tableLayoutPanel4.SetCellPosition(cogDisplayGlobal[1], new TableLayoutPanelCellPosition(1, 0));
                        tableLayoutPanel4.Controls.Add(cogDisplayGlobal[1]);
                        tableLayoutPanel4.SetCellPosition(cogDisplayGlobal[2], new TableLayoutPanelCellPosition(2, 0));
                        tableLayoutPanel4.Controls.Add(cogDisplayGlobal[2]);
                        tableLayoutPanel4.SetCellPosition(cogDisplayGlobal[3], new TableLayoutPanelCellPosition(0, 1));
                        tableLayoutPanel4.Controls.Add(cogDisplayGlobal[3]);
                        tableLayoutPanel4.SetCellPosition(cogDisplayGlobal[4], new TableLayoutPanelCellPosition(1, 1));
                        tableLayoutPanel4.Controls.Add(cogDisplayGlobal[4]);
                        tableLayoutPanel4.SetCellPosition(cogDisplayGlobal[5], new TableLayoutPanelCellPosition(2, 1));
                        tableLayoutPanel4.Controls.Add(cogDisplayGlobal[5]);
                        wd = splitContainer1.Size.Height * 5 / 4 * 3 / 2;
                        //if(wd+100> splitContainer1.Size.Width)
                        //Program.MsgBox($"{wd} {splitContainer1.Size}");
                        splitContainer1.SplitterDistance = wd;
                        break;
                    case 7:
                    case 8:
                        tableLayoutPanel4.SetCellPosition(cogDisplayGlobal[1], new TableLayoutPanelCellPosition(1, 0));
                        tableLayoutPanel4.Controls.Add(cogDisplayGlobal[1]);
                        tableLayoutPanel4.SetCellPosition(cogDisplayGlobal[2], new TableLayoutPanelCellPosition(2, 0));
                        tableLayoutPanel4.Controls.Add(cogDisplayGlobal[2]);
                        tableLayoutPanel4.SetCellPosition(cogDisplayGlobal[3], new TableLayoutPanelCellPosition(3, 0));
                        tableLayoutPanel4.Controls.Add(cogDisplayGlobal[3]);
                        tableLayoutPanel4.SetCellPosition(cogDisplayGlobal[4], new TableLayoutPanelCellPosition(0, 1));
                        tableLayoutPanel4.Controls.Add(cogDisplayGlobal[4]);
                        tableLayoutPanel4.SetCellPosition(cogDisplayGlobal[5], new TableLayoutPanelCellPosition(1, 1));
                        tableLayoutPanel4.Controls.Add(cogDisplayGlobal[5]);
                        tableLayoutPanel4.SetCellPosition(cogDisplayGlobal[6], new TableLayoutPanelCellPosition(2, 1));
                        tableLayoutPanel4.Controls.Add(cogDisplayGlobal[6]);
                        tableLayoutPanel4.SetCellPosition(cogDisplayGlobal[7], new TableLayoutPanelCellPosition(3, 1));
                        tableLayoutPanel4.Controls.Add(cogDisplayGlobal[7]);
                        break;
                }
            }
            else
            {
                cogDisplayGlobal = new Cognex.VisionPro.Display.CogDisplay[1];
                cogDisplayGlobal[0] = cogDisplay1;
                //GongweiCBox.Visible = false;
                //tableLayoutPanel3.SetCellPosition(FuncCbox, new TableLayoutPanelCellPosition(0, 0));
                //tableLayoutPanel3.SetColumnSpan(FuncCbox, 2);
            }
            if (Size.Width - splitContainer1.SplitterDistance < 100)
                splitContainer1.SplitterDistance = Size.Width - 100;
        }
        void loadfin()
        {
            ProgressBar1.Visible = false;
            if (JobManager.Inited == false)
            {
                StatusLabel1.Text = "初始化未成功";
            }
            else
            {
                StatusLabel1.Text = null;
                tableLayoutPanel3.Enabled = tableLayoutPanel6.Enabled = tableLayoutPanel2.Enabled = FuncCbox.Enabled = true;
                FuncCbox.SelectedIndex = 0;
                UpdateGlobal();
                totalMem = JobManager.TotalCount;
                timer1.Enabled = true;
            }
        }
        void testrun()
        {
            float[] ar = new float[30000];
            for (int i = 0; i < 100; i++)
                for (int j = 0; j < 100; j++)
                {
                    var xy = new PointF(i * 0.01f, j * 0.01f);
                    var cr = xy.CalcRGB(10);
                    ar[i * 300 + j] = cr.R;
                    ar[i * 300 + 100 + j] = cr.G;
                    ar[i * 300 + 200 + j] = cr.B;
                }
            var ig = new PhotometricaColorImage(ar, 100, 100);
            JobManager.TrigTest(ig);
        }
        private void InfoChangedHdl(object sender, EventArgs e)
        {
            BeginInvoke(new Action<string>((string text) => StatusLb.Text = text), new object[] { sender as string });
        }
        private void ImgGrabedHdl(object sender, EventArgs e)
        {
            BlockBase bb = sender as BlockBase;
            if (bb != null)
                BeginInvoke(new Action<string>((string text) => StatusLb.Text = text), new object[] { string.Format("工位{0}采像完成", bb.blockid) });
        }
        private void InitInfoHdl(object sender, EventArgs e)
        {
            if (this.Disposing || this.IsDisposed)
                JobManager.Dispose();
            else if (this.InvokeRequired)
                this.BeginInvoke(new EventHandler(InitInfoHdl), new object[] { sender, e });
            else
            {
                ProgressBar1.Value = (int)sender;
            }
        }
        bool TestBtnFlag = false;
        private void ResultUpdateHdl(RxInterface r, int e)
        {
            if (InvokeRequired)
                BeginInvoke(new Action<RxInterface, int>(ResultUpdateHdl), new object[] { r, e });
            else
            {
                SyncCount = 0;
                if (DisplayingResult == null)
                    UpdateGraphics(r, e);
                UpdateGlobal();
            }
        }
        private void ResultChangedHdl(ResultStruct sender, int e)
        {
            if (InvokeRequired)
                BeginInvoke(new Action<ResultStruct, int>(ResultChangedHdl), new object[] { sender, e });
            else
            {
                ResultStruct re = sender;
                if (DisplayingResult == null)
                {
                    if (re != null)
                    {
                        if (FuncCbox.SelectedIndex != e)
                            FuncCbox.SelectedIndex = e;
                        else
                            UpdateResult(null, e);
                        TestBtnFlag = false;
                    }
                }
                else
                    UpdateResult(re, e);
            }
        }
        private void JobManager_IOResultChanged(ResultStruct re)
        {
            if (DisplayingResult != null)
                return;
            if (InvokeRequired)
                BeginInvoke(new Action<ResultStruct>(JobManager_IOResultChanged), new object[] { re });
            else
            {
                if (re == null)
                {
                    propertyGrid1.SelectedObject = null;
                    ProdInfoTextbox.Text = null;
                    ProdInfoTextbox.BackColor = Color.LightGreen;
                    return;
                }
                propertyGrid1.SelectedObject = re;
                ProdInfoTextbox.Text = re.Info;
                if (re.Failed)
                {
                    ProdInfoTextbox.BackColor = Color.Red;
                }
                else
                {
                    ProdInfoTextbox.BackColor = Color.LightGreen;
                }
                UpdateGlobal();
                ParmsInputDialogue.MoveSplitterTo(propertyGrid1);
            }
        }
        private void ProdInfoTextbox_MouseEnter(object sender, EventArgs e)
        {
            if (ProdInfoTextbox.TextLength > 3)
                toolTip1.Show(ProdInfoTextbox.Text, ProdInfoTextbox);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (Program.ExitFlag == false)
                    Program.Loginfo("程序异常退出");
                Program.ExitFlag = true;
                if (li != null)
                    li.Close();
                JobManager.Dispose();
                CamerasGige.Dispose();
            }
            catch { }
            //if (camUsb != null)
            //    camUsb.Dispose();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            int wd = splitContainer1.Size.Height * 4 / 3;
            if (Settings.Default.StepCount > 4)
            {
                wd = splitContainer1.Size.Height * 2;
            }
            if (wd > splitContainer1.Size.Width * 0.85)
                wd = (int)(splitContainer1.Size.Width * 0.85);
            if (wd < splitContainer1.Size.Width * 0.7)
                wd = (int)(splitContainer1.Size.Width * 0.7);
            if (wd > 400)
                splitContainer1.SplitterDistance = wd;
            //ChkFalseShutdown();

            if (cogDisplayGlobal != null)
            {
                Label lb = new Label
                {
                    Location = new Point(10, 10),
                    Text = "相机1",
                    ForeColor = panel1.BackColor,
                    BackColor = Color.Transparent,
                    AutoSize = true
                };
                cogDisplayGlobal[0].Controls.Add(lb);
                for (int i = 1; i < cogDisplayGlobal.Length; i++)
                {
                    cogDisplayGlobal[i].AutoFit = true;
                    cogDisplayGlobal[i].VerticalScrollBar = false;
                    cogDisplayGlobal[i].HorizontalScrollBar = false;
                    cogDisplayGlobal[i].BackColor = cogDisplay1.BackColor;
                    lb = new Label
                    {
                        Location = new Point(10, 10),
                        Text = "相机" + (i + 1),
                        ForeColor = panel1.BackColor,
                        BackColor = Color.Transparent,
                        AutoSize = true
                    };
                    cogDisplayGlobal[i].Controls.Add(lb);
                }
            }
        }

        void ChkFalseShutdown()
        {
            var lg = new System.Diagnostics.EventLog();
            lg.Log = "System";
            //lg.Source = "Kernel-Power";
            bool falseflag = false;
            StringBuilder sb = new StringBuilder();
            DateTime tm = DateTime.Now.AddMonths(-1);
            int errct = 0;
            for (int i = lg.Entries.Count - 1; i > 0; i--)
            {
                var l = lg.Entries[i];
                if (l.InstanceId == 13 && sb.Length == 0) // shutdown
                {
                    //Program.MsgBox("关机时间：" + l.TimeGenerated);
                    break;
                }
                if (l.EventID == 6008) // 异常断电 41
                {
                    //Program.ErrHdl("检测到非正常关机：" + l.TimeGenerated);
                    //"上一次系统的 13:59:33 在 ‎2019/‎11/‎18 上的关闭是意外的。"
                    if (sb.Length == 0)
                        sb.Append("检测到异常关机：").Append(l.Message).AppendLine();
                    errct++;
                }
                if ((DateTime.Now - l.TimeGenerated).TotalDays > 30)
                {
                    if (sb.Length > 0)
                    {
                        sb.Append("近一个月异常关机次数：" + errct);
                        Program.ErrHdl(sb.ToString());
                    }
                    break;
                }
            }
        }
        static int FuncCboxSelectedIndex = 0;
        private void FuncCbox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (FuncCbox.SelectedIndex >= 0)
            {
                FuncCboxSelectedIndex = FuncCbox.SelectedIndex;
                if (DisplayingResult == null)
                {
                    if (JobManager.LastResult != null)
                        UpdateResult(null);
                }
                else
                    UpdateResult(DisplayingResult.Value);
                ParmsInputDialogue.MoveSplitterTo(propertyGrid1);
            }
        }

        private void propertyGrid1_SelectedGridItemChanged(object sender, SelectedGridItemChangedEventArgs e)
        {
            if (e.NewSelection.Value is ICogImage)
            {
                var cd = cogDisplay1;
                if (cogDisplayGlobal?.Length > FuncCboxSelectedIndex)
                    cd = cogDisplayGlobal[FuncCboxSelectedIndex];
                cd.Image = (ICogImage)e.NewSelection.Value;
            }
        }

        static System.Diagnostics.Process osk;
        static System.Diagnostics.Process showKeyBoard()
        {
            if (Environment.OSVersion.Version.Major >= 6)
                if (System.IO.File.Exists("osk.exe"))
                    return System.Diagnostics.Process.Start("osk.exe");
                //else if (System.IO.File.Exists(@"C:\Windows\System32\osk.exe"))
                //    return System.Diagnostics.Process.Start(@"C:\Windows\System32\osk.exe");
                else { }
            else if (System.IO.File.Exists("osk_xp.exe"))
                return System.Diagnostics.Process.Start("osk_xp.exe");
            return null;
        }
        internal static void ShowKeyBoard()
        {
            if (osk == null)
                osk = Form1.showKeyBoard();
            else if (osk.HasExited)
                osk.Start();
        }
        internal static void HideKeyBoard()
        {
            HideKeyBoard(null, EventArgs.Empty);
        }
        internal static void HideKeyBoard(object sender, EventArgs e)
        {
            if (osk != null)
            {
                try
                {
                    //if (osk.HasExited == false)
                    osk.CloseMainWindow();
                }
                catch
                {
                }
                osk = null;
            }
        }

        System.IO.FileInfo[] fiMem;
        int fileMemId;
        private void TestImgBtn_MouseDown(object sender, MouseEventArgs e)
        {
            if (JobManager.IsRunning)
            {
                Program.MsgBox("系统正在检测中，请先停止运行");
                return;
            }
            if (cogDisplayGlobal == null)
                cogDisplay1.Image = null;
            else
                cogDisplayGlobal[FuncCbox.SelectedIndex].Image = null;
            if (e.Button == MouseButtons.Left)
            {
                if (CameraHdlBase.extTrigFlag)
                {
                    JobManager.TestImgFlag = true;
                    plcHdl.WritePlcInternalAsync(PlcInterface.AddrBase + 36, (ushort)(1 + FuncCboxSelectedIndex));
                }
                else
                    JobManager.TrigTest(null, FuncCbox.SelectedIndex);
            }
            else
            {
                enableManualPrint = true;
                OpenFileDialog op = new OpenFileDialog();
                //op.InitialDirectory = Settings.SystemDIR + @"\img";
                if (op.ShowDialog() == DialogResult.OK)
                {
                    if (JobManager.MtdBlocks[FuncCbox.SelectedIndex] != null)
                    {
                        var bmp = (Bitmap)Image.FromFile(op.FileName);
                        if (bmp.PixelFormat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed)
                            JobManager.TrigTest(new CogImage8Grey(bmp), FuncCbox.SelectedIndex);
                        else
                            JobManager.TrigTest(new CogImage24PlanarColor(bmp), FuncCbox.SelectedIndex);

                        var dir = System.IO.Path.GetDirectoryName(op.FileName);
                        var dd = new System.IO.DirectoryInfo(dir);
                        fiMem = dd.GetFiles();
                        fileMemId = 0;
                    }
                }
                else
                    return;
            }
            TestBtnFlag = true;
            GC.Collect();
        }

        bool CaptureContinuous = false;
        private void PicInfoLb_DoubleClick(object sender, EventArgs e)
        {
            if (CameraHdlBase.extTrigFlag == false)
            {
                camTestid = FuncCbox.SelectedIndex;
                ThreadPool.QueueUserWorkItem(CamTest);
            }
        }
        int camTestid = 0;
        bool enableManualPrint = false;
        void CamTest(object state)
        {
            CaptureContinuous = true;
            ThreadPool.QueueUserWorkItem(CamTest2);
            Program.MsgBox("相机调试");
            CaptureContinuous = false;
        }
        void CamTest2(object state)
        {
            try
            {
                int ct = 0;
                if (JobManager.MtdBlocks != null && JobManager.MtdBlocks[camTestid] != null)
                {
                    JobManager.MtdBlocks[camTestid].SyncExposureTime();
                    CameraHdlBase cam = JobManager.MtdBlocks[camTestid].camUsb;
                    //if (cam == null)
                    //    cam = JobManager.camUsb;
                    while (CaptureContinuous)
                    {
                        if (JobManager.MtdBlocks[camTestid].fifo != null)
                        {
                            int trigid;
                            ICogImage img = JobManager.MtdBlocks[camTestid].fifo.Acquire(out trigid);
                            this.BeginInvoke(new Action(() => cogDisplay1.Image = img));
                        }
                        else
                        {
                            ICogImage img;
                            img = cam.GetImage(0);
                            this.BeginInvoke(new Action(() => cogDisplay1.Image = img));
                        }
                        Thread.Sleep(30);
                        if (ct++ > 5)
                        {
                            GC.Collect();
                            ct = 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Program.ErrHdl(ex);
            }
        }

        void CamAUTOTest(object state)
        {
            try
            {
                int ct = 0;
                if (JobManager.MtdBlocks != null && JobManager.MtdBlocks[camTestid] != null)
                {
                    var bk = JobManager.MtdBlocks[camTestid];
                    bk.SyncExposureTime();
                    CameraHdlBase cam = bk.camUsb;
                    //if (cam == null)
                    //    cam = JobManager.camUsb;
                    while (autoRun)
                    {
                        ICogImage img;
                        img = cam.GetImage(0);
                        var r1 = bk.RunSync(img);
                        BeginInvoke(new Action(() => UpdateGraphics(r1, 0)));
                        BeginInvoke(new Action(() => propertyGrid1.SelectedObject = r1));
                        Thread.Sleep(30);
                        if (ct++ > 5)
                        {
                            GC.Collect();
                            ct = 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Program.ErrHdl(ex);
            }
        }


        private void Form1_Shown(object sender, EventArgs e)
        {
            this.Size = Screen.PrimaryScreen.Bounds.Size;
            this.Location = Point.Empty;
        }

        private void cogDisplay1_Click(object sender, EventArgs e)
        {
            //Program.MsgBox(cogDisplay1.Size.ToString());
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
        private void cogDisplay1_MouseMove(object sender, MouseEventArgs e)
        {
            var disp = sender as Cognex.VisionPro.Display.CogDisplay;
            if (disp == null)
                disp = cogDisplay1;
            CogImage8Grey img = disp.Image as CogImage8Grey;
            if (img != null)
            {
                int x = (int)((e.X - disp.Width / 2.0) / disp.Zoom + img.Width / 2.0 - disp.PanX);
                int y = (int)((e.Y - disp.Height / 2.0) / disp.Zoom + img.Height / 2.0 - disp.PanY);
                if (x >= 0 && y >= 0 && x < img.Width && y < img.Height)
                {
                    byte pix = img.GetPixel(x, y);
                    StatusLabel1.Text = string.Format("({0},{1}) 处像素值 = {2}", x, y, pix);
                }
                return;
            }
            CogImage24PlanarColor imgc = disp.Image as CogImage24PlanarColor;
            if (imgc != null)
            {
                int x = (int)((e.X - disp.Width / 2.0) / disp.Zoom + imgc.Width / 2.0 - disp.PanX);
                int y = (int)((e.Y - disp.Height / 2.0) / disp.Zoom + imgc.Height / 2.0 - disp.PanY);
                if (x >= 0 && y >= 0 && x < imgc.Width && y < imgc.Height)
                {
                    byte r, g, b;
                    imgc.GetPixel(x, y, out r, out g, out b);
                    var ptf = Color.FromArgb(r, g, b).CalcHSI();
                    StatusLabel1.Text = string.Format("({0},{1}) 处RGB = {2}, {3}, {4}, HSI = {5:F1}, {6:F1}, {7:F1}", x, y, r, g, b, ptf.Plane0, ptf.Plane1, ptf.Plane2);
                }
                return;
            }
            CogImage16Range img16r = disp.Image as CogImage16Range;
            if (img16r != null)
            {
                int x = (int)((e.X - disp.Width / 2.0) / cogDisplay1.Zoom + img16r.Width / 2.0 - disp.PanX);
                int y = (int)((e.Y - disp.Height / 2.0) / cogDisplay1.Zoom + img16r.Height / 2.0 - disp.PanY);
                if (x >= 0 && y >= 0 && x < img16r.Width && y < img16r.Height)
                {
                    ushort px;
                    bool vs, vs2;
                    Cognex.VisionPro3D.Cog3DVect3 v3d;
                    img16r.GetPixel(x, y, out vs, out px);

                    img16r.MapPoint3DFrom2D(img16r.SelectedSpaceName3D, img16r.SelectedSpaceName, new Cognex.VisionPro3D.Cog3DVect2(x, y), out vs2, out v3d);
                    StatusLabel1.Text = string.Format("({0},{1}) 处 = {3}{4}", x, y, vs, px, v3d);
                }
                return;
            }
        }

        private void label20_DoubleClick(object sender, EventArgs e)
        {
            if (Settings.Default.test && JobManager.MtdBlocks?.Length > 0 && FuncCbox.SelectedIndex >= 0)
            {
                Cognex.VisionPro.ToolBlock.CogToolBlockEditV2 cv = new Cognex.VisionPro.ToolBlock.CogToolBlockEditV2
                {
                    Subject = JobManager.MtdBlocks[FuncCbox.SelectedIndex].internalblock,
                    Dock = DockStyle.Fill
                };
                Form fm = new Form();
                fm.Controls.Add(cv);
                fm.ShowDialog();
                cv.Subject = null;
                cv.Dispose();
            }
        }
        private void TotalCountLb_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && enableManualPrint && LogIn.CurrentUser.UserRights == UserRight.超级管理员)
            {
                enableManualPrint = false;
                summaryBtn_Click(null, EventArgs.Empty);
            }
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        private const int VM_NCLBUTTONDOWN = 0XA1;//定义鼠标左键按下
        private const int HTCAPTION = 2;
        private const int WM_NCHITTEST = 0x0084;
        private const int HTCLIENT = 0x0001;
        DateTime tmMem = DateTime.MinValue;
        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if ((DateTime.Now - tmMem).TotalSeconds < 0.5)
            {
                if (WindowState != FormWindowState.Normal)
                    WindowState = FormWindowState.Normal;
                else
                    WindowState = FormWindowState.Maximized;
                return;
            }
            tmMem = DateTime.Now;
            //为当前应用程序释放鼠标捕获
            ReleaseCapture();
            //发送消息 让系统误以为在标题栏上按下鼠标
            SendMessage((IntPtr)this.Handle, VM_NCLBUTTONDOWN, HTCAPTION, 0);
            if (Cursor == Cursors.SizeNWSE)
            {
                direction = MouseDirection.Declining;
                isMouseDown = true;
            }
            else
            {
                direction = MouseDirection.None;
                isMouseDown = false;
            }
        }
        bool isMouseDown = false; //表示鼠标当前是否处于按下状态，初始值为否 
        MouseDirection direction = MouseDirection.None;//表示拖动的方向，起始为None，表示不拖动
        private void Main_MouseMove(object sender, MouseEventArgs e)
        {
            //如果鼠标按下，同时有方向箭头那么直接调整大小,这里是改进的地方，不然斜角拉的过程中，会有问题
            if (isMouseDown && Cursor != Cursors.Default)
            {
                //设定好方向后，调用下面方法，改变窗体大小  
                ResizeWindow();
                return;
            }

            //鼠标移动过程中，坐标时刻在改变 
            //当鼠标移动时横坐标距离窗体右边缘5像素以内且纵坐标距离下边缘也在5像素以内时，要将光标变为倾斜的箭头形状，同时拖拽方向direction置为MouseDirection.Declining 
            if (e.Location.X >= this.Width - 10 && e.Location.Y > this.Height - 10)
            {
                this.Cursor = Cursors.SizeNWSE;
                direction = MouseDirection.Declining;
            }
            //当鼠标移动时横坐标距离窗体右边缘5像素以内时，要将光标变为倾斜的箭头形状，同时拖拽方向direction置为MouseDirection.Herizontal
            else if (e.Location.X >= this.Width - 10)
            {
                this.Cursor = Cursors.SizeWE;
                direction = MouseDirection.Herizontal;
            }
            //同理当鼠标移动时纵坐标距离窗体下边缘5像素以内时，要将光标变为倾斜的箭头形状，同时拖拽方向direction置为MouseDirection.Vertical
            else if (e.Location.Y >= this.Height - 10)
            {
                this.Cursor = Cursors.SizeNS;
                direction = MouseDirection.Vertical;

            }
            //否则，以外的窗体区域，鼠标星座均为单向箭头（默认）             
            else
                this.Cursor = Cursors.Arrow;
        }
        private void ResizeWindow()
        {
            //这个判断很重要，只有在鼠标按下时才能拖拽改变窗体大小，如果不作判断，那么鼠标弹起和按下时，窗体都可以改变 
            if (!isMouseDown)
                return;
            //MousePosition的参考点是屏幕的左上角，表示鼠标当前相对于屏幕左上角的坐标this.left和this.top的参考点也是屏幕，属性MousePosition是该程序的重点
            if (direction == MouseDirection.Declining)
            {
                //此行代码在mousemove事件中已经写过，在此再写一遍，并不多余，一定要写
                //this.Cursor = Cursors.SizeNWSE;
                //下面是改变窗体宽和高的代码，不明白的可以仔细思考一下
                this.Width = MousePosition.X - this.Left;
                this.Height = MousePosition.Y - this.Top;
            }
            //以下同理
            if (direction == MouseDirection.Herizontal)
            {
                this.Cursor = Cursors.SizeWE;
                this.Width = MousePosition.X - this.Left;
            }
            else if (direction == MouseDirection.Vertical)
            {
                this.Cursor = Cursors.SizeNS;
                this.Height = MousePosition.Y - this.Top;
            }
            //即使鼠标按下，但是不在窗口右和下边缘，那么也不能改变窗口大小
            else
                this.Cursor = Cursors.Arrow;
        }

        private void Main_MouseUp(object sender, MouseEventArgs e)
        {
            isMouseDown = false;
            direction = MouseDirection.None;
        }
        public enum MouseDirection
        {
            Herizontal,//水平方向拖动，只改变窗体的宽度
            Vertical,//垂直方向拖动，只改变窗体的高度
            Declining,//倾斜方向，同时改变窗体的宽度和高度
            None//不做标志，即不拖动窗体改变大小
        }

        private void panel1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (WindowState != FormWindowState.Normal)
                WindowState = FormWindowState.Normal;
            else
                WindowState = FormWindowState.Maximized;
        }
        private void StartStopBtn_MouseEnter(object sender, EventArgs e)
        {
            Control b = (Control)sender;
            if (b == StartStopBtn)
                b.BackgroundImage = Properties.Resources.btnMouseOver;
            else if (b == UsersBtn)
                b.BackgroundImage = btnROver;
            else if (b == AlgSettingsBtn || b == plcBtn || b == LogsBtn)
            {
                b.BackgroundImage = Properties.Resources.btn2over;
            }
            else
            {
                b.BackgroundImage = Properties.Resources.btn3over;
            }
        }
        private void StartStopBtn_MouseLeave(object sender, EventArgs e)
        {
            Control b = (Control)sender;
            if (b == StartStopBtn)
                b.BackgroundImage = Properties.Resources.btn;
            else if (b == UsersBtn)
                b.BackgroundImage = btnR;
            else if (b == AlgSettingsBtn || b == plcBtn || b == LogsBtn)
            {
                b.BackgroundImage = Properties.Resources.btn2;
            }
            else
            {
                b.BackgroundImage = Properties.Resources.btn3;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (CameraHdlBase.extTrigFlag == false)
            {
                timer1.Enabled = false;
                return;
            }
            int tt = JobManager.TotalCount;
            if (totalMem > tt)
                totalMem = tt;
            fpsMem = fpsMem * 0.98 + (tt - totalMem) * 2.4;
            totalMem = tt;
            if (plcHdl != null)
            {
                if (plcHdl.err != null)
                {
                    StartStopBtn.Text = "通讯异常";
                    StartStopBtn.ForeColor = Color.Red;
                }
                else if (plcHdl.DataStore.TryGetValue(PlcInterface.AddrBase - 48, out ushort d952))
                {
                    StartStopBtn.Text = $"{(FormPLC.PLCStatus)(d952)}";
                    if (d952 == 1)
                    {
                        if (SyncCount < 30)
                        {
                            SyncCount++;
                            if (SyncCount == 30)
                                JobManager.SyncBuf();
                        }
                        JobManager.IsRunning = true;
                        StartStopBtn.ForeColor = Color.LawnGreen;
                    }
                    else
                    {
                        SyncCount = 0;
                        JobManager.IsRunning = false;
                        if (d952 == 0 || d952 == 6)
                            StartStopBtn.ForeColor = Color.Black;
                        else
                            StartStopBtn.ForeColor = Color.Red;
                    }
                    var d = (DateTime.Now - startTime).TotalDays;
                    if (d > 2.5)
                    {
                        if (SyncCount >= 30 || d952 != 1)
                        {
                            Program.Loginfo("auto init");
                            startTime = DateTime.Now;
                            clcExe.TryInit();
                        }
                    }
                    else if (d > 2.9)
                    {
                        startTime = DateTime.Now;
                        Program.Loginfo("auto init 2");
                        clcExe.TryInit();
                    }
                }
            }
        }


        //protected override void WndProc(ref Message m)
        //{
        //    base.WndProc(ref m);

        //    if (m.Msg == WM_NCHITTEST && (int)m.Result == HTCLIENT)
        //    {
        //        m.Result = (IntPtr)HTCAPTION;
        //    }
        //}
    }
}
