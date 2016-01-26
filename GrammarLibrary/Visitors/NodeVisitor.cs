using System;
using System.Collections.Generic;
using System.Linq;
using Irony.Parsing;

namespace GrammarLibrary.Visitors
{
    public abstract class NodeVisitor<T>
    {
        protected readonly IDictionary<string, Func<ParseTreeNode, object, T>> Visitors =
            new SortedDictionary<string, Func<ParseTreeNode, object, T>>(StringComparer.CurrentCultureIgnoreCase);

        protected NodeVisitor()
        {
            var type = GetType();
            var methods =
                from method in type.GetMethods()
                let parameters = method.GetParameters()
                let attributes = method.GetCustomAttributes(typeof (VisitAttribute), true)
                where (
                    (method.Name.StartsWith("Visit") || (attributes != null && attributes.Any())) &&
                    ((typeof (T)).IsAssignableFrom(method.ReturnType)) &&
                    (parameters.Length == 2) &&
                    (parameters[0].ParameterType == typeof (ParseTreeNode) &&
                     (parameters[1].ParameterType == typeof (object))) &&
                    (method.IsPublic))
                select new {method, attributes};

            foreach (var visitMethod in methods)
            {
                var method = visitMethod.method;
                var attributes = visitMethod.attributes;
                var name = attributes.Any()
                    ? ((VisitAttribute) attributes[0]).Name
                    : method.Name.Replace("Visit", string.Empty);
                if (string.IsNullOrEmpty(name))
                {
                    continue;
                }
                Visitors[name] = (node, ctx) => (T) method.Invoke(this, new[] {node, ctx});
            }
        }

        public virtual T Visit(ParseTreeNode node, object context = null)
        {
            var key = node.Term.Name;
            return
                Visitors.ContainsKey(key)
                    ? Visitors[key](node, context)
                    : default(T);
        }
    }
}
