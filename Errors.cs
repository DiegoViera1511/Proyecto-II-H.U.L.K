using System.Text.RegularExpressions;

namespace HULK
{
    static class HULK_Errors
    {
        public static bool Found = false ;
        public static void LexicalError(string Token)
        {
            Found = true ;
            Console.ForegroundColor = ConsoleColor.Red;
            System.Console.WriteLine($"! LEXICAL ERROR: '{Token}' is not a valid token.");
            Console.ForegroundColor = ConsoleColor.Green;
        }   

        public static void SyntaxError(string Problem , string ProblemType , string ExpressionType , string Token)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Found = true;
            if ( ProblemType == "Missing Token")
            {
                System.Console.WriteLine($"! SYNTAX ERROR: {Problem} in '{ExpressionType}' expression after '{Token}' .");
            }
            else if( ProblemType == "Invalid Token")
            {
                System.Console.WriteLine($"! SYNTAX ERROR: {Problem} '{Token}' in '{ExpressionType}' expression");
            }
            Console.ForegroundColor = ConsoleColor.Green;
        }
        public static void UnExpectedToken(string Token)
        {
            Found = true ;
            Console.ForegroundColor = ConsoleColor.Red;
            System.Console.WriteLine($"! ERROR : Unexpected Token '{Token}'");
            Console.ForegroundColor = ConsoleColor.Green;
        }

        public static void NoID(string Token , string ProblemType)
        {
            Found = true ;
            Console.ForegroundColor = ConsoleColor.Red;
            if(ProblemType == "DoNotExistID")
            {
                System.Console.WriteLine($"! ERROR : The name '{Token}' does not exist in the current context");
                Console.ForegroundColor = ConsoleColor.Green;
            }
            else if(ProblemType == "KeyWordID")
            {
                System.Console.WriteLine($"! ERROR : Invalid ID , the name '{Token}' it's a keyword language");
                Console.ForegroundColor = ConsoleColor.Green;
            }
        }


        public static void SemanticError(string Problem , string ProblemType ,string TokenType = "token" , string Token2Type = "token" )
        {
            Found = true ;
            Console.ForegroundColor = ConsoleColor.Red;
            if(ProblemType == "Incorrect Operator")
            {
                System.Console.WriteLine($"{Problem} cannot be applied to operand of type '{TokenType}'");
            }
            else if(ProblemType == "Incorrect Binary Expression")
            {
                System.Console.WriteLine($"{Problem} cannot be used between '{TokenType}' and '{Token2Type}'");
            }
            
            Console.ForegroundColor = ConsoleColor.Green;
        }
        
    }
}
