namespace BlazorWasmTemplate.Domain.Users.Events
{
    public class UserUpdatedEvent
    {
        public Guid UserId { get; }

        public UserUpdatedEvent(Guid userId)
        {
            UserId = userId;
        }
    }
}