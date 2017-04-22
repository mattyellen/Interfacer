using System;

namespace Interfacer.Exceptions
{
    public class InvalidTypeException : Exception
    {
        public InvalidTypeException(string message) : base(message)
        {
        }
    }
}
