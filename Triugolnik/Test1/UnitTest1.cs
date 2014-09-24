using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Triugolnik;
using System.Diagnostics;
using System.IO;
using System.Threading;
namespace Test1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Test_Etalon()
        {
            double a = 5, b = 5, alpha = 90, c = 7.071;
            Program.Triugolnik t = new Program.Triugolnik(a, b, alpha);

            double c1 = t.getStorona();
            t.getAngleBetta();
            t.getAngleGamma();
            Assert.AreEqual(c, c1, 0.01, "Итоговое значение не совпадает с эталоном");
            //сравнить углы
        }

        [TestMethod]
        public void Test_Constructor()
        {
            double a = 5, b = 5, alpha = 180;
            Program.Triugolnik t = new Program.Triugolnik(a, b, alpha);
            Assert.AreEqual(t.getA, 0, 0, "Проверка данных в конструкторе не сработала");
        }

        [TestMethod]
        public void Test_FileInputExist()
        {
            Program.fileWork fw = new Program.fileWork();
            fw.Load("inTetst1.txt", "outTest.txt");
            //проверка на отсутвие входного файла
        }

        [TestMethod]
        public void Test_FileErrorData()
        {
            Program.fileWork fw = new Program.fileWork();
            fw.Load("inTest.txt", "outTest.txt");
            //неверный формат double
        }

        [TestMethod]
        public void Test_FileNotFullData()
        {
            Program.fileWork fw = new Program.fileWork();
            fw.Load("inTest2.txt", "outTest2.txt");
        }
    }
}
