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
            if( IsFunctionID(ActualToken()) ) iDLeft = true ;

            left.Evaluate();

            while(Lexer.index < Lexer.Tokens.Count)
            {
                if(ActualToken() == "&")
                {
                    Next();

                    if( IsFunctionID( ActualToken() ) ) iDRight = true ;

                    right.Evaluate();
                    
                    if(Lexer.IsBoolean(left.value) && Lexer.IsBoolean(right.value))
                    {
                        left.value = And(left.value , right.value);
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
                    if( IsFunctionID(ActualToken()) ) iDRight = true ;
                    right.Evaluate();
                    
                    if(Lexer.IsBoolean(left.value) && Lexer.IsBoolean(right.value))
                    {
                        left.value = Or(left.value , right.value);
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
            if( IsFunctionID( ActualToken() ) ) iDLeft = true ;

            left.Evaluate();

            while(Lexer.index < Lexer.Tokens.Count)
            {
                if( ActualToken() == ">" )
                {
                    Next();

                    if( IsFunctionID( ActualToken() ) ) iDRight = true ;

                    right.Evaluate();
                    
                    if(Lexer.IsNumber(left.value) && Lexer.IsNumber(right.value))
                    {
                        left.value = GreaterThan(left.value , right.value);
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

                    if( IsFunctionID(ActualToken()) ) iDRight = true ;

                    right.Evaluate();
                   
                    if(Lexer.IsNumber(left.value) && Lexer.IsNumber(right.value))
                    {
                        left.value = LessThan(left.value , right.value);
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

                    if( IsFunctionID( ActualToken() ) ) iDRight = true ;

                    right.Evaluate();
                    
                    if(Lexer.IsNumber(left.value) && Lexer.IsNumber(right.value))
                    {
                        left.value = LessThanOrEqual(left.value , right.value);
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

                    if( IsFunctionID( ActualToken() ) ) iDRight = true ;

                    right.Evaluate();
                    
                    if(Lexer.IsNumber(left.value) && Lexer.IsNumber(right.value))
                    {
                        left.value = GreaterThanOrEqual(left.value , right.value);
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

                    if( IsFunctionID(ActualToken()) ) iDRight = true ;

                    right.Evaluate();
                    
                    if(Lexer.TokenType(left.value) == Lexer.TokenType(right.value))
                    {
                        left.value = Equals(left.value , right.value);
                    }
                    else
                    {
                        if(iDLeft)
                        {
                            if(Lexer.TokenType(left.value) != Lexer.TokenType(right.value))
                            {
                                throw new ArgumentTypeError(Lexer.TokenType(right.value) , Lexer.TokenType(left.value));
                            }
                        }
                        else if(iDRight)
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

                    if( IsFunctionID(ActualToken()) ) iDRight = true ;

                    right.Evaluate();
                    
                    if(Lexer.TokenType(left.value) == Lexer.TokenType(right.value))
                    {
                        left.value = Inequality(left.value , right.value);
                    }
                    else
                    {
                        if(iDLeft)
                        {
                            if(Lexer.TokenType(left.value) != Lexer.TokenType(right.value))
                            {
                                throw new ArgumentTypeError(Lexer.TokenType(right.value) , Lexer.TokenType(left.value));
                            }
                        }
                        else if(iDRight)
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