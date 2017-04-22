using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;
using Interfacer.Attributes;
using Interfacer.Exceptions;

namespace Interfacer.Proxies
{
    internal abstract class ProxyBase
    {
        protected abstract object WrappedObject { get; }
        protected abstract Type WrappedType { get; }
        protected abstract bool IsMethodTypeStatic { get; }
        protected abstract IEnumerable<MethodSignatureInfo> GetMatchingSignatureInfo(
            IInvocation invocation);

        protected void InterceptBase(IInvocation invocation)
        {
            var matchingTarget = (from m in GetMatchingSignatureInfo(invocation)
                                  where m.SignaturesMatch
                                  select m).SingleOrDefault();

            if (matchingTarget == null)
            {
                throw IsConstructorCall(invocation)
                    ? new ConstructorNotFoundException(WrappedType, invocation.Method)
                    : new MethodNotFoundException(WrappedType, invocation.Method);
            }

            var parameterConverters = matchingTarget.ParameterConverters.Select(c => c.Convert()).ToArray();
            var convertedArgs = parameterConverters.Select(a => a.Value).ToArray();

            var constructor = matchingTarget.Method as ConstructorInfo;
            var result = constructor != null
                ? constructor.Invoke(convertedArgs)
                : matchingTarget.Method.Invoke(WrappedObject, convertedArgs);

            matchingTarget.ReturnValueConverter.Value = result;
            invocation.ReturnValue = matchingTarget.ReturnValueConverter.Convert().Value;

            for (var i = 0; i < convertedArgs.Length; i++)
            {
                parameterConverters[i].Value = convertedArgs[i];
                invocation.Arguments[i] = parameterConverters[i].ConvertBack().Value;                
            }
        }

        protected bool IsConstructorCall(IInvocation invocation)
        {
            return invocation.Method.GetCustomAttributes(typeof(ConstructorAttribute), true).Any();
        }

        protected MethodSignatureInfo GetMatchingSignatureInfoForMethod(
            MethodInfo checkTargetMethod,
            IInvocation invocation)
        {
            var invokedMethod = invocation.Method;

            if (!checkTargetMethod.IsPublic ||
                checkTargetMethod.IsStatic != IsMethodTypeStatic ||
                checkTargetMethod.Name != invokedMethod.Name ||
                checkTargetMethod.IsGenericMethod != invokedMethod.IsGenericMethod)
            {
                return new MethodSignatureInfo();
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
                    return new MethodSignatureInfo();
                }
            }

            return new MethodSignatureInfo
            {
                ParameterConverters = GetParameterConverters(checkTargetMethod, invocation),
                Method = checkTargetMethod,
                ReturnValueConverter = ValueConverter.For(checkTargetMethod.ReturnType).To(invokedMethod.ReturnType)
            };
        }

        protected IList<ValueConverter.IConvertableState> GetParameterConverters(MethodBase checkTargetMethod, IInvocation invocation)
        {
            var invokedMethod = invocation.Method;

            var sourceParamTypes = invokedMethod.GetParameters().Select(p => p.ParameterType).ToArray();
            var targetParamTypes = checkTargetMethod.GetParameters().Select(p => p.ParameterType).ToArray();

            if (sourceParamTypes.Length != targetParamTypes.Length)
            {
                return null;
            }

            var parameterConverters = new List<ValueConverter.IConvertableState>();
            var argCount = sourceParamTypes.Length;
            for (var i = 0; i < argCount; i++)
            {
                parameterConverters.Add(
                    ValueConverter.For(sourceParamTypes[i], invocation.Arguments[i])
                                  .To(targetParamTypes[i]));
            }

            return parameterConverters;
        }

        protected class MethodSignatureInfo
        {
            public MethodBase Method { get; set; }
            public IList<ValueConverter.IConvertableState> ParameterConverters { get; set; }
            public ValueConverter.IConvertableState ReturnValueConverter { get; set; }

            public bool SignaturesMatch
            {
                get
                {
                    return ParameterConverters != null &&
                           ReturnValueConverter != null &&
                           ParameterConverters.All(c => c.CanConvert()) &&
                           ReturnValueConverter.CanConvert();
                }
            }
        }
    }
}
