using System;
using System.Collections.Generic;
using System.Linq;
using Interfacer;
using Interfacer.Proxies;
using NUnit.Framework;

namespace Test
{
    [Interfacer(WrappedObjectType.Instance, typeof(TestObject))]
    public interface ITestInterface
    {
        int Value { get; set; }
        void DoIt();
        int GetValue();
        int GetValue(int num);
        int AddValueFromObject(ITestInterface obj);
        T GetObject<T>() where T : new();
        Tuple<T, T2> GetObject<T, T2>()
            where T : new()
            where T2 : new();
        ITestInterface GetNewObject(bool returnNull = false);
        void GetValueOut(out int val);
        void GetValueRef(ref int val);
        void GetObjectOut(out ITestInterface val);
        T GetFirst<T>(IEnumerable<T> values);
        event EventHandler<EventArgs> Event;
        void FireEvent();
        T NoSuchMethod<T>();
        int AddValuesFromArray(int[] vals);
        void GetTripleValue(out int[] vals);
    }

    [TestFixture]
    public class WrappedInstanceFixture
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
            ITestInterface outObject;
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
                Assert.That(s, Is.InstanceOf<TestObject>());
                Assert.That(e, Is.TypeOf<EventArgs>());
            };

            obj.FireEvent();
            Assert.That(firedEvent, Is.True);
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
            var obj = CreateObject();
            var result = CreateObject().AddValueFromObject(obj);

            Assert.That(result, Is.EqualTo(_testValue * 2));
        }

        private ITestInterface CreateObject()
        {
            var obj = InterfacerFactory.Create<ITestInterface>();
            obj.Value = _testValue;
            return obj;
        }
    }
}
