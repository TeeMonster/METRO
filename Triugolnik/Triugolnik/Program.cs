using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


namespace Triugolnik
{
    public class Program
    {
        public class Triugolnik
        {
            double a, b, c, alpha, betta, gamma;

            public Triugolnik(double newa, double newb, double newalpha)
            {
                if (newa > 0 && newb > 0 && newalpha < 180 && newalpha > 0)
                {
                    a = newa;
                    b = newb;
                    alpha = newalpha;
                }
                else
                {
                    a = 0;
                    c = 0;
                    alpha = 0;
                }
            }

            public double getA
            {
                get { return a; }

            }

            public double getB
            {
                get { return b; }
            }

            public double getAlpha
            {
                get { return alpha; }
            }


            public bool checkValue()
            {
                if (a == 0 || b == 0 || alpha == 0)
                    return false;
                else
                    return true;
            }

            public double getStorona()
            {

                c = Math.Sqrt(a * a + b * b - 2 * a * b * Math.Cos((alpha * Math.PI) / 180));

                return c;
            }

            public double getAngleBetta()
            {
                double value = (a * a + c * c - b * b) / (2 * a * c);
                betta = Math.Acos(value) * 180 / Math.PI;
                return betta;
            }

            public double getAngleGamma()
            {
                return 180 - alpha-betta;
            }
        }

        public class fileWork
        {
            public void Load(string fileInput, string fileOutput)
            {
                if (File.Exists(fileInput))
                {
                    using (StreamReader fileIn = new StreamReader(fileInput))
                    {
                        string s;
                        Int64  countString = 0;
                        StreamWriter fileOut = new StreamWriter(fileOutput, false);
                        while ((s = fileIn.ReadLine()) != null)
                        {
                            countString++;
                            string[] data = s.Split(';');
                            if (data.Length == 3)
                            {
                                double a, b, alpha, c, betta, gamma;
                                bool flag = false;
                                if (double.TryParse(data[0], out a))
                                    if (double.TryParse(data[1], out b))
                                        if (double.TryParse(data[2], out alpha))
                                        {
                                            flag = true;
                                            Triugolnik t = new Triugolnik(a, b, alpha);
                                            if (t.checkValue())
                                            {
                                                c = t.getStorona();
                                                betta = t.getAngleBetta();
                                                gamma = t.getAngleGamma();

                                                if (File.Exists(fileOutput))
                                                {
                                                    fileOut.WriteLine(a.ToString() + ";" + b.ToString() + ";" + c.ToString());
                                                    Console.WriteLine("Номер строки: " + countString.ToString() + " Сторона А=" + a.ToString() + " сторона B=" + b.ToString() + " сторона C=" + c.ToString() + ", углы Alpha=" + alpha.ToString() + " Betta=" + betta.ToString() + " Gamma=" + gamma.ToString());
                                                }
                                                else
                                                {
                                                    Console.WriteLine("Ошибка. Итоговый файл удален.");
                                                }

                                            }
                                            else
                                            {
                                                Console.WriteLine("Исходные данные не корректны. Вычисление невозможно");
                                            }

                                        }
                                if (!flag)
                                    Console.WriteLine("Ошибка формата данных во входном файле. Строка № {0}", countString);
                            }
                            else
                            {
                                Console.WriteLine("Неверная длина исходных данных в строке № {0}", countString);
                            }
                        }
                        if (countString == 0) Console.WriteLine("Входной файл пуст");
                        fileOut.Close();
                        fileIn.Close();
                    }
                }
                else
                {
                    Console.WriteLine("Входной файл не найден");
                }
            }

            public void Save(double c, double betta, double gamma, string fileOutputName)
            {
            }
        }

        static void Main(string[] args)
        {
            if (args.Length >= 2)
            {
                fileWork file = new fileWork();
                file.Load(args[0], args[1]);
            }
            else
            {
                Console.WriteLine("Неверное количество аргументов");
            }
            Console.ReadLine();
        }
    }
}
