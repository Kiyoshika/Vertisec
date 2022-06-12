using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Vertisec.Util;
using Vertisec.Tokens;


namespace Vertisec.Parsers
{
    public class QuoteParser
    {
        public static int ParseQuotes(List<Token> tokens, int selectTokenIndex)
        {
            Token beginQuote = tokens[selectTokenIndex];
            int quoteLength = 0;

            for (int i = selectTokenIndex + 1; i < tokens.Count(); ++i)
            {
                if (tokens[i].GetText() == beginQuote.GetText())
                    return quoteLength + 1;
                quoteLength++;
            }
            
            Console.WriteLine("Missing end quote on line " + beginQuote.GetLineNumber());
            return 0;
        }
    }
}
