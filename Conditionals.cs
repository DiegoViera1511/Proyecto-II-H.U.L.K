using System.Text.RegularExpressions;

namespace HULK
{
    /// <summary>
    /// Representa las expresiones condicionales if-else
    /// </summary>
    class Conditional : Expression
    {
        /// <summary>
        /// Analiza que sea correcta la sintaxis de if-else
        /// </summary>
        /// <exception cref="ConditionalErrors">Lanza error si la expresión de condición no es booleana</exception>
        /// <exception cref="SyntaxError">Error en la sintaxis en la expresión</exception>
        public override void Analize()
        {
            if( ActualToken() == "(" )
            {
                Next();

                Expression booleanExpression = new Union();
                booleanExpression.Analize();
                if(booleanExpression.type != "boolean")
                {
                    throw new ConditionalErrors(booleanExpression.GetExpType());
                }

                if( ActualToken() == ")" )
                {
                    Next();
        
                    Expression trueExp = new Union();
                    trueExp.Analize();
                        
                    if(ActualToken() == "else")
                    {
                        Next();
                        Expression falseExp = new Union();
                        falseExp.Analize();
                    }
                    else throw new SyntaxError("Missing ' else ' " , "Missing Token" , "if-else" , Lexer.Tokens[Lexer.index - 1]);
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
        /// <summary>
        /// Evalúa la expresión if-else , dándole valor a la expresión
        /// </summary>
        /// <exception cref="ConditionalErrors">Lanza error si la expresión de condición no es booleana</exception>
        /// <exception cref="SyntaxError">Error de sintaxis en la expresión</exception>
        public override void Evaluate()
        {
            if( ActualToken() == "(" )
            {
                Next();

                Expression booleanExpression = new Union();
                booleanExpression.Evaluate();
                if(booleanExpression.GetValue().GetType() != Type.GetType("System.Boolean"))
                {
                    throw new ConditionalErrors(Lexer.TokenType(booleanExpression));
                }

                if( ActualToken() == ")" )
                {
                    Next();
            
                    if((bool)booleanExpression.GetValue())
                    {

                        Expression trueExp = new Union();
                        trueExp.Evaluate();
                        
                        if(ActualToken() == "else")
                        {
                            value = trueExp.value ;
                            
                            while(Lexer.index < Lexer.Tokens.Count - 1 && ActualToken() != ";" && ActualToken() != ")" )
                            {
                                Next();
                            }
                        }
                        else throw new SyntaxError("Missing ' else ' " , "Missing Token" , "if-else" , Lexer.Tokens[Lexer.index - 1]);
                    }
                    else if(!(bool)booleanExpression.GetValue())
                    {
                        while(Lexer.index < Lexer.Tokens.Count - 1  && ActualToken() != "else" ) 
                        {
                            Next();
                        }
                        if(ActualToken() == "else")
                        {
                            Next();

                            Expression falseExp = new Union();
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