using System;
using System.Collections.Generic;
using System.Text;

namespace compiller.Lexer
{
    public class Token
    {
            public int tag { get; set; }
            public Token(int t) { tag = t; }
            public override string ToString() { return "" + (char)tag; }
    }
}
