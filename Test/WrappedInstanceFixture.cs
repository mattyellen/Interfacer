using System;
using System.Collections.Generic;
using Interfacer;
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
        T GetObject<T>() where T : new();
        Tuple<T, T2> GetObject<T, T2>()
            where T : new()
            where T2 : new();
        ITestInterface GetNewObject();
        void GetValueOut(out int val);
        T GetFirst<T>(IEnumerable<T> values);
        event EventHandler<EventArgs> Event;
        void FireEvent();
        T NoSuchMethod<T>();
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
        public void ShouldReturnNewWrappedObject()
        {
            var obj = CreateObject().GetNewObject();

            Assert.That(obj.DoIt, Throws.Nothing);
        }

        private ITestInterface CreateObject()
        {
            var obj = InterfacerFactory.Create<ITestInterface>();
            obj.Value = _testValue;
            return obj;
        }
    }
}
