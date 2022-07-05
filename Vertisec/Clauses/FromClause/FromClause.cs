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

        private void ValidPull()
        {
            int fromTokenIndex = 0;
            int quoteLength = 0;
            List<Token> tokenBuffer = new List<Token>();

			for (int i = 0; i < this.fromTokens.Count(); i++)
            {
                // skip "from" token
                if (this.fromTokens[i].GetText() == "from")
                {
					if (i == this.fromTokens.Count() - 1)
					   throw new SyntaxException("'from' missing a table.", this.fromTokens[0]);	
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
				
				tokenBuffer.Add(this.fromTokens[i]);
			
				// if 'as' is final keyword	
				if (i == this.fromTokens.Count() - 1 && this.fromTokens[i].GetText() == "as")
					throw new SyntaxException("'as' expecting an alias.", this.fromTokens[i]);
				// check proper aliasing rules, e.g. "from x as y"
				else if (i > 0 && this.fromTokens[i - 1].GetText() == "as")
                {
					if (tokenBuffer.Count() < 3)
						throw new SyntaxException("Too few tokens for 'as'. Are you missing a table/alias?", this.fromTokens[i - 1]);

                    Token asToken = tokenBuffer.Find(tok => tok.GetText() == "as");
                    
                    if (tokenBuffer.IndexOf(asToken) != 1 && tokenBuffer.Count() == 3)
                        throw new SyntaxException("Improper 'from' aliasing with 'as'.", this.fromTokens[fromTokenIndex - 1]);
					else if (this.fromTokens.Count() - tokenBuffer.Count() > 1)
						throw new SyntaxException("Too many tokens after 'as'.", this.fromTokens[fromTokenIndex - 1]);
                }
                //check to ensure short alias is correct ie 'from base b'
                else if (tokenBuffer.Count() > 4)
                {
                    throw new SyntaxException("Improper 'from' aliasing, too many tokens after 'from'.", this.fromTokens[fromTokenIndex - 1]);
                }
                else if (this.fromTokens[i].GetText() == "'" || this.fromTokens[i].GetText() == "\"")
                {
                    quoteLength = QuoteParser.Parse(this.fromTokens, fromTokenIndex);
                    // add dummy token to represent a parsed quote
                    tokenBuffer.Add(new Token("[quote]", this.fromTokens[fromTokenIndex].GetLineNumber()));
                }

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
