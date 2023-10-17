using System.Numerics;

namespace HULK
{
    class Union : Binary_Exrpessions // ( @ )
    {   
       private List<string> NextTokens = new List<string>(){";",")","in",",","else"};
        
        public Union()
        {
            this.left = new BooleanOperator();

            this.right = new BooleanOperator();
        }

        public override object Operation(object left , string operatorToken , object right)
        {
            return $"{left}{right}";
        }
        public override void Analize()
        {
            iDLeft = ActualToken() ;
            left.Analize();
            
            while(Lexer.index < Lexer.Tokens.Count)
            {
                if(ActualToken() == "@")
                {
                    Next();
                    right.Analize() ;
                    type = "string" ;
                }
                else if (NextTokens.Contains(ActualToken()))
                {
                    type = left.type;
                    break;
                }
                else 
                {
                    throw new UnExpectedToken(ActualToken());
                }
            }
        }
        public override void Evaluate()
        {
            iDLeft = ActualToken() ;
            left.Evaluate();
            
            while(Lexer.index < Lexer.Tokens.Count)
            {
                if(ActualToken() == "@")
                {
                    Next();
                    right.Evaluate() ;
                    left.value = Operation(left.value , "@" , right.value) ;
                }
                else if (NextTokens.Contains(ActualToken()))
                {
                    value = left.value;
                    break;
                }
                else 
                {
                    throw new UnExpectedToken(ActualToken());
                }
            }
        }
    }
}