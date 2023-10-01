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
            Console.WriteLine();
            //int c = 0 ;
            while(true)
            {
                Lexer.Restart();//Next line , clean the input and restart the index 
            
                Console.Write("> ");

                string? input = Console.ReadLine();

                if(input == null)
                {
                    continue ;
                }

                //string input = "print(\"hola\" + 5)";
                
                /*
                if(c == 0 )
                {
                    input = "function v( x ) => print ( x)" ;
                    c++;
                }
                else input = "v( 3 ) ;";
                */

                //Poner linda clase de errores .
                
                if(input == "exit")
                {
                    break;
                }

                try
                {
                    Lexer.TokenizeInput(input);

                    Expression result = new HulkExpression() ;

                    result.Evaluate();

                    if((Lexer.index >= Lexer.Tokens.Count || Expression.ActualToken() != ";") && Lexer.Tokens.Count != 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Red ;
                        System.Console.WriteLine("Missing ' ; '");
                        Console.ForegroundColor = ConsoleColor.Green;
                    }
                    else
                    {
                        foreach(string Prints in Lexer.ConsolePrints)
                        {
                            Console.WriteLine(Prints);
                        }
                    }
                }
                catch (HulkErrors he)
                {
                    he.PrintError();
                }
            }
            Console.ForegroundColor = ConsoleColor.White;
        
        }
    }
}
