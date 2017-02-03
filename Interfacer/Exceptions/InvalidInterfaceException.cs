using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Interfacer.Exceptions
{
    public class InvalidInterfaceException : Exception
    {
        public InvalidInterfaceException(Type interfaceType, IEnumerable<MethodNotFoundException> missingMethods)
        {
            InterfaceType = interfaceType;
            MissingMethods = missingMethods;
        }

        public Type InterfaceType { get; }
        public IEnumerable<MethodNotFoundException> MissingMethods { get; }

        public override string Message =>
            $"Interface {InterfaceType} isn't valid.  Some methods could not be matched on the target type.\n" +
            MissingMethodMessages;

        public string MissingMethodMessages =>
            string.Join("\n", MissingMethods.Select(m => "\t" + m.Message).ToArray());
    }
}
