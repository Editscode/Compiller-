using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace compiller.Lexer
{
    public class Lexer
    {
        public Token NextToken;
        public string NextTokenContent;
        public TokenType NextTokenType;

        public int Line = 1;
        public int Column = 0;
        public int FileIndex;
        public static int FileCount;
        private char _peek = ' ';
        private static readonly Dictionary<string, TokenType> s_keywordsSet = new Dictionary<string, TokenType>();
        private static readonly Dictionary<char, char> s_ECSet = new Dictionary<char, char>();
        private static readonly Dictionary<string, TokenType> s_signSet = new Dictionary<string, TokenType>();
        private StreamReader _streamReader;
        private string[] _files;

        //文件 第几个词
        private readonly List<List<Token>> s_tokens = new List<List<Token>>();
        private int _index;
        static Lexer() {
            s_keywordsSet.Add("if", TokenType.IfKeyword);
            s_keywordsSet.Add("else", TokenType.ElseKeyword);
            s_keywordsSet.Add("while", TokenType.WhileKeyword);
            s_keywordsSet.Add("for", TokenType.ForKeyword);
            s_keywordsSet.Add("break", TokenType.BreakKeyword);
            s_keywordsSet.Add("continue", TokenType.ContinueKeyword);
            s_keywordsSet.Add("return", TokenType.ReturnKeyword);
            s_keywordsSet.Add("class", TokenType.ClassKeyword);
            s_keywordsSet.Add("struct", TokenType.StructKeyword);
            s_keywordsSet.Add("interface", TokenType.InterfaceKeyword);
            s_keywordsSet.Add("namespace", TokenType.NameSpaceKeyword);
            s_keywordsSet.Add("using", TokenType.UsingKeyword);
            s_keywordsSet.Add("func", TokenType.FuncKeyword);
            s_keywordsSet.Add("var", TokenType.VarKeyword);
            s_keywordsSet.Add("let", TokenType.LetKeyword);
            s_keywordsSet.Add("inv", TokenType.InvKeyword);
            s_keywordsSet.Add("new", TokenType.NewKeyword);
            s_keywordsSet.Add("override", TokenType.OverrideKeyword);
            s_keywordsSet.Add("static", TokenType.StaticKeyword);
            s_keywordsSet.Add("true", TokenType.TrueKeyword);
            s_keywordsSet.Add("false", TokenType.FalseKeyword);
            s_keywordsSet.Add("cast", TokenType.CastKeyword);
            s_keywordsSet.Add("public", TokenType.PublicKeyword);
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
