using System;
using System.Collections.Generic;
using System.IO;



namespace compiller
{
    class App
    {
        static void Main(string[] args)
        {
            compiller.Lexer.Lexer lex = new compiller.Lexer.Lexer();
            Parser parse = new Parser(lex);
            parse.program();
        }
    }
}
            /**
            using (StreamReader sr = File.OpenText(args[0]))
            {
                string s,str = "";
                while ((s = sr.ReadLine()) != null)
                {
                    str += s;
                    
                }
                var t =new Tokenizer(str);
                Console.WriteLine(t.Next());
                
            }
        }
    **/
    
/**
    public enum TokenType
    {
        Int,
        Double,
        String,
        Bool,
        Char,
        operators,
        punctuators,
        constans,
        identifiers,
        keyWords,
    }
    public class Token
    {
        public int RowPos { get; set; }
        public int ColumnPos { get; set; }
        public string LiteralValue { get; set; }
        public TokenType TokenType { get; set; }
        public string SourceValue { get; set; }

        public override string ToString()
        {
            return SourceValue;
        }

    }
    public class Tokenizer
    {
        string str;
        public Token Next()
        {

            return
                new Token()
                {
                    SourceValue = "asdfdsfsd"
                };
        }
        public Tokenizer(string str)
        {
            this.str = str;

        }
    }
**/

