using Matrix.Sdk;

namespace MatrixNotifierApi.Services;

public class ApiService : IApiService
{
    private readonly ILogger<ApiService> _logger;
    private readonly IConcurrentQueueService _concurrentQueueService;

    public ApiService(ILogger<ApiService> logger, IConcurrentQueueService concurrentQueueService)
    {
        _logger = logger;
        _concurrentQueueService = concurrentQueueService;
    }

    public async Task Notify(MatrixNotifier matrixNotifier)
    {
        try
        {
            _concurrentQueueService.InstanceMatrixNotifier().Enqueue(matrixNotifier);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
        }
    }
}