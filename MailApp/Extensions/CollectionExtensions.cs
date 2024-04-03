using System;
using System.Collections.Generic;

namespace MailApp.Extensions
{
    internal static class CollectionExtensions
    {
        public static int RemoveAny<TItem>(this IList<TItem> list, Predicate<TItem> predicate)
        {
            int index = 0;
            int removed = 0;
            while (index < list.Count)
            {
                var item = list[index];
                if (predicate.Invoke(item))
                {
                    list.RemoveAt(index);
                    removed++;
                    continue;
                }

                index++;
            }

            return removed;
        }
    }
}
