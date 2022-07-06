using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Vertisec.Tokens;
using Vertisec.Errors;
using Vertisec.Exceptions;

namespace Vertisec.Parsers
{
    public class ParenthesisParser
    {
        private static Tuple<List<Token>, int> ParseOpen(List<Token> tokens, int tokenIndex)
        {
            int parenthesisCounter = 0; // assuming we start with a parenthesis
            int tokenCounter = 0; // counts the tokens inside the parenthesis
            List<Token> innerTokens = new List<Token>();
            Tuple<List<Token>, int> parenthesisTuple;

            for (int i = tokenIndex; i < tokens.Count(); ++i)
            {
                innerTokens.Add(tokens[i]);
                tokenCounter++;

                if (tokens[i].GetText() == "(")
                    parenthesisCounter++;
                else if (tokens[i].GetText() == ")")
                    parenthesisCounter--;

                if (parenthesisCounter == 0)
                {
                    parenthesisTuple = new Tuple<List<Token>, int>(innerTokens, tokenCounter);
                    return parenthesisTuple;
                }

            }

            if (parenthesisCounter != 0)
                throw new SyntaxException("Missing closing parenthesis.", tokens[tokenIndex]);

            return parenthesisTuple = new Tuple<List<Token>, int>(null, 0); // empty tuple
        }

        private static Tuple<List<Token>, int> ParseClosed(List<Token> tokens, int tokenIndex)
        {
            int parenthesisCounter = 0; // assuming we start with a parenthesis
            int tokenCounter = 0; // counts the tokens inside the parenthesis
            List<Token> innerTokens = new List<Token>();
            Tuple<List<Token>, int> parenthesisTuple;

            for (int i = tokenIndex; i > 0; i--)
            {
                if (tokens[i].GetText() == ")")
                    parenthesisCounter++;
                else if (tokens[i].GetText() == "(")
                    parenthesisCounter--;

                if (parenthesisCounter == 0)
                {
                    parenthesisTuple = new Tuple<List<Token>, int>(innerTokens, tokenCounter);
                    return parenthesisTuple;
                }
                else
                {
                    innerTokens.Add(tokens[i]);
                    tokenCounter++;
                }
            }

            if (parenthesisCounter != 0)
                throw new SyntaxException("Missing opening parenthesis.", tokens[tokenIndex]);

            return parenthesisTuple = new Tuple<List<Token>, int>(null, 0); // empty tuple
        }

        public static Tuple<List<Token>, int> Parse(List<Token> tokens, int tokenIndex, char startingParentehsis)
        {
            switch (startingParentehsis)
            {
                case '(':
                    return ParseOpen(tokens, tokenIndex);
                    break;

                case ')':
                    return ParseClosed(tokens, tokenIndex);
                    break;
            }

            return null; // technically unreachable (if used properly)
        }
    }
}
