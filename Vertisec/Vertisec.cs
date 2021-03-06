using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Vertisec.Tokens;
using Vertisec.FileIO;
using Vertisec.Clauses;
using Vertisec.Exceptions;
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

        public Vertisec() { }
        public Vertisec(string filepath)
        {
            ReadFile file = new ReadFile(filepath);
            string[] sqlLines = file.Read();
            Globals.SetOriginalSQL(sqlLines);
            tokens = Tokenizer.Tokenize(sqlLines);
        }
        
        public void SetTokens(List<Token> tokens)
        {
            this.tokens = tokens;
        }

        // the referenceToken is used to fetch the last token in case there is an error (e.g., an empty derived table "select from () x")
        public void BuildClauses(Token? referenceToken = null)
        {
            List<Token> tokensCopy = this.tokens.ToList<Token>();
            this.tokens_copy = tokensCopy;

            bool validStartToken = false;
            foreach (Token token in tokensCopy)
                if (Tokenizer.sqlStartCheck(token.GetText()))
                    validStartToken = true;

            if (!validStartToken)
                if (referenceToken != null)
                    throw new SyntaxException("Missing starting keyword: drop, create, with, select", referenceToken);
                else
                    InternalErrorMessage.PrintError("Missing starting keyword: drop, create, with, select");

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
                        i += (selectClause.BuildClause(tokensCopy, i) - 2);
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
