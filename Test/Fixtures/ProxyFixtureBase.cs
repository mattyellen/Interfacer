using System;
using System.Collections.Generic;
using System.Linq;
using Interfacer;
using Interfacer.Exceptions;
using NUnit.Framework;
using TestClasses;

#if !NET35
using Castle.MicroKernel.Registration;
using Castle.Windsor;
#endif

namespace Test.Fixtures
{
    public interface ITestObjectBase : ITestObjectValidBase
    {
        T NoSuchMethod<T>();
    }

    public interface ITestObjectValidBase
    {
        int Value { get; set; }
        void DoIt();
        int GetValue();
        int GetValue(int num);
        int AddValueFromObject(ITestObject obj);
        T GetObject<T>() where T : new();
        Tuple<T, T2> GetObject<T, T2>()
            where T : new()
            where T2 : new();
        Tuple<Tuple<T, T2>, Tuple<T, T2>> GetTupleTuple<T, T2>()
            where T : new()
            where T2 : new();
        ITestObject GetNewObject(bool returnNull = false);
        void GetValueOut(out int val);
        void GetValueRef(ref int val);
        void GetObjectOut(out ITestObject val);
        T GetFirst<T>(IEnumerable<T> values);
        event EventHandler<EventArgs> Event;
        void FireEvent();
        int AddValuesFromArray(int[] vals);
        void GetTripleValue(out int[] vals);
        int AddValuesFromParams(params int[] vals);
    }

    public class ProxyFixtureBase<TInterface, TValidInterface> 
        where TInterface : class, ITestObjectBase 
        where TValidInterface : class, ITestObjectValidBase
    {
        private int _testValue = 999;

        [Test]
        public void ShouldSupportMethodWithNoArgsAndNoReturn()
        {
            Assert.That(CreateObject().DoIt, Throws.Nothing);
        }

        [Test]
        public void ShouldSupportPropertyGetter()
        {
            Assert.That(CreateObject().Value, Is.EqualTo(_testValue));
        }

        [Test]
        public void ShouldSupportMethodWithNoArgs()
        {
            Assert.That(CreateObject().GetValue(), Is.EqualTo(_testValue));
        }

        [Test]
        public void ShouldSupportMethodWithSingleArg()
        {
            Assert.That(CreateObject().GetValue(2), Is.EqualTo(_testValue + 2));
        }

        [Test]
        public void ShouldSupportGenericMethod()
        {
            Assert.That(CreateObject().GetObject<TestObject>(), Is.InstanceOf<TestObject>());
        }

        [Test]
        public void ShouldSupportMethodWithMultipleGenericParameters()
        {
            var tuple = CreateObject().GetObject<TestObject, TestObject>();
            Assert.That(tuple.Item1, Is.InstanceOf<TestObject>());
            Assert.That(tuple.Item2, Is.InstanceOf<TestObject>());
        }

        [Test]
        public void ShouldSupportMethodWithGenericParameterWithGenericTypes()
        {
            var tuple2 = CreateObject().GetObject<TestObject, List<TestObject>>();
            Assert.That(tuple2.Item1, Is.InstanceOf<TestObject>());
            Assert.That(tuple2.Item2, Is.InstanceOf<List<TestObject>>());
        }

        [Test]
        public void ShouldSupportMethodWithComplexGenericParameterTypes()
        {
            var tuple2 = CreateObject().GetTupleTuple<TestObject, List<TestObject>>();
            Assert.That(tuple2.Item1.Item1, Is.InstanceOf<TestObject>());
            Assert.That(tuple2.Item1.Item2, Is.InstanceOf<List<TestObject>>());
            Assert.That(tuple2.Item2.Item1, Is.InstanceOf<TestObject>());
            Assert.That(tuple2.Item2.Item2, Is.InstanceOf<List<TestObject>>());
        }

        [Test]
        public void ShouldSupportGenericMethodWithCovarientParameter()
        {
            Assert.That(CreateObject().GetFirst(new List<int> { 123 }), Is.EqualTo(123));
            Assert.That(CreateObject().GetFirst(new[] { 123 }), Is.EqualTo(123));
        }

        [Test]
        public void ShouldSupportMethodWithOutParameter()
        {
            int outVal;
            CreateObject().GetValueOut(out outVal);
            Assert.That(outVal, Is.EqualTo(_testValue));
        }

        [Test]
        public void ShouldSupportMethodWithArrayParameter()
        {
            int[] vals = {123,456,789};
            int result = CreateObject().AddValuesFromArray(vals);
            Assert.That(result, Is.EqualTo(vals.Sum()));
        }

        [Test]
        public void ShouldSupportMethodWithOutArrayParameter()
        {
            int[] vals;
            CreateObject().GetTripleValue(out vals);
            Assert.That(vals, Is.EquivalentTo(new[] {_testValue, _testValue, _testValue}));
        }

        [Test]
        public void ShouldSupportMethodWithRefParameter()
        {
            var localTestValue = 123;
            var refVal = localTestValue;
            CreateObject().GetValueRef(ref refVal);
            Assert.That(refVal, Is.EqualTo(_testValue + localTestValue));
        }

        [Test]
        public void ShouldSupportMethodWithWrappedOutParamter()
        {
            ITestObject outObject;
            CreateObject().GetObjectOut(out outObject);

            Assert.That(outObject.Value, Is.EqualTo(_testValue));
        }

        [Test]
        public void ShouldSupportEvents()
        {
            var firedEvent = false;
            var obj = CreateObject();
            obj.Event += (s, e) =>
            {
                firedEvent = true;
                if (this is StaticFixture)
                {
                    Assert.That(s, Is.Null);
                }
                else if (this is InstanceFixture)
                {
                    Assert.That(s, Is.InstanceOf<TestObject>());
                }
                Assert.That(e, Is.TypeOf<EventArgs>());
            };

            obj.FireEvent();
            Assert.That(firedEvent, Is.True);
        }

        [Test]
        public void ShouldSupportMethodWithDynamicParameters()
        {
            var result = CreateObject().AddValuesFromParams(123, 456, 789);
            Assert.That(result, Is.EqualTo(new[]{ 123, 456, 789}.Sum()));
        }


        [Test]
        public void ShouldThrowIfMethodNotFoundOnWrappedObject()
        {
            Assert.That(() => CreateObject().NoSuchMethod<int>(), Throws.Exception.With.Message.Contains("NoSuchMethod"));
        }

        [Test]
        public void ShouldReturnNewInterfacerWrappedObject()
        {
            var obj = CreateObject().GetNewObject();

            Assert.That(obj.DoIt, Throws.Nothing);
        }

        [Test]
        public void ShouldNotReturnInterfacerWrappedObjectIfOriginalMethodReturnsNull()
        {
            var obj = CreateObject().GetNewObject(true);

            Assert.That(obj, Is.Null);
        }

        [Test]
        public void ShouldHandleInterfacerWrappedArguments()
        {
            var obj = CreateObjectForInterface<ITestObject>();
            var result = CreateObject().AddValueFromObject(obj);

            Assert.That(result, Is.EqualTo(_testValue * 2));
        }

        [Test]
        public void VerifyShouldNotThrowForValidInterface()
        {
            Assert.That(InterfacerFactory.Verify<TValidInterface>, Throws.Nothing);
        }

        [Test]
        public void VerifyShouldThrowForInvalidInterface()
        {
            Assert.That(InterfacerFactory.Verify<TInterface>, Throws.Exception.TypeOf<InvalidInterfaceException>());
        }

        #if !NET35
        [Test]
        public void ShouldBeResolvableWithCastleWindsor()
        {
            var container = new WindsorContainer();
            container.Register(Component.For<TInterface>()
                .UsingFactoryMethod(InterfacerFactory.Create<TInterface>));

            container.Resolve<TInterface>();
        }
        #endif
        
        private TInterface CreateObject()
        {
            return CreateObjectForInterface<TInterface>();
        }

        private T CreateObjectForInterface<T>() where T : class, ITestObjectBase
        {
            var obj = InterfacerFactory.Create<T>();
            obj.Value = _testValue;
            return obj;
        }
    }
}
