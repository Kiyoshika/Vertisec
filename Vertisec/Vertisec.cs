using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Vertisec.Tokens;
using Vertisec.FileIO;
using Vertisec.Clauses;
using Vertisec.Util;
using Vertisec.Clauses.SelectClause;

namespace Vertisec
{
    public class Vertisec
    {
        private string[] originalSQL;
        private List<Token> tokens;
        public List<Clauses.Clauses> clauses = new List<Clauses.Clauses>();
        public Vertisec(string filepath)
        {
            ReadFile file = new ReadFile(filepath);
            string[] sqlLines = file.Read();
            this.originalSQL = sqlLines;
            tokens = Tokenizer.Tokenize(sqlLines);
        }

        public void BuildClauses()
        {
            List<Token> tokensCopy = this.tokens.ToList<Token>();

            bool validStartToken = false;
            foreach (Token token in tokensCopy)
                if (Tokenizer.sqlStartCheck(token.GetText()))
                    validStartToken = true;

            if (!validStartToken)
                InternalErrorMessage.PrintError("SQL file missing starting keyword drop, create, with, select");


            foreach (Token token in tokensCopy)
            {
                switch (token.GetText())
                {

                    case "select":
                        SelectClause selectClause = new SelectClause();
                        selectClause.BuildClause(ref this.tokens, this.originalSQL);
                        clauses.Add(selectClause);
                        break;

                    default:
                        break;
                }
            }
        }

        public List<Token> GetTokens()
        {
            return this.tokens;
        }
    }
}
