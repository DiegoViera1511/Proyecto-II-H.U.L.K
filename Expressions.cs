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
        public string value ;
        public abstract void Evaluate();
    }

    abstract class Binary_Exrpessions : Expression
    {
        public Expression left ;

        public Expression right ;
    }


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
            double left_value = double.Parse(left.value);
            while(Lexer.index < Lexer.Tokens.Count)
            {
                if(Lexer.Tokens[Lexer.index] == "+")
                {
                    Lexer.index++;
                    //Expression right = new T();
                    right.Evaluate();
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
                    //return s;
                }
                else
                {
                    throw new Exception("Se esperaba )");
                }

            }
            else if (Lexer.index < Lexer.Tokens.Count && let_in.id_store.ContainsKey(Lexer.Tokens[Lexer.index]))
            {
                string s = let_in.id_store[Lexer.Tokens[Lexer.index]];
                Lexer.index++;
                value = s ;
                //return s ;

            }
            else throw new Exception("error");
        }
    }

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
                        //string s = Value.Evaluate();
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
                Expression expression = new N();
                expression.Evaluate();
                string value = expression.value;
                let_in.id_store.Clear();
                this.value = expression.value;
            }
            else throw new Exception("Error");
        }  
    }
    /*
    class Instruction : Expression
    {
        public override string Evaluate()
        {
            if(Lexer.Tokens[Lexer.index] == "print")
            {
                Lexer.index++;
                if(Lexer.Tokens[Lexer.index] == "(")
                {
                    Expression x = new N();
                    string value = x.Evaluate();
                    if(Lexer.Tokens[Lexer.index] == ")")
                    {
                        Lexer.index++;
                        Console.WriteLine(value);
                        return value;
                    }
                }
            }
            else throw new Exception("No es una instrucción");
        }
    }
    */
}