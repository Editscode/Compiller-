using System;
using System.Collections.Generic;
using System.Text;

namespace compiller.Lexer
{
    class Word : Token
    {
        public string lexeme = "";
        public Word(string s,int tag) : base(tag)
        {
            lexeme = s;
        }
        public override string ToString() { return lexeme; }
        public static Word 
            and   = new Word("&&",Tag.AND),
            or    = new Word("||", Tag.OR),
            eq    = new Word("==", Tag.EQ),
            ne    = new Word("!=", Tag.NE),
            le    = new Word("<=", Tag.LE),
            ge    = new Word(">=", Tag.GE),
            minus = new Word("minus", Tag.MINUS),
            True  = new Word("true", Tag.TRUE),
            False = new Word("false", Tag.FALSE),
            temp  = new Word("t", Tag.TEMP);

    }
}
