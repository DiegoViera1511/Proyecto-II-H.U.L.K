using System;
using System.Text.RegularExpressions;

namespace HULK
{
    /*
    public static class Tools
    {
        public static List<string> Tokens = new List<string>() ;
        public static int index = 0;
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
            if(Regex.IsMatch(left.value , @"\D+"))
            {
                //Construir
                value = left.value;
                return ;
            }
            double left_value = double.Parse(left.value);
            while(Lexer.index < Lexer.Tokens.Count)
            {
                if(Lexer.Tokens[Lexer.index] == "+")
                {
                    Lexer.index++;
                    //Expression right = new T();
                    right.Evaluate(); //Verificar que right value es un número
                    left_value += double.Parse(right.value);
                }
                else if(Lexer.Tokens[Lexer.index] == "-")
                {
                    Lexer.index++;
                    //Expression right = new T();
                    right.Evaluate();
                    left_value -= double.Parse(right.value);
                }
                else if (Lexer.Tokens[Lexer.index] == ";" || Lexer.Tokens[Lexer.index] == ")" ||Lexer.Tokens[Lexer.index] == "in" || Lexer.Tokens[Lexer.index] == ",")//Siguientes
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
            if(Regex.IsMatch(left.value , @"\D+"))
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
                else if(Lexer.Tokens[Lexer.index] == "+" || Lexer.Tokens[Lexer.index] == "-" || Lexer.Tokens[Lexer.index] == ")" || Lexer.Tokens[Lexer.index] == ";" || Lexer.Tokens[Lexer.index] == "in" || Lexer.Tokens[Lexer.index] == ",")
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
            if(Regex.IsMatch(left.value , @"\D+"))
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
            else if(Lexer.index < Lexer.Tokens.Count && Lexer.Tokens[Lexer.index][0] == '"' && Lexer.Tokens[Lexer.index][Lexer.Tokens[Lexer.index].Length - 1] == '"' ) 
            {
                value = Lexer.Tokens[Lexer.index].Substring( 0 , Lexer.Tokens[Lexer.index].Length);
            }
            else if(Lexer.index < Lexer.Tokens.Count && Regex.IsMatch(Lexer.Tokens[Lexer.index], @"let"))
            {
                Lexer.index++;
                Expression l = new let_in();
                l.Evaluate();
                value = l.value;
            }
            else if (Lexer.index < Lexer.Tokens.Count && let_in.id_store.ContainsKey(Lexer.Tokens[Lexer.index]))
            {
                string s = let_in.id_store[Lexer.Tokens[Lexer.index]];
                Lexer.index++;
                value = s ;
                //return s ;
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
                    string id = Lexer.Tokens[Lexer.index];
                    Lexer.index++;
                    if(Lexer.Tokens[Lexer.index] == "=")
                    {
                        Lexer.index++;
                        Expression Value = new N();
                        Value.Evaluate();
                        
                        string id_value = Value.value;//cambios
                        id_store.Add(id , id_value );
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

                this.value = result;
                id_store.Clear();
               
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
    
    class HulkExpression : Expression
    {
        public override void Evaluate()
        {
            if(Lexer.Tokens[Lexer.index] == "print")
            {
                Lexer.index++;
                Expression p = new Print();
                p.Evaluate();
                value = p.value;
            }
            else if(Regex.IsMatch(Lexer.Tokens[Lexer.index] , @"let"))
            {
                Lexer.index++;
                Expression l = new let_in();
                l.Evaluate();
                value = l.value;
            }
        }
    }
    
}