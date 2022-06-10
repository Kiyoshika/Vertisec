using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vertisec.Util
{
    internal class InternalErrorMessage
    {
        public static void PrintError(string message)
        {
            Console.WriteLine(message);
            Environment.Exit(-1);
        }
    }
}
