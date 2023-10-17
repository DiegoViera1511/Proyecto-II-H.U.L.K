using System;
using System.Data.Common;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

namespace HULK
{
    class BooleanOperator : Binary_Exrpessions //Logic boolean operators
    {
        List<string> NextTokens = new List<string>(){")",";",",","in","else","@"};//Siguientes
        public BooleanOperator()
        {
            this.left = new Comparison();

            this.right = new Comparison();
        }

        public override object Operation(object left , string operatorToken , object right)
        {
            if(operatorToken == "&")
            {
                return (bool)left && (bool)right ;
            }
            else // |
            {
                return (bool)left || (bool)right ;
            }
        }
        public override void Analize()
        {
            iDLeft = ActualToken() ;// guardar token que representa a left ej x = 5 

            left.Analize();

            while(Lexer.index < Lexer.Tokens.Count)
            {
                if(ActualToken() == "&" || ActualToken() == "|")
                {
                    string operatorToken = ActualToken();

                    if(left.type == "inference") left.type = "boolean";// si el type es inference asígnale el valor predeterminado del operador

                    Next();

                    iDRight = ActualToken() ;
                    right.Analize();
                    if(right.type == "inference") right.type = "boolean";// si el type es inference asígnale el valor predeterminado del operador

                    
                    if(!(left.type == "boolean" && right.type == "boolean"))
                    {
                        CatchArgumentTypeError(iDLeft , left.type , iDRight , right.type , "boolean");
                        throw new IncorrectBinaryExpression($"Operator ' {operatorToken} '" , left.type , right.type);
                    }
                    else left.type = "boolean" ;
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
                if(ActualToken() == "&" || ActualToken() == "|" )
                {
                    string operatorToken = ActualToken(); 
                    Next();

                    iDRight = ActualToken() ;

                    right.Evaluate();
                    
                    if(Lexer.TokenType(left.value) == "boolean" && Lexer.TokenType(left.value) == "boolean")
                    {
                        left.value = Operation(left.value , operatorToken , right.value) ;
                    }
                    else 
                    {
                        CatchArgumentTypeError(iDLeft , Lexer.TokenType(left.value) , iDRight , Lexer.TokenType(right.value) , "boolean");
        
                        throw new IncorrectBinaryExpression($"Operator ' {operatorToken} '", Lexer.TokenType(left.value) , Lexer.TokenType(right.value));
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
        List<string> NextTokens = new List<string>(){")",";",",","in","else","&","|","@"};
        public Comparison()
        {
            left = new SumExpression();

            right = new SumExpression();
        }

        public override void Analize()
        {
            string operatorToken ;
            iDLeft = ActualToken();

            left.Analize();

            while(Lexer.index < Lexer.Tokens.Count)
            {
                if( ActualToken() == ">" || ActualToken() == "<" || ActualToken() == "<=" || ActualToken() == ">=" )
                {
                    operatorToken = ActualToken();
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
                    else left.type = "boolean" ;
                }
                else if( ActualToken() == "=="  || ActualToken() == "!=" )
                {
                    operatorToken = ActualToken();

                    Next();

                    iDRight = ActualToken();
                    right.Analize();

                    if(left.type == "inference")
                    {
                        left.type = right.type ;
                    }
                    else if(right.type == "inference")
                    {
                        right.type = left.type ;
                    }
                    
                    if(left.type != right.type)
                    {
                        if(Function.functionsId.ContainsKey(iDLeft))
                        {
                            throw new ArgumentTypeError(Lexer.TokenType(right.value) , Lexer.TokenType(left.value));   
                        }
                        else if(Function.functionsId.ContainsKey(iDRight))
                        {
                            throw new ArgumentTypeError(Lexer.TokenType(left.value) , Lexer.TokenType(right.value));
                        }
                        throw new IncorrectBinaryExpression($"Operator ' {operatorToken} '" , left.type , left.type);
                    }
                    else left.type = "boolean" ;
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

        public  override object Operation(object left , string operatorToken , object right)
        {
            if(operatorToken == ">")
            {
                return (double)left > (double)right;
            }
            else if (operatorToken == "<")
            {
                return (double)left < (double)right;
            }
            else if (operatorToken == "<=")
            {
                return (double)left <= (double)right;
            }
            else // >=
            {
                return (double)left >= (double)right;
            }
        }
        public override void Evaluate()
        {   
            string operatorToken ;
            iDLeft = ActualToken();

            left.Evaluate();

            while(Lexer.index < Lexer.Tokens.Count)
            {
                if(  ActualToken() == ">" || ActualToken() == "<" || ActualToken() == "<=" || ActualToken() == ">=" )
                {
                    operatorToken = ActualToken();
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
                        
                        throw new IncorrectBinaryExpression($"Operator ' {operatorToken} '", Lexer.TokenType(left.value) , Lexer.TokenType(right.value));
                    }
                }
                else if( ActualToken() == "==" || ActualToken() == "!=" )
                {
                    operatorToken = ActualToken();
                    Next();

                    iDRight = ActualToken();

                    right.Evaluate();
                    
                    if(Lexer.TokenType(left.value) == Lexer.TokenType(right.value))
                    {
                        if(operatorToken == "==")
                        {
                            left.value = left.value.Equals(right.value);
                        }
                        else //(!=)
                        {
                            left.value = ! left.value.Equals(right.value); 
                        }
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
                        throw new IncorrectBinaryExpression($"Operator ' {operatorToken} '" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value));
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