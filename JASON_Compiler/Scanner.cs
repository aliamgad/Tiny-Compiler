using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum Token_Class
{
    Quotes,
    Float, Repeat, Elseif, Is_Equal, Number, String, Return, Endl, And, Or,
    Else, End, If, Integer, Read, Then, Until, Write,
    Dot, Semicolon, Comma, LParanthesis, RParanthesis, Assagniment, LessThanOp,
    GreaterThanOp, NotEqualOp, PlusOp, MinusOp, MultiplyOp, DivideOp,
    Idenifier
}
namespace JASON_Compiler
{


    public class Token
    {
        public string lex;
        public Token_Class token_type;
    }

    public class Scanner
    {
        public List<Token> Tokens = new List<Token>();
        Dictionary<string, Token_Class> ReservedWords = new Dictionary<string, Token_Class>();
        Dictionary<string, Token_Class> Operators = new Dictionary<string, Token_Class>();

        public Scanner()
        {
            ReservedWords.Add("IF", Token_Class.If);
            ReservedWords.Add("FLOAT", Token_Class.Float);
            ReservedWords.Add("REPEAT", Token_Class.Repeat);
            ReservedWords.Add("ELSEIF", Token_Class.Elseif);
            ReservedWords.Add("END", Token_Class.End);
            ReservedWords.Add("STRING", Token_Class.String);
            ReservedWords.Add("ELSE", Token_Class.Else);
            ReservedWords.Add("ENDL", Token_Class.Endl);
            ReservedWords.Add("RETURN", Token_Class.Return);
            ReservedWords.Add("INT", Token_Class.Integer);
            ReservedWords.Add("READ", Token_Class.Read);
            ReservedWords.Add("THEN", Token_Class.Then);
            ReservedWords.Add("UNTIL", Token_Class.Until);
            ReservedWords.Add("WRITE", Token_Class.Write);

            //Operators.Add(".", Token_Class.Dot);
            Operators.Add("&&", Token_Class.And);
            Operators.Add("||", Token_Class.Or);
            Operators.Add("\"", Token_Class.Quotes);
            Operators.Add(";", Token_Class.Semicolon);
            Operators.Add(",", Token_Class.Comma);
            Operators.Add("(", Token_Class.LParanthesis);
            Operators.Add(")", Token_Class.RParanthesis);
            Operators.Add("=", Token_Class.Is_Equal);
            Operators.Add("<", Token_Class.LessThanOp);
            Operators.Add(">", Token_Class.GreaterThanOp);
            Operators.Add("<>", Token_Class.NotEqualOp);
            Operators.Add(":=", Token_Class.Assagniment);
            Operators.Add("+", Token_Class.PlusOp);
            Operators.Add("-", Token_Class.MinusOp);
            Operators.Add("*", Token_Class.MultiplyOp);
            Operators.Add("/", Token_Class.DivideOp);



        }

        public void StartScanning(string SourceCode)
        {
            for (int i = 0; i < SourceCode.Length; i++)
            {
                int j = i;
                char CurrentChar = SourceCode[i];
                string CurrentLexeme = CurrentChar.ToString();

                if (CurrentChar == ' ' || CurrentChar == '\r' || CurrentChar == '\n')
                    continue;

                if (CurrentChar >= 'A' && CurrentChar <= 'z') //if you read a character
                {

                }

                else if (CurrentChar >= '0' && CurrentChar <= '9')
                {

                }
                else if (CurrentChar == '{') //will be changed to "/*"
                {

                }
                else
                {

                }
            }

            JASON_Compiler.TokenStream = Tokens;
        }
        void FindTokenClass(string Lex)
        {
            Token_Class TC;
            Token Tok = new Token();
            Tok.lex = Lex;
            //Is it a reserved word?


            //Is it an identifier?


            //Is it a Constant?

            //Is it an operator?

            //Is it an undefined?
        }



        bool isIdentifier(string lex)
        {
            bool isValid = true;
            // Check if the lex is an identifier or not.

            return isValid;
        }
        bool isConstant(string lex)
        {
            bool isValid = true;
            // Check if the lex is a constant (Number) or not.

            return isValid;
        }
    }
}
