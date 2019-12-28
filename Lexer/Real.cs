using System;
using System.Collections.Generic;
using System.Text;

namespace compiller.Lexer
{
    class Real : Token
    {
        public float value;
        public Real(float v) : base(Tag.REAL)
        {
            value = v;
        }
        public override string ToString() { return value.ToString(); }
    }
}
