using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vertisec
{
    public class Globals
    {
        private static string[] originalSQL;
        private static int errorDisplayLines = 3;

        public static void SetOriginalSQL(string[] originalSQL)
        {
            Globals.originalSQL = originalSQL;
        }

        public static string[] GetOriginalSQL()
        {
            return Globals.originalSQL;
        }

        public static int GetErrorDisplayLines()
        {
            return Globals.errorDisplayLines;
        }
    }
}
