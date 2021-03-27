using System;
using System.Collections.Generic;
using System.Text;

namespace SAND1
{
   public class Row
   {
      public int X17 { get; set; }
      public int X2 { get; set; }
      public int X5 { get; set; }
      public int X7 { get; set; }
      public int X14 { get; set; }
      public int X22 { get; set; }
      public int X12 { get; set; }
      public int X21 { get; set; }
      public int X6 { get; set; }
      public int X20 { get; set; }
      public int Y { get; set; }

      public Row(int x17, int x2, int x5, int x7,
         int x14, int x22, int x12, int x21, int x6,
         int x20, int y)
      {
         X17 = x17;
         X2 = x2;
         X5 = x5;
         X7 = x7;
         X14 = x14;
         X22 = x22;
         X12 = x12;
         X21 = x21;
         X6 = x6;
         X20 = x20;
         Y = y;
      }

   }
}
