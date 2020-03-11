namespace MyCore.LexicalAnalysis
{
    public class Token
    {
        public TokenType Type;
        public string Content;

        public int Line;
        public int Column;
        public int File;


        public Token(TokenType tokenType, int line, int column)
          => (Type, Line, Column) = (tokenType, line, column);

        public override string ToString() => Content ?? Type.ToString();
    }
}
