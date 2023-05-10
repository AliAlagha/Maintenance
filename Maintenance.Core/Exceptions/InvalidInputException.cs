using Maintenance.Core.Constants;

namespace Maintenance.Core.Exceptions
{
    public class InvalidInputException : BusinessException
    {
        public InvalidInputException()
           : base(MessagesKeys.InvalidInput)
        {
        }

    }
}
