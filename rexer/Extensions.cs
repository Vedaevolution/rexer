using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace rexer
{
    public static class Extensions
    {
        public static IEnumerable<T> ToEnumerable<T>(this IEnumerator<T> enumerator)
        {
            while (enumerator.MoveNext())
                yield return enumerator.Current;
        }
    }
}
