using System;
using System.Linq;
using System.Text;
using Irony.Parsing;

namespace GrammarLibrary.Visitors
{
    public class AngularJsVisitor : NodeVisitor<string>
    {
        public string VisitNumber(ParseTreeNode node, object context)
        {
            return node.Token.Text;
        }

        public string VisitIdentifier(ParseTreeNode node, object context)
        {
            var prefix = (context as JsVisitorContext)?.Prefix;
            return $"{prefix}{node.Token.Text}";
        }

        public string VisitString(ParseTreeNode node, object context)
        {
            return node.Token.Text;
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
            var property = actionExp.ChildNodes[0].Token.Text;
            var newContext = new JsVisitorContext {Prefix = "vm."};
            var member = Visit(actionExp.ChildNodes[1], newContext);
            var evalExp = Visit(node.ChildNodes[1]);
            var returnValue =
                $"Object.defineProperty({member}, '{property}',{{\r\n" +
                "get: function() {\r\n" +
                $"   if({evalExp}){{\r\n" +
                "       return true;\r\n" +
                "   }\r\n" +
                "   return false;\r\n" +
                "}});\r\n";
            return returnValue;
        }

        public string VisitAssignmentStmt(ParseTreeNode node, object context)
        {
            var newContext = new JsVisitorContext { Prefix = "vm." };
            var fullPropertyExp = Visit(node.ChildNodes[0], newContext);
            var lastIdentifier = fullPropertyExp.LastIndexOf('.');
            var member = fullPropertyExp.Substring(0, lastIdentifier);
            var property = fullPropertyExp.Substring(lastIdentifier + 1);
            var newvalue = Visit(node.ChildNodes[1], newContext);
            var evalExp = Visit(node.ChildNodes[2]);

            var returnValue =
                $"Object.defineProperty({member}, '{property}',{{\r\n" +
                "get: function() {\r\n" +
                $"   if({evalExp}){{\r\n" +
                $"       return {newvalue};\r\n" +
                "   }\r\n" +
                "   return null;\r\n" +
                "}});\r\n";
            return returnValue;
        }

        public string VisitBinExpr(ParseTreeNode node, object context)
        {
            var newContext = new JsVisitorContext {Prefix = "vm."};
            var left = Visit(node.ChildNodes[0], newContext);
            var op = node.ChildNodes[1];
            var right = Visit(node.ChildNodes[2], newContext);

            switch (op.Token.Text)
            {
                case "is":
                    return $"({left} === {right})";
                case "is not":
                    return $"({left} !== {right})";
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