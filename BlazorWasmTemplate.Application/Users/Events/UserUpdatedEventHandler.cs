using BlazorWasmTemplate.Application.Users.Cache;
using BlazorWasmTemplate.Domain.Events;
using BlazorWasmTemplate.Domain.Users.Events;

namespace BlazorWasmTemplate.Application.Users.Events
{
    /// <inheritdoc/>
    public class UserUpdatedEventHandler : IEventHandler<UserUpdatedEvent>
    {
        /// <summary>
        /// ユーザーキャッシュ
        /// </summary>
        private readonly IUserCache _cache;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="cache"></param>
        public UserUpdatedEventHandler(IUserCache cache)
        {
            _cache = cache;
        }

        /// <inheritdoc/>
        public async Task Handle(UserUpdatedEvent @event)
        {
            // キャッシュ更新
            await _cache.RefreshAsync();

            // ログ出力
            Console.WriteLine($"[EVENT] User cache refreshed for UserId: {@event.UserId}");
        }
    }
}