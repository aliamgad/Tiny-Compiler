using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace JASON_Compiler
{
    public class Node
    {
        public List<Node> Children = new List<Node>();

        public string Name;
        public Node(string N)
        {
            this.Name = N;
        }
    }
    public class Parser
    {
        int InputPointer = 0;
        List<Token> TokenStream;
        public Node root;

        public Node StartParsing(List<Token> TokenStream)
        {
            this.InputPointer = 0;
            this.TokenStream = TokenStream;
            root = new Node("Code");
            root.Children.Add(Program());
            return root;
        }

        Node Program()
        {
            Node program = new Node("Program");
            program.Children.Add(Comment_Statement());
            program.Children.Add(ProgramHelp());
            return program;
        }

        Node ProgramHelp()
        {
            Node programhelp = new Node("ProgramHelp");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Integer && TokenStream[InputPointer + 1].token_type == Token_Class.Main)
            {
                programhelp.Children.Add(MainFunc());
            }
            else if (InputPointer < TokenStream.Count)
            {
                programhelp.Children.Add(FuncMaker());
                programhelp.Children.Add(Comment_Statement());
                programhelp.Children.Add(MainFunc());
            }
            programhelp.Children.Add(Comment_Statement());
            return programhelp;
        }

        Node FuncMaker()
        {
            Node funcmaker = new Node("FuncMaker");

            if (TokenStream.Count > InputPointer && (TokenStream[InputPointer].token_type == Token_Class.Integer || TokenStream[InputPointer].token_type == Token_Class.Float || TokenStream[InputPointer].token_type == Token_Class.String)
                && TokenStream[InputPointer + 1].token_type != Token_Class.Main)
            {
                funcmaker.Children.Add(Function());
                funcmaker.Children.Add(FuncMaker());
                return funcmaker;
            }

            return null;
        }

        Node Function()
        {
            Node function = new Node("Function");

            function.Children.Add(FuncDecl());
            function.Children.Add(FuncBody());

            return function;
        }
        Node FuncDecl()
        {
            Node funcdecl = new Node("FuncDecl");

            funcdecl.Children.Add(Datatype());
            funcdecl.Children.Add(match(Token_Class.Idenifier));
            funcdecl.Children.Add(Parameters());

            return funcdecl;
        }

        Node Parameters()
        {
            Node parameters = new Node("Parameters");


            parameters.Children.Add(match(Token_Class.LParanthesis));
            parameters.Children.Add(ParamList());
            parameters.Children.Add(match(Token_Class.RParanthesis));

            return parameters;
        }

        Node ParamList()
        {
            Node paramlist = new Node("ParamList");

            if (TokenStream.Count > InputPointer && (TokenStream[InputPointer].token_type == Token_Class.Integer || TokenStream[InputPointer].token_type == Token_Class.Float || TokenStream[InputPointer].token_type == Token_Class.String))
            {
                paramlist.Children.Add(Parameter());
                paramlist.Children.Add(ParamHelp());
                return paramlist;
            }
            return null;

        }
        Node ParamHelp()
        {
            Node paramhelp = new Node("ParamHelp");

            if (TokenStream.Count > InputPointer && TokenStream[InputPointer].token_type == Token_Class.Comma)
            {
                paramhelp.Children.Add(match(Token_Class.Comma));
                paramhelp.Children.Add(Parameter());
                paramhelp.Children.Add(ParamHelp());
                return paramhelp;
            }
            return null;
        }

        Node Parameter()
        {
            Node parameter = new Node("Parameter");

            parameter.Children.Add(Datatype());
            parameter.Children.Add(match(Token_Class.Idenifier));

            return parameter;
        }

        Node FuncBody()
        {
            Node funcbody = new Node("FuncBody");

            funcbody.Children.Add(match(Token_Class.L_Curly_Bracket));
            funcbody.Children.Add(Statements());
            funcbody.Children.Add(Return_Statement());
            funcbody.Children.Add(match(Token_Class.R_Curly_Bracket));

            return funcbody;
        }

        Node Return_Statement()
        {
            Node return_statement = new Node("Return_Statement");

            return_statement.Children.Add(match(Token_Class.Return));
            return_statement.Children.Add(Expression());
            return_statement.Children.Add(match(Token_Class.Semicolon));


            return return_statement;
        }

        Node Expression()
        {
            Node expression = new Node("Expression");

            if (TokenStream.Count > InputPointer)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.Number || TokenStream[InputPointer].token_type == Token_Class.Idenifier)
                {
                    if (TokenStream[InputPointer + 1].token_type == Token_Class.MultiplyOp || TokenStream[InputPointer + 1].token_type == Token_Class.PlusOp ||
                    TokenStream[InputPointer + 1].token_type == Token_Class.DivideOp || TokenStream[InputPointer + 1].token_type == Token_Class.MinusOp)
                    {
                        expression.Children.Add(Equation());
                    }
                    else
                    {
                        expression.Children.Add(Term());
                    }
                }
                else
                {
                    expression.Children.Add(match(Token_Class.String_Value));
                }

                return expression;
            }

            return null;
        }


        Node Term()
        {
            Node term = new Node("Term");
            if (TokenStream.Count > InputPointer)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.Number)
                {
                    term.Children.Add(match(Token_Class.Number));
                }
                
                else if (TokenStream[InputPointer].token_type == Token_Class.Idenifier && TokenStream[InputPointer + 1].token_type == Token_Class.LParanthesis)
                {
                    term.Children.Add(Func_Call());
                }
                else
                {
                    term.Children.Add(match(Token_Class.Idenifier));
                }


                return term;
            }
            return null;

        }

        Node Func_Call()
        {
            Node func_call = new Node("Func_Call");

            func_call.Children.Add(match(Token_Class.Idenifier));
            func_call.Children.Add(ArgList());

            return func_call;
        }
        Node ArgList()
        {
            Node arglist = new Node("ArgList");

            arglist.Children.Add(match(Token_Class.LParanthesis));
            arglist.Children.Add(Arguments());
            arglist.Children.Add(match(Token_Class.RParanthesis));

            return arglist;
        }

        Node Arguments()
        {
            Node arguments = new Node("Arguments");

            if (TokenStream.Count > InputPointer && (TokenStream[InputPointer].token_type == Token_Class.Idenifier || TokenStream[InputPointer].token_type == Token_Class.Number
            || TokenStream[InputPointer].token_type == Token_Class.String_Value)) // omar hal al String haga ziada ? 
            {
                arguments.Children.Add(ArgHleper());
                arguments.Children.Add(Arg());
                return arguments;
            }
            return null;
        }

        private Node ArgHleper()
        {
            Node arghleper = new Node("ArgHleper");

            if (TokenStream[InputPointer].token_type == Token_Class.Idenifier)
            {
                arghleper.Children.Add(match(Token_Class.Idenifier));
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.Number)
            {
                arghleper.Children.Add(match(Token_Class.Number));
            }
            else
            {
                arghleper.Children.Add(match(Token_Class.String_Value));
            }
            return arghleper;
        }

        Node Arg()
        {
            Node arg = new Node("Arg");

            if (TokenStream.Count > InputPointer && TokenStream[InputPointer].token_type == Token_Class.Comma)
            {
                arg.Children.Add(match(Token_Class.Comma));
                arg.Children.Add(match(Token_Class.Idenifier));
                arg.Children.Add(Arg());
                return arg;
            }
            return null;
        }

        Node Equation()
        {
            Node equation = new Node("Equation");

            equation.Children.Add(Preceeded());
            equation.Children.Add(Equation_Helper());

            return equation;
        }

        Node Equation_Helper()
        {
            Node equation_helper = new Node("Equation_Helper");

            if (TokenStream.Count > InputPointer && (TokenStream[InputPointer].token_type == Token_Class.PlusOp || TokenStream[InputPointer].token_type == Token_Class.MinusOp))
            {
                equation_helper.Children.Add(Addop());
                equation_helper.Children.Add(Equation());
                equation_helper.Children.Add(Equation_Helper());
                return equation_helper;
            }
            return null;
        }

        Node Addop()
        {
            Node addop = new Node("Addop");

            if (TokenStream[InputPointer].token_type == Token_Class.PlusOp)
            {
                addop.Children.Add(match(Token_Class.PlusOp));
            }
            else
            {
                addop.Children.Add(match(Token_Class.MinusOp));
            }

            return addop;
        }

        Node Preceeded()
        {
            Node preceeded = new Node("Preceeded");

            preceeded.Children.Add(Factor());
            preceeded.Children.Add(Preceeded_Helper());

            return preceeded;
        }

        Node Preceeded_Helper()
        {
            Node preceeded_helper = new Node("Preceeded_Helper");

            if (TokenStream.Count > InputPointer && (TokenStream[InputPointer].token_type == Token_Class.MultiplyOp || TokenStream[InputPointer].token_type == Token_Class.DivideOp))
            {
                preceeded_helper.Children.Add(Mulop());
                preceeded_helper.Children.Add(Preceeded());
                preceeded_helper.Children.Add(Preceeded_Helper());
                return preceeded_helper;
            }
            return null;
        }

        Node Mulop()
        {
            Node mulop = new Node("Mulop");

            if (TokenStream[InputPointer].token_type == Token_Class.MultiplyOp)
            {
                mulop.Children.Add(match(Token_Class.MultiplyOp));
            }
            else
            {
                mulop.Children.Add(match(Token_Class.DivideOp));
            }

            return mulop;
        }

        Node Factor()
        {
            Node factor = new Node("Factor");

            if (TokenStream[InputPointer].token_type == Token_Class.LParanthesis)
            {

                factor.Children.Add(match(Token_Class.LParanthesis));
                factor.Children.Add(Equation());
                factor.Children.Add(match(Token_Class.RParanthesis));
            }
            else
            {
                factor.Children.Add(Term());
            }

            return factor;
        }


        Node Datatype()
        {
            Node datatype = new Node("Datatype");

            if(TokenStream.Count > InputPointer)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.Integer)
                {
                    datatype.Children.Add(match(Token_Class.Integer));
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.Float)
                {
                    datatype.Children.Add(match(Token_Class.Float));
                }
                else
                {
                    datatype.Children.Add(match(Token_Class.String));
                }

                return datatype;
            }
            return null;
            
        }

        Node MainFunc()
        {
            Node mainfunc = new Node("MainFunc");
            mainfunc.Children.Add(Datatype());
            mainfunc.Children.Add(match(Token_Class.Main));
            mainfunc.Children.Add(match(Token_Class.LParanthesis));
            mainfunc.Children.Add(match(Token_Class.RParanthesis));
            mainfunc.Children.Add(FuncBody());

            return mainfunc;
        }

        Node Statements()
        {
            Node statements = new Node("Statements");
            if (TokenStream.Count > InputPointer &&
                (TokenStream[InputPointer].token_type == Token_Class.Comment_Statement || TokenStream[InputPointer].token_type == Token_Class.Idenifier ||
                (TokenStream[InputPointer].token_type == Token_Class.Integer || TokenStream[InputPointer].token_type == Token_Class.Float || TokenStream[InputPointer].token_type == Token_Class.String)
                || TokenStream[InputPointer].token_type == Token_Class.Write || TokenStream[InputPointer].token_type == Token_Class.Read
                || TokenStream[InputPointer].token_type == Token_Class.If || TokenStream[InputPointer].token_type == Token_Class.Repeat))
            {
                statements.Children.Add(Statement());
                statements.Children.Add(Statements());
                return statements;
            }
            return null;
        }

        Node Comment_Statement()
        {
            Node comment_statement = new Node("Comment_Statement");
            if (TokenStream.Count > InputPointer && TokenStream[InputPointer].token_type == Token_Class.Comment_Statement)
            {
                comment_statement.Children.Add(match(Token_Class.Comment_Statement));
                comment_statement.Children.Add(Comment_Statement());
                return comment_statement;
            }
            return null;

        }

        Node Statement()
        {
            Node statement = new Node("Statement");

            if (TokenStream[InputPointer].token_type == Token_Class.Idenifier)//for Assignment_Statement and Func_Call
            {
                if (TokenStream[InputPointer].token_type == Token_Class.LParanthesis)
                {
                    statement.Children.Add(Func_Call());
                }
                else
                {
                    statement.Children.Add(Assignment_Statement());
                }

            }
            else if (TokenStream[InputPointer].token_type == Token_Class.Comment_Statement)
            {
                statement.Children.Add(Comment_Statement());
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.Integer || TokenStream[InputPointer].token_type == Token_Class.Float || TokenStream[InputPointer].token_type == Token_Class.String)
            {
                statement.Children.Add(Declaration_Statement());
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.Write)
            {
                statement.Children.Add(Write_Statement());
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.Read)
            {
                statement.Children.Add(Read_Statement());
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.If)
            {
                statement.Children.Add(If_Statement());
            }
            else
            {
                statement.Children.Add(Repeat_Statement());
            }

            return statement;
        }

        Node Assignment_Statement()
        {
            Node assignment_statement = new Node("Assignment_Statement");
            assignment_statement.Children.Add(match(Token_Class.Idenifier));
            assignment_statement.Children.Add(match(Token_Class.Assagniment));
            assignment_statement.Children.Add(Expression());
            assignment_statement.Children.Add(match(Token_Class.Semicolon));
            return assignment_statement;
        }

        Node Declaration_Statement()
        {
            Node declaration_statement = new Node("Declaration_Statement");
            declaration_statement.Children.Add(Datatype());
            declaration_statement.Children.Add(IdentifierDec());
            declaration_statement.Children.Add(Idenlist());
            declaration_statement.Children.Add(match(Token_Class.Semicolon));
            return declaration_statement;
        }

        Node IdentifierDec()
        {
            Node identifierDec = new Node("IdentifierDec");

            identifierDec.Children.Add(match(Token_Class.Idenifier));
            identifierDec.Children.Add(Expr());
            return identifierDec;

        }

        Node Idenlist()
        {
            Node idenlist = new Node("Idenlist");
            if (TokenStream.Count > InputPointer && TokenStream[InputPointer].token_type == Token_Class.Comma)
            {
                idenlist.Children.Add(match(Token_Class.Comma));
                idenlist.Children.Add(IdentifierDec());
                idenlist.Children.Add(Idenlist());
                return idenlist;
            }
            return null;
        }

        Node Expr()
        {
            Node expr = new Node("Expr");
            if (TokenStream.Count > InputPointer && TokenStream[InputPointer].token_type == Token_Class.Assagniment)
            {
                expr.Children.Add(match(Token_Class.Assagniment));
                expr.Children.Add(Expression());
                return expr;
            }
            return null;
        }

        Node If_Statement()
        {
            Node if_statement = new Node("If_Statement");

            if_statement.Children.Add(match(Token_Class.If));
            if_statement.Children.Add(Condition_Statement());
            if_statement.Children.Add(match(Token_Class.Then));
            if_statement.Children.Add(Statements());
            if_statement.Children.Add(Flase_Condition());
            if_statement.Children.Add(match(Token_Class.End));
            return if_statement;

        }

        Node Flase_Condition()
        {
            Node flase_condition = new Node("Flase_Condition");
            if (TokenStream.Count > InputPointer)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.Elseif)
                {
                    flase_condition.Children.Add(Else_If_Statments());
                    flase_condition.Children.Add(Flase_Condition());
                    return flase_condition;
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.Else)
                {
                    flase_condition.Children.Add(Else_Statment());
                    return flase_condition;
                }
            }
            return null;

        }

        Node Else_If_Statments()
        {
            Node else_if_statments = new Node("Else_If_Statments");

            else_if_statments.Children.Add(match(Token_Class.Elseif));
            else_if_statments.Children.Add(Condition_Statement());
            else_if_statments.Children.Add(match(Token_Class.Then));
            else_if_statments.Children.Add(Statements());
            return else_if_statments;
        }

        Node Else_Statment()
        {
            Node else_statment = new Node("Else_Statment");

            else_statment.Children.Add(match(Token_Class.Else));
            else_statment.Children.Add(Statements());
            return else_statment;
        }

        Node Condition_Statement()
        {
            Node condition_statement = new Node("Condition_Statement");

            condition_statement.Children.Add(Condition());
            condition_statement.Children.Add(Condition_Helper());
            return condition_statement;
        }

        Node Condition_Helper()
        {
            Node condition_helper = new Node("Condition_Helper");
            if (TokenStream.Count > InputPointer && (TokenStream[InputPointer].token_type == Token_Class.And || TokenStream[InputPointer].token_type == Token_Class.Or))
            {
                condition_helper.Children.Add(Boolean_Operator());
                condition_helper.Children.Add(Condition());
                condition_helper.Children.Add(Condition_Helper());
                return condition_helper;
            }
            return null;

        }

        Node Condition()
        {
            Node condition = new Node("Condition");

            condition.Children.Add(match(Token_Class.Idenifier));
            condition.Children.Add(Condition_Operator());
            condition.Children.Add(Term());
            return condition;
        }

        Node Condition_Operator()
        {
            Node condition_operator = new Node("Condition_Operator");
            if (TokenStream.Count > InputPointer)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.LessThanOp)
                {
                    condition_operator.Children.Add(match(Token_Class.LessThanOp));
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.GreaterThanOp)
                {
                    condition_operator.Children.Add(match(Token_Class.GreaterThanOp));
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.Is_Equal)
                {
                    condition_operator.Children.Add(match(Token_Class.Is_Equal));
                }
                else
                {
                    condition_operator.Children.Add(match(Token_Class.NotEqualOp));
                }
                return condition_operator;
            }
            return null;
            
        }

        Node Boolean_Operator()
        {
            Node boolean_operator = new Node("Boolean_Operator");

            if (TokenStream[InputPointer].token_type == Token_Class.And)
            {
                boolean_operator.Children.Add(match(Token_Class.And));
            }
            else
            {
                boolean_operator.Children.Add(match(Token_Class.Or));
            }
            return boolean_operator;
        }

        Node Repeat_Statement()
        {
            Node repeat_statement = new Node("Repeat_Statement");

            repeat_statement.Children.Add(match(Token_Class.Repeat));
            repeat_statement.Children.Add(Statements());
            repeat_statement.Children.Add(match(Token_Class.Until));
            repeat_statement.Children.Add(Condition_Statement());
            return repeat_statement;
        }
        Node Write_Statement()
        {
            Node write_statement = new Node("Write_Statement");

            write_statement.Children.Add(match(Token_Class.Write));
            write_statement.Children.Add(Write_Rest());
            write_statement.Children.Add(match(Token_Class.Semicolon));
            return write_statement;
        }

        Node Write_Rest()
        {
            Node write_rest = new Node("Write_Rest");

            if (TokenStream[InputPointer].token_type == Token_Class.Endl)
            {
                write_rest.Children.Add(match(Token_Class.Endl));
            }
            else
            {
                write_rest.Children.Add(Expression());
            }

            return write_rest;
        }

        Node Read_Statement()
        {
            Node read_statement = new Node("Read_Statement");

            read_statement.Children.Add(match(Token_Class.Read));
            read_statement.Children.Add(match(Token_Class.Idenifier));
            read_statement.Children.Add(match(Token_Class.Semicolon));
            return read_statement;
        }

        public Node match(Token_Class ExpectedToken)
        {

            if (InputPointer < TokenStream.Count)
            {
                if (ExpectedToken == TokenStream[InputPointer].token_type)
                {
                    InputPointer++;
                    Node newNode = new Node(ExpectedToken.ToString());

                    return newNode;

                }

                else
                {
                    Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + " and " +
                        TokenStream[InputPointer].token_type.ToString() +
                        "  found\r\n");
                    InputPointer++;
                    return null;
                }
            }
            else
            {
                Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + "\r\n");
                InputPointer++;
                return null;
            }
        }

        public static TreeNode PrintParseTree(Node root)
        {
            TreeNode tree = new TreeNode("Parse Tree");
            TreeNode treeRoot = PrintTree(root);
            if (treeRoot != null)
                tree.Nodes.Add(treeRoot);
            return tree;
        }
        static TreeNode PrintTree(Node root)
        {
            if (root == null || root.Name == null)
                return null;
            TreeNode tree = new TreeNode(root.Name);
            if (root.Children.Count == 0)
                return tree;
            foreach (Node child in root.Children)
            {
                if (child == null)
                    continue;
                tree.Nodes.Add(PrintTree(child));
            }
            return tree;
        }
    }
}
