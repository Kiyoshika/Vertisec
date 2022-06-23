using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Vertisec.Errors;
using Vertisec.Tokens;

namespace Vertisec.Exceptions
{
    public class SelectClauseException : Exception
    {
        public SelectClauseException()
        {
        }

        public SelectClauseException(string message, Token token) : base(message)
        {
            ErrorMessage.PrintError(token, "'from' token for 'select' not found.");
        }

        public SelectClauseException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
