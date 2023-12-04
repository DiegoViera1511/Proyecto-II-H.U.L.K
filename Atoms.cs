using System ;
using System.Text.RegularExpressions;

namespace HULK
{
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
            if(Lexer.index >= Lexer.Tokens.Count)
            {
                throw new DefaultError("Missing endOfFile");
            }
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
                        throw new ArgumentTypeError( "number" , num.GetExpType() );
                    }
                    throw new IncorrectOperator( num.GetExpType() , "Operator ' - '" , NumberType);
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
                        throw new ArgumentTypeError( "boolean" , boolean.GetExpType() );
                    }
                    throw new IncorrectOperator(boolean.GetExpType() , "Operator ' ! '" , BooleanType);
                }

            }
            else if(Lexer.index < Lexer.Tokens.Count && ActualToken() == "(") // ( expression )
            {
                Next();
                Expression result = new Union();
                result.Analize();
                string resultType = result.GetExpType();
                if(Lexer.index < Lexer.Tokens.Count && ActualToken() == ")")
                {
                    Next();
                    type = resultType;
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
                type = FunctionDeclaration.functionStore[Lexer.Tokens[i]].GetExpType() ;
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
                type = InferenceType;
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
            if(Lexer.index >= Lexer.Tokens.Count)
            {
                throw new DefaultError("Missing endOfFile");
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
            if(Lexer.index >= Lexer.Tokens.Count)
            {
                throw new DefaultError("Missing endOfFile");
            }
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
                
                if(num.GetValue().GetType() == Type.GetType("System.Double"))
                {
                    value = -1 * (double)num.GetValue() ;
                }
                else 
                {
                    if(isIdfunction)
                    {
                        throw new ArgumentTypeError( NumberType , Lexer.TokenType(num.GetValue()) );
                    }
                    throw new IncorrectOperator(Lexer.TokenType(num.GetValue()) , "Operator ' - '" , NumberType);
                }
            }
            else if(ActualToken() == "!")
            {
                Next() ;

                Expression boolean = new Atom() ;

                bool isIdfunction = false ;
                if(Function.functionsId.ContainsKey(ActualToken())) isIdfunction = true ;
                boolean.Evaluate();
                
                if(Lexer.TokenType(boolean.GetValue()) == BooleanType && (bool)boolean.GetValue())
                {
                    value = "false" ;
                }
                else if(Lexer.TokenType(boolean.GetValue()) == BooleanType && !(bool)boolean.GetValue())
                {
                    value = "true" ;
                }
                else 
                {
                    if(isIdfunction)
                    {
                        throw new ArgumentTypeError( BooleanType , Lexer.TokenType(boolean.GetValue()) );
                    }
                    throw new IncorrectOperator(Lexer.TokenType(boolean.GetValue()) , "Operator ' ! '" , BooleanType);
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
            if(Lexer.index >= Lexer.Tokens.Count)
            {
                throw new DefaultError("Missing endOfFile");
            }
        }
    }
}