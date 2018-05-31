using System;
using System.Collections.Generic;
using System.Linq;

namespace Test.TestClasses
{
    public static class TestStaticClass
    {
        public static event EventHandler<EventArgs> Event;

        public static int Value { get; set; }

        public static void DoIt()
        {
        }

        public static int GetValue()
        {
            return Value;
        }

        public static int GetValue(int num)
        {
            return Value + num;
        }

        public static int AddValueFromObject(TestObject obj)
        {
            return Value + obj.GetValue();
        }

        public static void GetValueOut(out int val)
        {
            val = Value;
        }

        public static void GetValueRef(ref int val)
        {
            val = val + Value;
        }

        public static void GetObjectOut(out TestObject val)
        {
            val = new TestObject(Value);
        }

        public static T GetFirst<T>(IEnumerable<T> values)
        {
            return values.First();
        }

        public static T GetObject<T>() where T : new()
        {
            return new T();
        }

        public static Tuple<T, T2> GetObject<T, T2>()
            where T : new()
            where T2 : new()
        {
            return Tuple.Create(new T(), new T2());
        }

        public static Tuple<Tuple<T, T2>, Tuple<T, T2>> GetTupleTuple<T, T2>()
            where T : new()
            where T2 : new()
        {
            return Tuple.Create(Tuple.Create(new T(), new T2()), Tuple.Create(new T(), new T2()));
        }

        public static TestObject GetNewObject(bool returnNull = false)
        {
            return returnNull ? null : new TestObject();
        }

        public static int AddValuesFromArray(int[] vals)
        {
            return vals.Sum();
        }

        public static void GetTripleValue(out int[] vals)
        {
            vals = new[] { Value, Value, Value };
        }

        public static void FireEvent()
        {
            Event?.Invoke(null, EventArgs.Empty);
        }

        public static int AddValuesFromParams(params int[] vals)
        {
            return vals.Sum();
        }
    }
}