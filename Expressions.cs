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
        public string value  ;
       
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

        public override void Evaluate()
        {
            if(Lexer.Tokens[Lexer.index] == "!")
            {
                Lexer.index++;
                left = new N();
                left.Evaluate();

                if(left.value == null)
                {
                    return;
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
                    HULK_Errors.SemanticError("Operator '!'", "Incorrect Operator" , Lexer.TokenType(left.value));
                    return;
                }
            }
            else if(Lexer.Tokens[Lexer.index] == "-")
            {
                Lexer.index++;
                left = new F();
                left.Evaluate();
                if(left.value == null)
                {
                    return;
                }   
                if(Lexer.TokenType(left.value) == "number")
                {
                    left.value = "-" + left.value;
                }
                else 
                {
                    HULK_Errors.SemanticError("Operator '-'", "Incorrect Operator" , Lexer.TokenType(left.value));
                }
            }
            else left.Evaluate();

            if(left.value == null)
            {
                return;
            }
            //VERIFICACIÓN DE NUMERO CON OPERADOR
            while(Lexer.index < Lexer.Tokens.Count)
            {

                if(Lexer.Tokens[Lexer.index] == "+")
                {
                    
                    Lexer.index++;
                    right.Evaluate();
                    if(right.value == null)
                    {
                        return;
                    }
                    if(Lexer.TokenType(left.value) == "number" && Lexer.TokenType(right.value) == "number")
                    {
                        left.value = Sum(left.value , right.value);
                    }
                    else 
                    {
                        HULK_Errors.SemanticError("Operator '+'" , "Incorrect Binary Expression" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value));
                        return;
                    }
                }
                else if(Lexer.Tokens[Lexer.index] == "-")
                {
                    Lexer.index++;
                    right.Evaluate();
                    if(right.value == null)
                    {
                        return;
                    }
                    if(Lexer.TokenType(left.value) == "number" && Lexer.TokenType(right.value) == "number")
                    {
                        left.value = Subtract(left.value , right.value);
                    }
                    else 
                    {
                        HULK_Errors.SemanticError("Operator '-'" , "Incorrect Binary Expression" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value));
                        return;
                    }
                }
                else if(Lexer.Tokens[Lexer.index] == "@")
                {
                    Lexer.index++;
                    if(Regex.IsMatch(left.value , @"(\u0022([^\u0022\\]|\\.)*\u0022)"))
                    {
                        left.value = left.value.Substring( 0 , left.value.Length - 1);
                    }
                    Expression literal = new B();
                    literal.Evaluate();
                    if(Regex.IsMatch(literal.value , @"(\u0022([^\u0022\\]|\\.)*\u0022)"))
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
                    HULK_Errors.UnExpectedToken(Lexer.Tokens[Lexer.index]);
                    return;
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
                return;
            }

            while(Lexer.index < Lexer.Tokens.Count)
            {
                if(Lexer.Tokens[Lexer.index] == "*")
                {   
                    Lexer.index++;
                    right.Evaluate(); //Verificar que right value es un número
                    if(right.value == null)
                    {
                        return;
                    }
                    if(Lexer.TokenType(left.value) == "number" && Lexer.TokenType(right.value) == "number")
                    {
                        left.value = Multiply(left.value , right.value);
                    }
                    else 
                    {
                        HULK_Errors.SemanticError("Operator '*'" , "Incorrect Binary Expression" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value));
                        return;
                    }
                }
                else if(Lexer.Tokens[Lexer.index] == "/")
                {
                    Lexer.index++;
                    right.Evaluate(); //Verificar que right value es un número
                    if(right.value == null)
                    {
                        return;
                    }
                    if(Lexer.TokenType(left.value) == "number" && Lexer.TokenType(right.value) == "number")
                    {
                        left.value = Division(left.value , right.value);
                    }
                    else 
                    {
                        HULK_Errors.SemanticError("Operator '/'" , "Incorrect Binary Expression" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value));
                        return;
                    }
                }
                else if(Lexer.Tokens[Lexer.index] == "%")
                {   
                    Lexer.index++;
                    right.Evaluate(); //Verificar que right value es un número
                    if(right.value == null)
                    {
                        return;
                    }
                    if(Lexer.TokenType(left.value) == "number" && Lexer.TokenType(right.value) == "number")
                    {
                        left.value = Modulo(left.value , right.value);
                    }
                    else 
                    {
                        HULK_Errors.SemanticError("Operator '%'" , "Incorrect Binary Expression" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value));
                        return;
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
                    HULK_Errors.UnExpectedToken(Lexer.Tokens[Lexer.index]);
                    return;
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
                return;
            }
            
            while(Lexer.index < Lexer.Tokens.Count)
            {
                if(Lexer.Tokens[Lexer.index] == "^")
                {
                    Lexer.index++;
                    right.Evaluate();
                    if(right.value == null)
                    {
                        return;
                    }
                    if(Lexer.TokenType(left.value) == "number" && Lexer.TokenType(right.value) == "number")
                    {
                        left.value = Pow(left.value , right.value);
                    }
                    else 
                    {
                        HULK_Errors.SemanticError("Operator '^'" , "Incorrect Binary Expression" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value));
                        return;
                    }
                }
                else if(NextTokens.Contains(Lexer.Tokens[Lexer.index]))
                {
                    value =  Convert.ToString(left.value);
                    break;
                }
                else 
                {
                    HULK_Errors.UnExpectedToken(Lexer.Tokens[Lexer.index]);
                    return;
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
                Lexer.index++;
                value = result;
                //return result;
            }
            else if(Lexer.index < Lexer.Tokens.Count && Lexer.Tokens[Lexer.index] == "(")
            {
                Lexer.index++;
                Expression result = new B();//Cambiado N
                result.Evaluate();
                string s = result.value;
                if(Lexer.index < Lexer.Tokens.Count && Lexer.Tokens[Lexer.index] == ")")
                {
                    Lexer.index++;
                    value = s;
                }
                else
                {
                   HULK_Errors.SyntaxError("Missing ' ) " , "Missing Token" , "let-in" , Lexer.Tokens[Lexer.index - 1] );
                   return;
                }

            }
            else if(Lexer.index < Lexer.Tokens.Count && Regex.IsMatch(Lexer.Tokens[Lexer.index], @"(\u0022([^\u0022\\]|\\.)*\u0022)") ) 
            {
                value = Lexer.Tokens[Lexer.index];//Quitar las comillas Lexer.Tokens[Lexer.index].Substring( 1 , Lexer.Tokens[Lexer.index].Length - 2);
                Lexer.index++;
            }
            else if(Lexer.index < Lexer.Tokens.Count && Regex.IsMatch(Lexer.Tokens[Lexer.index], @"let"))
            {
                Lexer.index++;
                Expression l = new Let_in();
                l.Evaluate();
                value = l.value;
            }
            else if(Lexer.index < Lexer.Tokens.Count && Lexer.Tokens[Lexer.index] == "print")
            {
                Lexer.index++;
                Expression p = new Print();
                p.Evaluate();
                value = p.value;
            }
            else if(Lexer.index < Lexer.Tokens.Count && Function.functions_id.ContainsKey(Lexer.Tokens[Lexer.index]))
            {
                string s = Function.functions_id[Lexer.Tokens[Lexer.index]];
                Lexer.index++;
                value = s ;
            }
            else if (Lexer.index < Lexer.Tokens.Count && Let_in.id_store.ContainsKey(Lexer.Tokens[Lexer.index]))
            {
                string s = Let_in.id_store[Lexer.Tokens[Lexer.index]];
                Lexer.index++;
                value = s ;
                //return s ;
            }
            else if(Lexer.index < Lexer.Tokens.Count && Function_Declaration.Function_Store.ContainsKey(Lexer.Tokens[Lexer.index]))
            {
                int i = Lexer.index;
                Lexer.index++;
                Function_Declaration.Function_Store[Lexer.Tokens[i]].Evaluate();
                value = Function_Declaration.Function_Store[Lexer.Tokens[i]].value;
            }
            else if(Lexer.index < Lexer.Tokens.Count && Lexer.Tokens[Lexer.index] == "true")
            {
                Lexer.index++;
                value = "true";
            }
            else if(Lexer.index < Lexer.Tokens.Count && Lexer.Tokens[Lexer.index] == "false")
            {
                Lexer.index++;
                value = "false";
            }
            else if (Lexer.Tokens[Lexer.index] == "if")
            {
                Lexer.index++;
                Expression c = new Conditional();
                c.Evaluate();
                value = c.value;
            }
            else 
            {
                if(Lexer.IsID(Lexer.Tokens[Lexer.index]))
                {
                    HULK_Errors.NoID(Lexer.Tokens[Lexer.index] , "DoNotExistID");
                    return;
                }
                else
                {
                    HULK_Errors.UnExpectedToken(Lexer.Tokens[Lexer.index]);
                    return;
                }
            }
        }
    }

    #endregion
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
                        HULK_Errors.NoID(Lexer.Tokens[Lexer.index] , "KeyWordID");
                        return;
                    }

                    string id = Lexer.Tokens[Lexer.index];
                    Lexer.index++;

                    if(Lexer.Tokens[Lexer.index] == "=")
                    {
                        Lexer.index++;
                        if(Lexer.Tokens[Lexer.index] == "in" || Lexer.Tokens[Lexer.index] == ",")
                        {
                            HULK_Errors.Found = true;
                            HULK_Errors.SyntaxError("Missing Expression" , "Missing Token" , "let-in" , $"variable {Lexer.Tokens[Lexer.index - 2]}");
                            return;
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
                        HULK_Errors.Found = true;
                        HULK_Errors.SyntaxError("Missing Token" , "Missing Token" , "let-in" ,Lexer.Tokens[Lexer.index - 1] ) ;
                        return;
                    }
                            
                }
                else
                {
                    HULK_Errors.SyntaxError("Missing ID" , "Missing Token" , "let-in" , Lexer.Tokens[Lexer.index - 1]);
                    return;
                }            

                if(Lexer.Tokens[Lexer.index] == ",")
                {
                    Lexer.index++;
                }
                else if(Lexer.Tokens[Lexer.index] == "in")
                {
                    Lexer.index++;
                    break;
                }
                else if (Regex.IsMatch(Lexer.Tokens[Lexer.index] , @"^[a-zA-Z]+\w*$"))
                {
                    HULK_Errors.Found = true ;
                    HULK_Errors.SyntaxError("Missing ' , '" , "Missing Token" , "let_in" , Lexer.Tokens[Lexer.index - 1]);
                    return;
                }
                else 
                {
                    HULK_Errors.Found = true ;
                    HULK_Errors.SyntaxError("Invalid Token" , "Invalid Token" , "let-in" , Lexer.Tokens[Lexer.index]);
                    return;
                }

            }    

            bool parenthesis = false;
            if(Lexer.Tokens[Lexer.index] == "(")
            {
                Lexer.index++;
                parenthesis = true ;
            }
        
            Expression HE = new HulkExpression();
            Expression e = new B();

            e.Evaluate();

            string result ;

            if(e.value != null)
            {
                result = e.value;
            }
            else
            {
                HE.Evaluate();
                result = HE.value;
            }

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
                    HULK_Errors.SyntaxError("Missing ' ) " , "Missing Token" , "let-in" , Lexer.Tokens[Lexer.index - 1] );
                    return;
                }
            }
            else
            {
                value = result;
                id_store.Clear();
            }
   
        }  
    }

    class Print : Expression
    {
        public override void Evaluate()
        {
            if(Lexer.Tokens[Lexer.index] == "(")
            {
                Lexer.index++;
                if(Lexer.Tokens[Lexer.index] == ")")
                {
                    Lexer.index++;
                    Console.WriteLine();
                    return;
                }

                Expression x = new B();
                x.Evaluate();
                if(x.value == null)
                {
                    return;
                }

                if(Lexer.Tokens[Lexer.index] == ")")
                {
                    Lexer.index++;
                    if(Regex.IsMatch(x.value , @"(\u0022([^\u0022\\]|\\.)*\u0022)"))
                    {
                        //Console.WriteLine(x.value.Substring( 1 , x.value.Length - 2));
                        Lexer.ConsolePrints.Add(x.value.Substring( 1 , x.value.Length - 2));
                        
                    }
                    else 
                    {
                        //Console.WriteLine(x.value);
                        Lexer.ConsolePrints.Add(x.value);
                    }
                    
                    value = x.value;
                }
                else
                {
                    HULK_Errors.Found = true;
                    HULK_Errors.SyntaxError("Missing ' ) '" , "Missing Token" , "print" , Lexer.Tokens[Lexer.index-1] );
                    return;
                }
            }
            else
            {
                HULK_Errors.Found = true;
                HULK_Errors.SyntaxError("Missing ' ( '" , "Missing Token" , "print" , Lexer.Tokens[Lexer.index-1] );
                return;
            }
           
        }
    }
  
    #region Booleans
    class B : Binary_Exrpessions //Logic boolean operators
    {
        List<string> NextTokens = new List<string>(){")",";",",","in","else"};
        public B()
        {
            this.left = new A();

            this.right = new A();
        }
        
        #region Methods
        private static string And(string a , string b)
        {
            bool left = a == "true" ? true : false ;
            bool right = b == "true" ? true : false ;

            if(left && right)
            {
                return "true" ;
            }
            else return "false" ;
        }
        private static string Or(string a , string b)
        {
            bool left = a == "true" ? true : false ;
            bool right = b == "true" ? true : false ;

            if(left || right)
            {
                return "true" ;
            }
            else return "false" ;
        }
        #endregion

        public override void Evaluate()
        {
            
            left.Evaluate();

            if(left.value == null)
            {
                return;
            }

            while(Lexer.index < Lexer.Tokens.Count)
            {
                if(Lexer.Tokens[Lexer.index] == "&")
                {
                    Lexer.index++;
                    right.Evaluate();
                    if(right.value == null)
                    {
                        return;
                    }

                    if(Lexer.TokenType(left.value) == "boolean" && Lexer.TokenType(right.value) == "boolean")
                    {
                        left.value = And(left.value , right.value);
                    }
                    else 
                    {
                        HULK_Errors.SemanticError("Operator ' & '" , "Incorrect Binary Expression" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value));
                        return;
                    }
                    
                }
                else if(Lexer.Tokens[Lexer.index] == "|")
                {
    
                    Lexer.index++;
                    right.Evaluate();
                    if(right.value == null)
                    {
                        return;
                    }

                    if(Lexer.TokenType(left.value) == "boolean" && Lexer.TokenType(right.value) == "boolean")
                    {
                        left.value = Or(left.value , right.value);
                    }
                    else 
                    {
                        HULK_Errors.SemanticError("Operator ' | '" , "Incorrect Binary Expression" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value));
                        return;
                    }
                    
                }
                else if(NextTokens.Contains(Lexer.Tokens[Lexer.index]))
                {
                    value = left.value;
                    break;
                }
                else 
                {
                    HULK_Errors.UnExpectedToken(Lexer.Tokens[Lexer.index]);
                    return;
                }
            }
            
        }

    }
    class A : Binary_Exrpessions //Boolean comparison
    {
        List<string> NextTokens = new List<string>(){")",";",",","in","else","&","|"};
        public A()
        {
            left = new N();

            right = new N();
        }

        #region Methods
        private static string GreaterThan(string a , string b)
        {
            return double.Parse(a) > double.Parse(b) ? "true" : "false";
        }

        private static string LessThan(string a , string b)
        {
            return double.Parse(a) < double.Parse(b) ? "true" : "false";
        }

        private static string GreaterThanOrEqual(string a , string b)
        {
            return double.Parse(a) >= double.Parse(b) ? "true" : "false";
        }

        private static string LessThanOrEqual(string a , string b)
        {
            return double.Parse(a) <= double.Parse(b) ? "true" : "false";
        }

        private static string Equals(string a , string b)
        {
            return a == b ? "true" : "false";
        }

        private static string Inequality(string a , string b)
        {
            return a != b ? "true" : "false";
        }

        #endregion
        public override void Evaluate()
        {   
           
            left.Evaluate();

            if(left.value == null)
            {
                return;
            }
            while(Lexer.index < Lexer.Tokens.Count)
            {

            
                if(Lexer.Tokens[Lexer.index] == ">")
                {
                    Lexer.index++;
                    right.Evaluate();
                    if(right.value == null)
                    {
                        return;
                    }
                    if(Lexer.TokenType(left.value) == "number" && Lexer.TokenType(right.value) == "number")
                    {
                        left.value = GreaterThan(left.value , right.value);
                    }
                    else 
                    {
                        HULK_Errors.SemanticError("Operator ' > '" , "Incorrect Binary Expression" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value));
                        return;
                    }
                }
                else if(Lexer.Tokens[Lexer.index] == "<")
                {
                    Lexer.index++;
                    right.Evaluate();
                    if(right.value == null)
                    {
                        return;
                    }
                    if(Lexer.TokenType(left.value) == "number" && Lexer.TokenType(right.value) == "number")
                    {
                        left.value = LessThan(left.value , right.value);
                    }
                    else 
                    {
                        HULK_Errors.SemanticError("Operator ' < '" , "Incorrect Binary Expression" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value));
                        return;
                    }
                }
                else if(Lexer.Tokens[Lexer.index] == "<=")
                {
                    Lexer.index++;
                    right.Evaluate();
                    if(right.value == null)
                    {
                        return;
                    }
                    if(Lexer.TokenType(left.value) == "number" && Lexer.TokenType(right.value) == "number")
                    {
                        left.value = LessThanOrEqual(left.value , right.value);
                    }
                    else 
                    {
                        HULK_Errors.SemanticError("Operator ' <= '" , "Incorrect Binary Expression" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value));
                        return;
                    }
                }
                else if(Lexer.Tokens[Lexer.index] == ">=")
                {
                    Lexer.index++;
                    right.Evaluate();
                    if(right.value == null)
                    {
                        return;
                    }
                    if(Lexer.TokenType(left.value) == "number" && Lexer.TokenType(right.value) == "number")
                    {
                        left.value = GreaterThanOrEqual(left.value , right.value);
                    }
                    else 
                    {
                        HULK_Errors.SemanticError("Operator ' >= '" , "Incorrect Binary Expression" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value));
                        return;
                    }
                }
                else if(Lexer.Tokens[Lexer.index] == "==")
                {
                    Lexer.index++;
                    right.Evaluate();
                    if(right.value == null)
                    {
                        return;
                    }
                    if(Lexer.TokenType(left.value) ==  Lexer.TokenType(right.value))
                    {
                        left.value = Equals(left.value , right.value);
                    }
                    else
                    {
                        HULK_Errors.SemanticError("Operator ' == '" , "Incorrect Binary Expression" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value));
                        return;
                    }
                    
                }
                else if(Lexer.Tokens[Lexer.index] == "!=")
                {
                    Lexer.index++;
                    right.Evaluate();
                    if(right.value == null)
                    {
                        return;
                    }
                    if(Lexer.TokenType(left.value) ==  Lexer.TokenType(right.value))
                    {
                        left.value = Inequality(left.value , right.value);
                    }
                    else
                    {
                        HULK_Errors.SemanticError("Operator ' != '" , "Incorrect Binary Expression" , Lexer.TokenType(left.value) , Lexer.TokenType(right.value));
                        return;
                    }
                    
                }
                else if (NextTokens.Contains(Lexer.Tokens[Lexer.index]))
                {
                    value = left.value ;
                    break;
                }
                else 
                {
                    HULK_Errors.UnExpectedToken(Lexer.Tokens[Lexer.index]);    
                    return;
                }
            }
            /*
            if (Regex.IsMatch(left.value , @"(\u0022([^\u0022\\]|\\.)*\u0022)"))
            {
                if(Lexer.Tokens[Lexer.index] == "==")
                {
                    Lexer.index++;
                    right.Evaluate();
                    if(Regex.IsMatch(right.value , @"^\d+$"))
                    {
                        double right_value = double.Parse(right.value) ;

                        if(left.value == right.value)
                        {
                            value = "true";
                        }
                        else value = "false";
                    }
                }
                else if(Lexer.Tokens[Lexer.index] == "!=")
                {
                    Lexer.index++;
                    right.Evaluate();
                    if(Regex.IsMatch(right.value , @"^\d+$"))
                    {
                
                        if(left.value != right.value)
                        {
                            value = "true";
                        }
                        else value = "false";
                    }
                }
                else if (Lexer.Tokens[Lexer.index] == ")" || Lexer.Tokens[Lexer.index] == ";" || Lexer.Tokens[Lexer.index] == "," || Lexer.Tokens[Lexer.index] == "in"  || Lexer.Tokens[Lexer.index] == "else" )
                {
                    value = left.value ;
                }
                else System.Console.WriteLine("error");

            }
            else if(left.value == "true" || left.value == "false")
            {
                value = left.value ;
            }
            else if (Lexer.Tokens[Lexer.index] == ")" || Lexer.Tokens[Lexer.index] == ";" || Lexer.Tokens[Lexer.index] == "," || Lexer.Tokens[Lexer.index] == "in"  || Lexer.Tokens[Lexer.index] == "else")
            {
                value = left.value ;
            }
            else 
            {
                System.Console.WriteLine("No es una expresión booleana " + Lexer.Tokens[Lexer.index]); 
                return;
            }
            */
        }
    }   
    #endregion

    class Function_Declaration : Expression
    {
        public static Dictionary < string , Function > Function_Store = new Dictionary<string, Function>();
        public static Dictionary < string , int > Function_Stack = new Dictionary<string, int>();

        public override void Evaluate()
        {
            
            List<string> function_Arguments = new List<string>();
            List<string> function_Expression = new List<string>();

            if(Lexer.IsID(Lexer.Tokens[Lexer.index]))
            {
                string function_id = Lexer.Tokens[Lexer.index];
                Lexer.index++;
                if(Lexer.Tokens[Lexer.index] == "(")
                {
                    Lexer.index++;
                    
                    while(Lexer.index < Lexer.Tokens.Count && Lexer.Tokens[Lexer.index] != ")")
                    {
                        if(Lexer.IsID(Lexer.Tokens[Lexer.index]))
                        {
                            if(!function_Arguments.Contains(Lexer.Tokens[Lexer.index]))
                            {
                                function_Arguments.Add(Lexer.Tokens[Lexer.index]);
                                Lexer.index++;
                            }
                            else
                            {
                                HULK_Errors.Found = true ;
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("El parámetro " + Lexer.Tokens[Lexer.index] + " está duplicado");
                                Console.ForegroundColor = ConsoleColor.Green;
                                return;
                            }
                        }
                        else 
                        {
                            HULK_Errors.SyntaxError("Missing ID" , "Missing Token" , "let-in" , Lexer.Tokens[Lexer.index - 1]);
                            return;
                        }
                        if(Lexer.Tokens[Lexer.index] != ",")
                        {
                            break;
                        }
                        else Lexer.index++;
                    }
                    if(Lexer.Tokens[Lexer.index] == ")")
                    {
                        Lexer.index++;
                        if(Lexer.Tokens[Lexer.index] == "=>")
                        {
                            Lexer.index++;
                            while(Lexer.Tokens[Lexer.index] != ";" && Lexer.index < Lexer.Tokens.Count)
                            {
                                if(Lexer.IsID(Lexer.Tokens[Lexer.index]) && !Lexer.Key_Words.Contains(Lexer.Tokens[Lexer.index]) && !function_Arguments.Contains(Lexer.Tokens[Lexer.index]) && Lexer.Tokens[Lexer.index] != function_id && !Function_Store.ContainsKey(Lexer.Tokens[Lexer.index]) && !Let_in.id_store.ContainsKey(Lexer.Tokens[Lexer.index]))
                                {
                                    HULK_Errors.NoID(Lexer.Tokens[Lexer.index] , "DoNotExistID");
                                    return;
                                }
                                function_Expression.Add(Lexer.Tokens[Lexer.index]);
                                Lexer.index++;
                            }

                            if(Lexer.index < Lexer.Tokens.Count && Lexer.Tokens[Lexer.index] == ";" )
                            {
                                function_Expression.Add(Lexer.Tokens[Lexer.index]);
                            }
                            else return;
                        
                            if(Function_Store.ContainsKey(function_id))
                            {
                                Function_Store[function_id] = new Function(function_Arguments , function_Expression , function_id);
                            }
                            else
                            {
                                Function_Store.Add(function_id , new Function(function_Arguments , function_Expression , function_id));
                                Function_Stack.Add(function_id , 0);
                            }
                        }
                        else 
                        {
                            HULK_Errors.SyntaxError("Missing ' => " , "Missing Token" , "Function Declaration" , Lexer.Tokens[Lexer.index - 1]);
                            return;
                        }
                    }
                    else 
                    {
                        HULK_Errors.SyntaxError("Missing ' ) " , "Missing Token" , "Function Declaration" , Lexer.Tokens[Lexer.index - 1]);
                        return;
                    }
                }
            }
        }
    }
    class Function : Expression
    {
        private string function_name ;
        public List<string> Arguments_id = new List<string>();
        public List<string> Arguments_value = new List<string>();
        public List<string> function_Expression = new List<string>();
        public static Dictionary<string , string > functions_id = new Dictionary<string, string>();

        public Function(List<string> function_Arguments_id , List<string> Expression , string name)
        {
            this.Arguments_id = function_Arguments_id;
            this.function_Expression = Expression;
            this.function_name = name ;
        }
        public override void Evaluate()
        {
            if(Function_Declaration.Function_Stack[function_name] > 1000)
            {
                HULK_Errors.Found = true ;
                Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine("Stack Overflow " + function_name);
                Console.ForegroundColor = ConsoleColor.Green;
                return ;
            }
            else Function_Declaration.Function_Stack[function_name]++;

            if(Lexer.Tokens[Lexer.index] == "(")
            {
                string result ;
                Lexer.index++;

                Expression e = new B();

                Arguments_value.Clear();

                Dictionary<string , string> Original_values = new Dictionary<string, string>();
                while(Lexer.index < Lexer.Tokens.Count && Lexer.Tokens[Lexer.index] != ")")
                {
                    e.Evaluate();
                    Arguments_value.Add(e.value);
                    if(Lexer.Tokens[Lexer.index] != ",")
                    {
                        break;
                    }
                    else Lexer.index++;
                }
                if(Arguments_id.Count == Arguments_value.Count)
                {
                    for(int i = 0 ; i < Arguments_id.Count ; i++)
                    {
                        if(functions_id.ContainsKey(Arguments_id[i]))
                        {
                            Original_values.Add(Arguments_id[i] , functions_id[Arguments_id[i]]);
                            //Actualiza
                            functions_id[Arguments_id[i]] = Arguments_value[i];
                        }
                        else
                        {
                            functions_id.Add(Arguments_id[i] , Arguments_value[i]);
                        }
                    }
                }
                else 
                {
                    HULK_Errors.Found = true ;
                    Console.ForegroundColor = ConsoleColor.Red;
                    System.Console.WriteLine($"Function '{function_name}' receives {Arguments_id.Count} argument(s), but {Arguments_value.Count} were given.");
                    Console.ForegroundColor = ConsoleColor.Green;
                    return;
                }
                
                List<string> Originals_Tokens = Lexer.Tokens;
                int Original_index = Lexer.index;

                Lexer.Tokens = function_Expression;
                Lexer.index = 0;

                Expression FE = new B();
                FE.Evaluate();
                
                result = FE.value;
                
                Lexer.Tokens = Originals_Tokens;
                Lexer.index = Original_index;

                foreach(string s in functions_id.Keys)
                {
                    if(Original_values.ContainsKey(s))
                    {
                        functions_id[s] = Original_values[s];
                    }
                }

                if(result == null)
                {
                    Lexer.index++;
                    Arguments_value.Clear();
                    return ;
                }

                if(Lexer.Tokens[Lexer.index] == ")")
                {
                    value = result ;
                    Lexer.index++;
                    Arguments_value.Clear();
                    Function_Declaration.Function_Stack[function_name]--;
                }
                else 
                {
                    HULK_Errors.SyntaxError("Missing ' ) " , "Missing Token" , "Function Declaration" , Lexer.Tokens[Lexer.index - 1]);
                    return;
                }

            }
        }
    }

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
                    Expression HE = new HulkExpression();
                    string result ;
                    if(b.value == "true")
                    {
                        e.Evaluate();
                        if(e.value == null)
                        {
                            return;
                        }
                        else value = e.value ;
                        
                        while(Lexer.index < Lexer.Tokens.Count - 1 && Lexer.Tokens[Lexer.index] != ";" )
                        {
                            Lexer.index++;
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
                            Expression two = new B();
                            Expression HE2 = new HulkExpression();
                            two.Evaluate();
                            if(two.value == null)
                            {
                                HE2.Evaluate();
                                result = HE2.value;
                            }
                            else result = two.value;

                            value = result ;
                        }
                    }
                }
            }
        }
    }
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
                Lexer.index++;
                Expression p = new Print();
                p.Evaluate();
                value = "";
            }
            else if(Regex.IsMatch(Lexer.Tokens[Lexer.index] , @"let "))
            {
                Lexer.index++;
                Expression l = new Let_in();
                l.Evaluate();
                value = l.value;
            }
            else if(Lexer.Tokens[Lexer.index] == "if")
            {
                Lexer.index++;
                Expression ifelse = new Conditional();
                ifelse.Evaluate();
                value = ifelse.value;
            }
            else if(Lexer.Tokens[Lexer.index] == "function")
            {
                Lexer.index++;
                Expression f = new Function_Declaration();
                f.Evaluate();
                value = "";
            }
            else if(Function_Declaration.Function_Store.ContainsKey(Lexer.Tokens[Lexer.index]))
            {

                int i = Lexer.index;
                Lexer.index++;
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
        }
    }  
}