using System;
using Cognex.VisionPro;
using Photometrica;
using System.Runtime.InteropServices;
using PhotometricaCore;
using System.Drawing;
using System.IO;
using WpCommon;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;

namespace vpc
{
    public class PhotometricaHdl : CameraHdlBase
    {
        public struct LumiData
        {
            public LumiData(double l, double xx, double yy)
            {
                Lumi = l;
                x = xx;
                y = yy;
            }
            public double Lumi;
            public double x;
            public double y;
        }
        PhotometricaAutomation pma = new PhotometricaAutomation();
        internal static PmCtrlBroker pmcb;
        internal static PmControl pmc;
        internal string TestLoadFile;
        Dictionary<string, LumiData> aoiMem;
        bool initflag = false;
        internal PhotometricaHdl(int index = 0)
        {
            camindex = index;
        }
        internal override ICogImage GetImage(int discardct = 3)
        {
            try
            {
                if (openPWFlag && Program.mf.InvokeRequired)
                    return (ICogImage)Program.mf.Invoke(new Func<ICogImage>(() => GetImage()));
                if (pmcb == null)
                    throw new Exception("相机未初始化");
                int res;
                if (TestLoadFile != null && File.Exists(TestLoadFile))
                {
                    JobManager.LoadJobFlag = true;
                    res = pma.OpenPMM(TestLoadFile, Photometrica.PmSaveDirty.Never, true, false);
                }
                else
                {
                    if (JobManager.LoadJobFlag)
                    {
                        var pth = Path.GetFullPath(string.Format("{0}\\{1}.PMM", Path.GetDirectoryName(Settings.Default.jobs), Path.GetFileNameWithoutExtension(Settings.Default.jobs)));
                        res = pma.OpenPMM(pth, Photometrica.PmSaveDirty.Never, false, false);
                        throwIfError("OpenPMM failed", res);
                    }
                    if (captureScheme >= 1)
                        res = pma.Capture(string.Format("Color{0:F0}", captureScheme));
                    else
                        res = pma.Capture("Color");
                    if (res == 318)
                        return null;
                    throwIfError("Capture failed", res);
                }
                Bitmap bmp;
                var ree = pmcb.GetMeasurementBitmap("@", true, false, false, false, false, false, string.Empty, out bmp);
                throwIfError("GetMeasurementBitmap", ree.ErrorCode);
                //PmControl pmc = dm2.pmc;
                //var ms = new System.IO.MemoryStream();
                //GdiPlusFileType.SaveBmp(pmc.graphicsDoc.RenderActiveWorkspaceGraphicToImage(), ms);
                //Bitmap bmp = new Bitmap(ms);
                if (bmp == null)
                    return null;
                else
                {
                    ClearAoi();
                    res = pma.RunNamedScript("CAEA_MAXFILTER");
                    throwIfError("RunScript CAEA_MAXFILTER", res);
                    return new CogImage24PlanarColor(bmp);
                }
            }
            catch (Exception exception)
            {
                Program.ErrHdl(exception);
            }
            return null;
        }
        internal override void Init()
        {
            try
            {
                int res = pma.SetPhotometricaCoreFolder(@"C:\Program Files (x86)\Westboro Photonics\Photometrica75");
                throwIfError("SetPhotometricaCoreFolder failed", res);
                if (Settings.Default.test)//false && 
                {
                    System.Threading.Thread.Sleep(1000);
                    res = pma.OpenPhotometrica(null, null, Path.GetFullPath("Measurement1.PMM"));
                    openPWFlag = true;
                    throwIfError("OpenPhotometrica failed", res);
                }
                else
                {
                    res = pma.StartPhotometricaServer(null, null, Path.GetFullPath("Measurement1.PMM"));
                    throwIfError("StartPhotometricaServer failed", res);
                }
                res = pma.LoadPackage("CAEATesting.PMPKG");
                throwIfError("LoadPackage", res);

                dynamic dm2 = ExposedObject.Exposed.From(pma);
                pmcb = dm2.pmcb;
                pmc = dm2.pmc;
                initflag = true;
            }
            catch (Exception ex)
            {
                Program.ErrHdl(ex);
            }
        }
        void throwIfError(string str1, int res)
        {
            if (res != 0)
            {
                string errMsg = string.Empty;
                int res2 = pma.GetLastError(out errMsg);
                if (res2 > 0)
                    throw new Exception(string.Format("{3}[{0},{2}]: {1}", res, errMsg, res2, str1));
                else
                    throw new Exception(string.Format("{2}[{0}]: {1}", res, errMsg, str1));
            }
        }
        void cropToRectangle(int x, int y, int width, int height)
        {
            var re = pma.RemoveCropping();
            throwIfError("RemoveCropping", re);
            pma.SetSelection("rectangle", x, y, width, height);
            re = pma.CropToSelection();
            throwIfError("CropToSelection", re);
        }
        double captureScheme = 0;
        internal override double ExposureTime
        {
            get
            {
                return captureScheme;
            }
            set
            {
                captureScheme = value;
            }
        }
        internal override double Gain
        {
            get
            {
                return -1;
            }
            set
            {
                if (value > 0)
                {

                }
            }
        }
        internal override double Brightness
        {
            get
            {
                return -1;
            }
            set
            {
                if (value > 0)
                {

                }
            }
        }
        internal override void Dispose()
        {
            if (pma != null)
                pma.ClosePhotometrica(Photometrica.PmSaveDirty.Never);
        }
        internal override bool Connected
        {
            get
            {
                return initflag;
            }
        }
        string GetCaptureScheme()
        {
            if (ExposureTime > 0)
            {

            }
            return null;
        }

        bool openPWFlag = false;
        void addAoi(string aoiName, float[] pts)
        {
            int re;
            if (openPWFlag)
                re = (int)Program.mf.Invoke(new Func<int>(() => pma.AddAoiFromPolygon(aoiName, pts)));
            else
                re = pma.AddAoiFromPolygon(aoiName, pts);
            throwIfError("AddAoiFromPolygon", re);
        }
        public double CalcLumi(ICogRegion region)
        {
            return CalcLumi((region as ICogGraphicInteractive)?.TipText, region);
        }
        public double CalcYxy(ICogRegion region, out double x, out double y)
        {
            return CalcYxy((region as ICogGraphicInteractive)?.TipText, region, out x, out y);
        }
        public double CalcLumi(string aoiName, ICogRegion region)
        {
            if (region == null)
                return -1;
            int re;
            if (string.IsNullOrEmpty(aoiName) == false)
            {
                bool flag;
                re = pma.AoiExists(aoiName, out flag);
                throwIfError("AoiExists", re);
                if (flag)
                    aoiName = null;
            }
            if (region is CogRectangle)
            {
                CogRectangle cr = (CogRectangle)region;
                float[] pts = new float[8] { (float)cr.X, (float)cr.Y, (float)(cr.X + cr.Width), (float)cr.Y, (float)(cr.X + cr.Width), (float)(cr.Y + cr.Height), (float)cr.X, (float)(cr.Y + cr.Height) };
                re = pma.AddAoiFromPolygon(aoiName, pts);
                throwIfError("AddAoiFromPolygon", re);
            }
            else if (region is CogRectangleAffine)
            {
                CogRectangleAffine cr = (CogRectangleAffine)region;
                float[] pts = new float[8] { (float)cr.CornerOriginX, (float)cr.CornerOriginY, (float)cr.CornerXX, (float)cr.CornerXY, (float)cr.CornerOppositeX, (float)cr.CornerOppositeY, (float)cr.CornerYX, (float)cr.CornerYY };
                re = pma.AddAoiFromPolygon(aoiName, pts);
                throwIfError("AddAoiFromPolygon", re);
            }
            else
                return -2;
            int ct;
            re = pma.GetAoiCount(null, out ct);
            throwIfError("GetAoiCount", re);
            string name;
            re = pma.GetAoiName(ct - 1, -1, out name);
            throwIfError("GetAoiName", re);
            double lumi;
            re = pma.GetAoiTableCellValue(name, "Luminance", out lumi);
            throwIfError("GetAoiTableCellValue Luminance", re);

            return lumi;
        }
        public double CalcYxy(string aoiName, ICogRegion region, out double x, out double y)
        {
            x = -1;
            y = -1;
            if (region == null)
                return -1;
            int re;
            if (aoiMem != null)
            {
                LumiData ld = new LumiData();
                if (aoiMem.TryGetValue(aoiName, out ld))
                {
                    x = ld.x;
                    y = ld.y;
                    return ld.Lumi;
                }
                else
                {
                    x = -2;
                    y = -2;
                    return -2;
                }
            }
            if (string.IsNullOrEmpty(aoiName) == false)
            {
                bool flag;
                re = pma.AoiExists(aoiName, out flag);
                throwIfError("AoiExists", re);
                if (flag)
                {
                    int ct;
                    re = pma.GetAoiCount(null, out ct);
                    throwIfError("GetAoiCount", re);
                    aoiName = "A" + (ct + 1);
                }
            }
            if (region is CogRectangle)
            {
                CogRectangle cr = (CogRectangle)region;
                float[] pts = new float[8] { (float)cr.X, (float)cr.Y, (float)(cr.X + cr.Width), (float)cr.Y, (float)(cr.X + cr.Width), (float)(cr.Y + cr.Height), (float)cr.X, (float)(cr.Y + cr.Height) };
                addAoi(aoiName, pts);
            }
            else if (region is CogRectangleAffine)
            {
                CogRectangleAffine cr = (CogRectangleAffine)region;
                float[] pts = new float[8] { (float)cr.CornerOriginX, (float)cr.CornerOriginY, (float)cr.CornerXX, (float)cr.CornerXY, (float)cr.CornerOppositeX, (float)cr.CornerOppositeY, (float)cr.CornerYX, (float)cr.CornerYY };
                addAoi(aoiName, pts);
            }
            else if (region is CogPolygon)
            {
                CogPolygon cr = (CogPolygon)region;
                float[] pts = new float[cr.NumVertices * 2];
                for (int i = 0; i < cr.NumVertices; i++)
                {
                    double x2, y2;
                    cr.GetVertex(i, out x2, out y2);
                    pts[i * 2] = (float)x2;
                    pts[i * 2 + 1] = (float)y2;
                }
                addAoi(aoiName, pts);
            }
            else
                return -3;
            double lumi = -5;

            string val;
            re = pma.RunNamedScriptGetReturn("CAEA_TestAoi", out val);
            throwIfError("RunScript CAEA_TestAoi", re);
            var vs = val.Split(new char[] { '\t' });
            double[] v = new double[vs.Length];
            for (int i = 0; i < vs.Length; i++)
            {
                int id = vs[i].IndexOf(':');
                if (double.TryParse(vs[i].Substring(id + 1), out v[i]) == false)
                    v[i] = -7;
            }
            if (v.Length != 3)
                return -9;
            else
            {
                lumi = v[0];
                x = v[1];
                y = v[2];
            }

            //int ct;
            //re = pma.GetAoiCount(null, out ct);
            //throwIfError("GetAoiCount", re);
            //string name;
            //re = pma.GetAoiName(ct - 1, -1, out name);
            //throwIfError("GetAoiName", re);
            //re = pma.GetAoiTableCellValue(name, "Luminance", out lumi);
            //throwIfError("GetAoiTableCellValue Luminance", re);
            //re = pma.GetAoiTableCellValue(name, "x", out x);
            //throwIfError("GetAoiTableCellValue x", re);
            //re = pma.GetAoiTableCellValue(name, "y", out y);
            //throwIfError("GetAoiTableCellValue y", re);

            return lumi;
        }
        public void ClearAoi()
        {
            var re = pma.DeleteAllAoi();
            throwIfError("ClearAoi", re);
        }
        internal override object GetTag()
        {
            try
            {
                if (Settings.Default.test)
                {
                    TestLoadFile = null;
                    aoiMem = null;
                    BinaryFile binaryFile = new BinaryFile();
                    binaryFile.loadContext = new LoadContext(pmc.Pmm);
                    binaryFile.Progress = null;
                    MemoryStream ms = new MemoryStream(1024);
                    PmError pmError = PmError.ConvertBinaryFileError(binaryFile.SaveBinaryStream(ms, pmc.Pmm), binaryFile);
                    binaryFile.loadContext = null;
                    if (pmError.IsError)
                    {
                        Program.MsgBox(binaryFile.errorDetails.ToString());
                        return null;
                    }
                    else
                    {
                        return ms;
                    }
                }
                else
                {
                    string s = null;
                    if (TestLoadFile == null && aoiMem == null)
                    {
                        var re = pma.GetAoiTableData(out s);
                        throwIfError("GetAoiTableData", re);
                    }
                    TestLoadFile = null;
                    aoiMem = null;
                    return s;
                }
            }
            catch (Exception ex)
            {
                return ex.Message + "\r\n" + ex.StackTrace;
            }
            //return null;
        }
        internal void ParseAoiData(string filename)
        {
            if (string.IsNullOrEmpty(filename))
                return;
            if (filename.EndsWith(".PMM"))
                filename = filename.Replace(".PMM", ".txt");
            if (!File.Exists(filename))
                return;
            string tbl = File.ReadAllText(filename);
            if (string.IsNullOrEmpty(tbl))
                return;

            Dictionary<string, LumiData> dt = new Dictionary<string, LumiData>();

            string s;
            var re = pma.GetAoiTableData(out s);
            var s2 = s.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var s3 = new string[s2.Length][];
            for (int i = 0; i < s2.Length; i++)
            {
                s3[i] = s2[i].Split(new char[] { '\t' });
                if (s3[i][0] != null && s3[i][0].IndexOf(':') < 0)
                {
                    double l = -1, x = -1, y = -1;
                    double.TryParse(s3[i][1], out l);
                    double.TryParse(s3[i][3], out x);
                    double.TryParse(s3[i][4], out y);
                    dt.Add(s3[i][0], new LumiData(l, x, y));
                }
            }
            aoiMem = dt;
        }
        class PmmSaver
        {
            public MemoryStream GetPMMStream()
            {
                MemoryStream ms = new MemoryStream(1024);
                binaryWriter = new BinaryWriter(ms, System.Text.Encoding.Unicode);
                chunkStack = new System.Collections.Stack();
                signature = CreateSignatureObject();
                WriteIntChunk(1179471184, 3);
                WriteverFingerprint();
                if (pmc?.Pmm?.documentSettings != null)
                {
                    StartChunk(12, 0);
                    WriteDocument(pmc.Pmm.documentSettings);
                    EndChunk();
                }
                //PmmMetaInfoChunkIO pmmMetaInfoChunkIO = new PmmMetaInfoChunkIO(this);
                StartChunk(19, 0);
                WritepmmMetaInfoChunkIO(pmc.Pmm);
                EndChunk();
                if (SciDataTypes.UserDefinedTypesById.Count > 0)
                {
                    foreach (KeyValuePair<int, UserDefinedType> keyValuePair in SciDataTypes.UserDefinedTypesById)
                    {
                        UserDefinedType value = keyValuePair.Value;
                        StartChunk(56, 0);
                        WriteInt(value.id);
                        WriteString(value.formulaText);
                        WriteString(value.unitSuffix);
                        WriteString(value.descriptiveText);
                        WriteInt((int)value.spaceType);
                        EndChunk();
                    }
                }
                var activeMe = pmc.Pmm.activeMe;
                if (activeMe.IsValid)
                {
                    if (activeMe.IsComponentRootActive)
                    {
                        WriteStringChunk(6, activeMe.ActiveMeasurementOnlyName);
                    }
                    else
                    {
                        WriteStringChunk(6, activeMe.ActiveComponentFormulaName);
                    }
                }
                WriteIntChunk(7, (int)pmc.Pmm._unitsFamilyAtLastSave);
                var Inst = pmc.Pmm.Inst;
                if (Inst._ImagerDeviceList.Count > 1)
                {
                    Inst.FileIO_MultiDeviceINST = false;
                    foreach (ImagerDevice fileIO_ImagerDevice in Inst._ImagerDeviceList)
                    {
                        StartChunk(65, 0);
                        Inst.FileIO_ImagerDevice = fileIO_ImagerDevice;
                        //Inst.WriteChunks(file);
                        if (Inst.FileIO_MultiDeviceINST)
                            throw new NotImplementedException("Inst.FileIO_MultiDeviceINST");
                        else
                            WriteChunksForSingleImagerINS();
                        EndChunk();
                    }
                    Inst.FileIO_ImagerDevice = null;
                }
                else
                {
                    Inst.FileIO_ImagerDevice = Inst._ImagerDeviceList[0];
                }

                return ms;
            }
            void WriteChunksForSingleImagerINS()
            {
                var Inst = pmc.Pmm.Inst;
                ImagerDevice fileIO_ImagerDevice = Inst.FileIO_ImagerDevice;
                WriteIntChunk(1179468099, fileIO_ImagerDevice.version);
                //fileIO_ImagerDevice.verFingerprint.WriteChunk(file);
                WriteverFingerprint();
                if (fileIO_ImagerDevice.version < 4)
                {
                    string text = fileIO_ImagerDevice.cameraIdentity.GetToLegacyCXXXNNN();
                    if (fileIO_ImagerDevice.BinningMode == enCameraBinningMode.TwoByTwo)
                    {
                        text += "_B2";
                    }
                    WriteStringChunk(200, text);
                    if (signature != null)
                    {
                        signature.AddData(text);
                    }
                }
                else
                {
                    WriteStringChunk(200, fileIO_ImagerDevice.cameraIdentity.ToString());
                    if (signature != null)
                    {
                        signature.AddData(fileIO_ImagerDevice.cameraIdentity.ToString());
                    }
                }
            }
            PhotometricaAutomation pma;
            DataSignature signature;
            PmControl pmc;
            BinaryWriter binaryWriter;
            System.Collections.Stack chunkStack;
            public PmmSaver(PhotometricaAutomation _pma, PmControl _pmc)
            {
                pma = _pma;
                pmc = _pmc;
            }
            void StartChunk(int id, int size)
            {
                binaryWriter.Write(id);
                chunkStack.Push(new ChunkStackNode(binaryWriter.BaseStream.Position, size));
                binaryWriter.Write(size);
            }
            public void EndChunk()
            {
                if (chunkStack.Count == 0)
                {
                    return;
                }
                long position = binaryWriter.BaseStream.Position;
                ChunkStackNode chunkStackNode = (ChunkStackNode)this.chunkStack.Pop();
                int num = (int)(position - chunkStackNode.chunkSizePosition - 4L);
                if (num != chunkStackNode.currentChunkSize)
                {
                    SafeWriteSeek(chunkStackNode.chunkSizePosition);
                    binaryWriter.Write(num);
                    SafeWriteSeek(position);
                }
            }
            public void SafeWriteSeek(long pos)
            {
                if (pos <= 2147483647L)
                {
                    binaryWriter.Seek((int)pos, SeekOrigin.Begin);
                    return;
                }
                binaryWriter.Seek(0, SeekOrigin.Begin);
                while (pos > 2147483647L)
                {
                    binaryWriter.Seek(int.MaxValue, SeekOrigin.Current);
                    pos -= 2147483647L;
                }
                binaryWriter.Seek((int)pos, SeekOrigin.Current);
            }
            public DataSignature CreateSignatureObject()
            {
                return pmc.Pmm.CreateSignatureObject();
            }

            void WriteInt(int val)
            {
                binaryWriter.Write(val);
            }
            void WriteBool(bool b)
            {
                binaryWriter.Write(b);
            }
            public void WriteString(string s)
            {
                if (s == null)
                {
                    binaryWriter.Write(0);
                    return;
                }
                if (s.Length > 32767)
                {
                    binaryWriter.Write(-4);
                    binaryWriter.Write(s.Length);
                    foreach (char c in s)
                    {
                        binaryWriter.Write((short)c);
                    }
                    return;
                }
                binaryWriter.Write((short)s.Length);
                foreach (char c2 in s)
                {
                    binaryWriter.Write((short)c2);
                }
            }
            public void WriteDouble(double d)
            {
                binaryWriter.Write(d);
            }
            public void WriteFloat(float d)
            {
                binaryWriter.Write(d);
            }
            public void WriteUint(uint n)
            {
                binaryWriter.Write(n);
            }

            void WriteIntChunk(int id, int value)
            {
                binaryWriter.Write(id);
                binaryWriter.Write(4);
                binaryWriter.Write(value);
            }
            public void WriteStringChunk(int id, string s)
            {
                binaryWriter.Write(id);
                if (s == null)
                {
                    binaryWriter.Write(0);
                    return;
                }
                binaryWriter.Write(s.Length << 1);
                foreach (char c in s)
                {
                    binaryWriter.Write((short)c);
                }
            }
            public void WriteFloatChunk(int id, float value)
            {
                binaryWriter.Write(id);
                binaryWriter.Write(4);
                binaryWriter.Write(value);
            }

            void WriteverFingerprint()
            {
                int verMajor = 0;
                int verMinor = 0;
                int build = 0;
                var res = pma.GetSoftwareInfo(out verMajor, out verMinor, out build);
                StartChunk(26, 12);
                binaryWriter.Write(verMajor);
                binaryWriter.Write(verMinor);
                binaryWriter.Write(build);
                EndChunk();
            }
            void WriteDocument(DocumentSettings dc)
            {
                StartChunk(1100, 68);
                WriteInt(0);
                WriteInt(0);
                WriteInt(0);
                WriteInt((int)dc.CaptureSettings.postCaptureAOIBehaviour);
                dynamic dcp = ExposedObject.Exposed.From(dc);
                bool onCaptureFlipVertical = dcp.onCaptureFlipVertical;
                bool onCaptureFlipHorizontal = dcp.onCaptureFlipHorizontal;
                int onCaptureRotate = dcp.onCaptureRotate;
                int coordinateSpace = (int)dcp.coordinateSpace;//enCoordinateType
                WriteBool(onCaptureFlipVertical);
                WriteBool(onCaptureFlipHorizontal);
                WriteBool(dc.showAoiHighlightColumn);
                WriteBool(dc.showAoiRefinementColumn);
                WriteInt(dc.aoiNameColumnWidth);
                WriteInt(dc.aoiHighlightColumnWidth);
                WriteInt(dc.aoiRefinementColumnWidth);
                WriteInt(onCaptureRotate);
                WriteInt((int)coordinateSpace);
                WriteBool(false);
                WriteBool(dc.showAoiHighlightResultColumn);
                WriteInt(dc.aoiHighlightResultColumnWidth);
                WriteBool(dc.showEvaluationPassFailColumn);
                WriteBool(false);
                WriteInt(0);
                WriteInt(0);
                WriteBool(dc.surfacePlotUseSelecton);
                WriteBool(dc.surfacePlotSelectionBB);
                WriteBool(true);
                EndChunk();
                StartChunk(1125, 34);
                WriteDouble((dc._sphericalLensPolarInfo != null) ? dc._sphericalLensPolarInfo.DegreesPerPx : double.NaN);
                WriteDouble((dc._sphericalLensPolarInfo != null) ? dc._sphericalLensPolarInfo.PolarRadiusInDegrees : 0.0);
                DocumentImagerSettings _primaryImager = dcp._primaryImager;
                WriteInt(_primaryImager.lensId);
                WriteInt(_primaryImager.FovSmallInfo.FovIdentifierCode);
                WriteString(_primaryImager.imagerIdentity.ToString());
                WriteDouble(_primaryImager.FovSmallInfo.FocalLengthOrFocusDistance_cm);
                EndChunk();
                System.Collections.Generic.List<DocumentImagerSettings> _imagerSettings = dcp._imagerSettings;
                foreach (DocumentImagerSettings documentImagerSettings in _imagerSettings)
                {
                    if (documentImagerSettings != _primaryImager)
                    {
                        StartChunk(1132, 0);
                        WriteString(documentImagerSettings.imagerIdentity.ToString());
                        WriteInt(documentImagerSettings.lensId);
                        WriteInt(documentImagerSettings.FovSmallInfo.FovIdentifierCode);
                        WriteDouble(documentImagerSettings.FovSmallInfo.FocalLengthOrFocusDistance_cm);
                        EndChunk();
                    }
                }
                dynamic pixelScale = ExposedObject.Exposed.From(dcp.pixelScale);
                string Suffix = pixelScale.Suffix;
                StartChunk(1111, 4 + (Suffix.Length + 1) * 2);
                //dc.pixelScale.WriteChunk(file);
                WriteFloat((float)pixelScale.PixelSize.RwuPerPixels); //PixelSize
                WriteString(pixelScale.PixelSize.RwuName);
                WriteDouble(pixelScale.PixelSize.NumberOfPixels);
                WriteDouble(pixelScale.PixelSize.NumberOfRwunits);
                EndChunk();

                StartChunk(1124, 21);
                int deltaXinPixels = dcp.deltaXinPixels;
                int deltaYinPixels = dcp.deltaYinPixels;
                WriteInt(deltaXinPixels);
                WriteInt(deltaYinPixels);
                WriteFloat((float)Distance.ConvertDistanceUnits(dcp._sourceZdistanceInRwu, dcp._unitsForSourceZ, dcp.pixelScale.ActiveDistanceUnits, dcp.pixelScale.MillimetersPerPixel()));
                WriteBool(dc.realWorldPov);
                WriteFloat((float)Distance.ConvertDistanceUnits(dcp._deltaXinRwu, dcp._unitsForDeltaXY, dcp.pixelScale.ActiveDistanceUnits, dc.PixelScale.MillimetersPerPixel()));
                WriteFloat((float)Distance.ConvertDistanceUnits(dcp._deltaYinRwu, dcp._unitsForDeltaXY, dc.PixelScale.ActiveDistanceUnits, dc.PixelScale.MillimetersPerPixel()));
                WriteInt((int)dcp.povDirection);
                WriteInt((int)dcp._unitsForDeltaXY);
                WriteDouble(dcp._deltaXinRwu);
                WriteDouble(dcp._deltaYinRwu);
                WriteInt((int)dcp._unitsForSourceZ);
                WriteDouble(dcp._sourceZdistanceInRwu);
                WriteBool(dc.UsingFactoryDxDySrcZ);
                EndChunk();
                WriteFloatChunk(1117, dc.ReferenceAngle);
                StartChunk(1119, 3);
                WriteString(dc.sortAoiTableByAmf);
                WriteBool(dc.sortAoiTableRvsAmf);
                EndChunk();
                WriteStringChunk(1120, dc.CaptureSettings.onCaptureFilterOption);
                StartChunk(645, 1);
                WriteBool(dc.CaptureSettings.turnOffMonitorDuringCapture);
                WriteInt(dc.CaptureSettings.monitorTurnOffDelayInMs);
                EndChunk();
                StartChunk(662, 8);
                WriteInt(dc.CaptureSettings.captureDefaultLongestExposureFactor);
                WriteInt(dc.CaptureSettings.captureDefaultShortestExposureFactor);
                EndChunk();
                StartChunk(651, 2);
                WriteBool(dc.CaptureSettings.onCaptureSelectOverexposed);
                WriteBool(dc.CaptureSettings.onCaptureSelectUnderexposed);
                EndChunk();
                StartChunk(1121, 60);
                WriteInt((int)dc.overlayType);
                WriteInt((int)dc.overlayLineStyle);
                WriteDouble(dc.overlayGridXSpacing);
                WriteDouble(dc.overlayGridYSpacing);
                WriteDouble(dc.overlayPhiSpacing);
                WriteDouble(dc.overlayThetaSpacing);
                WriteDouble(dc.overlayThetaHSpacing);
                WriteDouble(dc.overlayThetaVSpacing);
                WriteUint(dc.overlayColor.Bgra);
                EndChunk();
                StartChunk(660, 4);
                WriteInt((int)dc.CctMethod);
                EndChunk();
                StartChunk(666, 4);
                WriteInt((int)dc.CaptureSettings.UnderexposedAction);
                EndChunk();
                StartChunk(669, 3);
                WriteBool(dc.showCaptureCaptureColumn);
                WriteBool(dc.showCaptureMetaColumns);
                WriteBool(dc.showCaptureMetaProperty);
                EndChunk();
                StartChunk(670, 4 + (dc.passText.Length + dc.failText.Length) * 2);
                WriteString(dc.passText);
                WriteString(dc.failText);
                EndChunk();
                if (dc.ElcLensConfiguration != null)
                {
                    StartChunk(687, 0);
                    //dc.ElcLensConfiguration.WriteChunks(file);
                    var ec = dc.ElcLensConfiguration;
                    WriteStringChunk(684, ec._name);
                    if (ec._deviceId != null)
                    {
                        StartChunk(686, 0);
                        WriteString(ec._deviceId.ToString());
                        WriteInt(ec._lensId);
                        WriteInt(ec._fov.FovIdentifierCode);
                        WriteDouble(ec._fov.FocalLengthOrFocusDistance_cm);
                        EndChunk();
                    }
                    dynamic ecp = ExposedObject.Exposed.From(ec);
                    Dictionary<SpectralFilterName, LensFilterConfig> _filterConfigs = ecp._filterConfigs;
                    foreach (KeyValuePair<SpectralFilterName, LensFilterConfig> keyValuePair in ecp._filterConfigs)
                    {
                        LensFilterConfig value = keyValuePair.Value;
                        StartChunk(685, 0);
                        WriteString(value._spectralFilterName.ToString());
                        WriteDouble(value.ElcFocusNumber);
                        EndChunk();
                    }
                    EndChunk();
                }
                if (!string.IsNullOrEmpty(dc.InstConfigurationName))
                {
                    WriteStringChunk(675, dc.InstConfigurationName);
                }
            }
            void WritepmmMetaInfoChunkIO(PmmFile _pmm)
            {
                string s = _pmm.userNotes;
                StartChunk(1600, 0);
                WriteString(s);
                EndChunk();
                List<VariableObject> list = _pmm.pmmAtAtVariableList;
                for (int i = 0; i < list.Count; i++)
                {
                    VariableObject variableObject = list[i];
                    if (variableObject.ObjectName.Length > 0)
                    {
                        StartChunk(1601, 0);
                        WriteString(variableObject.ObjectName);
                        WriteString(variableObject.ScriptVariable.GetValue().Text);
                        EndChunk();
                    }
                }
            }
        }
        public class ChunkStackNode
        {
            public ChunkStackNode(long pos, int size)
            {
                this.chunkSizePosition = pos;
                this.currentChunkSize = size;
            }

            public long chunkSizePosition;

            public int currentChunkSize;
        }
    }
    class PhotometricaImage : CogImage8Grey
    {
        public float[] array;
        public PhotometricaImage(float[] ary, int wid, int height) : base(wid, height)
        {
            array = ary;
            if (ary == null)
                throw new Exception("ary == null");
            if (wid <= 0)
                throw new Exception("wid <= 0");
            if (height <= 0)
                throw new Exception("height <= 0");
            if (wid * height > ary.Length)
                throw new Exception("wid * height > ary.Length");
            var memory = Get8GreyPixelMemory(CogImageDataModeConstants.Write, 0, 0, wid, height);
            byte[] mem = new byte[wid * height];
            for (int i = 0; i < mem.Length; i++)
            {
                mem[i] = (byte)array[i];
            }
            for (int i = 0; i < memory.Height; i++)
                Marshal.Copy(mem, i * wid, memory.Scan0 + i * memory.Stride, wid);
            memory.Dispose();
        }
    }
    class PhotometricaColorImage : CogImage24PlanarColor
    {
        public float[] array;
        public PhotometricaColorImage(float[] ary, int wid, int height) : base(wid, height)
        {
            array = ary;
            if (ary == null)
                throw new Exception("ary == null");
            if (wid <= 0)
                throw new Exception("wid <= 0");
            if (height <= 0)
                throw new Exception("height <= 0");
            if (wid * height * 3 > ary.Length)
                throw new Exception("wid * height * 3 > ary.Length");
            ICogImage8PixelMemory m1, m2, m3;
            Get24PlanarColorPixelMemory(CogImageDataModeConstants.Write, 0, 0, wid, height, out m1, out m2, out m3);
            byte[] mem = new byte[wid * height * 3];
            for (int i = 0; i < mem.Length; i++)
            {
                mem[i] = (byte)array[i];
            }
            for (int i = 0; i < m1.Height; i++)
            {
                Marshal.Copy(mem, i * 3 * wid, m1.Scan0 + i * m1.Stride, wid);
                Marshal.Copy(mem, i * 3 * wid + wid, m2.Scan0 + i * m2.Stride, wid);
                Marshal.Copy(mem, i * 3 * wid + wid * 2, m3.Scan0 + i * m3.Stride, wid);
            }
            m1.Dispose();
            m2.Dispose();
            m3.Dispose();
        }
    }
    public static class ExtendPMM
    {
        public static BinaryFile.BinaryFileError SaveBinaryStream(this BinaryFile bf, MemoryStream ms, IBinaryFileWriter writer)
        {
            bf.errorCode = BinaryFile.BinaryFileError.Success;
            dynamic bfp = ExposedObject.Exposed.From(bf);
            try
            {
                //bfp.fileWriter = ms;
                bfp.binaryWriter = new BinaryWriter(ms);
                bfp.chunkStack = new System.Collections.Stack(16);
                if (!writer.WriteChunks(bf))
                {
                    bf.errorCode = BinaryFile.BinaryFileError.ParseError;
                }
            }
            catch (Exception ex)
            {
                bf.errorCode = bfp.GetErrorCode(ex);
                bf.errorDetails.Append(ex.Message);
            }
            return bf.errorCode;
        }
    }

    internal class WPColorToolsEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            if (context != null && context.Instance != null)
            {
                return UITypeEditorEditStyle.Modal;
            }
            return base.GetEditStyle(context);
        }
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            System.Windows.Forms.Design.IWindowsFormsEditorService editorService = null;
            if (context != null && context.Instance != null && provider != null)
            {
                editorService = (System.Windows.Forms.Design.IWindowsFormsEditorService)provider.GetService(typeof(System.Windows.Forms.Design.IWindowsFormsEditorService));
                if (editorService != null)
                {
                    if (value is float[])
                    {
                        var pts = (float[])value;
                        ColorRegion cr = new ColorRegion();
                        if (pts.Length == 6 && pts[0] == 1)
                        {
                            cr.SetEllipseData(pts[1], pts[2], pts[3], pts[4], pts[5]);
                        }
                        else if (pts.Length > 2 && pts[0] == 0)
                        {
                            var pt = new float[pts.Length + 1];
                            Array.Copy(pts, 1, pt, 0, pts.Length - 1);
                            Array.Copy(pts, 1, pt, pts.Length - 1, 2);
                            cr.SetPolygonData(2, pt);
                        }
                        var c = new ColorSpaceRegionForm(PhotometricaHdl.pmc, cr, true);
                        if (c.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            if (cr.shape == ColorRegion.ColorRegionShape.Polygon)
                            {
                                var rr = cr.GetPolygon();
                                var re = new float[rr.Count * 2 + 1];
                                re[0] = 0;
                                for (int i = 0; i < rr.Count; i++)
                                {
                                    re[i * 2 + 1] = rr[i].X;
                                    re[i * 2 + 2] = rr[i].Y;
                                }
                                return re;
                            }
                            else if (cr.shape == ColorRegion.ColorRegionShape.Ellipse)
                            {
                                var re = new float[6];
                                re[0] = 1;
                                re[1] = cr.EllipseCenterX;
                                re[2] = cr.EllipseCenterY;
                                re[3] = cr.EllipseRadiusX;
                                re[4] = cr.EllipseRadiusY;
                                re[5] = cr.EllipseRotation;
                                return re;
                            }
                        }
                    }
                }
            }
            return value;
        }
    }
}
