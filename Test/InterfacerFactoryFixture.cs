using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using ImpromptuInterface;
using ImpromptuInterface.InvokeExt;
using Interfacer;
using NUnit.Framework;

namespace Test
{
    [Interfacer(WrappedObjectType.Static, typeof(TestObject))]
    public interface ITestStaticInterface
    {
        int StaticGetValue();
        T StaticGetObject<T>() where T : new();
        event EventHandler<EventArgs> StaticEvent;
        void FireStaticEvent();
    }

    [Interfacer(WrappedObjectType.Instance, typeof(TestObject))]
    public interface ITestInterface
    {
        int Value { get; set; }
        int GetValue();
        T GetObject<T>() where T : new();
        void GetValueOut(out int val);
        event EventHandler<EventArgs> Event;
        void FireEvent();
    }

    [Interfacer(WrappedObjectType.Factory, typeof(TestObject))]
    public interface ITestFactoryInterface
    {
        ITestInterface Create();
        ITestInterface Create(int value);
    }

    public class TestObject
    {
        public TestObject()
        {
        }

        public TestObject(int value)
        {
            Value = value;
        }

        public event EventHandler<EventArgs> Event;

        public int Value { get; set; }
        public int GetValue()
        {
            return Value;
        }

        public void GetValueOut(out int val)
        {
            val = Value;
        }

        public T GetObject<T>() where T : new()
        {
            return new T();
        }

        public void FireEvent()
        {
            Event?.Invoke(this, EventArgs.Empty);
        }

        public static event EventHandler<EventArgs> StaticEvent;

        public static readonly int StaticValue = 123;
        public static int StaticGetValue()
        {
            return StaticValue;
        }

        public static T StaticGetObject<T>() where T : new()
        {
            return new T();
        }

        public static void StaticGetValueOut(out int val)
        {
            val = StaticValue;
        }

        public static void FireStaticEvent()
        {
            StaticEvent?.Invoke(null, EventArgs.Empty);
        }
    }

    [TestFixture]
    public class InterfacerFactoryFixture
    {
        [Test]
        public void ShouldCreateWrappedInstance()
        {
            var obj = InterfacerFactory.Create<ITestInterface>();
            ValidateTestObject(obj);
        }

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

            Assert.That(((IActLikeProxy)obj).Original, Is.TypeOf<TestObject>());
            Assert.That(obj.Value, Is.EqualTo(testValue));
            Assert.That(obj.GetValue(), Is.EqualTo(testValue));
            Assert.That(obj.GetObject<TestObject>(), Is.InstanceOf<TestObject>());

            int outVal;
            obj.GetValueOut(out outVal);
            Assert.That(outVal, Is.EqualTo(testValue));
        }
    }
}
