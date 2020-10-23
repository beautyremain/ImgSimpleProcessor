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
        private Graphics g_data;
        public Bitmap bitmap;
        private bool isChoosed=false;
        public Form1()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            label_state.Text = "normal";
            g_data = pictureBox_data.CreateGraphics();
            g2 = pictureBox2.CreateGraphics();
            
        }
        private void reset()
        {
            cleanWorkPlace();
            hsiReset();
            paintReset();
            greyReset();
            noiseReset();
            etcReset();

        }
        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            reset();
            if (isChoosed == false)
            {
                MessageBox.Show("未选择图片");
                return;
            }
            foreach (var item in this.panel1.Controls)
            {
                if (item is Button)
                {
                    continue;
                }
                RadioButton r = (RadioButton)item;
                if (r.Checked == true)
                {
                    Console.WriteLine(r.Tag);
                    operateLayout((string)r.Tag);
                    label_funcName.Text = (string)r.Text;
                    break;
                }
            }
            pictureBox2.Image= ToolFunctions.GetThumbnail((Bitmap)bitmap.Clone(), pictureBox2.Height, pictureBox2.Width) as Image;
        }
        #region //通用控件初始化函数
        //功能区无数据
        private void noDataInDataBox()
        {
            g_data.DrawString("无", new Font("等线", 14), Brushes.Black, new Point(160, 105));
        }
        private void noDataInDataPanel()
        {
            Label l = new Label();
            l.Location = new Point(160, 50);
            l.Text = "无";
            l.Font = new Font("等线", 14);
            panel_adata.Controls.Add(l);
        }
        //清空功能区域
        private void cleanWorkPlace()
        {
            this.panel_adata.Controls.Clear();
            this.panel_oper.Controls.Clear();
            g_data.Clear(Color.White);
        }
        #endregion
        //根据radio更改布局
        private void operateLayout(string tag)
        {
            cleanWorkPlace();
            ToolFunctions.ClearEvent(pictureBox2, "MouseMove");
            ToolFunctions.ClearEvent(pictureBox2, "MouseClick");
            if (tag == "noise")
            {
                noiseLayout();
            }
            else if(tag == "hsi")
            {
                hsiLayout();
            }
            else if (tag == "etc")
            {
                etcLayout();
            }
            else if (tag == "grey")
            {
                greyLayout();
            }
            else if (tag == "paint")
            {
                paintLayout();
            }
        }

        #region//noise部分
        private Bitmap noise_oldBmp;
        private void noiseLayout()
        {
            noiseReset();
            noDataInDataBox();
            noDataInDataPanel();
            noise_Oper_Layout();
        }
        private void noise_Oper_Layout()
        {
            this.panel_oper.Controls.Add(this.trackBar_Nd);
            this.panel_oper.Controls.Add(this.trackBar_Nk);
            this.panel_oper.Controls.Add(this.trackBar_Nu);
            this.panel_oper.Controls.Add(this.button_Mid_Filter);
            this.panel_oper.Controls.Add(this.button_Mean_Filter);
            this.panel_oper.Controls.Add(this.button_Add_Gauss);
            this.panel_oper.Controls.Add(this.label17);
            this.panel_oper.Controls.Add(this.label20);
            this.panel_oper.Controls.Add(this.label_N_u);
            this.panel_oper.Controls.Add(this.label_Nd);
            this.panel_oper.Controls.Add(this.label_Nk);
            this.panel_oper.Controls.Add(this.label14);
            this.panel_oper.Controls.Add(this.label12);
            this.panel_oper.Controls.Add(this.label_Pb);
            this.panel_oper.Controls.Add(this.label_Pa);
            this.panel_oper.Controls.Add(this.trackBar_Pb);
            this.panel_oper.Controls.Add(this.label19);
            this.panel_oper.Controls.Add(this.trackBar_Pa);
            this.panel_oper.Controls.Add(this.label18);
            this.panel_oper.Controls.Add(this.button_Add_Pepper);


        }
        private void Button_Mean_Filter_Click(object sender, System.EventArgs e)
        {
            Bitmap newBmp;
            ProcessFunctions.MeanFilter(noise_oldBmp, out newBmp);
            this.pictureBox2.Image = ToolFunctions.GetThumbnail((Bitmap)newBmp.Clone(), pictureBox2.Height, pictureBox2.Width) as Image;
            noise_oldBmp = newBmp;


        }
        private void button_Add_Gauss_Click(object sender, EventArgs e)
        {
            label_state.Text = "Processing";
            label_state.BackColor = Color.Yellow;
            Button b = (Button)sender;
            b.Enabled = false;
            Bitmap newBmp;
            ProcessFunctions.AddGaussSalt(noise_oldBmp, out newBmp, new GaussParam( ((float)trackBar_Nu.Value) / 10 , ((float)trackBar_Nd.Value) / 10, trackBar_Nk.Value));
            this.pictureBox2.Image = ToolFunctions.GetThumbnail((Bitmap)newBmp.Clone(), pictureBox2.Height, pictureBox2.Width) as Image;
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
            this.pictureBox2.Image = ToolFunctions.GetThumbnail((Bitmap)newBmp.Clone(), pictureBox2.Height, pictureBox2.Width) as Image;
            noise_oldBmp = newBmp;
            b.Enabled = true;
            label_state.Text = "Finish";
            label_state.BackColor = Color.Green;
        }
        private void Button_Mid_Filter_Click(object sender, System.EventArgs e)
        {
            Bitmap newBmp;
            ProcessFunctions.MedianFilter(noise_oldBmp, out newBmp);
            this.pictureBox2.Image = ToolFunctions.GetThumbnail((Bitmap)newBmp.Clone(), pictureBox2.Height, pictureBox2.Width) as Image;
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
        private void hsiLayout()
        {
            hsiReset();
            pictureBox2.MouseClick += Hsi_PictureBox2_MouseClick;
            Hsi_Oper_Layout();
            Hsi_Datapanel_Layout();
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
            g_data.Clear(Color.White);
            ProcessFunctions.Hue_Calculator(Hsi_nowBmp,out maxHuePixel,ref countHuePixel);
            Graphics g = pictureBox_data.CreateGraphics();

            Pen curPen = new Pen(Brushes.Black, 2);
            g.DrawString("色调直方图", new Font("New Timer", 15), Brushes.Black, new Point(139, 10));
            g.DrawLine(curPen, 50, 240, 350, 240);
            g.DrawLine(curPen, 50, 240, 50, 30);
            g.DrawLine(curPen, 100, 240, 100, 242);
            g.DrawLine(curPen, 150, 240, 150, 242);
            g.DrawLine(curPen, 200, 240, 200, 242);
            g.DrawLine(curPen, 250, 240, 250, 242);
            g.DrawLine(curPen, 300, 240, 300, 242);
            g.DrawLine(curPen, 350, 240, 350, 242);
            g.DrawString("0", new Font("New Timer", 8), Brushes.Black, new PointF(46, 242));
            g.DrawString("60", new Font("New Timer", 8), Brushes.Black, new PointF(92, 242));
            g.DrawString("120", new Font("New Timer", 8), Brushes.Black, new PointF(139, 242));
            g.DrawString("180", new Font("New Timer", 8), Brushes.Black, new PointF(189, 242));
            g.DrawString("240", new Font("New Timer", 8), Brushes.Black, new PointF(239, 242));
            g.DrawString("300", new Font("New Timer", 8), Brushes.Black, new PointF(289, 242));
            g.DrawString("360", new Font("New Timer", 8), Brushes.Black, new PointF(339, 242));
            g.DrawLine(curPen, 48, 40, 50, 40);
            g.DrawString("0", new Font("New Timer", 8), Brushes.Black, new PointF(34, 234));
            g.DrawString(maxHuePixel.ToString(), new Font("New Timer", 8), Brushes.Black, new PointF(18, 34));

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
                g.DrawLine(ContPen, (float)(60 + 3.5 * i), 240, (float)(60 + 3.5 * i), 240 - (int)temp);
            }

            curPen.Dispose();
        }
        private void Hsi_Datapanel_Layout()
        {
            this.panel_adata.Controls.Add(this.label_I);
            this.panel_adata.Controls.Add(this.label16);
            this.panel_adata.Controls.Add(this.label_S);
            this.panel_adata.Controls.Add(this.label13);
            this.panel_adata.Controls.Add(this.label_H);
            this.panel_adata.Controls.Add(this.label11);
        }
        private int HV = 0;
        private int SV = 0;
        private int IV = 0;
        private int HSI_FLAG = 0; //H=1,S=2,I=3
        private Bitmap Hsi_tempBmp;
        private Bitmap Hsi_nowBmp ;
        private void Hsi_Oper_Layout()
        {
            TrackBar tbH = new TrackBar();            
            tbH.Maximum = 10;
            tbH.Minimum = -10;
            tbH.Value = 0;           
            tbH.Location = new Point(100, 50);
            tbH.ValueChanged += Hsi_H_ValueChanged;
            tbH.ValueChanged += new EventHandler((a, b) => Hsi_Datapicturebox_Layout());
            this.panel_oper.Controls.Add(tbH);

            Label label = new Label();
            label.Location = new Point(60,45);
            label.Text = "色调";
            this.panel_oper.Controls.Add(label);

            TrackBar tbS = new TrackBar();
            tbS.Maximum = 9;
            tbS.Minimum = -9;
            tbS.Value = 0;
            tbS.Location = new Point(100, 110);
            tbS.ValueChanged += Hsi_S_ValueChanged;
            this.panel_oper.Controls.Add(tbS);

            Label label2 = new Label();
            label2.Location = new Point(60, 105);
            label2.Text = "饱和度";
            this.panel_oper.Controls.Add(label2);

            TrackBar tbI = new TrackBar();
            tbI.Maximum = 5;
            tbI.Minimum = -5;
            tbI.Value = 0;
            tbI.Location = new Point(100, 170);
            tbI.ValueChanged += Hsi_I_ValueChanged;
            this.panel_oper.Controls.Add(tbI);

            Label label3 = new Label();
            label3.Location = new Point(60, 165);
            label3.Text = "亮度";
            this.panel_oper.Controls.Add(label3);
        }

        
        
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
            this.pictureBox2.Image = ToolFunctions.GetThumbnail((Bitmap)dbitmap.Clone(), pictureBox2.Height, pictureBox2.Width) as Image;
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
            this.pictureBox2.Image = ToolFunctions.GetThumbnail((Bitmap)dbitmap.Clone(), pictureBox2.Height, pictureBox2.Width) as Image;
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
            this.pictureBox2.Image = ToolFunctions.GetThumbnail((Bitmap)dbitmap.Clone(), pictureBox2.Height, pictureBox2.Width) as Image;
            Hsi_nowBmp = dbitmap;
        }
        #endregion
        #region//etc部分
        private void etcLayout()
        {
            etcReset();
            noDataInDataBox();
            noDataInDataPanel();
            etc_Oper_Layout();
        }

        private void etc_Oper_Layout()
        {
            this.panel_oper.Controls.Add(this.label_DV);
            this.panel_oper.Controls.Add(this.label21);
            this.panel_oper.Controls.Add(this.trackBar_DV);
            this.panel_oper.Controls.Add(this.button_TwoD);
            this.panel_oper.Controls.Add(this.button_Sharpen);
            this.panel_oper.Controls.Add(this.label_SV);
            this.panel_oper.Controls.Add(this.label22);
            this.panel_oper.Controls.Add(this.trackBar_SV);
        }
        private void etcReset()
        {

        }
        private void button_Sharpen_Click(object sender, EventArgs e)
        {
            Bitmap sharpenBmp;
            sharpenBmp=ProcessFunctions.SharpenFilter2(bitmap, trackBar_SV.Value);
            this.pictureBox2.Image = ToolFunctions.GetThumbnail((Bitmap)sharpenBmp.Clone(), pictureBox2.Height, pictureBox2.Width) as Image;

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
            this.pictureBox2.Image = ToolFunctions.GetThumbnail((Bitmap)twoVBmp.Clone(), pictureBox2.Height, pictureBox2.Width) as Image;
        }
        #endregion
        #region //grey部分
        private System.Drawing.Bitmap bmpHist;
        private int[] countGreyPixel = new int[256];
        private int maxGreyPixel=0;

        private void greyReset()
        {
            bmpHist = bitmap;
            countGreyPixel = new int[256];
            maxGreyPixel = 0;
        }
        private void Grey_Datagraphic_Layout()
        {
            //画出坐标系
            Graphics g = pictureBox_data.CreateGraphics();

            Pen curPen = new Pen(Brushes.Black, 1);
            g.DrawString("灰度直方图", new Font("New Timer", 15), Brushes.Black, new Point(139, 10));
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
            g.DrawString(maxGreyPixel.ToString(), new Font("New Timer", 8), Brushes.Black, new PointF(18, 34));

            double temp = 0;
            for (int i = 0; i < 256; i++)
            {
                temp = 200.0 * countGreyPixel[i] / maxGreyPixel;
                g.DrawLine(curPen, 50 + i, 240, 50 + i, 240 - (int)temp);
            }

            curPen.Dispose();
        }
        private void Grey_Operate_Layout()
        {
            Button button = new Button();
            button.Text = "直方图均衡化";
            button.Location = new Point(50, 50);
            button.Click += Get_Balance_Click;
            this.panel_oper.Controls.Add(button);

            TrackBar tb = new TrackBar();
            tb.ValueChanged += Tb_ValueChanged;
            tb.Maximum = 10;
            tb.Minimum = -10;
            tb.Value = 0;
            tb.Location = new Point(100, 100);
            this.panel_oper.Controls.Add(tb);
        }

        private void Tb_ValueChanged(object sender, EventArgs e)
        {
            TrackBar self = (TrackBar)sender;
            Bitmap dbitmap;
            ProcessFunctions.Grey_Change(bmpHist, out dbitmap, self.Value);
            this.pictureBox2.Image = ToolFunctions.GetThumbnail((Bitmap)dbitmap.Clone(), pictureBox2.Height, pictureBox2.Width) as Image;
            this.pictureBox_data.CreateGraphics().Clear(Color.White);
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
            this.pictureBox2.Image = ToolFunctions.GetThumbnail((Bitmap)dbitmap.Clone(), pictureBox2.Height, pictureBox2.Width) as Image;
            this.pictureBox_data.CreateGraphics().Clear(Color.White);
            ProcessFunctions.Grey_Calculator(dbitmap, out maxGreyPixel, ref countGreyPixel);
            Grey_Datagraphic_Layout();
        }

        private void greyLayout()
        {
            bmpHist = bitmap;
            ProcessFunctions.Grey_Calculator(bmpHist, out maxGreyPixel, ref countGreyPixel);
            Grey_Datagraphic_Layout();
            Grey_Operate_Layout();
            Grey_Datapanel_Layout();
            this.pictureBox2.MouseClick += PictureBox2_MouseClick_GetColor;
        }
        private void Grey_Datapanel_Layout()
        {
            this.panel_adata.Controls.Add(this.pictureBox_eg);
            this.panel_adata.Controls.Add(this.label_Grey);
            this.panel_adata.Controls.Add(this.label15);
            this.panel_adata.Controls.Add(this.label_B);
            this.panel_adata.Controls.Add(this.label_G);
            this.panel_adata.Controls.Add(this.label_R);
            this.panel_adata.Controls.Add(this.label10);
            this.panel_adata.Controls.Add(this.label9);
            this.panel_adata.Controls.Add(this.label1);
        }
        private bool GetColor(MouseEventArgs e,out Color c)
        {
            int bw = bitmap.Width * pictureBox2.Height / bitmap.Height;
            int pw = pictureBox2.Width;
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
            c=ProcessFunctions.GetColor(MousePosition);
            return true;
        }
        private void PictureBox2_MouseClick_GetColor(object sender, MouseEventArgs e)
        {

            Color c;
            if(GetColor(e,out c) == false)
            {
                return;
            }
            label_R.Text = c.R.ToString();
            label_G.Text = c.G.ToString();
            label_B.Text = c.B.ToString();
            pictureBox_eg.BackColor = c;
            double Grey = c.R * 0.299 + c.G * 0.587 + c.B * 0.114;
            label_Grey.Text = Grey.ToString();


        }
        #endregion
        #region//Paint部分
        TrackBar paint_trackBar;
        Panel paint_panel;
        Panel paint_panel2;
        Graphics g_paint_temp;
        Graphics g_paint_temp2;
        private void paintLayout()
        {
            noDataInDataBox();
            noDataInDataPanel();

            Button btn = new Button();
            btn.Width += 20;
            btn.Text = "选择画笔颜色";
            btn.Click += Btn_Click;           
            btn.Location = new Point(50, 50);
            panel_oper.Controls.Add(btn);

            pictureBox2.MouseDown += pictureBox2_MouseDown;
            pictureBox2.MouseMove += pictureBox2_MouseMove;
            pictureBox2.MouseUp += pictureBox2_MouseUp;

            Label label = new Label();
            label.Text = "粗细:";
            label.Location = new Point(50, 110);
            label.Width = 40;
            panel_oper.Controls.Add(label);

            paint_trackBar = new TrackBar();
            paint_trackBar.Maximum = 10;
            paint_trackBar.Minimum = 1;
            paint_trackBar.Value = 1;
            paint_trackBar.Location = new Point(100, 100);
            paint_trackBar.ValueChanged += TrackBar_ValueChanged;
            panel_oper.Controls.Add(paint_trackBar);

            paint_panel = new Panel();
            paint_panel.Width = 60;
            paint_panel.Height = 10;
            paint_panel.Location = new Point(110, 150);
            panel_oper.Controls.Add(paint_panel);
            g_paint_temp = paint_panel.CreateGraphics();
            g_paint_temp.Clear(Color.White);
            g_paint_temp.DrawLine(new Pen(Brushes.Black, paint_trackBar.Value), new Point(5, 5), new Point(50,5));

            paint_panel2 = new Panel();
            paint_panel2.Location = new Point(150,50);
            paint_panel2.Width = 20;
            paint_panel2.Height = 20;
            panel_oper.Controls.Add(paint_panel2);
            g_paint_temp2 = paint_panel2.CreateGraphics();
            g_paint_temp2.Clear(choose_color);

        }

        private void TrackBar_ValueChanged(object sender, EventArgs e)
        {
            g_paint_temp.Clear(Color.White);
            g_paint_temp.DrawLine(new Pen(Brushes.Black, paint_trackBar.Value), new Point(5, 5), new Point(50, 5));
        }
        
        private void paintReset()
        {
            choose_color = Color.Red;
            in_box_flag = 0;
            lastP = new Point(-1, -1);
        }
        

        #region //涂鸦的颜色选择板
        private Color choose_color = Color.Red;
        private void Btn_Click(object sender, EventArgs e)
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
        private void pictureBox2_MouseEnter(object sender, MouseEventArgs e)
        {
           
        }
        private void pictureBox2_MouseDown(object sender, MouseEventArgs e)
        {
            in_box_flag = 1;
        }

        private Point lastP = new Point(-1, -1);
        private void myDrawLine(Pen pen, Point p1, Point p2)
        {
            g2.DrawLine(pen, p1, p2);
            gReal.DrawLine(pen, p1, p2);
        }
        private void pictureBox2_MouseMove(object sender, MouseEventArgs e)
        {
            if(isChoosed==false)
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

                    Pen pen = new Pen(solidBrush,paint_trackBar.Value);


                    myDrawLine(pen, lastP, e.Location);

                    lastP = e.Location;
                }


            }
        }

        private void pictureBox2_MouseUp(object sender, MouseEventArgs e)
        {
            lastP = new Point(-1, -1);
            in_box_flag = 0;

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
        //clean button
        private void button2_Click_1(object sender, EventArgs e)
        {
            if (isChoosed == false)
            {
                return;
            }
            pictureBox2.Image = ToolFunctions.GetThumbnail((Bitmap)bitmap.Clone(), pictureBox2.Height, pictureBox2.Width) as Image;            
            gReal = Graphics.FromImage(pictureBox2.Image);
        }
        //get button
        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                isChoosed = true;
                
                string path = openFileDialog1.FileName;
                bitmap = (Bitmap)Image.FromFile(path);
                reset();
                pictureBox1.Image = ToolFunctions.GetThumbnail((Bitmap)bitmap.Clone(), pictureBox1.Height, pictureBox1.Width) as Image;
                pictureBox2.Image = ToolFunctions.GetThumbnail((Bitmap)bitmap.Clone(), pictureBox2.Height, pictureBox2.Width) as Image;
                //pictureBox2.Width = pictureBox2.Image.Width;
                //pictureBox2.Height = pictureBox2.Image.Height;
                gReal = Graphics.FromImage(pictureBox2.Image);


            }
        }

        #endregion

        private void panel_oper_Paint(object sender, PaintEventArgs e)
        {

        }


    }
}

