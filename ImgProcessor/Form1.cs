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
    public partial class Form1 : Form
    {
        private Graphics g2;
        private Graphics gReal;
        public Bitmap bitmap;
        private Bitmap origin_bmp;
        private bool isChoosed = false;
        public Form1()
        {
            InitializeComponent();
            tabControllerInit();
            //this.tabControl1.SelectedIndexChanged += tabControl1_SelectedIndexChanged;
            this.StartPosition = FormStartPosition.CenterScreen;
            label_state.Text = "normal";
            g2 = pictureBox_WorkPlace.CreateGraphics();
            this.trackBar_Hsi_H.ValueChanged += new System.EventHandler((a, b) => Hsi_Datapicturebox_Layout());

        }
        private void tabControllerInit()
        {
            foreach (Control tab_page in this.tabControl1.TabPages)
            {
                tab_page.Parent = null;
            }
        }
        private void reset()
        {

            hsiReset();
            paintReset();
            greyReset();
            noiseReset();
            etcReset();

        }
        private void Reset_Click(object sender, EventArgs e)
        {
            if (isChoosed == false)
            {
                MessageBox.Show("未选择图片");
                return;
            }
            Page_Reset();
            pictureBox_WorkPlace.Image = ToolFunctions.GetThumbnail((Bitmap)bitmap.Clone(), pictureBox_WorkPlace.Height, pictureBox_WorkPlace.Width) as Image;
        }
        #region //通用控件初始化函数

        //清空功能区域

        #endregion
        //根据radio更改布局
        #region//noise部分
        private Bitmap noise_oldBmp;
        private void Button_Mean_Filter_Click(object sender, System.EventArgs e)
        {
            Bitmap newBmp;
            ProcessFunctions.MeanFilter(noise_oldBmp, out newBmp);
            this.pictureBox_WorkPlace.Image = ToolFunctions.GetThumbnail((Bitmap)newBmp.Clone(), pictureBox_WorkPlace.Height, pictureBox_WorkPlace.Width) as Image;
            noise_oldBmp = newBmp;


        }
        private void button_Add_Gauss_Click(object sender, EventArgs e)
        {
            label_state.Text = "Processing";
            label_state.BackColor = Color.Yellow;
            Button b = (Button)sender;
            b.Enabled = false;
            Bitmap newBmp;
            ProcessFunctions.AddGaussSalt(noise_oldBmp, out newBmp, new GaussParam(((float)trackBar_Nu.Value) / 10, ((float)trackBar_Nd.Value) / 10, trackBar_Nk.Value));
            this.pictureBox_WorkPlace.Image = ToolFunctions.GetThumbnail((Bitmap)newBmp.Clone(), pictureBox_WorkPlace.Height, pictureBox_WorkPlace.Width) as Image;
            noise_oldBmp = newBmp;
            b.Enabled = true;
            label_state.Text = "Finish";
            label_state.BackColor = Color.Green;
        }
        private void Button_Add_Pepper_Click(object sender, System.EventArgs e)
        {
            label_state.Text = "Processing";
            label_state.BackColor = Color.Yellow;
            Button b = (Button)sender;
            b.Enabled = false;
            Bitmap newBmp;
            ProcessFunctions.AddPepperSalt(noise_oldBmp, ((float)trackBar_Pa.Value) / 10, ((float)trackBar_Pb.Value) / 10, out newBmp);
            this.pictureBox_WorkPlace.Image = ToolFunctions.GetThumbnail((Bitmap)newBmp.Clone(), pictureBox_WorkPlace.Height, pictureBox_WorkPlace.Width) as Image;
            noise_oldBmp = newBmp;
            b.Enabled = true;
            label_state.Text = "Finish";
            label_state.BackColor = Color.Green;
        }
        private void Button_Mid_Filter_Click(object sender, System.EventArgs e)
        {
            Bitmap newBmp;
            ProcessFunctions.MedianFilter(noise_oldBmp, out newBmp);
            this.pictureBox_WorkPlace.Image = ToolFunctions.GetThumbnail((Bitmap)newBmp.Clone(), pictureBox_WorkPlace.Height, pictureBox_WorkPlace.Width) as Image;
            noise_oldBmp = newBmp;
        }
        private void TrackBar_Nd_ValueChanged(object sender, System.EventArgs e)
        {
            this.label_Nd.Text = (((float)trackBar_Nd.Value) / 10).ToString();
        }

        private void TrackBar_Nk_ValueChanged(object sender, System.EventArgs e)
        {
            this.label_Nk.Text = trackBar_Nk.Value.ToString();
        }

        private void TrackBar_Nu_ValueChanged(object sender, System.EventArgs e)
        {
            this.label_N_u.Text = (((float)trackBar_Nu.Value) / 10).ToString();
        }
        private void TrackBar_Pb_ValueChanged(object sender, System.EventArgs e)
        {
            this.label_Pb.Text = (((float)trackBar_Pb.Value) / 10).ToString();
        }

        private void TrackBar_Pa_ValueChanged(object sender, System.EventArgs e)
        {
            this.label_Pa.Text = (((float)trackBar_Pa.Value) / 10).ToString();
        }
        private void noiseReset()
        {
            noise_oldBmp = (Bitmap)bitmap.Clone();
        }
        #endregion
        #region//Hsi部分
        private int[] countHuePixel = new int[73];
        private int maxHuePixel = 0;
        private void hsiReset()
        {

            HV = 0;
            SV = 0;
            IV = 0;
            HSI_FLAG = 0;
            Hsi_bitmap = (Bitmap)bitmap.Clone();
            Hsi_tempBmp = (Bitmap)bitmap.Clone();
            Hsi_nowBmp = (Bitmap)bitmap.Clone();
        }
        private Bitmap Hsi_bitmap;

        private void Hsi_Page_Init()
        {
            hsiReset();
            pictureBox_WorkPlace.MouseClick += Hsi_PictureBox2_MouseClick;
            Hsi_Datapicturebox_Layout();
        }

        private void Hsi_PictureBox2_MouseClick(object sender, MouseEventArgs e)
        {
            Color c;
            if (GetColor(e, out c) == false)
            {
                return;
            }
            label_H.Text = c.GetHue().ToString();
            label_S.Text = c.GetSaturation().ToString();
            label_I.Text = c.GetBrightness().ToString();
        }
        private void Hsi_Datapicturebox_Layout()
        {

            ProcessFunctions.Hue_Calculator(Hsi_nowBmp, out maxHuePixel, ref countHuePixel);
            Graphics g = tabPage_Hsi.CreateGraphics();
            g.Clear(Color.White);
            int offset_Y = 350;
            Pen curPen = new Pen(Brushes.Black, 2);
            g.DrawString("色调直方图", new Font("New Timer", 15), Brushes.Black, new Point(139, 10 + offset_Y));
            g.DrawLine(curPen, 50, 240 + offset_Y, 350, 240 + offset_Y);
            g.DrawLine(curPen, 50, 240 + offset_Y, 50, 30 + offset_Y);
            g.DrawLine(curPen, 100, 240 + offset_Y, 100, 242 + offset_Y);
            g.DrawLine(curPen, 150, 240 + offset_Y, 150, 242 + offset_Y);
            g.DrawLine(curPen, 200, 240 + offset_Y, 200, 242 + offset_Y);
            g.DrawLine(curPen, 250, 240 + offset_Y, 250, 242 + offset_Y);
            g.DrawLine(curPen, 300, 240 + offset_Y, 300, 242 + offset_Y);
            g.DrawLine(curPen, 350, 240 + offset_Y, 350, 242 + offset_Y);
            g.DrawString("0", new Font("New Timer", 8), Brushes.Black, new PointF(46, 242 + offset_Y));
            g.DrawString("60", new Font("New Timer", 8), Brushes.Black, new PointF(92, 242 + offset_Y));
            g.DrawString("120", new Font("New Timer", 8), Brushes.Black, new PointF(139, 242 + offset_Y));
            g.DrawString("180", new Font("New Timer", 8), Brushes.Black, new PointF(189, 242 + offset_Y));
            g.DrawString("240", new Font("New Timer", 8), Brushes.Black, new PointF(239, 242 + offset_Y));
            g.DrawString("300", new Font("New Timer", 8), Brushes.Black, new PointF(289, 242 + offset_Y));
            g.DrawString("360", new Font("New Timer", 8), Brushes.Black, new PointF(339, 242 + offset_Y));
            g.DrawLine(curPen, 48, 40 + offset_Y, 50, 40 + offset_Y);
            g.DrawString("0", new Font("New Timer", 8), Brushes.Black, new PointF(34, 234 + offset_Y));
            g.DrawString(maxHuePixel.ToString(), new Font("New Timer", 8), Brushes.Black, new PointF(18, 34 + offset_Y));

            double temp = 0;
            for (int i = 0; i < 73; i++)
            {
                temp = 200.0 * countHuePixel[i] / maxHuePixel;
                if (temp > 199)
                {
                    Console.WriteLine(i);
                }
                SolidBrush solidBrush = new SolidBrush(ProcessFunctions.HSL2RGB(5 * i, 0.5, 0.5));
                Pen ContPen = new Pen(solidBrush, 2);
                g.DrawLine(ContPen, (float)(60 + 3.5 * i), 240 + offset_Y, (float)(60 + 3.5 * i), 240 - (int)temp + offset_Y);
            }

            curPen.Dispose();
        }
        private int HV = 0;
        private int SV = 0;
        private int IV = 0;
        private int HSI_FLAG = 0; //H=1,S=2,I=3
        private Bitmap Hsi_tempBmp;
        private Bitmap Hsi_nowBmp;

        private void Hsi_I_ValueChanged(object sender, EventArgs e)
        {
            TrackBar self = (TrackBar)sender;
            Bitmap dbitmap;
            IV = self.Value;
            if (3 == HSI_FLAG)
            {
                ProcessFunctions.Hsl_Change("l", IV, (Bitmap)Hsi_tempBmp.Clone(), out dbitmap);
            }
            else
            {
                Bitmap dbitmap1;
                ProcessFunctions.Hsl_Change("h", HV, (Bitmap)Hsi_bitmap.Clone(), out dbitmap1);
                Bitmap dbitmap2;
                ProcessFunctions.Hsl_Change("s", SV, (Bitmap)dbitmap1.Clone(), out dbitmap2);
                Hsi_tempBmp = dbitmap2;
                ProcessFunctions.Hsl_Change("l", IV, (Bitmap)Hsi_tempBmp.Clone(), out dbitmap);
            }
            //I_bmp = dbitmap;
            this.pictureBox_WorkPlace.Image = ToolFunctions.GetThumbnail((Bitmap)dbitmap.Clone(), pictureBox_WorkPlace.Height, pictureBox_WorkPlace.Width) as Image;
            Hsi_nowBmp = dbitmap;
        }

        private void Hsi_S_ValueChanged(object sender, EventArgs e)
        {
            TrackBar self = (TrackBar)sender;
            Bitmap dbitmap;
            SV = self.Value;
            if (2 == HSI_FLAG)
            {
                ProcessFunctions.Hsl_Change("s", SV, (Bitmap)Hsi_tempBmp.Clone(), out dbitmap);
            }
            else
            {
                Bitmap dbitmap1;
                ProcessFunctions.Hsl_Change("h", HV, (Bitmap)Hsi_bitmap.Clone(), out dbitmap1);
                Bitmap dbitmap2;
                ProcessFunctions.Hsl_Change("l", IV, (Bitmap)dbitmap1.Clone(), out dbitmap2);
                Hsi_tempBmp = dbitmap2;
                ProcessFunctions.Hsl_Change("s", SV, (Bitmap)Hsi_tempBmp.Clone(), out dbitmap);
            }
            //S_bmp = dbitmap;
            //Hsi_bitmap = dbitmap;
            this.pictureBox_WorkPlace.Image = ToolFunctions.GetThumbnail((Bitmap)dbitmap.Clone(), pictureBox_WorkPlace.Height, pictureBox_WorkPlace.Width) as Image;
            Hsi_nowBmp = dbitmap;
        }

        private void Hsi_H_ValueChanged(object sender, EventArgs e)
        {
            TrackBar self = (TrackBar)sender;
            Bitmap dbitmap;
            HV = self.Value;
            if (1 == HSI_FLAG)
            {
                ProcessFunctions.Hsl_Change("h", HV, (Bitmap)Hsi_tempBmp.Clone(), out dbitmap);
            }
            else
            {
                Bitmap dbitmap1;
                ProcessFunctions.Hsl_Change("l", IV, (Bitmap)Hsi_bitmap.Clone(), out dbitmap1);
                Bitmap dbitmap2;
                ProcessFunctions.Hsl_Change("S", SV, (Bitmap)dbitmap1.Clone(), out dbitmap2);
                Hsi_tempBmp = dbitmap2;
                ProcessFunctions.Hsl_Change("h", HV, (Bitmap)Hsi_tempBmp.Clone(), out dbitmap);
            }
            this.pictureBox_WorkPlace.Image = ToolFunctions.GetThumbnail((Bitmap)dbitmap.Clone(), pictureBox_WorkPlace.Height, pictureBox_WorkPlace.Width) as Image;
            Hsi_nowBmp = dbitmap;
        }
        #endregion
        #region//etc部分



        private void etcReset()
        {

        }
        private void button_Sharpen_Click(object sender, EventArgs e)
        {
            Bitmap sharpenBmp;
            sharpenBmp = ProcessFunctions.SharpenFilter2(bitmap, trackBar_SV.Value);
            this.pictureBox_WorkPlace.Image = ToolFunctions.GetThumbnail((Bitmap)sharpenBmp.Clone(), pictureBox_WorkPlace.Height, pictureBox_WorkPlace.Width) as Image;

        }
        private void trackBarDV_ValueChanged(object sender, EventArgs e)
        {
            this.label_DV.Text = trackBar_DV.Value.ToString();
        }
        private void trackBar_SV_ValueChanged(object sender, EventArgs e)
        {
            this.label_SV.Text = trackBar_SV.Value.ToString();
        }
        private void Button_TwoD_Click(object sender, System.EventArgs e)
        {
            Bitmap twoVBmp;
            ProcessFunctions.TwoDivision_Change(bitmap, out twoVBmp, trackBar_DV.Value);
            this.pictureBox_WorkPlace.Image = ToolFunctions.GetThumbnail((Bitmap)twoVBmp.Clone(), pictureBox_WorkPlace.Height, pictureBox_WorkPlace.Width) as Image;
        }
        #endregion
        #region //grey部分
        private System.Drawing.Bitmap bmpHist;
        private int[] countGreyPixel = new int[256];
        private int maxGreyPixel = 0;

        private void greyReset()
        {
            bmpHist = bitmap;
            countGreyPixel = new int[256];
            maxGreyPixel = 0;
        }
        private void Grey_Datagraphic_Layout()
        {
            //画出坐标系
            
            Graphics g = tabPage_Grey.CreateGraphics();
            g.Clear(Color.White);
            Pen curPen = new Pen(Brushes.Black, 1);
            int offset_Y = 350;
            g.DrawString("灰度直方图", new Font("New Timer", 15), Brushes.Black, new Point(139, 10 + offset_Y));

            g.DrawLine(curPen, 50, 240 + offset_Y, 320, 240 + offset_Y);
            g.DrawLine(curPen, 50, 240 + offset_Y, 50, 30 + offset_Y);
            g.DrawLine(curPen, 100, 240 + offset_Y, 100, 242 + offset_Y);
            g.DrawLine(curPen, 150, 240 + offset_Y, 150, 242 + offset_Y);
            g.DrawLine(curPen, 200, 240 + offset_Y, 200, 242 + offset_Y);
            g.DrawLine(curPen, 250, 240 + offset_Y, 250, 242 + offset_Y);
            g.DrawLine(curPen, 300, 240 + offset_Y, 300, 242 + offset_Y);
            g.DrawString("0", new Font("New Timer", 8), Brushes.Black, new PointF(46, 242 + offset_Y));
            g.DrawString("50", new Font("New Timer", 8), Brushes.Black, new PointF(92, 242 + offset_Y));
            g.DrawString("100", new Font("New Timer", 8), Brushes.Black, new PointF(139, 242 + offset_Y));
            g.DrawString("150", new Font("New Timer", 8), Brushes.Black, new PointF(189, 242 + offset_Y));
            g.DrawString("200", new Font("New Timer", 8), Brushes.Black, new PointF(239, 242 + offset_Y));
            g.DrawString("250", new Font("New Timer", 8), Brushes.Black, new PointF(289, 242 + offset_Y));
            g.DrawLine(curPen, 48, 40 + offset_Y, 50, 40 + offset_Y);
            g.DrawString("0", new Font("New Timer", 8), Brushes.Black, new PointF(34, 234 + offset_Y));
            g.DrawString(maxGreyPixel.ToString(), new Font("New Timer", 8), Brushes.Black, new PointF(18, 34 + offset_Y));

            double temp = 0;
            for (int i = 0; i < 256; i++)
            {
                temp = 200.0 * countGreyPixel[i] / maxGreyPixel;
                g.DrawLine(curPen, 50 + i, 240 + offset_Y, 50 + i, 240 - (int)temp + offset_Y);
            }

            curPen.Dispose();
        }
        private void TrackBar_Grey_ValueChange(object sender, EventArgs e)
        {
            TrackBar self = (TrackBar)sender;
            Bitmap dbitmap;
            ProcessFunctions.Grey_Change(bmpHist, out dbitmap, self.Value);
            this.pictureBox_WorkPlace.Image = ToolFunctions.GetThumbnail((Bitmap)dbitmap.Clone(), pictureBox_WorkPlace.Height, pictureBox_WorkPlace.Width) as Image;
            //this.pictureBox_data.CreateGraphics().Clear(Color.White);
            ProcessFunctions.Grey_Calculator(dbitmap, out maxGreyPixel, ref countGreyPixel);
            Grey_Datagraphic_Layout();
        }

        private void Get_Balance_Click(object sender, EventArgs e)
        {
            Button self = (Button)sender;
            self.Enabled = false;
            self.Text = "已均衡化";
            Bitmap dbitmap;
            ProcessFunctions.Balance(bmpHist, out dbitmap);
            this.pictureBox_WorkPlace.Image = ToolFunctions.GetThumbnail((Bitmap)dbitmap.Clone(), pictureBox_WorkPlace.Height, pictureBox_WorkPlace.Width) as Image;
            //this.pictureBox_data.CreateGraphics().Clear(Color.White);
            ProcessFunctions.Grey_Calculator(dbitmap, out maxGreyPixel, ref countGreyPixel);
            Grey_Datagraphic_Layout();
        }
        private void Grey_Page_Init()
        {
            greyReset();
            ProcessFunctions.Grey_Calculator(bmpHist, out maxGreyPixel, ref countGreyPixel);
            this.pictureBox_WorkPlace.MouseClick += PictureBox2_MouseClick_GetColor;
            Grey_Datagraphic_Layout();
        }


        private bool GetColor(MouseEventArgs e, out Color c)
        {
            int bw = bitmap.Width * pictureBox_WorkPlace.Height / bitmap.Height;
            int pw = pictureBox_WorkPlace.Width;
            int offset_x = (pw - bw) / 2;
            Point clickp = new Point(e.Location.X - offset_x, e.Location.Y);
            if (clickp.X < 0 || clickp.X > bw)
            {
                c = Color.White;
                return false;
            }
            //e.Location.X -= offset_x;
            Console.WriteLine(bw);
            Console.WriteLine(pw);
            c = ProcessFunctions.GetColor(MousePosition);
            return true;
        }
        private void PictureBox2_MouseClick_GetColor(object sender, MouseEventArgs e)
        {

            Color c;
            if (GetColor(e, out c) == false)
            {
                return;
            }
            label_GR.Text = c.R.ToString();
            label_GG.Text = c.G.ToString();
            label_GB.Text = c.B.ToString();
            pictureBox_eg.BackColor = c;
            double Grey = c.R * 0.299 + c.G * 0.587 + c.B * 0.114;
            label_GGrey.Text = Grey.ToString();
            panel_Grey_GetColor.CreateGraphics().Clear(c);


        }
        #endregion
        #region//Paint部分
        Graphics g_paint_temp;
        Graphics g_paint_temp2;
        private void Paint_Page_Init()
        {
            pictureBox_WorkPlace.MouseDown += pictureBox2_Paint_MouseDown;
            pictureBox_WorkPlace.MouseMove += pictureBox2_Paint_MouseMove;
            pictureBox_WorkPlace.MouseUp += pictureBox2_Paint_MouseUp;
            paintReset();
            g_paint_temp = panel_width.CreateGraphics();
            g_paint_temp.Clear(Color.White);
            g_paint_temp.DrawLine(new Pen(Brushes.Black, trackBar_Paint.Value), new Point(5, 5), new Point(200, 5));
            g_paint_temp2 = panel_ColorPick.CreateGraphics();
            g_paint_temp2.Clear(choose_color);
        }

        private void TrackBar_PaintWidth_ValueChanged(object sender, EventArgs e)
        {
            g_paint_temp.Clear(Color.White);
            g_paint_temp.DrawLine(new Pen(Brushes.Black, trackBar_Paint.Value), new Point(5, 5), new Point(200, 5));
        }

        private void paintReset()
        {
            choose_color = Color.Red;
            in_box_flag = 0;
            lastP = new Point(-1, -1);
        }


        #region //涂鸦的颜色选择板
        private Color choose_color = Color.Red;
        private void Btn_ColorBoard_Click(object sender, EventArgs e)
        {
            ColorDialog ColorForm = new ColorDialog();
            if (ColorForm.ShowDialog() == DialogResult.OK)
            {
                Color GetColor = ColorForm.Color;
                //GetColor就是用户选择的颜色，接下来就可以使用该颜色了
                Console.WriteLine(GetColor);
                choose_color = GetColor;
                g_paint_temp2.Clear(GetColor);
            }
        }
        #endregion

        #region //涂鸦时picturebox的响应事件
        private int in_box_flag = 0;
        private void pictureBox2_Paint_MouseDown(object sender, MouseEventArgs e)
        {
            in_box_flag = 1;
        }

        private Point lastP = new Point(-1, -1);
        private void myDrawLine(Pen pen, Point p1, Point p2)
        {
            g2.DrawLine(pen, p1, p2);
            gReal.DrawLine(pen, p1, p2);
        }
        private void pictureBox2_Paint_MouseMove(object sender, MouseEventArgs e)
        {
            if (isChoosed == false)
            {
                return;
            }
            if (in_box_flag == 1)
            {
                //Console.WriteLine(e.Location.X + "," + e.Location.Y + "  " + flag);
                if (lastP.X == -1)
                {
                    Console.WriteLine("in");
                    lastP = e.Location;
                    return;
                }
                else
                {
                    SolidBrush solidBrush = new SolidBrush(choose_color);

                    Pen pen = new Pen(solidBrush, trackBar_Paint.Value);


                    myDrawLine(pen, lastP, e.Location);

                    lastP = e.Location;
                }


            }
        }

        private void pictureBox2_Paint_MouseUp(object sender, MouseEventArgs e)
        {
            lastP = new Point(-1, -1);
            in_box_flag = 0;

        }
        #endregion
        #region 吸管响应事件
        private void button6_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            ToolFunctions.ClearEvent(this.pictureBox_WorkPlace, "MouseDown");
            ToolFunctions.ClearEvent(this.pictureBox_WorkPlace, "MouseUp");

            if (btn.Text == "使用吸管")
            {
                btn.Text = "使用画笔";
                this.pictureBox_WorkPlace.MouseDown += PictureBox_WorkPlace_MouseDown_GetColor;
            }
            else if (btn.Text == "使用画笔")
            {
                btn.Text = "使用吸管";
                this.pictureBox_WorkPlace.MouseDown += pictureBox2_Paint_MouseDown;
                this.pictureBox_WorkPlace.MouseUp += pictureBox2_Paint_MouseUp;
            }
        }

        private void PictureBox_WorkPlace_MouseDown_GetColor(object sender, MouseEventArgs e)
        {
            Color c;
            if (GetColor(e, out c) == false)
            {
                return;
            }
            panel_Straw.BackColor = c;
            label_PR.Text = c.R.ToString();
            label_PG.Text = c.G.ToString();
            label_PBlue.Text = c.B.ToString();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            button6.PerformClick();
            choose_color = this.panel_Straw.BackColor;
            panel_ColorPick.BackColor = choose_color;
        }
        #endregion
        #endregion
        #region //主界面按钮响应函数
        //save button
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
                            this.pictureBox_WorkPlace.Image.Save(fileName, imgformat);
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
        //clean button
        private void button2_Click_1(object sender, EventArgs e)
        {
            if (isChoosed == false)
            {
                return;
            }
            pictureBox_WorkPlace.Image = ToolFunctions.GetThumbnail((Bitmap)bitmap.Clone(), pictureBox_WorkPlace.Height, pictureBox_WorkPlace.Width) as Image;
            gReal = Graphics.FromImage(pictureBox_WorkPlace.Image);
        }
        //get button
        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                isChoosed = true;

                string path = openFileDialog1.FileName;
                bitmap = (Bitmap)Image.FromFile(path);
                origin_bmp = (Bitmap)bitmap.Clone();
                reset();
                Size picSize = new Size();
                //pictureBox1.Image = ToolFunctions.GetThumbnail((Bitmap)bitmap.Clone(), pictureBox1.Height, pictureBox1.Width) as Image;
                Image temp = ToolFunctions.GetInitThumbnail((Bitmap)bitmap.Clone(), pictureBox_WorkPlace.Height, pictureBox_WorkPlace.Width,out picSize) as Image;
                this.pictureBox_WorkPlace.Width = picSize.Width;
                this.pictureBox_WorkPlace.Location = new Point(14 + 1097 / 2 - picSize.Width / 2, 25);
                this.pictureBox_WorkPlace.Image = temp;
                gReal = Graphics.FromImage(pictureBox_WorkPlace.Image);
                if (tabControl1.SelectedTab == null)
                {
                    return;
                }
                foreach (Control control in tabControl1.SelectedTab.Controls)
                {
                    control.Enabled = true;
                }

            }
        }

        #endregion


        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Console.WriteLine("Change");
            if (isChoosed == false)
            {
                MessageBox.Show("未选择图片");
            }
            label_funcName.Text = tabControl1.SelectedTab == null ? "empty" : tabControl1.SelectedTab.Text;
            if (tabControl1.SelectedTab == null)
            {
                return;
            }

            foreach (Control c in tabControl1.SelectedTab.Controls)
            {
                c.Enabled = isChoosed;
            }
            DialogResult dr = MessageBox.Show("是否保存修改结果？", "提示", MessageBoxButtons.OKCancel);
            if (dr == DialogResult.OK)
            {
                bitmap = new Bitmap(this.pictureBox_WorkPlace.Image);
            }
            else
            {
                this.pictureBox_WorkPlace.Image = ToolFunctions.GetThumbnail((Bitmap)bitmap.Clone(), pictureBox_WorkPlace.Height, pictureBox_WorkPlace.Width);
            }
            
            Page_Reset();
            
        }
        
        private void Page_Reset(){
            ToolFunctions.ClearEvent(pictureBox_WorkPlace, "MouseMove");
            ToolFunctions.ClearEvent(pictureBox_WorkPlace, "MouseClick");
            ToolFunctions.ClearEvent(pictureBox_WorkPlace, "MouseUp");
            label_funcName.Text = tabControl1.SelectedTab==null?"empty": tabControl1.SelectedTab.Text;
            if (isChoosed == true)
            {
                if (tabControl1.SelectedTab == tabPage_Paint)
                {
                    Paint_Page_Init();
                    Console.WriteLine("in paint");
                }
                else if (tabControl1.SelectedTab == tabPage_Grey)
                {
                    Grey_Page_Init();
                    Console.WriteLine("in grey");
                }
                else if (tabControl1.SelectedTab == tabPage_Hsi)
                {
                    Hsi_Page_Init();
                }
            }
        }

        private void tabControl1_MouseLeave(object sender, EventArgs e)
        {
            Grey_Page_Init();
        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            //Page_Reset();
        }

        private void button_checkOrigin_Click(object sender, EventArgs e)
        {
            if (bitmap == null)
            {
                MessageBox.Show("no original image");
                return;
            }
            Form_ShowOrigin form = new Form_ShowOrigin((Bitmap)origin_bmp.Clone());
            form.ShowDialog();
        }

        private void 涂鸦ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Console.WriteLine(tabControl1.TabPages.Count);
            if(this.tabPage_Paint.Parent==null)
                this.tabPage_Paint.Parent = tabControl1;
            tabControl1.SelectedTab = tabPage_Paint;

        }

        private void 灰度变换ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.tabPage_Grey.Parent == null)
                this.tabPage_Grey.Parent = tabControl1;
            tabControl1.SelectedTab = tabPage_Grey;
        }

        private void tabControl1_ControlAdded(object sender, ControlEventArgs e)
        {
            if (isChoosed == false && tabControl1.TabPages.Count==1)
            {
                MessageBox.Show("未选择图片");
                foreach (Control c in e.Control.Controls)
                {
                    c.Enabled = false;
                }
            }
            if (tabControl1.TabPages.Count == 1)
            {
                Page_Reset();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == null)
            {
                MessageBox.Show("未打开任何工具");
            }
            else
            {
                tabControl1.SelectedTab.Parent = null;
            }
        }

        private void hsi模块ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.tabPage_Hsi.Parent == null)
                this.tabPage_Hsi.Parent = tabControl1;
            tabControl1.SelectedTab = tabPage_Hsi;
        }

        private void 噪声模块ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.tabPage_Noise.Parent == null)
                this.tabPage_Noise.Parent = tabControl1;
            tabControl1.SelectedTab = tabPage_Noise;
        }

        private void 其他功能ToolStripMenuItem_Click(object sender, EventArgs e)
        {           
            if (this.tabPage_etc.Parent == null)
                this.tabPage_etc.Parent = tabControl1;
            tabControl1.SelectedTab = tabPage_etc;
        }


    }
}

