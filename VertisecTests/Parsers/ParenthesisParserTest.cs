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
    public class ParenthesisParserTest
    {
        [TestMethod]
        public void HangingOpenParenthesis()
        {
            string[] parenText = { "(a b c" };
            List<Token> parenTokens = Tokenizer.Tokenize(parenText);
            SyntaxException se = Assert.ThrowsException<SyntaxException>(() => ParenthesisParser.Parse(parenTokens, 0, '('));
            Assert.AreEqual(se.Message, "Missing closing parenthesis.");
        }

        [TestMethod]
        public void HangingCloseParenthesis()
        {
            string[] parenText = { "a b c)" };
            List<Token> parenTokens = Tokenizer.Tokenize(parenText);
            SyntaxException se = Assert.ThrowsException<SyntaxException>(() => ParenthesisParser.Parse(parenTokens, 3, ')'));
            Assert.AreEqual(se.Message, "Missing opening parenthesis.");
        }

        [TestMethod]
        public void InnerTokensWithoutOffset()
        {
            string[] parenText = { "(a b c)" };
            List<Token> parenTokens = Tokenizer.Tokenize(parenText);
            Tuple<List<Token>, int> parsedParens = ParenthesisParser.Parse(parenTokens, 0, '(');
            for (int i = 0; i < parsedParens.Item1.Count(); i++)
                Assert.AreEqual(parsedParens.Item1[i].GetText(), parenTokens[i].GetText());
        }

        [TestMethod]
        public void InnerTokensWithOffset()
        {
            string[] parenText = { "hello there (a b c) this is the end" };
            List<Token> parenTokens = Tokenizer.Tokenize(parenText);
            Tuple<List<Token>, int> parsedParens = ParenthesisParser.Parse(parenTokens, 2, '(');
            for (int i = 0; i < parsedParens.Item1.Count(); i++)
                Assert.AreEqual(parsedParens.Item1[i].GetText(), parenTokens[i + 2].GetText());
        }

        [TestMethod]
        public void InnerTokenCountWithoutOffset()
        {
            string[] parenText = { "(a b c)" };
            List<Token> parenTokens = Tokenizer.Tokenize(parenText);
            Tuple<List<Token>, int> parsedParens = ParenthesisParser.Parse(parenTokens, 0, '(');
            Assert.AreEqual(parsedParens.Item2, 5);
        }

        [TestMethod]
        public void InnerTokenCountWithOffset()
        {
            string[] parenText = { "hello there (a b c) this is the end" };
            List<Token> parenTokens = Tokenizer.Tokenize(parenText);
            Tuple<List<Token>, int> parsedParens = ParenthesisParser.Parse(parenTokens, 2, '(');
            Assert.AreEqual(parsedParens.Item2, 5); // {"(", "a", "b", "c", "}"} 
        }

        [TestMethod]
        public void NestedParenthesisInnerTokens()
        {
            // nested parenthesis will return all tokens starting from the outermost brackets
            // i.e., (a b (c d e) f)

            string[] parenText = { "hello there (a b (c d e) f) this is the end" };
            List<Token> parenTokens = Tokenizer.Tokenize(parenText);
            Tuple<List<Token>, int> parsedParens = ParenthesisParser.Parse(parenTokens, 2, '(');
            for (int i = 0; i < parsedParens.Item1.Count(); i++)
                Assert.AreEqual(parsedParens.Item1[i].GetText(), parenTokens[i + 2].GetText());
        }

        [TestMethod]
        public void NestedParenthesisTokenCount()
        {
            // nested parenthesis will return all tokens starting from the outermost brackets
            // i.e., (a b (c d e) f)

            string[] parenText = { "hello there (a b (c d e) f) this is the end" };
            List<Token> parenTokens = Tokenizer.Tokenize(parenText);
            Tuple<List<Token>, int> parsedParens = ParenthesisParser.Parse(parenTokens, 2, '(');
            Assert.AreEqual(parsedParens.Item2, 10);
        }
    }
}
