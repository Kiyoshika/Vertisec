using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Vertisec.Tokens;
using Vertisec.Errors;

namespace Vertisec.Clauses.WhereClause
{
    internal class WhereClause : Clauses
    {
        private List<Token> whereTokens = new List<Token>();
        private HashSet<string> stopTokens = new HashSet<string>
        {
            "order",
            "group",
            "having",
            "limit"
        };
        
        private HashSet<string> mathConditionalTokens = new HashSet<string>
        {
            "=", "!=", "<>",
            ">", ">=",
            "<", "<="
        };

        private HashSet<string> wordConditionalTokens = new HashSet<string>
        {
            "not",
            "like", "ilike",
            "in"
        };

        private HashSet<string> conditionChainWords = new HashSet<string>
        {
            "and", "or"
        };

        // take token buffer and rearrange tokens if the logical was misplaced
        // e.g. "= a b" --> "a = b"
        // logical token will be in middle, left/right tokens will be in 0/2 index respectively.
        private string[] rearrangedTokens(List<Token> tokenBuffer, HashSet<string> conditionalTokens)
        {
            string[] tokens = new string[3];

            int logicalIndex = 0;
            foreach (Token _token in tokenBuffer)
                if (conditionalTokens.Contains(_token.GetText()))
                    break;
                else
                    logicalIndex++;

            // default values
            string leftToken = tokenBuffer[0].GetText();
            string logicalToken = tokenBuffer[1].GetText();
            string rightToken = tokenBuffer[2].GetText();

            // rearrange token values if the logical is misplaced (used in the error message)
            if (logicalIndex == 0)
            {
                logicalToken = tokenBuffer[0].GetText();
                leftToken = tokenBuffer[1].GetText();
                rightToken = tokenBuffer[2].GetText();
            }
            else if (logicalIndex == 2)
            {
                logicalToken = tokenBuffer[2].GetText();
                leftToken = tokenBuffer[0].GetText();
                rightToken = tokenBuffer[1].GetText();
            }

            tokens[0] = leftToken;
            tokens[1] = logicalToken;
            tokens[2] = rightToken;

            return tokens;
        }

        //
        //
        // TODO: fix multiple conditions (e.g. chaining with and & or)
        //
        //
        private void ValidateConditions()
        {
            int whereTokenIndex = 0;
            List<Token> tokenBuffer = new List<Token>();

            // NOTE: need the extra +1 to build the token buffer of 3 in case there are exactly 3 tokens
            for (int i = 0; i < this.whereTokens.Count(); ++i)
            {

                // skip "where" token
                if (this.whereTokens[i].GetText() == "where")
                {
                    whereTokenIndex++;
                    continue;
                };

                // add non-chaining words (and + or) to the token buffer
                if (!this.conditionChainWords.Contains(this.whereTokens[i].GetText()))
                    tokenBuffer.Add(this.whereTokens[i]);

                if (this.whereTokens[i].GetText() == "and" || this.whereTokens[i].GetText() == "or" || i == this.whereTokens.Count() - 1)
                {
                    // negated conditions "A not like B"
                    // only special case are parenthesis which we'll worry about later...
                    if (tokenBuffer.Count() == 4 && tokenBuffer.Find(tok => tok.GetText() == "not") != null)
                    {
                        if (tokenBuffer[1].GetText() != "not")
                            ErrorMessage.PrintError(tokenBuffer[1], "Negation token 'not' is in the incorrect position.");
                        else
                        {
                            if (!wordConditionalTokens.Contains(tokenBuffer[2].GetText())) // avoiding "not not"
                                ErrorMessage.PrintError(tokenBuffer[2], "Invalid negation condition. Expecing 'not like', 'not ilike', 'not in', etc.");
                            if (tokenBuffer[2].GetText() == "not")
                                ErrorMessage.PrintError(tokenBuffer[2], "Double negative 'not not'. Expecting 'not like', 'not ilike', 'not in', etc.");
                        }
                    }

                    else if (tokenBuffer.Count() == 4 && tokenBuffer.Find(tok => tok.GetText() == "not") == null)
                        ErrorMessage.PrintError(this.whereTokens[i], "Condition too long. Are you missing 'and' or 'or'?");

                    else if (tokenBuffer.Count() == 3 && tokenBuffer.Find(tok => tok.GetText() == "not") != null)
                    {
                        Token notToken = tokenBuffer.Find(tok => tok.GetText() == "not");
                        ErrorMessage.PrintError(notToken, "Incomplete negation condition. Expecting 'not like', 'not ilike', 'not in', etc.");
                    }

                    // standard conditions (three tokens) -- A = B
                    // only special case are parenthesis which we'll worry about later...
                    else if (tokenBuffer.Count() == 3 && tokenBuffer.Find(tok => tok.GetText() == "not") == null)
                    {
                        int mathLogicalCount = 0, wordLogicalCount = 0;

                        foreach (Token _token in tokenBuffer)
                        {
                            if (mathConditionalTokens.Contains(_token.GetText()))
                                mathLogicalCount++;
                            if (wordConditionalTokens.Contains(_token.GetText()))
                                wordLogicalCount++;
                        }

                        if (mathLogicalCount + wordLogicalCount > 1)
                            ErrorMessage.PrintError(tokenBuffer[0], "Too many logicals in condition. Only expecting one logical (=, >, like, etc.) per condition.");

                        // logical should be in middle of condition (e.g A = B)
                        if (mathLogicalCount == 1 && !mathConditionalTokens.Contains(tokenBuffer[1].GetText()))
                        {
                            string[] _rearrangedTokens = rearrangedTokens(tokenBuffer, mathConditionalTokens);
                            ErrorMessage.PrintError(tokenBuffer[1], "Misplaced logical. Did you mean '" + _rearrangedTokens[0] + " " + _rearrangedTokens[1] + " " + _rearrangedTokens[2] + "'?");
                        }
                        else if (wordLogicalCount == 1 && !wordConditionalTokens.Contains(tokenBuffer[1].GetText()))
                        {
                            string[] _rearrangedTokens = rearrangedTokens(tokenBuffer, wordConditionalTokens);
                            ErrorMessage.PrintError(tokenBuffer[1], "Misplaced logical. Did you mean '" + _rearrangedTokens[0] + " " + _rearrangedTokens[1] + " " + _rearrangedTokens[2] + "'?");
                        }

                        // validate correct logicals being used, e.g A = B instead of A == B
                        else if (wordLogicalCount == 0 && mathLogicalCount == 0)
                            ErrorMessage.PrintError(tokenBuffer[1], "Incorrect logical expression. Expecting '=', '<', '<=', etc.");

                        tokenBuffer.Clear();
                        //whereTokenIndex++;
                    }

                    else if (tokenBuffer.Count() < 3)
                        ErrorMessage.PrintError(this.whereTokens[i], "Condition too short.");

                    tokenBuffer.Clear();
                }
                else if (tokenBuffer.Count() > 4)
                    ErrorMessage.PrintError(this.whereTokens[i], "Condition too long.");
            }
        }

        public override List<Token> GetTokens()
        {
            return this.whereTokens;
        }

        public override int BuildClause(List<Token> tokens, int startIndex)
        {
            for (int i = startIndex; i < tokens.Count; ++i)
            {
                if (stopTokens.Contains(tokens[i].GetText())) break;
                this.whereTokens.Add(tokens[i]);
            }

            ValidateConditions();

            return this.whereTokens.Count();
        }
    }
}
