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
using Vertisec.Clauses.FromClause;
using Vertisec.Clauses.WhereClause;
using Vertisec;

namespace Vertisec
{
    public class Vertisec
    {
        private List<Token> tokens;
        public List<Token> tokens_copy;
        public List<Clauses.Clauses> clauses = new List<Clauses.Clauses>();
        public Vertisec(string filepath)
        {
            ReadFile file = new ReadFile(filepath);
            string[] sqlLines = file.Read();
            Globals.SetOriginalSQL(sqlLines);
            tokens = Tokenizer.Tokenize(sqlLines);
        }

        public void BuildClauses()
        {
            List<Token> tokensCopy = this.tokens.ToList<Token>();
            this.tokens_copy = tokensCopy;

            bool validStartToken = false;
            foreach (Token token in tokensCopy)
                if (Tokenizer.sqlStartCheck(token.GetText()))
                    validStartToken = true;

            if (!validStartToken)
                InternalErrorMessage.PrintError("SQL file missing starting keyword drop, create, with, select");

            int startIndex = 0, skipTokens = 0;
            foreach (Token token in tokensCopy)
            {
                // after parsing an expression, skip over tokens we've already encountered before
                // entering next clause
                while (skipTokens > 0)
                {
                    startIndex++;
                    skipTokens--;
                }

                switch (token.GetText())
                {
                    case "select":
                        SelectClause selectClause = new SelectClause();
                        skipTokens = selectClause.BuildClause(this.tokens, startIndex) - 1;
                        clauses.Add(selectClause);
                        break;

                    case "from":
                        FromClause fromClause = new FromClause();
                        skipTokens = fromClause.BuildClause(tokens, startIndex) - 1;
                        clauses.Add(fromClause);
                        break;

                    case "where":
                        WhereClause whereClause = new WhereClause();
                        skipTokens = whereClause.BuildClause(this.tokens, startIndex) - 1;
                        clauses.Add(whereClause);
                        break;

                    default:
                        break;
                }

                //Console.WriteLine(token.GetText() + ' ' + tokensCopy.IndexOf(token));
            }
        }

        public List<Token> GetTokens()
        {
            return this.tokens;
        }
    }
}
