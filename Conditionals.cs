using System.Text.RegularExpressions;

namespace HULK
{
    class Conditional : Expression
    {
        public override void Evaluate()
        {
            if( ActualToken() == "(" )
            {
                Next();

                Expression booleanExpression = new BooleanOperator();
                booleanExpression.Evaluate();
                if(!(Convert.ToString(booleanExpression.value.GetType()) == "System.Boolean"))
                {
                    throw new ConditionalErrors(Lexer.TokenType(booleanExpression));
                }

                if( ActualToken() == ")" )
                {
                    Next();
            
                    if((bool)booleanExpression.value)
                    {

                        Expression trueExp = new BooleanOperator();
                        trueExp.Evaluate();
                        
                        if(ActualToken() == "else")
                        {
                            value = trueExp.value ;
                            
                            while(Lexer.index < Lexer.Tokens.Count - 1 && ActualToken() != ";" )
                            {
                                Next();
                            }
                        }
                        else throw new SyntaxError("Missing ' else ' " , "Missing Token" , "if-else" , Lexer.Tokens[Lexer.index - 1]);
                    }
                    else if(!(bool)booleanExpression.value)
                    {
                        while(Lexer.index < Lexer.Tokens.Count - 1  && ActualToken() != "else" ) 
                        {
                            Next();
                        }
                        if(ActualToken() == "else")
                        {
                            Next();

                            Expression falseExp = new BooleanOperator();
                            falseExp.Evaluate(); 

                            value = falseExp.value ;
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