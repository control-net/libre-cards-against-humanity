using System;
using System.Collections.Generic;
using System.Linq;

namespace LibreCards.Core
{
    public static class EnumerableExtensions
    {
        private static readonly Random _rng = new Random();

        public static T Random<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException(nameof(enumerable));
            }

            var list = enumerable as IList<T> ?? enumerable.ToList();
            return list.Count == 0 ? default : list[_rng.Next(0, list.Count)];
        }
    }
}
