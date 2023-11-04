using System;
using System.Data.Common;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

namespace HULK
{

    //Abstract classes
    #region Abstract classes
    /// <summary>
    /// Clase abstracta de expresión
    /// </summary>
    abstract class Expression
    {
        public const string NumberType = "number" ;
        public const string StringType = "string" ;
        public const string BooleanType = "boolean" ;
        public const string InferenceType = "inference" ;

        public object value ;

        public string type ;
        /// <summary>
        /// Mueve el índice al próximo token
        /// </summary>
        static public void Next()
        {
            Lexer.index++;
        }
        /// <summary>
        /// Retorna el token actual
        /// </summary>
        /// <returns>Token actual</returns>
        static public string ActualToken()
        {
            return Lexer.Tokens[Lexer.index];
        }
        public abstract void Evaluate();

        public abstract void Analize();
    }
    /// <summary>
    /// Clase abstracta de expresiones binarias
    /// </summary>
    abstract class Binary_Exrpessions : Expression
    {
        public Expression left ;
        public string? iDLeft  ;
        public Expression right ;
        public string? iDRight  ;

        public abstract object Operation(object left , string operatorToken , object right);

        /// <summary>
        /// Encuentra errores de argumentos de funciones
        /// </summary>
        /// <param name="idLeft">Token que representa al valor de la izquierda</param>
        /// <param name="leftType">Tipo del valor de la expresión de la izquierda</param>
        /// <param name="iDRight">Token que representa al valor de la derecha</param>
        /// <param name="rightType">Tipo del valor de la expresión de la derecha</param>
        /// <param name="expectedType">Tipo de valor esperado</param>
        /// <exception cref="ArgumentTypeError">Lanza error si left o right no son del tipo correcto y son argumentos de función</exception>
        public void CatchArgumentTypeError(string idLeft , string leftType , string iDRight , string rightType , string expectedType)
        {
            if(Function.functionsId.ContainsKey(idLeft))
            {
                if(leftType != expectedType)
                {
                    throw new ArgumentTypeError(expectedType , leftType);
                }
            }
            if(Function.functionsId.ContainsKey(iDRight))
            {
                if(rightType != expectedType)
                {
                    throw new ArgumentTypeError(expectedType , rightType);
                }
            }
        }
    }

    #endregion
    /// <summary>
    /// Representa las expresiones atómicas
    /// </summary>
    class Atom : Expression
    {
        //Atomics expressions ( numbers , strings , booleans )
        /// <summary>
        /// Analiza el tipo de la expresión atómica . 
        /// </summary>
        /// <exception cref="ArgumentTypeError">Lanza error si left o right no son del tipo correcto y son argumentos de función</exception>
        /// <exception cref="IncorrectOperator">Lanza error si el operador unario ( ! , - ) no es válido para el tipo de la expresión</exception>
        /// <exception cref="SyntaxError">Lanza error de sintaxis</exception>
        /// <exception cref="UnExpectedToken">Lanza error si no encuentra un siguiente de la expresión o es un token no válido</exception>
        public override void Analize()
        {
            if(Lexer.IsNumber(ActualToken())) // numbers
            {
                type = NumberType;
                Next();
            }
            else if(Lexer.index < Lexer.Tokens.Count && Function.functionsId.ContainsKey(ActualToken())) // function variable 
            {
                string tokenType = (string)Function.functionsId[ActualToken()];
                Next();
                type = tokenType ;
            }
            else if (Lexer.index < Lexer.Tokens.Count && Let_in.idStore.ContainsKey(ActualToken())) // let-in variable
            {
                string tokenType = (string)Let_in.idStore[ActualToken()];
                Next();
                type = tokenType ;
            }
            else if(ActualToken() == "-") // negative numbers
            {
                Next();

                Expression num = new Atom();
                bool isIdfunction = false ;
                if(Function.functionsId.ContainsKey(ActualToken())) isIdfunction = true ;
                num.Analize();
                
                if(num.type == NumberType)
                {
                    type = NumberType ;
                }
                else 
                {
                    if(isIdfunction)
                    {
                        throw new ArgumentTypeError( "number" , num.type );
                    }
                    throw new IncorrectOperator( num.type , "Operator ' - '" , NumberType);
                }
            }
            else if(ActualToken() == "!")
            {
                Next() ;

                Expression boolean = new Atom() ;

                bool isIdfunction = false ;
                if(Function.functionsId.ContainsKey(ActualToken())) isIdfunction = true ;
                boolean.Analize();
                
                if(boolean.type == BooleanType)
                {
                    type = BooleanType ;
                }
                else 
                {
                    if(isIdfunction)
                    {
                        throw new ArgumentTypeError( "boolean" , boolean.type );
                    }
                    throw new IncorrectOperator(boolean.type , "Operator ' ! '" , BooleanType);
                }

            }
            else if(Lexer.index < Lexer.Tokens.Count && ActualToken() == "(") // ( expression )
            {
                Next();
                Expression result = new Union();
                result.Analize();
                string x = result.type;
                if(Lexer.index < Lexer.Tokens.Count && ActualToken() == ")")
                {
                    Next();
                    type = x;
                }
                else
                {
                   throw new SyntaxError("Missing ' ) " , "Missing Token" , "let-in" , Lexer.Tokens[Lexer.index - 1] ) ;
                }

            }
            else if(Lexer.index < Lexer.Tokens.Count && Lexer.IsString(ActualToken()) ) // strings
            {
                type = StringType ;
                Next();
            }
            else if(Lexer.index < Lexer.Tokens.Count && ActualToken() == "let ") // let-in expressions
            {
                Next();
                Expression l = new Let_in();
                l.Analize();
                type = l.type;
            }
            else if(Lexer.index < Lexer.Tokens.Count && ActualToken() == "print") // print expression
            {
                Next();
                Expression p = new Print();
                p.Analize();
                type = p.type;
            }
            else if(Lexer.index < Lexer.Tokens.Count && MathExpressions.MathFunctions.Contains(ActualToken())) // Math function expression
            {
                Expression M = new MathExpressions(ActualToken());
                Next();
                M.Analize();
                type = M.type ;
            }
            else if(Lexer.index < Lexer.Tokens.Count && FunctionDeclaration.functionStore.ContainsKey(ActualToken())) // function call
            {
                int i = Lexer.index;
                Next();
                FunctionDeclaration.functionStore[Lexer.Tokens[i]].Analize();
                type = InferenceType;
            }
            else if(Lexer.index < Lexer.Tokens.Count && ActualToken() == "true") // boolean true
            {
                Next();
                type = BooleanType ;
            }
            else if(Lexer.index < Lexer.Tokens.Count && ActualToken() == "false") // boolean false
            {
                Next();
                type = BooleanType ;
            }
            else if (ActualToken() == "if") // if-else expression
            {
                Next();
                Expression c = new Conditional();
                c.Analize();
                type = c.type;
            }
            else if(Lexer.index < Lexer.Tokens.Count && FunctionDeclaration.functionsIdInference.ContainsKey(ActualToken()))
            {
                if(FunctionDeclaration.functionsIdInference[ActualToken()] == "functionId")
                {
                    string functionName = ActualToken();
                    Next();
                    Function.CheckfunctionCall(functionName);
                    type = InferenceType ;
                }
                else if(FunctionDeclaration.functionsIdInference[ActualToken()] == "variable")
                {
                    Next();
                    type = InferenceType;
                }
            }
            else 
            {
                if(Lexer.IsID(ActualToken()))
                {
                    throw new SyntaxError(ActualToken() , "DoNotExistID");
                }
                else
                {
                    throw new UnExpectedToken(ActualToken()) ;
                }
            }
        }
        /// <summary>
        /// Evalúa la expresión atómica .
        /// </summary>
        /// <exception cref="ArgumentTypeError">Lanza error si left o right no son del tipo correcto y son argumentos de función</exception>
        /// <exception cref="IncorrectOperator">Lanza error si el operador unario ( ! , - ) no es válido para el tipo de la expresión</exception>
        /// <exception cref="SyntaxError">Lanza error de sintaxis</exception>
        /// <exception cref="UnExpectedToken">Lanza error si no encuentra un siguiente de la expresión o es un token no válido</exception>
        public override void Evaluate()
        {
            if(Lexer.IsNumber(ActualToken())) // numbers
            {
                value = Convert.ToDouble(ActualToken());
                Next();
            }
            else if(Lexer.index < Lexer.Tokens.Count && Function.functionsId.ContainsKey(ActualToken())) // function variable 
            {
                var x = Function.functionsId[ActualToken()];
                Next();
                value = x ;
            }
            else if (Lexer.index < Lexer.Tokens.Count && Let_in.idStore.ContainsKey(ActualToken())) // let-in variable
            {
                var x = Let_in.idStore[ActualToken()];
                Next();
                value = x ;
            }
            else if(ActualToken() == "-") // negative numbers
            {
                Next();

                Expression num = new Atom();
                bool isIdfunction = false ;
                if(Function.functionsId.ContainsKey(ActualToken())) isIdfunction = true ;
                num.Evaluate();
                
                if(num.value.GetType() == Type.GetType("System.Double"))
                {
                    value = -1 * (double)num.value ;
                }
                else 
                {
                    if(isIdfunction)
                    {
                        throw new ArgumentTypeError( NumberType , Lexer.TokenType(num.value) );
                    }
                    throw new IncorrectOperator(Lexer.TokenType(num.value) , "Operator ' - '" , NumberType);
                }
            }
            else if(ActualToken() == "!")
            {
                Next() ;

                Expression boolean = new Atom() ;

                bool isIdfunction = false ;
                if(Function.functionsId.ContainsKey(ActualToken())) isIdfunction = true ;
                boolean.Evaluate();
                
                if(Lexer.TokenType(boolean.value) == BooleanType && (bool)boolean.value)
                {
                    value = "false" ;
                }
                else if(Lexer.TokenType(boolean.value) == BooleanType && !(bool)boolean.value)
                {
                    value = "true" ;
                }
                else 
                {
                    if(isIdfunction)
                    {
                        throw new ArgumentTypeError( BooleanType , Lexer.TokenType(boolean.value) );
                    }
                    throw new IncorrectOperator(Lexer.TokenType(boolean.value) , "Operator ' ! '" , BooleanType);
                }

            }
            else if(Lexer.index < Lexer.Tokens.Count && ActualToken() == "(") // ( expression )
            {
                Next();
                Expression result = new Union();
                result.Evaluate();
                var x = result.value;
                if(Lexer.index < Lexer.Tokens.Count && ActualToken() == ")")
                {
                    Next();
                    value = x;
                }
                else
                {
                   throw new SyntaxError("Missing ' ) " , "Missing Token" , "let-in" , Lexer.Tokens[Lexer.index - 1] ) ;
                }

            }
            else if(Lexer.index < Lexer.Tokens.Count && Lexer.IsString(ActualToken()) ) // strings
            {
                value = ActualToken().Substring( 1 , ActualToken().Length - 2);
                Next();
            }
            else if(Lexer.index < Lexer.Tokens.Count && ActualToken() == "let ") // let-in expressions
            {
                Next();
                Expression l = new Let_in();
                l.Evaluate();
                value = l.value;
            }
            else if(Lexer.index < Lexer.Tokens.Count && ActualToken() == "print") // print expression
            {
                Next();
                Expression p = new Print();
                p.Evaluate();
                value = p.value;
            }
            else if(Lexer.index < Lexer.Tokens.Count && MathExpressions.MathFunctions.Contains(ActualToken())) // Math function expression
            {
                Expression M = new MathExpressions(ActualToken());
                Next();
                M.Evaluate();
                value = M.value ;

            }
            else if(Lexer.index < Lexer.Tokens.Count && FunctionDeclaration.functionStore.ContainsKey(ActualToken())) // function call
            {
                int i = Lexer.index;
                Next();
                FunctionDeclaration.functionStore[Lexer.Tokens[i]].Evaluate();
                value = FunctionDeclaration.functionStore[Lexer.Tokens[i]].value;
            }
            else if(Lexer.index < Lexer.Tokens.Count && ActualToken() == "true") // boolean true
            {
                Next();
                value = true;
            }
            else if(Lexer.index < Lexer.Tokens.Count && ActualToken() == "false") // boolean false
            {
                Next();
                value = false;
            }
            else if (ActualToken() == "if") // if-else expression
            {
                Next();
                Expression c = new Conditional();
                c.Evaluate();
                value = c.value;
            }
            else 
            {
                if(Lexer.IsID(ActualToken()))
                {
                    throw new SyntaxError(ActualToken() , "DoNotExistID");
                }
                else
                {
                    throw new UnExpectedToken(ActualToken()) ;
                }
            }
        }
    }
    /// <summary>
    /// Clase inicializadora , evalúa todo tipo de expresiones HULK .
    /// </summary>
    class HulkExpression : Expression
    {
        //Inicio de Expressión

        public override void Analize()
        {
            if(Lexer.Tokens.Count == 0)// Void Expression
            {
                return;
            }

            if(ActualToken() == "print")
            {
                Next();
                Expression printExp = new Print();
                printExp.Analize();
            }
            else if(ActualToken() == "let ")
            {
                Next();
                Expression letIn = new Let_in();
                letIn.Analize();
            }
            else if(ActualToken() == "if")
            {
                Next();
                Expression ifelse = new Conditional();
                ifelse.Analize();
            }
            else if(ActualToken() == "function")
            {
                Next();
                Expression f = new FunctionDeclaration();
                f.Analize();
            }
            else if(FunctionDeclaration.functionStore.ContainsKey(ActualToken()))
            {
                int i = Lexer.index;
                Next();
                FunctionDeclaration.functionStore[Lexer.Tokens[i]].Analize() ;
            }
            else //Atom expression
            {
                Expression exp = new Union();
                exp.Analize();
            }
        }
        public override void Evaluate()
        {
            
            if(Lexer.Tokens.Count == 0)// Void Expression
            {
                return;
            }

            if(ActualToken() == "print")
            {
                Next();
                Expression printExp = new Print();
                printExp.Evaluate();
                value = printExp.value;
            }
            else if(ActualToken() == "let ")
            {
                Next();
                Expression letIn = new Let_in();
                letIn.Evaluate();
                value = letIn.value;
            }
            else if(ActualToken() == "if")
            {
                Next();
                Expression ifelse = new Conditional();
                ifelse.Evaluate();
                value = ifelse.value;
            }
            else if(ActualToken() == "function")
            {
                Next();
                Expression f = new FunctionDeclaration();
                f.Evaluate();
                value = "";
            }
            else if(FunctionDeclaration.functionStore.ContainsKey(ActualToken()))
            {

                int i = Lexer.index;
                Next();
                FunctionDeclaration.functionStore[Lexer.Tokens[i]].Evaluate() ;
                value = FunctionDeclaration.functionStore[Lexer.Tokens[i]].value ;
                
                Lexer.ConsolePrints.Add(value);
              
            }
            else //Atom expression
            {
                Expression exp = new Union();
                exp.Evaluate();
                value = exp.value ;
            }
        }
    }  

}