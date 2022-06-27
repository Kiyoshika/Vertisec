using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Vertisec.Tokens;
using Vertisec.Exceptions;
using Vertisec.Parsers;
using Vertisec.Clauses.SelectClause;

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
        private string[] RearrangedTokens(List<Token> tokenBuffer, HashSet<string> conditionalTokens)
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

        private void ParseCondition(List<Token> tokens)
        {
            List<Token> tokenBuffer = new List<Token>();
            int tokenIndex = 0;

            for (int i = 0; i < tokens.Count(); ++i)
            {

                // skip "where" token
                if (tokens[i].GetText() == "where")
                {
                    tokenIndex++;
                    continue;
                };

                // add non-chaining words (and + or) to the token buffer
                if (!this.conditionChainWords.Contains(tokens[i].GetText()))
                    tokenBuffer.Add(tokens[i]);

                // parenthesis parsing, e.g. where x in (select ... ) or where (x = 5 and y < 3) or ...
                if (tokens[i].GetText() == "(")
                {
                    Tuple<List<Token>, int> parenthesis = ParenthesisParser.Parse(tokens, i, '(');
                    ParseInnerParenthesis(parenthesis.Item1);
                    tokenIndex += parenthesis.Item2 + 1; // extra +1 to skip closing parenthesis
                    i += parenthesis.Item2 + 1; // extra +1 to skip closing parenthesis
                }

                // quote parsing e.g. where x like 'wild%'
                else if (tokens[i].GetText() == "'" || tokens[i].GetText() == "\"")
                {
                    int quoteLen = QuoteParser.Parse(tokens, i);
                    tokenIndex += quoteLen + 1; // extra +1 to skip end quote
                    i += quoteLen + 1; // extra +1 to skip end quote
                }

                // for each chaining keyword (or end of condition), check token buffer to validate correct syntax
                else if (tokens[i].GetText() == "and" || tokens[i].GetText() == "or" || i == tokens.Count() - 1)
                {
                    // negated conditions "A not like B"
                    if (tokenBuffer.Count() == 4 && tokenBuffer.Find(tok => tok.GetText() == "not") != null)
                    {
                        if (tokenBuffer[1].GetText() != "not")
                            throw new SyntaxException("Negation token 'not' is in the incorrect position.", tokenBuffer[1]);
                        else
                        {
                            if (!wordConditionalTokens.Contains(tokenBuffer[2].GetText())) // avoiding "not not"
                                throw new SyntaxException("Invalid negation condition. Expecting 'not like', 'not ilike', 'not in', etc.", tokenBuffer[2]);
                            if (tokenBuffer[2].GetText() == "not")
                                throw new SyntaxException("Double negative 'not not'. Expecting 'not like', 'not ilike', 'not in', etc.", tokenBuffer[2]);
                        }
                    }

                    else if (tokenBuffer.Count() == 4 && tokenBuffer.Find(tok => tok.GetText() == "not") == null)
                        throw new SyntaxException("Condition too long. Are you missing 'and' or 'or'?", tokens[i]);

                    else if (tokenBuffer.Count() == 3 && tokenBuffer.Find(tok => tok.GetText() == "not") != null)
                    {
                        Token notToken = tokenBuffer.Find(tok => tok.GetText() == "not");
                        throw new SyntaxException("Incomplete negation condition. Expecting 'not like', 'not ilike', 'not in', etc.", notToken);
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
                            throw new SyntaxException("Too many logicals in condition. Only expecting one logical (=, >, like, etc.) per condition.", tokenBuffer[0]);

                        // logical should be in middle of condition (e.g A = B)
                        if (mathLogicalCount == 1 && !mathConditionalTokens.Contains(tokenBuffer[1].GetText()))
                        {
                            string[] _rearrangedTokens = RearrangedTokens(tokenBuffer, mathConditionalTokens);
                            throw new SyntaxException("Misplaced logical. Did you mean '" + _rearrangedTokens[0] + " " + _rearrangedTokens[1] + " " + _rearrangedTokens[2] + "'?", tokenBuffer[1]);
                        }
                        else if (wordLogicalCount == 1 && !wordConditionalTokens.Contains(tokenBuffer[1].GetText()))
                        {
                            string[] _rearrangedTokens = RearrangedTokens(tokenBuffer, wordConditionalTokens);
                            throw new SyntaxException("Misplaced logical. Did you mean '" + _rearrangedTokens[0] + " " + _rearrangedTokens[1] + " " + _rearrangedTokens[2] + "'?", tokenBuffer[1]);
                        }
                        // validate correct logicals being used, e.g A = B instead of A == B
                        else if (wordLogicalCount == 0 && mathLogicalCount == 0)
                            throw new SyntaxException("Incorrect logical expression. Expecting '=', '<', '<=', etc.", tokenBuffer[1]);

                        tokenBuffer.Clear();
                        //whereTokenIndex++;
                    }

                    else if (tokenBuffer.Count() < 3)
                        throw new SyntaxException("Condition too short.", tokens[i]);

                    tokenBuffer.Clear();
                }
                else if (tokenBuffer.Count() > 4)
                    throw new SyntaxException("Condition too long.", tokens[i]);
            }
        }

        private void ParseInnerParenthesis(List<Token> innerTokens)
        {
            bool containsConditional = false;

            if (innerTokens[0].GetText() == "select")
            {
                // call SelectClause parser
                SelectClause.SelectClause sc = new SelectClause.SelectClause();
                sc.BuildClause(innerTokens, 0);
                return;
            }

            foreach (Token _token in innerTokens)
            {
                if (mathConditionalTokens.Contains(_token.GetText()) || wordConditionalTokens.Contains(_token.GetText()))
                {
                    containsConditional = true;
                    break;
                }
            }

            if (containsConditional)
                ParseCondition(innerTokens);
            else
                throw new SyntaxException("Invalid inner expression. Expecting subquery or valid logicals such as x = y.", innerTokens[0]);
        }

        private void ValidateConditions()
        {
            ParseCondition(this.whereTokens);
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
