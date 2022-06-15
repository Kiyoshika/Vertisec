using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Vertisec.Tokens;
using Vertisec.Util;
using Vertisec.Parsers;
using Vertisec.Errors;
using Vertisec;

namespace Vertisec.Clauses.FromClause
{
    internal class FromClause : Clauses
    {
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

        private void FromTableExists(ref List<Token> tokens)
        {
            List<Token> newTokens = new List<Token>();
            Token startingToken = newTokens[0];
            int fromIndex = 0;
            foreach (Token token in newTokens)
            {
                if (token.GetText() == "from")
                    fromIndex = newTokens.IndexOf(token);
            }

            if (fromIndex != newTokens.Count())
                return;

            ErrorMessage.PrintError(startingToken, "'from' token missing table.");
        }

        private void ValidPull()
        {
            int fromTokenIndex = 0;
            int quoteLength = 0;
            List<Token> tokenBuffer = new List<Token>();

            foreach (Token token in this.fromTokens)
            {
                // skip "from" token
                if (token.GetText() == "from")
                {
                    fromTokenIndex++;
                    continue;
                }

                //we check for quotes and skip
                if (quoteLength > 0)
                {
                    quoteLength--;
                    fromTokenIndex++;
                    continue;
                }

                if (token.GetText() == "as")
                {
                    Token asToken = tokenBuffer.Find(tok => tok.GetText() == "as");
                    
                    if (asToken != null && tokenBuffer.IndexOf(asToken) != 1 && tokenBuffer.Count() == 3)
                        ErrorMessage.PrintError(this.fromTokens[fromTokenIndex - 1], "Improper 'from' aliasing with 'as'.");

                    tokenBuffer.Clear();
                }
                //check to ensure short alias is correct ie 'from base b'
                else if (tokenBuffer.Count() > 2)
                {
                    ErrorMessage.PrintError(this.fromTokens[fromTokenIndex - 1], "Improper 'from' aliasing, too many tokens after 'from'.");
                }
                else if (token.GetText() == "'" || token.GetText() == "\"")
                {
                    quoteLength = QuoteParser.Parse(this.fromTokens, fromTokenIndex);
                    // add dummy token to represent a parsed quote
                    tokenBuffer.Add(new Token("[quote]", this.fromTokens[fromTokenIndex].GetLineNumber()));
                }
                else
                    tokenBuffer.Add(token);

                fromTokenIndex++;
            }
        }

        public override List<Token> GetTokens()
        {
            return this.fromTokens;
        }
        public override int BuildClause(List<Token> tokens, int startIndex)
        {
            for (int i = startIndex; i < tokens.Count; ++i)
            {
                if (stopTokens.Contains(tokens[i].GetText())) break;
                this.fromTokens.Add(tokens[i]);
            }

            ValidPull();

            return this.fromTokens.Count();
        }
    }
}