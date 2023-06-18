using Maintenance.Core.Constants;

namespace Maintenance.Core.Exceptions
{
    public class CustomMessageException : BusinessException
    {
        public string Message { get; set; }

        public CustomMessageException(string message)
           : base(message)
        {
            Message = message;
        }

    }
}
