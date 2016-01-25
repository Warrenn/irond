using System;
using Irony.Parsing;

namespace GrammarLibrary
{
    public class StartPageGrammar : Grammar
    {

        public StartPageGrammar() : base(false)
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
            var stringLiteral = new StringLiteral("string", "\"", StringOptions.AllowsAllEscapes);
            stringLiteral.AddStartEnd("'", StringOptions.AllowsAllEscapes);
            var expression = new NonTerminal("Expr");
            SnippetRoots.Add(expression);
            var term = new NonTerminal("Term");
            var binExpr = new NonTerminal("BinExpr");
            var parExpr = new NonTerminal("ParExpr");
            var memberAccess = new NonTerminal("MemberAccess");
            var objectRef = new NonTerminal("ObjectRef");
            var unOp = new NonTerminal("UnOp");
            var actionOp = new NonTerminal("ActionOp");
            var binOp = new NonTerminal("BinOp", "operator");
            var assignmentStmt = new NonTerminal("AssignmentStmt");
            var statement = new NonTerminal("Statement");
            var program = new NonTerminal("Program");
            var whenCondition = new NonTerminal("WhenCondition");
            var actionExpr = new NonTerminal("ActionExpr");
            var actionStmt = new NonTerminal("ActionStmt");

            actionOp.Rule = ToTerm("Hide") | "Disable";
            actionExpr.Rule = actionOp + objectRef;
            actionStmt.Rule = actionExpr + whenCondition;
            whenCondition.Rule = (ToTerm("when") + expression);
            expression.Rule = (term | binExpr);
            term.Rule = (numberLiteral | parExpr | stringLiteral | identifierTerminal | memberAccess);
            parExpr.Rule = "(" + expression + ")";
            unOp.Rule = (ToTerm("not"));
            binExpr.Rule = expression + binOp + expression;
            binOp.Rule = (ToTerm("is") | "is not" | "<" | "<=" | ">" | ">=" | "and" | "or");
            memberAccess.Rule = expression + PreferShiftHere() + "." + identifierTerminal;
            assignmentStmt.Rule = ToTerm("set") + objectRef + "to" + expression + whenCondition;
            statement.Rule = (assignmentStmt | actionStmt | expression | Empty);
            objectRef.Rule = (identifierTerminal | memberAccess);
            program.Rule = MakePlusRule(program, NewLine, statement);
            Root = program;
            RegisterOperators(15, "and", "or");
            RegisterOperators(20, "<", "<=", ">", ">=", "is", "is not");
            RegisterOperators(60, "not");
            MarkPunctuation("(", ")", "[", "]", "the");
            RegisterBracePair("(", ")");
            RegisterBracePair("[", "]");
            MarkTransient(term, expression, statement, binOp, unOp, parExpr);
            AddToNoReportGroup(NewLine);
            LanguageFlags = (LanguageFlags.NewLineBeforeEOF | LanguageFlags.SupportsBigInt);
        }
    }
}
