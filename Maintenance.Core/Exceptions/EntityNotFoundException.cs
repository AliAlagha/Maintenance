using Maintenance.Core.Constants;

namespace Maintenance.Core.Exceptions
{
    public class EntityNotFoundException : BusinessException
    {
        public EntityNotFoundException()
           : base(MessagesKeys.EntityNotFound)
        {
        }

    }
}
