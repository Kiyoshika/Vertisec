using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Vertisec.Tokens;
using Vertisec.Util;
using Vertisec.Parsers;
using Vertisec.Errors;

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
        private static uint errorLineNumbers = 3; // display +/- 2 lines of the original SQL when displaying error messages

        private void FromTokenExists(ref List<Token> tokens)
        {
            List<Token> tokensCopy = tokens.ToList<Token>();
            Token beginning_token = tokensCopy[0];
            foreach (Token token in tokensCopy)
            {
                // transfer tokens from "main" token pool to this select clause
                tokens.Remove(token);
                this.selectTokens.Add(token);

                if (token.GetText() == "from")
                    return;

            }

            InternalErrorMessage.PrintError("'select' is missing 'from' token on line " + beginning_token.GetLineNumber());
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
                        ErrorMessage.PrintError(base.GetOriginalSQL(), this.selectTokens[selectTokenIndex - 1], SelectClause.errorLineNumbers, "Improper column aliasing with 'as'.");
                        //Console.WriteLine("Improper column aliasing with 'as' on line " + this.selectTokens[selectTokenIndex - 1].GetLineNumber());

                    /** SIDE NOTE: after parsing quotes and casts, reduce their tokens to a length of at most three, e.g. {"jimmy", "as", "[quote]"} **/

                    // shorthand aliasing (select a b)
                    else if (asToken == null && tokenBuffer.Count() > 2 || tokenBuffer.Count() > 3)
                        ErrorMessage.PrintError(base.GetOriginalSQL(), this.selectTokens[selectTokenIndex - tokenBuffer.Count() + 2], SelectClause.errorLineNumbers, "Improper column aliasing. Did you forget a comma?");
                        //Console.WriteLine("Improper column aliasing on line " + this.selectTokens[selectTokenIndex - tokenBuffer.Count() + 2].GetLineNumber() + ". Did you forget a comma?");

                    // no columns specified (e.g. "select from" or "select , from")
                    else if (tokenBuffer.Count() == 0 && this.selectTokens[selectTokenIndex].GetText() == "select")
                        Console.WriteLine("'select' has no columns on line " + this.selectTokens[selectTokenIndex - 1].GetLineNumber());

                    // trailing commas such as "select wh_id, from"
                    else if (tokenBuffer.Count() == 0 && this.selectTokens[selectTokenIndex].GetText() == "from")
                        Console.WriteLine("Trailing comma on line " + this.selectTokens[selectTokenIndex - 1].GetLineNumber());

                    // repeated commas such as "select wh_id,,,"
                    else if (tokenBuffer.Count() == 0)
                        Console.WriteLine("Repeated commas on line " + this.selectTokens[selectTokenIndex - 1].GetLineNumber());

                    tokenBuffer.Clear();
                }
                // quote aliasing
                else if (token.GetText() == "'" || token.GetText() == "\"")
                {
                    quoteLength = QuoteParser.Parse(this.selectTokens, selectTokenIndex);
                    // add dummy token to represent a parsed quote
                    tokenBuffer.Add(new Token("[quote]", this.selectTokens[selectTokenIndex].GetLineNumber()));
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

        public override void BuildClause(ref List<Token> tokens, string[] originalSQL)
        {
            base.SetOriginalSQL(originalSQL);
            FromTokenExists(ref tokens);
            ValidAliasing();
        }
    }
}
