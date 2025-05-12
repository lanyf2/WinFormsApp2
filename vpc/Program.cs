using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace vpc
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                bool createNew;
                bool restartflag = false;
                using (System.Threading.Mutex mutex = new System.Threading.Mutex(true, Application.ProductName, out createNew))
                {
                    if (createNew)
                    {
                        Application.ThreadException += ExceptionEventHandler;
                        Settings.LoadToDefault();
                        if (System.IO.File.Exists("license.zip") && false == Settings.Default.DisableRecover && false == System.IO.File.Exists("test.txt"))
                            if (isAdmin() == false)
                            {//&& Settings.Default.test == false 
                                //restartflag = true;
                            }
                            else
                            {
                                try
                                {
                                    CSLC.clcExe.TryInit();
                                    //System.Threading.Thread.Sleep(500);
                                    //Cognex.VisionPro.JobManager.CheckLicense();
                                }
                                catch
                                {
                                    //restartflag = true;
                                }
                            }

                        //if (restartflag)
                        //{
                        //}
                        //else
                        {
                            try
                            {
                                string fp = null;
                                string[] args = Environment.GetCommandLineArgs();
                                bool autofit = false;
                                if (args != null && args.Length > 1)
                                {
                                    ushort us;
                                    if (args.Length > 2 && ushort.TryParse(args[2], out us))
                                        Settings.Default.StepCount = us;
                                    else
                                        autofit = false;

                                    //if (args.Length > 3 && args[3].StartsWith("COM"))
                                    //    Settings.Default.PLCPort = args[3];

                                    string fptmp = string.Format("jobs\\{0}.vpp", args[1]);
                                    if (System.IO.File.Exists(fptmp))
                                    {
                                        fp = fptmp;
                                        if (Settings.Default.jobs != fp)
                                        {
                                            Settings.Default.jobs = fp;
                                            Cognex.VisionPro.JobManager.TryLoadSettings(fp, autofit);
                                        }
                                    }
                                    else if (!args[1].IsNullOrEmpty())
                                    {
                                        Settings.Default.jobs = fptmp;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                ErrHdl(ex);
                            }
                            Program.Loginfo("程序启动");
                            mf = new Form1();
                            Application.Run(mf);
                            return;
                        }
                    }
                    else
                    {
                        Program.MsgBox("应用程序已经在运行中...");
                        Application.Exit();
                    }
                }
                //if (restartflag)
                //{
                //    System.IO.File.Create("test.txt");
                //    System.Diagnostics.ProcessStartInfo pi = new System.Diagnostics.ProcessStartInfo();
                //    pi.UseShellExecute = true;
                //    pi.FileName = Application.ExecutablePath;
                //    pi.Verb = "runas";
                //    try
                //    {
                //        System.Diagnostics.Process.Start(pi);
                //    }
                //    catch
                //    {
                //    }
                //    Application.Exit();
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace);
            }
        }
        static bool isAdmin()
        {
            var wi = System.Security.Principal.WindowsIdentity.GetCurrent();
            var wp = new System.Security.Principal.WindowsPrincipal(wi);
            return wp.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
        }

        public static void ExceptionEventHandler(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            ErrHdl(e.Exception);
        }
        public static void ErrHdl(Exception ex)
        {
            string str = string.Format("{0}\r\n{1}\r\n{2}", ex.GetType().ToString(), ex.Message, ex.StackTrace);
            Loginfo(ex.Message);
            MsgBox(str);
        }
        public static void ErrHdl(string ex)
        {
            Loginfo(ex);
            MsgBox(ex);
        }
        public static bool MsgBoxYesNo(string str)
        {
            if (MessageBox.Show(str, "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                return true;
            else
                return false;
        }
        public static void MsgBox(string str, bool Async = false)
        {
            if (mf != null && mf.IsDisposed == false)
            {
                if (Async)
                    mf.BeginInvoke(new Action(() => MessageBox.Show(str)));
                else
                    mf.Invoke(new Action(() => MessageBox.Show(str)));
            }
            else
                MessageBox.Show(str);
            //System.Diagnostics.Debug.WriteLine(str);
        }
        static DatabaseDataSetTableAdapters.logsTableAdapter logsTbAdapter = new DatabaseDataSetTableAdapters.logsTableAdapter();
        public static void Loginfo(string str)
        {
            try
            {
                logsTbAdapter.Insert(DateTime.Now, LogIn.CurrentUser.Name, str);
            }
            catch
            {
            }
        }
        internal static Form1 mf;
        public static bool ExitFlag = false;
    }
}
