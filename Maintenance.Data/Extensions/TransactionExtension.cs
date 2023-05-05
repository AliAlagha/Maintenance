using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Data.Extensions
{
    public static class TransactionExtension
    {
        public static async Task UseTransaction(this ApplicationDbContext context, Func<Task> func)
        {
            var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                await func();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
