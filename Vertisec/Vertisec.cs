using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Vertisec.Tokens;
using Vertisec.FileIO;
using Vertisec.Clauses;
using Vertisec.Util;
using Vertisec.Parsers;
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

            //foreach (Token token in tokensCopy)
            for (int i = 0; i < tokensCopy.Count(); i++)
            {
                switch (tokensCopy[i].GetText())
                {
                    case "(":
                        Tuple<List<Token>, int> openParenthesis = ParenthesisParser.Parse(tokensCopy, i, '(');
                        i += openParenthesis.Item2;
                        break;

                    case ")":
                        Tuple<List<Token>, int> closedParenthesis = ParenthesisParser.Parse(tokensCopy, i, ')');
                        i += closedParenthesis.Item2;
                        break;

                    case "select":
                        SelectClause selectClause = new SelectClause();
                        i += selectClause.BuildClause(tokensCopy, i);
                        clauses.Add(selectClause);
                        break;

                    case "from":
                        FromClause fromClause = new FromClause();
                        i += fromClause.BuildClause(tokensCopy, i);
                        clauses.Add(fromClause);
                        break;

                    case "where":
                        WhereClause whereClause = new WhereClause();
                        i += whereClause.BuildClause(tokensCopy, i);
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
