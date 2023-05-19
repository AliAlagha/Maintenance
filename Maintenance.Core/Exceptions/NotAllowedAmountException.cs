using Maintenance.Core.Constants;

namespace Maintenance.Core.Exceptions
{
    public class NotAllowedAmountException : BusinessException
    {
        public NotAllowedAmountException()
           : base(MessagesKeys.NotAllowedAmount)
        {
        }

    }
}
