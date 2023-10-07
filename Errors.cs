using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

namespace HULK
{
    abstract class HulkErrors : Exception 
    {
        public abstract void PrintError();
    }

    class LexicalError : HulkErrors
    {
        public string BadToken ;
        public LexicalError(string BadToken)
        {
            this.BadToken = BadToken ;
        }
        public override void PrintError()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            System.Console.WriteLine($"! LEXICAL ERROR: '{BadToken}' is not a valid token.");
            Console.ForegroundColor = ConsoleColor.Green;
        }
    }

    class SyntaxError : HulkErrors
    {
        public string? Problem ;
        public string ProblemType ;
        public string? ExpressionType ;
        public string Token ;

        public SyntaxError(string Problem , string ProblemType , string ExpressionType , string Token)
        {
            this.Problem = Problem ;
            this.ProblemType = ProblemType ;
            this.ExpressionType = ExpressionType ;
            this.Token = Token ;
        }
        public SyntaxError(string Token , string ProblemType)
        {
            this.Token = Token ;
            this.ProblemType = ProblemType ;
        }
        public override void PrintError()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            if ( ProblemType == "Missing Token")
            {
                System.Console.WriteLine($"! SYNTAX ERROR: {Problem} in '{ExpressionType}' expression after '{Token}' .");
            }
            else if( ProblemType == "Invalid Token")
            {
                System.Console.WriteLine($"! SYNTAX ERROR: {Problem} '{Token}' in '{ExpressionType}' expression");
            }
            else if(ProblemType == "DoNotExistID")
            {
                System.Console.WriteLine($"! SYNTAX ERROR: The name '{Token}' does not exist in the current context");
                Console.ForegroundColor = ConsoleColor.Green;
            }
            else if(ProblemType == "KeyWordID")
            {
                System.Console.WriteLine($"! SYNTAX ERROR: Invalid ID , the name '{Token}' it's a keyword language");
                Console.ForegroundColor = ConsoleColor.Green;
            }
            Console.ForegroundColor = ConsoleColor.Green;
        }
    }

    class UnExpectedToken : HulkErrors 
    {
        public string BadToken ;
        public UnExpectedToken(string BadToken)
        {
            this.BadToken = BadToken ;
        }
        public override void PrintError()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            System.Console.WriteLine($"! ERROR : Unexpected Token '{BadToken}'");
            Console.ForegroundColor = ConsoleColor.Green;
        }
    }

    #region SemanticError 
    abstract class SemanticError : HulkErrors
    {
        public string? ProblemType ;
    }

    class IncorrectOperator : SemanticError
    {
        public string operatorProblem ;
        public string badToken ;
        public string expectedToken ;
        public IncorrectOperator(string badToken , string operatorProblem , string expectedToken)
        {
            ProblemType = "Incorrect Operator" ;
            this.badToken = badToken ;
            this.operatorProblem = operatorProblem ;
            this.expectedToken = expectedToken ;
        }

        public override void PrintError()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{operatorProblem} cannot be applied to operand of type '{badToken}'");
            Console.ForegroundColor = ConsoleColor.Green;
        }
    }

    class IncorrectBinaryExpression : SemanticError
    {
        public string operatorProblem ;
        public string leftTokenType ;
        public string rightTokenType ;

        public IncorrectBinaryExpression(string operatorProblem , string leftTokenType , string rightTokenType )
        {
            ProblemType = "Incorrect Binary Expression" ;
            this.leftTokenType = leftTokenType ;
            this.rightTokenType = rightTokenType ;
            this.operatorProblem = operatorProblem ;
        }

        public override void PrintError()
        {   
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{operatorProblem} cannot be used between '{leftTokenType}' and '{rightTokenType}'");
            Console.ForegroundColor = ConsoleColor.Green;
        }
    }

    class DuplicateArgument : SemanticError
    {
        string badToken ;
        
        public DuplicateArgument(string badToken)
        {
            ProblemType = "DuplicateArgument" ;
            this.badToken = badToken ;
        }

        public override void PrintError()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"The parameter name '{badToken}' is a duplicate");
            Console.ForegroundColor = ConsoleColor.Green;
        }

    }

    class ArgumentTypeError : SemanticError
    {
        string? functionName ;
        public string expectedToken ;
        public string badToken ;

        public ArgumentTypeError(string expectedToken , string badToken , string? functionName = null)
        {
            ProblemType = "ArgumentTypeError" ;
            this.functionName = functionName ;
            this.expectedToken = expectedToken ;
            this.badToken = badToken ;
        }

        public override void PrintError()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Function '{functionName}' receives '{expectedToken}', not '{badToken}'.");
            Console.ForegroundColor = ConsoleColor.Green;
        }
    }

    class ArgumentsCountError : SemanticError
    {
        string functionName ;
        int argumentsIdCount ;
        int argumentsValueCount ;

        public ArgumentsCountError(string functionName , int argumentsIdCount , int argumentsValueCount )
        {
            ProblemType = "ArgumentsCountError" ;
            this.functionName = functionName ;
            this.argumentsIdCount = argumentsIdCount ;
            this.argumentsValueCount = argumentsValueCount ;
        }

        public override void PrintError()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Function '{functionName}' receives {argumentsIdCount} argument(s), but {argumentsValueCount} were given.");
            Console.ForegroundColor = ConsoleColor.Green;
        }
    }

    #endregion

    class DefaultError : HulkErrors
    {
        public string ProblemType ;

        public string? functionName ;

        public DefaultError(string ProblemType)
        {
            this.ProblemType = ProblemType ;
        }
        public DefaultError(string ProblemType , string functionName)
        {
            this.ProblemType = ProblemType ;
            this.functionName = functionName ;
        }
        public override void PrintError()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            if(ProblemType == "DivisionByZero")
            {
                System.Console.WriteLine("! ERROR: Division by constant zero");
            }
            else if (ProblemType == "ErrorFunctionBody")
            {
                System.Console.WriteLine("! ERROR: Invalid Function Declaration.");
            }
            else if (ProblemType == "StackOverflow")
            {
                Console.WriteLine("! ERROR: Stack Overflow " + functionName);
            }
            Console.ForegroundColor = ConsoleColor.Green;
        }
    }
    
}
