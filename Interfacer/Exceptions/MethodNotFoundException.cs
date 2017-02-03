using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Interfacer.Exceptions
{
    public class MethodNotFoundException : Exception
    {
        public MethodNotFoundException(Type targetType, MethodInfo method) : this(targetType, method, null)
        {
        }

        public MethodNotFoundException(Type targetType, MethodInfo method, IEnumerable<MethodBase> possibleMatches)
        {
            TargetType = targetType;
            Method = method;
            PossibleMatches = possibleMatches;
        }

        public Type TargetType { get; }
        public MethodInfo Method { get; }
        public IEnumerable<MethodBase> PossibleMatches { get; }

        public virtual string MessagePrefix => "Failed to find a method on";

        public override string Message
        {
            get
            {
                var possibleMatches =
                    PossibleMatches != null && PossibleMatches.Any()
                        ? string.Join("; ", PossibleMatches.Select(m => m.ToString()).ToArray())
                        : null;

                var possibleMatchMessage = possibleMatches != null
                    ? $"  Maybe it should be one of: {possibleMatches}"
                    : string.Empty;

                return $"{MessagePrefix} {TargetType} with matching " +
                       $"signature for: {Method}.{possibleMatchMessage}";
            }
        }
    }
}
