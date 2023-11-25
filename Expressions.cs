using System;
using System.Data.Common;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

namespace HULK
{

    //Abstract classes
    #region Abstract classes
    /// <summary>
    /// Clase abstracta de expresión
    /// </summary>
    public abstract class Expression
    {
        public const string NumberType = "number" ;
        public const string StringType = "string" ;
        public const string BooleanType = "boolean" ;
        public const string InferenceType = "inference" ;

        public object? value ;

        public string? type ;

        public object GetValue()
        {
            if(value != null)
            {
                return value ;
            }
            else throw new DefaultError("Null value");
        }

        public string GetExpType()
        {
            if(type != null)
            {
                return type ;
            }
            else throw new DefaultError("Null value");
        }
        /// <summary>
        /// Mueve el índice al próximo token
        /// </summary>
        static public void Next()
        {
            Lexer.index++;
        }
        /// <summary>
        /// Retorna el token actual
        /// </summary>
        /// <returns>Token actual</returns>
        static public string ActualToken()
        {
            return Lexer.Tokens[Lexer.index];
        }
        public abstract void Evaluate();

        public abstract void Analize();
    }
    /// <summary>
    /// Clase abstracta de expresiones binarias
    /// </summary>
    abstract class Binary_Exrpessions : Expression
    {

        public string? iDLeft  ;
        public string? iDRight  ;

        public abstract object Operation(object left , string operatorToken , object right);

        /// <summary>
        /// Encuentra errores de argumentos de funciones
        /// </summary>
        /// <param name="idLeft">Token que representa al valor de la izquierda</param>
        /// <param name="leftType">Tipo del valor de la expresión de la izquierda</param>
        /// <param name="iDRight">Token que representa al valor de la derecha</param>
        /// <param name="rightType">Tipo del valor de la expresión de la derecha</param>
        /// <param name="expectedType">Tipo de valor esperado</param>
        /// <exception cref="ArgumentTypeError">Lanza error si left o right no son del tipo correcto y son argumentos de función</exception>
        public void CatchArgumentTypeError(string idLeft , string leftType , string iDRight , string rightType , string expectedType)
        {
            if(Function.functionsId.ContainsKey(idLeft))
            {
                if(leftType != expectedType)
                {
                    throw new ArgumentTypeError(expectedType , leftType);
                }
            }
            if(Function.functionsId.ContainsKey(iDRight))
            {
                if(rightType != expectedType)
                {
                    throw new ArgumentTypeError(expectedType , rightType);
                }
            }
        }
    }

    #endregion
     
    /// <summary>
    /// Clase inicializadora , evalúa todo tipo de expresiones HULK .
    /// </summary>
    class HulkExpression : Expression
    {
        //Inicio de Expressión

        public override void Analize()
        {
            if(Lexer.Tokens.Count == 0)// Void Expression
            {
                return;
            }

            if(ActualToken() == "print")
            {
                Next();
                Expression printExp = new Print();
                printExp.Analize();
            }
            else if(ActualToken() == "let ")
            {
                Next();
                Expression letIn = new Let_in();
                letIn.Analize();
            }
            else if(ActualToken() == "if")
            {
                Next();
                Expression ifelse = new Conditional();
                ifelse.Analize();
            }
            else if(ActualToken() == "function")
            {
                Next();
                FunctionDeclaration f = new FunctionDeclaration();
                f.Analize();
            }
            else if(FunctionDeclaration.functionStore.ContainsKey(ActualToken()))
            {
                int i = Lexer.index;
                Next();
                FunctionDeclaration.functionStore[Lexer.Tokens[i]].Analize() ;
            }
            else //Atom expression
            {
                Expression exp = new Union();
                exp.Analize();
            }
        }
        public override void Evaluate()
        {
            
            if(Lexer.Tokens.Count == 0)// Void Expression
            {
                return;
            }

            if(ActualToken() == "print")
            {
                Next();
                Expression printExp = new Print();
                printExp.Evaluate();
                value = printExp.value;
            }
            else if(ActualToken() == "let ")
            {
                Next();
                Expression letIn = new Let_in();
                letIn.Evaluate();
                value = letIn.value;
            }
            else if(ActualToken() == "if")
            {
                Next();
                Expression ifelse = new Conditional();
                ifelse.Evaluate();
                value = ifelse.value;
            }
            else if(FunctionDeclaration.functionStore.ContainsKey(ActualToken()))
            {

                int i = Lexer.index;
                Next();
                FunctionDeclaration.functionStore[Lexer.Tokens[i]].Evaluate() ;
                value = FunctionDeclaration.functionStore[Lexer.Tokens[i]].value ;
                
                Lexer.ConsolePrints.Add(GetValue());
              
            }
            else //Atom expression
            {
                Expression exp = new Union();
                exp.Evaluate();
                value = exp.value ;
            }
        }
    }  

}