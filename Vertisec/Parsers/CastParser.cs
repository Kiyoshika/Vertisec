using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Vertisec.Tokens;

namespace Vertisec.Parsers
{
    public class CastParser
    {
        private static HashSet<string> validDataTypes = new HashSet<string> { 
            "int", "integer", "number",
            "float", "numeric",
            "char", "varchar"
        };

        public static void Parse(List<Token> tokens, int tokenIndex)
        {
            // immediate token to the right should also be a colon
            if (tokens[tokenIndex + 1].GetText() != ":")
                Console.WriteLine("Bad syntax on line " + tokens[tokenIndex + 1].GetLineNumber() + ". Did you mean to use '::'?");
            // tokenIndex + 2 should be a valid data type
            if (!validDataTypes.Contains(tokens[tokenIndex + 2].GetText()))
                Console.WriteLine("Casting to invalid data type on line " + tokens[tokenIndex + 2].GetLineNumber());
        }
    }
}
