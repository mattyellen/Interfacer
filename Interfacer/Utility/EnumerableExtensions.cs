using System.Collections.Generic;

namespace Interfacer.Utility
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<Tuple<T1, T2>> ToTuplesWith<T1, T2>(this IEnumerable<T1> e1, IEnumerable<T2> e2)
        {
            using (var enum2 = e2.GetEnumerator())
            {
                foreach (var item1 in e1)
                {
                    var haveItem2 = enum2.MoveNext();
                    yield return new Tuple<T1, T2>(item1, haveItem2 ? enum2.Current : default(T2));
                }
            }                
        }
    }

    public class Tuple<T1, T2>
    {
        public Tuple(T1 item1, T2 item2)
        {
            Item1 = item1;
            Item2 = item2;
        }

        public T1 Item1 { get; }
        public T2 Item2 { get; }
    }
}
