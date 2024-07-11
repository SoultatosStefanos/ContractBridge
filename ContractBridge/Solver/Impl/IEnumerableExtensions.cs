using System.Collections.Generic;
using System.Linq;

namespace ContractBridge.Solver.Impl
{
    internal static class EnumerableExtensions
    {
        public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> self)
        {
            return self.Select((item, index) => (item, index));
        }
    }
}