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

                //tester
                //string input = "print(\"hola\" + 5)"; 

                //Guardar el token en una variable y luego si entro al error ver si está en functon ids
        
                if(input == "stop hulk")
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
                        throw new DefaultError("Missing endOfFile");
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


