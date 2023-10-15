using System;
using System.Data.Common;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

namespace HULK
{
    class BooleanOperator : Binary_Exrpessions //Logic boolean operators
    {
        List<string> NextTokens = new List<string>(){")",";",",","in","else"};//Siguientes
        public BooleanOperator()
        {
            this.left = new Comparison();

            this.right = new Comparison();
        }
        
        #region Methods
        private static string And(string a , string b)
        {
            bool left = a == "true" ? true : false ;
            bool right = b == "true" ? true : false ;

            if(left && right)
            {
                return "true" ;
            }
            else return "false" ;
        }
        private static string Or(string a , string b)
        {
            bool left = a == "true" ? true : false ;
            bool right = b == "true" ? true : false ;

            if(left || right)
            {
                return "true" ;
            }
            else return "false" ;
        }
        #endregion

        public override void Evaluate()
        {
            
            iDLeft = ActualToken() ;

            left.Evaluate();

            while(Lexer.index < Lexer.Tokens.Count)
            {
                if(ActualToken() == "&")
                {
                    Next();

                    iDRight = ActualToken() ;

                    right.Evaluate();
                    
                    if(Lexer.TokenType(left.value) == "boolean" && Lexer.TokenType(left.value) == "boolean")
                    {
                        left.value = (bool)left.value && (bool)right.value ;
                    }
                    else 
                    {
                        CatchArgumentTypeError(iDLeft , Lexer.TokenType(left.value) , iDRight , Lexer.TokenType(right.value) , "boolean");
        
                        throw new IncorrectBinaryExpression("Operator ' & '" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value));
                    }
                }
                else if( ActualToken() == "|" )
                {
                    Next();
                    iDRight = ActualToken() ;
                    right.Evaluate();
                    
                    if(Lexer.TokenType(left.value) == "boolean" && Lexer.TokenType(left.value) == "boolean")
                    {
                        left.value = (bool)left.value || (bool)right.value;
                    }
                    else 
                    {
                        CatchArgumentTypeError(iDLeft , Lexer.TokenType(left.value) , iDRight , Lexer.TokenType(right.value) , "boolean");
                        
                        throw new IncorrectBinaryExpression("Operator ' | '" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value));
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

    class Comparison : Binary_Exrpessions //Boolean comparison
    {
        List<string> NextTokens = new List<string>(){")",";",",","in","else","&","|"};
        public Comparison()
        {
            left = new SumExpression();

            right = new SumExpression();
        }

        #region Methods
        private static string GreaterThan(string a , string b)
        {
            return double.Parse(a) > double.Parse(b) ? "true" : "false";
        }

        private static string LessThan(string a , string b)
        {
            return double.Parse(a) < double.Parse(b) ? "true" : "false";
        }

        private static string GreaterThanOrEqual(string a , string b)
        {
            return double.Parse(a) >= double.Parse(b) ? "true" : "false";
        }

        private static string LessThanOrEqual(string a , string b)
        {
            return double.Parse(a) <= double.Parse(b) ? "true" : "false";
        }

        private static string Equals(string a , string b)
        {
            return a == b ? "true" : "false";
        }

        private static string Inequality(string a , string b)
        {
            return a != b ? "true" : "false";
        }

        #endregion
        public override void Evaluate()
        {   
            
            iDLeft = ActualToken();

            left.Evaluate();

            while(Lexer.index < Lexer.Tokens.Count)
            {
                if( ActualToken() == ">" )
                {
                    Next();

                    iDRight = ActualToken() ;

                    right.Evaluate();
                    
                    if(Lexer.TokenType(left.value) == "number" && Lexer.TokenType(right.value) == "number")
                    {
                        left.value = (double)left.value > (double)right.value;
                    }
                    else 
                    {
                        CatchArgumentTypeError(iDLeft , Lexer.TokenType(left.value) , iDRight , Lexer.TokenType(right.value) , "number");
                        
                        throw new IncorrectBinaryExpression("Operator ' > '" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value));
                    }
                }
                else if( ActualToken() == "<" )
                {
                    Next();

                   iDRight = ActualToken();

                    right.Evaluate();
                   
                    if(Lexer.TokenType(left.value) == "number" && Lexer.TokenType(right.value) == "number")
                    {
                        left.value = (double)left.value < (double)right.value;
                    }
                    else 
                    {
                        CatchArgumentTypeError(iDLeft , Lexer.TokenType(left.value) , iDRight , Lexer.TokenType(right.value) , "number");
                        
                        throw new IncorrectBinaryExpression("Operator ' < '" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value));
                    }
                }
                else if( ActualToken() == "<=" )
                {
                    Next();

                   iDRight = ActualToken() ;

                    right.Evaluate();
                    
                    if(Lexer.TokenType(left.value) == "number" && Lexer.TokenType(right.value) == "number")
                    {
                        left.value = (double)left.value <= (double)right.value;
                    }
                    else 
                    {
                        CatchArgumentTypeError(iDLeft , Lexer.TokenType(left.value) , iDRight , Lexer.TokenType(right.value) , "number");
                        //Si no hay problema de argumento lanza el error Binary expression
                        throw new IncorrectBinaryExpression("Operator ' <= '" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value));
                    }
                }
                else if( ActualToken() == ">=" )
                {
                    Next();

                    iDRight = ActualToken() ;

                    right.Evaluate();
                    
                    if(Lexer.TokenType(left.value) == "number" && Lexer.TokenType(right.value) == "number")
                    {
                        left.value = (double)left.value >= (double)right.value;
                    }
                    else 
                    {
                        CatchArgumentTypeError(iDLeft , Lexer.TokenType(left.value) , iDRight , Lexer.TokenType(right.value) , "number");
                        
                        throw new IncorrectBinaryExpression("Operator ' >= '" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value));
                    }
                }
                else if( ActualToken() == "==" )
                {
                    Next();

                    iDRight = ActualToken();

                    right.Evaluate();
                    
                    if(Lexer.TokenType(left.value) == Lexer.TokenType(right.value))
                    {
                        left.value = left.value.Equals(right.value);
                    }
                    else
                    {
                        if(Function.functionsId.ContainsKey(iDLeft))
                        {
                            if(Lexer.TokenType(left.value) != Lexer.TokenType(right.value))
                            {
                                throw new ArgumentTypeError(Lexer.TokenType(right.value) , Lexer.TokenType(left.value));
                            }
                        }
                        else if(Function.functionsId.ContainsKey(iDRight))
                        {
                            if(Lexer.TokenType(right.value) != Lexer.TokenType(left.value))
                            {
                                throw new ArgumentTypeError(Lexer.TokenType(left.value) , Lexer.TokenType(right.value));
                            }
                        }
                        throw new IncorrectBinaryExpression("Operator ' == '" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value));
                    }
                }
                else if( ActualToken() == "!=" )
                {
                    Next();

                    iDRight = ActualToken() ;

                    right.Evaluate();
                    
                    if(Lexer.TokenType(left.value) == Lexer.TokenType(right.value))
                    {
                        left.value = ! left.value.Equals(right.value); 
                    }
                    else
                    {
                        if(Function.functionsId.ContainsKey(iDLeft))
                        {
                            if(Lexer.TokenType(left.value) != Lexer.TokenType(right.value))
                            {
                                throw new ArgumentTypeError(Lexer.TokenType(right.value) , Lexer.TokenType(left.value));
                            }
                        }
                        else if(Function.functionsId.ContainsKey(iDRight))
                        {
                            if(Lexer.TokenType(right.value) != Lexer.TokenType(left.value))
                            {
                                throw new ArgumentTypeError(Lexer.TokenType(left.value) , Lexer.TokenType(right.value));
                            }
                        }
                        throw new IncorrectBinaryExpression("Operator ' != '" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value));
                    }
                }
                else if (NextTokens.Contains(ActualToken()))
                {
                    value = left.value ;
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