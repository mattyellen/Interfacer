using System;

namespace Interfacer.Attributes
{
    [AttributeUsage(AttributeTargets.Interface)]
    public class ApplyToStaticAttribute : InterfacerBaseAttribute
    {
        public ApplyToStaticAttribute(Type @class) : base(@class)
        {
        }
    }
}