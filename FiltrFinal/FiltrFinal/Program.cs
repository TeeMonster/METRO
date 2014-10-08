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
            public byte[] Filtr(byte[] inmass, int iheight, int iwidth, bool normtype)
            {
                int[] intrgb = new int[inmass.Length / 3];
                //если картинка формата 24
                if (!normtype)
                {
                    for (Int64 i = 0; i < inmass.Length/3*3; i += 3)
                    {
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
                Parallel.For(0,3,i2=>
                    {
                        int istart=i2*iwidth/3,ifinish=i2*iwidth/3+iwidth/3;
                        if (ifinish > iwidth - 2) ifinish = iwidth - 2;
                        for (int i=istart; i<ifinish;i++)
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
            if (args.Length != 2)
            {
                Console.WriteLine("Arguments error");
                Console.ReadLine();
                Environment.Exit(0);
            }
            if (!File.Exists(args[0]))
            {
                Console.WriteLine("File not found");
                Console.ReadLine();
                Environment.Exit(0);
            }
            Bitmap bmp = new Bitmap(args[0]);
            if (!(bmp.PixelFormat == PixelFormat.Format8bppIndexed || bmp.PixelFormat == PixelFormat.Format24bppRgb))
            {
                Console.WriteLine(bmp.PixelFormat.ToString() + " not supported. Let's try PixelFormat 8 or 24 bits");
                Console.ReadLine();
                Environment.Exit(0);
            }
            if (bmp.Width < 3 || bmp.Height < 3) 
            {
                Console.WriteLine("Picture is small");
                Console.ReadLine();
                Environment.Exit(0);
            }
            BitmapData bmpd = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, bmp.PixelFormat);
            IntPtr ptr = bmpd.Scan0;
            int lengthrgb = bmpd.Stride * bmp.Height;
            byte[] inrgb = new byte[lengthrgb];
            Marshal.Copy(ptr, inrgb, 0, lengthrgb);
            Obertka ob = new Obertka();
            inrgb = ob.Filtr(inrgb, bmp.Height, bmp.Width, (bmp.PixelFormat==PixelFormat.Format8bppIndexed));
            Marshal.Copy(inrgb, 0, ptr, lengthrgb);
            bmp.UnlockBits(bmpd);
            string[] s = args[1].Split('.');
            string format = s[s.Length-1].ToUpper();
            if (format=="JPG"||format=="JPEG")
            {
                bmp.Save(args[1], ImageFormat.Jpeg);
            }
            else
                if (format=="BMP")
                {
                    bmp.Save(args[1], ImageFormat.Bmp);
                }
            else
                    if (format == "PNG")
                    {
                        bmp.Save(args[1], ImageFormat.Png);
                    }
                    else
                    {
                        Console.WriteLine("Format not supported. Save as JPEG");
                        bmp.Save(args[1], ImageFormat.Jpeg);
                    }
            Console.ReadLine();
        }
    }
}
