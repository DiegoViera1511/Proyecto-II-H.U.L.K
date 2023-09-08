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
            Function_Declaration.Function_Store.Clear();
            int c = 0 ;
            while(true)
            {
                Console.Write("> ");

               string input = Console.ReadLine();

                /*
                string input;
                if(c == 0 )
                {
                    input = "function fac(n) => if( n == 1 ) 1 else n * fac(n - 1) ;" ;
                    c++;
                }
                else input = "let n = 3 in fac(n);";
                */
                
            
                

                
            
                
                
                if(input == "exit")
                {
                    break;
                }

                Lexer.index = 0;
                Lexer.Tokens.Clear();
                Lexer l = new Lexer(input);

                /*
                foreach(string m in Lexer.Tokens)
                {
                    Console.WriteLine(m);
                }
                */
    
                Expression result = new HulkExpression();
                result.Evaluate();
               

                if(Lexer.index >= Lexer.Tokens.Count || Lexer.Tokens[Lexer.index] != ";")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Se esperaba \" ; \"");
                    Console.ForegroundColor = ConsoleColor.Green;
                }
        
            }

            Console.ForegroundColor = ConsoleColor.White;
        
        }
    }
}
