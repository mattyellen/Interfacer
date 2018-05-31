using System;

namespace Interfacer.Attributes
{
    public abstract class InterfacerBaseAttribute : Attribute
    {
        public readonly Type Class;
        protected InterfacerBaseAttribute(Type @class)
        {
            Class = @class;
        }

        public bool Autogenerate { get; set; }
    }
}
