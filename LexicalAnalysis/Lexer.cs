﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Compiller.LexicalAnalysis
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

        public void NextLine()
        {
            Line++;
            Next();
        }

        public bool Match(TokenType type) => NextTokenType == type;
        public bool Match(string content) => NextTokenContent == content;

        public bool MatchNow(TokenType type)
        {
            bool b = NextTokenType == type;
            Next();
            return b;
        }

        public bool MatchNow(string content)
        {
            bool b = NextTokenContent == content;
            Next();
            return b;
        }

        public bool MatchNext(TokenType type)
        {
            Next();
            return NextTokenType == type;
        }

        public bool MatchNext(string content)
        {
            Next();
            return NextTokenContent == content;
        }

        public Token Eat(TokenType type)
        {
            if (Match(type))
            {
                Token t = NextToken;
                Next();
                return t;
            }
            else return null;
        }

        public Token Eat(string content)
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
                //загрузка тестовых файлов
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
                }
                _streamReader.Close();
            }
        }

        private void ScanString()
        {
            Readch();
            StringBuilder sb = new StringBuilder();
            for (; _peek != '"'; Readch())
            {
                if (_peek == '\n') { return; }
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

        private void Readch()
        {
            _peek = (char)_streamReader.Read();
            Column++;
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

        private void AddToken(string content, int offset) =>
            s_tokens[FileIndex].Add(new Token(content, Line, Column + offset));

        private void AddToken(TokenType type, string content, int offset)
        {
            Token token = new Token(type, Line, Column + offset);
            token.Content = content;
            s_tokens[FileIndex].Add(token);
        }
        private char ScanEC()
        {
            Readch();
            if (!s_ECSet.ContainsKey(_peek)) { return '\0'; } //TODO:报错 无效的转义字符
            return s_ECSet[_peek];
        }
    }
}
