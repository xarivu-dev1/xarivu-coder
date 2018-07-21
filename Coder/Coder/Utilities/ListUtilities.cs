using System.Collections.Generic;
using System.Linq;

namespace Xarivu.Coder.Utilities
{
    public static class ListUtilities
    {
        public static bool HasElements<T>(IEnumerable<T> list)
        {
            return list != null && list.Count() > 0;
        }
    }
}
