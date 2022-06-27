using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Vertisec.Util;
using Vertisec.Tokens;
using Vertisec.Exceptions;
using Vertisec;


namespace Vertisec.Parsers
{
    public class QuoteParser
    {
        public static int Parse(List<Token> tokens, int tokenIndex)
        {
            Token beginQuote = tokens[tokenIndex];
            int quoteLength = 0;

            for (int i = tokenIndex + 1; i < tokens.Count(); ++i)
            {
                if (tokens[i].GetText() == beginQuote.GetText())
                    return quoteLength + 1;
                quoteLength++;
            }
            
            throw new SyntaxException("Missing end quote.", beginQuote);

            return 0;
        }
    }
}
