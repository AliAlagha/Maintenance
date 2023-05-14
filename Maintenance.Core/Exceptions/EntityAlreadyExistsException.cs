using Maintenance.Core.Constants;

namespace Maintenance.Core.Exceptions
{
    public class EntityAlreadyExistsException : BusinessException
    {
        public EntityAlreadyExistsException()
           : base(MessagesKeys.EntityAlreadyExists)
        {
        }

    }
}
