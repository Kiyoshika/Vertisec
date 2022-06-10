using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vertisec.Tokens
{
    public class Token
    {
        private string _text;
        private uint _lineNumber;

        public Token(string text, uint lineNumber)
        {
            _text = text;
            _lineNumber = lineNumber;
        }

        public string GetText()
        {
            return _text;
        }
        public uint GetLineNumber()
        {
            return _lineNumber;
        }

    }
}
