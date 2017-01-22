using System;

namespace Interfacer.Attributes
{
    [AttributeUsage(AttributeTargets.Interface)]
    public class ApplyToInstanceAttribute : InterfacerBaseAttribute
    {
        public ApplyToInstanceAttribute(Type @class) : base(@class)
        {            
        }
    }
}