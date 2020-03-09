﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace MyCore.LexicalAnalysis
{
    internal class Lexer
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

        //подача нескольких слов
        public readonly List<List<Token>> s_tokens = new List<List<Token>>();
        private int _index;

        static Lexer()
        {
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
            s_ECSet.Add('b', '\b'); //возврат на шаг
            s_ECSet.Add('f', '\f'); //прогон страницы
            s_ECSet.Add('t', '\t'); //горизонтальная табуляция
            s_ECSet.Add('r', '\r'); //Возврат каретки
            s_ECSet.Add('n', '\n'); //перевод строки

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

       

        public Token Back()
        {
            _index--;
            while (true)
            {
                if (_index == 0)
                {
                    FileIndex--;
                    _index = s_tokens[FileIndex].Count - 1;
                }
                NextToken = s_tokens[FileIndex][--_index];
                NextTokenType = NextToken.Type;
                switch (NextTokenType)
                {
                    case TokenType.Blank:
                        continue;
                    case TokenType.EOL:
                        Line++;
                        continue;
                }
                break;
            }
            _index++;
            NextTokenContent = NextToken.Content;
            return NextToken;
        }

        public bool Match(TokenType type) => NextTokenType == type; //TODO: Добавить обработку ошибок
        public bool Match(string content) => NextTokenContent == content; //TODO: Добавить обработку ошибок

        public bool MatchNext(string content) //TODO: Добавить обработку ошибок
        {
            Next();
            return NextTokenContent == content;
        }
        public Token Eat(TokenType type) //TODO: Добавить обработку ошибок
        {
            if (Match(type))
            {
                Token t = NextToken;
                Next();
                return t;
            }
            else return null;
        }
        public Token Eat(string content) //TODO: Добавить обработку ошибок
        {
            if (Match(content))
            {
                Token t = NextToken;
                Next();
                return t;
            }
            else return null;
        }

        public void Scan(string[] files)
        {
            _files = files;
            for (int i = 0; i < _files.Length; i++)
                s_tokens.Add(new List<Token>());
            ScanFiles();
            FileIndex = 0;
            Line = 1;
            _index = 0;
            FileCount = files.Length;
            Next();
        }


        private void ScanFiles()
        {
            for (int i = 0; i < _files.Length; i++, FileIndex = i)
            {
                _streamReader = new StreamReader(new FileStream(_files[i], FileMode.Open));
                Readch();
                for (; !_streamReader.EndOfStream; ScanBlank())
                {
                    if (char.IsLetter(_peek) || _peek == '_')
                    {
                        ScanIdentifer();
                        continue;
                    }
                    if (_peek == '"')
                    {
                        ScanString();
                        continue;
                    }
                    if (_peek == '\'')
                    {
                        ScanChar();
                        continue;
                    }
                    if (_peek == '/')
                    {
                        JumpComment();
                        continue;
                    }
                    if (char.IsSymbol(_peek) || s_signSet.ContainsKey(_peek.ToString()))
                    {
                        ScanSign();
                        continue;
                    }
                    if (char.IsNumber(_peek))
                    {
                        ScanNumber();
                        continue;
                    }
                    return; 
                }
                
                _streamReader.Close();
            }
        }

        private void ScanSign()
        {
            char c = _peek;
            Readch();
            if (char.IsSymbol(_peek))
            {
                string s = c.ToString() + _peek;
                if (s_signSet.ContainsKey(s))
                {
                    AddToken(s_signSet[s], s, -1);
                    Readch();
                    return;
                }
                if (s_signSet.ContainsKey(_peek.ToString()))
                {
                    if (s_signSet.ContainsKey(c.ToString())) AddToken(s_signSet[c.ToString()], c.ToString(), -1);
                    else return; 
                    AddToken(s_signSet[_peek.ToString()], _peek.ToString(), 0);
                    Readch();
                    return;
                }
                else return; 
            }
            if (s_signSet.ContainsKey(c.ToString()))
                AddToken(s_signSet[c.ToString()], c.ToString(), -1);
            else return; 
        }

        private void ScanIdentifer()
        {
            StringBuilder sb = new StringBuilder();
            do
            {
                sb.Append(_peek);
                Readch();
            } while (char.IsLetterOrDigit(_peek) || _peek == '_');

            string s = sb.ToString();
            if (s_keywordsSet.ContainsKey(s))
                AddToken(s_keywordsSet[s], s, -s.Length);
            else
                AddToken(TokenType.Identifer, s, -s.Length);
        }

        private void ScanNumber()
        {
            StringBuilder sb = new StringBuilder();
            while (char.IsDigit(_peek))
            {
                sb.Append(_peek);
                Readch();
            }

            if (char.IsLetter(_peek) || _peek == '_') { return; } //TODO:Идентификатор ошибки не может начинаться с цифры
            if (_peek != '.')
            {
                if (int.TryParse(sb.ToString(), out int n))
                {
                    AddToken(TokenType.NumericLiteralToken, sb.ToString(), -sb.Length);
                    return;
                }
                return; //TODO:константа слишком велика
            }

            sb.Append(_peek);
            Readch();
            for (; ; Readch())
            {
                if (!char.IsDigit(_peek)) break;
                sb.Append(_peek);
            }

            if (char.IsLetter(_peek) || _peek == '_')
            {
                if (_peek == 'e')
                {
                    sb.Append(_peek);
                    Readch();
                    for (; ; Readch())
                    {
                        if (!char.IsDigit(_peek)) break;
                        sb.Append(_peek);
                    }
                }
                else return;


            } //TODO:Идентификатор ошибки не может начинаться с цифры



            if (float.TryParse(sb.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out float f))
                AddToken(TokenType.FloatLiteralToken, sb.ToString(), -sb.Length);
            else
                return; //TODO: константа с плавающей запятой выходит за пределы диапазона
        }

        private void ScanBlank()
        {
            for (; ; Readch())
            {
                switch (_peek)
                {
                    case ' ':
                    case '\t':
                        continue;
                    case '\r':
                        Column = 0;
                        continue;
                    case '\n':
                        AddToken(TokenType.EOL, 0);
                        continue;
                }
                if (_streamReader.EndOfStream)
                {
                    AddToken(TokenType.EOF, 0);
                    break;
                }
                break;
            }
        }

        private void JumpComment()
        {
            Readch();
            if (_peek == '/')
                for (; ; Readch())
                {
                    if (_peek == '\n')
                    {
                        AddToken(TokenType.EOL, 0);
                        Readch();
                        return;
                    }

                    if (_streamReader.EndOfStream)
                        return;
                }

            if (_peek == '*')
            {
                Readch();
                for (; ; Readch())
                {
                    if (_peek == '\n')
                    {
                        AddToken(TokenType.EOL, 0);
                        Readch();
                    }

                    if (_peek == '*')
                    {
                        Readch();
                        if (_peek == '/')
                        {
                            Readch();
                            return;
                        }
                    }

                    if (_streamReader.EndOfStream)
                    {
                        AddToken(TokenType.EOF, 0);
                        return;
                    }
                }
            }

            AddToken(TokenType.Sign, "/", -1);
        }

        private void ScanString()
        {
            Readch();
            StringBuilder sb = new StringBuilder();
            for (; _peek != '"'; Readch())
            {
                if (_peek == '\n') { return; } //TODO:Вывод ошибки
                if (_peek == '\\')
                {
                    sb.Append(ScanEC());
                    continue;
                }
                sb.Append(_peek);
            }
            AddToken(TokenType.StringLiteralToken, sb.ToString(), -sb.Length - 1);
            Readch();
        }

        private void ScanChar()
        {
            Readch();
            if (_peek == '\\')
            {
                char ec = ScanEC();
                Readch();
                if (_peek != '\'') { return; } //TODO:слишком много символов
                AddToken(TokenType.CharacterLiteralToken, ec.ToString(), -3);
                Readch();
                return;
            }
            char c = _peek;
            Readch();
            if (_peek != '\'') { return; } //TODO:слишком много символов
            AddToken(TokenType.CharacterLiteralToken, c.ToString(), -2);
            Readch();
        }

        private char ScanEC()
        {
            Readch();
            if (!s_ECSet.ContainsKey(_peek)) { return '\0'; } //TODO:Escape символ
            return s_ECSet[_peek];
        }


        private void Readch()
        {
            _peek = (char)_streamReader.Read();
            Column++;
        }

        private void AddToken(TokenType type, int offset)
        {
            s_tokens[FileIndex].Add(new Token(type, Line, Column + offset));
            if (type == TokenType.EOF)
                FileIndex++;
            if (type == TokenType.EOL)
            {
                Line++;
                Column = 0;
            }
        }

        private void AddToken(TokenType type, string content, int offset)
        {
            Token token = new Token(type, Line, Column + offset);
            token.Content = content;
            s_tokens[FileIndex].Add(token);
        }



        public Token Peek(int forward)
        {
            Debug.Assert(forward > 0);
            int index = _index;
            Token resuilt = null;
            for (int i = 0; i < forward; i++)
            {
                resuilt = Next();
            }
            _index = index;
            return resuilt;
        }
        public bool MatchNow(TokenType type) //TODO: Добавить обработку ошибок
        {
            bool b = NextTokenType == type;
            Next();
            return b;
        }
        public bool MatchNow(string content) //TODO: Добавить обработку ошибок
        {
            bool b = NextTokenContent == content;
            Next();
            return b;
        }
        public bool MatchNext(TokenType type) //TODO: Добавить обработку ошибок
        {
            Next();
            return NextTokenType == type;
        }
    }
}