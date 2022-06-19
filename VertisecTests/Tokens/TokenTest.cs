using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Vertisec.Tokens;

namespace VertisecTests.Tokens
{
    [TestClass]
    public class TokenTest
    {
        [TestMethod]
        public void CreateToken()
        {
            Token token = new Token("tokenText", 13);
            Assert.IsTrue(token.GetText() == "tokenText");
            Assert.IsTrue(token.GetLineNumber() == 13);
        }
    }
}
