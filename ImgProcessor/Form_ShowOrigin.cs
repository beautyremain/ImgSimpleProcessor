using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImgProcessor
{
    public partial class Form_ShowOrigin : Form
    {
        Bitmap ori_bmp;
        public Form_ShowOrigin(Bitmap bitmap)
        {
            InitializeComponent();
            ori_bmp = bitmap;
            this.pictureBox1.Image = ToolFunctions.GetThumbnail((Bitmap)ori_bmp.Clone(), pictureBox1.Height, pictureBox1.Width);
        }
    }
}
