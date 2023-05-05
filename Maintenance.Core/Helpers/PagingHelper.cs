using Maintenance.Core.Dtos;

namespace Maintenance.Core.Helpers
{
    public static class PagingHelper
    {
        public static int GetSkipValue(this Pagination pagination)
        {
            return (pagination.Page - 1) * pagination.PerPage;
        }
        public static int GetPages(this Pagination pagination, int dataCount)
        {
            return Convert.ToInt32(Math.Ceiling(dataCount / (float)pagination.PerPage));
        }
    }
}
