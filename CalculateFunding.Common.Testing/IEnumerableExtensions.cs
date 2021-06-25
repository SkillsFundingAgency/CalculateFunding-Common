using System;
using System.Collections.Generic;
using System.Linq;

namespace CalculateFunding.Common.Testing
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<T> Flatten<T>(this IEnumerable<T> enumerable, Func<T, IEnumerable<T>> func)
        {
            enumerable ??= new T[0];

            return enumerable.SelectMany(c => func(c).Flatten(func)).Concat(enumerable);
        }
    }
}
