using System.Text.RegularExpressions;

namespace HULK
{
    class FunctionDeclaration : Expression
    {
        public static Dictionary < string , Function > functionStore = new Dictionary<string, Function>();
        public static Dictionary < string , int > functionStack = new Dictionary<string, int>();
        public List<string>  functionArguments = new List<string>();
        public static int functionArgumentsCount = 0;
        public static Dictionary<string , string> functionsIdInference = new Dictionary<string, string>();

        public override void Analize()
        {
            List<string> functionExpression = new List<string>();

            if( Lexer.IsID(ActualToken()) )
            {
                if(Lexer.Key_Words.Contains(ActualToken()))
                {
                    throw new SyntaxError( ActualToken() , "KeyWordID" );
                }

                string functionId = ActualToken();
                functionsIdInference.Add(functionId , "functionId");

                Next();
                if(ActualToken() == "(")
                {
                    Next();
                    
                    while(Lexer.index < Lexer.Tokens.Count && ActualToken() != ")")
                    {
                        if(Lexer.IsID(ActualToken()))
                        {
                            if(!functionArguments.Contains(ActualToken()))
                            {
                                functionsIdInference.Add(ActualToken() , "variable");
                                functionArguments.Add(ActualToken());
                                functionArgumentsCount += 1 ;
                                Next();
                            }
                            else
                            {
                                throw new DuplicateArgument( ActualToken() ) ;
                            }
                        }
                        else 
                        {
                            throw new SyntaxError("Missing ID" , "Missing Token" , "Function declaration"  , Lexer.Tokens[Lexer.index - 1] );
                        }
                        if(ActualToken() != ",")
                        {
                            break;
                        }
                        else Next();
                    }
                    if(ActualToken() == ")")
                    {
                        Next();
                        if(ActualToken() == "=>")
                        {
                            Next();
                            while( Lexer.index < Lexer.Tokens.Count && ActualToken() != ";" )
                            {
                                if(Lexer.IsID(ActualToken()) && !Lexer.Key_Words.Contains(ActualToken()) && !functionArguments.Contains(ActualToken()) && ActualToken() != functionId && !functionStore.ContainsKey(ActualToken()) && !Let_in.idStore.ContainsKey(ActualToken()))
                                {
                                    throw new SyntaxError(ActualToken() , "DoNotExistID");
                                }
                                functionExpression.Add(ActualToken());
                                Next();
                            }

                            if(Lexer.index < Lexer.Tokens.Count && ActualToken() == ";" )
                            {
                                functionExpression.Add(ActualToken());
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
                        
                            if(functionStore.ContainsKey(functionId))
                            {
                                functionStore[functionId] = new Function(functionArguments , functionExpression , functionId);
                            }
                            else 
                            {
                                functionStore.Add(functionId , new Function(functionArguments , functionExpression , functionId));
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
        public override void Evaluate()
        {
            List<string> functionExpression = new List<string>();

            if( Lexer.IsID(ActualToken()) )
            {
                if(Lexer.Key_Words.Contains(ActualToken()))
                {
                    throw new SyntaxError( ActualToken() , "KeyWordID" );
                }

                string functionId = ActualToken();
                Next();
                if(ActualToken() == "(")
                {
                    Next();
                    
                    while(Lexer.index < Lexer.Tokens.Count && ActualToken() != ")")
                    {
                        if(Lexer.IsID(ActualToken()))
                        {
                            if(!functionArguments.Contains(ActualToken()))
                            {
                                functionArguments.Add(ActualToken());
                                Next();
                            }
                            else
                            {
                                throw new DuplicateArgument( ActualToken() ) ;
                            }
                        }
                        else 
                        {
                            throw new SyntaxError("Missing ID" , "Missing Token" , "Function declaration"  , Lexer.Tokens[Lexer.index - 1] );
                        }
                        if(ActualToken() != ",")
                        {
                            break;
                        }
                        else Next();
                    }
                    if(ActualToken() == ")")
                    {
                        Next();
                        if(ActualToken() == "=>")
                        {
                            Next();
                            while(ActualToken() != ";" && Lexer.index < Lexer.Tokens.Count)
                            {
                                if(Lexer.IsID(ActualToken()) && !Lexer.Key_Words.Contains(ActualToken()) && !functionArguments.Contains(ActualToken()) && ActualToken() != functionId && !functionStore.ContainsKey(ActualToken()) && !Let_in.idStore.ContainsKey(ActualToken()))
                                {
                                    throw new SyntaxError(ActualToken() , "DoNotExistID");
                                }
                                functionExpression.Add(ActualToken());
                                Next();
                            }

                            if(Lexer.index < Lexer.Tokens.Count && ActualToken() == ";" )
                            {
                                functionExpression.Add(ActualToken());
                            }
                            else return;
                        
                            if(functionStore.ContainsKey(functionId))
                            {
                                functionStore[functionId] = new Function(functionArguments , functionExpression , functionId);
                            }
                            else
                            {
                                functionStore.Add(functionId , new Function(functionArguments , functionExpression , functionId));
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

                Dictionary<string , object> Original_values = new Dictionary<string, object>();
                while(Lexer.index < Lexer.Tokens.Count && ActualToken() != ")")
                {
                    parameter.Evaluate();
                    argumentsValue.Add(parameter.value);
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

    class Print : Expression
    {
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
                    Lexer.ConsolePrints.Add(printExp.value);
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