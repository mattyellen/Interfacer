using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Castle.DynamicProxy;
using Interfacer.Exceptions;
using Interfacer.Utility;

namespace Interfacer.Proxies
{
    public abstract class ProxyBase
    {
        protected abstract object WrappedObject { get; }
        protected abstract Type WrappedType { get; }
        protected abstract bool IsMethodTypeStatic { get; }

        protected void InterceptBase(IInvocation invocation)
        {
            var matchingTarget = (from m in WrappedType.GetMethods()
                                  let info = GetMatchingSignatureInfo(m, invocation)
                                  where info.SignaturesMatch
                                  select info).SingleOrDefault();

            if (matchingTarget == null)
            {
                throw new MethodNotFoundException($"Failed to find a method of type {WrappedType} with signature {invocation.Method}");
            }

            var parameterConverters = matchingTarget.ParameterConverters.Select(c => c.Convert()).ToArray();
            var convertedArgs = parameterConverters.Select(a => a.Value).ToArray();

            matchingTarget.ReturnValueConverter.Value = matchingTarget.Method.Invoke(WrappedObject, convertedArgs);
            invocation.ReturnValue = matchingTarget.ReturnValueConverter.Convert().Value;

            for (var i = 0; i < convertedArgs.Length; i++)
            {
                parameterConverters[i].Value = convertedArgs[i];
                invocation.Arguments[i] = parameterConverters[i].ConvertBack().Value;                
            }
        }

        private MethodSignatureInfo GetMatchingSignatureInfo(MethodInfo checkTargetMethod, IInvocation invocation)
        {
            var invokedMethod = invocation.Method;

            if (!checkTargetMethod.IsPublic ||
                checkTargetMethod.IsStatic != IsMethodTypeStatic ||
                checkTargetMethod.Name != invokedMethod.Name ||
                checkTargetMethod.IsGenericMethod != invokedMethod.IsGenericMethod)
            {
                return MethodSignatureInfo.NoMatch();
            }

            if (invocation.GenericArguments != null &&
                checkTargetMethod.IsGenericMethod)
            {
                if (checkTargetMethod.GetGenericArguments().Length == invocation.GenericArguments.Length)
                {
                    checkTargetMethod = checkTargetMethod.MakeGenericMethod(invocation.GenericArguments);
                }
                else
                {
                    return MethodSignatureInfo.NoMatch();
                }
            }

            var sourceParamTypes = invokedMethod.GetParameters().Select(p => p.ParameterType).ToArray();
            var targetParamTypes = checkTargetMethod.GetParameters().Select(p => p.ParameterType).ToArray();

            if (sourceParamTypes.Length != targetParamTypes.Length)
            {
                return MethodSignatureInfo.NoMatch();
            }

            var signatureInfo = new MethodSignatureInfo()
            {
                ParameterConverters = new List<ValueConverter.IConvertableState>()
            };

            var argCount = sourceParamTypes.Length;
            for (var i = 0; i < argCount; i++)
            {
                signatureInfo.ParameterConverters.Add(
                    ValueConverter.For(sourceParamTypes[i], invocation.Arguments[i])
                                  .To(targetParamTypes[i]));
            }

            signatureInfo.Method = checkTargetMethod;

            signatureInfo.ReturnValueConverter =
                ValueConverter.For(checkTargetMethod.ReturnType).To(invokedMethod.ReturnType);

            signatureInfo.SignaturesMatch = 
                signatureInfo.ParameterConverters.All(c => c.CanConvert()) &&
                signatureInfo.ReturnValueConverter.CanConvert();

            return signatureInfo;
        }

        private class MethodSignatureInfo
        {
            public MethodInfo Method { get; set; }
            public bool SignaturesMatch { get; set; }
            public IList<ValueConverter.IConvertableState> ParameterConverters { get; set; }
            public ValueConverter.IConvertableState ReturnValueConverter { get; set; }

            public static MethodSignatureInfo NoMatch()
            {
                return new MethodSignatureInfo()
                {
                    SignaturesMatch = false
                };
            }
        }
    }
}
