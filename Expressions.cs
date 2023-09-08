using System;
using System.Data.Common;
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
        public N()
        {
            this.left = new T();

            this.right = new T();
        }

        public override void Evaluate()
        {
            //Expression result = new T();
            left.Evaluate();
            if(left.value == null)
            {
                return;
            }
            else if( !Regex.IsMatch(left.value , @"\d+") )
            {
                //Construir Lexer.index >= Lexer.Tokens.Count ||
                if(Lexer.Tokens[Lexer.index] == ")" || Lexer.Tokens[Lexer.index] == ";" || Lexer.Tokens[Lexer.index] == "in" || Lexer.Tokens[Lexer.index] == ",")
                {
                    value = left.value ;
                    return;
                }
                if(Lexer.Tokens[Lexer.index] == "@")
                {
                    Lexer.index++;
                    Expression literal = new N();
                    literal.Evaluate();
                    value = left.value + Convert.ToString(literal.value);
                    return ;
                }
            }

            double left_value = double.Parse(left.value);

            while(Lexer.index < Lexer.Tokens.Count)
            {
                if(Lexer.Tokens[Lexer.index] == "+")
                {
                    Lexer.index++;
                    right.Evaluate(); //Verificar que right value es un número
                    left_value += double.Parse(right.value);
                }
                else if(Lexer.Tokens[Lexer.index] == "-")
                {
                    Lexer.index++;
                    right.Evaluate();
                    left_value -= double.Parse(right.value);
                }
                else if(Lexer.Tokens[Lexer.index] == "@")
                {
                    Lexer.index++;
                    Expression literal = new N();
                    literal.Evaluate();
                    value = left_value + Convert.ToString(literal.value);
                    return;
                }
                else if (Lexer.Tokens[Lexer.index] == ";" || Lexer.Tokens[Lexer.index] == ")" ||Lexer.Tokens[Lexer.index] == "in" || Lexer.Tokens[Lexer.index] == "," || Lexer.Tokens[Lexer.index] == ">" || Lexer.Tokens[Lexer.index] == "else"
                || Lexer.Tokens[Lexer.index] == "<" || Lexer.Tokens[Lexer.index] == "<=" || Lexer.Tokens[Lexer.index] == ">=" || Lexer.Tokens[Lexer.index] == "&" || Lexer.Tokens[Lexer.index] == "|" || Lexer.Tokens[Lexer.index] == "==" || Lexer.Tokens[Lexer.index] == "!=") //Siguientes
                {
                    break;
                }
            }
            value = Convert.ToString(left_value);
            //return Convert.ToString(left_value);
        }
    }

    class T : Binary_Exrpessions
    {
        public T()
        {
            this.left = new W();

            this.right = new W();
        }
        public override void Evaluate()
        {
            //Expression result = new W();
            left.Evaluate();
            if(left.value == null)
            {
                return;
            }
            else if(!Regex.IsMatch(left.value , @"\d+") )
            {
                value = left.value;
                return ;
            }
            double left_value = double.Parse(left.value);
            while(Lexer.index < Lexer.Tokens.Count)
            {
                if(Lexer.Tokens[Lexer.index] == "*")
                {
                    Lexer.index++;
                    //Expression right = new W();
                    right.Evaluate();
                    left_value *= double.Parse(right.value);
                }
                else if(Lexer.Tokens[Lexer.index] == "/")
                {
                    Lexer.index++;
                    //Expression right = new W();
                    right.Evaluate();
                    left_value /= double.Parse(right.value);
                }
                else if(Lexer.Tokens[Lexer.index] == "%")
                {
                    Lexer.index++;
                    //Expression right = new W();
                    right.Evaluate();
                    left_value %= double.Parse(right.value);
                }
                else if(Lexer.Tokens[Lexer.index] == "+" || Lexer.Tokens[Lexer.index] == "-" || Lexer.Tokens[Lexer.index] == ")" || Lexer.Tokens[Lexer.index] == ";" || Lexer.Tokens[Lexer.index] == "in" || Lexer.Tokens[Lexer.index] == "," || Lexer.Tokens[Lexer.index] == "@" || Lexer.Tokens[Lexer.index] == "else"
                || Lexer.Tokens[Lexer.index] == "<" || Lexer.Tokens[Lexer.index] == ">" || Lexer.Tokens[Lexer.index] == "<=" || Lexer.Tokens[Lexer.index] == ">=" ||  Lexer.Tokens[Lexer.index] == "==" || Lexer.Tokens[Lexer.index] == "!=" || Lexer.Tokens[Lexer.index] == "&" || Lexer.Tokens[Lexer.index] == "|")
                {
                    //Siguientes
                    break;
                }
            }    
            value = Convert.ToString(left_value);
            //return Convert.ToString(left_value);
        }
    }


    class W : Binary_Exrpessions 
    {
        public W()
        {
            this.left = new F();

            this.right = new F();
        }

        public override void Evaluate()
        {
            //Expression result = new F();
            left.Evaluate();
            if(left.value == null)
            {
                return;
            }
            else if(!Regex.IsMatch(left.value , @"\d+") )
            {
                value = left.value;
                return ;
            }

            double left_value = double.Parse(left.value);

            while(Lexer.index < Lexer.Tokens.Count)
            {
                if(Lexer.Tokens[Lexer.index] == "^")
                {
                    Lexer.index++;
                    //Expression right = new W();
                    right.Evaluate();
                    left_value = Math.Pow(left_value , double.Parse(right.value));
                }
                else break;
               
            }    
            value = Convert.ToString(left_value);
            //return Convert.ToString(left_value);
        }
    }

    class F : Expression
    {
        //Expresiones atómicas
        public override void Evaluate()
        {
            if(Regex.IsMatch(Lexer.Tokens[Lexer.index] , @"\d"))
            {
                string result = Lexer.Tokens[Lexer.index];
                Lexer.index++;
                value = result;
                //return result;
            }
            else if(Lexer.index < Lexer.Tokens.Count && Lexer.Tokens[Lexer.index] == "(")
            {
                Lexer.index++;
                Expression result = new N();
                result.Evaluate();
                string s = result.value;
                if(Lexer.index < Lexer.Tokens.Count && Lexer.Tokens[Lexer.index] == ")")
                {
                    Lexer.index++;
                    value = s;
                }
                else
                {
                    throw new Exception("Se esperaba )");
                }

            }
            else if(Lexer.index < Lexer.Tokens.Count && Regex.IsMatch(Lexer.Tokens[Lexer.index], @"(\u0022([^\u0022\\]|\\.)*\u0022)") ) 
            {
                value = Lexer.Tokens[Lexer.index].Substring( 1 , Lexer.Tokens[Lexer.index].Length - 2);//Quitar las comillas !
                Lexer.index++;
            }
            else if(Lexer.index < Lexer.Tokens.Count && Regex.IsMatch(Lexer.Tokens[Lexer.index], @"let"))
            {
                Lexer.index++;
                Expression l = new let_in();
                l.Evaluate();
                value = l.value;
            }
            else if(Lexer.index < Lexer.Tokens.Count && Function.functions_id.ContainsKey(Lexer.Tokens[Lexer.index]))
            {
                string s = Function.functions_id[Lexer.Tokens[Lexer.index]];
                Lexer.index++;
                value = s ;
            }
            else if (Lexer.index < Lexer.Tokens.Count && let_in.id_store.ContainsKey(Lexer.Tokens[Lexer.index]))
            {
                string s = let_in.id_store[Lexer.Tokens[Lexer.index]];
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
        }
    }

    #endregion
    class let_in : Expression
    {
        public static Dictionary< string , string> id_store = new Dictionary<string, string>();

        public override void Evaluate()
        {
            while(Lexer.index < Lexer.Tokens.Count)
            {
                if(Regex.IsMatch(Lexer.Tokens[Lexer.index] , @"^[a-zA-Z]+\w*$"))
                {
                    if(Lexer.Key_Words.Contains(Lexer.Tokens[Lexer.index]))
                    {
                        System.Console.WriteLine("error token inválido");
                        return;
                    }
                    string id = Lexer.Tokens[Lexer.index];
                    Lexer.index++;
                    if(Lexer.Tokens[Lexer.index] == "=")
                    {
                        Lexer.index++;
                        Expression Value = new N();
                        Value.Evaluate();
                        
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
                }
                if(Lexer.Tokens[Lexer.index] != ",")
                {
                    break;
                }
                else Lexer.index++;

            }    
            if(Lexer.Tokens[Lexer.index] == "in")
            {
                Lexer.index++;
                bool parenthesis = false;
                if(Lexer.Tokens[Lexer.index] == "(")
                {
                    Lexer.index++;
                    parenthesis = true ;
                }
                Expression HE = new HulkExpression();
                HE.Evaluate();

                string result ;
                if(HE.value != null)
                {
                    result = HE.value;
                }
                else
                {
                    Expression e = new N();
                    e.Evaluate();
                    result = e.value;
                }

                if(parenthesis)
                {
                    if(Lexer.Tokens[Lexer.index] == ")")
                    {
                        Lexer.index++;
                        value = result;
                        id_store.Clear();
                    }
                }
                else
                {
                    value = result;
                    id_store.Clear();
                }

               
            }
            else throw new Exception("Error");
        }  
    }

    class Print : Expression
    {
        public override void Evaluate()
        {
            if(Lexer.Tokens[Lexer.index] == "(")
            {
                Lexer.index++;
                Expression x = new N();
                x.Evaluate();
                if(Lexer.Tokens[Lexer.index] == ")")
                {
                    Lexer.index++;
                    Console.WriteLine(x.value);
                    value = x.value;
                }
            }
            //Errores en desarrollo
        }
    }
  
    #region Booleans
    class boolean_operator : Binary_Exrpessions
    {
        public boolean_operator()
        {
            this.left = new boolean_comparison();

            this.right = new boolean_comparison();
        }

        public override void Evaluate()
        {
            left.Evaluate();
            string left_value = left.value;

            while(Lexer.index < Lexer.Tokens.Count)
            {
                if(Lexer.Tokens[Lexer.index] == "&")
                {
                    Lexer.index++;
                    right.Evaluate();
                    string right_value = right.value;
                    if(right_value == "false" || left_value == "false")
                    {
                        left_value = "false";
                    }
                    else left_value = "true";
                }
                else if(Lexer.Tokens[Lexer.index] == "|")
                {
                    Lexer.index++;
                    right.Evaluate();
                    string right_value = right.value;
                    if(right_value == "true" || left_value == "true")
                    {

                        left_value = "true";
                    }
                    else left_value = "false";
                }
                else if(Lexer.Tokens[Lexer.index] == ")" || Lexer.Tokens[Lexer.index] == ";" || Lexer.Tokens[Lexer.index] == "," || Lexer.Tokens[Lexer.index] == "in")
                {
                    //Siguientes
                    value = left_value;
                    break;
                }
            }
            
        }

    }
    class boolean_comparison : Binary_Exrpessions
    {
        public boolean_comparison()
        {
            left = new N();

            right = new N();
        }

        public override void Evaluate()
        {
            left.Evaluate();
            if(Regex.IsMatch(left.value , @"\d+"))
            {
                double left_value = double.Parse(left.value);

                if(Lexer.Tokens[Lexer.index] == ">")
                {
                    Lexer.index++;
                    right.Evaluate();
                    if(Regex.IsMatch(right.value , @"\d+"))
                    {
                        double right_value = double.Parse(right.value);

                        if(left_value > right_value)
                        {
                            value = "true";
                        }
                        else value = "false";
                    }
                }
                else if(Lexer.Tokens[Lexer.index] == "<")
                {
                    Lexer.index++;
                    right.Evaluate();
                    if(Regex.IsMatch(right.value , @"\d+"))
                    {
                        double right_value = double.Parse(right.value);

                        if(left_value < right_value)
                        {
                            value = "true";
                        }
                        else value = "false";
                    }
                }
                else if(Lexer.Tokens[Lexer.index] == "<=")
                {
                    Lexer.index++;
                    right.Evaluate();
                    if(Regex.IsMatch(right.value , @"\d+"))
                    {
                        double right_value = double.Parse(right.value);

                        if(left_value <= right_value)
                        {
                            value = "true";
                        }
                        else value = "false";
                    }
                }
                else if(Lexer.Tokens[Lexer.index] == ">=")
                {
                    Lexer.index++;
                    right.Evaluate();
                    if(Regex.IsMatch(right.value , @"\d+"))
                    {
                        double right_value = double.Parse(right.value);

                        if(left_value >= right_value)
                        {
                            value = "true";
                        }
                        else value = "false";
                    }
                }
                else if(Lexer.Tokens[Lexer.index] == "==")
                {
                    Lexer.index++;
                    right.Evaluate();
                    if(Regex.IsMatch(right.value , @"\d+"))
                    {
                        double right_value = double.Parse(right.value) ;

                        if(left_value == right_value)
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
                    if(Regex.IsMatch(right.value , @"\d+"))
                    {
                        double right_value = double.Parse(right.value);

                        if(left_value != right_value)
                        {
                            value = "true";
                        }
                        else value = "false";
                    }
                }

            }
            else if(left.value == "true" || left.value == "false")
            {
                value = left.value ;
            }
            else System.Console.WriteLine("No es una expresión booleana"); 
            
        }
    }   
    #endregion

    class Function_Declaration : Expression
    {
        public static Dictionary<string , Function > Function_Store = new Dictionary<string, Function>();
        public override void Evaluate()
        {
            
            List<string> function_Arguments = new List<string>();
            List<string> function_Expression = new List<string>();

            if(Regex.IsMatch(Lexer.Tokens[Lexer.index] , @"^[a-zA-Z]+\w*$"))
            {
                string function_id = Lexer.Tokens[Lexer.index];
                Lexer.index++;
                if(Lexer.Tokens[Lexer.index] == "(")
                {
                    Lexer.index++;
                    
                    while(Lexer.index < Lexer.Tokens.Count && Lexer.Tokens[Lexer.index] != ")")
                    {
                        if(Regex.IsMatch(Lexer.Tokens[Lexer.index] , @"^[a-zA-Z]+\w*$"))
                        {
                            function_Arguments.Add(Lexer.Tokens[Lexer.index]);
                            Lexer.index++;
                        }
                        //Añadir error
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
                                if(Regex.IsMatch(Lexer.Tokens[Lexer.index] , @"^[a-zA-Z]+\w*$") && !Lexer.Key_Words.Contains(Lexer.Tokens[Lexer.index]) && !function_Arguments.Contains(Lexer.Tokens[Lexer.index]) && Lexer.Tokens[Lexer.index] != function_id && !Function_Store.ContainsKey(Lexer.Tokens[Lexer.index]) )
                                {
                                    System.Console.WriteLine("error en linea 589" + " Token \"" +Lexer.Tokens[Lexer.index] + "\" no identificado" );//La variable no se declara en los argumentos de la función
                                    break; //Añadir que no pueden ser dos argumentos con id iguales
                                }
                                function_Expression.Add(Lexer.Tokens[Lexer.index]);
                                Lexer.index++;
                            }

                            if(Function_Store.ContainsKey(function_id))
                            {
                                Function_Store[function_id] = new Function(function_Arguments , function_Expression);
                            }
                            else
                            {
                                Function_Store.Add(function_id , new Function(function_Arguments , function_Expression));
                            }
                        }

                    }
                }
            }
        }
    }
    class Function : Expression
    {
        public List<string> Arguments_id = new List<string>();
        public List<string> Arguments_value = new List<string>();
        public List<string> function_Expression = new List<string>();
        public static Dictionary<string , string > functions_id = new Dictionary<string, string>();

        public Function(List<string> function_Arguments_id , List<string> Expression)
        {
            this.Arguments_id = function_Arguments_id;
            this.function_Expression = Expression;
        }
        public override void Evaluate()
        {
            if(Lexer.Tokens[Lexer.index] == "(")
            {
                string result ;
                Lexer.index++;

                Expression e = new N();

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
                else System.Console.WriteLine("error en lina 661");
                
                List<string> Originals_Tokens = Lexer.Tokens;
                int Original_index = Lexer.index;

                Lexer.Tokens = function_Expression;
                Lexer.index = 0;
                Expression HE = new HulkExpression();
                HE.Evaluate() ;

                if(HE.value != null)
                {
                    result = HE.value;
                }
                else
                {
                    e.Evaluate();
                    result = e.value;
                }
        
                Lexer.Tokens = Originals_Tokens;
                Lexer.index = Original_index;
                foreach(string s in functions_id.Keys)
                {
                    if(Original_values.ContainsKey(s))
                    {
                        functions_id[s] = Original_values[s];
                    }
                }

                if(Lexer.Tokens[Lexer.index] == ")")
                {
                    value = result;
                    Lexer.index++;
                    Arguments_value.Clear();
                }
                else System.Console.WriteLine("eROR");

            }
        }
    }
    class HulkExpression : Expression
    {
        public override void Evaluate()
        {
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
                Expression l = new let_in();
                l.Evaluate();
                value = l.value;
            }
            else if(Lexer.Tokens[Lexer.index] == "if")
            {
                Lexer.index++;
                if(Lexer.Tokens[Lexer.index] == "(")
                {
                    Lexer.index++;
                    Expression b = new boolean_operator();
                    b.Evaluate();
                    if(Lexer.Tokens[Lexer.index] == ")")
                    {
                        Lexer.index++;
                        Expression e = new N();//Va una HE
                        Expression HE = new HulkExpression();
                        string result ;

                        if(b.value == "true")
                        {
                            e.Evaluate();
                            if(e.value == null)
                            {
                                HE.Evaluate();
                                result = HE.value;
                            }
                            else result = e.value;

                            value = result ;

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
                                e.Evaluate();
                                if(e.value == null)
                                {
                                    HE.Evaluate();
                                    result = HE.value;
                                }
                                else result = e.value;

                                value = result ;
                            }
                        }
                    }
                }
            }
            else if(Lexer.Tokens[Lexer.index] == "function")
            {
                Lexer.index++;
                Expression f = new Function_Declaration();
                f.Evaluate();
            }
            else if(Function_Declaration.Function_Store.ContainsKey(Lexer.Tokens[Lexer.index]))
            {

                int i = Lexer.index;
                Lexer.index++;
                Function_Declaration.Function_Store[Lexer.Tokens[i]].Evaluate();
                value = Function_Declaration.Function_Store[Lexer.Tokens[i]].value;
                if(value != "")
                {
                    System.Console.WriteLine(value);
                }
            }
        }
    }  
}