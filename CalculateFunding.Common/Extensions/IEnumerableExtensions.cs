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
    }
}
