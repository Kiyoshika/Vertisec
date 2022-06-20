using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Vertisec.Tokens;

namespace VertisecTests.Tokens
{
    [TestClass]
    public class TokenizerTest : Tokenizer
    {
        [TestMethod]
        public void ContainsSpecialToken()
        {
            string specialToken = "";
            Tokenizer tokenizer = new Tokenizer();
            
            foreach (string tokenText in Tokenizer.specialTokens)
            {
                Assert.IsTrue(Tokenizer.ContainsSpecialToken(tokenText, ref specialToken));
                Assert.AreEqual(tokenText, specialToken); // the found token will be written to special token
            }
            Assert.IsFalse(Tokenizer.ContainsSpecialToken("nonExistent", ref specialToken));
        }

        [TestMethod]
        public void SplitTokenComma()
        {
            List<Token> tokens = new List<Token>();
            Tokenizer.SplitToken(ref tokens, ",tok,,en,", (uint)1);

            Assert.AreEqual(tokens[0].GetText(), ",");
            Assert.AreEqual(tokens[0].GetLineNumber(), (uint)1);

            Assert.AreEqual(tokens[1].GetText(), "tok");
            Assert.AreEqual(tokens[1].GetLineNumber(), (uint)1);

            Assert.AreEqual(tokens[2].GetText(), ",");
            Assert.AreEqual(tokens[2].GetLineNumber(), (uint)1);

            Assert.AreEqual(tokens[3].GetText(), ",");
            Assert.AreEqual(tokens[3].GetLineNumber(), (uint)1);

            Assert.AreEqual(tokens[4].GetText(), "en");
            Assert.AreEqual(tokens[4].GetLineNumber(), (uint)1);

            Assert.AreEqual(tokens[5].GetText(), ",");
            Assert.AreEqual(tokens[5].GetLineNumber(), (uint)1);
        }

        [TestMethod]
        public void SplitTokenOpenParenthesis()
        {
            List<Token> tokens = new List<Token>();
            Tokenizer.SplitToken(ref tokens, "(tok((en(", (uint)1);

            Assert.AreEqual(tokens[0].GetText(), "(");
            Assert.AreEqual(tokens[0].GetLineNumber(), (uint)1);

            Assert.AreEqual(tokens[1].GetText(), "tok");
            Assert.AreEqual(tokens[1].GetLineNumber(), (uint)1);

            Assert.AreEqual(tokens[2].GetText(), "(");
            Assert.AreEqual(tokens[2].GetLineNumber(), (uint)1);

            Assert.AreEqual(tokens[3].GetText(), "(");
            Assert.AreEqual(tokens[3].GetLineNumber(), (uint)1);

            Assert.AreEqual(tokens[4].GetText(), "en");
            Assert.AreEqual(tokens[4].GetLineNumber(), (uint)1);

            Assert.AreEqual(tokens[5].GetText(), "(");
            Assert.AreEqual(tokens[5].GetLineNumber(), (uint)1);
        }

        [TestMethod]
        public void SplitTokenCloseParenthesis()
        {
            List<Token> tokens = new List<Token>();
            Tokenizer.SplitToken(ref tokens, ")tok))en)", (uint)1);

            Assert.AreEqual(tokens[0].GetText(), ")");
            Assert.AreEqual(tokens[0].GetLineNumber(), (uint)1);

            Assert.AreEqual(tokens[1].GetText(), "tok");
            Assert.AreEqual(tokens[1].GetLineNumber(), (uint)1);

            Assert.AreEqual(tokens[2].GetText(), ")");
            Assert.AreEqual(tokens[2].GetLineNumber(), (uint)1);

            Assert.AreEqual(tokens[3].GetText(), ")");
            Assert.AreEqual(tokens[3].GetLineNumber(), (uint)1);

            Assert.AreEqual(tokens[4].GetText(), "en");
            Assert.AreEqual(tokens[4].GetLineNumber(), (uint)1);

            Assert.AreEqual(tokens[5].GetText(), ")");
            Assert.AreEqual(tokens[5].GetLineNumber(), (uint)1);
        }

        [TestMethod]
        public void SplitTokenColon()
        {
            List<Token> tokens = new List<Token>();
            Tokenizer.SplitToken(ref tokens, ":tok::en:", (uint)1);

            Assert.AreEqual(tokens[0].GetText(), ":");
            Assert.AreEqual(tokens[0].GetLineNumber(), (uint)1);

            Assert.AreEqual(tokens[1].GetText(), "tok");
            Assert.AreEqual(tokens[1].GetLineNumber(), (uint)1);

            Assert.AreEqual(tokens[2].GetText(), ":");
            Assert.AreEqual(tokens[2].GetLineNumber(), (uint)1);

            Assert.AreEqual(tokens[3].GetText(), ":");
            Assert.AreEqual(tokens[3].GetLineNumber(), (uint)1);

            Assert.AreEqual(tokens[4].GetText(), "en");
            Assert.AreEqual(tokens[4].GetLineNumber(), (uint)1);

            Assert.AreEqual(tokens[5].GetText(), ":");
            Assert.AreEqual(tokens[5].GetLineNumber(), (uint)1);
        }

        [TestMethod]
        public void SplitTokenDoubleQuote()
        {
            List<Token> tokens = new List<Token>();
            Tokenizer.SplitToken(ref tokens, "\"tok\"\"en\"", (uint)1);

            Assert.AreEqual(tokens[0].GetText(), "\"");
            Assert.AreEqual(tokens[0].GetLineNumber(), (uint)1);

            Assert.AreEqual(tokens[1].GetText(), "tok");
            Assert.AreEqual(tokens[1].GetLineNumber(), (uint)1);

            Assert.AreEqual(tokens[2].GetText(), "\"");
            Assert.AreEqual(tokens[2].GetLineNumber(), (uint)1);

            Assert.AreEqual(tokens[3].GetText(), "\"");
            Assert.AreEqual(tokens[3].GetLineNumber(), (uint)1);

            Assert.AreEqual(tokens[4].GetText(), "en");
            Assert.AreEqual(tokens[4].GetLineNumber(), (uint)1);

            Assert.AreEqual(tokens[5].GetText(), "\"");
            Assert.AreEqual(tokens[5].GetLineNumber(), (uint)1);
        }

        [TestMethod]
        public void SplitTokenSingleQuote()
        {
            List<Token> tokens = new List<Token>();
            Tokenizer.SplitToken(ref tokens, "'tok''en'", (uint)1);

            Assert.AreEqual(tokens[0].GetText(), "'");
            Assert.AreEqual(tokens[0].GetLineNumber(), (uint)1);

            Assert.AreEqual(tokens[1].GetText(), "tok");
            Assert.AreEqual(tokens[1].GetLineNumber(), (uint)1);

            Assert.AreEqual(tokens[2].GetText(), "'");
            Assert.AreEqual(tokens[2].GetLineNumber(), (uint)1);

            Assert.AreEqual(tokens[3].GetText(), "'");
            Assert.AreEqual(tokens[3].GetLineNumber(), (uint)1);

            Assert.AreEqual(tokens[4].GetText(), "en");
            Assert.AreEqual(tokens[4].GetLineNumber(), (uint)1);

            Assert.AreEqual(tokens[5].GetText(), "'");
            Assert.AreEqual(tokens[5].GetLineNumber(), (uint)1);
        }

        [TestMethod]
        public void SplitTokenMultipleSpecialTokens()
        {
            List<Token> tokens = new List<Token>();
            Tokenizer.SplitToken(ref tokens, ",(:,)\"'", (uint)1);

            Assert.AreEqual(tokens[0].GetText(), ",");
            Assert.AreEqual(tokens[0].GetLineNumber(), (uint)1);

            Assert.AreEqual(tokens[1].GetText(), "(");
            Assert.AreEqual(tokens[1].GetLineNumber(), (uint)1);

            Assert.AreEqual(tokens[2].GetText(), ":");
            Assert.AreEqual(tokens[2].GetLineNumber(), (uint)1);

            Assert.AreEqual(tokens[3].GetText(), ",");
            Assert.AreEqual(tokens[3].GetLineNumber(), (uint)1);

            Assert.AreEqual(tokens[4].GetText(), ")");
            Assert.AreEqual(tokens[4].GetLineNumber(), (uint)1);

            Assert.AreEqual(tokens[5].GetText(), "\"");
            Assert.AreEqual(tokens[5].GetLineNumber(), (uint)1);

            Assert.AreEqual(tokens[6].GetText(), "'");
            Assert.AreEqual(tokens[6].GetLineNumber(), (uint)1);
        }

        [TestMethod]
        public void TokenizeNormal()
        {
            string[] lines =
            {
                "hello there",
                "this is a sentence",
                "and another one"
            };

            List<Token> tokens = Tokenizer.Tokenize(lines);
            string[] expectedTokenText = { "hello", "there", "this", "is", "a", "sentence", "and", "another", "one" };
            int indexCounter = 0;
            foreach (Token token in tokens)
            {
                Assert.AreEqual(token.GetText(), expectedTokenText[indexCounter]);

                // check correct line numbers
                if (indexCounter >= 0 && indexCounter <= 1)
                    Assert.AreEqual(token.GetLineNumber(), (uint)1);
                else if (indexCounter >= 2 && indexCounter <= 5)
                    Assert.AreEqual(token.GetLineNumber(), (uint)2);
                else
                    Assert.AreEqual(token.GetLineNumber(), (uint)3);

                indexCounter++;
            }
        }

        [TestMethod]
        public void TokenizeUnclean()
        {
            string[] lines =
            {
                "hello     there\n",
                "\t\tthis     is \ta \nsente\nnce",
                "and \t\t\nanother     \rone\n"
            };

            List<Token> tokens = Tokenizer.Tokenize(lines);
            string[] expectedTokenText = { "hello", "there", "this", "is", "a", "sentence", "and", "another", "one" };
            int indexCounter = 0;
            foreach (Token token in tokens)
            {
                Assert.AreEqual(token.GetText(), expectedTokenText[indexCounter]);

                // check correct line numbers
                if (indexCounter >= 0 && indexCounter <= 1)
                    Assert.AreEqual(token.GetLineNumber(), (uint)1);
                else if (indexCounter >= 2 && indexCounter <= 5)
                    Assert.AreEqual(token.GetLineNumber(), (uint)2);
                else
                    Assert.AreEqual(token.GetLineNumber(), (uint)3);

                indexCounter++;
            }
        }
    }
}
