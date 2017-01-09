using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Interfacer;
using NUnit.Framework;

namespace Test
{
    //[Interfacer(WrappedObjectType.Static, typeof(TestObject))]
    public interface ITestStaticInterface
    {
        int StaticGetValue();
        T StaticGetObject<T>() where T : new();
        event EventHandler<EventArgs> StaticEvent;
        void FireStaticEvent();
    }

    //[Interfacer(WrappedObjectType.Factory, typeof(TestObject))]
    public interface ITestFactoryInterface
    {
        ITestInterface Create();
        ITestInterface Create(int value);
    }

    [TestFixture]
    public class InterfacerFixture
    {      
        [Test]
        public void ShouldCreateWrappedInstanceForStaticMethods()
        {
            var obj = InterfacerFactory.Create<ITestStaticInterface>();

            Assert.That(obj.StaticGetValue(), Is.EqualTo(TestObject.StaticValue));
            Assert.That(obj.StaticGetObject<TestObject>(), Is.InstanceOf<TestObject>());

            var firedEvent = false;
            obj.StaticEvent += (s, e) =>
            {
                firedEvent = true;
                Assert.That(s, Is.Null);
                Assert.That(e, Is.TypeOf<EventArgs>());
            };

            obj.FireStaticEvent();
            Assert.That(firedEvent, Is.True);
        }

        [Test]
        public void ShouldCreateFactory()
        {
            var testObjectFactory = InterfacerFactory.Create<ITestFactoryInterface>();
            ValidateTestObject(testObjectFactory.Create());

            const int testValue = 12345;
            ValidateTestObject(testObjectFactory.Create(testValue), testValue);
        }

        private static void ValidateTestObject(ITestInterface obj, int? testValue = null)
        {
            if (!testValue.HasValue)
            {
                testValue = 999;
                obj.Value = testValue.Value;
            }

            Assert.That(obj.DoIt, Throws.Nothing);
            Assert.That(obj.Value, Is.EqualTo(testValue));
            Assert.That(obj.GetValue(), Is.EqualTo(testValue));
            Assert.That(obj.GetValue(2), Is.EqualTo(testValue + 2));
            Assert.That(obj.GetObject<TestObject>(), Is.InstanceOf<TestObject>());

            var tuple = obj.GetObject<TestObject, TestObject>();
            Assert.That(tuple.Item1, Is.InstanceOf<TestObject>());
            Assert.That(tuple.Item2, Is.InstanceOf<TestObject>());

            var tuple2 = obj.GetObject<TestObject, List<TestObject>>();
            Assert.That(tuple2.Item1, Is.InstanceOf<TestObject>());
            Assert.That(tuple2.Item2, Is.InstanceOf<List<TestObject>>());

            Assert.That(obj.GetFirst(new List<int> { 123 }), Is.EqualTo(123));
            Assert.That(obj.GetFirst(new[] { 123 }), Is.EqualTo(123));

            int outVal;
            obj.GetValueOut(out outVal);
            Assert.That(outVal, Is.EqualTo(testValue));

            var firedEvent = false;
            obj.Event += (s, e) =>
            {
                firedEvent = true;
                Assert.That(s, Is.InstanceOf<TestObject>());
                Assert.That(e, Is.TypeOf<EventArgs>());
            };

            obj.FireEvent();
            Assert.That(firedEvent, Is.True);
        }



    }
}
