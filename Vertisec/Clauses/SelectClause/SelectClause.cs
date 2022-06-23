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

namespace Vertisec.Clauses.SelectClause
{
    /**
     * SelectClause Parsing Rules
     * 1. Check that a "from" token exists
     * 2. If no "as" token is present, field length must be 2 (shorthand aliasing)
     * 3. If "as" token is present, must be token to left and right (regular aliasing)
     * 4. quote parsing
     * 5. shorthand cast parsing, e.g. abc::int -- would use a separate parser (like quotes) to check valid cast types
     */
    public class SelectClause : Clauses
    {
        private List<Token> selectTokens = new List<Token>();

        private void FromTokenExists(ref List<Token> tokens)
        {
            List<Token> tokensCopy = tokens.ToList<Token>();
            Token beginningToken = tokensCopy[0];
            foreach (Token token in tokensCopy)
            {
                // transfer tokens from "main" token pool to this select clause
                tokens.Remove(token);
                this.selectTokens.Add(token);

                if (token.GetText() == "from")
                    return;

            }

            throw new SelectClauseException("'from' token for 'select' not found.", beginningToken);
        }

        private void ValidAliasing()
        {
            int selectTokenIndex = 0;
            int quoteLength = 0;
            List<Token> tokenBuffer = new List<Token>();

            foreach (Token token in this.selectTokens)
            {

                // skip "select" token
                if (token.GetText() == "select")
                {
                    selectTokenIndex++;
                    continue;
                }

                // skip tokens if we are inside quotes
                if (quoteLength > 0)
                {
                    quoteLength--;
                    selectTokenIndex++;
                    continue;
                }

                // shorthand type casting (e.g. select tran_qty::int)
                if (token.GetText() == ":")
                {
                    CastParser.Parse(this.selectTokens, selectTokenIndex);
                    quoteLength = 2; // skip two tokens after a successful cast: {a, :, :, int}
                }
                else if (token.GetText() == "," || token.GetText() == "from")
                {
                    Token asToken = tokenBuffer.Find(tok => tok.GetText() == "as");

                    // regular aliasing (select a as b)
                    if (asToken != null && tokenBuffer.IndexOf(asToken) != 1 && tokenBuffer.Count() == 3)
                        throw new SelectClauseException("Improper column aliasing with 'as'.", this.selectTokens[selectTokenIndex - 1]);

                        // shorthand aliasing (select a b)
                    else if (asToken == null && tokenBuffer.Count() > 2 || tokenBuffer.Count() > 3)
                        throw new SelectClauseException("Improper column aliasing. Did you forget a comma?", this.selectTokens[selectTokenIndex - tokenBuffer.Count() + 2]);

                    // no columns specified (e.g. "select from")
                    else if (tokenBuffer.Count() == 0 && this.selectTokens[selectTokenIndex - 1].GetText() == "select")
                        throw new SelectClauseException("'select' has no columns.", this.selectTokens[selectTokenIndex - 1]);

                    // trailing commas such as "select wh_id, from"
                    else if (tokenBuffer.Count() == 0 && this.selectTokens[selectTokenIndex].GetText() == "from")
                        throw new SelectClauseException("Trailing comma.", this.selectTokens[selectTokenIndex - 1]);

                    // repeated commas such as "select wh_id,,,"
                    else if (tokenBuffer.Count() == 0)
                        throw new SelectClauseException("Repeated commas.", this.selectTokens[selectTokenIndex - 1]);

                    tokenBuffer.Clear();
                }
                // quote aliasing
                else if (token.GetText() == "'" || token.GetText() == "\"")
                {
                    quoteLength = QuoteParser.Parse(this.selectTokens, selectTokenIndex);
                    // add dummy token to represent a parsed quote
                    tokenBuffer.Add(new Token("[quote]", this.selectTokens[selectTokenIndex].GetLineNumber()));
                }
                else if (token.GetText() == "(" || token.GetText() == ")")
                {
                    Tuple<List<Token>, int> parenthesis = ParenthesisParser.Parse(this.selectTokens, selectTokenIndex, this.selectTokens[selectTokenIndex].GetText().ToCharArray()[0]);
                    quoteLength = parenthesis.Item2;
                }
                else
                    tokenBuffer.Add(token);

                selectTokenIndex++;
            }
        }

        public override List<Token> GetTokens()
        {
            return this.selectTokens;
        }

        public override int BuildClause(List<Token> tokens, int startIndex)
        {
            FromTokenExists(ref tokens);
            ValidAliasing();
            return this.selectTokens.Count();
        }
    }
}
