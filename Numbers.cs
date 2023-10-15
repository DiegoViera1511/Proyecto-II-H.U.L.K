using System.Text.RegularExpressions;

namespace HULK
{
    class SumExpression : Binary_Exrpessions // ( + ; - ; @ )
    {   
       private List<string> NextTokens = new List<string>(){";",")","in",",",">","<","else","<","<=",">=","&","|","==","!="};
        
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
            iDLeft = ActualToken() ;
            left.Evaluate();
            
            while(Lexer.index < Lexer.Tokens.Count)
            {

                if(ActualToken() == "+")
                {
                    
                    Next();

                    iDRight = ActualToken() ;
                    right.Evaluate();

                    if(Lexer.TokenType(left.value) == "number" && Lexer.TokenType(right.value) == "number")
                    {
                        left.value = (double)left.value + (double)right.value;
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

                    iDRight = ActualToken() ;
                    right.Evaluate();

                    if(Lexer.TokenType(left.value) == "number" && Lexer.TokenType(right.value) == "number")
                    {
                        left.value = (double)left.value - (double)right.value;
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

                    Expression literal = new BooleanOperator();
                    literal.Evaluate();
                    string result = $"{left.value}{literal.value}";
                    left.value = result ;
                }
                else if (NextTokens.Contains(ActualToken()))
                {
                    value = left.value;
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
            
            iDLeft = ActualToken() ;
            left.Evaluate();

            while(Lexer.index < Lexer.Tokens.Count)
            {

                if(ActualToken() == "*")
                {   
                    Next();
                    iDRight = ActualToken() ;
                    right.Evaluate();
                    
                    if(Lexer.TokenType(left.value) == "number" && Lexer.TokenType(right.value) == "number")
                    {
                        left.value = (double)left.value * (double)right.value;
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
                    iDRight = ActualToken() ;
                    right.Evaluate(); 
                    
                    if(Lexer.TokenType(left.value) == "number" && Lexer.TokenType(right.value) == "number")
                    {
                        if((double)right.value == 0)
                        {
                            throw new DefaultError("DivisionByZero");
                        }

                        left.value = (double)left.value / (double)right.value;
                    }
                    else 
                    {
                        CatchArgumentTypeError(iDLeft , Lexer.TokenType(left.value) , iDRight , Lexer.TokenType(right.value) , "number");
                        throw new IncorrectBinaryExpression("Operator ' / '" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value));
                    }
                }
                else if(ActualToken() == "%")
                {   
                    Next() ;
                    iDRight = ActualToken() ;
                    right.Evaluate(); 

                    if(Lexer.TokenType(left.value) == "number" && Lexer.TokenType(right.value) == "number")
                    {
                        left.value = (double)left.value % (double)right.value;
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
                    value = left.value;
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

            iDLeft = ActualToken() ;
            left.Evaluate();
            
            while(Lexer.index < Lexer.Tokens.Count)
            {
                if(ActualToken() == "^")
                {
                    Next();
                    iDRight = ActualToken() ;
                    right.Evaluate();
                    
                    if(Lexer.TokenType(left.value) == "number" && Lexer.TokenType(right.value) == "number")
                    {
                        left.value = Math.Pow((double)left.value , (double)right.value);
                    }
                    else 
                    {
                        CatchArgumentTypeError(iDLeft , Lexer.TokenType(left.value) , iDRight , Lexer.TokenType(right.value) , "number");
                        throw new IncorrectBinaryExpression("Operator ' ^ '" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value));
                    }
                }
                else if(NextTokens.Contains(ActualToken()))
                {
                    value = left.value;
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