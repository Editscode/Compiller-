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
            Lexer lexer = new Lexer();
            lexer.Scan(args);
        }
    }
}
        

