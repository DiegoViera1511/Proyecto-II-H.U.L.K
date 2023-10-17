using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Globalization;

namespace HULK
{
    class Lexer
    {
        public static List<string> Tokens = new List<string>();

        public static List<object> ConsolePrints = new List<object>();

        static private NumberFormatInfo nfi = CultureInfo.CurrentCulture.NumberFormat;

        static public string decimalSeparator = nfi.NumberDecimalSeparator;
        public static int index = 0 ;

        public static List<string> Key_Words  = new List<string>()
        {"print" , "let " , "in", "function" , "if" , "else" , "true" , "false" ,"sin" , "cos" , "sqrt" , "rand" , "exp" , "log" , "PI" , "E" };
       

        #region Methods 

        public static void Restart()
        {
            foreach(string id in FunctionDeclaration.functionStack.Keys)
            {
                FunctionDeclaration.functionStack[id] = 0 ;
            }
            ConsolePrints.Clear();
            FunctionDeclaration.functionsIdInference.Clear();
            FunctionDeclaration.functionArgumentsCount = 0 ;
            Function.functionsId.Clear();
            index = 0;
            Tokens.Clear();
        }
        public static void TokenizeInput(string input)
        {
            input = Regex.Replace(input , @"\s+" , " ");
            Regex AllTokens ;//= new(@"\d+$|\d+[\.,]{1}\d+|\+|\-|\*|\^|/|%|\(|\)|(=>)|(>=)|(<=)|<[=]{0}|>[=]{0}|!=|;|,|let |={1,2}|function|if|else|!|\&|\||true|false|(\u0022([^\u0022\\]|\\.)*\u0022)|@|\w+|[^\(\)\+\-\*/\^%<>=!&\|,;\s]+");
            Regex GoodTokens ;//= new(@"^\d+$|^\d+[\.,]{1}\d+$|\+|\-|\*|\^|/|%|\(|\)|(=>)|(>=)|(<=)|<[=]{0}|>[=]{0}|!=|;|,|let |={1,2}|function|if|else|!|\&|\||true|false|(\u0022([^\u0022\\]|\\.)*\u0022)|@|^[a-zA-Z]+\w*$");
            
            if(decimalSeparator == ".")//Mac
            {
                AllTokens = new(@"\d+$|\d+\.\d+|\+|\-|\*|\^|/|%|\(|\)|(=>)|(>=)|(<=)|<[=]{0}|>[=]{0}|!=|;|,|let |={1,2}|function|if|else|!|\&|\||true|false|(\u0022([^\u0022\\]|\\.)*\u0022)|@|\w+|[^\(\)\+\-\*/\^%<>=!&\|,;\s]+");
                GoodTokens = new(@"^\d+$|^\d+\.\d+$|\+|\-|\*|\^|/|%|\(|\)|(=>)|(>=)|(<=)|<[=]{0}|>[=]{0}|!=|;|,|let |={1,2}|function|if|else|!|\&|\||true|false|(\u0022([^\u0022\\]|\\.)*\u0022)|@|^[a-zA-Z]+\w*$");         
            }
            else//Windows
            {
                AllTokens = new(@"\d+$|\d+,\d+|\+|\-|\*|\^|/|%|\(|\)|(=>)|(>=)|(<=)|<[=]{0}|>[=]{0}|!=|;|,|let |={1,2}|function|if|else|!|\&|\||true|false|(\u0022([^\u0022\\]|\\.)*\u0022)|@|\w+|[^\(\)\+\-\*/\^%<>=!&\|,;\s]+");
                GoodTokens = new(@"^\d+$|^\d+,\d+$|\+|\-|\*|\^|/|%|\(|\)|(=>)|(>=)|(<=)|<[=]{0}|>[=]{0}|!=|;|,|let |={1,2}|function|if|else|!|\&|\||true|false|(\u0022([^\u0022\\]|\\.)*\u0022)|@|^[a-zA-Z]+\w*$");         
            }
            List<Match> t = AllTokens.Matches(input).ToList() ;
            
            foreach(Match m in t )
            {
                if( GoodTokens.IsMatch (m.Value) )
                {
                    Tokens.Add(m.Value);
                }
                else 
                {
                    throw new LexicalError(m.Value);
                }
            }
        }
        public static bool IsNumber(string Token)
        {
            if(decimalSeparator == ".")
            {
                return Regex.IsMatch(Token , @"^-{0,1}\d+$|^-{0,1}\d+\.\d+E(\+|-)\d+$|^-{0,1}\d+\.\d+$") || Token == Convert.ToString(double.PositiveInfinity) ? true : false ;
            }
            else 
            {
                return Regex.IsMatch(Token , @"^-{0,1}\d+$|^-{0,1}\d+,\d+E(\+|-)\d+$|^-{0,1}\d+,\d+$") || Token == Convert.ToString(double.PositiveInfinity) ? true : false ;
            }
        }
        public static bool IsString(string Token)
        {
            return Regex.IsMatch(Token , @"(\u0022([^\u0022\\]|\\.)*\u0022)") ? true : false ;
        }
        public static bool IsBoolean(string Token)
        {
           return Regex.IsMatch(Token , @"^true$|^false$") ?  true : false ;
        }
        public static bool IsID(string Token)
        {
            return Regex.IsMatch( Token , @"^[a-zA-Z]+\w*$") ? true : false ;
        }
        public static string TokenType(object Token)
        {
            if(Token.GetType() == Type.GetType("System.Double"))
            {
                return "number" ;
            }
            else if(Token.GetType() == Type.GetType("System.String"))
            {
                return "string" ;
            }
            else if(Token.GetType() == Type.GetType("System.Boolean"))
            {
                return "boolean" ;
            }
            else 
            {
                System.Console.WriteLine(Token);
                return "error in code";
            }
        }

        public static string GetIncorrectToken(string a , string b , string expectedToken)
        {
            if(a != expectedToken )
            {
                return TokenType(a) ;
            }
            else return TokenType(b) ;            
        }
        #endregion

    }
}