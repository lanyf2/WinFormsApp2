using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.IO;
using Cognex.VisionPro;
using Cognex.VisionPro.ToolBlock;

namespace vpc
{
    public static class ExcelHdl
    {
        internal static Block1Collection b1c;
        public static void SaveMtds(string file, string FileName = "test.xlsx")
        {
            if (File.Exists(file) && JobManager.MtdBlocks != null && JobManager.MtdBlocks[0] != null)
            {
                CogToolBlock ct = CogSerializer.LoadObjectFromFile(file) as CogToolBlock;
                if (b1c == null)
                    b1c = new Block1Collection(JobManager.MtdBlocks[0] as Block1Collection, 0);
                if (ct != null)
                {
                    IWorkbook workbook = new XSSFWorkbook();
                    for (int i = 0; i < ct.Tools.Count; i++)
                    {
                        CogToolBlock bk = ct.Tools[i] as CogToolBlock;
                        if (bk != null)
                        {
                            ISheet sheet = workbook.CreateSheet("画面" + (i + 1));
                            var parms = bk.Inputs["Parms"].Value as List<object[]>;
                            if (parms != null)
                            {
                                IRow row = sheet.CreateRow(0);
                                ICell cell;
                                b1c.internalblock.Inputs["Parms"].Value = parms;
                                b1c.list.Clear();
                                b1c.SyncParms();

                                cell = row.CreateCell(1);
                                cell.SetCellValue("工具数");
                                cell = row.CreateCell(2);
                                cell.SetCellValue(b1c.Count);

                                cell = row.CreateCell(3);
                                cell.SetCellValue("转换模式");
                                cell = row.CreateCell(4);
                                cell.SetCellValue(b1c.runMode.ToString());

                                cell = row.CreateCell(5);
                                cell.SetCellValue("曝光时间：");
                                cell = row.CreateCell(6);
                                cell.SetCellValue(b1c.ExposureTime.ToString());
                                for (int j = 0; j < b1c.Count; j++)
                                {
                                    Type tp = b1c.list[j].GetType();
                                    var pps = tp.GetProperties();
                                    row = sheet.CreateRow(j + 1);
                                    int index = 1;
                                    cell = row.CreateCell(0);
                                    cell.SetCellValue("工具" + (j + 1));
                                    for (int k = 0; k < pps.Length; k++)
                                    {
                                        if (pps[k].Name != "Region" && pps[k].Name != "RegionType" && pps[k].Name != "TrainModel"
                                            && pps[k].Name != "TrainColor" && pps[k].Name != "HistMeanDifThres" && pps[k].Name != "HardThreshold"
                                             && pps[k].Name != "HistMeanDifThres" && pps[k].Name != "HardThreshold")
                                        {
                                            cell = row.CreateCell(index);
                                            string cname = null;
                                            var enu = pps[k].GetCustomAttributesData();
                                            for (int kk = 0; kk < enu.Count; kk++)
                                            {
                                                if (enu[kk].AttributeType.Name == "DisplayNameAttribute")
                                                {
                                                    cname = enu[kk].ConstructorArguments[0].Value as string;
                                                    break;
                                                }
                                            }
                                            if (cname != null)
                                                cell.SetCellValue(cname);
                                            else
                                                cell.SetCellValue(pps[k].Name);
                                            cell = row.CreateCell(index + 1);
                                            if (pps[k].PropertyType == typeof(System.Drawing.Color))
                                            {
                                                System.Drawing.Color cr = (System.Drawing.Color)pps[k].GetValue(b1c.list[j]);
                                                cell.SetCellValue(string.Format("R={0},G={1},B={2}", cr.R, cr.G, cr.B));
                                            }
                                            else
                                                cell.SetCellValue(pps[k].GetValue(b1c.list[j]).ToString());
                                            index += 2;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    string ff = Path.GetFileNameWithoutExtension(file) + ".xlsx";
                    FileStream sw = File.Create(ff);
                    workbook.Write(sw);
                    sw.Close();
                    Program.MsgBox("保存成功：" + ff);
                }
            }
        }
        public static void SaveMtds(BlockBase[] blocks, string FileName = "test.xlsx")
        {
            if (blocks != null)
            {
                IWorkbook workbook = new XSSFWorkbook();
                for (int i = 0; i < blocks.Length; i++)
                {
                    Block1Collection b1c = blocks[i] as Block1Collection;
                    if (b1c != null)
                    {
                        ISheet sheet = workbook.CreateSheet("步骤" + (i + 1));
                        IRow row = sheet.CreateRow(0);
                        ICell cell;

                        cell = row.CreateCell(1);
                        cell.SetCellValue("工具数");
                        cell = row.CreateCell(2);
                        cell.SetCellValue(b1c.Count);

                        cell = row.CreateCell(3);
                        cell.SetCellValue("转换模式");
                        cell = row.CreateCell(4);
                        cell.SetCellValue(b1c.runMode.ToString());

                        cell = row.CreateCell(5);
                        cell.SetCellValue("曝光时间：");
                        cell = row.CreateCell(6);
                        cell.SetCellValue(b1c.ExposureTime);
                        if (b1c.list == null || b1c.list.Count != b1c.Count)
                            b1c.SyncParms();
                        if (b1c.list != null)
                            for (int j = 0; j < b1c.list.Count; j++)
                            {
                                Type tp = b1c.list[j].GetType();
                                var pps = tp.GetProperties();
                                row = sheet.CreateRow(j + 1);
                                int index = 1;
                                cell = row.CreateCell(0);
                                cell.SetCellValue("工具" + (j + 1));
                                for (int k = 0; k < pps.Length; k++)
                                {
                                    if (pps[k].Name != "Region" && pps[k].Name != "RegionType")
                                    {
                                        cell = row.CreateCell(index);
                                        if (pps[k].Name == "Mtd")
                                            cell.SetCellValue("检测模式");
                                        else
                                            cell.SetCellValue(pps[k].Name);
                                        cell = row.CreateCell(index + 1);
                                        if (pps[k].DeclaringType == typeof(System.Drawing.Color))
                                        {
                                            System.Drawing.Color cr = (System.Drawing.Color)pps[k].GetValue(b1c.list[j]);
                                            cell.SetCellValue(string.Format("R={0},G={1},B={2}", cr.R, cr.G, cr.B));
                                        }
                                        else
                                            cell.SetCellValue(pps[k].GetValue(b1c.list[j]).ToString());
                                        index += 2;
                                    }
                                }
                            }
                    }
                }
                FileStream sw = File.Create(FileName);
                workbook.Write(sw);
                sw.Close();
            }
        }
        public static void SaveExcel(System.Windows.Forms.DataGridView dataGridView1, string FileName, string SheetName = "sheet1")
        {
            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet(SheetName);
            int rowLimit = dataGridView1.Rows.Count;
            List<int> colslist = new List<int>();
            for (int i = 0; i < dataGridView1.ColumnCount; i++)
            {
                if (dataGridView1.Columns[i].Visible)
                    colslist.Add(i);
            }
            int cols = colslist.Count;
            IRow row = sheet.CreateRow(0);
            //(NPOI.HSSF.UserModel.HSSFCellStyle)(NPOI.XSSF.UserModel.XSSFDataFormat)
            var style = workbook.CreateCellStyle();
            var format = workbook.CreateDataFormat();
            style.DataFormat = format.GetFormat("yyyy年M月d日 HH时mm分ss秒");
            sheet.SetColumnWidth(0, 28 * 256);
            //sheet.SetColumnWidth(1, 90 * 256);
            //sheet.SetColumnWidth(2, 10 * 256);
            for (int i = 0; i < cols; i++)
            {
                ICell cell = row.CreateCell(i);
                cell.SetCellValue(dataGridView1.Columns[colslist[i]].HeaderText);
            }
            for (int i = 0; i < rowLimit; i++)
            {
                row = sheet.CreateRow(i + 1);
                ICell cell;
                if (dataGridView1[0, i].Value is DateTime)
                {
                    cell = row.CreateCell(0);
                    cell.SetCellValue((DateTime)dataGridView1[0, i].Value);
                    cell.CellStyle = style;
                }
                for (int j = 1; j < cols; j++)
                {
                    if (dataGridView1[colslist[j], i].Value is string)
                    {
                        cell = row.CreateCell(j);
                        cell.SetCellValue((string)dataGridView1[colslist[j], i].Value);
                    }
                }
                //sheet.SetActiveCell(i, 0);
            }
            for (int i = 1; i < cols; i++)
            {
                sheet.AutoSizeColumn(i);
            }
            FileStream sw = File.Create(FileName);
            workbook.Write(sw);
            sw.Close();
        }
    }
}
