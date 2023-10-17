using System.Data.SqlTypes;
using System.Text.RegularExpressions ;
namespace HULK
{
    class MathExpressions : Expression
    {
        public string mathExp ;
        public static List<string> MathFunctions = new List<string>(){ "sin" , "cos" , "sqrt" , "rand" , "exp" , "log" , "PI" , "E"};
        List<object> arguments = new List<object>();
        public MathExpressions(string mathExp)
        {
            this.mathExp = mathExp;
        }

        public override void Analize()
        {
            if(mathExp == "PI" || mathExp == "E")
            {
                type = "number" ;
            }

            if(ActualToken() == "(")
            {   
                Next();

                int countOfArguments = 0 ;
                while(Lexer.index < Lexer.Tokens.Count && ActualToken() != ")")
                {
                    Expression e = new Union();
                    e.Analize();
                    if(e.type == "inference") e.type = "number" ;
                    if(e.type != "number")
                    {
                        throw new ArgumentTypeError("number" , e.type , mathExp );
                    }
                    countOfArguments += 1 ;
                    if(ActualToken() == ",")
                    {
                        Next();
                    }
                    else if(ActualToken() == ")")
                    {
                        break ;
                    }
                    else throw new SyntaxError("Missing ' , '" , "Missing Token" , "Math function" , Lexer.Tokens[Lexer.index - 1]);
                }
                if(ActualToken() == ")")
                {
                    Next();
                    if(mathExp == "sqrt" || mathExp == "sin" || mathExp == "cos" || mathExp == "exp")
                    {
                        if(countOfArguments != 1)
                        {
                            throw new ArgumentsCountError(mathExp , 1 , countOfArguments );
                        }
                    }
                    else if ( mathExp == "log")
                    {
                        if(countOfArguments != 2)
                        {
                            throw new ArgumentsCountError(mathExp , 2 , countOfArguments );
                        }
                    }
                    else if( mathExp == "rand")
                    {
                        if(countOfArguments != 0)
                        {
                            throw new ArgumentsCountError(mathExp , 0 , countOfArguments );
                        }
                    }
                }
                type = "number" ;
            }
        }
        public override void Evaluate()
        {
            if(mathExp == "PI")
            {
               value = Math.PI;
               return ;
            }
            else if (mathExp == "E")
            {
                value = Math.E;
                return ;
            }
            if(ActualToken() == "(")
            {   
                Next();

                while(Lexer.index < Lexer.Tokens.Count && ActualToken() != ")")
                {
                    Expression e = new Union();
                    e.Evaluate();
                    arguments.Add(e.value);
                    if(ActualToken() == ",")
                    {
                        Next();
                    }
                    else if(ActualToken() == ")")
                    {
                        break ;
                    }
                    else throw new SyntaxError("Missing ' , '" , "Missing Token" , "Math function" , Lexer.Tokens[Lexer.index - 1]);
                }
                if(ActualToken() == ")")
                {
                    if(mathExp == "sqrt")
                    {
                        sqrt();
                    }
                    else if(mathExp == "sin")
                    {
                        sin();
                    }
                    else if(mathExp == "cos")
                    {
                        cos();
                    }
                    else if(mathExp == "exp")
                    {
                        exp();
                    }
                    else if(mathExp == "log")
                    {
                        log();
                    }
                    else if(mathExp == "rand")
                    {
                        rand();
                    }

                    Next();
                }
                else throw new SyntaxError("Missing ' ) '" , "Missing Token" , "Math function" , Lexer.Tokens[Lexer.index-1]);
            }
            else throw new SyntaxError("Missing ' ( '" , "Missing Token" , "Math function" , Lexer.Tokens[Lexer.index-1]);
        }

        #region Methods
        public void sqrt()
        {
            if(arguments.Count == 1)
            {
                if(Lexer.TokenType(arguments[0]) == "number")
                {
                    double result = Math.Sqrt((double)arguments[0]);
                    value = result;
                }
                else throw new ArgumentTypeError("number" , Lexer.TokenType(arguments[0]) , "sqrt" );
            }
            else throw new ArgumentsCountError("sqrt" , 1 , arguments.Count );
        }
        public void sin()
        {
            if(arguments.Count == 1)
            {
                if(Lexer.TokenType(arguments[0]) == "number")
                {
                    double result = Math.Sin((double)arguments[0]);
                    value = result;
                }
                else throw new ArgumentTypeError("number" , Lexer.TokenType(arguments[0]) , "sin" );
            }
            else throw new ArgumentsCountError("sin" , 1 , arguments.Count );
        }
        public void cos()
        {
           if(arguments.Count == 1)
            {
                if(Lexer.TokenType(arguments[0]) == "number")
                {
                    double result = Math.Cos((double)arguments[0]);
                    value = result;
                }
                else throw new ArgumentTypeError("number" , Lexer.TokenType(arguments[0]) , "cos" );
            }
            else throw new ArgumentsCountError("cos"  , 1 , arguments.Count );

        }
        public void exp()
        {
            if(arguments.Count == 1)
            {
                if(Lexer.TokenType(arguments[0]) == "number")
                {
                    double result = Math.Cos((double)arguments[0]);
                    value = Convert.ToString(result);
                }
                else throw new ArgumentTypeError("number" , Lexer.TokenType(arguments[0]) , "exp" );
            }
            else throw new ArgumentsCountError("exp" , 1 , arguments.Count );
        }
        public void log()
        {
            if(arguments.Count == 2)
            {
                if(Lexer.TokenType(arguments[0]) == "number")
                {
                    double logBase = (double)arguments[0];
                    if(Lexer.TokenType(arguments[1]) == "number")
                    {
                        double n = (double)arguments[1];
                        double result = Math.Log(n , logBase);
                        value = result ;
                    }
                    else throw new ArgumentTypeError("number" , Lexer.TokenType(arguments[1]) , "log" );
                }
                else throw new ArgumentTypeError("number" , Lexer.TokenType(arguments[0]) , "log");
            }
            else throw new ArgumentsCountError( "log" , 2 , arguments.Count );
        }
        public void rand()
        {
            if(arguments.Count == 0)
            {
                Random r = new Random() ;
                double result = r.NextDouble();
                value = result;
            }
            else throw new ArgumentsCountError("rand" , 0 , arguments.Count );
        }
        
        #endregion 
        
    }
}