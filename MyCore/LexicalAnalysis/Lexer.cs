﻿using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace MyCore.LexicalAnalysis
{
    internal class Lexer
    {

        private int Line;
        private int Column;
        private int realColumn = 1;

        private List<char> _peek = new List<char>();

        private static IDictionary<string, TokenType> tokenTypeMap = new Dictionary<string, TokenType>();

        //лист полученных токенов
        public readonly List<Token> s_tokens = new List<Token>();

        private static readonly Dictionary<char, char> s_ECSet = new Dictionary<char, char>() {
            {'\"', '\"'},
            {'\'', '\''},
            {'\\', '\\'},
            {'b', '\b' },
            {'f', '\f' },
            {'t', '\t' },
            {'r', '\r' },
            {'n', '\n' }
        };

        public static string[] LiteralNames = {
        null, "///", null, null, null, null, null, "#", "abstract",
        "add", "alias", "__arglist", "as", "ascending", "async", "await",
        "base", "bool", "break", "by", "byte", "case", "catch",
        "char", "checked", "class", "const", "continue", "decimal",
        "default", "delegate", "descending", "do", "double", "dynamic",
        "else", "enum", "equals", "event", "explicit", "extern", "false",
        "finally", "fixed", "float", "for", "foreach", "from", "get",
        "goto", "group", "if", "implicit", "in", "int", "interface",
        "internal", "into", "is", "join", "let", "lock", "long",
        "nameof", "namespace", "new", "null", "object", "on", "operator",
        "orderby", "out", "override", "params", "partial", "private",
        "protected", "public", "readonly", "ref", "remove", "return",
        "sbyte", "sealed", "select", "set", "short", "sizeof", "stackalloc",
        "static", "string", "struct", "switch", "this", "throw", "true",
        "try", "typeof", "uint", "ulong", "unchecked", "unsafe", "ushort",
        "using", "var", "virtual", "void", "volatile", "when", "where",
        "while", "yield", null, null, null, null, null, null, null, null,
        null, null, "{", "}", "[", "]", "(", ")", ".", ",", ":",
        ";", "+", "-", "*", "/", "%", "&", "|", "^", "!",
        "~", "=", "<", ">", "?", "::", "??", "++", "--", "&&",
        "||", "->", "==", "!=", "<=", ">=", "+=", "-=", "*=",
        "/=", "%=", "&=", "|=", "^=", "<<", "<<=", "{{", null,
        null, null, null, null, null, null, null, null, null, "define", "undef",
        "elif", "endif", "line", null, null, null, null, null, "hidden",
        null, null, null, "}}"
    };
        public Lexer()
        {
            
            Recognizer recognizer = new Recognizer(LiteralNames);
            tokenTypeMap = recognizer.TokenTypeMap;  
        }
        public void Scan(StreamReader stream)
        {
            Line = 1;
            Column = 0;
            List<char> ab = ScanFileToListСhar(stream);
            ScanLexem(ab);
        }
        private List<char> ScanFileToListСhar(StreamReader stream)
        {
            char[] buffer = new char[1024];
            List<char> CharList = new List<char>();
            int read;

            while ((read = stream.ReadBlock(buffer, 0, buffer.Length)) > 0)
            {
                for (int i = 0; i < read; i++)
                    CharList.Add(buffer[i]);
            }
            CharList.Add('\uffff');
            return CharList;
        }
        private void Readch()
        {
            Column++;
            realColumn++;
        }
        private void Stop()
        {
            Column = _peek.Count;
        }
        private void ScanBlank()
        {
            for (; this._peek.Count > Column; Readch())
            {
                switch (_peek[Column])
                {
                    case ' ':
                    case '\t':
                        continue;
                    case '\r':
                        continue;
                    case '\uffff':
                        Stop();
                        continue;
                    case '\n':
                        realColumn = 0;
                        Line++;
                        continue;
                }
                break;
            }
            return;
        }

        private void ScanLexem(List<char> _peek)
        {
            this._peek = _peek;
            for (; Column < this._peek.Count; ScanBlank())
            {
                if (this._peek[Column] == '"')
                {
                    ScanString();
                }
                else if (this._peek[Column] == '@')
                {
                    Readch();
                    if (_peek[Column] != '"')
                    {
                        AddToken(TokenType.ERROR, 0);
                        Stop();
                    }
                    ScanString();
                }
                else if (char.IsLetter(this._peek[Column]) || this._peek[Column] == '_')
                {
                    ScanIdentifer();
                }
                else if (this._peek[Column] == '\'')
                {
                    ScanChar();
                }
                else if (this._peek[Column] == '/')
                {
                    JumpComment();
                }
                else if (tokenTypeMap.ContainsKey(this._peek[Column].ToString()))
                {
                    ScanSign();
                }
                else if (char.IsNumber(this._peek[Column]))
                {
                    ScanNumber();
                }
                else
                    Stop();
            }
        }

        private void AddToken(TokenType type, int offset)
        {
            s_tokens.Add(new Token(type, Line, Column + offset));
        }

        private void AddToken(TokenType type, string content, int offset)
        {
            Token token = new Token(type, Line, realColumn + offset);
            token.Content = content;
            s_tokens.Add(token);
        }

        private char ScanEC()
        {
            Readch();
            if (!tokenTypeMap.ContainsKey(_peek.ToString())) { return '\0'; } //TODO:Escape символ           
            return s_ECSet[_peek[Column]];
        }


        private void ScanString()
        {
            Readch();
            StringBuilder sb = new StringBuilder();
            for (; _peek[Column] != '"'; Readch())
            {
                if (_peek[Column] == '\n')
                {
                    AddToken(TokenType.ERROR, sb.ToString(), -sb.Length); Stop(); return;
                } //TODO:Вывод ошибки
                if (_peek[Column] == '\\')
                {
                    sb.Append(ScanEC());
                    continue;
                }
                sb.Append(_peek[Column]);
            }
            AddToken(TokenType.STRING, sb.ToString(), -sb.Length - 1);
            Readch();
        }
        private void ScanIdentifer()
        {
            StringBuilder sb = new StringBuilder();
            do
            {
                sb.Append(_peek[Column]);
                Readch();

            } while (char.IsLetterOrDigit(_peek[Column]) || _peek[Column] == '_');

            string s = sb.ToString();
            if (tokenTypeMap.ContainsKey(s))
                AddToken(tokenTypeMap[s], s, -s.Length);
            else
                AddToken(TokenType.IDENTIFIERS, s, -s.Length);
        }
        private void ScanChar()
        {
            Readch();

            if (_peek[Column] == '\\')
            {
                char ec = ScanEC();
                Readch();
                if (_peek[Column] != '\'')
                {
                    AddToken(TokenType.ERROR, _peek[Column - 1] + ec.ToString(), -2);
                    Stop();
                    return;
                } //TODO:слишком много символов
                AddToken(TokenType.CHARACTER_LITERAL, ec.ToString(), -3);
                Readch();
                return;
            }

            char c = _peek[Column];
            Readch();
            if (_peek[Column] != '\'')
            {
                AddToken(TokenType.ERROR, _peek[Column - 1].ToString(), -2);
                Stop();
                return;
            } //TODO:слишком много символов
            AddToken(TokenType.CHARACTER_LITERAL, c.ToString(), -2);
            Readch();
        }

        private void ScanSign()
        {
            char c = _peek[Column];
            Readch();
            if (char.IsSymbol(_peek[Column]) || _peek[Column] == '&' || _peek[Column] == ':' || _peek[Column] == '{' || _peek[Column] == '}' || _peek[Column] == '?' || _peek[Column] == '-')
            {
                string s = c.ToString() + _peek[Column];
                if (tokenTypeMap.ContainsKey(s))
                {
                    AddToken(tokenTypeMap[s], s, -1);
                    Readch();
                    return;
                }
                if (tokenTypeMap.ContainsKey(_peek[Column].ToString()))
                {
                    if (tokenTypeMap.ContainsKey(c.ToString())) AddToken(tokenTypeMap[c.ToString()], c.ToString(), -1);
                    else return;
                    AddToken(tokenTypeMap[_peek[Column].ToString()], _peek[Column].ToString(), 0);
                    Readch();
                    return;
                }
                else return;
            }
            if (tokenTypeMap.ContainsKey(c.ToString()))
            {
                AddToken(tokenTypeMap[c.ToString()], c.ToString(), -1);
            }
            else return;
        }
        private void ScanNumber()
        {
            StringBuilder sb = new StringBuilder();

            while (char.IsDigit(_peek[Column]))
            {
                sb.Append(_peek[Column]);
                Readch();
            }

            ///сканирование на double
            
            if (char.IsLetter(_peek[Column]) || _peek[Column] == '_')
            {
                DoubleleMetod(sb);
                return;
            } //TODO:Идентификатор ошибки не может начинаться с цифры

            ///сканирование на int

            if (_peek[Column] != '.')
            {
                if (int.TryParse(sb.ToString(), out int n))
                {
                    AddToken(TokenType.INTEGER_LITERAL, sb.ToString(), -sb.Length);
                    return;
                }
                AddToken(TokenType.ERROR, sb.ToString(), -sb.Length);
                Stop();
                return; //TODO:константа слишком велика
            }

            sb.Append(_peek[Column]);
            Readch();

            for (; ; Readch())
            {
                if (!char.IsDigit(_peek[Column]))
                {
                    if (_peek[Column] == '.')
                        sb.Append(_peek[Column]);
                    break;
                }
                sb.Append(_peek[Column]);
            }

            ///сканирование на double
            
            if (char.IsLetter(_peek[Column]) || _peek[Column] == '_')
            {
                DoubleleMetod(sb);
                return;

            }

            ///сканирование на float

            if (float.TryParse(sb.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out _))
            {
                AddToken(TokenType.REAL_LITERAL, sb.ToString(), -sb.Length);
            }
            else
            {
                AddToken(TokenType.ERROR, sb.ToString(), -sb.Length);
                Stop();
            }

            return; //TODO: константа с плавающей запятой выходит за пределы диапазона
        }

        private void DoubleleMetod(StringBuilder sb)
        {
            if (_peek[Column] == 'e')
            {
                sb.Append(_peek[Column]);
                Readch();
                if (char.IsDigit(_peek[Column]) || _peek[Column] == '+' || _peek[Column] == '-')
                {
                    sb.Append(_peek[Column]);
                    Readch();
                    while (char.IsDigit(_peek[Column]))
                    {
                        sb.Append(_peek[Column]);
                        Readch();
                    }
                    if (double.TryParse(sb.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out _))
                    {
                        AddToken(TokenType.REAL_LITERAL, sb.ToString(), -sb.Length);
                        return;
                    }
                    else
                    {
                        AddToken(TokenType.ERROR, sb.ToString(), -sb.Length);
                        Stop();
                        return;
                    }
                }
                else
                {
                    AddToken(TokenType.ERROR, sb.ToString(), -sb.Length);
                    Stop();
                    return;
                }
            }
            else
            {
                AddToken(TokenType.ERROR, sb.ToString(), -sb.Length);
                Stop();
                return;
            }
        }

        private void JumpComment()
        {
            Readch();
            if (_peek[Column] == '/')
                for (; ; Readch())
                {
                    if (Column >= _peek.Count)
                        return;
                    if (_peek[Column] == '\n')
                    {
                        AddToken(TokenType.WHITESPACES, 0);
                        return;
                    }

                }

            if (_peek[Column] == '*')
            {
                Readch();
                for (; ; Readch())
                {
                    if (_peek[Column] == '\n')
                    {
                        AddToken(TokenType.WHITESPACES, 0);
                        Readch();
                    }

                    if (_peek[Column] == '*')
                    {
                        Readch();
                        if (_peek[Column] == '/')
                        {
                            Readch();
                            return;
                        }
                    }

                    if (Column > _peek.Count)
                    {
                        AddToken(TokenType.EOF, 0);
                        return;
                    }
                }
            }

            AddToken(TokenType.DIV, "/", -1);
        }

    }
}
















