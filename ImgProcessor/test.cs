using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImgProcessor
{
    public partial class test : Form
    {
        public Bitmap bitmap;
        private Graphics g2;
        private Graphics gReal;
        //private Image imgTemp;
        public test()
        {
            InitializeComponent();
            g2 = pictureBox2.CreateGraphics();
            //this.Load += new System.EventHandler(this.Grey_ScaleMapForm_Load);
            //this.pictureBox3.Paint += new System.Windows.Forms.PaintEventHandler(this.Grey_ScaleMapForm_Paint);
            string path = "F:\\b.png";
            bitmap = (Bitmap)Image.FromFile(path);
            bmpHist = bitmap;

            countPixel = new int[256];
            //imgTemp = new Bitmap(pictureBox2.Width, pictureBox2.Height);

        }

        private System.Drawing.Bitmap bmpHist;
        private int[] countPixel;
        private int maxPixel;
        private void Grey_ScaleMapForm_Paint()
        {
            //画出坐标系
            Graphics g = pictureBox3.CreateGraphics();

            Pen curPen = new Pen(Brushes.Black, 1);

            g.DrawLine(curPen, 50, 240, 320, 240);
            g.DrawLine(curPen, 50, 240, 50, 30);
            g.DrawLine(curPen, 100, 240, 100, 242);
            g.DrawLine(curPen, 150, 240, 150, 242);
            g.DrawLine(curPen, 200, 240, 200, 242);
            g.DrawLine(curPen, 250, 240, 250, 242);
            g.DrawLine(curPen, 300, 240, 300, 242);
            g.DrawString("0", new Font("New Timer", 8), Brushes.Black, new PointF(46, 242));
            g.DrawString("50", new Font("New Timer", 8), Brushes.Black, new PointF(92, 242));
            g.DrawString("100", new Font("New Timer", 8), Brushes.Black, new PointF(139, 242));
            g.DrawString("150", new Font("New Timer", 8), Brushes.Black, new PointF(189, 242));
            g.DrawString("200", new Font("New Timer", 8), Brushes.Black, new PointF(239, 242));
            g.DrawString("250", new Font("New Timer", 8), Brushes.Black, new PointF(289, 242));
            g.DrawLine(curPen, 48, 40, 50, 40);
            g.DrawString("0", new Font("New Timer", 8), Brushes.Black, new PointF(34, 234));
            g.DrawString(maxPixel.ToString(), new Font("New Timer", 8), Brushes.Black, new PointF(18, 34));

            double temp = 0;
            for (int i = 0; i < 256; i++)
            {
                temp = 200.0 * countPixel[i] / maxPixel;
                g.DrawLine(curPen, 50 + i, 240, 50 + i, 240 - (int)temp);
            }

            curPen.Dispose();
        }

        private void Grey_ScaleMapForm_Load(object sender, EventArgs e)
        {
            //将图像数据复制到byte中
            Rectangle rect = new Rectangle(0, 0, bmpHist.Width, bmpHist.Height);
            System.Drawing.Imaging.BitmapData bmpdata = bmpHist.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, bmpHist.PixelFormat);
            IntPtr ptr = bmpdata.Scan0;

            int bytes = bmpHist.Width * bmpHist.Height * 3;
            byte[] grayValues = new byte[bytes];

            System.Runtime.InteropServices.Marshal.Copy(ptr, grayValues, 0, bytes);

            //统计直方图信息
            byte temp = 0;
            maxPixel = 0;
            Array.Clear(countPixel, 0, 256);
            for (int i = 0; i < bytes; i++)
            {
                temp = grayValues[i];
                countPixel[temp]++;
                if (countPixel[temp] > maxPixel)
                {
                    maxPixel = countPixel[temp];
                }
            }

            System.Runtime.InteropServices.Marshal.Copy(grayValues, 0, ptr, bytes);

            bmpHist.UnlockBits(bmpdata);
            Console.WriteLine(countPixel[0]);

        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string path = openFileDialog1.FileName;
                bitmap = (Bitmap)Image.FromFile(path);
                pictureBox1.Image = ToolFunctions.GetThumbnail( (Bitmap)bitmap.Clone(),pictureBox1.Height,pictureBox1.Width) as Image;
                gReal = Graphics.FromImage(pictureBox1.Image);
                pictureBox2.Image = pictureBox1.Image;
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            bool isSave = true;
   
            
            //pictureBox2.Image = imgTemp;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string fileName = saveFileDialog1.FileName.ToString();

                if (fileName != "" && fileName != null)
                {
                    string fileExtName = fileName.Substring(fileName.LastIndexOf(".") + 1).ToString();

                    System.Drawing.Imaging.ImageFormat imgformat = null;

                    if (fileExtName != "")
                    {
                        switch (fileExtName)
                        {
                            case "jpg":
                                imgformat = System.Drawing.Imaging.ImageFormat.Jpeg;
                                break;
                            case "bmp":
                                imgformat = System.Drawing.Imaging.ImageFormat.Bmp;
                                break;
                            case "gif":
                                imgformat = System.Drawing.Imaging.ImageFormat.Gif;
                                break;
                            default:
                                MessageBox.Show("只能存取为: jpg,bmp,gif 格式");
                                isSave = false;
                                break;
                        }

                    }

                    //默认保存为JPG格式   
                    if (imgformat == null)
                    {
                        imgformat = System.Drawing.Imaging.ImageFormat.Jpeg;
                    }

                    if (isSave)
                    {
                        try
                        {
                           this.pictureBox2.Image.Save(fileName, imgformat);
                            //MessageBox.Show("图片已经成功保存!");   
                        }
                        catch
                        {
                            MessageBox.Show("保存失败,你还没有截取过图片或已经清空图片!");
                        }
                    }
                }
            }
        }

        private int flag = 0;
        private void pictureBox2_MouseDown(object sender, MouseEventArgs e)
        {
            flag = 1;
        }

        private Point lastP=new Point(-1,-1);
        private void myDrawLine(Pen pen,Point p1,Point p2)
        {
            g2.DrawLine(pen, p1,p2);
            gReal.DrawLine(pen, p1,p2);
        }
        private void pictureBox2_MouseMove(object sender, MouseEventArgs e)
        {
            if (flag == 1)
            {
                //Console.WriteLine(e.Location.X + "," + e.Location.Y + "  " + flag);
                if (lastP.X==-1)
                {
                    Console.WriteLine("in");
                    lastP = e.Location;
                    return;
                }
                else
                {
                    SolidBrush solidBrush = new SolidBrush(Color.Red);
                    
                    Pen pen = new Pen(solidBrush);

                    
                    myDrawLine(pen, lastP, e.Location);
                  
                    lastP = e.Location;
                }

               
            }
        }

        private void pictureBox2_MouseUp(object sender, MouseEventArgs e)
        {
            lastP = new Point(-1, -1);
            flag = 0;

        }

        private void button3_Click(object sender, EventArgs e)
        {
            ProcessFunctions.Grey_Calculator(bmpHist,out maxPixel,ref countPixel);
            Grey_ScaleMapForm_Paint();
            pictureBox1.Image = ToolFunctions.GetThumbnail((Bitmap)bitmap.Clone(), pictureBox1.Height, pictureBox1.Width) as Image;
            Bitmap destB;
            ProcessFunctions.Balance(bitmap, out destB);
            pictureBox2.Image = ToolFunctions.GetThumbnail((Bitmap)destB.Clone(), pictureBox2.Height, pictureBox2.Width) as Image;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Bitmap dest;
            pictureBox1.Image = ToolFunctions.GetThumbnail((Bitmap)bitmap.Clone(), pictureBox1.Height, pictureBox1.Width) as Image;
            ProcessFunctions.Grey_Change(bitmap,out dest, -5);
            pictureBox2.Image = ToolFunctions.GetThumbnail((Bitmap)dest.Clone(), pictureBox2.Height, pictureBox2.Width) as Image;
            pictureBox1.MouseClick += PictureBox1_MouseClick;
        }

        private void PictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            // w1/h1=w/h  h1=H w1=H*w/h

            int bw = bitmap.Width * pictureBox1.Height / bitmap.Height;
            int pw = pictureBox1.Width;
            int offset_x = (pw - bw) / 2;
            Point clickp = new Point(e.Location.X - offset_x, e.Location.Y);
            if (clickp.X < 0 || clickp.X > bw)
            {
                return;
            }
            //e.Location.X -= offset_x;
            Console.WriteLine(bw);
            Console.WriteLine(pw);
            Color c = ProcessFunctions.GetColor(MousePosition);
            this.pictureBox3.BackColor = c;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Bitmap srcm;
            pictureBox1.Image= ToolFunctions.GetThumbnail((Bitmap)bitmap.Clone(), pictureBox1.Height, pictureBox1.Width) as Image;
            ProcessFunctions.Hsl_Change("s", -5, bitmap,out srcm);
            pictureBox2.Image = ToolFunctions.GetThumbnail((Bitmap)srcm.Clone(), pictureBox1.Height, pictureBox1.Width) as Image;
            Color c = Color.FromArgb(15, 40, 200);
            Color c2=ProcessFunctions.Hsl_Pixel_Change(c, "h", 24);
            //Console.WriteLine(hue + "," + sa + "," + l);
            Console.WriteLine(c.R + ","+ c.G + "," + c.B + " ; " + c2.R + "," + c2.G + "," + c2.B);
            //pictureBox3.BackColor = Color.FromArgb(258, 2, -1);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            pictureBox3.Image = bitmap;
            for(int i = 0; i < 72; i++) { 
                Color color = ProcessFunctions.HSL2RGB(5*i, 0.5, 0.5);
                SolidBrush solidBrush = new SolidBrush(color);
                Pen pen = new Pen(solidBrush, 2);
                Graphics g2 = pictureBox1.CreateGraphics();
                g2.DrawLine(pen, new Point(i*4+2, 20), new Point(i*4+2, 200));
            }
            int maxHuePixel;
            int[] countHuePixel = new int[73];
            ProcessFunctions.Hue_Calculator(bitmap, out maxHuePixel, ref countHuePixel);
            Graphics g = pictureBox2.CreateGraphics();

            Pen curPen = new Pen(Brushes.Black, 1);
            g.DrawString("色调直方图", new Font("New Timer", 15), Brushes.Black, new Point(139, 10));
            g.DrawLine(curPen, 50, 240, 360, 240);
            g.DrawLine(curPen, 50, 240, 50, 30);
            g.DrawLine(curPen, 110, 240, 110, 242);
            g.DrawLine(curPen, 160, 240, 160, 242);
            g.DrawLine(curPen, 210, 240, 210, 242);
            g.DrawLine(curPen, 260, 240, 260, 242);
            g.DrawLine(curPen, 310, 240, 310, 242);
            g.DrawLine(curPen, 360, 240, 360, 242);
            g.DrawString("0", new Font("New Timer", 8), Brushes.Black, new PointF(56, 242));
            g.DrawString("60", new Font("New Timer", 8), Brushes.Black, new PointF(102, 242));
            g.DrawString("120", new Font("New Timer", 8), Brushes.Black, new PointF(149, 242));
            g.DrawString("180", new Font("New Timer", 8), Brushes.Black, new PointF(199, 242));
            g.DrawString("240", new Font("New Timer", 8), Brushes.Black, new PointF(249, 242));
            g.DrawString("300", new Font("New Timer", 8), Brushes.Black, new PointF(299, 242));
            g.DrawString("360", new Font("New Timer", 8), Brushes.Black, new PointF(349, 242));
            g.DrawLine(curPen, 48, 40, 50, 40);
            g.DrawString("0", new Font("New Timer", 8), Brushes.Black, new PointF(34, 234));
            g.DrawString(maxHuePixel.ToString(), new Font("New Timer", 8), Brushes.Black, new PointF(18, 34));

            double temp = 0;
            for (int i = 0; i < 73; i++)
            {
                temp = 200.0 * countHuePixel[i] / maxHuePixel;
                if(temp>199)
                {
                    Console.WriteLine(i);
                }
                SolidBrush solidBrush = new SolidBrush(ProcessFunctions.HSL2RGB(5 * i, 0.5, 0.5));
                Pen ContPen = new Pen(solidBrush, 2);
                g.DrawLine(ContPen, (float)(60 + 3.5 * i), 240, (float)(60 + 3.5 * i), 240 - (int)temp);
            }

            curPen.Dispose();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            double x=ProcessFunctions.GaussNoise(0,1);
            Console.WriteLine(x);
            Bitmap noiseBmp;
            ProcessFunctions.AddPepperSalt(bitmap, 0.2, 0.2, out noiseBmp);
            pictureBox1.Image = ToolFunctions.GetThumbnail((Bitmap)noiseBmp.Clone(), pictureBox1.Height, pictureBox1.Width) as Image;
            Bitmap cleanBmp;
            ProcessFunctions.MedianFilter(noiseBmp, out cleanBmp);
            pictureBox2.Image = ToolFunctions.GetThumbnail((Bitmap)cleanBmp.Clone(), pictureBox2.Height, pictureBox2.Width) as Image;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            //for (int i = 0; i < 1000; i++)
            //{
            //    double x = ProcessFunctions.GaussNoise(0,1);
            //    if(Math.Abs(x)>=1)
            //        Console.WriteLine(x);
            //}
            Bitmap noiseBmp;
            ProcessFunctions.AddGaussSalt(bitmap, out noiseBmp,new GaussParam(0,1,32));
            pictureBox1.Image = ToolFunctions.GetThumbnail((Bitmap)noiseBmp.Clone(), pictureBox1.Height, pictureBox1.Width) as Image;
            Bitmap cleanBmp;
            ProcessFunctions.MeanFilter(noiseBmp, out cleanBmp);
            pictureBox2.Image = ToolFunctions.GetThumbnail((Bitmap)cleanBmp.Clone(), pictureBox2.Height, pictureBox2.Width) as Image;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Bitmap SharpenBmp;
            ProcessFunctions.SharpenFilter1(bitmap,out SharpenBmp);
            pictureBox2.Image = ToolFunctions.GetThumbnail((Bitmap)SharpenBmp.Clone(), pictureBox2.Height, pictureBox2.Width) as Image;
            Bitmap SharpenBmp2 = ProcessFunctions.SharpenFilter2(bitmap, 10F);
            pictureBox1.Image = ToolFunctions.GetThumbnail((Bitmap)SharpenBmp2.Clone(), pictureBox1.Height, pictureBox1.Width) as Image;
            pictureBox3.Image = ToolFunctions.GetThumbnail((Bitmap)bitmap.Clone(), pictureBox3.Height, pictureBox3.Width) as Image;
        }

        private void button10_Click(object sender, EventArgs e)
        {
            Bitmap twoBmp;
            ProcessFunctions.TwoDivision_Change(bitmap, out twoBmp, 150);
            pictureBox2.Image = ToolFunctions.GetThumbnail((Bitmap)twoBmp.Clone(), pictureBox2.Height, pictureBox2.Width) as Image;
        }
    }
}
