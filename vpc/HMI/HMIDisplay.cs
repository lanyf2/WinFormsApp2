using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace vpc
{
    internal partial class HMIDisplay : UserControl
    {
        HMISettings sts = new HMISettings();

        internal HMISettings Sts
        {
            get { return sts; }
            set
            {
                if (value != null)
                {
                    sts = value;
                    if (cogDisplay1 != null)
                    {
                        cogDisplay1.Image = sts.img;
                        Cognex.VisionPro.CogGraphicLabel cg = new Cognex.VisionPro.CogGraphicLabel();
                        cg.BackgroundColor = Cognex.VisionPro.CogColorConstants.LightGrey;
                        cg.X = 200;
                        cg.Y = 200;
                        cg.Color = Cognex.VisionPro.CogColorConstants.Black;
                        cogDisplay1.InteractiveGraphics.Add(cg, null, false);
                    }
                }
            }
        }

        public HMIDisplay()
        {
            InitializeComponent();
            cogDisplay1.MouseWheelMode = Cognex.VisionPro.Display.CogDisplayMouseWheelModeConstants.None;
            cogDisplay1.MouseMode = Cognex.VisionPro.Display.CogDisplayMouseModeConstants.UserDefined;
            cogDisplay1.ContextMenuStrip = null;
        }

        private void HMIDisplay_Load(object sender, EventArgs e)
        {

        }

        internal void UpdateData(Modbus.Device.ModbusMaster modbus)
        {
            if (modbus != null)
            {
                try
                {

                }
                catch
                {
                }
            }
        }
    }
    [Serializable]
    internal class HMISettings
    {
        public Cognex.VisionPro.ICogImage img { get; set; }
        public byte ToolIndex { get; set; }
        public int ModbusAddr { get; set; }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public List<Uinfo> InfoArray { get; set; }

        public Dictionary<int, ushort> DataStore;
        public List<KeyValuePair<int, ushort>> DataAddrStore;
        public void InitAddrStore()
        {
            if (InfoArray != null && InfoArray.Count > 0)
            {
                SortedSet<int> tmpss = new SortedSet<int>();
                for (int i = 0; i < InfoArray.Count; i++)
                {
                    if (false == tmpss.Contains(InfoArray[i].Addr))
                        tmpss.Add(InfoArray[i].Addr);
                }
                while (tmpss.Count > 0)
                {
                    int len = 1;
                    int startAddr = tmpss.GetEnumerator().Current;
                    tmpss.Remove(startAddr);
                    while (tmpss.Remove(startAddr - 1))
                    {
                        len++;
                        startAddr--;
                    }
                    while (tmpss.Remove(startAddr + len))
                    {
                        len++;
                    }
                    if (startAddr < 0)
                        DataAddrStore.Add(new KeyValuePair<int, ushort>(startAddr + len, (ushort)len));
                    else
                        DataAddrStore.Add(new KeyValuePair<int, ushort>(startAddr, (ushort)len));
                }
            }
        }
    }
    public class Uinfo
    {
        public int GraphicType { get; set; }
        public int Addr { get; set; }
        public ushort TextAddr { get; set; }
        public double Rate { get; set; }
        public string StringFormat { get; set; }
    }
}
