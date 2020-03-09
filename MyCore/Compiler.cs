using MyCore.LexicalAnalysis;
using System;

namespace MyCore
{
    public class Compiler
    {
        public static void Compile(string[] files)
        {
            Lexer lexer = new Lexer();
            lexer.Scan(files);
            
            while (lexer.NextToken.Type != TokenType.EOF)
            {
                // Console.WriteLine("Line \t\t\t\t |" + lexer.NextToken + "\tTokenType " + lexer.NextTokenType);
                Console.WriteLine("{0,-8}\t  |  {1}\t", lexer.NextToken, lexer.NextTokenType);
                lexer.Next();
            }
            Console.ReadKey();
            double a = 55_121;
            for (int b = 0; b < lexer.s_tokens.Count; b++)
            {
                Console.WriteLine(files[b]);
                Console.WriteLine("_____________________");
                for (int i = 0; i < lexer.s_tokens[b].Count; i++)
                {
                   
                    if (lexer.s_tokens[b][i].Type!= TokenType.EOF) 
                    { 
                    Console.WriteLine("{0,-8}\t  |  {1}\t", lexer.s_tokens[b][i].Content, lexer.s_tokens[b][i].Type);
                    }
                  
                }
                Console.WriteLine("\n");

            }
        }
    }
}
