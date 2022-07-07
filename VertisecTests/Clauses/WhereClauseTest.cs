using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Vertisec.Clauses.WhereClause;
using Vertisec.Exceptions;
using Vertisec.Tokens;
using Vertisec;

namespace VertisecTests.Clauses
{
    [TestClass]
    public class WhereClauseTest
    {
        WhereClause whereClause = new WhereClause();

        [TestMethod]
        public void ConditionTooShort()
        {
            string[] sampleSQL = { "where x = " };
            Globals.SetOriginalSQL(sampleSQL);
            List<Token> tokens = Tokenizer.Tokenize(sampleSQL);
            SyntaxException se = Assert.ThrowsException<SyntaxException>(() => whereClause.BuildClause(tokens, 0));
            Assert.AreEqual(se.Message, "Condition too short.");
        }

        [TestMethod]
        public void TooManyLogicals()
        {
            string exceptionMsg = "Too many logicals in condition. Only expecting one logical (=, >, like, etc.) per condition.";

            string[] sampleSQL = { "where x = = 5" };
            Globals.SetOriginalSQL(sampleSQL);
            List<Token> tokens = Tokenizer.Tokenize(sampleSQL);
            SyntaxException se = Assert.ThrowsException<SyntaxException>(() => whereClause.BuildClause(tokens, 0));
            Assert.AreEqual(se.Message, exceptionMsg);

            string[] sampleSQL2 = { "where x = like 5" };
            tokens = Tokenizer.Tokenize(sampleSQL2);
            Globals.SetOriginalSQL(sampleSQL2);
            se = Assert.ThrowsException<SyntaxException>(() => whereClause.BuildClause(tokens, 0));
            Assert.AreEqual(se.Message, exceptionMsg);

            string[] sampleSQL3 = { "where x like like 5" };
            tokens = Tokenizer.Tokenize(sampleSQL3);
            Globals.SetOriginalSQL(sampleSQL3);
            se = Assert.ThrowsException<SyntaxException>(() => whereClause.BuildClause(tokens, 0));
            Assert.AreEqual(se.Message, exceptionMsg);

            string[] sampleSQL4 = { "where x = 4 y = 12 z < 3" };
            Globals.SetOriginalSQL(sampleSQL4);
            tokens = Tokenizer.Tokenize(sampleSQL4);
            se = Assert.ThrowsException<SyntaxException>(() => whereClause.BuildClause(tokens, 0));
            Assert.AreEqual(se.Message, exceptionMsg);

            string[] sampleSQL5 = { "where x = 4 y = 12 and z < 3" };
            Globals.SetOriginalSQL(sampleSQL5);
            tokens = Tokenizer.Tokenize(sampleSQL5);
            se = Assert.ThrowsException<SyntaxException>(() => whereClause.BuildClause(tokens, 0));
            Assert.AreEqual(se.Message, exceptionMsg);

            string[] sampleSQL6 = { "where x not not 5" };
            Globals.SetOriginalSQL(sampleSQL6);
            tokens = Tokenizer.Tokenize(sampleSQL6);
            se = Assert.ThrowsException<SyntaxException>(() => whereClause.BuildClause(tokens, 0));
            Assert.AreEqual(se.Message, exceptionMsg);

            string[] sampleSQL7 = { "where x not = 5" };
            Globals.SetOriginalSQL(sampleSQL7);
            tokens = Tokenizer.Tokenize(sampleSQL7);
            se = Assert.ThrowsException<SyntaxException>(() => whereClause.BuildClause(tokens, 0));
            Assert.AreEqual(se.Message, exceptionMsg);

            string[] sampleSQL8 = { "where x like not 5" };
            Globals.SetOriginalSQL(sampleSQL8);
            tokens = Tokenizer.Tokenize(sampleSQL8);
            se = Assert.ThrowsException<SyntaxException>(() => whereClause.BuildClause(tokens, 0));
            Assert.AreEqual(se.Message, exceptionMsg);
        }

        [TestMethod]
        public void IncorrectLogicalExpression()
        {
            string[] sampleSQL = { "where x == 5" }; // == is a typo of = and not a valid logical 
            Globals.SetOriginalSQL(sampleSQL);
            List<Token> tokens = Tokenizer.Tokenize(sampleSQL);
            SyntaxException se = Assert.ThrowsException<SyntaxException>(() => whereClause.BuildClause(tokens, 0));
            Assert.AreEqual(se.Message, "Incorrect logical expression. Expecting '=', '<', '<=', etc.");

            string[] sampleSQL2 = { "where x fake 5" }; 
            tokens = Tokenizer.Tokenize(sampleSQL2);
            Globals.SetOriginalSQL(sampleSQL2);
            tokens = Tokenizer.Tokenize(sampleSQL2);
            se = Assert.ThrowsException<SyntaxException>(() => whereClause.BuildClause(tokens, 0));
            Assert.AreEqual(se.Message, "Incorrect logical expression. Expecting '=', '<', '<=', etc.");
        }

        [TestMethod]
        public void MisplacedLogical()
        {
            string[] sampleSQL = { "where = x 5" }; 
            Globals.SetOriginalSQL(sampleSQL);
            List<Token> tokens = Tokenizer.Tokenize(sampleSQL);
            SyntaxException se = Assert.ThrowsException<SyntaxException>(() => whereClause.BuildClause(tokens, 0));
            Assert.AreEqual(se.Message, "Misplaced logical. Did you mean 'x = 5'?");
        }

        [TestMethod]
        public void InvalidListOfValues()
        {
            string[] sampleSQL = { "where x in (1 2 3)" }; 
            Globals.SetOriginalSQL(sampleSQL);
            List<Token> tokens = Tokenizer.Tokenize(sampleSQL);
            SyntaxException se = Assert.ThrowsException<SyntaxException>(() => whereClause.BuildClause(tokens, 0));
            Assert.AreEqual(se.Message, "Too many tokens between commas. Are you forgetting a comma?");
        }

        [TestMethod]
        public void HangingOpenParenthesis()
        {
            string[] sampleSQL = { "where x in (1,2,3" }; 
            Globals.SetOriginalSQL(sampleSQL);
            List<Token> tokens = Tokenizer.Tokenize(sampleSQL);
            SyntaxException se = Assert.ThrowsException<SyntaxException>(() => whereClause.BuildClause(tokens, 0));
            Assert.AreEqual(se.Message, "Missing closing parenthesis.");
        }

        [TestMethod]
        public void HangingClosingParenthesis()
        {
            string[] sampleSQL = { "where x = 4)" }; 
            Globals.SetOriginalSQL(sampleSQL);
            List<Token> tokens = Tokenizer.Tokenize(sampleSQL);
            SyntaxException se = Assert.ThrowsException<SyntaxException>(() => whereClause.BuildClause(tokens, 0));
            Assert.AreEqual(se.Message, "Missing open parenthesis.");
        }

        /*
         * Below are a collection of tests that make no assertions.
         * This tests "valid" cases where the test fails if an exception occurs (since these should be correct)
         */
        [TestMethod]
        public void ValidSingleCondition()
        {
            string[] sampleSQL = { "where x = 5" }; 
            Globals.SetOriginalSQL(sampleSQL);
            List<Token> tokens = Tokenizer.Tokenize(sampleSQL);
            whereClause.BuildClause(tokens, 0);
        }

        [TestMethod]
        public void ValidDoubleCondition()
        {
            string[] sampleSQL = { "where x = 5 and y != 4" }; 
            Globals.SetOriginalSQL(sampleSQL);
            List<Token> tokens = Tokenizer.Tokenize(sampleSQL);
            whereClause.BuildClause(tokens, 0);
        }

        [TestMethod]
        public void ValidMultiCondition()
        {
            string[] sampleSQL = { "where x = 5 and y != 8 or z like 14 or w not like 15" }; 
            Globals.SetOriginalSQL(sampleSQL);
            List<Token> tokens = Tokenizer.Tokenize(sampleSQL);
            whereClause.BuildClause(tokens, 0);
        }

        [TestMethod]
        public void ValidLikeWithQuotes()
        {
            string[] sampleSQL = { "where x like 'some thing with quotes'" }; 
            Globals.SetOriginalSQL(sampleSQL);
            List<Token> tokens = Tokenizer.Tokenize(sampleSQL);
            whereClause.BuildClause(tokens, 0);
        }

        [TestMethod]
        public void ValidParenthesis1()
        {
            string[] sampleSQL = { "where (x = 15)" }; 
            Globals.SetOriginalSQL(sampleSQL);
            List<Token> tokens = Tokenizer.Tokenize(sampleSQL);
            whereClause.BuildClause(tokens, 0);
        }

        [TestMethod]
        public void ValidParenthesis2()
        {
            string[] sampleSQL = { "where (x = 5) or y != 14 and (z like 'more quotes')" }; 
            Globals.SetOriginalSQL(sampleSQL);
            List<Token> tokens = Tokenizer.Tokenize(sampleSQL);
            whereClause.BuildClause(tokens, 0);
        }

        [TestMethod]
        public void ValidNestedParenthesis()
        {
            string[] sampleSQL = { "where ((x = 5 or y != 14) and (z like 'more quotes') or w < 3 and (v >= 18)) and q > 10" }; 
            Globals.SetOriginalSQL(sampleSQL);
            List<Token> tokens = Tokenizer.Tokenize(sampleSQL);
            whereClause.BuildClause(tokens, 0);
        }

        [TestMethod]
        public void ValidListOfValues()
        {
            string[] sampleSQL = { "where x in (1, 2, 3)" }; 
            Globals.SetOriginalSQL(sampleSQL);
            List<Token> tokens = Tokenizer.Tokenize(sampleSQL);
            whereClause.BuildClause(tokens, 0);

            string[] sampleSQL2 = { "where x in ('quote one', 'quote two', 'quote three')" }; 
            Globals.SetOriginalSQL(sampleSQL2);
            tokens = Tokenizer.Tokenize(sampleSQL2);
            whereClause.BuildClause(tokens, 0);

            string[] sampleSQL3 = { "where x not in (1, 2, 3)" }; 
            Globals.SetOriginalSQL(sampleSQL3);
            tokens = Tokenizer.Tokenize(sampleSQL3);
            whereClause.BuildClause(tokens, 0);

            string[] sampleSQL4 = { "where x not in ('quote one', 'quote two', 'quote three')" }; 
            Globals.SetOriginalSQL(sampleSQL4);
            tokens = Tokenizer.Tokenize(sampleSQL4);
            whereClause.BuildClause(tokens, 0);
        }
    }
}
