﻿using System;
using System.Collections.Generic;
using System.Reflection;

namespace Interfacer.Exceptions
{
    public class ConstructorNotFoundException : MethodNotFoundException
    {
        public ConstructorNotFoundException(Type targetType, MethodInfo method) : 
            base(targetType, method)
        {
        }

        public ConstructorNotFoundException(Type targetType, MethodInfo method, IEnumerable<MethodBase> possibleMatches) : 
            base(targetType, method, possibleMatches)
        {
        }

        public override string MessagePrefix => "Failed to find a constructor for";
    }
}
