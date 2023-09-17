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

                string input = Console.ReadLine();

                //string input = "print ( 5 + 2 * 4) ;";
                
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
                Lexer l = new Lexer(input);

                /*
                foreach(string m in Lexer.Tokens)
                {
                    Console.WriteLine(m);
                }
                */

                Expression result = new HulkExpression() ;

                if(!HULK_Errors.Found)
                {
                    result.Evaluate();
                }

                if(! HULK_Errors.Found && (Lexer.index >= Lexer.Tokens.Count || Lexer.Tokens[Lexer.index] != ";"))
                {
                    HULK_Errors.Found = true ;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Se esperaba \" ; \"");
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                
                if (! HULK_Errors.Found )
                {
                    foreach(string Prints in Lexer.ConsolePrints)
                    {
                        Console.WriteLine(Prints);
                    }
                }
                Lexer.ConsolePrints.Clear();

            }

            Console.ForegroundColor = ConsoleColor.White;
        
        }
    }
}
