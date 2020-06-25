using System.Collections.Generic;
using System.Linq;

namespace WebUI.Extensions
{
    public static class EnumerableExtensions
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source) => source == null || !source.Any();

        public static string ToJoinedStringOrEmpty(this IEnumerable<string> source, string separator) => string.Join(separator, source.Where(x => !string.IsNullOrWhiteSpace(x)));
    }
}
