using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Interfacer.Exceptions
{
    public class InvalidTypeException : Exception
    {
        public InvalidTypeException(string message) : base(message)
        {
        }
    }
}
