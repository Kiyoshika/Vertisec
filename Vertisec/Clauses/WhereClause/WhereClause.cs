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
        private string[] rearrangedTokens(List<Token> tokenBuffer)
        {
            string[] tokens = new string[3];

            int logicalIndex = 0;
            foreach (Token _token in tokenBuffer)
                if (mathConditionalTokens.Contains(_token.GetText()))
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

        private void ValidateConditions()
        {
            int whereTokenIndex = 0;
            List<Token> tokenBuffer = new List<Token>();

            for (int i = 0; i < this.whereTokens.Count() + 1; ++i)
            {
                // skip "where" token
                if (i < this.whereTokens.Count() && this.whereTokens[i].GetText() == "where")
                {
                    whereTokenIndex++;
                    continue;
                }

                // conditions should always be three tokens long with the logical in the middle
                // only special case are parenthesis which we'll worry about later...
                if (tokenBuffer.Count() == 3)
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
                        int logicalIndex = 0;
                        foreach (Token _token in tokenBuffer)
                            if (mathConditionalTokens.Contains(_token.GetText()))
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

                        ErrorMessage.PrintError(tokenBuffer[1], "Misplaced logical. Did you mean '" + leftToken + " " + logicalToken + " " + rightToken + "'?");
                    }
                    else if (wordLogicalCount == 1 && !wordConditionalTokens.Contains(tokenBuffer[1].GetText()))
                    {
                        int logicalIndex = 0;
                        foreach (Token _token in tokenBuffer)
                            if (wordConditionalTokens.Contains(_token.GetText()))
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

                        ErrorMessage.PrintError(tokenBuffer[1], "Misplaced logical. Did you mean '" + leftToken + " " + logicalToken + " " + rightToken + "'?");
                    }
                }
                else
                {
                    tokenBuffer.Add(this.whereTokens[i]);
                    whereTokenIndex++;
                }
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
