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
    internal class SelectClause : Clauses
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

            ErrorMessage.PrintError(beginningToken, "'from' token for 'select' not found.");
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
                        ErrorMessage.PrintError(this.selectTokens[selectTokenIndex - 1], "Improper column aliasing with 'as'.");
                        //Console.WriteLine("Improper column aliasing with 'as' on line " + this.selectTokens[selectTokenIndex - 1].GetLineNumber());

                    /** SIDE NOTE: after parsing quotes and casts, reduce their tokens to a length of at most three, e.g. {"jimmy", "as", "[quote]"} **/

                    // shorthand aliasing (select a b)
                    else if (asToken == null && tokenBuffer.Count() > 2 || tokenBuffer.Count() > 3)
                        ErrorMessage.PrintError(this.selectTokens[selectTokenIndex - tokenBuffer.Count() + 2], "Improper column aliasing. Did you forget a comma?");
                        //Console.WriteLine("Improper column aliasing on line " + this.selectTokens[selectTokenIndex - tokenBuffer.Count() + 2].GetLineNumber() + ". Did you forget a comma?");

                    // no columns specified (e.g. "select from" or "select , from")
                    else if (tokenBuffer.Count() == 0 && this.selectTokens[selectTokenIndex].GetText() == "select")
                        ErrorMessage.PrintError(this.selectTokens[selectTokenIndex - 1], "'select' has no columns.");

                    // trailing commas such as "select wh_id, from"
                    else if (tokenBuffer.Count() == 0 && this.selectTokens[selectTokenIndex].GetText() == "from")
                        ErrorMessage.PrintError(this.selectTokens[selectTokenIndex - 1], "Trailing comma.");

                    // repeated commas such as "select wh_id,,,"
                    else if (tokenBuffer.Count() == 0)
                        ErrorMessage.PrintError(this.selectTokens[selectTokenIndex - 1], "Repeated commas.");

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
