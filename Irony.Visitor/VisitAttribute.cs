using System;

namespace Irony.Visitor
{
    [AttributeUsage(AttributeTargets.Method)]
    public class VisitAttribute : Attribute
    {
        public VisitAttribute(string name)
        {
            this.Name = name;
        }

        public string Name { get; }
    }
}
