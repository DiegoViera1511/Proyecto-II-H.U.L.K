using System;
using System.Data.Common;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

namespace HULK
{
    class B : Binary_Exrpessions //Logic boolean operators
    {
        List<string> NextTokens = new List<string>(){")",";",",","in","else"};
        public B()
        {
            this.left = new A();

            this.right = new A();
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
            
            left.Evaluate();

            if(left.value == null)
            {
                left.value = "true";
            }

            while(Lexer.index < Lexer.Tokens.Count)
            {
                if(Lexer.Tokens[Lexer.index] == "&")
                {
                    Next();
                    right.Evaluate();
                    if(right.value == null)
                    {
                        right.value = "true";
                    }

                    if(Lexer.TokenType(left.value) == "boolean" && Lexer.TokenType(right.value) == "boolean")
                    {
                        left.value = And(left.value , right.value);
                    }
                    else 
                    {
                        
                       //HULK_Errors.SemanticError("Operator ' & '" , "Incorrect Binary Expression" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value));
                        throw new SemanticError("Operator ' & '" , "Incorrect Binary Expression" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value) , "boolean" , Lexer.GetIncorrectToken(left.value , right.value , "boolean"));
                        //return;
                    }
                    
                }
                else if(Lexer.Tokens[Lexer.index] == "|")
                {
    
                    Next();
                    right.Evaluate();
                    if(right.value == null)
                    {
                        right.value = "true";//Tester
                    }

                    if(Lexer.TokenType(left.value) == "boolean" && Lexer.TokenType(right.value) == "boolean")
                    {
                        left.value = Or(left.value , right.value);
                    }
                    else 
                    {
                        //HULK_Errors.SemanticError("Operator ' | '" , "Incorrect Binary Expression" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value));
                        throw new SemanticError("Operator ' | '" , "Incorrect Binary Expression" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value) , "boolean" , Lexer.GetIncorrectToken(left.value , right.value , "boolean"));
                        //return;
                    }
                    
                }
                else if(NextTokens.Contains(Lexer.Tokens[Lexer.index]))
                {
                    value = left.value;
                    break;
                }
                else 
                {
                    //HULK_Errors.UnExpectedToken(Lexer.Tokens[Lexer.index]);
                    throw new UnExpectedToken(Lexer.Tokens[Lexer.index]);
                    //return;
                }
            }
            
        }

    }
    class A : Binary_Exrpessions //Boolean comparison
    {
        List<string> NextTokens = new List<string>(){")",";",",","in","else","&","|"};
        public A()
        {
            left = new N();

            right = new N();
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
           
            left.Evaluate();

            if(left.value == null)
            {
                left.value = "1";
            }
            while(Lexer.index < Lexer.Tokens.Count)
            {

            
                if(Lexer.Tokens[Lexer.index] == ">")
                {
                    Next();
                    right.Evaluate();
                    if(right.value == null)
                    {
                        right.value = "1";
                    }
                    if(Lexer.TokenType(left.value) == "number" && Lexer.TokenType(right.value) == "number")
                    {
                        left.value = GreaterThan(left.value , right.value);
                    }
                    else 
                    {
                        //HULK_Errors.SemanticError("Operator ' > '" , "Incorrect Binary Expression" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value));
                        throw new SemanticError("Operator '>'" , "Incorrect Binary Expression" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value) , "number" , Lexer.GetIncorrectToken(left.value , right.value , "number"));
                        //return;
                    }
                }
                else if(Lexer.Tokens[Lexer.index] == "<")
                {
                    Next();
                    right.Evaluate();
                    if(right.value == null)
                    {
                        right.value = "1";
                    }
                    if(Lexer.TokenType(left.value) == "number" && Lexer.TokenType(right.value) == "number")
                    {
                        left.value = LessThan(left.value , right.value);
                    }
                    else 
                    {
                        //HULK_Errors.SemanticError("Operator ' < '" , "Incorrect Binary Expression" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value));
                        throw new SemanticError("Operator ' < '" , "Incorrect Binary Expression" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value) , "number" , Lexer.GetIncorrectToken(left.value , right.value , "number"));
                        //return;
                    }
                }
                else if(Lexer.Tokens[Lexer.index] == "<=")
                {
                    Next();
                    right.Evaluate();
                    if(right.value == null)
                    {
                        right.value = "1";
                    }
                    if(Lexer.TokenType(left.value) == "number" && Lexer.TokenType(right.value) == "number")
                    {
                        left.value = LessThanOrEqual(left.value , right.value);
                    }
                    else 
                    {
                        //HULK_Errors.SemanticError("Operator ' <= '" , "Incorrect Binary Expression" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value));
                        throw new SemanticError("Operator ' <= '" , "Incorrect Binary Expression" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value) , "number" , Lexer.GetIncorrectToken(left.value , right.value , "number"));
                        //return;
                    }
                }
                else if(Lexer.Tokens[Lexer.index] == ">=")
                {
                    Next();
                    right.Evaluate();
                    if(right.value == null)
                    {
                        right.value = "1";
                    }
                    if(Lexer.TokenType(left.value) == "number" && Lexer.TokenType(right.value) == "number")
                    {
                        left.value = GreaterThanOrEqual(left.value , right.value);
                    }
                    else 
                    {
                        //HULK_Errors.SemanticError("Operator ' >= '" , "Incorrect Binary Expression" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value));
                        throw new SemanticError("Operator ' >= '" , "Incorrect Binary Expression" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value) , "number" , Lexer.GetIncorrectToken(left.value , right.value , "number"));
                        //return;
                    }
                }
                else if(Lexer.Tokens[Lexer.index] == "==")
                {
                    Next();
                    right.Evaluate();
                    if(right.value == null)
                    {
                        right.value = "1";
                    }
                    if(Lexer.TokenType(left.value) ==  Lexer.TokenType(right.value))
                    {
                        left.value = Equals(left.value , right.value);
                    }
                    else
                    {
                        //HULK_Errors.SemanticError("Operator ' == '" , "Incorrect Binary Expression" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value));
                        throw new SemanticError("Operator ' == '" , "Incorrect Binary Expression" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value) , "number" , Lexer.GetIncorrectToken(left.value , right.value , "number"));
                        //return;
                    }
                    
                }
                else if(Lexer.Tokens[Lexer.index] == "!=")
                {
                    Next();
                    right.Evaluate();
                    if(right.value == null)
                    {
                        right.value = "1";
                    }
                    if(Lexer.TokenType(left.value) ==  Lexer.TokenType(right.value))
                    {
                        left.value = Inequality(left.value , right.value);
                    }
                    else
                    {
                        //HULK_Errors.SemanticError("Operator ' != '" , "Incorrect Binary Expression" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value));
                        throw new SemanticError("Operator ' != '" , "Incorrect Binary Expression" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value) , "number" , Lexer.GetIncorrectToken(left.value , right.value , "number"));
                        //return;
                    }
                    
                }
                else if (NextTokens.Contains(Lexer.Tokens[Lexer.index]))
                {
                    value = left.value ;
                    break;
                }
                else 
                {
                    //HULK_Errors.UnExpectedToken(Lexer.Tokens[Lexer.index]);    
                    throw new UnExpectedToken(Lexer.Tokens[Lexer.index]);
                    //return;
                }
            } 
        }
    }   
}