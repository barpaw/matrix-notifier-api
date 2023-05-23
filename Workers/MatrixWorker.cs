using Matrix.Sdk;
using MatrixNotifierApi.Services;
using Microsoft.Extensions.Caching.Memory;

namespace MatrixNotifierApi.Workers;

public class MatrixWorker : BackgroundService
{
    private readonly ILogger<MatrixWorker> _logger;
    private readonly IConcurrentQueueService _concurrentQueueService;
    private readonly IMemoryCache _inMemoryCache;

    public MatrixWorker(ILogger<MatrixWorker> logger, IConcurrentQueueService concurrentQueueService,
        IMemoryCache inMemoryCache)
    {
        _logger = logger;
        _concurrentQueueService = concurrentQueueService;
        _inMemoryCache = inMemoryCache;
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation($"[MatrixWorker] Started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (_concurrentQueueService.InstanceMatrixNotifier().Any())
                {
                    _logger.LogInformation(
                        $"[MatrixWorker] TryDequeue.[{_concurrentQueueService.InstanceMatrixNotifier().Count()}]");

                    _concurrentQueueService.InstanceMatrixNotifier().TryDequeue(out MatrixNotifier matrixNotifier);

                    _inMemoryCache.Set("key", matrixNotifier);

                    if (matrixNotifier is not null)
                    {
                        var factory = new MatrixClientFactory();
                        IMatrixClient client = factory.Create();

                        await client.LoginAsync(new Uri(matrixNotifier.MatrixHomeserverURL),
                            matrixNotifier.MatrixHomeserverUser,
                            matrixNotifier.MatrixHomeserverPasswd, matrixNotifier.DeviceId);

                        await client.SendMessageAsync(matrixNotifier.MatrixHomeserverRoom, matrixNotifier.Message);

                        _inMemoryCache.Remove("key");
                    }
                }
            }
            catch (Matrix.Sdk.ApiException ex)
            {
                var matrixNotifier = _inMemoryCache.Get<MatrixNotifier>("key");
                _concurrentQueueService.InstanceMatrixNotifier().Enqueue(matrixNotifier);
                _inMemoryCache.Remove("key");

                _logger.LogError(ex.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }


            await Task.Delay(TimeSpan.FromSeconds(10));
        }
    }
}