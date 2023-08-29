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
                
                Expression result = new HulkExpression();
                result.Evaluate();

                if(Lexer.index >= Lexer.Tokens.Count || Lexer.Tokens[Lexer.index] != ";")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Se esperaba \" ; \"");
                    Console.ForegroundColor = ConsoleColor.Green;
                }
        
            }
            
        }
    }
}
