using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Vertisec.Tokens;

namespace Vertisec.Clauses
{
    public abstract class Clauses
    {
        public abstract List<Token> GetTokens();
        public abstract void BuildClause(ref List<Token> tokens);
    }
}
