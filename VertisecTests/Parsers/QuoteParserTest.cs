using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Vertisec.Parsers;
using Vertisec.Tokens;
using Vertisec.Exceptions;

namespace VertisecTests.Parsers
{
    [TestClass]
    public class QuoteParserTest
    {
        [TestMethod]
        public void HangingQuote()
        {
            string[] sampleText = { "'this is a sample quote left open" };
            List<Token> quoteTokens = Tokenizer.Tokenize(sampleText);
            SyntaxException se = Assert.ThrowsException<SyntaxException>(() => QuoteParser.Parse(quoteTokens, 0));
            Assert.AreEqual(se.Message, "Missing end quote.");
        }

        [TestMethod]
        public void InnerTokenCountWithoutOffset()
        {
            string[] sampleText = { "'this is a sample quote'" };
            List<Token> quoteTokens = Tokenizer.Tokenize(sampleText);
            int tokenCount = QuoteParser.Parse(quoteTokens, 0);
            Assert.AreEqual(tokenCount, 6); // extra +1 to move past the end quote 
        }


        [TestMethod]
        public void InnerTokenCountWithOffset()
        {
            string[] sampleText = { "some random text and 'this is a sample quote' I like cats" };
            List<Token> quoteTokens = Tokenizer.Tokenize(sampleText);
            int tokenCount = QuoteParser.Parse(quoteTokens, 4);
            Assert.AreEqual(tokenCount, 6); // extra +1 to move past the end quote
        }
    }
}
