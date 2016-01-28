using System;
using System.Linq;
using System.Text;
using Irony.Parsing;
using Irony.Visitor;

namespace GrammarLibrary
{
    public class AngularJsVisitor : NodeVisitor<string>
    {
        private readonly JsVisitorContext vistorContext;

        public AngularJsVisitor(string prefix, string questionsProperty, string answersProperty)
        {
            vistorContext = new JsVisitorContext
            {
                Prefix = prefix,
                AnswersProperty = answersProperty,
                QuestionsProperty = questionsProperty
            };
        }

        public AngularJsVisitor() : this("vm.", "questions", "answers")
        {
        }

        private static string ToCammelCase(string term)
        {
            return (string.IsNullOrEmpty(term) || term.Length < 2)
                ? term
                : char.ToLower(term[0]) + term.Substring(1);
        }

        public string VisitQAProperty(ParseTreeNode node, object context)
        {
            var prefix = (context as JsVisitorContext)?.Prefix;
            var objectString = Visit(node.ChildNodes[0]);
            var propertyString = Visit(node.ChildNodes[2]);

            return $"{prefix}{objectString}.{propertyString}";
        }

        public string VisitNumber(ParseTreeNode node, object context)
        {
            return ToCammelCase(node.Token.Text);
        }

        public string VisitAnswerTerm(ParseTreeNode node, object context)
        {
            var jsContext = (context as JsVisitorContext);
            var prefix = jsContext?.Prefix;
            var answersProperty = jsContext?.AnswersProperty;
            var questionIdentifier = Visit(node.ChildNodes[0]);
            var propertyString = Visit(node.ChildNodes[1]);

            return $"{prefix}{questionIdentifier}.{answersProperty}.{propertyString}";
        }

        public string VisitQuestionTerm(ParseTreeNode node, object context)
        {
            var jsContext = (context as JsVisitorContext);
            var prefix = jsContext?.Prefix;
            var questionsProperty = jsContext?.QuestionsProperty;
            var identifier = Visit(node.ChildNodes[0]);

            return $"{prefix}{questionsProperty}.{identifier}";
        }

        public string VisitIdentifier(ParseTreeNode node, object context)
        {
            var prefix = (context as JsVisitorContext)?.Prefix;

            return $"{prefix}{ToCammelCase(node.Token.Text)}";
        }

        public string VisitString(ParseTreeNode node, object context)
        {
            return ToCammelCase(node.Token.Text);
        }

        public string VisitMemberAccess(ParseTreeNode node, object context)
        {
            var prefix = (context as JsVisitorContext)?.Prefix;
            var objectString = Visit(node.ChildNodes[0]);
            var propertyString = Visit(node.ChildNodes[2]);

            return $"{prefix}{objectString}.{propertyString}";
        }

        public string VisitActionStmt(ParseTreeNode node, object context)
        {
            var actionExp = node.ChildNodes[0];
            var property = ToCammelCase(actionExp.ChildNodes[0].Token.Text);
            var member = Visit(actionExp.ChildNodes[2], vistorContext);
            var evalExp = Visit(node.ChildNodes[1]);

            return 
                $"   if({evalExp}){{\r\n" +
                $"       {member}.{property} = true;\r\n" +
                "   }\r\n";
        }

        public string VisitAssignmentStmt(ParseTreeNode node, object context)
        {
            var fullPropertyExp = Visit(node.ChildNodes[1], vistorContext);
            var newvalue = Visit(node.ChildNodes[2], vistorContext);
            var evalExp = Visit(node.ChildNodes[3]);

           return
                $"   if({evalExp}){{\r\n" +
                $"       {fullPropertyExp} = {newvalue};\r\n" +
                "   }\r\n";
        }

        public string VisitBinExpr(ParseTreeNode node, object context)
        {
            var left = Visit(node.ChildNodes[0], vistorContext);
            var op = node.ChildNodes[1];
            var right = Visit(node.ChildNodes[2], vistorContext);

            switch (op.Token.Text)
            {
                case "is":
                    return $"({left} == {right})";
                case "is not":
                    return $"({left} != {right})";
                case "<":
                    return $"({left} < {right})";
                case ">":
                    return $"({left} > {right})";
                case ">=":
                    return $"({left} >= {right})";
                case "<=":
                    return $"({left} <= {right})";
                case "and":
                    return $"({left} && {right})";
                case "or":
                    return $"({left} || {right})";
            }
            throw new InvalidOperationException(op.Term.Name);
        }

        public string VisitProgram(ParseTreeNode node, object context)
        {
            var stringBuilder = new StringBuilder();
            for (var index = 0; index < node.ChildNodes.Count(); index++)
            {
                var childNode = node.ChildNodes[index];
                var line = Visit(childNode);
                stringBuilder.Append(line);
            }

            return stringBuilder.ToString();
        }
    }
}