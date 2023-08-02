using Microsoft.AspNetCore.Authorization;

namespace MMeetupAPI.Authorization
{
    public enum OperationType
    {
        Create, Read, Update, Delete
    }
    public class ResourceOperationRequirement : IAuthorizationRequirement
    {
        public ResourceOperationRequirement(OperationType operationType)
        {
            OperationType = operationType;
        }
        public OperationType OperationType { get; }
    }
}
