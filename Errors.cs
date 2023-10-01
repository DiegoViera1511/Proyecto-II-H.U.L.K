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


    class SemanticError : HulkErrors
    {
        public string? ProblemType ;
        public string? Problem ;
        public string? BadToken ;
        public string? ExpectedToken ;
        public string? LeftToken ;
        public string? RightToken ;
        

        
        public SemanticError(string Problem , string ProblemType  , string BadToken )
        {
            this.Problem = Problem ;
            this.BadToken = BadToken ; 
            this.ProblemType = ProblemType;
        }
        public SemanticError(string Problem , string ProblemType ,string LeftToken , string RightToken , string expectedToken , string BadToken)
        {
            this.Problem = Problem ;
            this.LeftToken = LeftToken ;
            this.RightToken = RightToken ;
            this.ProblemType = ProblemType ;
            this.BadToken = BadToken ;
            this.ExpectedToken = expectedToken ;
        }
        public SemanticError(string Problem ,string ProblemType ,  string expectedToken , string BadToken)
        {
            this.BadToken = BadToken ;
            this.Problem = Problem ;
            this.ExpectedToken = expectedToken ;
            this.ProblemType = ProblemType ;
        }
        public SemanticError(string BadToken , string ProblemType)
        {
            this.BadToken = BadToken ;
            this.ProblemType = ProblemType ;
        }
    
        public override void PrintError()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            if(ProblemType == "Incorrect Operator")
            {
                System.Console.WriteLine($"{Problem} cannot be applied to operand of type '{BadToken}'");
            }
            else if(ProblemType == "Incorrect Binary Expression")
            {
                System.Console.WriteLine($"{Problem} cannot be used between '{LeftToken}' and '{RightToken}'");
            }
            else if(ProblemType == "DuplicateArgument")
            {
                Console.WriteLine($"The parameter name '{BadToken}' is a duplicate");
            }
            else if(ProblemType == "StackOverflow")
            {
                System.Console.WriteLine($"Stack OverFlow Function {BadToken}.");
            }
            else if(ProblemType == "ArgumentTypeError")
            {
                System.Console.WriteLine($"{Problem} receives `{ExpectedToken}`, not `{BadToken}`.");
            }
            
            Console.ForegroundColor = ConsoleColor.Green;
        }
        
    }

    /*
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
           Console.WriteLine($"{operatorProblem} cannot be applied to operand of type '{badToken}'");
        }
    }

    class IncorrectBinaryExpression : SemanticError
    {
        public string operatorProblem ;
        public string leftToken ;
        public string rightToken ;

        public IncorrectBinaryExpression(string operatorProblem , string leftToken , string rightToken )
        {
            ProblemType = "Incorrect Binary Expression" ;
            this.leftToken = leftToken ;
            this.rightToken = rightToken ;
            this.operatorProblem = operatorProblem ;
        }

        public override void PrintError()
        {
            Console.WriteLine($"{operatorProblem} cannot be used between '{leftToken}' and '{rightToken}'");
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
            Console.WriteLine($"The parameter name '{badToken}' is a duplicate");
        }

    }

    class ArgumentTypeError : SemanticError
    {
        string functionName ;
        string expectedToken ;
        string badToken ;

        public ArgumentTypeError(string functionName , string expectedToken , string badToken)
        {
            ProblemType = "ArgumentTypeError" ;
            this.functionName = functionName ;
            this.expectedToken = expectedToken ;
            this.badToken = badToken ;
        }

        public override void PrintError()
        {
            Console.WriteLine($"Function '{functionName}' receives '{expectedToken}', not `{badToken}`.");
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
            Console.WriteLine($"Function '{functionName}' receives {argumentsIdCount} argument(s), but {argumentsValueCount} were given.");
        }
    }
    */
    class FunctionsErrors : HulkErrors 
    {
        public string functionName ;
        public string ProblemType ;
        public int? ArgumentsIdCount ;
        public int? ArgumentsValueCount ;
        public string? BadToken ;
        public string? expectedToken ;
        public FunctionsErrors(string functionName , string ProblemType)
        {
            this.functionName = functionName ;
            this.ProblemType = ProblemType ;
        }

        public FunctionsErrors(string functionName , string ProblemType , int ArgumentsIdCount , int ArgumentsValueCount)
        {
            this.functionName = functionName ;
            this.ProblemType = ProblemType ;
            this.ArgumentsIdCount = ArgumentsIdCount;
            this.ArgumentsValueCount = ArgumentsValueCount ;
        }

        public FunctionsErrors(string functionName , string ProblemType ,  string expectedToken , string BadToken )
        {
            this.functionName = functionName ;
            this.ProblemType = ProblemType ;
            this.expectedToken = expectedToken ;
            this.BadToken = BadToken ;
        }

        public override void PrintError()
        {   
            Console.ForegroundColor = ConsoleColor.Red;
            if(ProblemType == "StackOverflow")
            { 
               Console.WriteLine("Stack Overflow " + functionName);
            }
            else if(ProblemType == "ArgumentsCountError") 
            {
                System.Console.WriteLine($"Function '{functionName}' receives {ArgumentsIdCount} argument(s), but {ArgumentsValueCount} were given.");
            }
            else if(ProblemType == "ArgumentTypeError")
            {
                System.Console.WriteLine($"Function '{functionName}' receives '{expectedToken}', not `{BadToken}`.");
            }
            else if(ProblemType == "DuplicateArgument")
            {
                Console.WriteLine($"The parameter name '{BadToken}' is a duplicate");
            }
            Console.ForegroundColor = ConsoleColor.Green;
        }
    }



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
