using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace HULK
{
    class Lexer
    {
       public static List<string> Tokens = new List<string>();

       public static int index = 0 ;

       public static List<string> Key_Words  = new List<string>()
       {"print" , "let " , "in", "function" , "if" , "else" , "true" , "false" };
   
        public Lexer(string input)
       {
       
        input = Regex.Replace(input , @"\s+" , " ");
        //System.Console.WriteLine(input);
        
        List<Match> t = Regex.Matches(input , @"\d+|\+|\-|\*|\^|/|%|\(|\)|(=>)|;|,|let\s|={1,2}|function|[a-zA-Z]+\w*|if|else|(>=)|(<=)|<[=]{0}|>[=]{0}|!=|!|\&|\||true|false|(\u0022([^\u0022\\]|\\.)*\u0022)|@").ToList();

        foreach(Match m in t )
        {
            Tokens.Add(m.Value);
        }
       }

    }
}