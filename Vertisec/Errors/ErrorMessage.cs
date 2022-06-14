using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Vertisec.Tokens;
using Vertisec;

namespace Vertisec.Errors
{
    public class ErrorMessage
    {
        public static void PrintError(Token errorToken, string errorMessage)
        {
            uint plusMinusNLines = Globals.GetErrorDisplayLines();
            string[] originalSQL = Globals.GetOriginalSQL();

            // display +/- plusMinusNLines of the original SQL when displaying error message
            uint errorLineNumber = errorToken.GetLineNumber();
            uint lowerBound = (errorLineNumber - plusMinusNLines) < 0 ? 0 : (errorLineNumber - plusMinusNLines);
            uint upperBound = (errorLineNumber + plusMinusNLines) > originalSQL.Length ? (uint)originalSQL.Length : (errorLineNumber + plusMinusNLines);

            for (uint i = lowerBound; i < upperBound; ++i)
            {
                if ((i+1) == errorToken.GetLineNumber())
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(">>> " + (i + 1) + ": " + originalSQL[i]);
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                else
                    Console.WriteLine("    " + (i+1) + ": " + originalSQL[i]);
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nError on line " + errorToken.GetLineNumber() + ": " + errorMessage);
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}
