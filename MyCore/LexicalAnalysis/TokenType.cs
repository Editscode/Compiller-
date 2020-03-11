namespace MyCore.LexicalAnalysis
{
    public enum TokenType
    {
        FloatLiteralToken,
        StringLiteralToken,
        NumericLiteralToken,
        CharacterLiteralToken,

        ERRORTOKEN,

        IntKeyword,
        DoubleKeyword,

        TrueKeyword,
        FalseKeyword,
        IfKeyword,
        ElseKeyword,
        WhileKeyword,
        ForKeyword,
        ClassKeyword,
        StructKeyword,
        InterfaceKeyword,
        NameSpaceKeyword,
        UsingKeyword,
        FuncKeyword,
        VarKeyword,
        LetKeyword,
        InvKeyword,
        CastKeyword,
        NewKeyword,
        OverrideKeyword,
        StaticKeyword,
        PublicKeyword,
        ContinueKeyword,
        ReturnKeyword,
        BreakKeyword,

        Sign,
        Colon,
        Assign,
        OpenParen,
        CloseParen,
        OpenBracket,
        CloseBracket,
        Semicolon,
        OpenBrace,
        CloseBrace,
        BasicType,
        BasicTypeI,
        Identifer,
        LeftShift,
        RightShift,

        Blank, EOL, EOF,
    }
}