using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace CalculateFunding.Common.Extensions
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    static public class IEnumerableExtensions
    {
        static public bool AnyWithNullCheck<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null)
                return false;

            return enumerable.Any();
        }

        static public bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            return !enumerable.AnyWithNullCheck();
        }

        static public decimal? NullableSum<T>(this IEnumerable<T> values, Func<T, decimal?> func)
        {
            IEnumerable<decimal?> nonNullValues = values.Select(_ => func(_)).Where(_ => _ != null);

            if (nonNullValues.AnyWithNullCheck())
            {
                return nonNullValues.Sum();
            }
            else
            {
                return null;
            }
        }
    }
}
