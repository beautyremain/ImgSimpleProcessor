using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ImgProcessor
{
    public class ProcessFunctions
    {
        public static void AddPepperSalt(Bitmap srcBmp, double Pa, double Pb, out Bitmap dstBmp)
        {
            // Bitmap pic = (Bitmap)Bitmap.FromFile(filename, false);
            dstBmp = srcBmp;
            double P = Pb / (1 - Pa);//程序要,为了得到一个概率Pb事件
            int width = dstBmp.Width;
            int height = dstBmp.Height;
            Random rand = new Random();
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    Color noiseC = dstBmp.GetPixel(j, i);
                    double probility = rand.NextDouble();
                    if (probility < Pa)
                    {
                        noiseC = Color.White;//有Pa概率 噪声设为最大值
                    }
                    else
                    {
                        double temp = rand.NextDouble();
                        if (temp < P)//有1 - Pa的几率到达这里，再乘以 P ，刚好等于Pb
                            noiseC = Color.Black;
                    }

                    Color color = noiseC;
                    dstBmp.SetPixel(j, i, color);
                }
            }
        }
        public static void AddGaussSalt(Bitmap srcBmp, out Bitmap dstBmp,GaussParam gp)
        {
            // Bitmap pic = (Bitmap)Bitmap.FromFile(filename, false);
            dstBmp = srcBmp;
            int width = dstBmp.Width;
            int height = dstBmp.Height;
            
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    int noise = (int) (GaussNoise(gp.u,gp.d)*gp.k);
                    Color noiseC = dstBmp.GetPixel(j, i);                    
                    Color color = Color.FromArgb(valChange(noiseC.R,noise), valChange(noiseC.G,noise), valChange(noiseC.B,noise));
                    dstBmp.SetPixel(j, i, color);
                }
            }
        }
        private static Byte getMid(List<byte> list)
        {
            list.Sort();
            return list[list.Count / 2];
        }
        private static Byte getMean(List<byte> list)
        {
            int sum = 0;
            foreach (byte item in list)
            {
                sum += item;
            }
            return (Byte)(sum / list.Count);
        }
        private static Byte getLaplace(List<byte> list)
        {
            int sum = 0;
            int[] Laplacian = {1,1,1,1,-8,1,1,1,1};
            for (int i = 0; i < 9; i++)
            {
                int a = i < list.Count ? list[i] : 0;

                sum += Laplacian[i] * a;
            }
            return valChange((Byte)(sum));
        }
        public static Bitmap SharpenFilter2(Bitmap b, float val)
        {
            if (b == null)
            {
                return null;
            }

            int w = b.Width;
            int h = b.Height;

            try
            {

                Bitmap bmpRtn = new Bitmap(w, h, PixelFormat.Format24bppRgb);

                BitmapData srcData = b.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                BitmapData dstData = bmpRtn.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

                unsafe
                {
                    byte* pIn = (byte*)srcData.Scan0.ToPointer();
                    byte* pOut = (byte*)dstData.Scan0.ToPointer();
                    int stride = srcData.Stride;
                    byte* p;

                    for (int y = 0; y < h; y++)
                    {
                        for (int x = 0; x < w; x++)
                        {
                            //取周围9点的值。位于边缘上的点不做改变。
                            if (x == 0 || x == w - 1 || y == 0 || y == h - 1)
                            {
                                //不做
                                pOut[0] = pIn[0];
                                pOut[1] = pIn[1];
                                pOut[2] = pIn[2];
                            }
                            else
                            {
                                int r1, r2, r3, r4, r5, r6, r7, r8, r0;
                                int g1, g2, g3, g4, g5, g6, g7, g8, g0;
                                int b1, b2, b3, b4, b5, b6, b7, b8, b0;

                                float vR, vG, vB;

                                //左上
                                p = pIn - stride - 3;
                                r1 = p[2];
                                g1 = p[1];
                                b1 = p[0];

                                //正上
                                p = pIn - stride;
                                r2 = p[2];
                                g2 = p[1];
                                b2 = p[0];

                                //右上
                                p = pIn - stride + 3;
                                r3 = p[2];
                                g3 = p[1];
                                b3 = p[0];

                                //左侧
                                p = pIn - 3;
                                r4 = p[2];
                                g4 = p[1];
                                b4 = p[0];

                                //右侧
                                p = pIn + 3;
                                r5 = p[2];
                                g5 = p[1];
                                b5 = p[0];

                                //右下
                                p = pIn + stride - 3;
                                r6 = p[2];
                                g6 = p[1];
                                b6 = p[0];

                                //正下
                                p = pIn + stride;
                                r7 = p[2];
                                g7 = p[1];
                                b7 = p[0];

                                //右下
                                p = pIn + stride + 3;
                                r8 = p[2];
                                g8 = p[1];
                                b8 = p[0];

                                //自己
                                p = pIn;
                                r0 = p[2];
                                g0 = p[1];
                                b0 = p[0];

                                vR = (float)r0 - (float)(r1 + r2 + r3 + r4 + r5 + r6 + r7 + r8) / 8;
                                vG = (float)g0 - (float)(g1 + g2 + g3 + g4 + g5 + g6 + g7 + g8) / 8;
                                vB = (float)b0 - (float)(b1 + b2 + b3 + b4 + b5 + b6 + b7 + b8) / 8;

                                vR = r0 + vR * val;
                                vG = g0 + vG * val;
                                vB = b0 + vB * val;

                                if (vR > 0)
                                {
                                    vR = Math.Min(255, vR);
                                }
                                else
                                {
                                    vR = Math.Max(0, vR);
                                }

                                if (vG > 0)
                                {
                                    vG = Math.Min(255, vG);
                                }
                                else
                                {
                                    vG = Math.Max(0, vG);
                                }

                                if (vB > 0)
                                {
                                    vB = Math.Min(255, vB);
                                }
                                else
                                {
                                    vB = Math.Max(0, vB);
                                }

                                pOut[0] = (byte)vB;
                                pOut[1] = (byte)vG;
                                pOut[2] = (byte)vR;

                            }

                            pIn += 3;
                            pOut += 3;
                        }// end of x

                        pIn += srcData.Stride - w * 3;
                        pOut += srcData.Stride - w * 3;
                    } // end of y
                }

                b.UnlockBits(srcData);
                bmpRtn.UnlockBits(dstData);

                return bmpRtn;
            }
            catch
            {
                return null;
            }

        }   //  end of KiSharpen

        public static bool TwoDivision_Change(Bitmap srcBmp, out Bitmap dstBmp, int level)
        {
            int w = grey_change_weight;
            if (srcBmp == null)
            {
                dstBmp = null;
                return false;
            }
            dstBmp = new Bitmap(srcBmp);
            Rectangle rt = new Rectangle(0, 0, srcBmp.Width, srcBmp.Height);
            BitmapData bmpData = dstBmp.LockBits(rt, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            unsafe
            {

                for (int i = 0; i < bmpData.Height; i++)
                {
                    byte* ptr = (byte*)bmpData.Scan0 + i * bmpData.Stride;
                    for (int j = 0; j < bmpData.Width; j++)
                    {
                        int grey = (int)(0.299 * *(ptr + j * 3 + 1) + 0.587 * *(ptr + j * 3 + 2) + 0.114 * *(ptr + j * 3));
                        if (grey > level)
                            grey = 255;
                        else
                            grey = 0;
                        *(ptr + j * 3) = (byte)grey; //B
                        *(ptr + j * 3 + 1) = (byte)grey;//G
                        *(ptr + j * 3 + 2) = (byte)grey; //R
                    }
                }
            }
            dstBmp.UnlockBits(bmpData);
            return true;
        }
        public static void SharpenFilter1(Bitmap srcBmp, out Bitmap dstBmp)
        {
            dstBmp = new Bitmap(srcBmp);
            Bitmap oriBmp = (Bitmap)dstBmp.Clone();
            Rectangle rt = new Rectangle(0, 0, srcBmp.Width, srcBmp.Height);
            BitmapData bmpData = dstBmp.LockBits(rt, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            BitmapData bmpData2 = srcBmp.LockBits(rt, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int width = bmpData.Width;
            int height = bmpData.Height;
            unsafe
            {

                for (int i = 0; i < bmpData.Height; i++)
                {
                    byte* ptr = (byte*)bmpData.Scan0 + i * bmpData.Stride;
                    for (int j = 0; j < bmpData.Width; j++)
                    {
                        List<byte> adjoinR = new List<byte>();
                        List<byte> adjoinG = new List<byte>();
                        List<byte> adjoinB = new List<byte>();
                        for (int ii = -1; ii < 2; ii++)
                        {
                            for (int jj = -1; jj < 2; jj++)
                            {
                                if (j + jj >= 0 && j + jj < width && i + ii >= 0 && i + ii < height)
                                {
                                    adjoinB.Add(*((byte*)bmpData2.Scan0 + (i + ii) * bmpData2.Stride + 3 * (j + jj)));
                                    adjoinG.Add(*((byte*)bmpData2.Scan0 + (i + ii) * bmpData2.Stride + 3 * (j + jj) + 1));
                                    adjoinR.Add(*((byte*)bmpData2.Scan0 + (i + ii) * bmpData2.Stride + 3 * (j + jj) + 2));
                                }
                            }
                        }
                        *(ptr + j * 3) += getLaplace(adjoinB); //B
                        *(ptr + j * 3 + 1) += getLaplace(adjoinG);//G
                        *(ptr + j * 3 + 2) += getLaplace(adjoinR); //R
                    }
                }
            }
            dstBmp.UnlockBits(bmpData);
            srcBmp.UnlockBits(bmpData2);

        }
        public static void MeanFilter(Bitmap srcBmp, out Bitmap dstBmp)
        {
            dstBmp = new Bitmap(srcBmp);
            Bitmap oriBmp = (Bitmap)dstBmp.Clone();
            Rectangle rt = new Rectangle(0, 0, srcBmp.Width, srcBmp.Height);
            BitmapData bmpData = dstBmp.LockBits(rt, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            BitmapData bmpData2 = srcBmp.LockBits(rt, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int width = bmpData.Width;
            int height = bmpData.Height;
            unsafe
            {

                for (int i = 0; i < bmpData.Height; i++)
                {
                    byte* ptr = (byte*)bmpData.Scan0 + i * bmpData.Stride;
                    for (int j = 0; j < bmpData.Width; j++)
                    {
                        List<byte> adjoinR = new List<byte>();
                        List<byte> adjoinG = new List<byte>();
                        List<byte> adjoinB = new List<byte>();
                        for (int ii = -1; ii < 2; ii++)
                        {
                            for (int jj = -1; jj < 2; jj++)
                            {
                                if (j + jj >= 0 && j + jj < width && i + ii >= 0 && i + ii < height)
                                {
                                    adjoinB.Add(*((byte*)bmpData2.Scan0 + (i + ii) * bmpData2.Stride + 3 * (j + jj)));
                                    adjoinG.Add(*((byte*)bmpData2.Scan0 + (i + ii) * bmpData2.Stride + 3 * (j + jj) + 1));
                                    adjoinR.Add(*((byte*)bmpData2.Scan0 + (i + ii) * bmpData2.Stride + 3 * (j + jj) + 2));
                                }
                            }
                        }
                        *(ptr + j * 3) = getMean(adjoinB); //B
                        *(ptr + j * 3 + 1) = getMean(adjoinG);//G
                        *(ptr + j * 3 + 2) = getMean(adjoinR); //R
                    }
                }
            }
            dstBmp.UnlockBits(bmpData);
            srcBmp.UnlockBits(bmpData2);

        }
        public static void MedianFilter(Bitmap srcBmp, out Bitmap dstBmp)
        {
            dstBmp = new Bitmap(srcBmp);
            Bitmap oriBmp = (Bitmap)dstBmp.Clone();         
            Rectangle rt = new Rectangle(0, 0, srcBmp.Width, srcBmp.Height);
            BitmapData bmpData = dstBmp.LockBits(rt, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            BitmapData bmpData2 = srcBmp.LockBits(rt, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int width = bmpData.Width;
            int height = bmpData.Height;
            unsafe
            {

                for (int i = 0; i < bmpData.Height; i++)
                {
                    byte* ptr = (byte*)bmpData.Scan0 + i * bmpData.Stride;
                    for (int j = 0; j < bmpData.Width; j++)
                    {
                        List<byte> adjoinR = new List<byte>();
                        List<byte> adjoinG = new List<byte>();
                        List<byte> adjoinB = new List<byte>();
                        for (int ii = -1; ii < 2; ii++)
                        {
                            for (int jj = -1; jj < 2; jj++)
                            {
                                if (j + jj >= 0 && j + jj < width && i + ii >= 0 && i + ii < height)
                                {
                                    adjoinB.Add(*((byte*)bmpData2.Scan0 + (i + ii) * bmpData2.Stride + 3 * (j + jj)));
                                    adjoinG.Add(*((byte*)bmpData2.Scan0 + (i + ii) * bmpData2.Stride + 3 * (j + jj)+1));
                                    adjoinR.Add(*((byte*)bmpData2.Scan0 + (i + ii) * bmpData2.Stride + 3 * (j + jj)+2));
                                }
                            }
                        }
                        *(ptr + j * 3) = getMid(adjoinB); //B
                        *(ptr + j * 3 + 1) = getMid(adjoinG);//G
                        *(ptr + j * 3 + 2) = getMid(adjoinR); //R
                    }
                }
            }
            dstBmp.UnlockBits(bmpData);
            srcBmp.UnlockBits(bmpData2);

        }

        public static double GaussNoise(double u, double d)
        {
            double u1, u2, z, x;
            //Random ram = new Random();
            if (d <= 0)
            {

                return u;
            }
            u1 = (new Random(ToolFunctions.GetRandomSeed())).NextDouble();
            u2 = (new Random(ToolFunctions.GetRandomSeed())).NextDouble();

            z = Math.Sqrt(-2 * Math.Log(u1)) * Math.Sin(2 * Math.PI * u2);

            x = u + d * z;
            return x;

        }


        public static Color Hsl_Pixel_Change(Color c, string type, int level)
        {
            float w;
            if (type == "h")
            {
                w = 18;
                float H = (c.GetHue() + 360 + level * w) % 360;
                //Console.WriteLine(H + "," + c.GetHue());
                return HSL2RGB(H, c.GetSaturation(), c.GetBrightness());
            }
            else if (type == "s")
            {
                float S;
                float old = c.GetSaturation();
                if (level < 0)
                {
                    S = old / 10 * level + old;
                }
                else
                {
                    S = (1 - old) / 10 * level + old;
                }
                return HSL2RGB(c.GetHue(), S, c.GetBrightness());
            }
            else
            {

                float L;
                float old = c.GetBrightness();
                if (level < 0)
                {
                    L = old / 10 * level + old;
                }
                else
                {
                    L = (1 - old) / 10 * level + old;
                }
                if (L >= 1)
                {
                    Console.WriteLine("L>1");
                }
                return HSL2RGB(c.GetHue(), c.GetSaturation(), L);
            }
        }
        public static bool Hsl_Change(string type, int level, Bitmap srcBmp, out Bitmap dstBmp)
        {
            if (srcBmp == null)
            {
                dstBmp = null;
                return false;
            }
            dstBmp = new Bitmap(srcBmp);
            Rectangle rt = new Rectangle(0, 0, srcBmp.Width, srcBmp.Height);
            BitmapData bmpData = dstBmp.LockBits(rt, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            unsafe
            {

                for (int i = 0; i < bmpData.Height; i++)
                {
                    byte* ptr = (byte*)bmpData.Scan0 + i * bmpData.Stride;
                    for (int j = 0; j < bmpData.Width; j++)
                    {
                        Color c = Color.FromArgb(*(ptr + j * 3 + 2), *(ptr + j * 3 + 1), *(ptr + j * 3));
                        Color nc = Hsl_Pixel_Change(c, type, level);

                        *(ptr + j * 3) = nc.B; //B
                        *(ptr + j * 3 + 1) = nc.G;//G
                        *(ptr + j * 3 + 2) = nc.R; //R
                    }
                }
            }
            dstBmp.UnlockBits(bmpData);
            return true;
        }
        public static Color HSL2RGB(double h, double sl, double l)
        {
            h /= 360;
            double v;
            double r, g, b;
            r = l;   // default to gray
            g = l;
            b = l;
            v = (l <= 0.5) ? (l * (1.0 + sl)) : (l + sl - l * sl);
            if (v > 0)
            {
                double m;
                double sv;
                int sextant;
                double fract, vsf, mid1, mid2;
                m = l + l - v;
                sv = (v - m) / v;
                h *= 6.0;
                sextant = (int)h;
                fract = h - sextant;
                vsf = v * sv * fract;
                mid1 = m + vsf;
                mid2 = v - vsf;
                switch (sextant)
                {
                    case 0:
                        r = v;
                        g = mid1;
                        b = m;
                        break;
                    case 1:
                        r = mid2;
                        g = v;
                        b = m;
                        break;
                    case 2:
                        r = m;
                        g = v;
                        b = mid1;
                        break;
                    case 3:
                        r = m;
                        g = mid2;
                        b = v;
                        break;
                    case 4:
                        r = mid1;
                        g = m;
                        b = v;
                        break;
                    case 5:
                        r = v;
                        g = m;
                        b = mid2;
                        break;
                }
            }
            Color rgb = Color.FromArgb(Convert.ToByte(r * 255.0f), Convert.ToByte(g * 255.0f), Convert.ToByte(b * 255.0f));
            return rgb;

        }
        #region //getColor prepare
        public static int grey_change_weight = 10;
        [DllImport("gdi32.dll")]
        private static extern uint GetPixel(IntPtr hDC, int XPos, int YPos);
        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateDC(string driverName, string deviceName, string output, IntPtr lpinitData);
        [DllImport("gdi32.dll")]
        private static extern bool DeleteDC(IntPtr DC);
        private static byte GetRValue(uint color)
        {
            return (byte)color;
        }
        private static byte GetGValue(uint color)
        {
            return ((byte)(((short)(color)) >> 8));
        }
        private static byte GetBValue(uint color)
        {
            return ((byte)((color) >> 16));
        }
        private static byte GetAValue(uint color)
        {
            return ((byte)((color) >> 24));
        }
        #endregion
        public static Color GetColor(Point screenPoint)
        {
            IntPtr displayDC = CreateDC("DISPLAY", null, null, IntPtr.Zero);
            uint colorref = GetPixel(displayDC, screenPoint.X, screenPoint.Y);
            DeleteDC(displayDC);
            byte R = GetRValue(colorref);
            byte G = GetGValue(colorref);
            byte B = GetBValue(colorref);
            Console.WriteLine("Point:(" + screenPoint.X + "," + screenPoint.Y + ") RGB:(" + R + "," + G + "," + B + ")");
            return Color.FromArgb(R, G, B);
        }
        private static byte valChange(int o, int a=0)
        {
            int temp = o + a;
            if (temp > 255)
            {
                return 255;
            }
            else if (temp < 0)
            {
                return 0;
            }
            else
            {
                return (byte)temp;
            }
        }
        public static bool Grey_Change(Bitmap srcBmp, out Bitmap dstBmp, int level)
        {
            int w = grey_change_weight;
            if (srcBmp == null)
            {
                dstBmp = null;
                return false;
            }
            dstBmp = new Bitmap(srcBmp);
            Rectangle rt = new Rectangle(0, 0, srcBmp.Width, srcBmp.Height);
            BitmapData bmpData = dstBmp.LockBits(rt, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            unsafe
            {

                for (int i = 0; i < bmpData.Height; i++)
                {
                    byte* ptr = (byte*)bmpData.Scan0 + i * bmpData.Stride;
                    for (int j = 0; j < bmpData.Width; j++)
                    {
                        *(ptr + j * 3) = valChange(*(ptr + j * 3), w * level); //B
                        *(ptr + j * 3 + 1) = valChange(*(ptr + j * 3 + 1), w * level);//G
                        *(ptr + j * 3 + 2) = valChange(*(ptr + j * 3 + 2), w * level); //R
                    }
                }
            }
            dstBmp.UnlockBits(bmpData);
            return true;
        }
        public static void Hue_Calculator(Bitmap srcBmp, out int maxHuePixel, ref int[] countHuePixel)
        {
            maxHuePixel = 1;
            Rectangle rect = new Rectangle(0, 0, srcBmp.Width, srcBmp.Height);
            System.Drawing.Imaging.BitmapData bmpData = srcBmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, srcBmp.PixelFormat);
            //IntPtr ptr = bmpData.Scan0;

            int bytes = srcBmp.Width * srcBmp.Height;
            countHuePixel = new int[73];
            unsafe
            {

                for (int i = 0; i < bmpData.Height; i++)
                {
                    byte* ptr = (byte*)bmpData.Scan0 + i * bmpData.Stride;
                    for (int j = 0; j < bmpData.Width; j++)
                    {

                        Color c = Color.FromArgb(*(ptr + j * 3 + 2), *(ptr + j * 3 + 1), *(ptr + j * 3));
                        float H = c.GetHue();
                        float S = c.GetSaturation();
                        float L = c.GetBrightness();
                        if (S < 0.05)//去除黑白色
                        {
                            continue;
                        }
                        countHuePixel[(int)H / 5]++;
                        if (countHuePixel[(int)H / 5] > maxHuePixel)
                        {
                            maxHuePixel = countHuePixel[(int)H / 5];
                        }

                    }
                }
            }


            srcBmp.UnlockBits(bmpData);
        }
        public static void Grey_Calculator(Bitmap bmpHist, out int maxPixel, ref int[] countPixel)
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
            //Console.WriteLine(countPixel[0]);

        }
        /// <summary>
        /// 直方图均衡化 直方图均衡化就是对图像进行非线性拉伸，重新分配图像像素值，使一定灰度范围内的像素数量大致相同
        /// 增大对比度，从而达到图像增强的目的。是图像处理领域中利用图像直方图对对比度进行调整的方法
        /// </summary>
        /// <param name="srcBmp">原始图像</param>
        /// <param name="dstBmp">处理后图像</param>
        /// <returns>处理成功 true 失败 false</returns>
        public static bool Balance(Bitmap srcBmp, out Bitmap dstBmp)
        {
            if (srcBmp == null)
            {
                dstBmp = null;
                return false;
            }
            int[] histogramArrayR = new int[256];//各个灰度级的像素数R
            int[] histogramArrayG = new int[256];//各个灰度级的像素数G
            int[] histogramArrayB = new int[256];//各个灰度级的像素数B
            int[] tempArrayR = new int[256];
            int[] tempArrayG = new int[256];
            int[] tempArrayB = new int[256];
            byte[] pixelMapR = new byte[256];
            byte[] pixelMapG = new byte[256];
            byte[] pixelMapB = new byte[256];
            dstBmp = new Bitmap(srcBmp);
            Rectangle rt = new Rectangle(0, 0, srcBmp.Width, srcBmp.Height);
            BitmapData bmpData = dstBmp.LockBits(rt, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            unsafe
            {
                //统计各个灰度级的像素个数
                for (int i = 0; i < bmpData.Height; i++)
                {
                    byte* ptr = (byte*)bmpData.Scan0 + i * bmpData.Stride;
                    for (int j = 0; j < bmpData.Width; j++)
                    {
                        histogramArrayB[*(ptr + j * 3)]++;
                        histogramArrayG[*(ptr + j * 3 + 1)]++;
                        histogramArrayR[*(ptr + j * 3 + 2)]++;
                    }
                }
                //计算各个灰度级的累计分布函数
                for (int i = 0; i < 256; i++)
                {
                    if (i != 0)
                    {
                        tempArrayB[i] = tempArrayB[i - 1] + histogramArrayB[i];
                        tempArrayG[i] = tempArrayG[i - 1] + histogramArrayG[i];
                        tempArrayR[i] = tempArrayR[i - 1] + histogramArrayR[i];
                    }
                    else
                    {
                        tempArrayB[0] = histogramArrayB[0];
                        tempArrayG[0] = histogramArrayG[0];
                        tempArrayR[0] = histogramArrayR[0];
                    }
                    //计算累计概率函数，并将值放缩至0~255范围内
                    pixelMapB[i] = (byte)(255.0 * tempArrayB[i] / (bmpData.Width * bmpData.Height) + 0.5);//加0.5为了四舍五入取整
                    pixelMapG[i] = (byte)(255.0 * tempArrayG[i] / (bmpData.Width * bmpData.Height) + 0.5);
                    pixelMapR[i] = (byte)(255.0 * tempArrayR[i] / (bmpData.Width * bmpData.Height) + 0.5);
                }
                //映射转换
                for (int i = 0; i < bmpData.Height; i++)
                {
                    byte* ptr = (byte*)bmpData.Scan0 + i * bmpData.Stride;
                    for (int j = 0; j < bmpData.Width; j++)
                    {
                        *(ptr + j * 3) = pixelMapB[*(ptr + j * 3)];
                        *(ptr + j * 3 + 1) = pixelMapG[*(ptr + j * 3 + 1)];
                        *(ptr + j * 3 + 2) = pixelMapR[*(ptr + j * 3 + 2)];
                    }
                }
            }
            dstBmp.UnlockBits(bmpData);
            return true;
        }
    }
}
