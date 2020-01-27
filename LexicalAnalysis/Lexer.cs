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
        /// <summary>
        /// Ключевые слова
        /// </summary>
        private static readonly Dictionary<string, TokenType> s_keywordsSet = new Dictionary<string, TokenType>();
        /// <summary>
        /// Управляющие символы
        /// </summary>
        private static readonly Dictionary<char, char> s_ECSet = new Dictionary<char, char>();
        /// <summary>
        /// Лолгические
        /// </summary>
        private static readonly Dictionary<string, TokenType> s_signSet = new Dictionary<string, TokenType>();
        private StreamReader _streamReader;
        private string[] _files;

        
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

            s_ECSet.Add('\"', '\"');
            s_ECSet.Add('\'', '\'');
            s_ECSet.Add('\\', '\\');
            s_ECSet.Add('b', '\b'); 
            s_ECSet.Add('f', '\f'); 
            s_ECSet.Add('t', '\t'); 
            s_ECSet.Add('r', '\r'); 
            s_ECSet.Add('n', '\n');

            s_signSet.Add("[", TokenType.OpenBracket);
            s_signSet.Add("]", TokenType.CloseBracket);
            s_signSet.Add("(", TokenType.OpenParen);
            s_signSet.Add(")", TokenType.CloseParen);
            s_signSet.Add("-", TokenType.Sign);
            s_signSet.Add("+", TokenType.Sign);
            s_signSet.Add("!", TokenType.Sign);
            s_signSet.Add("/", TokenType.Sign);
            s_signSet.Add("*", TokenType.Sign);
            s_signSet.Add("%", TokenType.Sign);
            s_signSet.Add(">", TokenType.Sign);
            s_signSet.Add(">=", TokenType.Sign);
            s_signSet.Add("<", TokenType.Sign);
            s_signSet.Add("<=", TokenType.Sign);
            s_signSet.Add("==", TokenType.Sign);
            s_signSet.Add("!=", TokenType.Sign);
            s_signSet.Add("&&", TokenType.Sign);
            s_signSet.Add("||", TokenType.Sign);
            s_signSet.Add(",", TokenType.Sign);
            s_signSet.Add(".", TokenType.Sign);
            s_signSet.Add(":", TokenType.Colon);
            s_signSet.Add("=", TokenType.Assign);
            s_signSet.Add("+=", TokenType.Assign);
            s_signSet.Add("-=", TokenType.Assign);
            s_signSet.Add("*=", TokenType.Assign);
            s_signSet.Add("/=", TokenType.Assign);
            s_signSet.Add("%=", TokenType.Assign);
            s_signSet.Add(";", TokenType.Semicolon);
            s_signSet.Add("{", TokenType.OpenBrace);
            s_signSet.Add("}", TokenType.CloseBrace);

        }

        public Token Next()
        {
            while (true)
            {
                NextToken = s_tokens[FileIndex][_index++];
                switch (NextToken.Type)
                {
                    case TokenType.EOL:
                        Line++;
                        continue;
                    case TokenType.EOF:
                        Line = 1;
                        FileIndex++;
                        break;
                }
                break;
            }
            NextTokenType = NextToken.Type;
            NextTokenContent = NextToken.Content;
            Line = NextToken.Line;
            return NextToken;
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
