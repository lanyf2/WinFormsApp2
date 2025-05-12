using Cognex.VisionPro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace vpc
{
    internal partial class ParmsInputDialogue : Form
    {
        static internal string SelectedString;
        internal bool changed = false;
        internal ParmsInputDialogue(string title, object obj)
        {
            InitializeComponent();
            //this.Icon = Tofine.CamExplorer.Properties.Resources.camera;
            this.Text = title;
            splitContainer1.Panel2Collapsed = true;
            //propertyGrid1.SelectedObject = obj.Clone();
            propertyGrid1.SelectedObject = obj;
            if (GlobalMemo != null)
                pasteToolStripMenuItem.Enabled = true;
        }

        internal object SelectedObject
        {
            get { return propertyGrid1.SelectedObject; }
        }

        private void ParmsInputDialogue_Load(object sender, EventArgs e)
        {
        }
        private void ParmsInputDialogue_Shown(object sender, EventArgs e)
        {
            MoveSplitterTo(propertyGrid1);
            Application.DoEvents();
            Thread.Sleep(1);
            this.ShowIcon = false;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                Type tp = propertyGrid1.SelectedObject.GetType();
                propertyGrid1.SelectedObject = Activator.CreateInstance(tp);
            }
            catch (System.Exception ex)
            {
                Program.ErrHdl(ex);
            }
        }

        internal static string GetLabel(GridItem gi)
        {
            if (gi == null)
                return null;
            Stack<GridItem> stk = new Stack<GridItem>();
            while (gi.Parent != null)
            {
                stk.Push(gi);
                gi = gi.Parent;
            }
            StringBuilder sb = new StringBuilder();
            while (stk.Count > 0)
            {
                gi = stk.Pop();
                sb.Append(gi.Label).Append('_');
            }
            if (sb.Length > 0)
                return sb.ToString(0, sb.Length - 1);
            else
                return null;
        }
        private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (SelectedString != null && e.ChangedItem.Value.Equals(e.OldValue) == false)
                Program.Loginfo("{3}-{0}：{1} -> {2}".FormatWith(GetLabel(e.ChangedItem), e.OldValue, e.ChangedItem.Value, SelectedString));
            changed = true;
            propertyGrid1.Refresh();
        }

        internal static void MoveSplitterTo(System.Windows.Forms.PropertyGrid grid, int add = 50)
        {
            Type tp = typeof(PropertyGrid);
            FieldInfo fi = tp.GetField("gridView", BindingFlags.Instance | BindingFlags.NonPublic);
            object ob = fi.GetValue(grid);
            tp = ob.GetType();
            MethodInfo mi = tp.GetMethod("MoveSplitterTo", BindingFlags.Instance | BindingFlags.NonPublic);
            FieldInfo f2 = tp.GetField("allGridEntries", BindingFlags.Instance | BindingFlags.NonPublic);
            GridItemCollection grids = f2.GetValue(ob) as GridItemCollection;

            if (grids == null)
                return;
            float len = 5;
            System.Drawing.Graphics graphics = grid.CreateGraphics();
            foreach (GridItem item in grids)
            {
                SizeF sizeF = graphics.MeasureString(item.Label, grid.Font);

                //if (item.Label.Length > len)
                //len = item.Label.Length;
                if (sizeF.Width > len)
                    len = sizeF.Width;
            }
            if (len > grid.Width / 2)
                len = grid.Width / 2;
            mi.Invoke(ob, new object[] { (int)(len) + 50 });
            //mi.Invoke(ob, new object[] { (int)(len * propertyGrid1.Font.Size * rate) });
        }

        internal static GridItem selectedItemGlobal;
        GridItem selectedItem;
        private void propertyGrid1_SelectedGridItemChanged(object sender, SelectedGridItemChangedEventArgs e)
        {
            if (e.NewSelection != null)
                selectedItemGlobal = e.NewSelection;
            selectedItem = e.NewSelection;
            if (selectedItem != null && (e.NewSelection.Value is Cognex.VisionPro.Block1Settings == false))
                selectedItem = null;

            if (selectedItem != null)
                propertyGrid1.ContextMenuStrip = contextMenuStrip1;
            else
                propertyGrid1.ContextMenuStrip = null;

            cogDisplay1.StaticGraphics.Clear();
            if (e.NewSelection.Value != null)
            {
                if (e.NewSelection.Value.GetType().IsValueType || e.NewSelection.Value is string)
                    Form1.ShowKeyBoard();
                else
                    Form1.HideKeyBoard();
                var dispimg = e.NewSelection.Value as Cognex.VisionPro.ICogImage;
                if (dispimg == null && e.NewSelection.PropertyDescriptor.Converter is MyTrainCogToolBlockConverter)
                {
                    var pm = e.NewSelection.Value as Cognex.VisionPro.ToolBlock.CogToolBlock;

                    if (pm.Inputs.Contains("TrainModel") && pm.Inputs.Contains("Parm"))
                    {
                        object[] parm = pm.Inputs["Parm"].Value as object[];
                        if (parm != null && parm.Length >= 3)
                        {
                            Cognex.VisionPro.PMAlign.CogPMAlignPattern pattern = parm[2] as Cognex.VisionPro.PMAlign.CogPMAlignPattern;
                            if (pattern != null && pattern.Trained)
                            {
                                dispimg = pattern.GetTrainedPatternImage();
                                CogImage8Grey trainedPatternImageMask = pattern.GetTrainedPatternImageMask();
                                if (trainedPatternImageMask != null)
                                {
                                    CogMaskGraphic graphic = CreateMaskGraphic(trainedPatternImageMask);
                                    this.cogDisplay1.StaticGraphics.Add(graphic, "Mask");
                                }
                                cogDisplay1.StaticGraphics.AddList(pattern.CreateGraphicsFine(Cognex.VisionPro.CogColorConstants.Green), null);
                                cogDisplay1.StaticGraphics.AddList(pattern.CreateGraphicsCoarse(Cognex.VisionPro.CogColorConstants.Yellow), null);
                            }
                        }
                    }
                    if (dispimg != null)
                    {
                        cogDisplay1.Image = dispimg;
                        splitContainer1.Panel2Collapsed = false;
                        this.Size = new Size(this.Size.Width + 300, this.Size.Height);
                    }
                }
                if (dispimg == null && splitContainer1.Panel2Collapsed == false)
                {
                    splitContainer1.Panel2Collapsed = true;
                    int w = this.Size.Width - 300;
                    if (w < 300)
                        w = 300;
                    this.Size = new Size(w, this.Size.Height);
                }
                if (string.IsNullOrEmpty(e.NewSelection.PropertyDescriptor.Description))
                    propertyGrid1.HelpVisible = false;
                else
                    propertyGrid1.HelpVisible = true;
            }
        }

        private void ParmsInputDialogue_FormClosing(object sender, FormClosingEventArgs e)
        {
            Form1.HideKeyBoard();
            //if (changed && DialogResult != DialogResult.OK)
            //    MessageBox.Show("参数已改变但未保存，重启程序可还原最后一次保存时的参数");
        }
        internal void RefreshHdl(object sender, EventArgs e)
        {
            if (this.WindowState != FormWindowState.Maximized)
            {
                if (this.InvokeRequired)
                    this.BeginInvoke(new Action(propertyGrid1.Refresh));
                else
                    propertyGrid1.Refresh();
            }
        }

        internal static Block1Settings GlobalMemo;
        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedItem != null)
            {
                var val = selectedItem.Value as Cognex.VisionPro.Block1Settings;
                if (val != null)
                {
                    val.Remove();
                    if (SelectedString != null)
                        Program.Loginfo("删除工具：{0}-{1}".FormatWith(SelectedString, GetLabel(selectedItem)));
                    changed = true;
                    propertyGrid1.Refresh();
                }
            }
        }
        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedItem != null)
            {
                var val = selectedItem.Value as Cognex.VisionPro.Block1Settings;
                if (val != null)
                {
                    val.Insert();
                    changed = true;
                    propertyGrid1.Refresh();
                }
            }
        }
        private void moveupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedItem != null)
            {
                var val = selectedItem.Value as Cognex.VisionPro.Block1Settings;
                if (val != null)
                {
                    val.MoveUp();
                    changed = true;
                    propertyGrid1.Refresh();
                }
            }
        }
        private void movedownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedItem != null)
            {
                var val = selectedItem.Value as Cognex.VisionPro.Block1Settings;
                if (val != null)
                {
                    val.MoveDown();
                    changed = true;
                    propertyGrid1.Refresh();
                }
            }
        }
        private void appendToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedItem != null)
            {
                var val = selectedItem.Value as Cognex.VisionPro.Block1Settings;
                if (val != null)
                {
                    val.Append();
                    changed = true;
                    propertyGrid1.Refresh();
                }
            }
        }
        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedItem != null)
            {
                GlobalMemo = selectedItem.Value as Cognex.VisionPro.Block1Settings;
                if (GlobalMemo != null)
                    pasteToolStripMenuItem.Enabled = true;
            }
        }
        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (GlobalMemo != null && selectedItem != null)
            {
                var val = selectedItem.Value as Cognex.VisionPro.Block1Settings;
                if (val != null)
                {
                    val.Append(GlobalMemo);
                    changed = true;
                    propertyGrid1.Refresh();
                }
            }
        }

        internal static CogMaskGraphic CreateMaskGraphic(CogImage8Grey mask)
        {
            CogMaskGraphic cogMaskGraphic = new CogMaskGraphic();
            for (short num = 0; num < 256; num += 1)
            {
                CogColorConstants value;
                CogMaskGraphicTransparencyConstants value2;
                if (num < 64)
                {
                    value = CogColorConstants.DarkRed;
                    value2 = CogMaskGraphicTransparencyConstants.Half;
                }
                else
                {
                    if (num < 128)
                    {
                        value = CogColorConstants.Yellow;
                        value2 = CogMaskGraphicTransparencyConstants.Half;
                    }
                    else
                    {
                        if (num < 192)
                        {
                            value = CogColorConstants.Red;
                            value2 = CogMaskGraphicTransparencyConstants.None;
                        }
                        else
                        {
                            value = CogColorConstants.Yellow;
                            value2 = CogMaskGraphicTransparencyConstants.Full;
                        }
                    }
                }
                cogMaskGraphic.SetColorMap((byte)num, value);
                cogMaskGraphic.SetTransparencyMap((byte)num, value2);
            }
            cogMaskGraphic.Image = mask;
            cogMaskGraphic.Color = CogColorConstants.None;
            return cogMaskGraphic;
        }
    }
    [AttributeUsage(AttributeTargets.All)]
    public class TooltipAttribute : Attribute
    {
        string desc;
        public TooltipAttribute(string description)
        {
            desc = description;
        }

        public string TooltipText { get { return desc; } }
    }
}
