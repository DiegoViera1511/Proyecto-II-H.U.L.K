using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace HULK
{
    /// <summary>
    /// Representa las expresiones let-in
    /// </summary>
    class Let_in : Expression
    {
        public static Dictionary< string , object> idStore = new Dictionary<string, object>();

        /// <summary>
        /// Analiza que no hayan errores en la expresión 
        /// </summary>
        /// <exception cref="SyntaxError">Lanza error de sintaxis</exception>
        private void RemoveVariables(List<string> vars)
        {
            foreach(string variable in vars)
            {
                idStore.Remove(variable);
            }
        }
        public override void Analize()
        {
            List<string> LocalVariables = new();
            while(Lexer.index < Lexer.Tokens.Count)
            {
                if(Lexer.IsID(ActualToken()))
                {
                    if(Lexer.Key_Words.Contains(ActualToken()))
                    {
                        throw new SyntaxError(ActualToken() , "KeyWordID" );
                    }

                    string id = ActualToken();
                    Next();

                    if(ActualToken() == "=")
                    {
                        Next();
                        if(ActualToken() == "in" || ActualToken() == ",")
                        {
                            throw new SyntaxError("Missing Expression" ,  "Missing Token" , "let-in" , $"variable {Lexer.Tokens[Lexer.index - 2]}" );
                        }

                        Expression Value = new Union();
                        Value.Analize();
                            
                        string idType = Value.GetExpType();

                        if(idStore.ContainsKey(id))
                        {
                            throw new DuplicateArgument(id);
                        }
                        else if(Function.functionsId.ContainsKey(id))
                        {
                            Function.functionsId[id] = idType;
                        }
                        else
                        {
                            idStore.Add(id , idType);
                            LocalVariables.Add(id);
                        }
                    }
                    else
                    {
                        throw new SyntaxError("Missing ' = '" , "Missing Token" , "let-in" ,Lexer.Tokens[Lexer.index - 1] );
                    }  
                }
                else
                {
                    throw new SyntaxError("Missing ID" , "Missing Token" , "let-in" , Lexer.Tokens[Lexer.index - 1]);
                }            

                if(ActualToken() == ",")
                {
                    Next();
                }
                else if(ActualToken() == "in")
                {
                    Next();
                    break;
                }
                else if (Lexer.IsID(ActualToken()))
                {
                    throw new SyntaxError("Missing ' , '" , "Missing Token" , "let_in" , Lexer.Tokens[Lexer.index - 1]);
                }
                else 
                {
                    throw new SyntaxError("Invalid Token" , "Invalid Token" , "let-in" , ActualToken());
                }

            }    

            Expression letInExp = new Union();
            letInExp.Analize();
            type = letInExp.type ;
            RemoveVariables(LocalVariables);
            
        }
        /// <summary>
        /// Evalúa la expresión let-in , dándole valor a la expresión .
        /// </summary>
        /// <exception cref="SyntaxError">Lanza error de sintaxis</exception>
        public override void Evaluate()
        {
            List<string> LocalVariables = new();
            while(Lexer.index < Lexer.Tokens.Count)
            {
                if(Lexer.IsID(ActualToken()))
                {
                    if(Lexer.Key_Words.Contains(ActualToken()))
                    {
                        throw new SyntaxError(ActualToken() , "KeyWordID" );
                    }

                    string id = ActualToken();
                    Next();

                    if(ActualToken() == "=")
                    {
                        Next();
                        if(ActualToken() == "in" || ActualToken() == ",")
                        {
                            throw new SyntaxError("Missing Expression" ,  "Missing Token" , "let-in" , $"variable {Lexer.Tokens[Lexer.index - 2]}" );
                        }

                        Expression Value = new Union();
                        Value.Evaluate();
                            
                        object idValue = Value.GetValue();

                        if(idStore.ContainsKey(id))
                        {
                            throw new DuplicateArgument(id);
                        }
                        else if(Function.functionsId.ContainsKey(id))
                        {
                            Function.functionsId[id] = idValue;
                        }
                        else
                        {
                            idStore.Add(id , idValue);
                            LocalVariables.Add(id);
                        }
                            
                    }
                    else
                    {
                        throw new SyntaxError("Missing ' = '" , "Missing Token" , "let-in" ,Lexer.Tokens[Lexer.index - 1] );
                    }  
                }
                else
                {
                    throw new SyntaxError("Missing ID" , "Missing Token" , "let-in" , Lexer.Tokens[Lexer.index - 1]);
                }            

                if(ActualToken() == ",")
                {
                    Next();
                }
                else if(ActualToken() == "in")
                {
                    Next();
                    break;
                }
                else if (Lexer.IsID(ActualToken()))
                {
                    throw new SyntaxError("Missing ' , '" , "Missing Token" , "let_in" , Lexer.Tokens[Lexer.index - 1]);
                }
                else 
                {
                    throw new SyntaxError("Invalid Token" , "Invalid Token" , "let-in" , ActualToken());
                }

            }    

            Expression letInExp = new Union();
            letInExp.Evaluate();

            value = letInExp.GetValue();

            RemoveVariables(LocalVariables);

        }  
    }
    
}