using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MyCore.LexicalAnalysis
{

    public class Recognizer
    {
        string[] LiteralNames;
        public Recognizer(String []LiteralNames)
        {
            this.LiteralNames = LiteralNames;
        }
        public IDictionary<string, TokenType> TokenTypeMap => CreateTokenTypeMap();
        private IDictionary<string, TokenType> CreateTokenTypeMap()
        {
            var result = new Dictionary<string, TokenType>();
            for (int i = 1; i <= Enum.GetNames(typeof(TokenType)).Length - 1; i++)
            {
                TokenType tokenTypeI = (TokenType)i;
                
                string symbolicName = LiteralNames[i];

                if (symbolicName != null)
                {
                    result[symbolicName] = (TokenType)i;
                }
                else 
                {
                    string literalName =  tokenTypeI.ToString();
                    result[literalName] = (TokenType)i;
                }
            }
            result["EOF"] = TokenType.EOF;
            return result;
        }


    }
}
