using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Concurrent;
using Cognex.VisionPro;
using System.Threading;

namespace vpc
{
    internal class SaveFile
    {
        string folder = "img";
        static string checkdir;
        ConcurrentQueue<ResultStruct> cQueue = new ConcurrentQueue<ResultStruct>();
        AutoResetEvent wait = new AutoResetEvent(false);
        internal SaveFile()
        {
            ThreadPool.QueueUserWorkItem(Run);
        }
        internal void SaveFileAsync(ResultStruct re)
        {
            cQueue.Enqueue(re);
            wait.Set();
        }
        void Run(object state)
        {
            checkdir = Settings.SystemDIR;
            string pth = Settings.Default.ImgSavePath;
            if (pth != null && pth.Length > 3)
            {
                try
                {
                    if (Directory.Exists(pth) == false)
                    {
                        System.IO.Directory.CreateDirectory(pth);
                    }

                    if (Directory.Exists(pth))
                    {
                        folder = Settings.Default.ImgSavePath;
                        checkdir = folder;
                    }
                }
                catch (Exception ex)
                {
                    Program.ErrHdl(ex);
                }
            }
            while (Program.ExitFlag == false)
            {
                try
                {
                    if (cQueue.IsEmpty)
                        wait.WaitOne(3000);
                    else
                    {
                        ResultStruct re;
                        if (cQueue.TryDequeue(out re))
                        {
                            CheckOldFolders();
                            string path = null;

                            using (Cognex.VisionPro.ImageFile.CogImageFileBMP cfp = new Cognex.VisionPro.ImageFile.CogImageFileBMP())
                            using (Cognex.VisionPro.ImageFile.CogImageFileJPEG cfj = new Cognex.VisionPro.ImageFile.CogImageFileJPEG())
                            {
                                for (int i = 0; i < re.Count; i++)
                                {
                                    if (Settings.Default.SaveLastImg && re.Failed == false)
                                        i = re.Count - 1;
                                    if (re[i] != null && re[i].img != null)
                                    {
                                        if (path == null)
                                        {
                                            string extrainfo = null;
                                            if (re.ExtraInfo != null && re.ExtraInfo.Length > 1 && false == string.IsNullOrEmpty(re.ExtraInfo[1]))
                                            {
                                                StringBuilder rBuilder2 = new StringBuilder();
                                                rBuilder2.Append("_").Append(re.ExtraInfo[1]);
                                                if (re.ExtraInfo[0] != "序列号" && re.ExtraInfo[0] != "MTC")
                                                    for (int kk = 3; kk < re.ExtraInfo.Length; kk += 2)
                                                    {
                                                        rBuilder2.Append(re.ExtraInfo[kk]);
                                                    }
                                                foreach (char rInvalidChar in Path.GetInvalidFileNameChars())
                                                    rBuilder2.Replace(rInvalidChar, '□');
                                                rBuilder2.Replace(' ', '_');
                                                extrainfo = rBuilder2.ToString();
                                            }
                                            else
                                            {
                                                var s = Settings.Default.PrintSerialNum;
                                                if (string.IsNullOrEmpty(s) == false)
                                                    extrainfo = "_" + s;
                                            }
                                            if (re.Count > 2)
                                            {
                                                string job = System.IO.Path.GetFileNameWithoutExtension(Settings.Default.jobs);
                                                path = string.Format(@"{0}\{3}\{1:M-d}_{2}\{1:HHmmss}{4}\", folder, re[i].LogTime, (re.Failed) ? "次品" : "合格品", job, extrainfo);
                                                System.IO.Directory.CreateDirectory(path);
                                            }
                                            else
                                            {
                                                string job = System.IO.Path.GetFileNameWithoutExtension(Settings.Default.jobs);
                                                path = string.Format(@"{0}\{3}\{1:M-d}_{2}\{1:HHmmss}{4}_", folder, re[i].LogTime, (re.Failed) ? "次品" : "合格品", job, extrainfo);
                                                System.IO.Directory.CreateDirectory(string.Format(@"{0}\{3}\{1:M-d}_{2}\", folder, re[i].LogTime, (re.Failed) ? "次品" : "合格品", job, extrainfo));
                                            }
                                        }

                                        StringBuilder rBuilder;
                                        R1Class r1 = re[i] as R1Class;
                                        rBuilder = new StringBuilder(re[i].Info);
                                        foreach (char rInvalidChar in Path.GetInvalidFileNameChars())
                                            rBuilder.Replace(rInvalidChar, '□');
                                        rBuilder.Replace(' ', '_');
                                        if (rBuilder.Length > 100)
                                        {
                                            rBuilder.Remove(100, rBuilder.Length - 100);
                                            rBuilder.Append("。。。");
                                        }
                                        //string filename = string.Format("{0}{2}_{3:mmss}_{1}.BMP", path, rBuilder, i, re[i].LogTime);
                                        if (re[i].SaveFlag == false)
                                        {
                                            //CogStopwatch cw = new CogStopwatch();
                                            //cw.Start();
                                            CogImage16Range im16 = re[i].img as CogImage16Range;
                                            if (im16 != null)
                                            {
                                                using (Cognex.VisionPro.ImageFile.CogImageFilePNG ims = new Cognex.VisionPro.ImageFile.CogImageFilePNG())
                                                {
                                                    string filename = string.Format("{0}{2}_{3:mmss.f}_{1}.png", path, rBuilder, i, re[i].LogTime);
                                                    ims.Open(filename, Cognex.VisionPro.ImageFile.CogImageFileModeConstants.Write);
                                                    ims.Append(im16.GetPixelData());
                                                }
                                            }
                                            else
                                            {
                                                if (re[i].Tag == null || !(re[i].Tag is MemoryStream))
                                                {
                                                    if (Settings.Default.SaveJpeg == false)
                                                    {
                                                        string filename = string.Format("{0}{2}_{3:mmss.f}_{1}.BMP", path, rBuilder, i, re[i].LogTime);
                                                        cfp.Open(filename, Cognex.VisionPro.ImageFile.CogImageFileModeConstants.Write);
                                                        cfp.Append(re[i].img);
                                                    }
                                                    else
                                                    {
                                                        string filename = string.Format("{0}{2}_{3:mmss.f}_{1}.jpg", path, rBuilder, i, re[i].LogTime);
                                                        cfj.Open(filename, Cognex.VisionPro.ImageFile.CogImageFileModeConstants.Write);
                                                        cfj.Append(re[i].img);
                                                    }
                                                    string aoidata = re[i].Tag as string;
                                                    if (!string.IsNullOrEmpty(aoidata))
                                                    {
                                                        string filename2 = string.Format("{0}{2}_{3:mmss.f}_{1}.txt", path, rBuilder, i, re[i].LogTime);
                                                        StreamWriter tw = new StreamWriter(filename2, false);
                                                        tw.Write(aoidata);
                                                        tw.Flush();
                                                        tw.Close();
                                                    }
                                                }
                                                else
                                                {//save pmm
                                                    MemoryStream ms = re[i].Tag as MemoryStream;
                                                    if (ms != null && ms.Length > 0)
                                                    {
                                                        string filename2 = string.Format("{0}{2}_{3:mmss.f}_{1}.PMM", path, rBuilder, i, re[i].LogTime);
                                                        StreamWriter sw = new StreamWriter(filename2, false);
                                                        sw.BaseStream.Write(ms.GetBuffer(), 0, (int)ms.Length);
                                                        sw.Flush();
                                                        sw.Close();
                                                    }
                                                }
                                            }
                                            //Program.MsgBox(cw.Milliseconds.ToString());
                                            re[i].SaveFlag = true;

                                            //var InfosTableAdapter = new DatabaseDataSetTableAdapters.infosTableAdapter();
                                            //InfosTableAdapter.Insert(DateTime.Now, rBuilder.ToString(), filename, null, null, null, LogIn.CurrentUser.Name);
                                        }
                                    }
                                }
                                cfp.Close();
                                cfj.Close();
                            }
                        }
                        Thread.Sleep(10);
                    }
                }
                catch (System.OutOfMemoryException) { }
                catch (Exception ex)
                {
                    Program.ErrHdl(ex);
                }
            }
        }

        internal int Count
        {
            get { return cQueue.Count; }
        }
        DateTime LastCheckedTime = DateTime.MinValue;
        DateTime NextCheckTime = DateTime.MinValue;
        void CheckOldFolders()
        {
            if (DateTime.Now > NextCheckTime)
            {
                double freespace = GetFreeSpace();

                if (freespace < 3700)
                {
                    NextCheckTime = DateTime.Now.AddSeconds(5);
                    DirectoryInfo di = new DirectoryInfo(folder);
                    if (di.Exists)
                    {
                        DirectoryInfo[] dis = di.GetDirectories();
                        if (dis.Length > 0)
                        {
                            Array.Sort(dis, new FolderTimeComparer());
                            var d2 = dis[0].GetDirectories();
                            int val;
                            if (d2.Length <= 1 || int.TryParse(dis[0].Name, out val))
                            {
                                dis[0].Delete(true);
                                Program.Loginfo(string.Format("剩余空间：{1:F0} MB，自动删除：{0}", dis[0].Name, freespace));
                            }
                            else
                            {
                                List<DirectoryInfo> ld = new List<DirectoryInfo>();
                                for (int i = 0; i < dis.Length; i++)
                                {
                                    ld.AddRange(dis[i].GetDirectories());
                                }
                                ld.Sort(new FolderTimeComparer());
                                ld[0].Delete(true);
                                Program.Loginfo(string.Format("剩余空间：{1:F0} MB，自动删除：{2}\\{0}", ld[0].Name, freespace, ld[0].Parent.Name));

                                //Array.Sort(d2, new FolderTimeComparer());
                                //d2[0].Delete(true);
                                //Program.Loginfo(string.Format("剩余空间：{1:F0} MB，自动删除：{2}_{0}", d2[0].Name, freespace, dis[0].Name));
                            }
                        }
                        else
                            Program.MsgBox("硬盘空间不足");
                    }
                    else
                        Program.MsgBox("硬盘空间不足");
                }
                else
                    NextCheckTime = DateTime.Now.AddSeconds(20 + freespace / 1000);
            }
        }
        internal static double GetFreeSpace()
        {
            DriveInfo drive = new DriveInfo(checkdir);

            return drive.AvailableFreeSpace / 1048576.0;
        }
        internal class FolderTimeComparer : IComparer<DirectoryInfo>
        {
            public int Compare(DirectoryInfo x, DirectoryInfo y)
            {
                return (int)(x.LastWriteTime - y.LastWriteTime).TotalSeconds;
            }
        }
    }
}
