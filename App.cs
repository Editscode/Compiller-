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
                @"test\10.b", @"test\9.b", @"test\8.b", @"test\7.b", @"test\6.b", @"test\5.b", @"test\4.b", @"test\3.b", @"test\2.b", @"test\1.b",@"test\Example.b",
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
        

