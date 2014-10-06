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
        //проверять размер картинки
        static byte[] Filtr(byte[] inmass, int iheight, int iwidth)
        {
            //int b=3;
            byte b = 3;
            int[] intrgb=new int[inmass.Length/3];
            Console.WriteLine(inmass.Length);
            //for (Int64 i = 0; i < inmass.Length; i += 3)
            //{
                //Console.WriteLine(inmass[i]);
                //Console.ReadLine();
                // если бмп, то сразу дает интенсивность.
                //intrgb[i / 3] = Color.FromArgb(inmass[i], inmass[i + 1], inmass[i + 2]).ToArgb();
            //}
            //Console.WriteLine("Перегнали в инт");
            int[] tmpmass = new int[9];
            byte[] outmass=new byte[inmass.Length];
            for (int i = 0; i < iwidth - 2; i++)
                for (int j = 0; j < iheight - 2; j++)
                {
                    for (int i1 = 0; i1 < 3; i1++)
                        for (int j1 = 0; j1 < 3; j1++)
                        {
                            tmpmass[i1 * 3 + j1] = inmass[i + j1 + (i1 + j) * iwidth];//intrgb[i + j1 + (i1 + j) * iwidth];
                        }
                    GetB(tmpmass, out b);
                    //outmass[(i + 1)*3 + (j + 1) * iwidth*3] = Color.FromArgb(b).R;
                    //outmass[(i + 1)*3 + (j + 1) * iwidth*3+ 2] = Color.FromArgb(b).G;
                    //outmass[(i + 1)*3 + (j + 1) * iwidth*3+ 3] = Color.FromArgb(b).B;
                    outmass[(i + 1) + (j + 1) * iwidth] = b;
                }
            return outmass;
        }
        static void GetB(int[] inb, out byte/*int */ outb)
        {
            Array.Sort(inb);
            outb =(byte)/* */ inb[4];
        }

        static void Main(string[] args)
        {
            Bitmap bmp = new Bitmap("pic12.bmp");
            //Bitmap bmp=new Bitmap("1.bmp");
            //Bitmap bmp = new Bitmap("1.jpg");
            //Bitmap bmp = new Bitmap("MFExample.jpg");
            //Bitmap bmp = new Bitmap("pic12.jpg");
            Console.WriteLine(bmp.Width);
            Console.WriteLine(bmp.Height);
            BitmapData bmpd=bmp.LockBits(new Rectangle(0,0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, bmp.PixelFormat);
            Console.WriteLine("Локбитс");
            IntPtr ptr = bmpd.Scan0;
            Console.WriteLine("Скан0");
            Console.WriteLine(bmpd.Stride);//проверять bmpd.Stride если не равно ширине то делать версию с *3, иначе воспринимать как байты
            int lengthrgb=bmpd.Stride * bmp.Height;
            byte[] inrgb = new byte[lengthrgb];
            Marshal.Copy(ptr, inrgb, 0, lengthrgb);
            Console.WriteLine("Скопировали в массив inrgb");
            Console.WriteLine(inrgb.Length);
            inrgb=Filtr(inrgb,bmp.Height, bmp.Width);
            Console.WriteLine("Прогнали фильтр");
            Marshal.Copy(inrgb, 0, ptr, lengthrgb);
            Console.WriteLine("Скопировали результаты фильтра в бмпдату");
            bmp.UnlockBits(bmpd);
            //bmp.Save("2.jpg");
            bmp.Save("pic122.bmp");

            Console.WriteLine(inrgb.Length);
            Console.ReadLine();
        }
    }
}
