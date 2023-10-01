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
            if(Lexer.IsID(ActualToken()) && IsFunctionID(ActualToken())) iDLeft = true ;

            left.Evaluate();

            while(Lexer.index < Lexer.Tokens.Count)
            {
                if(ActualToken() == "&")
                {
                    Next();

                    if(Lexer.IsID( ActualToken() ) && IsFunctionID( ActualToken() )) iDRight = true ;

                    right.Evaluate();
                    
                    if(Lexer.TokenType(left.value) == "boolean" && Lexer.TokenType(right.value) == "boolean")
                    {
                        left.value = And(left.value , right.value);
                    }
                    else 
                    {
                        if(iDLeft)
                        {
                            if(Lexer.TokenType(left.value) != "boolean")
                            {
                                throw new SemanticError("Operator ' & '" , "ArgumentTypeError" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value) , "boolean" , Lexer.TokenType(left.value) );
                            }
                        }
                        else if(iDRight)
                        {
                            if(Lexer.TokenType(right.value) != "boolean")
                            {
                                throw new SemanticError("Operator ' & '" , "ArgumentTypeError" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value) , "boolean" , Lexer.TokenType(right.value) );
                            }
                        }
                        throw new SemanticError("Operator ' & '" , "Incorrect Binary Expression" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value) , "boolean" , Lexer.GetIncorrectToken(left.value , right.value , "boolean"));
                    }
                }
                else if( ActualToken() == "|" )
                {
                    Next();
                    if(Lexer.IsID(ActualToken()) && IsFunctionID(ActualToken())) iDRight = true ;
                    right.Evaluate();
                    
                    if(Lexer.TokenType(left.value) == "boolean" && Lexer.TokenType(right.value) == "boolean")
                    {
                        left.value = Or(left.value , right.value);
                    }
                    else 
                    {
                        if(iDLeft)
                        {
                            if(Lexer.TokenType(left.value) != "boolean")
                            {
                                throw new SemanticError("Operator ' | '" , "ArgumentTypeError" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value) , "boolean" , Lexer.TokenType(left.value) );
                            }
                        }
                        else if(iDRight)
                        {
                            if(Lexer.TokenType(right.value) != "boolean")
                            {
                                throw new SemanticError("Operator ' | '" , "ArgumentTypeError" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value) , "boolean" , Lexer.TokenType(right.value) );
                            }
                        }
                        throw new SemanticError("Operator ' | '" , "Incorrect Binary Expression" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value) , "boolean" , Lexer.GetIncorrectToken(left.value , right.value , "boolean"));
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
            if(Lexer.IsID( ActualToken() ) && IsFunctionID( ActualToken() )) iDLeft = true ;

            left.Evaluate();

            while(Lexer.index < Lexer.Tokens.Count)
            {
                if( ActualToken() == ">" )
                {
                    Next();

                    if(Lexer.IsID( ActualToken() ) && IsFunctionID( ActualToken() )) iDRight = true ;

                    right.Evaluate();
                    
                    if(Lexer.TokenType(left.value) == "number" && Lexer.TokenType(right.value) == "number")
                    {
                        left.value = GreaterThan(left.value , right.value);
                    }
                    else 
                    {
                        if(iDLeft)
                        {
                            if(Lexer.TokenType(left.value) != "number")
                            {
                                throw new SemanticError("Operator ' > '" , "ArgumentTypeError" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value) , "number" , Lexer.TokenType(left.value) );
                            }
                        }
                        else if(iDRight)
                        {
                            if(Lexer.TokenType(right.value) != "number")
                            {
                                throw new SemanticError("Operator ' > '" , "ArgumentTypeError" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value) , "number" , Lexer.TokenType(right.value) );
                            }
                        }
                        throw new SemanticError("Operator ' > '" , "Incorrect Binary Expression" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value) , "number" , Lexer.GetIncorrectToken(left.value , right.value , "number"));
                        
                    }
                }
                else if( ActualToken() == "<" )
                {
                    Next();

                    if(Lexer.IsID(ActualToken()) && IsFunctionID(ActualToken())) iDRight = true ;

                    right.Evaluate();
                   
                    if(Lexer.TokenType(left.value) == "number" && Lexer.TokenType(right.value) == "number")
                    {
                        left.value = LessThan(left.value , right.value);
                    }
                    else 
                    {
                        if(iDLeft)
                        {
                            if(Lexer.TokenType(left.value) != "number")
                            {
                                throw new SemanticError("Operator ' < '" , "ArgumentTypeError" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value) , "number" , Lexer.TokenType(left.value) );
                            }
                        }
                        else if(iDRight)
                        {
                            if(Lexer.TokenType(right.value) != "number")
                            {
                                throw new SemanticError("Operator ' < '" , "ArgumentTypeError" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value) , "number" , Lexer.TokenType(right.value) );
                            }
                        }
                        throw new SemanticError("Operator ' < '" , "Incorrect Binary Expression" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value) , "number" , Lexer.GetIncorrectToken(left.value , right.value , "number"));
                    }
                }
                else if( ActualToken() == "<=" )
                {
                    Next();

                    if(Lexer.IsID( ActualToken() ) && IsFunctionID( ActualToken() )) iDRight = true ;

                    right.Evaluate();
                    
                    if(Lexer.TokenType(left.value) == "number" && Lexer.TokenType(right.value) == "number")
                    {
                        left.value = LessThanOrEqual(left.value , right.value);
                    }
                    else 
                    {
                        if(iDLeft)
                        {
                            if(Lexer.TokenType(left.value) != "number")
                            {
                                throw new SemanticError("Operator ' <= '" , "ArgumentTypeError" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value) , "number" , Lexer.TokenType(left.value) );
                            }
                        }
                        else if(iDRight)
                        {
                            if(Lexer.TokenType(right.value) != "number")
                            {
                                throw new SemanticError("Operator ' <= '" , "ArgumentTypeError" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value) , "number" , Lexer.TokenType(right.value) );
                            }
                        }
                        throw new SemanticError("Operator ' <= '" , "Incorrect Binary Expression" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value) , "number" , Lexer.GetIncorrectToken(left.value , right.value , "number"));
                    }
                }
                else if( ActualToken() == ">=" )
                {
                    Next();

                    if(Lexer.IsID( ActualToken() ) && IsFunctionID( ActualToken() )) iDRight = true ;

                    right.Evaluate();
                    
                    if(Lexer.TokenType(left.value) == "number" && Lexer.TokenType(right.value) == "number")
                    {
                        left.value = GreaterThanOrEqual(left.value , right.value);
                    }
                    else 
                    {
                        if(iDLeft)
                        {
                            if(Lexer.TokenType(left.value) != "number")
                            {
                                throw new SemanticError("Operator ' >= '" , "ArgumentTypeError" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value) , "number" , Lexer.TokenType(left.value) );
                            }
                        }
                        else if(iDRight)
                        {
                            if(Lexer.TokenType(right.value) != "number")
                            {
                                throw new SemanticError("Operator ' >= '" , "ArgumentTypeError" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value) , "number" , Lexer.TokenType(right.value) );
                            }
                        }
                        throw new SemanticError("Operator ' >= '" , "Incorrect Binary Expression" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value) , "number" , Lexer.GetIncorrectToken(left.value , right.value , "number"));
                    }
                }
                else if( ActualToken() == "==" )
                {
                    Next();

                    if(Lexer.IsID(ActualToken()) && IsFunctionID(ActualToken())) iDRight = true ;

                    right.Evaluate();
                    
                    if(Lexer.TokenType(left.value) == Lexer.TokenType(right.value))
                    {
                        left.value = Equals(left.value , right.value);
                    }
                    else
                    {
                        throw new SemanticError("Operator ' == '" , "Incorrect Binary Expression" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value) , "number" , Lexer.GetIncorrectToken(left.value , right.value , "number"));
                    }
                }
                else if( ActualToken() == "!=" )
                {
                    Next();

                    if(Lexer.IsID(ActualToken()) && IsFunctionID(ActualToken())) iDRight = true ;

                    right.Evaluate();
                    
                    if(Lexer.TokenType(left.value) == Lexer.TokenType(right.value))
                    {
                        left.value = Inequality(left.value , right.value);
                    }
                    else
                    {
                        throw new SemanticError("Operator ' != '" , "Incorrect Binary Expression" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value) , "number" , Lexer.GetIncorrectToken(left.value , right.value , "number"));
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