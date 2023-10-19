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
        public object value ;

        public string type ;
        static public void Next()
        {
            Lexer.index++;
        }
        static public string ActualToken()
        {
            return Lexer.Tokens[Lexer.index];
        }
        public abstract void Evaluate();

        public abstract void Analize();
    }

    abstract class Binary_Exrpessions : Expression
    {
        public Expression left ;
        public string? iDLeft  ;
        public Expression right ;
        public string? iDRight  ;

        public abstract object Operation(object left , string operatorToken , object right);

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
        public override void Analize()
        {
            if(Lexer.IsNumber(ActualToken())) // numbers
            {
                type = "number";
                Next();
            }
            else if(Lexer.index < Lexer.Tokens.Count && Function.functionsId.ContainsKey(ActualToken())) // function variable 
            {
                string tokenType = (string)Function.functionsId[ActualToken()];
                Next();
                type = tokenType ;
            }
            else if (Lexer.index < Lexer.Tokens.Count && Let_in.idStore.ContainsKey(ActualToken())) // let-in variable
            {
                string tokenType = (string)Let_in.idStore[ActualToken()];
                Next();
                type = tokenType ;
            }
            else if(ActualToken() == "-") // negative numbers
            {
                Next();

                Expression num = new Atom();
                bool isIdfunction = false ;
                if(Function.functionsId.ContainsKey(ActualToken())) isIdfunction = true ;
                num.Analize();
                
                if(num.type == "number")
                {
                    type = "number" ;
                }
                else 
                {
                    if(isIdfunction)
                    {
                        throw new ArgumentTypeError( "number" , num.type );
                    }
                    throw new IncorrectOperator( num.type , "Operator ' - '" , "number");
                }
            }
            else if(ActualToken() == "!")
            {
                Next() ;

                Expression boolean = new Atom() ;

                bool isIdfunction = false ;
                if(Function.functionsId.ContainsKey(ActualToken())) isIdfunction = true ;
                boolean.Analize();
                
                if(boolean.type == "boolean")
                {
                    type = "boolean" ;
                }
                else 
                {
                    if(isIdfunction)
                    {
                        throw new ArgumentTypeError( "boolean" , boolean.type );
                    }
                    throw new IncorrectOperator(boolean.type , "Operator ' ! '" , "boolean");
                }

            }
            else if(Lexer.index < Lexer.Tokens.Count && ActualToken() == "(") // ( expression )
            {
                Next();
                Expression result = new Union();
                result.Analize();
                string x = result.type;
                if(Lexer.index < Lexer.Tokens.Count && ActualToken() == ")")
                {
                    Next();
                    type = x;
                }
                else
                {
                   throw new SyntaxError("Missing ' ) " , "Missing Token" , "let-in" , Lexer.Tokens[Lexer.index - 1] ) ;
                }

            }
            else if(Lexer.index < Lexer.Tokens.Count && Lexer.IsString(ActualToken()) ) // strings
            {
                type = "string" ;
                Next();
            }
            else if(Lexer.index < Lexer.Tokens.Count && ActualToken() == "let ") // let-in expressions
            {
                Next();
                Expression l = new Let_in();
                l.Analize();
                type = l.type;
            }
            else if(Lexer.index < Lexer.Tokens.Count && ActualToken() == "print") // print expression
            {
                Next();
                Expression p = new Print();
                p.Analize();
                type = p.type;
            }
            else if(Lexer.index < Lexer.Tokens.Count && MathExpressions.MathFunctions.Contains(ActualToken())) // Math function expression
            {
                Expression M = new MathExpressions(ActualToken());
                Next();
                M.Analize();
                type = M.type ;
            }
            else if(Lexer.index < Lexer.Tokens.Count && FunctionDeclaration.functionStore.ContainsKey(ActualToken())) // function call
            {
                int i = Lexer.index;
                Next();
                FunctionDeclaration.functionStore[Lexer.Tokens[i]].Analize();
                type = FunctionDeclaration.functionStore[Lexer.Tokens[i]].type;
            }
            else if(Lexer.index < Lexer.Tokens.Count && ActualToken() == "true") // boolean true
            {
                Next();
                type = "boolean" ;
            }
            else if(Lexer.index < Lexer.Tokens.Count && ActualToken() == "false") // boolean false
            {
                Next();
                type = "boolean" ;
            }
            else if (ActualToken() == "if") // if-else expression
            {
                Next();
                Expression c = new Conditional();
                c.Analize();
                type = c.type;
            }
            else if(Lexer.index < Lexer.Tokens.Count && FunctionDeclaration.functionsIdInference.ContainsKey(ActualToken()))
            {
                if(FunctionDeclaration.functionsIdInference[ActualToken()] == "functionId")
                {
                    string functionName = ActualToken();
                    Next();
                    Function.CheckfunctionCall(functionName);
                    type = "inference";
                }
                else if(FunctionDeclaration.functionsIdInference[ActualToken()] == "variable")
                {
                    Next();
                    type = "inference";
                }
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
        public override void Evaluate()
        {
            if(Lexer.IsNumber(ActualToken())) // numbers
            {
                value = Convert.ToDouble(ActualToken());
                Next();
            }
            else if(Lexer.index < Lexer.Tokens.Count && Function.functionsId.ContainsKey(ActualToken())) // function variable 
            {
                var x = Function.functionsId[ActualToken()];
                Next();
                value = x ;
            }
            else if (Lexer.index < Lexer.Tokens.Count && Let_in.idStore.ContainsKey(ActualToken())) // let-in variable
            {
                var x = Let_in.idStore[ActualToken()];
                Next();
                value = x ;
            }
            else if(ActualToken() == "-") // negative numbers
            {
                Next();

                Expression num = new Atom();
                bool isIdfunction = false ;
                if(Function.functionsId.ContainsKey(ActualToken())) isIdfunction = true ;
                num.Evaluate();
                
                if(num.value.GetType() == Type.GetType("System.Double"))
                {
                    value = -1 * (double)num.value ;
                }
                else 
                {
                    if(isIdfunction)
                    {
                        throw new ArgumentTypeError( "number" , Lexer.TokenType(num.value) );
                    }
                    throw new IncorrectOperator(Lexer.TokenType(num.value) , "Operator ' - '" , "number");
                }
            }
            else if(ActualToken() == "!")
            {
                Next() ;

                Expression boolean = new Atom() ;

                bool isIdfunction = false ;
                if(Function.functionsId.ContainsKey(ActualToken())) isIdfunction = true ;
                boolean.Evaluate();
                
                if(Lexer.TokenType(boolean.value) == "boolean" && (bool)boolean.value)
                {
                    value = "false" ;
                }
                else if(Lexer.TokenType(boolean.value) == "boolean" && !(bool)boolean.value)
                {
                    value = "true" ;
                }
                else 
                {
                    if(isIdfunction)
                    {
                        throw new ArgumentTypeError( "boolean" , Lexer.TokenType(boolean.value) );
                    }
                    throw new IncorrectOperator(Lexer.TokenType(boolean.value) , "Operator ' ! '" , "boolean");
                }

            }
            else if(Lexer.index < Lexer.Tokens.Count && ActualToken() == "(") // ( expression )
            {
                Next();
                Expression result = new Union();
                result.Evaluate();
                var x = result.value;
                if(Lexer.index < Lexer.Tokens.Count && ActualToken() == ")")
                {
                    Next();
                    value = x;
                }
                else
                {
                   throw new SyntaxError("Missing ' ) " , "Missing Token" , "let-in" , Lexer.Tokens[Lexer.index - 1] ) ;
                }

            }
            else if(Lexer.index < Lexer.Tokens.Count && Lexer.IsString(ActualToken()) ) // strings
            {
                value = ActualToken().Substring( 1 , ActualToken().Length - 2);
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
                value = true;
            }
            else if(Lexer.index < Lexer.Tokens.Count && ActualToken() == "false") // boolean false
            {
                Next();
                value = false;
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

        public override void Analize()
        {
            if(Lexer.Tokens.Count == 0)// Void Expression
            {
                return;
            }

            if(ActualToken() == "print")
            {
                Next();
                Expression printExp = new Print();
                printExp.Analize();
            }
            else if(ActualToken() == "let ")
            {
                Next();
                Expression letIn = new Let_in();
                letIn.Analize();
            }
            else if(ActualToken() == "if")
            {
                Next();
                Expression ifelse = new Conditional();
                ifelse.Analize();
            }
            else if(ActualToken() == "function")
            {
                Next();
                Expression f = new FunctionDeclaration();
                f.Analize();
            }
            else if(FunctionDeclaration.functionStore.ContainsKey(ActualToken()))
            {
                int i = Lexer.index;
                Next();
                FunctionDeclaration.functionStore[Lexer.Tokens[i]].Analize() ;
            }
            else //Atom expression
            {
                Expression exp = new Union();
                exp.Analize();
            }
        }
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
                
                Lexer.ConsolePrints.Add(value);
              
            }
            else //Atom expression
            {
                Expression exp = new Union();
                exp.Evaluate();
                value = exp.value ;
            }
        }
    }  

}