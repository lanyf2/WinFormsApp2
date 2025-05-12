namespace vpc
{
    partial class FormDispImg
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDispImg));
            this.cogDisplay1 = new Cognex.VisionPro.Display.CogDisplay();
            ((System.ComponentModel.ISupportInitialize)(this.cogDisplay1)).BeginInit();
            this.SuspendLayout();
            // 
            // cogDisplay1
            // 
            this.cogDisplay1.ColorMapLowerClipColor = System.Drawing.Color.Black;
            this.cogDisplay1.ColorMapLowerRoiLimit = 0D;
            this.cogDisplay1.ColorMapPredefined = Cognex.VisionPro.Display.CogDisplayColorMapPredefinedConstants.None;
            this.cogDisplay1.ColorMapUpperClipColor = System.Drawing.Color.Black;
            this.cogDisplay1.ColorMapUpperRoiLimit = 1D;
            this.cogDisplay1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cogDisplay1.Location = new System.Drawing.Point(0, 0);
            this.cogDisplay1.Margin = new System.Windows.Forms.Padding(0);
            this.cogDisplay1.MouseWheelMode = Cognex.VisionPro.Display.CogDisplayMouseWheelModeConstants.Zoom1;
            this.cogDisplay1.MouseWheelSensitivity = 1D;
            this.cogDisplay1.Name = "cogDisplay1";
            this.cogDisplay1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("cogDisplay1.OcxState")));
            this.cogDisplay1.Size = new System.Drawing.Size(800, 450);
            this.cogDisplay1.TabIndex = 7;
            // 
            // FormDispImg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.cogDisplay1);
            this.Name = "FormDispImg";
            this.Text = "FormDispImg";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormDispImg_FormClosing);
            this.Load += new System.EventHandler(this.FormDispImg_Load);
            ((System.ComponentModel.ISupportInitialize)(this.cogDisplay1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        internal Cognex.VisionPro.Display.CogDisplay cogDisplay1;
    }
}