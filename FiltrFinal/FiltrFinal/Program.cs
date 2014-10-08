using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;
using System.Threading.Tasks;


namespace FiltrFinal
{

    class Program
    {
        public class Obertka
        {
            //проверять размер картинки
            public byte[] Filtr(byte[] inmass, int iheight, int iwidth, bool normtype)
            {
                int[] intrgb = new int[inmass.Length / 3];
                if (!normtype)
                {
                    for (Int64 i = 0; i < inmass.Length; i += 3)
                    {
                        //intrgb[i / 3] = Color.FromArgb(inmass[i], inmass[i + 1], inmass[i+2]).ToArgb();
                        intrgb[i / 3] = Color.FromArgb(inmass[i+2], inmass[i +1], inmass[i]).ToArgb();
                    }
                }
                byte[] outmass = new byte[inmass.Length];
                //заполняем края картинки
                if (normtype)
                {
                    for (int i = 0; i < iwidth; i++)
                    {
                        outmass[i] = inmass[i];
                        outmass[outmass.Length - 1 - i] = inmass[outmass.Length - 1 - i];
                    }
                    for (int i = 0; i < iheight; i++)
                    {
                        outmass[i * iwidth] = inmass[i * iwidth];
                        if (i > 0)
                            outmass[i * iwidth - 1] = inmass[i * iwidth - 1];
                    }
                }
                else
                {
                    for (int i = 0; i < iwidth; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            outmass[3 * i + j] = inmass[3 * i + j];
                            outmass[outmass.Length - (j + 1) - 3 * i] = inmass[outmass.Length - (j + 1) - 3 * i];
                        }
                    }
                    for (int i = 0; i < iheight; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            outmass[3 * i * iwidth + j] = inmass[3 * i * iwidth + j];
                            if (i > 0)
                                outmass[3 * i * iwidth - (j + 1)] = inmass[3 * i * iwidth - (j + 1)];
                        }
                    }
                }
                //for (int i = 0; i < iwidth - 2; i++)
                Parallel.For(0, iwidth - 2, i =>
                    {
                        int[] tmpmass = new int[9];
                        object b;
                        for (int j = 0; j < iheight - 2; j++)
                        {
                            for (int i1 = 0; i1 < 3; i1++)
                                for (int j1 = 0; j1 < 3; j1++)
                                {
                                    if (normtype)
                                    {
                                        tmpmass[i1 * 3 + j1] = inmass[i + j1 + (i1 + j) * iwidth]; //для байтов
                                    }
                                    else
                                    {
                                        tmpmass[i1 * 3 + j1] = intrgb[i + j1 + (i1 + j) * iwidth]; //для цветов
                                    }
                                }
                            b = GetB(tmpmass);
                            if (normtype)
                            {
                                outmass[(i + 1) + (j + 1) * iwidth] = Convert.ToByte(b); //для байтов
                            }
                            else
                            {
                                outmass[(i + 1) * 3 + (j + 1) * iwidth * 3] = Color.FromArgb((int)b).B; //для цветов
                                outmass[(i + 1) * 3 + (j + 1) * iwidth * 3 + 1] = Color.FromArgb((int)b).G; //для цветов
                                outmass[(i + 1) * 3 + (j + 1) * iwidth * 3 + 2] = Color.FromArgb((int)b).R; //для цветов
                            }
                        }
                    });
                return outmass;
            }
            object GetB(int[] inb)
            {
                Array.Sort(inb);
                return inb[4];
            }
        }

        static void Main(string[] args)
        {
            //в крайний случай
            /* 
             * BitmapData bmpd = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
             * насильно указываем что формат байт на пиксель, но тогда дикое мыльцо, если картиника не такого формата PixelFormat
             * можно сделать, если у нас PixelFormat не поддерживается, ибо их много, то использовтаь этот "пожарный" вариант
             * потестить принудительное сохранение в bmp и потом работа с ним
             */
            //Bitmap bmp = new Bitmap("pic12.bmp");
            //Bitmap bmp=new Bitmap("1.bmp");
            //BitmapData bmpd1 = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
            //bmp.UnlockBits(bmpd1);
            //bmp.Save("test.bmp");
            //Bitmap bmp = new Bitmap("test.bmp");
            //Console.WriteLine(bmp1.PixelFormat.ToString());
            //Console.ReadLine();
            //Bitmap bmp = new Bitmap("1.jpg");
            //bmp2.Save("test.bmp", ImageFormat.Bmp);
            //Bitmap bmp22 = new Bitmap("test.bmp");
             //Console.WriteLine(bmp22.PixelFormat.ToString());
             //Console.ReadLine();
            //Bitmap  bmp = new Bitmap("MFExample.jpg");
             //Console.WriteLine(bmp3.PixelFormat.ToString());
            Bitmap bmp = new Bitmap("pic12.jpg");
             //Console.WriteLine(bmp4.PixelFormat.ToString());
            //Console.ReadLine();
            Console.WriteLine(bmp.PixelFormat.ToString());
            Console.WriteLine(bmp.Width);
            Console.WriteLine(bmp.Height);
            BitmapData bmpd = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, bmp.PixelFormat);
            IntPtr ptr = bmpd.Scan0;
            //проверять bmpd.Stride если не равно ширине то делать версию с *3, иначе воспринимать как байты
            int lengthrgb=bmpd.Stride * bmp.Height;
            byte[] inrgb = new byte[lengthrgb];
            Marshal.Copy(ptr, inrgb, 0, lengthrgb);
            Obertka ob = new Obertka();
            DateTime dd = DateTime.Now;
            inrgb=ob.Filtr(inrgb,bmp.Height, bmp.Width,false);
            TimeSpan ts = DateTime.Now - dd;
            Console.WriteLine(ts.ToString());
            Marshal.Copy(inrgb, 0, ptr, lengthrgb);
            bmp.UnlockBits(bmpd);
            //bmp.Save("2.jpg");
            bmp.Save("pic121.jpg", ImageFormat.Jpeg);
            Console.WriteLine(inrgb.Length);
            Console.ReadLine();
        }
    }
}
