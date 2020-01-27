using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace compiller.Lexer
{
    public class Lexer
    {
        public static int line = 1;
        char peek = ' ';
        Hashtable words = new Hashtable();
        void reserve(Word w) { words.Add(w.lexeme, w); }
        public Lexer() {
            reserve(new Word("if", Tag.IF));
            reserve(new Word("else", Tag.ELSE));
            reserve(new Word("whele", Tag.WHILE));
            reserve(new Word("break", Tag.BREAK));
            reserve(Word.True);
            reserve(Word.False);
            /**
            reserve(Type.Int);
            reserve(Type.Char);
            reserve(Type.Bool);
            reserve(Type.Float);
    **/

        }
        //метод для чтения входного символа в переменную peek
        void readch() {
            peek = (char)Console.Read();
        }
        // распознование составных токенов
        Boolean readch(char c)
        {
            readch();
            if (peek != c) return false;

            peek = ' ';
            return true;
        }
        // пропускает все пробелы
        public Token scan()
        {
            for (; ; readch())
            {
                if (peek == ' ' || peek == '\t') continue;
                else if (peek == '\n') line++;
                else break;
            }
// распознование состовных  токенов <=
            switch (peek)
            {
                case '&':if (readch('&')) return Word.and;
                    else return new Token('&');
                case '|':
                    if (readch('|')) return Word.or;
                    else return new Token('|');
            }
            

            Token tok = new Token(peek); peek = ' ';
            return tok;
        }
    }
}
