using CleArchKit.Application.Users.Cache;
using CleArchKit.Domain.Events;
using CleArchKit.Domain.Users.Events;

namespace CleArchKit.Application.Users.Events
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
        public async Task HandleAsync(UserUpdatedEvent @event, CancellationToken cancellationToken = default)
        {
            try
            {
                // キャッシュ更新
                await _cache.RefreshAsync();

                // ログ出力
                Console.WriteLine($"[EVENT] User cache refreshed for UserId: {@event.UserId}");
            }
            catch (Exception ex)
            {
                // ログ出力
                Console.WriteLine($"{ex},Failed to refresh user cache. UserId: {@event.UserId}");
            }
        }
    }
}