using System;
using System.Linq;
using System.Text;
using Irony.Parsing;
using Irony.Visitor;

namespace GrammarLibrary
{
    public class AngularJsVisitor : NodeVisitor<string>
    {
        private readonly string prefix;
        private readonly string answersProperty;
        private readonly string questionsProperty;

        public AngularJsVisitor(string prefix, string questionsProperty, string answersProperty)
        {
            this.prefix = prefix;
            this.answersProperty = answersProperty;
            this.questionsProperty = questionsProperty;
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
            var objectString = Visit(node.ChildNodes[0]);
            var propertyString = Visit(node.ChildNodes[2]);

            return $"{context}{objectString}.{propertyString}";
        }

        public string VisitNumber(ParseTreeNode node, object context)
        {
            return ToCammelCase(node.Token.Text);
        }

        public string VisitAnswerTerm(ParseTreeNode node, object context)
        {
            var questionIdentifier = Visit(node.ChildNodes[0]);
            var propertyString = Visit(node.ChildNodes[1]);

            return $"{context}{questionIdentifier}.{answersProperty}.{propertyString}";
        }

        public string VisitQuestionTerm(ParseTreeNode node, object context)
        {
            var identifier = Visit(node.ChildNodes[0]);

            return $"{context}{questionsProperty}.{identifier}";
        }

        public string VisitIdentifier(ParseTreeNode node, object context)
        {
            return $"{context}{ToCammelCase(node.Token.Text)}";
        }

        public string VisitString(ParseTreeNode node, object context)
        {
            return node.Token.Text;
        }

        public string VisitConstant(ParseTreeNode node, object context)
        {
            return ToCammelCase(node.Token.Text);
        }

        public string VisitMemberAccess(ParseTreeNode node, object context)
        {
            var objectString = Visit(node.ChildNodes[0]);
            var propertyString = Visit(node.ChildNodes[2]);

            return $"{context}{objectString}.{propertyString}";
        }

        public string VisitActionStmt(ParseTreeNode node, object context)
        {
            var actionExp = node.ChildNodes[0];
            var property = ToCammelCase(actionExp.ChildNodes[0].Token.Text);
            var member = Visit(actionExp.ChildNodes[2], prefix);
            var evalExp = Visit(node.ChildNodes[1]);

            return 
                $"   if({evalExp}){{\r\n" +
                $"       {member}.{property} = true;\r\n" +
                "  }else{" +
                $"       {member}.{property} = false;\r\n" +
                "   }\r\n";
        }

        public string VisitAssignmentStmt(ParseTreeNode node, object context)
        {
            var fullPropertyExp = Visit(node.ChildNodes[1], prefix);
            var newvalue = Visit(node.ChildNodes[2], prefix);
            var evalExp = Visit(node.ChildNodes[3]);

           return
                $"   if({evalExp}){{\r\n" +
                $"       {fullPropertyExp} = {newvalue};\r\n" +
                "   }\r\n";
        }

        public string VisitBinExpr(ParseTreeNode node, object context)
        {
            var left = Visit(node.ChildNodes[0], prefix);
            var op = node.ChildNodes[1];
            var right = Visit(node.ChildNodes[2], prefix);

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