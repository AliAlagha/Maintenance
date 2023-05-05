using Maintenance.Core.Constants;

namespace Maintenance.Core.Exceptions
{
    public class EmailAlreadyExistsException : BusinessException
    {
        public EmailAlreadyExistsException()
           : base(MessagesKeys.EmailAlreadyExists)
        {
        }

    }
}
