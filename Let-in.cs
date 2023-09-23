using System.Text.RegularExpressions;

namespace HULK
{
    class Let_in : Expression
    {
        public static Dictionary< string , string> id_store = new Dictionary<string, string>();

        public override void Evaluate()
        {
            while(Lexer.index < Lexer.Tokens.Count)
            {
                if(Lexer.IsID(Lexer.Tokens[Lexer.index]))
                {
                    if(Lexer.Key_Words.Contains(Lexer.Tokens[Lexer.index]))
                    {
                        //HULK_Errors.NoID(Lexer.Tokens[Lexer.index] , "KeyWordID");
                        throw new SyntaxError(Lexer.Tokens[Lexer.index] , "KeyWordID" );
                        //return;
                    }

                    string id = Lexer.Tokens[Lexer.index];
                    Next();

                    if(Lexer.Tokens[Lexer.index] == "=")
                    {
                        Next();
                        if(Lexer.Tokens[Lexer.index] == "in" || Lexer.Tokens[Lexer.index] == ",")
                        {
                            //HULK_Errors.SyntaxError("Missing Expression" , "Missing Token" , "let-in" , $"variable {Lexer.Tokens[Lexer.index - 2]}");
                            throw new SyntaxError("Missing Expression" ,  "Missing Token" , "let-in" , $"variable {Lexer.Tokens[Lexer.index - 2]}" );
                            //return;
                        }

                        Expression Value = new B();
                        Value.Evaluate();

                        if(Value.value == null)
                        {
                            return;
                        }
                            
                        string id_value = Value.value;//cambios

                        if(id_store.ContainsKey(id))
                        {
                            id_store[id] = id_value;
                        }
                        else if(Function.functions_id.ContainsKey(id))
                        {
                            Function.functions_id[id] = id_value;
                        }
                        else id_store.Add(id , id_value );
                            
                    }
                    else
                    {
                        //HULK_Errors.SyntaxError("Missing Token" , "Missing Token" , "let-in" ,Lexer.Tokens[Lexer.index - 1] ) ;
                        throw new SyntaxError("Missing ' = '" , "Missing Token" , "let-in" ,Lexer.Tokens[Lexer.index - 1] );
                        //return;
                    }
                            
                }
                else
                {
                    //HULK_Errors.SyntaxError("Missing ID" , "Missing Token" , "let-in" , Lexer.Tokens[Lexer.index - 1]);
                    throw new SyntaxError("Missing ID" , "Missing Token" , "let-in" , Lexer.Tokens[Lexer.index - 1]);
                   //return;
                }            

                if(Lexer.Tokens[Lexer.index] == ",")
                {
                    Next();
                }
                else if(Lexer.Tokens[Lexer.index] == "in")
                {
                    Next();
                    break;
                }
                else if (Regex.IsMatch(Lexer.Tokens[Lexer.index] , @"^[a-zA-Z]+\w*$"))
                {
                    
                    //HULK_Errors.SyntaxError("Missing ' , '" , "Missing Token" , "let_in" , Lexer.Tokens[Lexer.index - 1]);
                    throw new SyntaxError("Missing ' , '" , "Missing Token" , "let_in" , Lexer.Tokens[Lexer.index - 1]);
                    //return;
                }
                else 
                {
                    //HULK_Errors.SyntaxError("Invalid Token" , "Invalid Token" , "let-in" , Lexer.Tokens[Lexer.index]);
                    throw new SyntaxError("Invalid Token" , "Invalid Token" , "let-in" , Lexer.Tokens[Lexer.index]);
                    //return;
                }

            }    

            bool parenthesis = false;
            if(Lexer.Tokens[Lexer.index] == "(")
            {
                Lexer.index++;
                parenthesis = true ;
            }
        
            //Expression HE = new HulkExpression();
            Expression e = new B();
            e.Evaluate();

            string result = e.value ;
            /*
            if(e.value != null)
            {
                result = e.value;
            }
            else
            {
                HE.Evaluate();
                result = HE.value;
            }
            */

            if(result == null)
            {
                return;
            }

            if(parenthesis)
            {
                if(Lexer.Tokens[Lexer.index] == ")")
                {
                    Lexer.index++;
                    value = result;
                    id_store.Clear();
                }
                else 
                {
                    //HULK_Errors.SyntaxError("Missing ' ) " , "Missing Token" , "let-in" , Lexer.Tokens[Lexer.index - 1] );
                    throw new SyntaxError("Missing ' ) " , "Missing Token" , "let-in" , Lexer.Tokens[Lexer.index - 1] );
                    //return;
                }
            }
            else
            {
                value = result;
                id_store.Clear();
            }
   
        }  
    }
}