using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Vertisec.Tokens;
using Vertisec.Util;
using Vertisec.Parsers;
using Vertisec.Errors;
using Vertisec.Exceptions;
using Vertisec.Clauses.SelectClause;
using Vertisec;

namespace Vertisec.Clauses.FromClause
{
    public class FromClause : Clauses
    {
        private Token fromToken; // used to fetch "from" token if no table is specified (i.e., fromTokens = {})
        private List<Token> fromTokens = new List<Token>();
        private HashSet<string> stopTokens = new HashSet<string>
        {
            "order",
            "group",
            "limit",
            "where",
            "having",
            "left",
            "right",
            "join",
            "full",
            "inner",
            "union",
            "select"
        };

        private void ValidPull()
        {
            if (this.fromTokens.Count() == 0)
                throw new SyntaxException("Missing table for 'from' token.", fromToken);

            Token openParenthesis = this.fromTokens.Find(tok => tok.GetText() == "(");
            if (openParenthesis != null)
            {
                int startIndex = this.fromTokens.IndexOf(openParenthesis);
                Tuple<List<Token>, int> parens = ParenthesisParser.Parse(this.fromTokens, startIndex, '(');

                // remove front and back parenthesis
                parens.Item1.RemoveAt(0);
                parens.Item1.RemoveAt(parens.Item1.Count() - 1);

                // if parenthesis are empty, this is an empty derived table
                if (parens.Item1.Count() == 0)
                    throw new SyntaxException("Derived table is empty.", this.fromTokens[0]);

                // ensure inner tokens is valid SQL
                Vertisec vertisec = new Vertisec();
                vertisec.SetTokens(parens.Item1);
                vertisec.BuildClauses(parens.Item1[0]);

                this.fromTokens.RemoveRange(startIndex, parens.Item2); // remove all the inner parenthesis
                Token psuedoTable = new Token("derived_table", openParenthesis.GetLineNumber());
                this.fromTokens.Insert(startIndex, psuedoTable);
            }

            // any hanging closing parenthesis is an error
            Token closeParenthesis = this.fromTokens.Find(tok => tok.GetText() == ")");
            if (closeParenthesis != null)
                throw new SyntaxException("Missing opening parenthesis.", closeParenthesis);

            Token asToken = this.fromTokens.Find(tok => tok.GetText() == "as");

            // if 'as' is final keyword	
            if (this.fromTokens[this.fromTokens.Count() - 1].GetText() == "as") 
                throw new SyntaxException("'as' expecting an alias.", this.fromTokens[this.fromTokens.Count() - 1]);

            // if derived table doesn't have an alias
            if (this.fromTokens[this.fromTokens.Count() - 1].GetText() == "derived_table")
                throw new SyntaxException("Derived table must have an alias.", this.fromTokens[this.fromTokens.Count() - 1]);

            // check proper aliasing rules, e.g. "from x as y"
            else if (asToken != null)
            {
                int asTokenIndex = this.fromTokens.IndexOf(asToken);

                if (this.fromTokens.Count() < 3)
                    throw new SyntaxException("Too few tokens for 'as'. Are you missing a table/alias?", this.fromTokens[asTokenIndex]);

                if (asTokenIndex != 1 && this.fromTokens.Count() == 3)
                    throw new SyntaxException("Improper 'from' aliasing with 'as'.", this.fromTokens[asTokenIndex]);
                else if (this.fromTokens.Count() - asTokenIndex > 2)
                    throw new SyntaxException("Too many tokens after 'as'.", this.fromTokens[asTokenIndex]);
            }

            //check to ensure short alias is correct ie 'from base b'
            else if (this.fromTokens.Count() >= 4 || (this.fromTokens.Count() == 3 && this.fromTokens.Find(tok => tok.GetText() == "as") == null))
            {
                int tokenIdx = this.fromTokens.Count() >= 4 ? 3 : 2;
                throw new SyntaxException("Improper 'from' aliasing, too many tokens after 'from'.", this.fromTokens[tokenIdx]);
            }
        }

        public override List<Token> GetTokens()
        {
            return this.fromTokens;
        }

        public override int BuildClause(List<Token> tokens, int startIndex)
        {
            int totalFromTokens = 0;
            fromToken = tokens[startIndex];

            for (int i = startIndex + 1; i < tokens.Count; ++i) // the + 1 automatically skips "from" token
            {
                if (tokens[i].GetText() == "(")
                {
                    Tuple<List<Token>, int> parens = ParenthesisParser.Parse(tokens, i, '(');

                    foreach (Token innerToken in parens.Item1) // add all the inner tokens
                        this.fromTokens.Add(innerToken);

                    i += parens.Item2;
                    totalFromTokens += parens.Item2;
                }

//                if (tokens[i].GetText() == ")") break; // at the end of a derived table
           
                // if index increment after parenthesis reaches end of token list, want to avoid out of bounds indices
                if (i < tokens.Count())
                {
                    if (stopTokens.Contains(tokens[i].GetText())) break;

                    this.fromTokens.Add(tokens[i]);
                    totalFromTokens++;
                }
            }

            ValidPull();

            return totalFromTokens; 
        }
    }
}
