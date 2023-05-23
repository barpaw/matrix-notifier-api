using MediatR;
using MatrixNotifierApi.Services;

namespace MatrixNotifierApi.Commands.Handlers;

public class WebhookHandler : IRequestHandler<WebhookCommand, string>
{
    private readonly IApiService _apiService;
    private readonly ILogger<WebhookHandler> _logger;

    public WebhookHandler(ILogger<WebhookHandler> logger, IApiService apiService)
    {
        _logger = logger;
        _apiService = apiService;
    }

    public async Task<string> Handle(WebhookCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _apiService.Notify(request.MatrixNotifier);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            _logger.LogError(e.StackTrace);
            return "Error";
        }

        return "Ok";
    }
}