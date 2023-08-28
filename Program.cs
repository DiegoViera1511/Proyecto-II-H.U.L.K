using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace HULK
{
    class Program
    {
        public static void Main(string[] args )
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("H.U.L.K");
            System.Console.WriteLine();
            while(true)
            {
                Console.Write("> ");
                string input = Console.ReadLine();
                if(input == "exit")
                {
                    break;
                }
                Lexer.index = 0;
                Lexer.Tokens.Clear();
                Lexer l = new Lexer(input);
                
                Expression result = new let_in();

                Expression r = new N();

                r.Evaluate();
                System.Console.WriteLine(r.value);

                if(Regex.IsMatch(Lexer.Tokens[Lexer.index], @"let"))
                {
                    Lexer.index++;
                    result.Evaluate();
                    System.Console.WriteLine(result.value);
                }

            }

        }
    }
}
