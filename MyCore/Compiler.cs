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
            List<string> a = new List<string>();
            for (int b = 0; b < 63; b++)
            {
                a.Add("000.csc.txt");
            }
            Lexer lexer = new Lexer();
            lexer.Scan(files);
            for (int b = 0; b < files.Length; b++)
            {


                for (int i = 0; i < lexer.s_tokens[b].Count; i++)
                {
                    if (lexer.s_tokens[b][i].Type != TokenType.EOF && lexer.s_tokens[b][i].Type != TokenType.WHITESPACES)
                    {
                        text += lexer.s_tokens[b][i].Line + "\t" + lexer.s_tokens[b][i].Column + "\t" + lexer.s_tokens[b][i].Content + "\t" + lexer.s_tokens[b][i].Type + "\n";
                        Console.WriteLine("{0,-8}\t  |  {1}\t", lexer.s_tokens[b][i].Content, lexer.s_tokens[b][i].Type);
                    }
                }
                using (FileStream fstream = new FileStream(@$"\out\{b+2}.txt", FileMode.OpenOrCreate))
                {
                    byte[] array = System.Text.Encoding.Default.GetBytes(text);
                    fstream.Write(array, 0, array.Length);
                }
                text = "";
            }
        }

        /*
        string text = "";
        while (lexer.NextToken.Type != TokenType.EOF)
        {
            // Console.WriteLine("Line \t\t\t\t |" + lexer.NextToken + "\tTokenType " + lexer.NextTokenType);

            Console.WriteLine("{0,-8}\t  |  {1}\t", lexer.NextToken, lexer.NextTokenType);
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

