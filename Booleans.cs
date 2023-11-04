using System;
using System.Data.Common;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

namespace HULK
{
    /// <summary>
    /// Representa las expresiones binarias booleanas de & y | .
    /// </summary>
    class BooleanOperator : Binary_Exrpessions //Logic boolean operators
    {
        List<string> NextTokens = new List<string>(){")",";",",","in","else","@"};//Siguientes
        public BooleanOperator()
        {
            this.left = new Comparison();

            this.right = new Comparison();
        }
        /// <summary>
        /// Operación de la clase BooleanOperator
        /// </summary>
        /// <param name="left">Representa el valor de la expresión de la izquierda</param>
        /// <param name="operatorToken">Operador de la expresión binaria ( & , | )</param>
        /// <param name="right">Representa el valor de la expresión de la derecha</param>
        /// <returns>Retorna la operación entre left y right (bool)</returns>
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
        /// <summary>
        /// Analiza la expresión , si hay un token de  ( & , | ) , verifica que left y right sean de tipo boolean y 
        /// define el tipo de la expresión actualizando left hasta que encuentre un siguiente de la expresión .
        /// </summary>
        /// <exception cref="IncorrectBinaryExpression">Lanza la excepción si left o right no son de tipo boolean</exception>
        /// <exception cref="UnExpectedToken">Lanza la execpción si no encuentra un siguiente de la expresión</exception>
        public override void Analize()
        {
            iDLeft = ActualToken() ;// guardar token que representa a left ej x = 5 

            left.Analize();

            while(Lexer.index < Lexer.Tokens.Count)
            {
                if(ActualToken() == "&" || ActualToken() == "|")
                {
                    string operatorToken = ActualToken();

                    if(left.type == InferenceType) left.type = BooleanType;// si el type es inference asígnale el valor predeterminado del operador

                    Next();

                    iDRight = ActualToken() ;
                    right.Analize();
                    if(right.type == InferenceType) right.type = BooleanType;// si el type es inference asígnale el valor predeterminado del operador

                    
                    if(!(left.type == BooleanType && right.type == BooleanType))
                    {
                        CatchArgumentTypeError(iDLeft , left.type , iDRight , right.type , BooleanType);
                        throw new IncorrectBinaryExpression($"Operator ' {operatorToken} '" , left.type , right.type);
                    }
                    else left.type = BooleanType ;
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
        /// <summary>
        /// Evalúa la expresión left , si hay un operador ( & , | ) , Evalúa la expresión right y actulaliza el valor de left
        /// con el valor de la operación entre el valor de left y right.
        /// </summary>
        /// <exception cref="IncorrectBinaryExpression">Lanza la excepción si left o right no son de tipo boolean en tiempo de ejecución</exception>
        /// <exception cref="UnExpectedToken">Lanza la execpción si no encuentra un siguiente de la expresión</exception>
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
                    
                    if(Lexer.TokenType(left.value) == BooleanType && Lexer.TokenType(left.value) == BooleanType)
                    {
                        left.value = Operation(left.value , operatorToken , right.value) ;
                    }
                    else 
                    {
                        CatchArgumentTypeError(iDLeft , Lexer.TokenType(left.value) , iDRight , Lexer.TokenType(right.value) , BooleanType);
        
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
    /// <summary>
    /// Representa las expresiones binarias de ( < , > , <= , >= , == , != )
    /// </summary>
    class Comparison : Binary_Exrpessions //Boolean comparison
    {
        List<string> NextTokens = new List<string>(){")",";",",","in","else","&","|","@"};
        public Comparison()
        {
            left = new SumExpression();

            right = new SumExpression();
        }
        /// <summary>
        /// Analiza la expresión , si hay un token de  ( < , > , <= , >= , == , != ) , verifica que left y right sean 
        /// de tipo number (< , > , <= , >= ) o del mismo tipo ( == , != ) y define el tipo de la expresión actualizando
        /// left hasta que encuentre un siguiente de la expresión .
        /// </summary>
        /// <exception cref="IncorrectBinaryExpression">Lanza la excepción si left o right no son del mismo tipo en tiempo de ejecución</exception>
        /// <exception cref="ArgumentTypeError">Lanza error si left o right no son del tipo correcto y son argumentos de función</exception>
        /// <exception cref="UnExpectedToken">Lanza la execpción si no encuentra un siguiente de la expresión</exception>
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
                    if(left.type == InferenceType) left.type = NumberType;

                    Next();

                    iDRight = ActualToken() ;
                    right.Analize();
                    if(right.type == InferenceType) right.type = NumberType;
                    
                    if(!(left.type == NumberType && right.type == NumberType))
                    {
                        CatchArgumentTypeError(iDLeft , left.type , iDRight , right.type , NumberType);
                        throw new IncorrectBinaryExpression($"Operator ' {operatorToken} '" , left.type , right.type);
                    }
                    else left.type = BooleanType ;
                }
                else if( ActualToken() == "=="  || ActualToken() == "!=" )
                {
                    operatorToken = ActualToken();

                    Next();

                    iDRight = ActualToken();
                    right.Analize();

                    if(left.type == InferenceType)
                    {
                        left.type = right.type ;
                    }
                    else if(right.type == InferenceType)
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
                    else left.type = BooleanType ;
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
        /// <summary>
        /// Operación de la clase Comparison
        /// </summary>
        /// <param name="left">Representa el valor de la expresión de la izquierda</param>
        /// <param name="operatorToken">Operador de la expresión binaria ( < , > , <= , >= , == , != )</param>
        /// <param name="right">Representa el valor de la expresión de la derecha</param>
        /// <returns></returns>
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
        /// <summary>
        /// Evalúa la expresión left , si hay un operador  ( < , > , <= , >= , == , != ) , Evalúa la expresión right y actulaliza el valor de left
        /// con el valor de la operación entre el valor de left y right.
        /// </summary>
        /// <exception cref="IncorrectBinaryExpression">Lanza la excepción si left o right no son del mismo tipo en tiempo de ejecución</exception>
        /// <exception cref="ArgumentTypeError">Lanza error si left o right no son del tipo correcto y son argumentos de función</exception>
        /// <exception cref="UnExpectedToken">Lanza la execpción si no encuentra un siguiente de la expresión</exception>
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
                    
                    if(Lexer.TokenType(left.value) == NumberType && Lexer.TokenType(right.value) == NumberType)
                    {
                        left.value = Operation(left.value , operatorToken , right.value) ;
                    }
                    else 
                    {
                        CatchArgumentTypeError(iDLeft , Lexer.TokenType(left.value) , iDRight , Lexer.TokenType(right.value) , NumberType);
                        
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