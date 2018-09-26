using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranVanSong_DoAn
{
    class Program
    {
        static List<string> Dong = new List<string>();
        static int countTable = 1;

        static bool isMax;
        static int SoAn, SoRangBuoc;
        static int SoAnPhu = 0;
        static double[] HeSoHMT;
        static double[,] MaTran;
        static string[] DauRangBuoc;
        static double[,] MaTranMoRong;
        static double[] LamdaCol;
        static double[] DeltaCol;
        static double Chot;
        static int ChotCol = 0;
        static int ChotRow = 0;
        static double ChotDelta, ChotLamda;
        static double GTToiUu = 0;

        static double[] GTAn;
        public static void DocFile(string file)
        {
            try
            {
                StreamReader sr = new StreamReader(file);
                {
                    string line;

                    while ((line = sr.ReadLine()) != null)
                    {
                        Dong.Add(line);
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Error! Can't Read File");
                Console.WriteLine(e.Message);
            }
        }

        public static void XuatFile()
        {
            FileStream fs = new FileStream("OutPut.txt", FileMode.Create);
            TextWriter tmp = Console.Out;
            StreamWriter sw = new StreamWriter(fs);
            Console.SetOut(sw);
            ThongTinFileOutPut();
            Console.SetOut(tmp);
            sw.Close();
        }

        public static void MaxOrMin()
        {
            isMax = (Dong[0].ToLower().Contains("max")) ? true : false;

            string[] temp = Dong[1].Split(' ');
            SoAn = int.Parse(temp[0]);
            SoRangBuoc = int.Parse(temp[1]);
            if (isMax) MaxPrb();
            else MinPrb();
        }

        public static void MaxPrb()
        {
            HeSoHMT = new double[SoAn];
            MaTran = new double[SoRangBuoc, SoAn];
            LamdaCol = new double[SoRangBuoc];
            DauRangBuoc = new string[SoRangBuoc];
            //dua cac he so cua ham muc tieu vao mang HeSoHMT
            string[] tmpHsHMT = Dong[2].Split(' ');
            for (int i = 0; i < SoAn; i++)
            {
                HeSoHMT[i] = double.Parse(tmpHsHMT[i]);
            }
            for (int i = 3; i < Dong.Count; i++)
            {
                string[] tmpDong = Dong[i].Split(' ');
                //chuyen dau cua rang buoc ve <= hoac =; dem xem co bao nhieu an phu duoc them vao
                int Dau = (tmpDong[SoAn] == ">=") ? -1 : 1;
                DauRangBuoc[i - 3] = (tmpDong[SoAn] == "=") ? "=" : "<=";
                SoAnPhu = (tmpDong[SoAn] == "=") ? SoAnPhu : SoAnPhu + 1;
                //dua cac he so cua rang buoc vao ma tran he so cac rang buoc
                for (int j = 0; j < SoAn; j++)
                {
                    MaTran[i - 3, j] = Dau * double.Parse(tmpDong[j]);
                }
                //dua gia tri vao cot lamda
                LamdaCol[i - 3] = double.Parse(tmpDong[SoAn + 1]);//

            }
            LapBang();
        }

        public static void MinPrb()
        {
            double[] HeSoHMT_MinPrb = new double[SoAn];
            double[,] Matran_MinPrb = new double[SoRangBuoc, SoAn];
            double[] LamdaCol_MinPrb = new double[SoRangBuoc];
            string[] tmpHsHMT = Dong[2].Split(' ');
            for (int i = 0; i < SoAn; i++)
            {
                HeSoHMT_MinPrb[i] = double.Parse(tmpHsHMT[i]);
            }
            for (int i = 3; i < Dong.Count; i++)
            {
                string[] tmpDong = Dong[i].Split(' ');

                int checkDau = (tmpDong[SoAn] == "<=") ? -1 : 1;
                SoAnPhu = SoAn;

                for (int j = 0; j < SoAn; j++)
                {
                    Matran_MinPrb[i - 3, j] = checkDau * double.Parse(tmpDong[j]);
                }

                LamdaCol_MinPrb[i - 3] = double.Parse(tmpDong[SoAn + 1]);
            }
            //chuyen thanh bai toan max
            HeSoHMT = new double[SoRangBuoc];
            MaTran = new double[SoAn, SoRangBuoc];
            LamdaCol = new double[SoAn];
            DauRangBuoc = new string[SoAn];
            for (int i = 0; i < DauRangBuoc.Length; i++)
            {
                DauRangBuoc[i] = "<=";
            }
            LamdaCol_MinPrb.CopyTo(HeSoHMT, 0);
            HeSoHMT_MinPrb.CopyTo(LamdaCol, 0);

            for (int i = 0; i < MaTran.GetLength(0); i++)
            {
                for (int j = 0; j < MaTran.GetLength(1); j++)
                {
                    MaTran[i, j] = Matran_MinPrb[j, i];
                }
            }
            //chuyen doi cac he so
            int tmp = SoAn;
            SoAn = SoRangBuoc;
            SoRangBuoc = tmp;
            LapBang();
        }

        public static void LapBang()
        {
            MaTranMoRong = new double[SoRangBuoc, SoAn + SoAnPhu];
            for (int i = 0; i < MaTran.GetLength(0); i++)
            {
                for (int j = 0; j < MaTran.GetLength(1); j++)
                {
                    MaTranMoRong[i, j] = MaTran[i, j];
                }
            }

            int indexAnPhu = 0;
            for (int i = 0; i < MaTranMoRong.GetLength(0); i++)
            {
                if (DauRangBuoc[i] == "<=")
                {
                    MaTranMoRong[i, MaTran.GetLength(1) + indexAnPhu] = 1;
                    indexAnPhu++;
                }
            }

            DeltaCol = new double[SoAn + SoAnPhu];
            for (int i = 0; i < HeSoHMT.Length; i++)
            {
                DeltaCol[i] = -HeSoHMT[i];
            }
        }

        public static void ShowBang()
        {
            TimChot();
            if (!isFinalTable())
            {
                int tmpRow = ChotRow + 1;
                int tmpCol = ChotCol + 1;
                Console.WriteLine("Phan tu Chot a[" + tmpRow  + "," + tmpCol  + "] =" + Chot);
            }
            else Console.WriteLine("Day la bang don hinh cuoi:");
            int countName = 1;
            //in ra cac thanh phan cua bang don hinh
            for (int i = 0; i < SoAn + SoAnPhu; i++)
            {
                string TenAn = ((isMax == true) ? "x" : "y") + countName;//neu bt max ten an la x, min la y
                Console.Write(String.Format("{0,10:[0.####]}", TenAn));
                countName++;
            }
            Console.Write(String.Format("{0,10:[0.####]}", "λ"));
            Console.WriteLine();
            for (int i = 0; i < SoAnPhu + SoAn + 1; i++)
            {
                Console.Write("__________");
            }
            Console.WriteLine();

            for (int i = 0; i < MaTranMoRong.GetLength(0); i++)
            {
                for (int j = 0; j < MaTranMoRong.GetLength(1); j++)
                {
                    if (i == ChotRow && j == ChotCol && !isFinalTable())
                    {
                        Console.Write(String.Format("{0,10:[0.####]}", MaTranMoRong[i, j]));
                    }
                    else
                        Console.Write(String.Format("{0,10:0.####}", MaTranMoRong[i, j]));
                }
                Console.Write(String.Format("{0,10:0.####}", LamdaCol[i]));
                Console.WriteLine();
            }
            for (int i = 0; i < SoAnPhu + SoAn + 1; i++)
            {
                Console.Write("__________");
            }
            Console.WriteLine();
            foreach (double Delta in DeltaCol)
            {
                Console.Write(String.Format("{0,10:0.####}", Delta));
            }

            Console.WriteLine(String.Format("{0,10:0.####}", GTToiUu));
        }

        public static void TimChot()
        {
            ChotDelta = ChotLamda = double.PositiveInfinity;//gia tri duong lon nhat

            for (int i = 0; i < DeltaCol.Length; i++)
            {
                if (DeltaCol[i] < 0 && DeltaCol[i] < ChotDelta)
                {
                    ChotDelta = DeltaCol[i];
                    ChotCol = i;
                }
            }

            for (int i = 0; i < LamdaCol.Length; i++)
            {
                double tmp = LamdaCol[i] / MaTranMoRong[i, ChotCol];
                if (tmp > 0 && tmp < ChotLamda)
                {
                    ChotLamda = tmp;
                    ChotRow = i;
                }
            }

            Chot = MaTranMoRong[ChotRow, ChotCol];
        }

        public static void ChuyenDoiBang()
        {
            for (int j = 0; j < MaTranMoRong.GetLength(1); j++)
            {
                MaTranMoRong[ChotRow, j] = MaTranMoRong[ChotRow, j] / Chot;
            }

            LamdaCol[ChotRow] = LamdaCol[ChotRow] / Chot;
            for (int i = 0; i < MaTranMoRong.GetLength(0); i++)
            {
                if (i != ChotRow)
                {
                    double delta = -(MaTranMoRong[i, ChotCol]);
                    LamdaCol[i] += delta * LamdaCol[ChotRow];
                    for (int j = 0; j < MaTranMoRong.GetLength(1); j++)
                    {
                        MaTranMoRong[i, j] += delta * MaTranMoRong[ChotRow, j];
                    }
                }
            }
            double deltaLambda = -DeltaCol[ChotCol];
            for (int j = 0; j < DeltaCol.Length; j++)
            {
                DeltaCol[j] += deltaLambda * MaTranMoRong[ChotRow, j];
            }
            GTToiUu += deltaLambda * LamdaCol[ChotRow];
        }

        public static bool isFinalTable()
        {
            foreach (double delta in DeltaCol)
            {
                if (delta < 0) return false;
            }
            return true;
        }

        public static bool isSolutionExist()
        {
            for (int i = 0; i < LamdaCol.Length; i++)
            {
                if (LamdaCol[i] / MaTranMoRong[i, ChotCol] > 0) return true;
            }
            return false;
        }

        public static void TimGTCacAn()
        {
            Console.WriteLine("\n-----------  Ket Qua  ----------");
            if (isMax == true)
            {
                GTAn = new double[SoAn + SoAnPhu];
                bool[] checkValue = new bool[SoAn + SoAnPhu];
                for (int i = 0; i < DeltaCol.Length; i++)
                {
                    if (DeltaCol[i] != 0)
                    {
                        GTAn[i] = 0;
                        checkValue[i] = true;
                    }
                }

                for (int i = 0; i < MaTranMoRong.GetLength(0); i++)
                {
                    for (int j = 0; j < MaTranMoRong.GetLength(1); j++)
                    {
                        if (checkValue[j] == false && MaTranMoRong[i, j] != 0)
                        {
                            GTAn[j] = LamdaCol[i] / MaTranMoRong[i, j];
                            checkValue[j] = true;
                        }
                    }
                }
            }
            else
            {
                GTAn = new double[SoAnPhu];
                for (int i = SoAn; i < DeltaCol.Length; i++)
                {
                    GTAn[i - SoAn] = DeltaCol[i];
                }
            }
        }

        public static void FinalOutput()
        {
            int SoAn = 1;
            foreach (double value in GTAn)
            {
                Console.Write(String.Format("{0,10:0.####}", "x" + SoAn + " = " + value));
                SoAn++;
            }
            Console.WriteLine(String.Format("{0,18:0.####}", "f(" + ((isMax == true) ? "max) = " : "min) = ") + GTToiUu));
        }

        public static void ShowPrb()
        {
            if (isMax == false) Console.WriteLine("Bai toan MIN vua nhap duoc chuyen thanh bai toan MAX");
            Console.WriteLine("Ham Muc Tieu:");
            for (int i = 0; i < HeSoHMT.Length; i++)
            {
                Console.Write(String.Format("{0,10:0.####}", HeSoHMT[i]));
            }
            Console.WriteLine();
            Console.WriteLine("\nMa Tran He So Cac Rang Buoc Va Cot λ:");

            for (int i = 0; i < MaTran.GetLength(0); i++)
            {
                for (int j = 0; j < MaTran.GetLength(1); j++)
                {
                    Console.Write(String.Format("{0,10:0.####}", MaTran[i, j]));
                }
                Console.Write(String.Format("{0,10:0.####}", LamdaCol[i]));

                Console.WriteLine();
            }
        }

        public static void ThongTinFileOutPut()
        {
            MaxOrMin();
            ShowPrb();
            Console.WriteLine("\nBang don hinh thu " + countTable);
            ShowBang();
            while (!isFinalTable())
            {
                if (!isSolutionExist())
                {
                    Console.WriteLine("Bai toan khong co Phuong An Toi Uu");
                    break;
                }
                countTable++;
                Console.WriteLine("\nBang don hinh thu " + countTable);
                TimChot();
                ChuyenDoiBang();
                ShowBang();
            }
            if (countTable == 1) Console.WriteLine("Bai toan khong co Phuong An Toi Uu");
            else
            {
                TimGTCacAn();
                FinalOutput();
            }
        }
        static void Main(string[] args)
        {
            Console.WriteLine("------------------------ CHUONG TRINH GIAI BAI TOAN QUY HOACH TUYEN TINH ---------------------");
            Console.WriteLine("\n* Bai toan can giai duoc dat trong thu muc " + @"E:\Demo");
            Console.WriteLine("* Cu phap nhap bai toan: tenbaitoan + .txt  (vi du: input.txt)");
            Console.WriteLine("______________________________________________________________________________________________\n");
            Console.Write("Nhap ten bai toan can giai: ");
            string tmp = Console.ReadLine();
            string add = @"E:\Demo\" + tmp;
            Console.WriteLine();
            DocFile(add);
            XuatFile();
            ShowBang();
            Console.WriteLine("\n---------- Ket Qua ----------");
            FinalOutput();
            Console.WriteLine("______________________________________________________________________________________________\n");
            Console.WriteLine("Xem them ket qua tai file " + @"E:\DoAn\TranVanSong_DoAn\TranVanSong_DoAn\bin\Debug\OutPut.txt");
            Console.ReadKey();
        }
    }
}
