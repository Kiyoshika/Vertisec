using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vertisec.Tokens
{
    public class Tokenizer
    {
        // main keywords
        // TODO: add missing keywords
        private static HashSet<string> keywords = new HashSet<string> { "select", "from", "where", "join", "group", "by", "as", "order" };

        // take a delimiter and split string and delimiter into their own tokens, e.g. "mytoken," --> "mytoken" and ","
        private static void TokenizeComma(ref List<Token> tokens, string cleanToken, uint lineNumber)
        {
            cleanToken = cleanToken.Substring(0, cleanToken.Length - 1);

            // incase of repeated delimiters, initially only take up to the first occurrence
            if (cleanToken.IndexOf(",") > 0) // if delimiter is not the only text on the line
                tokens.Add(new Token(cleanToken.Substring(0, cleanToken.IndexOf(",")), lineNumber));

            // if quote hangs at end, e.g. "select field as 'with quotes', ..."
            else if (cleanToken.IndexOf("'") >= 0)
                TokenizeSingleQuote(ref tokens, cleanToken, lineNumber);

            else if (cleanToken.IndexOf("\"") >= 0)
                TokenizeDoubleQuote(ref tokens, cleanToken, lineNumber);

            else
                tokens.Add(new Token(cleanToken, lineNumber));

            tokens.Add(new Token(",", lineNumber));

            // check for repeated delimiters, e.g. select wh_id,,,
            while (cleanToken.IndexOf(",") > 0)
            {
                tokens.Add(new Token(",", lineNumber));
                cleanToken = cleanToken.Substring(cleanToken.IndexOf(",") + 1);
            }
        }

        private static void TokenizeSingleQuote(ref List<Token> tokens, string cleanToken, uint lineNumber)
        {
            if (cleanToken[0] == '\'') // quote is at beginning of token
            {
                cleanToken = cleanToken.Substring(1);
                tokens.Add(new Token("'", lineNumber));
                tokens.Add(new Token(cleanToken, lineNumber));
            }
            
            if (cleanToken.Length >= 1 && cleanToken[cleanToken.Length - 1] == '\'') // quote is at end of token
            {
                cleanToken = cleanToken.Substring(0, cleanToken.Length - 1);
                tokens.Add(new Token(cleanToken, lineNumber));
                tokens.Add(new Token("'", lineNumber));
            }
        }

        private static void TokenizeDoubleQuote(ref List<Token> tokens, string cleanToken, uint lineNumber)
        {
            if (cleanToken[0] == '\"') // quote is at beginning of token
            {
                cleanToken = cleanToken.Substring(1);
                tokens.Add(new Token("\"", lineNumber));
                tokens.Add(new Token(cleanToken, lineNumber));
            }

            if (cleanToken[cleanToken.Length - 1] == '\"') // quote is at end of token
            {
                cleanToken = cleanToken.Substring(0, cleanToken.Length - 1);
                tokens.Add(new Token(cleanToken, lineNumber));
                tokens.Add(new Token("\"", lineNumber));
            }
        }

        public static List<Token> Tokenize(string[] sqlLines)
        {

            uint lineNumber = 1;
            List<Token> tokens = new List<Token>();
            string[] sqlLineSplit;
            foreach (string sqlLine in sqlLines)
            {
                sqlLineSplit = sqlLine.Split(' ');

                foreach(string token in sqlLineSplit)
                {
                    string cleanToken = token.Replace("\n", "").Replace("\t", "").Replace("\r", "").Replace(" ", "").Trim();

                    // if it's a main keyword, force it to lowercase
                    if (keywords.Contains(cleanToken.ToLower()))
                        cleanToken = cleanToken.ToLower();

                    // if token contains comma, e.g. "wh_id," split it into two tokens "wh_id" and ","
                    if (cleanToken.IndexOf(",") > 0)
                        TokenizeComma(ref tokens, cleanToken, lineNumber);
                    else if (cleanToken.IndexOf("'") >= 0)
                        TokenizeSingleQuote(ref tokens, cleanToken, lineNumber);
                    else if (cleanToken.IndexOf("\"") >= 0)
                        TokenizeSingleQuote(ref tokens, cleanToken, lineNumber);
                    else if (cleanToken.Length > 0) // ignore extra whitespaces that still pass through under the replace methods above
                        tokens.Add(new Token(cleanToken, lineNumber));
                }
                lineNumber++;
            }
            return tokens;
        }
    }
}
