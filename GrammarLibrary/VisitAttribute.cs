using System;

namespace GrammarLibrary
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
