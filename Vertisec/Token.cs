using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vertisec
{
    internal class Token
    {
        private string _text;
        private uint _lineNumber;

        public Token(string text, uint lineNumber)
        {
            this._text = text.ToLower();
            this._lineNumber = lineNumber;
        }

        string Text { get; } // _text uses a special setter (to convert to lowercase)
        uint LineNumber { get; set; }

        void SetText(string text)
        {
            this._text = text.ToLower();
        }
    }
}
