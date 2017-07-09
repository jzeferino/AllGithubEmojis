using System.Collections.Generic;
using System.Linq;

namespace AllGithubEmojis.Core.Extensions
{
    public static class ListExtension
    {
        public static T AddAndPick<T>(this List<T> list, T item)
        {
            list.Add(item);
            return item;
        }

        public static void RemoveWhen<T>(this List<T> list, T item, bool condition)
        {
            if (condition)
            {
                list.Remove(item);
            }
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null)
            {
                return true;
            }

            var collection = enumerable as ICollection<T>;
            if (collection != null)
            {
                return collection.Count < 1;
            }
            return !enumerable.Any();
        }
    }
}
