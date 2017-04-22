using System;
using System.Collections.Generic;
using System.Linq;

namespace Test.TestClasses
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

        public int AddValueFromObject(TestObject obj)
        {
            return Value + obj.GetValue();
        }

        public void GetValueOut(out int val)
        {
            val = Value;
        }

        public void GetValueRef(ref int val)
        {
            val = val + Value;
        }

        public void GetObjectOut(out TestObject val)
        {
            val = new TestObject(Value);
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

        public TestObject GetNewObject(bool returnNull = false)
        {
            return returnNull ? null : new TestObject();
        }

        public int AddValuesFromArray(int[] vals)
        {
            return vals.Sum();
        }

        public void GetTripleValue(out int[] vals)
        {
            vals = new[] {Value, Value, Value};
        }

        public void FireEvent()
        {
            Event?.Invoke(this, EventArgs.Empty);
        }
    }
}