using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

namespace HULK
{
    /// <summary>
    /// Representa la clase abstracta de errores en HULK
    /// </summary>
    abstract class HulkErrors : Exception 
    {
        public abstract void PrintError();
    }
    /// <summary>
    /// Representa la clase de errores léxicos
    /// </summary>
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
    /// <summary>
    /// Representa la clase de errores sintácticos
    /// </summary>
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

    /// <summary>
    /// Representa el error de tokens inesperados
    /// </summary>
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
    /// <summary>
    /// Representa la clase abstracta de errores semánticos
    /// </summary>
    #region SemanticError 
    abstract class SemanticError : HulkErrors
    {
        public string? ProblemType ;
    }
    /// <summary>
    /// Representa el error de operador unario incorrecto
    /// </summary>
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
            Console.WriteLine($"! SEMANTIC ERROR: {operatorProblem} cannot be applied to operand of type '{badToken}'");
            Console.ForegroundColor = ConsoleColor.Green;
        }
    }
    /// <summary>
    /// Representa el error de operadores binarios 
    /// </summary>
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
            Console.WriteLine($"! SEMANTIC ERROR: {operatorProblem} cannot be used between '{leftTokenType}' and '{rightTokenType}'");
            Console.ForegroundColor = ConsoleColor.Green;
        }
    }
    /// <summary>
    /// Representa el error de argumentos duplicados
    /// </summary>
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
            Console.WriteLine($" ! SEMANTIC ERROR: The parameter name '{badToken}' is a duplicate");
            Console.ForegroundColor = ConsoleColor.Green;
        }

    }
    /// <summary>
    /// Representa el error de tipo de argumento incorrecto
    /// </summary>
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
            Console.WriteLine($"! SEMANTIC ERROR: Function '{functionName}' receives '{expectedToken}', not '{badToken}'.");
            Console.ForegroundColor = ConsoleColor.Green;
        }
    }
    /// <summary>
    /// Representa el error de cantidad de argumentos incorrectos
    /// </summary>
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
            Console.WriteLine($"! SEMANTIC ERROR: Function '{functionName}' receives {argumentsIdCount} argument(s), but {argumentsValueCount} were given.");
            Console.ForegroundColor = ConsoleColor.Green;
        }
    }
    /// <summary>
    /// Representa el error de expresiones condicionales
    /// </summary>
    class ConditionalErrors : SemanticError
    {
         public string badToken ;
        public ConditionalErrors(string badToken)
        {
            ProblemType = "ifArgumentTypeError";
            this.badToken = badToken ;
        }
        public override void PrintError()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"! SEMANTIC ERROR: ' if ' expression receives boolean not {badToken}");
            Console.ForegroundColor = ConsoleColor.Green;
        }
    }

    #endregion
    /// <summary>
    /// Otros errores (Stack overflow , Division by zero , missing ; )
    /// </summary>
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
                Console.WriteLine("! ERROR: Division by constant zero");
            }
            else if (ProblemType == "ErrorFunctionBody")
            {
                Console.WriteLine("! ERROR: Invalid Function Declaration.");
            }
            else if (ProblemType == "StackOverflow")
            {
                Console.WriteLine("! ERROR: Stack Overflow " + functionName);
            }
            else if(ProblemType == "Missing endOfFile")
            {
                Console.WriteLine("Missing ' ; '");
            }
            else if(ProblemType == "Null value")
            {
                Console.WriteLine("Null value");
            }
            Console.ForegroundColor = ConsoleColor.Green;
        }
    }
    
}
