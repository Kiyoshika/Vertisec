using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Vertisec.Clauses.FromClause;
using Vertisec.Exceptions;
using Vertisec.Tokens;
using Vertisec;

namespace VertisecTests.Clauses
{
    [TestClass]
    public class FromClauseTest
    {
        FromClause fromClause = new FromClause(); 

        [TestMethod]
        public void NoTable()
        {
            string[] sampleSQL = { "from" };
            Globals.SetOriginalSQL(sampleSQL);
            List<Token> tokens = Tokenizer.Tokenize(sampleSQL);
            SyntaxException se = Assert.ThrowsException<SyntaxException>(() => fromClause.BuildClause(tokens, 0));
            Assert.AreEqual(se.Message, "Missing table for 'from' token.");
        }
        
        [TestMethod]
        public void TooManyTokensAfterFrom()
        {
            string[] sampleSQL = { "from x y z" };
            Globals.SetOriginalSQL(sampleSQL);
            List<Token> tokens = Tokenizer.Tokenize(sampleSQL);
            SyntaxException se = Assert.ThrowsException<SyntaxException>(() => fromClause.BuildClause(tokens, 0));
            Assert.AreEqual(se.Message, "Improper 'from' aliasing, too many tokens after 'from'.");
        }
        
        [TestMethod]
        public void ImproperAsAliasing()
        {
            string[] sampleSQL = { "from as x" };
            Globals.SetOriginalSQL(sampleSQL);
            List<Token> tokens = Tokenizer.Tokenize(sampleSQL);
            SyntaxException se = Assert.ThrowsException<SyntaxException>(() => fromClause.BuildClause(tokens, 0));
            Assert.AreEqual(se.Message, "Too few tokens for 'as'. Are you missing a table/alias?");
        }

        [TestMethod]
        public void MissingDerivedTableAlias()
        {
            string[] sampleSQL = { "from (select x from y)" };
            Globals.SetOriginalSQL(sampleSQL);
            List<Token> tokens = Tokenizer.Tokenize(sampleSQL);
            SyntaxException se = Assert.ThrowsException<SyntaxException>(() => fromClause.BuildClause(tokens, 0));
            Assert.AreEqual(se.Message, "Derived table must have an alias.");
        }

        [TestMethod]
        public void EmptyDerivedTable()
        {
            string[] sampleSQL = { "from ()" };
            Globals.SetOriginalSQL(sampleSQL);
            List<Token> tokens = Tokenizer.Tokenize(sampleSQL);
            SyntaxException se = Assert.ThrowsException<SyntaxException>(() => fromClause.BuildClause(tokens, 0));
            Assert.AreEqual(se.Message, "Derived table is empty.");
        }

        [TestMethod]
        public void DerivedTableSQLError()
        {
            string[] sampleSQL = { "from (select wh_id, from y) x" };
            Globals.SetOriginalSQL(sampleSQL);
            List<Token> tokens = Tokenizer.Tokenize(sampleSQL);
            SyntaxException se = Assert.ThrowsException<SyntaxException>(() => fromClause.BuildClause(tokens, 0));
            Assert.AreEqual(se.Message, "Trailing comma.");
        }

        [TestMethod]
        public void NestedDerivedTableSQLError()
        {
            string[] sampleSQL = { "from (select x from (select z, from) y) x" };
            Globals.SetOriginalSQL(sampleSQL);
            List<Token> tokens = Tokenizer.Tokenize(sampleSQL);
            SyntaxException se = Assert.ThrowsException<SyntaxException>(() => fromClause.BuildClause(tokens, 0));
            Assert.AreEqual(se.Message, "Trailing comma.");
        }

        /*
         * Below is a collection of valid FromClauses so no assertions are made.
         * If any exceptions are thrown, the tests fail (as these should be successful)
         */
        [TestMethod]
        public void ValidNoAlias()
        {
            string[] sampleSQL = { "from x" };
            Globals.SetOriginalSQL(sampleSQL);
            List<Token> tokens = Tokenizer.Tokenize(sampleSQL);
            fromClause.BuildClause(tokens, 0);
        }

        [TestMethod]
        public void ValidShortAlias()
        {
            string[] sampleSQL = { "from x y" };
            Globals.SetOriginalSQL(sampleSQL);
            List<Token> tokens = Tokenizer.Tokenize(sampleSQL);
            fromClause.BuildClause(tokens, 0);
        }

        [TestMethod]
        public void ValidLongAlias()
        {
            string[] sampleSQL = { "from x as y" };
            Globals.SetOriginalSQL(sampleSQL);
            List<Token> tokens = Tokenizer.Tokenize(sampleSQL);
            fromClause.BuildClause(tokens, 0);
        }

        [TestMethod]
        public void ValidDerivedTableShortAlias()
        {
            string[] sampleSQL = { "from (select x from y) x" };
            Globals.SetOriginalSQL(sampleSQL);
            List<Token> tokens = Tokenizer.Tokenize(sampleSQL);
            fromClause.BuildClause(tokens, 0);
        }

        [TestMethod]
        public void ValidDerivedTableLongAlias()
        {
            string[] sampleSQL = { "from (select x from y) x as y" };
            Globals.SetOriginalSQL(sampleSQL);
            List<Token> tokens = Tokenizer.Tokenize(sampleSQL);
            fromClause.BuildClause(tokens, 0);
        }

        [TestMethod]
        public void ValidNestedDerivedTable()
        {
            string[] sampleSQL = { "from (select x from (select x from y) z) w" };
            Globals.SetOriginalSQL(sampleSQL);
            List<Token> tokens = Tokenizer.Tokenize(sampleSQL);
            fromClause.BuildClause(tokens, 0);
        }

    }
}
