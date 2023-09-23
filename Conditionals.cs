using System.Text.RegularExpressions;

namespace HULK
{
    class Conditional : Expression
    {
        public override void Evaluate()
        {
            if(Lexer.Tokens[Lexer.index] == "(")
            {
                Lexer.index++;
                Expression b = new B();
                b.Evaluate();
                if(Lexer.Tokens[Lexer.index] == ")")
                {
                    Lexer.index++;
                    Expression e = new B();//Va una HE
                    //Expression HE = new HulkExpression();
                    if(b.value == "true")
                    {
                        e.Evaluate();
                        value = e.value ;
                        
                        while(Lexer.index < Lexer.Tokens.Count - 1 && Lexer.Tokens[Lexer.index] != "else" )
                        {
                            Lexer.index++;
                        }
                        if(Lexer.Tokens[Lexer.index] == "else")
                        {
                            Lexer.index++;
                            Expression checkElse = new B();
                            checkElse.Evaluate();
                        }
                    }
                    else if(b.value == "false")
                    {
                        while(Lexer.index < Lexer.Tokens.Count - 1  && Lexer.Tokens[Lexer.index] != "else" ) 
                        {
                            Lexer.index++;
                        }
                        if(Lexer.Tokens[Lexer.index] == "else")
                        {
                            Lexer.index++;
                            Expression ElseExp = new B();
                            //Expression HE2 = new HulkExpression();
                            
                            ElseExp.Evaluate();
                            /*
                            if(two.value == null)
                            {
                                HE2.Evaluate();
                                result = HE2.value;
                            }
                            else result = two.value;
                            */
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