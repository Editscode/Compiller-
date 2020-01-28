using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Compiller.LexicalAnalysis;

namespace Compiller
{
    class App
    {
        static void Main(string[] args)
        {
            string[] lines = new[]
            {
                @"test\1.b"
            };
            Lexer lexer = new Lexer();
            lexer.Scan(lines);




            for (int i = 0; i < lexer.s_tokens.Count; i++)
            {
                var cells = lexer.s_tokens.ToArray();
                foreach (Token cell in cells[i])
                {
                    Console.WriteLine("Line " + cell.Line + "\tColumn " + cell.Column + "\tContent " + cell.Content +
                                      "\tType " + cell.Type);
                }
                Console.WriteLine();
            }

            Console.ReadKey();
        }
    }
}
        

