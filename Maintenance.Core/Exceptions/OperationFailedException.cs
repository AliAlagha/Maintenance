using Maintenance.Core.Constants;

namespace Maintenance.Core.Exceptions
{
    public class OperationFailedException : BusinessException
    {
        public OperationFailedException()
           : base(MessagesKeys.OperationFailed)
        {
        }

    }
}
