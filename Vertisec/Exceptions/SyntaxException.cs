using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Vertisec.Errors;
using Vertisec.Tokens;

namespace Vertisec.Exceptions
{
    public class SyntaxException : Exception
    {
        public SyntaxException()
        {
        }

        public SyntaxException(string message, Token token) : base(message)
        {
            ErrorMessage.PrintError(token, message);
        }

        public SyntaxException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
