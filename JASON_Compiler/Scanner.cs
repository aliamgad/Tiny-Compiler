using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

public enum Token_Class
{
    Comment_Statement, L_Curly_Bracket, R_Curly_Bracket,String_Value,
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

            Operators.Add("&&", Token_Class.And);
            Operators.Add("||", Token_Class.Or);

            Operators.Add(";", Token_Class.Semicolon);
            Operators.Add(",", Token_Class.Comma);
            Operators.Add("(", Token_Class.LParanthesis);
            Operators.Add(")", Token_Class.RParanthesis);

            Operators.Add("{", Token_Class.L_Curly_Bracket);
            Operators.Add("}", Token_Class.R_Curly_Bracket);

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
                //string CurrentLexeme = CurrentChar.ToString();
                string CurrentLexeme = "";
                if (CurrentChar == ' ' || CurrentChar == '\r' || CurrentChar == '\n' || CurrentChar == '\t')
                    continue;

                if (CurrentChar >= 'A' && CurrentChar <= 'z') //if you read a character
                {
                    while ((CurrentChar >= 'A' && CurrentChar <= 'z') || (CurrentChar >= '0' && CurrentChar <= '9'))
                    {

                        CurrentLexeme += CurrentChar;

                        j++;

                        if (j >= SourceCode.Length)
                            break;

                        CurrentChar = SourceCode[j];

                    }
                    i = j - 1;
                    FindTokenClass(CurrentLexeme);
                }

                else if (CurrentChar >= '0' && CurrentChar <= '9')
                {
                    while ((CurrentChar >= '0' && CurrentChar <= '9') || (CurrentChar == '.') || (CurrentChar >= 'A' && CurrentChar <= 'z'))
                    {


                        CurrentLexeme += CurrentChar;

                        j++;

                        if (j >= SourceCode.Length)
                            break;

                        CurrentChar = SourceCode[j];


                    }
                    i = j - 1;
                    FindTokenClass(CurrentLexeme);
                }
                else if (CurrentChar == '/') //will be changed to "/*" and take care that the comment is multiline
                {
                    CurrentLexeme += CurrentChar;
                    j++;
                    if (j >= SourceCode.Length)
                    {
                        FindTokenClass(CurrentLexeme); // Operator '/'
                    }
                    else
                    {
                        CurrentChar = SourceCode[j];

                        if (CurrentChar == '*')
                        {
                            // start reading the comment;
                            CurrentLexeme += CurrentChar;

                            bool prev_was_astrik = false;
                            j++;
                            bool reach_end = false;
                            while (j < SourceCode.Length)
                            {
                                if (SourceCode[j] == '*')
                                    prev_was_astrik = true;
                                else if (SourceCode[j] == '/' && prev_was_astrik)
                                {
                                    // end of comment
                                    reach_end = true;
                                }
                                else
                                    prev_was_astrik = false;


                                CurrentLexeme += SourceCode[j++];
                                if (reach_end)
                                {
                                    break;
                                }
                            }
                            FindTokenClass(CurrentLexeme);
                        }
                        else
                        {
                            FindTokenClass(CurrentLexeme); // Operator '/'

                        }
                    }
                    i = j - 1;
                }
                else if (CurrentChar == '"')
                {
                    CurrentLexeme += CurrentChar;

                    j++;

                    if (j >= SourceCode.Length)
                        FindTokenClass(CurrentLexeme);
                    else
                    {
                        CurrentChar = SourceCode[j];
                        while (CurrentChar != '"')
                        {
                            CurrentLexeme += CurrentChar;

                            j++;

                            if (j >= SourceCode.Length)
                                break;

                            CurrentChar = SourceCode[j];
                        }
                        CurrentLexeme += CurrentChar;
                        i = j;
                        FindTokenClass(CurrentLexeme);
                    }


                }
                else
                {
                    CurrentLexeme += CurrentChar;
                    if (CurrentChar == '&')
                    {
                        j++;
                        if (j >= SourceCode.Length)
                            FindTokenClass(CurrentLexeme);
                        else
                        {
                            CurrentLexeme += SourceCode[j];
                            FindTokenClass(CurrentLexeme);
                        }
                    }
                    else if (CurrentChar == '|')
                    {
                        j++;
                        if (j >= SourceCode.Length)
                            FindTokenClass(CurrentLexeme);
                        else
                        {
                            CurrentLexeme += SourceCode[j];
                            FindTokenClass(CurrentLexeme);
                        }
                    }
                    else if (CurrentChar == ':')
                    {
                        j++;
                        if (j >= SourceCode.Length)
                            FindTokenClass(CurrentLexeme);
                        else
                        {
                            CurrentLexeme += SourceCode[j];
                            FindTokenClass(CurrentLexeme);
                        }
                    }
                    else if (CurrentChar == '<')
                    {
                        j++;
                        if (j >= SourceCode.Length)
                            FindTokenClass(CurrentLexeme);
                        else
                        {
                            if (SourceCode[j] != '>')
                            {
                                FindTokenClass(CurrentLexeme);//oporator less than
                            }
                            else
                            {
                                CurrentLexeme += SourceCode[j];//oporator not equal
                                FindTokenClass(CurrentLexeme);
                            }
                        }
                    }

                    else
                    {
                        FindTokenClass(CurrentLexeme);
                    }
                    i = j;
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
            if (ReservedWords.ContainsKey(Lex.ToUpper()))
            {
                Tok.token_type = ReservedWords[Lex.ToUpper()];
                Tokens.Add(Tok);
            }

            //Is it an identifier?
            else if (isIdentifier(Lex))
            {
                Tok.token_type = Token_Class.Idenifier;
                Tokens.Add(Tok);
            }

            //Is it a Constant?
            else if (isConstant(Lex))
            {
                Tok.token_type = Token_Class.Number;
                Tokens.Add(Tok);

            }

            //Is it an operator?
            else if (Operators.ContainsKey(Lex))
            {
                Tok.token_type = Operators[Lex];
                Tokens.Add(Tok);
            }

            else if (isComment(Lex))
            {
                Tok.token_type = Token_Class.Comment_Statement;
                Tokens.Add(Tok);

            }

            else if (isString(Lex))
            {
                Tok.token_type = Token_Class.String_Value;
                Tokens.Add(Tok);

            }

            //Is it an undefined?
            else
            {
                Errors.Error_List.Add("Lexical Error: " + Lex + "\r\n");
            }

        }



        bool isIdentifier(string lex)
        {
            bool isValid = true;

            var rgx = new Regex(@"^([a-zA-Z]([0-9a-zA-z])*)$", RegexOptions.Compiled);

            if (!rgx.IsMatch(lex))
            {
                isValid = false;
            }

            return isValid;
        }
        bool isConstant(string lex)
        {
            bool isValid = true;
            // Check if the lex is a constant (Number) or not.

            var rgx = new Regex(@"^([0-9]+(\.[0-9]+)?)$", RegexOptions.Compiled);

            if (!rgx.IsMatch(lex))
            {
                isValid = false;
            }

            return isValid;
        }

        bool isComment(string lex)
        {
            bool isValid = true;

            var rgx = new Regex(@"^(\/\*((\*+[^(\/)\*])|[^\*])*\*+(\/))$", RegexOptions.Compiled);
            //var rgx = new Regex(@"^(\/\*(([^\*](\/)*)|((\*)*[^\/])|[^\*\/])*\*\/)$", RegexOptions.Compiled);

            if (!rgx.IsMatch(lex))
            {
                isValid = false;
            }
            return isValid;
        }
        bool isString(string lex)
        {
            bool isValid = true;

            var rgx = new Regex("^(\"[^\"]*\")$", RegexOptions.Compiled);//doesnt  work with {  "\""  }

            if (!rgx.IsMatch(lex))
            {
                isValid = false;
            }
            return isValid;
        }
    }
}
