using System;
using System.Data.Common;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

namespace HULK
{
    //Abstract classes
    #region Abstract classes
    abstract class Expression
    {
        public Object value ;
        static public void Next()
        {
            Lexer.index++;
        }
        static public string ActualToken()
        {
            return Lexer.Tokens[Lexer.index];
        }
        public abstract void Evaluate();
    }
    abstract class Binary_Exrpessions : Expression
    {
        public Expression left ;
        public string? iDLeft  ;
        public Expression right ;
        public string? iDRight  ;

        public void CatchArgumentTypeError(string idLeft , string leftType , string iDRight , string rightType , string expectedType)
        {
            if(Function.functionsId.ContainsKey(idLeft))
            {
                if(leftType != expectedType)
                {
                    throw new ArgumentTypeError(expectedType , leftType);
                }
            }
            if(Function.functionsId.ContainsKey(iDRight))
            {
                if(rightType != expectedType)
                {
                    throw new ArgumentTypeError(expectedType , rightType);
                }
            }
        }
    }

    #endregion

    class Atom : Expression
    {
        //Atomics expressions ( numbers , strings , booleans )
        public override void Evaluate()
        {
            if(Lexer.IsNumber(ActualToken())) // numbers
            {
                value = Convert.ToDouble(ActualToken());
                Next();
            }
            else if(Lexer.index < Lexer.Tokens.Count && Function.functionsId.ContainsKey(ActualToken())) // function variable 
            {
                var s = Function.functionsId[ActualToken()];
                Next();
                value = s ;
            }
            else if (Lexer.index < Lexer.Tokens.Count && Let_in.idStore.ContainsKey(ActualToken())) // let-in variable
            {
                var s = Let_in.idStore[ActualToken()];
                Next();
                value = s ;
            }
            else if(ActualToken() == "-") // negative numbers
            {
                Next();

                Expression num = new Atom();
                bool isIdfunction = false ;
                if(Function.functionsId.ContainsKey(ActualToken())) isIdfunction = true ;
                num.Evaluate();
                
                if(num.value.Equals(typeof(double)))
                {
                    value = Convert.ToString(-1 * Convert.ToDouble(num.value));
                }
                else 
                {
                    if(isIdfunction)
                    {
                        throw new ArgumentTypeError( "number" , Lexer.TokenType(num.value) );
                    }
                    throw new IncorrectOperator(num.value , "Operator ' - '" , "number");
                }
            }
            else if(ActualToken() == "!")
            {
                Next() ;

                Expression boolean = new Atom() ;

                bool isIdfunction = false ;
                if(Function.functionsId.ContainsKey(ActualToken())) isIdfunction = true ;
                boolean.Evaluate();
                
                if(boolean.value == "true")
                {
                    value = "false" ;
                }
                else if(boolean.value == "false")
                {
                    value = "true" ;
                }
                else 
                {
                    if(isIdfunction)
                    {
                        throw new ArgumentTypeError( "boolean" , Lexer.TokenType(boolean.value) );
                    }
                    throw new IncorrectOperator(boolean.value , "Operator ' ! '" , "boolean");
                }

            }
            else if(Lexer.index < Lexer.Tokens.Count && ActualToken() == "(") // ( expression )
            {
                Next();
                Expression result = new BooleanOperator();
                result.Evaluate();
                string s = result.value;
                if(Lexer.index < Lexer.Tokens.Count && ActualToken() == ")")
                {
                    Next();
                    value = s;
                }
                else
                {
                   throw new SyntaxError("Missing ' ) " , "Missing Token" , "let-in" , Lexer.Tokens[Lexer.index - 1] ) ;
                }

            }
            else if(Lexer.index < Lexer.Tokens.Count && Lexer.IsString(ActualToken()) ) // strings
            {
                value = ActualToken();
                Next();
            }
            else if(Lexer.index < Lexer.Tokens.Count && ActualToken() == "let ") // let-in expressions
            {
                Next();
                Expression l = new Let_in();
                l.Evaluate();
                value = l.value;
            }
            else if(Lexer.index < Lexer.Tokens.Count && ActualToken() == "print") // print expression
            {
                Next();
                Expression p = new Print();
                p.Evaluate();
                value = p.value;
            }
            else if(Lexer.index < Lexer.Tokens.Count && MathExpressions.MathFunctions.Contains(ActualToken())) // Math function expression
            {
                Expression M = new MathExpressions(ActualToken());
                Next();
                M.Evaluate();
                value = M.value ;

            }
            else if(Lexer.index < Lexer.Tokens.Count && FunctionDeclaration.functionStore.ContainsKey(ActualToken())) // function call
            {
                int i = Lexer.index;
                Next();
                FunctionDeclaration.functionStore[Lexer.Tokens[i]].Evaluate();
                value = FunctionDeclaration.functionStore[Lexer.Tokens[i]].value;
            }
            else if(Lexer.index < Lexer.Tokens.Count && ActualToken() == "true") // boolean true
            {
                Next();
                value = "true";
            }
            else if(Lexer.index < Lexer.Tokens.Count && ActualToken() == "false") // boolean false
            {
                Next();
                value = "false";
            }
            else if (ActualToken() == "if") // if-else expression
            {
                Next();
                Expression c = new Conditional();
                c.Evaluate();
                value = c.value;
            }
            else 
            {
                if(Lexer.IsID(ActualToken()))
                {
                    throw new SyntaxError(ActualToken() , "DoNotExistID");
                }
                else
                {
                    throw new UnExpectedToken(ActualToken()) ;
                }
            }
        }
    }
    
    class HulkExpression : Expression
    {
        //Inicio de Expressión
        public override void Evaluate()
        {
            
            if(Lexer.Tokens.Count == 0)// Void Expression
            {
                return;
            }

            if(ActualToken() == "print")
            {
                Next();
                Expression printExp = new Print();
                printExp.Evaluate();
                value = printExp.value;
            }
            else if(ActualToken() == "let ")
            {
                Next();
                Expression letIn = new Let_in();
                letIn.Evaluate();
                value = letIn.value;
            }
            else if(ActualToken() == "if")
            {
                Next();
                Expression ifelse = new Conditional();
                ifelse.Evaluate();
                value = ifelse.value;
            }
            else if(ActualToken() == "function")
            {
                Next();
                Expression f = new FunctionDeclaration();
                f.Evaluate();
                value = "";
            }
            else if(FunctionDeclaration.functionStore.ContainsKey(ActualToken()))
            {

                int i = Lexer.index;
                Next();
                FunctionDeclaration.functionStore[Lexer.Tokens[i]].Evaluate() ;
                value = FunctionDeclaration.functionStore[Lexer.Tokens[i]].value ;
                if(value != "" && value != null)
                {
                    if(Lexer.IsString(value))
                    {
                        Lexer.ConsolePrints.Add(value.Substring( 1 , value.Length - 2));// string value 
                    }
                    else 
                    {
                        Lexer.ConsolePrints.Add(value);
                    }
                }
            }
            else //Atom expression
            {
                Expression exp = new BooleanOperator();
                exp.Evaluate();
                value = exp.value ;
            }
        }
    }  

}