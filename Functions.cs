using System.Text.RegularExpressions;

namespace HULK
{
    class FunctionDeclaration : Expression
    {
        public static Dictionary < string , Function > functionStore = new Dictionary<string, Function>();
        public static Dictionary < string , int > functionStack = new Dictionary<string, int>();
        public List<string>  functionArguments = new List<string>();

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
                                throw new FunctionsErrors(ActualToken(), "DuplicateArgument" );
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
        public List<string> argumentsValue = new List<string>();
        public List<string> functionExpression = new List<string>();
        public static Dictionary<string , string > functionsId = new Dictionary<string, string>();

        public Function(List<string> argumentsId , List<string> functionExpression , string functionName)
        {
            this.argumentsId = argumentsId ;
            this.functionExpression = functionExpression;
            this.functionName = functionName;
        }
        public override void Evaluate()
        {
            if(FunctionDeclaration.functionStack[functionName] > 500)
            {   
                throw new FunctionsErrors(functionName , "StackOverflow");
            }
            else FunctionDeclaration.functionStack[functionName]++;

            if(ActualToken() == "(")
            {
                //string result ;
                Next();

                Expression parameter = new BooleanOperator();

                argumentsValue.Clear();

                Dictionary<string , string> Original_values = new Dictionary<string, string>();
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
                    throw new FunctionsErrors(functionName , "ArgumentsCountError", argumentsId.Count , argumentsValue.Count );
                }
                
                List<string> originalsTokens = Lexer.Tokens;
                int originalIndex = Lexer.index;

                Lexer.Tokens = functionExpression;
                Lexer.index = 0;

                Expression FE = new BooleanOperator();
                
                try
                {
                    FE.Evaluate();
                }
                catch(SemanticError se)//Catch argument type error
                {
                    Lexer.Tokens = originalsTokens;
                    Lexer.index = originalIndex;
                    if(se.ProblemType == "ArgumentTypeError")
                    {
                        throw new FunctionsErrors( functionName , "ArgumentTypeError" , se.ExpectedToken , se.BadToken);
                    }
                    else throw se ;
                }
                catch(HulkErrors he)
                {
                    Lexer.Tokens = originalsTokens;
                    Lexer.index = originalIndex;
                    throw he;
                }
                
                //result = FE.value;
                
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
                    throw new SyntaxError("Missing ' ) ' " , "Missing Token" , "Function Declaration" , Lexer.Tokens[Lexer.index - 1]);
                }
            }
        }
    }

    class Print : Expression
    {
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

                Expression printExp = new BooleanOperator();
                printExp.Evaluate();

                if(ActualToken() == ")")
                {
                    Next();
                    if(Lexer.IsString(printExp.value))
                    {
                        Lexer.ConsolePrints.Add(printExp.value.Substring( 1 , printExp.value.Length - 2));
                    }
                    else 
                    {
                        Lexer.ConsolePrints.Add(printExp.value);
                    }
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