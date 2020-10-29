using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenCvSharp;
using OpenCvSharp.Dnn;
using OpenCvSharp.Extensions;

namespace ImgProcessor
{
    public partial class Uitest : Form
    {
        public Uitest()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenT();
            
        }
        private int OpenT()
        {
            

            String filename = "F:\\b.png";
            
            //Mat I = (Bitmap)Image.FromFile(filename);
            Mat I = new Mat(filename, ImreadModes.Color);
            Mat X=new Mat();
            Mat padded=new Mat();                 //以0填充输入图像矩阵
            //Cv2.GetOptimalDFTSize();
            int m = Cv2.GetOptimalDFTSize(I.Rows);
            int n = Cv2.GetOptimalDFTSize(I.Cols);
            

            //填充输入图像I，输入矩阵为padded，上方和左方不做填充处理
            Cv2.CopyMakeBorder(I, padded, 0, m - I.Rows, 0, n - I.Cols, BorderTypes.Constant);


            pictureBox1.Image = ToolFunctions.GetThumbnail(X.ToBitmap(), pictureBox1.Height, pictureBox1.Width);
 


            return 0;
        }
    }
}
