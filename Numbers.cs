using System.Text.RegularExpressions;

namespace HULK
{
    class SumExpression : Binary_Exrpessions // ( + ; - )
    {   
       private List<string> NextTokens = new List<string>(){";",")","in",",",">","<","else","<","<=",">=","&","|","==","!=","@"};
        
        public SumExpression()
        {
            this.left = new MultiplyExpression();

            this.right = new MultiplyExpression();
        }

        public override object Operation(object left , string operatorToken , object right)
        {
            if(operatorToken == "+")
            {
                return (double)left + (double)right ;
            }
            else // -
            {
                return (double)left - (double)right ;
            }
        }

        public override void Analize()
        {
            iDLeft = ActualToken() ;
            left.Analize();
            
            while(Lexer.index < Lexer.Tokens.Count)
            {

                if(ActualToken() == "+" || ActualToken() == "-")//Cambiar el operador en el error
                {
                    string operatorToken = ActualToken();
                    if(left.type == "inference") left.type = "number";
                    
                    Next();

                    iDRight = ActualToken() ;
                    right.Analize();
                    if(right.type == "inference") right.type = "number";
                
                    if(!(left.type == "number" && right.type == "number"))
                    {
                        CatchArgumentTypeError(iDLeft , left.type , iDRight , right.type , "number");
                        throw new IncorrectBinaryExpression($"Operator ' {operatorToken} '" , left.type , right.type);
                    }
                    else left.type = "number" ;
                }
                else if (NextTokens.Contains(ActualToken()))
                {
                    type = left.type;
                    break;
                }
                else 
                {
                    throw new UnExpectedToken(ActualToken());
                }
            }
        }
        public override void Evaluate()
        {

            iDLeft = ActualToken() ;
            left.Evaluate();
            
            while(Lexer.index < Lexer.Tokens.Count)
            {

                if(ActualToken() == "+" || ActualToken() == "-")
                {
                    string operatorToken = ActualToken();
                    Next();

                    iDRight = ActualToken() ;
                    right.Evaluate();
                
                    if(Lexer.TokenType(left.value) == "number" && Lexer.TokenType(right.value) == "number")
                    {
                        left.value = Operation(left.value , operatorToken , right.value) ;
                    }
                    else 
                    {
                        CatchArgumentTypeError(iDLeft , Lexer.TokenType(left.value) , iDRight , Lexer.TokenType(right.value) , "number");
                        throw new IncorrectBinaryExpression($"Operator ' {operatorToken} '" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value));
                    }
                }
                // else if(ActualToken() == "-")
                // {
                //     Next();

                //     iDRight = ActualToken() ;
                //     right.Evaluate();

                //     if(Lexer.TokenType(left.value) == "number" && Lexer.TokenType(right.value) == "number")
                //     {
                //         left.value = (double)left.value - (double)right.value;
                //     }
                //     else 
                //     {
                //         CatchArgumentTypeError(iDLeft , Lexer.TokenType(left.value) , iDRight , Lexer.TokenType(right.value) , "number");
                //         throw new IncorrectBinaryExpression("Operator ' - '" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value));
                //     }
                // }
                else if (NextTokens.Contains(ActualToken()))
                {
                    value = left.value;
                    break;
                }
                else 
                {
                    throw new UnExpectedToken(ActualToken());
                }
                // else if(ActualToken() == "@")
                // {
                //     Next();

                //     Expression literal = new BooleanOperator();
                //     literal.Evaluate();
                //     string result = $"{left.value}{literal.value}";
                //     left.value = result ;
                // }
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

        public override object Operation(object left , string operatorToken , object right)
        {
            if(operatorToken == "*")
            {
                return (double)left * (double)right ;
            }
            else if(operatorToken == "/")
            {
                return (double)left / (double)right ;
            }
            else // %
            {
                return (double)left % (double)right ;
            }
        }

        public override void Analize()
        {
            iDLeft = ActualToken() ;
            left.Analize();

            while(Lexer.index < Lexer.Tokens.Count)
            {
                if(ActualToken() == "*" || ActualToken() == "/" || ActualToken() == "%")
                {   
                    string operatorToken = ActualToken();

                    if(left.type == "inference") left.type = "number";

                    Next();

                    iDRight = ActualToken() ;
                    right.Analize();
                    if(right.type == "inference") right.type = "number";
                    
                    if(!(left.type == "number" && right.type == "number"))
                    {
                        CatchArgumentTypeError(iDLeft , left.type , iDRight , right.type , "number");
                        throw new IncorrectBinaryExpression($"Operator ' {operatorToken} '" , left.type , right.type);
                    }
                    else left.type = "number" ;
                }
                else if(NextTokens.Contains(ActualToken()))
                {
                    //Siguientes
                    type = left.type ;
                    break;
                }
                else 
                {
                    throw new UnExpectedToken(ActualToken());
                }

            }    
        }
        public override void Evaluate()
        {
            
            iDLeft = ActualToken() ;
            left.Evaluate();

            while(Lexer.index < Lexer.Tokens.Count)
            {

                if(ActualToken() == "*" || ActualToken() == "/" || ActualToken() == "%")
                {   
                    string operatorToken = ActualToken();
                    Next();
                    iDRight = ActualToken() ;
                    right.Evaluate();
                    
                    if(Lexer.TokenType(left.value) == "number" && Lexer.TokenType(right.value) == "number")
                    {
                        left.value = Operation(left.value , operatorToken , right.value);
                    }
                    else 
                    {
                        CatchArgumentTypeError(iDLeft , Lexer.TokenType(left.value) , iDRight , Lexer.TokenType(right.value) , "number");
                        throw new IncorrectBinaryExpression($"Operator ' {operatorToken} '" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value));
                    }
                }
                // else if(ActualToken() == "/")
                // {
                //     Next();
                //     iDRight = ActualToken() ;
                //     right.Evaluate(); 
                    
                //     if(Lexer.TokenType(left.value) == "number" && Lexer.TokenType(right.value) == "number")
                //     {
                //         if((double)right.value == 0)
                //         {
                //             throw new DefaultError("DivisionByZero");
                //         }

                //         left.value = (double)left.value / (double)right.value;
                //     }
                //     else 
                //     {
                //         CatchArgumentTypeError(iDLeft , Lexer.TokenType(left.value) , iDRight , Lexer.TokenType(right.value) , "number");
                //         throw new IncorrectBinaryExpression("Operator ' / '" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value));
                //     }
                // }
                // else if(ActualToken() == "%")
                // {   
                //     Next() ;
                //     iDRight = ActualToken() ;
                //     right.Evaluate(); 

                //     if(Lexer.TokenType(left.value) == "number" && Lexer.TokenType(right.value) == "number")
                //     {
                //         left.value = (double)left.value % (double)right.value;
                //     }
                //     else 
                //     {
                //         CatchArgumentTypeError(iDLeft , Lexer.TokenType(left.value) , iDRight , Lexer.TokenType(right.value) , "number");
                //         throw new IncorrectBinaryExpression("Operator ' % '" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value));
                //     }
                    
                // }
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

        public override object Operation(object left , string operatorToken , object right)
        {
            return Math.Pow((double)left , (double)right);
        }

        public override void Analize()
        {
            iDLeft = ActualToken() ;
            left.Analize();
            
            while(Lexer.index < Lexer.Tokens.Count)
            {
                if(ActualToken() == "^")
                {
                   if(left.type == "inference") left.type = "number";

                    Next();

                    iDRight = ActualToken() ;
                    right.Analize();
                    if(right.type == "inference") right.type = "number";
                    
                    if(!(left.type == "number" && right.type == "number"))
                    {
                        CatchArgumentTypeError(iDLeft , left.type , iDRight , right.type , "number");
                        throw new IncorrectBinaryExpression("Operator ' ^ '" , left.type , right.type);
                    }
                    else left.type = "number";
                }
                else if(NextTokens.Contains(ActualToken()))
                {
                    type = left.type;
                    break;
                }
                else 
                {
                    throw new UnExpectedToken(ActualToken());
                }
            }    
        }
        public override void Evaluate()
        {

            iDLeft = ActualToken() ;
            left.Evaluate();
            
            while(Lexer.index < Lexer.Tokens.Count)
            {
                if(ActualToken() == "^")
                {
                    string operatorToken = ActualToken();
                    Next();
                    iDRight = ActualToken() ;
                    right.Evaluate();
                    
                    if(Lexer.TokenType(left.value) == "number" && Lexer.TokenType(right.value) == "number")
                    {
                        left.value = Operation(left.value , operatorToken , right.value) ;
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