using System.Text.RegularExpressions;

namespace HULK
{

    /// <summary>
    /// Representa las expresiones binarias de + y - .
    /// </summary>
    class SumExpression : Binary_Exrpessions // ( + ; - )
    {   
       private List<string> NextTokens = new List<string>(){";",")","in",",",">","<","else","<","<=",">=","&","|","==","!=","@"};
        
        public SumExpression()
        {
            this.left = new MultiplyExpression();

            this.right = new MultiplyExpression();
        }
        /// <summary>
        /// Operación de la clase SumExrpession
        /// </summary>
        /// <param name="left">Representa el valor de la expresión de la izquierda</param>
        /// <param name="operatorToken">Operador de la expresión binaria ( + , - )</param>
        /// <param name="right">Representa el valor de la expresión de la derecha</param>
        /// <returns>Retorna la operación entre left y right (number)</returns>
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
        /// <summary>
        /// Analiza la expresión , si hay un token de + o - , verifica que left y right sean de tipo number y 
        /// define el tipo de la expresión actualizando left hasta que encuentre un siguiente de la expresión ;
        /// </summary>
        /// <exception cref="IncorrectBinaryExpression">Lanza la excepción si left o right no son de tipo number</exception>
        /// <exception cref="UnExpectedToken">Lanza la execpción si no encuentra un siguiente de la expresión</exception>
        public override void Analize()
        {
            iDLeft = ActualToken() ;
            left.Analize();//Analiza la izquierda
            
            while(Lexer.index < Lexer.Tokens.Count)
            {

                if(ActualToken() == "+" || ActualToken() == "-")
                {
                    string operatorToken = ActualToken();
                    if(left.type == "inference") left.type = "number";
                    
                    Next();

                    iDRight = ActualToken() ;
                    right.Analize();//Analiza la derecha
                    if(right.type == "inference") right.type = "number";
                
                    if(!(left.type == "number" && right.type == "number"))//Verifica que sean ambos de tipo number
                    {
                        CatchArgumentTypeError(iDLeft , left.type , iDRight , right.type , "number");//Verifica si son argumentos de una función
                        throw new IncorrectBinaryExpression($"Operator ' {operatorToken} '" , left.type , right.type);//Lanza error
                    }
                    else left.type = "number" ;//Actualiza el tipo de la expresión 
                }
                else if (NextTokens.Contains(ActualToken()))//Verifica que el token sea un siguiente de la expresión
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
        /// Evalúa la expresión left , si hay un operador ( + , -) , Evalúa la expresión right y actulaliza el valor de left
        /// con el valor de la operación entre el valor de left y right.
        /// </summary>
        /// <exception cref="IncorrectBinaryExpression">Lanza la excepción si left o right no son de tipo number en tiempo de ejecución</exception>
        /// <exception cref="UnExpectedToken">Lanza la execpción si no encuentra un siguiente de la expresión</exception>
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
                        left.value = Operation(left.value , operatorToken , right.value) ;//Actualiza el valor de left 
                    }
                    else 
                    {
                        CatchArgumentTypeError(iDLeft , Lexer.TokenType(left.value) , iDRight , Lexer.TokenType(right.value) , "number");
                        throw new IncorrectBinaryExpression($"Operator ' {operatorToken} '" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value));
                    }
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

    /// <summary>
    /// Representa las expresiones binarias de * , / , % .
    /// </summary>
    class MultiplyExpression : Binary_Exrpessions  // ( * ; / ; % )
    {
        private List<string> NextTokens = new List<string>(){";", ")" ,"in",",",">","<","else","<","<=",">=","&","|","==","!=","@","+","-"};
        public MultiplyExpression()
        {
            this.left = new PowerExpression();

            this.right = new PowerExpression();
        }
        /// <summary>
        /// Operación de la clase MultiplyExpressión 
        /// </summary>
        /// <param name="left">Representa el valor de la expresión de la izquierda</param>
        /// <param name="operatorToken">Operador de la expresión binaria ( * , / ,% )</param>
        /// <param name="right">Representa el valor de la expresión de la derecha</param>
        /// <returns>Retorna la operación entre left y right (number)</returns>
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
        /// <summary>
        /// Analiza la expresión , si hay un token de  ( * , / % ) , verifica que left y right sean de tipo number y 
        /// define el tipo de la expresión actualizando left hasta que encuentre un siguiente de la expresión ;
        /// </summary>
        /// <exception cref="IncorrectBinaryExpression">Lanza la excepción si left o right no son de tipo number</exception>
        /// <exception cref="UnExpectedToken">Lanza la execpción si no encuentra un siguiente de la expresión</exception>
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
        /// <summary>
        /// Evalúa la expresión left , si hay un operador ( * , / , % ) , Evalúa la expresión right y actulaliza el valor de left
        /// con el valor de la operación entre el valor de left y right.
        /// </summary>
        /// <exception cref="IncorrectBinaryExpression">Lanza la excepción si left o right no son de tipo number en tiempo de ejecución</exception>
        /// <exception cref="UnExpectedToken">Lanza la execpción si no encuentra un siguiente de la expresión</exception>
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

    /// <summary>
    /// Representa las expresiones binarias de ^ .
    /// </summary>
    class PowerExpression : Binary_Exrpessions // ( ^ )
    {
        private List<string> NextTokens = new List<string>(){";", ")" ,"in",",",">","<","else","<=",">=","&","|","==","!=","@","+","-","*","/","%"};
        public PowerExpression()
        {
            this.left = new Atom();

            this.right = new Atom();
        }
         /// <summary>
        /// Operación de la clase PowerExpression 
        /// </summary>
        /// <param name="left">Representa el valor de la expresión de la izquierda</param>
        /// <param name="operatorToken">Operador de la expresión binaria ( ^ )</param>
        /// <param name="right">Representa el valor de la expresión de la derecha</param>
        /// <returns>Retorna la operación entre left y right (number)/returns>
        public override object Operation(object left , string operatorToken , object right)
        {
            return Math.Pow((double)left , (double)right);
        }
        /// <summary>
        /// Analiza la expresión , si hay un token de  ( ^ ) , verifica que left y right sean de tipo number y 
        /// define el tipo de la expresión actualizando left hasta que encuentre un siguiente de la expresión .
        /// </summary>
        /// <exception cref="IncorrectBinaryExpression">Lanza la excepción si left o right no son de tipo number</exception>
        /// <exception cref="UnExpectedToken">Lanza la execpción si no encuentra un siguiente de la expresión</exception>
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
        /// <summary>
        /// Evalúa la expresión left , si hay un operador ( ^ ) , Evalúa la expresión right y actulaliza el valor de left
        /// con el valor de la operación entre el valor de left y right.
        /// </summary>
        /// <exception cref="IncorrectBinaryExpression">Lanza la excepción si left o right no son de tipo number en tiempo de ejecución</exception>
        /// <exception cref="UnExpectedToken">Lanza la execpción si no encuentra un siguiente de la expresión</exception>
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