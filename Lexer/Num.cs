namespace compiller.Lexer
{
    class Num : Token
    {
        public int value;
        public Num(int t) : base(Tag.NUM)
        {
            value = t;
        }

        public override string ToString() { return value.ToString(); }

    }
}
