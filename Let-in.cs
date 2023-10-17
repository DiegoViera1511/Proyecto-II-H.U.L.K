using System.Text.RegularExpressions;

namespace HULK
{
    class Let_in : Expression
    {
        public static Dictionary< string , object> idStore = new Dictionary<string, object>();

        public override void Analize()
        {
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
                            
                        string idType = Value.type;

                        if(idStore.ContainsKey(id))
                        {
                            idStore[id] = idType;
                        }
                        else if(Function.functionsId.ContainsKey(id))
                        {
                            Function.functionsId[id] = idType;
                        }
                        else idStore.Add(id , idType);
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

            bool parenthesis = false;
            if(ActualToken() == "(")
            {
                Next();
                parenthesis = true ;
            }
        
            Expression letInExp = new Union();
            letInExp.Analize();

            if(parenthesis)
            {
                if(ActualToken() == ")")
                {
                    Next();
                    type = letInExp.type;
                    idStore.Clear();
                }
                else 
                {
                    throw new SyntaxError("Missing ' ) " , "Missing Token" , "let-in" , Lexer.Tokens[Lexer.index - 1] );
                }
            }
            else
            {
                type = letInExp.type ;
                idStore.Clear();
            }
        }
        public override void Evaluate()
        {
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
                            
                        object idValue = Value.value;

                        if(idStore.ContainsKey(id))
                        {
                            idStore[id] = idValue;
                        }
                        else if(Function.functionsId.ContainsKey(id))
                        {
                            Function.functionsId[id] = idValue;
                        }
                        else idStore.Add(id , idValue );
                            
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

            bool parenthesis = false;
            if(ActualToken() == "(")
            {
                Lexer.index++;
                parenthesis = true ;
            }
        
            Expression letInExp = new Union();
            letInExp.Evaluate();

            object result = letInExp.value ;

            if(parenthesis)
            {
                if(ActualToken() == ")")
                {
                    Lexer.index++;
                    value = result;
                    idStore.Clear();
                }
                else 
                {
                    throw new SyntaxError("Missing ' ) " , "Missing Token" , "let-in" , Lexer.Tokens[Lexer.index - 1] );
                }
            }
            else
            {
                value = result;
                idStore.Clear();
            }
        }  
    }
    
}