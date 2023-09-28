using System.Text.RegularExpressions;

namespace HULK
{
    class Conditional : Expression
    {
        public override void Evaluate()
        {
            if(Lexer.Tokens[Lexer.index] == "(")
            {
                Next();
                Expression b = new B();
                b.Evaluate();
                if(Lexer.Tokens[Lexer.index] == ")")
                {
                    Next();
                    Expression t = new B();
            
                    if(b.value == "true")
                    {

                        t.Evaluate();
                        
                        if(Lexer.Tokens[Lexer.index] == "else")
                        {
                            value = t.value ;
                            while(Lexer.index < Lexer.Tokens.Count - 1 && Lexer.Tokens[Lexer.index] != ";" )
                            {
                                Next();
                            }
                        }
                        else throw new SyntaxError("Missing ' else ' " , "Missing Token" , "if-else" , Lexer.Tokens[Lexer.index - 1]);
                    }
                    else if(b.value == "false")
                    {
                        while(Lexer.index < Lexer.Tokens.Count - 1  && Lexer.Tokens[Lexer.index] != "else" ) 
                        {
                            Next();
                        }
                        if(Lexer.Tokens[Lexer.index] == "else")
                        {
                            Next();
                            Expression f = new B();
                            f.Evaluate();
                            value = f.value ;
                        }
                    }
                }
                else
                {
                    throw new SyntaxError("Missing ' ) ' " , "Missing Token" , "if-else" , Lexer.Tokens[Lexer.index - 1]);
                }
            }
            else 
            {
                throw new SyntaxError("Missing ' ( ' " , "Missing Token" , "if-else" , Lexer.Tokens[Lexer.index - 1]);
            }
        }
    }
}