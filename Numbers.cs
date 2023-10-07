using System.Text.RegularExpressions;

namespace HULK
{
    class SumExpression : Binary_Exrpessions // ( + ; - ; @ )
    {   
       private List<string> NextTokens = new List<string>(){";", ")" ,"in",",",">","<","else","<","<=",">=","&","|","==","!="};
        
        public SumExpression()
        {
            this.left = new MultiplyExpression();

            this.right = new MultiplyExpression();
        }
        

        #region Methods
        private static string Sum( string a , string b)
        {
            double result = double.Parse(a) + double.Parse(b);

            return Convert.ToString(result);  
        }
        private static string Subtract( string a , string b)
        {
            double result = double.Parse(a) - double.Parse(b);

            return Convert.ToString(result);  
        }

        #endregion

        public override void Evaluate()
        {
            
            if(Lexer.IsID(ActualToken()) && IsFunctionID(ActualToken())) iDLeft = true ;
            left.Evaluate();
            
            while(Lexer.index < Lexer.Tokens.Count)
            {

                if(ActualToken() == "+")
                {
                    
                    Next();

                    if(Lexer.IsID(ActualToken()) && IsFunctionID(ActualToken())) iDRight = true ;
                    right.Evaluate();

                    if(Lexer.TokenType(left.value) == "number" && Lexer.TokenType(right.value) == "number")
                    {
                        left.value = Sum(left.value , right.value);
                    }
                    else 
                    {
                        CatchArgumentTypeError(iDLeft , Lexer.TokenType(left.value) , iDRight , Lexer.TokenType(right.value) , "number");
                        throw new IncorrectBinaryExpression("Operator ' + '" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value));
                    }
                }
                else if(ActualToken() == "-")
                {
                    Next();

                    if(Lexer.IsID(ActualToken()) && IsFunctionID(ActualToken())) iDRight = true ;
                    right.Evaluate();

                    if(Lexer.TokenType(left.value) == "number" && Lexer.TokenType(right.value) == "number")
                    {
                        left.value = Subtract(left.value , right.value);
                    }
                    else 
                    {
                        CatchArgumentTypeError(iDLeft , Lexer.TokenType(left.value) , iDRight , Lexer.TokenType(right.value) , "number");
                        throw new IncorrectBinaryExpression("Operator ' - '" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value));
                    }
                }
                else if(ActualToken() == "@")
                {
                    Next();

                    if(Lexer.IsString(left.value))
                    {
                        left.value = left.value.Substring( 0 , left.value.Length - 1);
                    }
                    Expression literal = new BooleanOperator();
                    literal.Evaluate();
                    if(Lexer.IsString(literal.value))
                    {
                        literal.value = literal.value.Substring( 1 , literal.value.Length - 1 );
                    }

                    value = left.value + Convert.ToString(literal.value);
                    return ;
                    
                }
                else if (NextTokens.Contains(ActualToken()))
                {
                    value = Convert.ToString(left.value);
                    break;
                }
                else 
                {
                    throw new UnExpectedToken(ActualToken());
                }
            }
        }
    }

    class MultiplyExpression : Binary_Exrpessions  // ( * ; / ; % )
    {
        private List<string> NextTokens = new List<string>(){";", ")" ,"in",",",">","<","else","<","<=",">=","&","|","==","!=","@","+","-"};
        public MultiplyExpression()
        {
            this.left = new PowerExpression();

            this.right = new PowerExpression();
        }

        #region Methods
        private static string Multiply( string a , string b)
        {
            double result = double.Parse(a) * double.Parse(b);

            return Convert.ToString(result);  
        }
        private static string Division( string a , string b)
        {
            double result = double.Parse(a) / double.Parse(b);

            return Convert.ToString(result);  
        }
        private static string Modulo( string a , string b )
        {
            double result = double.Parse(a) % double.Parse(b);

            return Convert.ToString(result);  
        }

        #endregion
        public override void Evaluate()
        {
            if(Lexer.IsID(ActualToken()) && IsFunctionID(ActualToken())) iDLeft = true ;
            left.Evaluate();

            while(Lexer.index < Lexer.Tokens.Count)
            {

                if(ActualToken() == "*")
                {   
                    Next();
                    if(Lexer.IsID(ActualToken()) && IsFunctionID(ActualToken())) iDRight = true ;
                    right.Evaluate();
                    
                    if(Lexer.TokenType(left.value) == "number" && Lexer.TokenType(right.value) == "number")
                    {
                        left.value = Multiply(left.value , right.value);
                    }
                    else 
                    {
                        CatchArgumentTypeError(iDLeft , Lexer.TokenType(left.value) , iDRight , Lexer.TokenType(right.value) , "number");
                        throw new IncorrectBinaryExpression("Operator ' * '" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value));
                    }
                }
                else if(ActualToken() == "/")
                {
                    Next();
                    if(Lexer.IsID(ActualToken()) && IsFunctionID(ActualToken())) iDRight = true ;
                    right.Evaluate(); 
                    
                    if(right.value == "0")
                    {
                        throw new DefaultError("DivisionByZero");
                    }

                    if(Lexer.TokenType(left.value) == "number" && Lexer.TokenType(right.value) == "number")
                    {
                        left.value = Division(left.value , right.value);
                    }
                    else 
                    {
                        CatchArgumentTypeError(iDLeft , Lexer.TokenType(left.value) , iDRight , Lexer.TokenType(right.value) , "number");
                        throw new IncorrectBinaryExpression("Operator ' / '" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value));
                    }
                }
                else if(ActualToken() == "%")
                {   
                    Next();
                    if(Lexer.IsID(ActualToken()) && IsFunctionID(ActualToken())) iDRight = true ;
                    right.Evaluate(); 

                    if(Lexer.TokenType(left.value) == "number" && Lexer.TokenType(right.value) == "number")
                    {
                        left.value = Modulo(left.value , right.value);
                    }
                    else 
                    {
                        CatchArgumentTypeError(iDLeft , Lexer.TokenType(left.value) , iDRight , Lexer.TokenType(right.value) , "number");
                        throw new IncorrectBinaryExpression("Operator ' % '" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value));
                    }
                    
                }
                else if(NextTokens.Contains(ActualToken()))
                {
                    //Siguientes
                    value = Convert.ToString(left.value);//No hace falta Convert
                    break;
                }
                else 
                {
                    throw new UnExpectedToken(ActualToken());
                }

            }    
        }
    }

    class PowerExpression : Binary_Exrpessions // ( ^ )
    {
        private List<string> NextTokens = new List<string>(){";", ")" ,"in",",",">","<","else","<=",">=","&","|","==","!=","@","+","-","*","/","%"};
        public PowerExpression()
        {
            this.left = new Atom();

            this.right = new Atom();
        }

        private static string Pow( string a , string b)
        {
            double result = Math.Pow(double.Parse(a) , double.Parse(b)) ;
            return Convert.ToString(result);  
        }

        public override void Evaluate()
        {
            if(Lexer.IsID(ActualToken()) && IsFunctionID(ActualToken())) iDLeft = true ;
            left.Evaluate();
            
            while(Lexer.index < Lexer.Tokens.Count)
            {
                if(ActualToken() == "^")
                {
                    Next();
                    if(Lexer.IsID(ActualToken()) && IsFunctionID(ActualToken())) iDRight = true ;
                    right.Evaluate();
                    
                    if(Lexer.TokenType(left.value) == "number" && Lexer.TokenType(right.value) == "number")
                    {
                        left.value = Pow(left.value , right.value);
                    }
                    else 
                    {
                        CatchArgumentTypeError(iDLeft , Lexer.TokenType(left.value) , iDRight , Lexer.TokenType(right.value) , "number");
                        throw new IncorrectBinaryExpression("Operator ' ^ '" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value));
                    }
                }
                else if(NextTokens.Contains(ActualToken()))
                {
                    value =  Convert.ToString(left.value);
                    break;
                }
                else 
                {
                    throw new UnExpectedToken(ActualToken());
                }
            }    
        }
    }
    
}