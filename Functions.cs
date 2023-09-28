using System.Text.RegularExpressions;

namespace HULK
{
    class Function_Declaration : Expression
    {
        public static Dictionary < string , Function > Function_Store = new Dictionary<string, Function>();
        public static Dictionary < string , int > Function_Stack = new Dictionary<string, int>();

        public List<string>  function_Arguments = new List<string>();

        public override void Evaluate()
        {
            
            //List<string> function_Arguments = new List<string>();
            List<string> function_Expression = new List<string>();

            if(Lexer.IsID(Lexer.Tokens[Lexer.index]))
            {
                string function_id = Lexer.Tokens[Lexer.index];
                Next();
                if(Lexer.Tokens[Lexer.index] == "(")
                {
                    Next();
                    
                    while(Lexer.index < Lexer.Tokens.Count && Lexer.Tokens[Lexer.index] != ")")
                    {
                        if(Lexer.IsID(Lexer.Tokens[Lexer.index]))
                        {
                            if(!function_Arguments.Contains(Lexer.Tokens[Lexer.index]))
                            {
                                function_Arguments.Add(Lexer.Tokens[Lexer.index]);
                                Next();
                            }
                            else
                            {
                                throw new FunctionsErrors(Lexer.Tokens[Lexer.index], "DuplicateArgument" );
                            }
                        }
                        else 
                        {
                            throw new SyntaxError("Missing ID" , "Missing Token" , "Function declaration"  , Lexer.Tokens[Lexer.index - 1] );
                        }
                        if(Lexer.Tokens[Lexer.index] != ",")
                        {
                            break;
                        }
                        else Next();
                    }
                    if(Lexer.Tokens[Lexer.index] == ")")
                    {
                        Next();
                        if(Lexer.Tokens[Lexer.index] == "=>")
                        {
                            Next();
                            while(Lexer.Tokens[Lexer.index] != ";" && Lexer.index < Lexer.Tokens.Count)
                            {
                                if(Lexer.IsID(Lexer.Tokens[Lexer.index]) && !Lexer.Key_Words.Contains(Lexer.Tokens[Lexer.index]) && !function_Arguments.Contains(Lexer.Tokens[Lexer.index]) && Lexer.Tokens[Lexer.index] != function_id && !Function_Store.ContainsKey(Lexer.Tokens[Lexer.index]) && !Let_in.id_store.ContainsKey(Lexer.Tokens[Lexer.index]))
                                {
                                    throw new SyntaxError(Lexer.Tokens[Lexer.index] , "DoNotExistID");
                                }
                                function_Expression.Add(Lexer.Tokens[Lexer.index]);
                                Next();
                            }

                            if(Lexer.index < Lexer.Tokens.Count && Lexer.Tokens[Lexer.index] == ";" )
                            {
                                function_Expression.Add(Lexer.Tokens[Lexer.index]);
                            }
                            else return;
                        
                            if(Function_Store.ContainsKey(function_id))
                            {
                                Function_Store[function_id] = new Function(function_Arguments , function_Expression , function_id);
                            }
                            else
                            {
                                Function_Store.Add(function_id , new Function(function_Arguments , function_Expression , function_id));
                                Function_Stack.Add(function_id , 0);
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
        }
    }

    class Function : Expression
    {
        private string function_name ;
        public List<string> Arguments_id = new List<string>();
        public List<string> Arguments_value = new List<string>();
        public List<string> function_Expression = new List<string>();
        public static Dictionary<string , string > functions_id = new Dictionary<string, string>();

        public Function(List<string> function_Arguments_id , List<string> Expression , string name)
        {
            this.Arguments_id = function_Arguments_id;
            this.function_Expression = Expression;
            this.function_name = name ;
        }
        public override void Evaluate()
        {
            if(Function_Declaration.Function_Stack[function_name] > 500)
            {   
                throw new FunctionsErrors(function_name , "StackOverflow");
            }
            else Function_Declaration.Function_Stack[function_name]++;

            if(Lexer.Tokens[Lexer.index] == "(")
            {
                string result ;
                Next();

                Expression e = new B();

                Arguments_value.Clear();

                Dictionary<string , string> Original_values = new Dictionary<string, string>();
                while(Lexer.index < Lexer.Tokens.Count && Lexer.Tokens[Lexer.index] != ")")
                {
                    e.Evaluate();
                    Arguments_value.Add(e.value);
                    if(Lexer.Tokens[Lexer.index] != ",")
                    {
                        break;
                    }
                    else Next();
                }
                if(Arguments_id.Count == Arguments_value.Count)
                {
                    for(int i = 0 ; i < Arguments_id.Count ; i++)
                    {
                        if(functions_id.ContainsKey(Arguments_id[i]))
                        {
                            Original_values.Add(Arguments_id[i] , functions_id[Arguments_id[i]]);
                            //Actualiza
                            functions_id[Arguments_id[i]] = Arguments_value[i];
                        }
                        else
                        {
                            functions_id.Add(Arguments_id[i] , Arguments_value[i]);
                        }
                    }
                }
                else 
                {
                    throw new FunctionsErrors(function_name , "ArgumentsCountError", Arguments_id.Count , Arguments_value.Count );
                }
                
                List<string> Originals_Tokens = Lexer.Tokens;
                int Original_index = Lexer.index;

                Lexer.Tokens = function_Expression;
                Lexer.index = 0;

                Expression FE = new B();
                
                try
                {
                    FE.Evaluate();
                }
                catch(SemanticError se)
                {
                    Lexer.Tokens = Originals_Tokens;
                    Lexer.index = Original_index;
                    if(se.ProblemType == "ArgumentTypeError")
                    {
                        throw new FunctionsErrors( function_name , "ArgumentTypeError" , se.ExpectedToken , se.BadToken);
                    }
                    else throw se ;
                }
                catch(HULK_Errors he)
                {
                    Lexer.Tokens = Originals_Tokens;
                    Lexer.index = Original_index;
                    throw he;
                }
                
                result = FE.value;
                
                Lexer.Tokens = Originals_Tokens;
                Lexer.index = Original_index;

                foreach(string s in functions_id.Keys)
                {
                    if(Original_values.ContainsKey(s))
                    {
                        functions_id[s] = Original_values[s];
                    }
                }

                if(Lexer.Tokens[Lexer.index] == ")")
                {
                    value = result ;
                    Next();
                    Arguments_value.Clear();
                    Function_Declaration.Function_Stack[function_name]--;
                }
                else 
                {
                    //HULK_Errors.SyntaxError("Missing ' ) " , "Missing Token" , "Function Declaration" , Lexer.Tokens[Lexer.index - 1]);
                    throw new SyntaxError("Missing ' ) ' " , "Missing Token" , "Function Declaration" , Lexer.Tokens[Lexer.index - 1]);
                    //return;
                }

            }
        }
    }

    class Print : Expression
    {
        public override void Evaluate()
        {
            if(Lexer.Tokens[Lexer.index] == "(")
            {
                Next();
                if(Lexer.Tokens[Lexer.index] == ")")
                {
                    Lexer.index++;
                    Console.WriteLine();
                    return;
                }

                Expression x = new B();
                x.Evaluate();

                if(Lexer.Tokens[Lexer.index] == ")")
                {
                    Next();
                    if(Regex.IsMatch(x.value , @"(\u0022([^\u0022\\]|\\.)*\u0022)"))
                    {
                        //Console.WriteLine(x.value.Substring( 1 , x.value.Length - 2));
                        Lexer.ConsolePrints.Add(x.value.Substring( 1 , x.value.Length - 2));
                        
                    }
                    else 
                    {
                        //Console.WriteLine(x.value);
                        Lexer.ConsolePrints.Add(x.value);
                    }
                    
                    value = x.value;
                }
                else
                {
                    //HULK_Errors.SyntaxError("Missing ' ) '" , "Missing Token" , "print" , Lexer.Tokens[Lexer.index-1] );
                    throw new SyntaxError("Missing ' ) '" , "Missing Token" , "print" , Lexer.Tokens[Lexer.index-1] );
                    //return;
                }
            }
            else
            {
               // HULK_Errors.SyntaxError("Missing ' ( '" , "Missing Token" , "print" , Lexer.Tokens[Lexer.index-1] );
                throw new SyntaxError("Missing ' ( '" , "Missing Token" , "print" , Lexer.Tokens[Lexer.index-1]);
                //return;
            }
           
        }
    }
}