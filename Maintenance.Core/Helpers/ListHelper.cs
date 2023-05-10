using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Core.Helpers
{
    public class ListHelper<T>
    {
        public static bool ContainsAllItems(List<T> a, List<T> b)
        {
            return !b.Except(a).Any();
        }
    }
}
