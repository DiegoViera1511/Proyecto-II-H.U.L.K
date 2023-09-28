using System.Data.SqlTypes;
using System.Text.RegularExpressions ;
namespace HULK
{
    class MathExpressions : Expression
    {
        public string mathExp ;
        public static List<string> MathFunctions = new List<string>(){ "sin" , "cos" , "sqrt" , "rand" , "exp" , "log" , "PI" , "E"};
        List<string> Arguments = new List<string>();
        public MathExpressions(string mathExp)
        {
            this.mathExp = mathExp;
        }

        public override void Evaluate()
        {
            if(mathExp == "PI")
            {
               value = Convert.ToString(Math.PI);
               return ;
            }
            else if (mathExp == "E")
            {
                value = Convert.ToString(Math.E);
                return ;
            }
            if(Lexer.Tokens[Lexer.index] == "(")
            {   
                Next();

                while(Lexer.index < Lexer.Tokens.Count && Lexer.Tokens[Lexer.index] != ")")
                {
                    Expression e = new B();
                    e.Evaluate();
                    Arguments.Add(e.value);
                    if(Lexer.Tokens[Lexer.index] == ",")
                    {
                        Next();
                    }
                    else if(Lexer.Tokens[Lexer.index] == ")")
                    {
                        break ;
                    }
                    else throw new SyntaxError("Missing ' , '" , "Missing Token" , "Math function" , Lexer.Tokens[Lexer.index - 1]);
                }
                if(Lexer.Tokens[Lexer.index] == ")")
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
            if(Arguments.Count == 1)
            {
                if(Lexer.IsNumber(Arguments[0]))
                {
                    double result = Math.Sqrt(Convert.ToDouble(Arguments[0]));
                    value = Convert.ToString(result);
                }
                else throw new FunctionsErrors("sqrt" , "ArgumentTypeError" , "number" , Lexer.TokenType(Arguments[0]));
            }
            else throw new FunctionsErrors("sqrt" , "ArgumentsCountError" , 1 , Arguments.Count );
        }
        public void sin()
        {
            if(Arguments.Count == 1)
            {
                if(Lexer.IsNumber(Arguments[0]))
                {
                    double result = Math.Sin(Convert.ToDouble(Arguments[0]));
                    value = Convert.ToString(result);
                }
                else throw new FunctionsErrors("sin" , "ArgumentTypeError" , "number" , Lexer.TokenType(Arguments[0]));
            }
            else throw new FunctionsErrors("sin" , "ArgumentsCountError" , 1 , Arguments.Count );
        }
        public void cos()
        {
           if(Arguments.Count == 1)
            {
                if(Lexer.IsNumber(Arguments[0]))
                {
                    double result = Math.Cos(Convert.ToDouble(Arguments[0]));
                    value = Convert.ToString(result);
                }
                else throw new FunctionsErrors("cos" , "ArgumentTypeError" , "number" , Lexer.TokenType(Arguments[0]));
            }
            else throw new FunctionsErrors("cos" , "ArgumentsCountError" , 1 , Arguments.Count );

        }
        public void exp()
        {
            if(Arguments.Count == 1)
            {
                if(Lexer.IsNumber(Arguments[0]))
                {
                    double result = Math.Cos(Convert.ToDouble(Arguments[0]));
                    value = Convert.ToString(result);
                }
                else throw new FunctionsErrors("exp" , "ArgumentTypeError" , "number" , Lexer.TokenType(Arguments[0]));
            }
            else throw new FunctionsErrors("exp" , "ArgumentsCountError" , 1 , Arguments.Count );
        }
        public void log()
        {
            if(Arguments.Count == 2)
            {
                if(Lexer.IsNumber(Arguments[0]))
                {
                    double logBase = Convert.ToDouble(Arguments[0]);
                    if(Lexer.IsNumber(Arguments[1]))
                    {
                        double n = Convert.ToDouble(Arguments[1]);
                        double result = Math.Log(n , logBase);
                        value = Convert.ToString(result) ;
                    }
                    else throw new FunctionsErrors("log" , "ArgumentTypeError" , "number" , Lexer.TokenType(Arguments[0]));
                }
                else throw new FunctionsErrors("log" , "ArgumentTypeError" , "number" , Lexer.TokenType(Arguments[0]));
            }
            else throw new FunctionsErrors("log" , "ArgumentsCountError" , 2 , Arguments.Count );
        }
        public void rand()
        {
            if(Arguments.Count == 0)
            {
                Random r = new Random() ;
                double result = r.NextDouble();
                value = Convert.ToString(result);
            }
            else throw new FunctionsErrors("rand" , "ArgumentsCountError" , 0 , Arguments.Count );

        }
        public void PI()
        {
            value = Convert.ToString(Math.PI) ;
        }

        public void E()
        {
            value = Convert.ToString(Math.E) ;
        }
        #endregion 
        
    }
}