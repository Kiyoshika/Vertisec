using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vertisec
{
    internal class InternalErrorMessage
    {
        public static void PrintError(string message)
        {
            Console.WriteLine(message);
            System.Environment.Exit(-1);
        }
    }
}
