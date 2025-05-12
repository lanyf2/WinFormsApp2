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
    public partial class CogToolEditor : Form
    {
        Cognex.VisionPro.CogToolEditControlBaseV2 Editor;
        public CogToolEditor(Cognex.VisionPro.CogToolEditControlBaseV2 editor)
        {
            InitializeComponent();
            if (editor != null)
            {
                Editor = editor;
                Controls.Add(editor);
                editor.Dock = DockStyle.Fill;
            }
        }

        private void CogToolEditor_Load(object sender, EventArgs e)
        {

        }
    }
}
