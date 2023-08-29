using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace HULK
{
    class Lexer
    {
       public static List<string> Tokens = new List<string>();

       public static int index = 0 ;
       public Lexer(string input)
       {
        input = Regex.Replace(input , @"\s+" , " ");
        
        List<Match> t = Regex.Matches(input , @"\d+|\+|\-|\*|\^|/|\(|\)|;|,|let\s|=|[a-zA-Z]+\w*|\""").ToList();

        foreach(Match m in t )
        {
            Tokens.Add(m.Value);
        }
       }

    }
}