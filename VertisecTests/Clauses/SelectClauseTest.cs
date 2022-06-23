using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Vertisec.Clauses.SelectClause;
using Vertisec.Exceptions;
using Vertisec.Tokens;
using Vertisec;

namespace VertisecTests.Clauses
{
    [TestClass]
    public class SelectClauseTest
    {
        SelectClause select = new SelectClause();

        [TestMethod]
        public void FromTokenExists()
        {
            string[] sampleSQL = { "select a" };
            Globals.SetOriginalSQL(sampleSQL);
            List<Token> tokens = Tokenizer.Tokenize(sampleSQL);
            SelectClauseException se = Assert.ThrowsException<SelectClauseException>(() => select.BuildClause(tokens, 0));
            Assert.AreEqual(se.Message, "'from' token for 'select' not found.");
        }

        [TestMethod]
        public void ImproperColumnAliasingWithAs()
        {
            string[] sampleSQL = { "select a b as from x" };
            Globals.SetOriginalSQL(sampleSQL);
            List<Token> tokens = Tokenizer.Tokenize(sampleSQL);
            SelectClauseException se = Assert.ThrowsException<SelectClauseException>(() => select.BuildClause(tokens, 0));
            Assert.AreEqual(se.Message, "Improper column aliasing with 'as'.");

            string[] sampleSQL2 = { "select as a b from x" };
            Globals.SetOriginalSQL(sampleSQL2);
            tokens = Tokenizer.Tokenize(sampleSQL2);
            se = Assert.ThrowsException<SelectClauseException>(() => select.BuildClause(tokens, 0));
            Assert.AreEqual(se.Message, "Improper column aliasing with 'as'.");
        }

        [TestMethod]
        public void ImproperColumnAliasingWithNoAs()
        {
            string[] sampleSQL = { "select a b c from x" };
            Globals.SetOriginalSQL(sampleSQL);
            List<Token> tokens = Tokenizer.Tokenize(sampleSQL);
            SelectClauseException se = Assert.ThrowsException<SelectClauseException>(() => select.BuildClause(tokens, 0));
            Assert.AreEqual(se.Message, "Improper column aliasing. Did you forget a comma?");
        }

        [TestMethod]
        public void SelectNoColumns()
        {
            string[] sampleSQL = { "select from x" };
            Globals.SetOriginalSQL(sampleSQL);
            List<Token> tokens = Tokenizer.Tokenize(sampleSQL);
            SelectClauseException se = Assert.ThrowsException<SelectClauseException>(() => select.BuildClause(tokens, 0));
            Assert.AreEqual(se.Message, "'select' has no columns.");
        }

        [TestMethod]
        public void TrailingComma()
        {
            string[] sampleSQL = { "select a, from x" };
            Globals.SetOriginalSQL(sampleSQL);
            List<Token> tokens = Tokenizer.Tokenize(sampleSQL);
            SelectClauseException se = Assert.ThrowsException<SelectClauseException>(() => select.BuildClause(tokens, 0));
            Assert.AreEqual(se.Message, "Trailing comma.");
        }

        [TestMethod]
        public void RepeatedCommas()
        {
            string[] sampleSQL = { "select a,, from x" };
            Globals.SetOriginalSQL(sampleSQL);
            List<Token> tokens = Tokenizer.Tokenize(sampleSQL);
            SelectClauseException se = Assert.ThrowsException<SelectClauseException>(() => select.BuildClause(tokens, 0));
            Assert.AreEqual(se.Message, "Repeated commas.");
        }
    }
}
