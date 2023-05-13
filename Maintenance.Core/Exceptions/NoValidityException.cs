using Maintenance.Core.Constants;

namespace Maintenance.Core.Exceptions
{
    public class NoValidityException : BusinessException
    {
        public NoValidityException()
           : base(MessagesKeys.NoValidity)
        {
        }

    }
}
