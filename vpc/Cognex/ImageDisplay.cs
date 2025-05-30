﻿using System;
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
    public partial class ImageDisplay : Form
    {
        public ImageDisplay(Bitmap bmp)
        {
            InitializeComponent();
            pictureBox1.Image = bmp;
        }
        public ImageDisplay(string filepath)
        {
            InitializeComponent();
            if (System.IO.File.Exists(filepath))
            {
                this.Text = filepath;
                pictureBox1.Image = Bitmap.FromFile(filepath);
            }
            else
                MessageBox.Show("未找到文件");
        }
    }
}
