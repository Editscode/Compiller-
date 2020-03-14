using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System;
using System.Globalization;

namespace MyCore.LexicalAnalysis
{
    internal class Lexer
    {
        private Token NextToken;


        private int Line;
        private int Column;

        private List<char> _peek = new List<char>();


        private static IDictionary<string, TokenType> TokenTypeMap = new Dictionary<string, TokenType>();


        public readonly List<Token> s_tokens = new List<Token>();


        private static readonly Dictionary<char, char> s_ECSet = new Dictionary<char, char>();
        private static Recognizer a = new Recognizer();

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
            TokenTypeMap = a.TokenTypeMap;
            s_ECSet.Add('\"', '\"');
            s_ECSet.Add('\'', '\'');
            s_ECSet.Add('\\', '\\');
            s_ECSet.Add('b', '\b');
            s_ECSet.Add('f', '\f');
            s_ECSet.Add('t', '\t');
            s_ECSet.Add('r', '\r');
            s_ECSet.Add('n', '\n');
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
        }
        private void Stop()
        {
            Column = _peek.Count;
        }
        private void ScanLexem(List<char> _peek)
        {
            List<Token> tokens = new List<Token>();
            this._peek = _peek;
            for (; Column < this._peek.Count; ScanBlank())
            {
                if (this._peek[Column] == '"')
                {
                    ScanString();
                    continue;
                }
                if (char.IsLetter(this._peek[Column]) || this._peek[Column] == '_' || _peek[Column] == '&' || _peek[Column] == '-')
                {
                    ScanIdentifer();
                    continue;
                }
                if (this._peek[Column] == '\'')
                {
                    ScanChar();
                    continue;
                }
                if (this._peek[Column] == '/')
                {
                    JumpComment();
                    continue;
                }
                if (char.IsSymbol(this._peek[Column]) || TokenTypeMap.ContainsKey(this._peek[Column].ToString()))
                {
                    ScanSign();
                    continue;
                }
                if (char.IsNumber(this._peek[Column]))
                {
                    ScanNumber();
                    continue;
                }
            }
            return;
        }



        private void AddToken(TokenType type, int offset)
        {
            s_tokens.Add(new Token(type, Line, Column + offset));
            switch (type)
            {
                case TokenType.EOF:
                    break;
                case TokenType.WHITESPACES:
                    Line++;
                    break;
                case TokenType.ERROR:
                    break;
            }
        }

        private void AddToken(TokenType type, string content, int offset)
        {
            Token token = new Token(type, Line, Column + offset);
            token.Content = content;
            s_tokens.Add(token);
        }

        private char ScanEC()
        {
            Readch();
            if (!TokenTypeMap.ContainsKey(_peek.ToString())) { return '\0'; } //TODO:Escape символ           
            return s_ECSet[_peek[Column]];
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
                        case '\n':
                           
                            Column++;
                            AddToken(TokenType.WHITESPACES, 0);
                            continue;
                    }
                    break;
                }
            Column++;
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
               
            } while (char.IsLetterOrDigit(_peek[Column]) || _peek[Column] == '_' || _peek[Column] == '&' || _peek[Column] == '-');

            string s = sb.ToString();
            if (TokenTypeMap.ContainsKey(s))
                AddToken(TokenTypeMap[s], s, -s.Length);
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
                if (_peek[Column] != '\'') { return; } //TODO:слишком много символов
                AddToken(TokenType.CHARACTER_LITERAL, ec.ToString(), -3);
                Readch();
                return;
            }
            
            char c = _peek[Column];
            Readch();
            if (_peek[Column] != '\'')
            {
                AddToken(TokenType.ERROR, c.ToString(), -2);
                return;
            } //TODO:слишком много символов
            AddToken(TokenType.CHARACTER_LITERAL, c.ToString(), -2);
            Readch();
        }

        private void ScanSign()
        {
            char c = _peek[Column];
            if (char.IsSymbol(_peek[Column]))
            {
                string s = c.ToString() + _peek[Column];
                if (TokenTypeMap.ContainsKey(s))
                {
                    AddToken(TokenTypeMap[s], s, Column);
                    Readch();
                    return;
                }
                if (TokenTypeMap.ContainsKey(_peek[Column].ToString()))
                {
                    if (TokenTypeMap.ContainsKey(c.ToString())) AddToken(TokenTypeMap[c.ToString()], c.ToString(), Column);
                    else return;
                    AddToken(TokenTypeMap[_peek[Column].ToString()], _peek[Column].ToString(), Column);
                    Readch();
                    return;
                }
                else return;
            }
            if (TokenTypeMap.ContainsKey(c.ToString()))
                AddToken(TokenTypeMap[c.ToString()], c.ToString(), -1);
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

            if (char.IsLetter(_peek[Column]) || _peek[Column] == '_')
            {
                AddToken(TokenType.ERROR, sb.ToString(), -sb.Length);
                Stop();
                return;
            } //TODO:Идентификатор ошибки не может начинаться с цифры
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

            if (char.IsLetter(_peek[Column]) || _peek[Column] == '_')
            {
                if (_peek[Column] == 'e')
                {
                    sb.Append(_peek[Column]);
                    Readch();
                    for (; ; Readch())
                    {
                        if (!char.IsDigit(_peek[Column])) break;
                        sb.Append(_peek[Column]);
                    }
                }
                AddToken(TokenType.ERROR, sb.ToString(), -sb.Length);
                Stop();
                return;
            }
// TODO: Идентификатор ошибки не может начинаться с цифры

            if (float.TryParse(sb.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out _)) { 
                AddToken(TokenType.REAL_LITERAL, sb.ToString(), -sb.Length); 
            }
            else
            {
                AddToken(TokenType.ERROR, sb.ToString(), -sb.Length);
                Stop();
            }
            return; //TODO: константа с плавающей запятой выходит за пределы диапазона
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

                    if (Column < _peek.Count)
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
















