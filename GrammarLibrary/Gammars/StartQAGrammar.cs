using System;
using Irony.Parsing;

namespace GrammarLibrary.Gammars
{
    public class StartQAGrammar : Grammar
    {

        public StartQAGrammar() : base(false)
        {
            var numberLiteral = new NumberLiteral("number")
            {
                DefaultIntTypes = new[]
                {
                    TypeCode.Int32,
                    TypeCode.Int64,
                    (TypeCode) 30
                }
            };
            var identifierTerminal = new IdentifierTerminal("identifier");
            var delimiter = ToTerm(",");
            var stringLiteral = new StringLiteral("string", "\"", StringOptions.AllowsAllEscapes);
            stringLiteral.AddStartEnd("'", StringOptions.AllowsAllEscapes);
            var expression = new NonTerminal("Expr");
            SnippetRoots.Add(expression);
            var term = new NonTerminal("Term");
            var binExpr = new NonTerminal("BinExpr");
            var parExpr = new NonTerminal("ParExpr");
            var unExpr = new NonTerminal("UnExpr");
            var ternaryIf = new NonTerminal("TernaryIf");
            var argList = new NonTerminal("ArgList");
            var functionCall = new NonTerminal("FunctionCall");
            var memberAccess = new NonTerminal("MemberAccess");
            var indexAccess = new NonTerminal("IndexedAccess");
            var objectRef = new NonTerminal("ObjectRef");
            var unOp = new NonTerminal("UnOp");
            var binOp = new NonTerminal("BinOp", "operator");
            var prefixIncDec = new NonTerminal("PrefixIncDec");
            var postfixIncDec = new NonTerminal("PostfixIncDec");
            var incDecOp = new NonTerminal("IncDecOp");
            var assignmentStmt = new NonTerminal("AssignmentStmt");
            var assignmentOp = new NonTerminal("AssignmentOp", "assignment operator");
            var statement = new NonTerminal("Statement");
            var program = new NonTerminal("Program");
            expression.Rule = (term | unExpr | binExpr | prefixIncDec | postfixIncDec | ternaryIf);
            term.Rule = (numberLiteral | parExpr | stringLiteral | functionCall | identifierTerminal | memberAccess | indexAccess);
            parExpr.Rule = "(" + expression + ")";
            unExpr.Rule = unOp + term + ReduceHere();
            unOp.Rule = (ToTerm("+") | "-" | "!");
            binExpr.Rule = expression + binOp + expression;
            binOp.Rule = (ToTerm("+") | "-" | "*" | "/" | "**" | "==" | "<" | "<=" | ">" | ">=" | "!=" | "&&" | "||" | "&" | "|");
            prefixIncDec.Rule = incDecOp + identifierTerminal;
            postfixIncDec.Rule = identifierTerminal + PreferShiftHere() + incDecOp;
            incDecOp.Rule = (ToTerm("++") | "--");
            ternaryIf.Rule = expression + "?" + expression + ":" + expression;
            memberAccess.Rule = expression + PreferShiftHere() + "." + identifierTerminal;
            assignmentStmt.Rule = objectRef + assignmentOp + expression;
            assignmentOp.Rule = (ToTerm("=") | "+=" | "-=" | "*=" | "/=");
            statement.Rule = (assignmentStmt | expression | Empty);
            argList.Rule = MakeStarRule(argList, delimiter, expression);
            functionCall.Rule = expression + PreferShiftHere() + "(" + argList + ")";
            functionCall.NodeCaptionTemplate = "call #{0}(...)";
            objectRef.Rule = (identifierTerminal | memberAccess | indexAccess);
            indexAccess.Rule = expression + PreferShiftHere() + "[" + expression + "]";
            program.Rule = MakePlusRule(program, NewLine, statement);
            Root = program;
            RegisterOperators(10, "?");
            RegisterOperators(15, "&", "&&", "|", "||");
            RegisterOperators(20, "==", "<", "<=", ">", ">=", "!=");
            RegisterOperators(30, "+", "-");
            RegisterOperators(40, "*", "/");
            RegisterOperators(50, Associativity.Right, "**");
            RegisterOperators(60, "!");
            MarkPunctuation("(", ")", "?", ":", "[", "]");
            RegisterBracePair("(", ")");
            RegisterBracePair("[", "]");
            MarkTransient(term, expression, statement, binOp, unOp, incDecOp, assignmentOp, parExpr, objectRef);
            MarkNotReported("++", "--");
            AddToNoReportGroup("(", "++", "--");
            AddToNoReportGroup(NewLine);
            AddOperatorReportGroup("operator");
            AddTermsReportGroup("assignment operator", "=", "+=", "-=", "*=", "/=");
            ConsoleTitle = "Irony Expression Evaluator";
            ConsoleGreeting = "Irony Expression Evaluator \r\n\r\n  Supports variable assignments, arithmetic operators (+, -, *, /),\r\n    augmented assignments (+=, -=, etc), prefix/postfix operators ++,--, string operations. \r\n  Supports big integer arithmetics, string operations.\r\n  Supports strings with embedded expressions : \"name: #{name}\"\r\n\r\nPress Ctrl-C to exit the program at any time.\r\n";
            ConsolePrompt = "?";
            ConsolePromptMoreInput = "?";
            LanguageFlags = (LanguageFlags.NewLineBeforeEOF | LanguageFlags.SupportsBigInt);
        }
    }
}
