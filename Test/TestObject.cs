using System;
using System.Collections.Generic;
using System.Linq;

namespace Test
{
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

        public T GetObject<T>() where T : new()
        {
            return new T();
        }

        public Tuple<T, T2> GetObject<T, T2>() 
            where T : new()
            where T2: new()
        {
            return Tuple.Create(new T(), new T2());
        }

        public TestObject GetNewObject()
        {
            return new TestObject();
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
}