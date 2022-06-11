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
        private static void tokenizeString(ref List<Token> tokens, string cleanToken, uint lineNumber, string stringDelimiter)
        {
            cleanToken = cleanToken.Substring(0, cleanToken.Length - stringDelimiter.Length);

            // incase of repeated delimiters, initially only take up to the first occurrence
            if (cleanToken.IndexOf(stringDelimiter) > 0) // if delimiter is not the only text on the line
                tokens.Add(new Token(cleanToken.Substring(0, cleanToken.IndexOf(stringDelimiter)), lineNumber));
            else
                tokens.Add(new Token(cleanToken, lineNumber));

            tokens.Add(new Token(stringDelimiter, lineNumber));

            // check for repeated delimiters, e.g. select wh_id,,,
            while (cleanToken.IndexOf(stringDelimiter) > 0)
            {
                tokens.Add(new Token(stringDelimiter, lineNumber));
                cleanToken = cleanToken.Substring(cleanToken.IndexOf(stringDelimiter) + 1);
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
                    if (cleanToken.IndexOf(',') > 0)
                        tokenizeString(ref tokens, cleanToken, lineNumber, ",");
                    else if (cleanToken.Length > 0) // ignore extra whitespaces that still pass through under the replace methods above
                        tokens.Add(new Token(cleanToken, lineNumber));
                }
                lineNumber++;
            }
            return tokens;
        }
    }
}
