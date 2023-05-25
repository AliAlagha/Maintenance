using Maintenance.Core.Constants;

namespace Maintenance.Core.Exceptions
{
    public class AlreadyExistsException : BusinessException
    {
        public AlreadyExistsException()
           : base(MessagesKeys.AlreadyExists)
        {
        }

    }
}
