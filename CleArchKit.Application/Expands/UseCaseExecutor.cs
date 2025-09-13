using Microsoft.Extensions.DependencyInjection;

namespace CleArchKit.Application.Expands
{
    public class UseCaseExecutor : IUseCaseExecutor
    {
        /// <summary>
        /// Service Provider
        /// </summary>
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="serviceProvider"></param>
        public UseCaseExecutor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <inheritdoc/>
        public async Task CreateScope<TUseCase>(Func<TUseCase, Task> useCaseFunc) where TUseCase : notnull
        {
            //ユースケースごとにDbContextのスコープを生成
            using (var scope = _serviceProvider.CreateScope())
            {
                var useCase = scope.ServiceProvider.GetRequiredService<TUseCase>();
                await useCaseFunc(useCase);
            }
        }
    }
}