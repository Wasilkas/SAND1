using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SAND1
{
   public class Table
   {
      public List<Row> Data { set; get; }

      public Table(string fname)
      {
         InputTableFromFile(fname);
      }

      void InputTableFromFile(string fname)
      {
         var f = File.ReadAllText(fname);
         char[] delimiterChars = { ' ', '\n', '\t', '\r',  };
         var a = f.Split(delimiterChars);
         a = a.Where(val => val != "").ToArray();
         var data = new List<Row>();
         for (int i = 0; i < 1000; i++)
         {
            data.Add(new Row(int.Parse(a[11 * i]), int.Parse(a[11 * i + 1]),
               int.Parse(a[11 * i + 2]), int.Parse(a[11 * i + 3]), int.Parse(a[11 * i + 4]),
               int.Parse(a[11 * i + 5]), int.Parse(a[11 * i + 6]), int.Parse(a[11 * i + 7]),
               int.Parse(a[11 * i + 8]), int.Parse(a[11 * i + 9]), int.Parse(a[11 * i + 10])));
         }
         Data = data;
      }

      public void OutputToFile()
      {
         using (StreamWriter sw = new StreamWriter("newData.txt"))
         {
            foreach (var r in Data)
            {
               sw.Write($"{r.X17}\t{r.X2}\t{r.X5}\t{r.X7}\t{r.X14}\t{r.X22}\t{r.X12}\t{r.X21}\t{r.X6}\t{r.X20}\t{r.Y}\r\n");
            }
         }
      }

      void FillUniqueValues(List<int> uniqueA, List<int> uniqueB, Row r, int col, int row)
      {
         var props = new int[] { r.X17, r.X2, r.X5, r.X7, r.X14, r.X22, r.X12, r.X21, r.X6, r.X20, r.Y };
         var colProp = props[col];
         var rowProp = props[row];
         if (uniqueA.Contains(colProp) && !uniqueB.Contains(rowProp))
         {
            uniqueB.Add(rowProp);
         }
         else if (!uniqueA.Contains(colProp) && uniqueB.Contains(rowProp))
         {
            uniqueA.Add(colProp);
         }
         else if (!uniqueA.Contains(colProp) && !uniqueB.Contains(rowProp))
         {
            uniqueA.Add(colProp);
            uniqueB.Add(rowProp);
         }
      }

      void insertIntoFreqTable(List<int> uniqueA, List<int> uniqueB, double[,] table, Row r, int col, int row)
      {
         var props = new int[] { r.X17, r.X2, r.X5, r.X7, r.X14, r.X22, r.X12, r.X21, r.X6, r.X20, r.Y };
         var colProp = props[col];
         var rowProp = props[row];

         table[uniqueB.IndexOf(rowProp), uniqueA.IndexOf(colProp)]++;
      }

      public double[,] CreateRelativeFreqTable(int col, int row)
      {
         var uniqueA = new List<int>();
         var uniqueB = new List<int>();

         foreach (var r in Data)
         {
            FillUniqueValues(uniqueA, uniqueB, r, col, row);
         }

         uniqueA = uniqueA.OrderBy(x => x).ToList();
         uniqueB = uniqueB.OrderBy(x => x).ToList();
         var table = new double[uniqueB.Count, uniqueA.Count];

         foreach (var r in Data)
         {
            insertIntoFreqTable(uniqueA, uniqueB, table, r, col, row);
         }

         for (int i = 0; i < uniqueB.Count; i++)
         {
            for (int j = 0; j < uniqueA.Count; j++)
            {
               table[i, j] /= 1000;
            }
         }

         return table;
         //WriteFreqTable(uniqueA, uniqueB, table, col, row);
      }

      void WriteFreqTable(List<int> uA, List<int> uB, double[,] table, int col, int row)
      {
         var propsName = new string[] { "X17", "X2", "X5", "X7", "X14", "X22", "X12", "X21", "X6", "X20", "Y" };

         using (StreamWriter sw = new StreamWriter("FreqTables.txt", true))
         {
            sw.Write($"{propsName[row]} \\ {propsName[col]}\t");
            foreach (var i in uA)
            {
               sw.Write($"{i}\t");
            }
            sw.WriteLine();
            for (var i = 0; i < uB.Count; i++)
            {
               sw.Write($"{uB[i]}\t");
               for (var j = 0; j < uA.Count; j++)
               {
                  sw.Write($"{table[i, j]}\t");
               }
               sw.WriteLine();
            }
            sw.WriteLine();
            sw.WriteLine();
         }
      }

      public void FillKruskalTable()
      {
         var kruskal = new double[11, 11];
         var hypotesis = new bool[11, 11];
         for (int i = 0; i < 11; i++)
         {
            for (int j = i + 1; j < 11; j++)
            {
               var table = CreateRelativeFreqTable(i, j);
               kruskal[i, j] = CalcKruskal(table);
               kruskal[j, i] = kruskal[i, j];
               hypotesis[i, j] = CalcCriterion(table, kruskal[i, j]);
               hypotesis[j, i] = hypotesis[i, j];
            }
         }
         WriteKruskalTable(kruskal);
         WriteCriterionTable(hypotesis);
      }

      bool CalcCriterion(double [,] table, double G)
      {
         var I = CalcI(table);
         var IV = CalcIV(table);
         var II = CalcII(table);
         var III = CalcIII(table);
         var S = new double[table.GetLength(0), table.GetLength(1)];
         var D = new double[table.GetLength(0), table.GetLength(1)];
         var PiS = 0.0;
         var PiD = 0.0;
         for (int i = 0; i < table.GetLength(0); i++)
         {
            for (int j = 0; j < table.GetLength(1); j++)
            {
               PiS += table[i, j] * I[i, j];
               PiD += table[i, j] * IV[i, j];
               S[i, j] = I[i, j] + III[i, j];
               D[i, j] = II[i, j] + IV[i, j];
            }
         }
         var varG = 0.0;
         for (int i = 0; i < table.GetLength(0); i++)
         {
            for (int j = 0; j < table.GetLength(1); j++)
            {
               varG += table[i, j] * Math.Pow((PiS * D[i, j] - PiD * S[i, j]), 2);
            }
         }
         varG *= 0.016 / Math.Pow((PiD + PiS), 4);
         var Sg = Math.Abs(G) / Math.Sqrt(varG);
         return Sg > 1.96;
      }

      double CalcKruskal(double [,] table)
      {
         var I = CalcI(table);
         var IV = CalcIV(table);
         var PiS = 0.0;
         var PiD = 0.0;
         for (int i = 0; i < table.GetLength(0); i++)
         {
            for (int j = 0; j < table.GetLength(1); j++)
            {
               PiS += table[i, j] * I[i, j];
               PiD += table[i, j] * IV[i, j];
            }   
         }
         PiS *= 2;
         PiD *= 2;

         var G = (PiS - PiD) / (PiS + PiD);
         return G;
      }

      double [,] CalcI(double [,] table)
      {
         var n = table.GetLength(0);
         var m = table.GetLength(1);
         var I = new double[n, m];
         for (int i = 0; i < n; i++)
         {
            for (int j = 0; j < m; j++)
            {
               for (int k = i + 1; k < n; k++)
                  for (int l = j + 1; l < m; l++)
                     I[i, j] += table[k, l];
            }
         }
         return I;
      }

      double[,] CalcII(double[,] table)
      {
         var n = table.GetLength(0);
         var m = table.GetLength(1);
         var II = new double[n, m];
         for (int i = 0; i < n; i++)
         {
            for (int j = 0; j < m; j++)
            {
               for (int k = 0; k < i; k++)
                  for (int l = j + 1; l < m; l++)
                     II[i, j] += table[k, l];
            }
         }
         return II;
      }

      double[,] CalcIII(double[,] table)
      {
         var n = table.GetLength(0);
         var m = table.GetLength(1);
         var III = new double[n, m];
         for (int i = 0; i < n; i++)
         {
            for (int j = 0; j < m; j++)
            {
               for (int k = 0; k < i; k++)
                  for (int l = 0; l < j; l++)
                     III[i, j] += table[k, l];
            }
         }
         return III;
      }

      double[,] CalcIV(double[,] table)
      {
         var n = table.GetLength(0);
         var m = table.GetLength(1);
         var IV = new double[n, m];
         for (int i = 0; i < n; i++)
         {
            for (int j = 0; j < m; j++)
            {
               for (int k = i + 1; k < n; k++)
                  for (int l = 0; l < j; l++)
                     IV[i, j] += table[k, l];
            }
         }
         return IV;
      }

      void WriteKruskalTable(double [,] kruskal)
      {
         var propsName = new string[] { "X17", "X2", "X5", "X7", "X14", "X22", "X12", "X21", "X6", "X20", "Y" };

         using (StreamWriter sw = new StreamWriter("KruskalTable.txt"))
         {
            sw.Write("\t");
            foreach (var i in propsName)
            {
               sw.Write($"{i}\t");
            }
            sw.WriteLine();
            for (var i = 0; i < propsName.Length; i++)
            {
               sw.Write($"{propsName[i]}\t");
               for (var j = 0; j < propsName.Length; j++)
               {
                  sw.Write($"{kruskal[i, j]}\t");
               }
               sw.WriteLine();
            }
            sw.WriteLine();
            sw.WriteLine();
         }
      }

      void WriteCriterionTable(bool [,] criterion)
      {
         var propsName = new string[] { "X17", "X2", "X5", "X7", "X14", "X22", "X12", "X21", "X6", "X20", "Y" };

         using (StreamWriter sw = new StreamWriter("CriterionTable.txt"))
         {
            sw.Write("\t");
            foreach (var i in propsName)
            {
               sw.Write($"{i}\t");
            }
            sw.WriteLine();
            for (var i = 0; i < propsName.Length; i++)
            {
               sw.Write($"{propsName[i]}\t");
               for (var j = 0; j < propsName.Length; j++)
               {
                  sw.Write($"{criterion[i, j]}\t");
               }
               sw.WriteLine();
            }
            sw.WriteLine();
            sw.WriteLine();
         }
      }

      public void ReplaceQuntityToQuality()
      {
         ReplaceX12();
         ReplaceX14();
         ReplaceX17();
         ReplaceX20();
         ReplaceX21();
         ReplaceX22();
         ReplaceX5();
      }

      bool IsReplaceOn(int d, int A, int B)
      {
         if (d > A && d <= B)
         {
            return true;
         }
         return false;
      }

      void ReplaceX17()
      {
         foreach (var d in Data)
         {
            var x17 = d.X17;
            if (x17 >= -6948 && x17 <= 390)
            {
               d.X17 = 1;
            }
            else if (IsReplaceOn(x17, 390, 8838))
            {
               d.X17 = 2;
            } 
            else if (IsReplaceOn(x17, 8838, 25126))
            {
               d.X17 = 3;
            }
            else if (IsReplaceOn(x17, 25126, 64879))
            {
               d.X17 = 4;
            } 
            else if (IsReplaceOn(x17, 64879, 402289))
            {
               d.X17 = 5;
            }
         }
      }

      void ReplaceX5()
      {
         foreach (var d in Data)
         {
            var x = d.X5;
            if (x >= 21 && x <= 27)
            {
               d.X5 = 1;
            }
            else if (IsReplaceOn(x, 27, 32))
            {
               d.X5 = 2;
            }
            else if (IsReplaceOn(x, 32, 37))
            {
               d.X5 = 3;
            }
            else if (IsReplaceOn(x, 37, 44))
            {
               d.X5 = 4;
            }
            else if (IsReplaceOn(x, 44, 62))
            {
               d.X5 = 5;
            }
         }
      }

      void ReplaceX14()
      {
         foreach (var d in Data)
         {
            var x = d.X14;
            if (x >= -6674 && x <= 795)
            {
               d.X14 = 1;
            }
            else if (IsReplaceOn(x, 795, 11664))
            {
               d.X14 = 2;
            }
            else if (IsReplaceOn(x, 11664, 28609))
            {
               d.X14 = 3;
            }
            else if (IsReplaceOn(x, 28609, 74954))
            {
               d.X14 = 4;
            }
            else if (IsReplaceOn(x, 74954, 578971))
            {
               d.X14 = 5;
            }
         }
      }

      void ReplaceX22()
      {
         foreach (var d in Data)
         {
            var x = d.X22;
            if (x >= 0 && x <= 1000)
            {
               d.X22 = 1;
            }
            else if (IsReplaceOn(x, 1000, 2261))
            {
               d.X22 = 2;
            }
            else if (IsReplaceOn(x, 2261, 5000))
            {
               d.X22 = 3;
            }
            else if (IsReplaceOn(x, 5000, 302823))
            {
               d.X22 = 4;
            }
         }
      }

      void ReplaceX12()
      {
         foreach (var d in Data)
         {
            var x = d.X12;
            if (x >= -3549 && x <= 1540)
            {
               d.X12 = 1;
            }
            else if (IsReplaceOn(x, 1540, 10992))
            {
               d.X12 = 2;
            }
            else if (IsReplaceOn(x, 10992, 31746))
            {
               d.X12 = 3;
            }
            else if (IsReplaceOn(x, 31746, 79262))
            {
               d.X12 = 4;
            }
            else
            {
               d.X12 = 5;
            }
         }
      }

      void ReplaceX21()
      {
         foreach (var d in Data)
         {
            var x = d.X21;
            if (x >= 0 && x <= 1000)
            {
               d.X21 = 1;
            }
            else if (IsReplaceOn(x, 1000, 2100))
            {
               d.X21 = 2;
            }
            else if (IsReplaceOn(x, 2100, 5000))
            {
               d.X21 = 3;
            }
            else if (IsReplaceOn(x, 5000, 292962))
            {
               d.X21 = 4;
            }
         }
      }

      void ReplaceX20()
      {
         foreach (var d in Data)
         {
            var x = d.X20;
            if (x >= 0 && x <= 6)
            {
               d.X20 = 1;
            }
            else if (IsReplaceOn(x, 6, 1300))
            {
               d.X20 = 2;
            }
            else if (IsReplaceOn(x, 1300, 2716))
            {
               d.X20 = 3;
            }
            else if (IsReplaceOn(x, 2716, 6000))
            {
               d.X20 = 4;
            }
            else if (IsReplaceOn(x, 6000, 154826))
            {
               d.X20 = 5;
            }
         }
      }
   }
}
