using System;
using System.Collections.Generic;
using System.IO;
using Compiller.LexicalAnalysis;

namespace Compiller
{
    class App
    {
        static void Main(string[] args)
        {
            string[] lines = new[] { @"Example.b" };
            Lexer lexer = new Lexer();
            lexer.Scan(lines);
            var cells = lexer.s_tokens.ToArray();
            foreach (Token cell in cells[0])
                Console.WriteLine("Line "+cell.Line+ "\tColumn " + cell.Column+ "\tContent " + cell.Content + "\tType " + cell.Type);
            Console.WriteLine();

            Console.ReadKey();
        }
    }
}
        

