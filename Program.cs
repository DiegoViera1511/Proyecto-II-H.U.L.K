using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Globalization;

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

            while(true) 
            {
                Lexer.Restart();//Next line , clean the input and restart the index 
            
                Console.Write("> ");

                string? input = Console.ReadLine();

                if(input == null)
                {
                    continue ;
                }

                //tester debugg
                //string input = "print ( 5 )"; 

                if(input == "stop hulk")
                {
                    break;
                }

                try
                {
                    Lexer.TokenizeInput(input);

                    Expression result = new HulkExpression() ;

                    result.Analize();// analiza la expresión primero antes de evaluar , verificando types
                    Lexer.index = 0; // reinicio el index
                    result.Evaluate(); // evalua la expresión

                    if((Lexer.index >= Lexer.Tokens.Count || Expression.ActualToken() != ";") && Lexer.Tokens.Count != 0)
                    {
                        throw new DefaultError("Missing endOfFile");
                    }
                    else
                    {
                        foreach(object Prints in Lexer.ConsolePrints)
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


