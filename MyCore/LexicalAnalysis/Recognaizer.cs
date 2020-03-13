using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MyCore.LexicalAnalysis
{
    /// <summary>Get a map from token names to token types.</summary>
    /// <remarks>
    /// Get a map from token names to token types.
    /// <p>Used for XPath and tree pattern compilation.</p>
    /// </remarks>
    public class Recognizer
    {
        [NotNull]
        public IDictionary<string, TokenType> TokenTypeMap => CreateTokenTypeMap();
        private IDictionary<string, TokenType> CreateTokenTypeMap()
        {
            var result = new Dictionary<string, TokenType>();
            for (int i = 1; i <= Enum.GetNames(typeof(TokenType)).Length - 1; i++)
            {
                TokenType myTest = (TokenType)i;
                string literalName = myTest.ToString();

                result[literalName] = (TokenType)i;

                string symbolicName = Lexer.LiteralNames[i];
                if (symbolicName != null)
                {
                    result[symbolicName] = (TokenType)i;
                }
            }
            result["EOF"] = TokenType.EOF;
            return result;
        }


    }
}
