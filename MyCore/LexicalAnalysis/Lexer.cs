using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace MyCore.LexicalAnalysis
{
    internal class Lexer
    {
        private Token NextToken;
        private string NextTokenContent;
        private TokenType NextTokenType;

        private int Line = 1;
        private int Column = 0;
        private int FileIndex;
        private static int FileCount;
        private char _peek = ' ';
        private static IDictionary<string, TokenType> map = new Dictionary<string, TokenType>();

        private StreamReader _streamReader;
        private string[] _files;

        //подача нескольких слов
        public readonly List<List<Token>> s_tokens = new List<List<Token>>();
        private int _index;
        //private static readonly Dictionary<string, Recognizer.IVocabulary> map = new Dictionary<string, Recognizer.IVocabulary>();
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
            map = a.TokenTypeMap;
            s_ECSet.Add('\"', '\"');
            s_ECSet.Add('\'', '\'');
            s_ECSet.Add('\\', '\\');
            s_ECSet.Add('b', '\b');
            s_ECSet.Add('f', '\f');
            s_ECSet.Add('t', '\t');
            s_ECSet.Add('r', '\r');
            s_ECSet.Add('n', '\n');
        }


        private Token Next()
        {

            while (true)
            {
                NextToken = s_tokens[FileIndex][_index++];
                switch (NextToken.Type)
                {
                    case TokenType.WHITESPACES:
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


        public void Scan(string[] files)
        {
            _files = files;
            for (int i = 0; i < _files.Length; i++)
                s_tokens.Add(new List<Token>());

            for (int i = 0; i < _files.Length; i++, FileIndex = i)
            {
                _streamReader = new StreamReader(new FileStream(_files[i], FileMode.Open));

                ScanFiles(_streamReader);
            }

            FileIndex = 0;
            Line = 1;
            _index = 0;
            FileCount = files.Length;
            Next();
        }


        private void ScanFiles(StreamReader _streamReader)
        {
            Readch();
            for (; !(_streamReader.BaseStream.Length < Column); ScanBlank())
            {
                if (char.IsLetter(_peek) || _peek == '_' || _peek == '&' || _peek == '-')
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
                if (char.IsSymbol(_peek) || map.ContainsKey(_peek.ToString()))
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
        private void ScanIdentifer()
        {
            StringBuilder sb = new StringBuilder();
            do
            {
                sb.Append(_peek);
                Readch();
            } while (char.IsLetterOrDigit(_peek) || _peek == '_' || _peek == '&' || _peek == '-');

            string s = sb.ToString();
            if (map.ContainsKey(s))
                AddToken(map[s], s, -s.Length);
            else
                AddToken(TokenType.iNDIFICATOR, s, -s.Length);
        }

        private void AddToken(TokenType type, int offset)
        {
            s_tokens[FileIndex].Add(new Token(type, Line, Column + offset));
            switch (type)
            {
                case TokenType.EOF:
                    FileIndex++;
                    break;
                case TokenType.WHITESPACES:
                    Line++;
                    break;
            }
            Column = 0;
        }

        private void AddToken(TokenType type, string content, int offset)
        {
            Token token = new Token(type, Line, Column + offset);
            token.Content = content;
            s_tokens[FileIndex].Add(token);
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
                        AddToken(TokenType.WHITESPACES, 0);
                        continue;
                }
                if (_streamReader.BaseStream.Length < Column)
                {
                    AddToken(TokenType.EOF, 0);
                    break;
                }
                break;
            }
        }

        private void ScanSign()
        {
            char c = _peek;
            Readch();
            if (char.IsSymbol(_peek))
            {
                string s = c.ToString() + _peek;
                if (map.ContainsKey(s))
                {
                    AddToken(map[s], s, -1);
                    Readch();
                    return;
                }
                if (map.ContainsKey(_peek.ToString()))
                {
                    if (map.ContainsKey(c.ToString())) AddToken(map[c.ToString()], c.ToString(), -1);
                    else return;
                    AddToken(map[_peek.ToString()], _peek.ToString(), 0);
                    Readch();
                    return;
                }
                else return;
            }
            if (map.ContainsKey(c.ToString()))
                AddToken(map[c.ToString()], c.ToString(), -1);
            else return;
        }



        private void ScanNumber()
        {
            StringBuilder sb = new StringBuilder();
            while (char.IsDigit(_peek))
            {
                sb.Append(_peek);
                Readch();
            }

            if (char.IsLetter(_peek) || _peek == '_')
            {
                AddToken(TokenType.ERROR, sb.ToString(), -sb.Length);
                return;
            } //TODO:Идентификатор ошибки не может начинаться с цифры
            if (_peek != '.')
            {
                if (int.TryParse(sb.ToString(), out int n))
                {
                    AddToken(TokenType.INTEGER_LITERAL, sb.ToString(), -sb.Length);
                    return;
                }
                AddToken(TokenType.ERROR, sb.ToString(), -sb.Length);
                return; //TODO:константа слишком велика
            }

            sb.Append(_peek);
            Readch();
            for (; ; Readch())
            {
                if (!char.IsDigit(_peek))
                {
                    if (_peek == '.')
                        sb.Append(_peek);
                    break;
                }
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

                AddToken(TokenType.ERROR, sb.ToString(), -sb.Length);
                return;
            }
            //TODO:Идентификатор ошибки не может начинаться с цифры

            if (float.TryParse(sb.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out _))
                AddToken(TokenType.REAL_LITERAL, sb.ToString(), -sb.Length);
            else
            {
                AddToken(TokenType.ERROR, sb.ToString(), -sb.Length);
            }
            //AddToken(TokenType.ERRORTOKEN, sb.ToString(), -sb.Length);
            return; //TODO: константа с плавающей запятой выходит за пределы диапазона
        }



        private void JumpComment()
        {
            Readch();
            if (_peek == '/')
                for (; ; Readch())
                {
                    if (_peek == '\n')
                    {
                        AddToken(TokenType.WHITESPACES, 0);
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
                        AddToken(TokenType.WHITESPACES, 0);
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

            AddToken(TokenType.DIV, "/", -1);
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
            AddToken(TokenType.STRING, sb.ToString(), -sb.Length - 1);
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
                AddToken(TokenType.CHARACTER_LITERAL, ec.ToString(), -3);
                Readch();
                return;
            }
            char c = _peek;
            Readch();
            if (_peek != '\'')
            {
                AddToken(TokenType.ERROR, c.ToString(), -2);
                return;
            } //TODO:слишком много символов
            AddToken(TokenType.CHARACTER_LITERAL, c.ToString(), -2);
            Readch();
        }

        private char ScanEC()
        {
            Readch();
            if (!map.ContainsKey(_peek.ToString())) { return '\0'; } //TODO:Escape символ           
            return s_ECSet[_peek];
        }

    }

}


