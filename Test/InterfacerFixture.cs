using System;
using System.Collections.Generic;
using System.Linq;
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

    [Interfacer(WrappedObjectType.Instance, typeof(TestObject))]
    public interface ITestInterface 
    {
        int Value { get; set; }
        void DoIt();
        int GetValue();
        int GetValue(int num);
        T GetObject<T>() where T : ITag, new();
        Tuple<T, T2> GetObject<T, T2>() 
            where T : new() 
            where T2 : new();
        void GetValueOut(out int val);
        T GetFirst<T>(IEnumerable<T> values);
        event EventHandler<EventArgs> Event;
        void FireEvent();
    }

    //[Interfacer(WrappedObjectType.Factory, typeof(TestObject))]
    public interface ITestFactoryInterface
    {
        ITestInterface Create();
        ITestInterface Create(int value);
    }

    public interface ITag {
    }
    public class TestObject : ITag
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

        public void DoIt()
        {            
        }

        public int GetValue()
        {
            return Value;
        }

        public int GetValue(int num)
        {
            return Value + num;
        }

        public void GetValueOut(out int val)
        {
            val = Value;
        }

        public T GetFirst<T>(IEnumerable<T> values)
        {
            return values.First();
        }

        public T GetObject<T>() where T : ITag, new()
        {
            return new T();
        }

        public Tuple<T, T2> GetObject<T, T2>() 
            where T : new()
            where T2: new()
        {
            return Tuple.Create(new T(), new T2());
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
    public class InterfacerFixture
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

            Assert.That(obj.GetFirst(new List<int> {123}), Is.EqualTo(123));
            Assert.That(obj.GetFirst(new[] {123}), Is.EqualTo(123));

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
