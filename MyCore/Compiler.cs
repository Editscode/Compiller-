using MyCore.LexicalAnalysis;
using System;
using System.Collections.Generic;
using System.IO;

namespace MyCore
{
    public class Compiler
    {
        public static void Compile(string[] files)
        {
            string text = "";
            for (int b = 0; b < 55; b++)
            {
                Lexer lexer = new Lexer();
                StreamReader _streamReader = new StreamReader(new FileStream($"{b}.cs", FileMode.Open));
                lexer.Scan(_streamReader);
                var bac = lexer.s_tokens;
                for (int i = 0; i < bac.Count; i++)
                {
                    if (bac[i].Type != TokenType.WHITESPACES)
                    {
                        text += bac[i].Line + "\t" + bac[i].Column + "\t" + bac[i].Content + "\t" + bac[i].Type + "\n";
                        Console.WriteLine("{0}\t | {1}\t | {3}\t | {2}\t", bac[i].Line, bac[i].Column, bac[i].Type, bac[i].Content);
                    }
                }
                Console.WriteLine("_____________________________" + b);
                using (FileStream fstream = new FileStream(@$"\out\{b}.txt", FileMode.Create))
                        {
                            byte[] array = System.Text.Encoding.Default.GetBytes(text);
                            fstream.Write(array, 0, array.Length);
                        }
                text = "";


            }
          
            //for (int b = 0; b < files.Length; b++)
            //{
            //    for (int i = 0; i < lexer.s_tokens[b].Count; i++)
            //    {
            //        if (lexer.s_tokens[b][i].Type != TokenType.EOF && lexer.s_tokens[b][i].Type != TokenType.WHITESPACES)
            //        {
            //            text += lexer.s_tokens[b][i].Line + "\t" + lexer.s_tokens[b][i].Column + "\t" + lexer.s_tokens[b][i].Content + "\t" + lexer.s_tokens[b][i].Type + "\n";
            //            Console.WriteLine("{0,-8}\t  |  {1}\t", lexer.s_tokens[b][i].Content, lexer.s_tokens[b][i].Type);
            //        }
            //    }
            //    using (FileStream fstream = new FileStream(@$"\out\{b+2}.txt", FileMode.OpenOrCreate))
            //    {
            //        byte[] array = System.Text.Encoding.Default.GetBytes(text);
            //        fstream.Write(array, 0, array.Length);
            //    }
            //    text = "";
            //}
        }

        /*
        string text = "";
        while (lexer.NextToken.Type != TokenType.EOF)
        {
            // Console.WriteLine("Line \t\t\t\t |" + lexer.NextToken + "\tTokenType " + lexer.NextToken.Type);

            Console.WriteLine("{0,-8}\t  |  {1}\t", lexer.NextToken, lexer.NextToken.Type);
            lexer.Next();
        }



        Console.ReadKey();
        for (int b = 0; b < lexer.s_tokens.Count; b++)
        {
            Console.WriteLine(files[b]);
            Console.WriteLine("_____________________");
            for (int i = 0; i < lexer.s_tokens[b].Count; i++)
            {
                if (lexer.s_tokens[b][i].Type!= TokenType.EOF) 
                {
                    text += lexer.s_tokens[b][i].Line + "\t"+ lexer.s_tokens[b][i].Column + "\t" + lexer.s_tokens[b][i].Content + "\t" + lexer.s_tokens[b][i].Type + "\n";
                    Console.WriteLine("{0,-8}\t  |  {1}\t", lexer.s_tokens[b][i].Content, lexer.s_tokens[b][i].Type);
                }
                using (FileStream fstream = new FileStream($"{path}" + @"\"+files[b], FileMode.OpenOrCreate))
                {
                    // преобразуем строку в байты
                    byte[] array = System.Text.Encoding.Default.GetBytes(text);
                    // запись массива байтов в файл
                    fstream.Write(array, 0, array.Length);
                }
            }
            text = "";
            Console.WriteLine("\n");

        }*/
    }
}

