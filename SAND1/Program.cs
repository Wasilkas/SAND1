using System;
using System.Linq;

namespace SAND1
{
   class Program
   {
      static void Main(string[] args)
      {
         var t = new Table("Data.txt");
         t.ReplaceQuntityToQuality();
         t.FillKruskalTable();
         //t.OutputToFile();
      }
   }
}
