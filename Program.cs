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
                HULK_Errors.Found = false ;
                foreach(string id in Function_Declaration.Function_Stack.Keys)
                {
                    Function_Declaration.Function_Stack[id] = 0 ;
                }
                Console.Write("> ");

               // string input = Console.ReadLine();

                string input = " ";
                
                /*
                if(c == 0 )
                {
                    input = "function v( x ) => print ( x)" ;
                    c++;
                }
                else input = "v( 3 ) ;";
                */

                //List PROBLEMS
                //Semantics errors
                //Expressiones Unarias
                
                
                if(input == "exit")
                {
                    break;
                }

                Lexer.index = 0;
                Lexer.Tokens.Clear();
                try
                {
                    Lexer l = new Lexer(input);

                    Expression result = new HulkExpression() ;

                    result.Evaluate();
                    if((Lexer.index >= Lexer.Tokens.Count || Lexer.Tokens[Lexer.index] != ";") && Lexer.Tokens.Count != 0)
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
                         Lexer.ConsolePrints.Clear();
                    }

                }
                catch (HULK_Errors he)
                {
                    he.PrintError();
                }

            }
            Console.ForegroundColor = ConsoleColor.White;
        
        }
    }
}
