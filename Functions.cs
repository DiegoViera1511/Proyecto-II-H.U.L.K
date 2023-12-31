using System.Text.RegularExpressions;

namespace HULK
{
    /// <summary>
    /// Representa las expresiones de declaraciones de funciones
    /// </summary>
    class FunctionDeclaration 
    {
        public static Dictionary < string , Function > functionStore = new Dictionary<string, Function>();
        public static Dictionary < string , int > functionStack = new Dictionary<string, int>();
        public List<string>  functionArguments = new List<string>();
        public static int functionArgumentsCount = 0;
        public static Dictionary<string , string> functionsIdInference = new Dictionary<string, string>();

        /// <summary>
        /// Analiza que sea correcta la declaración de la función .
        /// </summary>
        /// <exception cref="SyntaxError">Lanza error de sintaxis</exception>
        /// <exception cref="DuplicateArgument">Lanza error si existen argumentos de la función duplicados</exception>
        public void Analize()
        {
            List<string> functionExpression = new List<string>();

            if( Lexer.IsID(Expression.ActualToken()) )
            {
                if(Lexer.Key_Words.Contains(Expression.ActualToken()))
                {
                    throw new SyntaxError( Expression.ActualToken() , "KeyWordID" );
                }

                string functionId = Expression.ActualToken();
                functionsIdInference.Add(functionId , "functionId");

                Expression.Next();
                if(Expression.ActualToken() == "(")
                {
                    Expression.Next();
                    
                    while(Lexer.index < Lexer.Tokens.Count && Expression.ActualToken() != ")")
                    {
                        if(Lexer.IsID(Expression.ActualToken()))
                        {
                            if(!functionArguments.Contains(Expression.ActualToken()))
                            {
                                functionsIdInference.Add(Expression.ActualToken() , "variable");
                                functionArguments.Add(Expression.ActualToken());
                                functionArgumentsCount += 1 ;
                                Expression.Next();
                            }
                            else
                            {
                                throw new DuplicateArgument( Expression.ActualToken() ) ;
                            }
                        }
                        else 
                        {
                            throw new SyntaxError("Missing ID" , "Missing Token" , "Function declaration"  , Lexer.Tokens[Lexer.index - 1] );
                        }
                        if(Expression.ActualToken() != ",")
                        {
                            break;
                        }
                        else Expression.Next();
                    }
                    if(Expression.ActualToken() == ")")
                    {
                        Expression.Next();
                        if(Expression.ActualToken() == "=>")
                        {
                            Expression.Next();
                            while( Lexer.index < Lexer.Tokens.Count && Expression.ActualToken() != ";" )
                            {
                                if(Lexer.IsID(Expression.ActualToken()) && !Lexer.Key_Words.Contains(Expression.ActualToken()) && !functionArguments.Contains(Expression.ActualToken()) && Expression.ActualToken() != functionId && !functionStore.ContainsKey(Expression.ActualToken()) && !Let_in.idStore.ContainsKey(Expression.ActualToken()))
                                {
                                    throw new SyntaxError(Expression.ActualToken() , "DoNotExistID");
                                }
                                functionExpression.Add(Expression.ActualToken());
                                Expression.Next();
                            }

                            if(Lexer.index < Lexer.Tokens.Count && Expression.ActualToken() == ";" )
                            {
                                functionExpression.Add(Expression.ActualToken());
                            }
                            else return;

                            List<string> originalsTokens = Lexer.Tokens;
                            int originalIndex = Lexer.index;

                            Lexer.Tokens = functionExpression;
                            Lexer.index = 0;
                            
                            Expression functionbody = new Union();

                            try
                            {
                                functionbody.Analize();
                            }
                            catch(HulkErrors he)
                            {
                                Lexer.Tokens = originalsTokens ;
                                Lexer.index = originalIndex ;
                                throw he ;
                            }

                            Lexer.Tokens = originalsTokens ;
                            Lexer.index = originalIndex ;

                            Function f = new Function(functionArguments , functionExpression , functionId){type = functionbody.type};
                        
                            if(functionStore.ContainsKey(functionId))
                            {
                                functionStore[functionId] = f ;
                            }
                            else 
                            {
                                functionStore.Add(functionId , f);
                                functionStack.Add(functionId , 0);
                            }
                        }
                        else 
                        {
                            throw new SyntaxError("Missing ' => " , "Missing Token" , "Function Declaration" , Lexer.Tokens[Lexer.index - 1]);
                        }
                    }
                    else 
                    {
                        throw new SyntaxError("Missing ' ) " , "Missing Token" , "Function Declaration" , Lexer.Tokens[Lexer.index - 1]);
                    }
                }
            }
            else 
            {
                throw new SyntaxError("Missing ID" , "Missing Token" , "let-in" , Lexer.Tokens[Lexer.index - 1]);
            }
        }
    }
    
    /// <summary>
    /// Representa las expresiones de llamado de función
    /// </summary>
    class Function : Expression
    {
        private string functionName ;
        public List<string> argumentsId = new List<string>();
        public List<object> argumentsValue = new List<object>();
        public List<string> functionExpression = new List<string>();
        public static Dictionary<string , object > functionsId = new Dictionary<string, object>();

        public Function(List<string> argumentsId , List<string> functionExpression , string functionName)
        {
            this.argumentsId = argumentsId;
            this.functionExpression = functionExpression ;
            this.functionName = functionName ;
        }
        /// <summary>
        /// Analiza que el llamado de la función sea correcto sin evaluarla
        /// </summary>
        /// <param name="functionName">Nombre de la función</param>
        /// <exception cref="ArgumentsCountError">Lanza error si la cantidad de argumentos esperados son incorrectos</exception>
        public static void CheckfunctionCall(string functionName)
        {
            if(ActualToken() == "(")
            {
                Next();

                Expression parameter = new Union();
                int countOfArguments = 0 ;
                while(Lexer.index < Lexer.Tokens.Count && ActualToken() != ")")
                {
                    parameter.Analize();
                    countOfArguments += 1 ;
                    if(ActualToken() != ",")
                    {
                        break;
                    }
                    else Next();
                }
                if(FunctionDeclaration.functionArgumentsCount != countOfArguments)
                {
                    throw new ArgumentsCountError(functionName , FunctionDeclaration.functionArgumentsCount , countOfArguments );
                }
                else 
                Next();
            }   
        }
        /// <summary>
        /// Analiza que el llamado de la función sea correcto
        /// </summary>
        /// <exception cref="SyntaxError">Lanza error de sintaxis</exception>
        /// <exception cref="ArgumentsCountError">Lanza error si la cantidad de argumentos esperados son incorrectos</exception>
        public override void Analize()
        {
            if(ActualToken() == "(")
            {
                Next();

                Expression parameter = new Union();

                int argumentsValueCount = 0;

                while(Lexer.index < Lexer.Tokens.Count && ActualToken() != ")")
                {
                    parameter.Analize();
                    argumentsValueCount += 1 ;
                    if(ActualToken() != ",")
                    {
                        break;
                    }
                    else Next();
                }
                if(ActualToken() != ")")
                {
                    throw new SyntaxError("Missing ' ) ' " , "Missing Token" , "Function call" , Lexer.Tokens[Lexer.index - 1]);
                }

                Next();

                if(argumentsId.Count != argumentsValueCount)
                {
                    throw new ArgumentsCountError(functionName , argumentsId.Count , argumentsValueCount );
                }
            }
            else
            {
                throw new SyntaxError("Missing ' ( ' " , "Missing Token" , "Function call" , Lexer.Tokens[Lexer.index - 1]);
            }
        }

        /// <summary>
        /// Evalúa la función , devolviendo el valor de su cuerpo con los argumentos dados
        /// </summary>
        /// <exception cref="DefaultError">Stack Overflow</exception>
        /// <exception cref="ArgumentsCountError">La cantidad de argumentos esperados son incorrectos</exception>
        /// <exception cref="ArgumentTypeError"></exception>
        /// <exception cref="SyntaxError">Lanza error de sintaxis</exception>
        public override void Evaluate()
        {
            if(FunctionDeclaration.functionStack[functionName] > 700)
            {   
                throw new DefaultError("StackOverflow" , functionName );
            }
            else FunctionDeclaration.functionStack[functionName]++;

            if(ActualToken() == "(")
            {
                Next();

                Expression parameter = new Union();

                argumentsValue.Clear();

                Dictionary<string , object> Original_values = new Dictionary<string, object>();//Guardar los argumentos de la llamada de función actual
                while(Lexer.index < Lexer.Tokens.Count && ActualToken() != ")")
                {
                    parameter.Evaluate();
                    argumentsValue.Add(parameter.GetValue());
                    if(ActualToken() != ",")
                    {
                        break;
                    }
                    else Next();
                }
                if(argumentsId.Count == argumentsValue.Count)
                {
                    for(int i = 0 ; i < argumentsId.Count ; i++)
                    {
                        if(functionsId.ContainsKey(argumentsId[i]))
                        {
                            Original_values.Add(argumentsId[i] , functionsId[argumentsId[i]]);
                            //Actualiza
                            functionsId[argumentsId[i]] = argumentsValue[i];
                        }
                        else
                        {
                            functionsId.Add(argumentsId[i] , argumentsValue[i]);
                        }
                    }
                }
                else 
                {
                    throw new ArgumentsCountError(functionName , argumentsId.Count , argumentsValue.Count );
                }
                
                List<string> originalsTokens = Lexer.Tokens;
                int originalIndex = Lexer.index;

                Lexer.Tokens = functionExpression;
                Lexer.index = 0;

                Expression FE = new Union();
                
                try
                {
                    FE.Evaluate();
                }
                catch(ArgumentTypeError ae)//Catch argument type error
                {
                    Lexer.Tokens = originalsTokens;
                    Lexer.index = originalIndex;
                    throw new ArgumentTypeError(ae.expectedToken , ae.badToken , functionName );
                }
                catch(HulkErrors he)
                {
                    Lexer.Tokens = originalsTokens;
                    Lexer.index = originalIndex;
                    throw he;
                }
                            
                Lexer.Tokens = originalsTokens;
                Lexer.index = originalIndex;

                foreach(string s in functionsId.Keys)
                {
                    if(Original_values.ContainsKey(s))
                    {
                        functionsId[s] = Original_values[s];
                    }
                }

                if(ActualToken() == ")")
                {
                    value = FE.value ;
                    
                    Next();
                    argumentsValue.Clear();
                    FunctionDeclaration.functionStack[functionName]--;
                }
                else 
                {
                    throw new SyntaxError("Missing ' ) ' " , "Missing Token" , "Function call" , Lexer.Tokens[Lexer.index - 1]);
                }
            }
            else
            {
                throw new SyntaxError("Missing ' ( ' " , "Missing Token" , "Function call" , Lexer.Tokens[Lexer.index - 1]);
            }
        }
        
    }

    /// <summary>
    /// Representa la función de Print .
    /// </summary>
    class Print : Expression
    {
        /// <summary>
        /// Analiza que no hayan errores de sintaxis en la expresión 
        /// </summary>
        /// <exception cref="SyntaxError">Lanza error de sintaxis</exception>
        public override void Analize()
        {
            if(ActualToken() == "(")
            {
                Next();

                if(ActualToken() == ")")
                {
                    Next();
                    return;
                }

                Expression printExp = new Union();
                printExp.Analize();

                if(ActualToken() == ")")
                {
                    Next();
                    type = printExp.type;
                }
                else
                {
                    throw new SyntaxError("Missing ' ) '" , "Missing Token" , "print" , Lexer.Tokens[Lexer.index-1] );
                }
            }
            else
            {
                throw new SyntaxError("Missing ' ( '" , "Missing Token" , "print" , Lexer.Tokens[Lexer.index-1]);
            }
        }

        /// <summary>
        /// Evalúa la expresión , imprimiendo en pantalla el valor de la expresión 
        /// </summary>
        /// <exception cref="SyntaxError"></exception>
        public override void Evaluate()
        {
            if(ActualToken() == "(")
            {
                Next();

                if(ActualToken() == ")")
                {
                    Lexer.index++;
                    Console.WriteLine();
                    return;
                }

                Expression printExp = new Union();
                printExp.Evaluate();

                if(ActualToken() == ")")
                {
                    Next();
                    Lexer.ConsolePrints.Add(printExp.GetValue());
                    value = printExp.value;
                }
                else
                {
                    throw new SyntaxError("Missing ' ) '" , "Missing Token" , "print" , Lexer.Tokens[Lexer.index-1] );
                }
            }
            else
            {
                throw new SyntaxError("Missing ' ( '" , "Missing Token" , "print" , Lexer.Tokens[Lexer.index-1]);
            }
        }
    }
    
}