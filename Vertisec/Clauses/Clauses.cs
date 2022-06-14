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
        protected string[] originalSQL;
        protected void SetOriginalSQL(string[] sql)
        {
            this.originalSQL = sql;
        }

        protected string[] GetOriginalSQL()
        {
            return this.originalSQL;
        }

        public abstract List<Token> GetTokens();
        public abstract void BuildClause(ref List<Token> tokens, string[] originalSQL);
    }
}
