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
        private static HashSet<string> sqlstart = new HashSet<string> { "select", "drop", "create", "with" };
        private static HashSet<string> specialTokens = new HashSet<string> { ":", "(", ")", "\"", "'", "," }; // NOTE: this orders special token highest -> lowest precedence
        
        private static bool ContainsSpecialToken(string cleanToken, ref string specialToken)
        {
            bool isContained = false;
            int minIndex = cleanToken.Length;
            foreach (string token in specialTokens)
            {
                if (cleanToken.Contains(token))
                {
                    if (cleanToken.IndexOf(token) < minIndex)
                    {
                        specialToken = token;
                        minIndex = cleanToken.IndexOf(token);
                    }
                    isContained = true;
                }
            }
            return isContained;
        }

        private static void SplitToken(ref List<Token> tokens, string cleanToken, uint lineNumber)
        {
            string specialToken = "";
            while (ContainsSpecialToken(cleanToken, ref specialToken))
            {
                string tokenText = cleanToken.Substring(0, cleanToken.IndexOf(specialToken));

                if (tokenText.Length > 0)
                    tokens.Add(new Token(tokenText, lineNumber));
                else if (tokenText.Length == 0)
                    tokens.Add(new Token(specialToken, lineNumber));

                // if clean token already starts with (, shift it over by one otherwise we end up in an infinite loop
                if (cleanToken[0] == specialToken.ToCharArray()[0])
                    cleanToken = cleanToken.Substring(cleanToken.IndexOf(specialToken) + 1);
                else
                    cleanToken = cleanToken.Substring(cleanToken.IndexOf(specialToken));
            }
            if (cleanToken.Length > 0)
                tokens.Add(new Token(cleanToken, lineNumber)); // add remaining text
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

                    string specialToken = "";
                    if (ContainsSpecialToken(cleanToken, ref specialToken))
                        SplitToken(ref tokens, cleanToken, lineNumber);
                    else if (cleanToken.Length > 0) // ignore extra whitespaces that still pass through under the replace methods above
                        tokens.Add(new Token(cleanToken, lineNumber));
                }
                lineNumber++;
            }
            return tokens;
        }
        public static bool sqlStartCheck(string sqlLine)
        {
            if (sqlstart.Contains(sqlLine))
                return true;
            else
                return false;
        }
    }
}
