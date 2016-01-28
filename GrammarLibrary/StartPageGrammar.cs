using System;
using Irony.Parsing;

namespace GrammarLibrary
{
    public class StartPageGrammar : Grammar
    {

        public StartPageGrammar() : base(false)
        {
            var numberLiteral = new NumberLiteral("Number")
            {
                DefaultIntTypes = new[]
                {
                    TypeCode.Int32,
                    TypeCode.Int64
                }
            };
            var identifierTerminal = new IdentifierTerminal("Identifier");
            var stringLiteral = new StringLiteral("String", "\"", StringOptions.AllowsAllEscapes);
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
            var questionTerm = new NonTerminal("QuestionTerm");
            var answerTerm = new NonTerminal("AnswerTerm");
            var objectIdentifier = new NonTerminal("ObjectIdentifier");
            var setObjectIdentifier = new NonTerminal("SetObjectIdentifier");
            var qaIdentifier = new NonTerminal("QAIdentifier");
            var qaProperty = new NonTerminal("QAProperty");

            objectIdentifier.Rule = objectRef | qaIdentifier;
            setObjectIdentifier.Rule = objectRef | qaProperty;
            qaIdentifier.Rule = questionTerm | answerTerm;
            qaProperty.Rule = qaIdentifier + PreferShiftHere()  + "'s " + identifierTerminal;
            actionOp.Rule = ToTerm("hide") | "disable";
            actionExpr.Rule = actionOp + ToTerm("the").Q() + objectIdentifier;
            actionStmt.Rule = actionExpr + whenCondition;
            whenCondition.Rule = (ToTerm("when") + expression);
            expression.Rule = (term | binExpr);
            term.Rule = (numberLiteral | parExpr | stringLiteral | identifierTerminal | memberAccess);
            parExpr.Rule = "(" + expression + ")";
            unOp.Rule = (ToTerm("not"));
            binExpr.Rule = expression + binOp + expression;
            binOp.Rule = (ToTerm("is") | "is not" | "<" | "<=" | ">" | ">=" | "and" | "or");
            memberAccess.Rule = expression + PreferShiftHere() + "." + identifierTerminal;
            assignmentStmt.Rule = ToTerm("set") + ToTerm("the").Q() + setObjectIdentifier + "to" + expression + whenCondition;
            statement.Rule = (assignmentStmt | actionStmt | expression | Empty);
            objectRef.Rule = (identifierTerminal | memberAccess);
            program.Rule = MakePlusRule(program, NewLine, statement);
            answerTerm.Rule = questionTerm + identifierTerminal + ToTerm("answer");
            questionTerm.Rule = identifierTerminal + ToTerm("question");
            Root = program;
            RegisterOperators(15, "and", "or");
            RegisterOperators(20, "<", "<=", ">", ">=", "is", "is not");
            RegisterOperators(60, "not");
            MarkPunctuation("(", ")", "[", "]", "the", "when", "set", "to", "question", "answer");
            RegisterBracePair("(", ")");
            RegisterBracePair("[", "]");
            MarkTransient(term, expression, statement, binOp, unOp, parExpr, actionOp, objectRef, whenCondition,
                objectIdentifier, setObjectIdentifier, qaIdentifier);
            AddToNoReportGroup(NewLine);
            LanguageFlags = (LanguageFlags.NewLineBeforeEOF | LanguageFlags.SupportsBigInt);
        }
    }
}
