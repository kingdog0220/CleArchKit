using BlazorWasmTemplate.Application.Users.Cache;
using BlazorWasmTemplate.Domain.Events;
using BlazorWasmTemplate.Domain.Users.Events;

namespace BlazorWasmTemplate.Application.Users.Events
{
    public class UserUpdatedEventHandler : IEventHandler<UserUpdatedEvent>
    {
        private readonly IUserCache _cache;

        public UserUpdatedEventHandler(IUserCache cache)
        {
            _cache = cache;
        }

        public async Task Handle(UserUpdatedEvent @event)
        {
            // キャッシュ更新
            await _cache.RefreshAsync();

            // ログ出力
            Console.WriteLine($"[EVENT] User cache refreshed for UserId: {@event.UserId}");
        }
    }
}