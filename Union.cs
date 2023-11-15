using System.Numerics;

namespace HULK
{
    /// <summary>
    /// Representa las expresiones binarias de @
    /// </summary>
    class Union : Binary_Exrpessions // ( @ )
    {   
        public Expression left = new BooleanOperator();
        public Expression right = new BooleanOperator();
        private List<string> NextTokens = new List<string>(){";",")","in",",","else"};
        
        
        /// <summary>
        /// Operación de la clase Union ( Une como string dos valores )
        /// </summary>
        /// <param name="left">Representa la expresión de la izquierda</param>
        /// <param name="operatorToken">Operador de la expresión binaria ( @ )</param>
        /// <param name="right">Representa la expresión de la derecha</param>
        /// <returns>Retorna la unión de los valores de left y right (string)</returns>
        public override object Operation(object left , string operatorToken , object right)//Operación de @
        {
            return $"{left}{right}";
        }
        /// <summary>
        /// Analiza la expresión left , si hay un token de @ analiza la expresión right y define el tipo como string
        /// si no define el tipo de la expresión con el tipo de left.
        /// </summary>
        /// <exception cref="UnExpectedToken">Lanza error si no encuentra un siguiente de la expresión</exception>
        public override void Analize()
        {
            iDLeft = ActualToken() ;
            left.Analize(); // analiza la izquierda 
            
            while(Lexer.index < Lexer.Tokens.Count)
            {
                if(ActualToken() == "@")
                {
                    Next();
                    right.Analize() ;//analiza la derecha
                    type = "string" ;// @ devuelve un valor de type string
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
        /// <summary>
        /// Evalúa la expresión left , si hay un token de @ evalúa la expresión right y une los valores de left y right como string ,
        /// definiendo el valor de la expresión.
        /// </summary>
        /// <exception cref="UnExpectedToken">Lanza error si no encuentra un siguiente de la expresión</exception>
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
                    left.value = Operation(left.GetValue() , "@" , right.GetValue()) ;
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