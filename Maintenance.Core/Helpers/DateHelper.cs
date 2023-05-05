using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Core.Helpers
{
    public class DateHelper
    {
        public static string GetDayName(DateTime date)
        {
            return date.ToString("dddd");
        }

        public static string GetMonthName(DateTime date)
        {
            return date.ToString("MMMM");
        }
    }
}
