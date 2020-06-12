using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interlex.BusinessLayer.Helpers
{
    public static class EnumerableExtensions
    {
        public static string ToString(this IEnumerable<string> collection, string separator)
        {
            return string.Join(separator, collection);
        }
    }
}
