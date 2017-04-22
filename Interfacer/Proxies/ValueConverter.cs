using System;
using Castle.DynamicProxy;
using Interfacer.Exceptions;
using Interfacer.Utility;

namespace Interfacer.Proxies
{
    internal class ValueConverter :
        ValueConverter.IInitialState,
        ValueConverter.IConvertableState,
        ValueConverter.IConvertedState,
        ValueConverter.IFinalState
    {
        private readonly Type _currentType;
        private Type _targetType;
        public object Value { get; set; }

        public interface IHasValue
        {
            object Value { get; }
        }

        public interface ICanSetValue
        {
            object Value { get; set; }
        }

        public interface IInitialState : IHasValue
        {
            IConvertableState To(Type newType);
        }

        public interface IConvertableState : ICanSetValue
        {
            bool CanConvert();
            IConvertedState Convert();
        }

        public interface IConvertedState : ICanSetValue
        {
            IFinalState ConvertBack();
        }

        public interface IFinalState : IHasValue
        {
        }

        public static IInitialState For(Type type)
        {
            return For(type, null);
        }

        public static IInitialState For(Type type, object value)
        {
            return new ValueConverter(type, value);
        }

        private ValueConverter(Type type, object value)
        {
            type = RemoveRef(type);
            if (value != null && !type.IsInstanceOfType(value))
            {
                throw new InvalidTypeException($"Value is an instance of {value.GetType()}, but must be {type} or null");
            }

            _targetType = null;
            _currentType = type;

            Value = value;
        }

        private ValueConverter(Type fromType, Type toType, object value)
        {
            _targetType = fromType;
            _currentType = toType;

            if (fromType == toType || value == null)
            {
                Value = value;
                return;
            }

            var wrappedValue = TryWrapObject(fromType, toType, value);
            if (wrappedValue != null)
            {
                Value = wrappedValue;
                return;
            }

            var unwrappedValue = TryUnwrapObject(fromType, toType, value);
            if (unwrappedValue != null)
            {
                Value = unwrappedValue;
                return;
            }

            throw new InvalidTypeException($"Value is an instance of {value.GetType()}, it cannot be converted from {fromType} to {toType}");
        }

        private Type RemoveRef(Type type)
        {
            return type.IsByRef ? 
                type.GetElementType() : 
                type;
        }

        public IConvertableState To(Type newType)
        {
            _targetType = RemoveRef(newType);
            return this;
        }

        public bool CanConvert()
        {
            return _currentType == _targetType ||
                   _currentType.IsInstanceInterfaceOf(_targetType) ||
                   _targetType.IsInstanceInterfaceOf(_currentType);
        }

        public IConvertedState Convert()
        {
            return new ValueConverter(_currentType, _targetType, Value);
        }

        public IFinalState ConvertBack()
        {
            return new ValueConverter(_currentType, _targetType, Value);
        }

        private object TryWrapObject(Type fromType, Type toType, object value)
        {
            return toType.IsInstanceInterfaceOf(fromType)
                ? CreateInstanceProxyForObject(toType, value)
                : null;
        }

        private object TryUnwrapObject(Type fromType, Type toType, object value)
        {
            return fromType.IsInstanceInterfaceOf(toType)
                ? ((ICanGetWrappedObject)value).__InterfacerWrappedObject__
                : null;
        }

        private object CreateInstanceProxyForObject(Type interfacerType, object wrappedObject)
        {
            var proxyGenerator = new ProxyGenerator();
            return proxyGenerator.CreateInterfaceProxyWithoutTarget(interfacerType, new[] { typeof(ICanGetWrappedObject) }, new InstanceProxy(wrappedObject));
        }
    }
}
