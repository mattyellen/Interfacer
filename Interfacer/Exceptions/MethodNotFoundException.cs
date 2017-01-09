using System;

namespace Interfacer.Exceptions
{
    public class MethodNotFoundException : Exception
    {
        public MethodNotFoundException(string message) : base(message)
        {
        }
    }
}
