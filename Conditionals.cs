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
                    Expression e = new B();//Va una HE
                    //Expression HE = new HulkExpression();
                    if(b.value == "true")
                    {
                        e.Evaluate();
                        value = e.value ;
                        
                        while(Lexer.index < Lexer.Tokens.Count - 1 && Lexer.Tokens[Lexer.index] != ";" )
                        {
                           Next();
                        }
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
                            Expression ElseExp = new B();
                            
                            ElseExp.Evaluate();
                            value = ElseExp.value ;
                        }
                    }
                }
                else
                {
                    throw new SyntaxError("Missing ' ) ' " , "Missing Token" , "Function Declaration" , Lexer.Tokens[Lexer.index - 1]);
                }
            }
            else 
            {
                throw new SyntaxError("Missing ' ( ' " , "Missing Token" , "Function Declaration" , Lexer.Tokens[Lexer.index - 1]);
            }
        }
    }
}