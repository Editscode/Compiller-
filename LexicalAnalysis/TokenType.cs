﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Compiller.LexicalAnalysis
{
    public enum TokenType
    {
        FloatLiteralToken,
        StringLiteralToken,
        NumericLiteralToken,
        CharacterLiteralToken,

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

        Blank, EOL, EOF,
    }
}
