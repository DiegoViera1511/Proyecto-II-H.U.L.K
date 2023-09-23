using System;
using System.Data.Common;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

namespace HULK
{
    /*
    public static class Tools
    {
        
    }
    */
    abstract class Expression
    {
        public string value ;

        static public void Next()
        {
            Lexer.index++;
        }
        public abstract void Evaluate();
    }
    abstract class Binary_Exrpessions : Expression
    {
        public Expression left ;

        public Expression right ;
    }

    #region Numeric Expressions
    class N : Binary_Exrpessions
    {   
       private List<string> NextTokens = new List<string>(){";", ")" ,"in",",",">","<","else","<","<=",">=","&","|","==","!="};
        public N()
        {
            this.left = new T();

            this.right = new T();
        }

        #region Methods
        private static string Sum( string a , string b)
        {
            double result = double.Parse(a) + double.Parse(b);

            return Convert.ToString(result);  
        }
        private static string Subtract( string a , string b)
        {
            double result = double.Parse(a) - double.Parse(b);

            return Convert.ToString(result);  
        }

        #endregion

        public override void Evaluate()
        {
            if(Lexer.Tokens[Lexer.index] == "!")
            {
                Next();
                left = new N();
                left.Evaluate();

                if(left.value == null)
                {
                    left.value = "true";//tester
                }   
                if(left.value == "true")
                {
                    left.value = "false";
                }
                else if(left.value == "false")
                {
                    left.value = "true";
                }
                else 
                {
                    //HULK_Errors.SemanticError("Operator '!'", "Incorrect Operator" , Lexer.TokenType(left.value));
                    throw new SemanticError("Operator '!'" , "Incorrect Operator" , Lexer.TokenType(left.value));
                    // return;
                }
            }
            else if(Lexer.Tokens[Lexer.index] == "-")
            {
                Next();
                left = new F();
                left.Evaluate();
                if(left.value == null)
                {
                    left.value = "1";
                }   
                if(Lexer.TokenType(left.value) == "number")
                {
                    left.value = "-" + left.value;
                }
                else 
                {
                    //HULK_Errors.SemanticError("Operator '-'", "Incorrect Operator" , Lexer.TokenType(left.value));
                    throw new SemanticError("Operator '-'" , "Incorrect Operator" , Lexer.TokenType(left.value));
                }
            }
            else left.Evaluate();

            if(left.value == null)
            {
                left.value = "1";//Tester
            }
            //VERIFICACIÓN DE NUMERO CON OPERADOR
            while(Lexer.index < Lexer.Tokens.Count)
            {

                if(Lexer.Tokens[Lexer.index] == "+")
                {
                    
                    Next();
                    right.Evaluate();
                    if(right.value == null)
                    {
                        right.value = "1";
                    }
                    if(Lexer.TokenType(left.value) == "number" && Lexer.TokenType(right.value) == "number")
                    {
                        left.value = Sum(left.value , right.value);
                    }
                    else 
                    {
                        //HULK_Errors.SemanticError("Operator '+'" , "Incorrect Binary Expression" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value));
                        throw new SemanticError("Operator '+'" , "Incorrect Binary Expression" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value) , "number" , Lexer.GetIncorrectToken(left.value , right.value , "number") ) ;
                        //return;
                    }
                }
                else if(Lexer.Tokens[Lexer.index] == "-")
                {
                    Next();
                    right.Evaluate();
                    if(right.value == null)
                    {
                        right.value = "1";//Tester
                    }
                    if(Lexer.TokenType(left.value) == "number" && Lexer.TokenType(right.value) == "number")
                    {
                        left.value = Subtract(left.value , right.value);
                    }
                    else 
                    {
                        //HULK_Errors.SemanticError("Operator '-'" , "Incorrect Binary Expression" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value));
                        throw new SemanticError("Operator '-'" , "Incorrect Binary Expression" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value) , "number" , Lexer.GetIncorrectToken(left.value , right.value , "number") ) ;
                        //return;
                    }
                }
                else if(Lexer.Tokens[Lexer.index] == "@")
                {
                    Next();
                    if(Lexer.IsString(left.value))
                    {
                        left.value = left.value.Substring( 0 , left.value.Length - 1);
                    }
                    Expression literal = new B();
                    literal.Evaluate();
                    if(Lexer.IsString(literal.value))
                    {
                        literal.value = literal.value.Substring( 1 , literal.value.Length - 1 );
                    }

                    value = left.value + Convert.ToString(literal.value);
                    return ;
                    
                }
                else if (NextTokens.Contains(Lexer.Tokens[Lexer.index]))
                {
                    value = Convert.ToString(left.value);
                    break;
                }
                else 
                {
                    //HULK_Errors.UnExpectedToken(Lexer.Tokens[Lexer.index]);
                    throw new UnExpectedToken(Lexer.Tokens[Lexer.index]);
                    //return;
                }
            }
           
        }
    }

    class T : Binary_Exrpessions
    {
       
        //Lexer.Tokens[Lexer.index] == "+" || Lexer.Tokens[Lexer.index] == "-" || Lexer.Tokens[Lexer.index] == ")" || Lexer.Tokens[Lexer.index] == ";" || Lexer.Tokens[Lexer.index] == "in" || Lexer.Tokens[Lexer.index] == "," || Lexer.Tokens[Lexer.index] == "@" || Lexer.Tokens[Lexer.index] == "else" || Lexer.Tokens[Lexer.index] == "<" || Lexer.Tokens[Lexer.index] == ">" || Lexer.Tokens[Lexer.index] == "<=" || Lexer.Tokens[Lexer.index] == ">=" ||  Lexer.Tokens[Lexer.index] == "==" || Lexer.Tokens[Lexer.index] == "!=" || Lexer.Tokens[Lexer.index] == "&" || Lexer.Tokens[Lexer.index] == "|" ;
        private List<string> NextTokens = new List<string>(){";", ")" ,"in",",",">","<","else","<","<=",">=","&","|","==","!=","@","+","-"};
        public T()
        {
            this.left = new W();

            this.right = new W();
        }

        #region Methods
        private static string Multiply( string a , string b)
        {
            double result = double.Parse(a) * double.Parse(b);

            return Convert.ToString(result);  
        }
        private static string Division( string a , string b)
        {
            double result = double.Parse(a) / double.Parse(b);

            return Convert.ToString(result);  
        }
        private static string Modulo( string a , string b)
        {
            double result = double.Parse(a) % double.Parse(b);

            return Convert.ToString(result);  
        }

        #endregion
        public override void Evaluate()
        {
            //Expression result = new W();
            left.Evaluate();
            if(left.value == null)
            {
                left.value = "1";//Tester
            }

            while(Lexer.index < Lexer.Tokens.Count)
            {
                if(Lexer.Tokens[Lexer.index] == "*")
                {   
                    Next();
                    right.Evaluate(); //Verificar que right value es un número
                    if(right.value == null)
                    {
                        right.value = "1";//Tester
                    }
                    if(Lexer.TokenType(left.value) == "number" && Lexer.TokenType(right.value) == "number")
                    {
                        left.value = Multiply(left.value , right.value);
                    }
                    else 
                    {
                        //HULK_Errors.SemanticError("Operator '*'" , "Incorrect Binary Expression" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value));
                        throw new SemanticError("Operator '*'" , "Incorrect Binary Expression" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value) , "number" , Lexer.GetIncorrectToken(left.value , right.value , "number") ) ;
                        //return;
                    }
                }
                else if(Lexer.Tokens[Lexer.index] == "/")
                {
                    Next();
                    right.Evaluate(); //Verificar que right value es un número
                    if(right.value == null)
                    {
                       right.value = "1";//Tester
                    }
                    if(right.value == "0")
                    {
                        throw new DefaultError("DivisionByZero");
                    }

                    if(Lexer.TokenType(left.value) == "number" && Lexer.TokenType(right.value) == "number")
                    {
                        left.value = Division(left.value , right.value);
                    }
                    
                    else 
                    {
                        //HULK_Errors.SemanticError("Operator '/'" , "Incorrect Binary Expression" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value));
                        throw new SemanticError("Operator '/'" , "Incorrect Binary Expression" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value) , "number" , Lexer.GetIncorrectToken(left.value , right.value , "number") ) ;
                        //return;
                    }
                }
                else if(Lexer.Tokens[Lexer.index] == "%")
                {   
                    Lexer.index++;
                    right.Evaluate(); //Verificar que right value es un número
                    if(right.value == null)
                    {
                        right.value = "1";//Tester
                    }
                    if(Lexer.TokenType(left.value) == "number" && Lexer.TokenType(right.value) == "number")
                    {
                        left.value = Modulo(left.value , right.value);
                    }
                    else 
                    {
                        //HULK_Errors.SemanticError("Operator '%'" , "Incorrect Binary Expression" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value));
                        throw new SemanticError("Operator '%'" , "Incorrect Binary Expression" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value) , "number" , Lexer.GetIncorrectToken(left.value , right.value , "number") ) ;
                        //return;
                    }
                    
                }
                else if(NextTokens.Contains(Lexer.Tokens[Lexer.index]))
                {
                    //Siguientes
                    value = Convert.ToString(left.value);
                    break;
                }
                else 
                {
                    //System.Console.WriteLine("200");
                    //HULK_Errors.UnExpectedToken(Lexer.Tokens[Lexer.index]);
                    throw new UnExpectedToken(Lexer.Tokens[Lexer.index]);
                    //return;
                }
            }    

        }
    }

    class W : Binary_Exrpessions 
    {
        //Añadir NextTokens
        private List<string> NextTokens = new List<string>(){";", ")" ,"in",",",">","<","else","<=",">=","&","|","==","!=","@","+","-","*","/","%"};
        public W()
        {
            this.left = new F();

            this.right = new F();
        }

        private static string Pow( string a , string b)
        {
            double result = Math.Pow(double.Parse(a) , double.Parse(b)) ;
            return Convert.ToString(result);  
        }

        public override void Evaluate()
        {
            
            left.Evaluate();
            if(left.value == null)
            {
                left.value = "1";//Tester
            }
            
            while(Lexer.index < Lexer.Tokens.Count)
            {
                if(Lexer.Tokens[Lexer.index] == "^")
                {
                    Next();
                    right.Evaluate();
                    if(right.value == null)
                    {
                        right.value = "1";//Tester
                    }
                    if(Lexer.TokenType(left.value) == "number" && Lexer.TokenType(right.value) == "number")
                    {
                        left.value = Pow(left.value , right.value);
                    }
                    else 
                    {
                        //HULK_Errors.SemanticError("Operator '^'" , "Incorrect Binary Expression" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value));
                        throw new SemanticError("Operator '^'" , "Incorrect Binary Expression" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value) , "number" , Lexer.GetIncorrectToken(left.value , right.value , "number") ) ;
                        //return;
                    }
                }
                else if(NextTokens.Contains(Lexer.Tokens[Lexer.index]))
                {
                    value =  Convert.ToString(left.value);
                    break;
                }
                else 
                {
                    //HULK_Errors.UnExpectedToken(Lexer.Tokens[Lexer.index]);
                    throw new UnExpectedToken(Lexer.Tokens[Lexer.index]);
                    //return;
                }
            }    
            //value = Convert.ToString(left.value);
            //return Convert.ToString(left_value);
        }
    }

    class F : Expression
    {
        //Expresiones atómicas
        public override void Evaluate()
        {
            if(Regex.IsMatch(Lexer.Tokens[Lexer.index] , @"^\d+$"))
            {
                string result = Lexer.Tokens[Lexer.index];
                Next();
                value = result;
                //return result;
            }
            else if(Lexer.index < Lexer.Tokens.Count && Lexer.Tokens[Lexer.index] == "(")
            {
                Next();
                Expression result = new B();//Cambiado N
                result.Evaluate();
                string s = result.value;
                if(Lexer.index < Lexer.Tokens.Count && Lexer.Tokens[Lexer.index] == ")")
                {
                    Next();
                    value = s;
                }
                else
                {
                   //HULK_Errors.SyntaxError("Missing ' ) " , "Missing Token" , "let-in" , Lexer.Tokens[Lexer.index - 1] );
                   throw new SyntaxError("Missing ' ) " , "Missing Token" , "let-in" , Lexer.Tokens[Lexer.index - 1] ) ;
                   //return;
                }

            }
            else if(Lexer.index < Lexer.Tokens.Count && Regex.IsMatch(Lexer.Tokens[Lexer.index], @"(\u0022([^\u0022\\]|\\.)*\u0022)") ) 
            {
                value = Lexer.Tokens[Lexer.index];//Quitar las comillas Lexer.Tokens[Lexer.index].Substring( 1 , Lexer.Tokens[Lexer.index].Length - 2);
                Next();
            }
            else if(Lexer.index < Lexer.Tokens.Count && Regex.IsMatch(Lexer.Tokens[Lexer.index], @"let"))
            {
                Next();
                Expression l = new Let_in();
                l.Evaluate();
                value = l.value;
            }
            else if(Lexer.index < Lexer.Tokens.Count && Lexer.Tokens[Lexer.index] == "print")
            {
                Next();
                Expression p = new Print();
                p.Evaluate();
                value = p.value;
            }
            else if(Lexer.index < Lexer.Tokens.Count && MathExpressions.MathFunctions.Contains(Lexer.Tokens[Lexer.index]))
            {
                Expression M = new MathExpressions(Lexer.Tokens[Lexer.index]);
                Next();
                M.Evaluate();
                value = M.value ;

            }
            else if(Lexer.index < Lexer.Tokens.Count && Function.functions_id.ContainsKey(Lexer.Tokens[Lexer.index]))
            {
                string s = Function.functions_id[Lexer.Tokens[Lexer.index]];
                Next();
                value = s ;
            }
            else if (Lexer.index < Lexer.Tokens.Count && Let_in.id_store.ContainsKey(Lexer.Tokens[Lexer.index]))
            {
                string s = Let_in.id_store[Lexer.Tokens[Lexer.index]];
                Next();
                value = s ;
                //return s ;
            }
            else if(Lexer.index < Lexer.Tokens.Count && Function_Declaration.Function_Store.ContainsKey(Lexer.Tokens[Lexer.index]))
            {
                int i = Lexer.index;
                Next();
                Function_Declaration.Function_Store[Lexer.Tokens[i]].Evaluate();
                value = Function_Declaration.Function_Store[Lexer.Tokens[i]].value;
            }
            else if(Lexer.index < Lexer.Tokens.Count && Lexer.Tokens[Lexer.index] == "true")
            {
                Next();
                value = "true";
            }
            else if(Lexer.index < Lexer.Tokens.Count && Lexer.Tokens[Lexer.index] == "false")
            {
                Next();
                value = "false";
            }
            else if (Lexer.Tokens[Lexer.index] == "if")
            {
                Next();
                Expression c = new Conditional();
                c.Evaluate();
                value = c.value;
            }
            else 
            {
                if(Lexer.IsID(Lexer.Tokens[Lexer.index]))
                {
                   // HULK_Errors.NoID(Lexer.Tokens[Lexer.index] , "DoNotExistID");
                    throw new SyntaxError(Lexer.Tokens[Lexer.index] , "DoNotExistID");
                   // return;
                }
                else
                {
                    //HULK_Errors.UnExpectedToken(Lexer.Tokens[Lexer.index]);
                    throw new UnExpectedToken(Lexer.Tokens[Lexer.index]) ;
                    //return;
                }
            }
        }
    }

    #endregion
    
    class HulkExpression : Expression
    {
        public override void Evaluate()
        {
            
            if(Lexer.Tokens.Count == 0)
            {
                HULK_Errors.Found = true ;
                return;
            }

            if(Lexer.Tokens[Lexer.index] == "print")
            {
                Next();
                Expression p = new Print();
                p.Evaluate();
                value = "";
            }
            else if(Regex.IsMatch(Lexer.Tokens[Lexer.index] , @"let "))
            {
                Next();
                Expression l = new Let_in();
                l.Evaluate();
                value = l.value;
            }
            else if(Lexer.Tokens[Lexer.index] == "if")
            {
                Next();
                Expression ifelse = new Conditional();
                ifelse.Evaluate();
                value = ifelse.value;
            }
            else if(Lexer.Tokens[Lexer.index] == "function")
            {
                Next();
                Expression f = new Function_Declaration();
                f.Evaluate();
                value = "";
            }
            else if(Function_Declaration.Function_Store.ContainsKey(Lexer.Tokens[Lexer.index]))
            {

                int i = Lexer.index;
                Next();
                Function_Declaration.Function_Store[Lexer.Tokens[i]].Evaluate();
                value = Function_Declaration.Function_Store[Lexer.Tokens[i]].value;
                if(value != "" && value != null)
                {
                    if(Regex.IsMatch(value , @"(\u0022([^\u0022\\]|\\.)*\u0022)"))
                    {
                        //Console.WriteLine(value.Substring( 1 , Lexer.Tokens[Lexer.index].Length - 2));
                        Lexer.ConsolePrints.Add(value.Substring( 1 , value.Length - 2));
                    }
                    else 
                    {
                        //Console.WriteLine(value);
                        Lexer.ConsolePrints.Add(value);
                    }
                    
                }
            }
            else
            {
                Expression exp = new B();
                exp.Evaluate();
                value = exp.value ;
            }
        }
    }  
}