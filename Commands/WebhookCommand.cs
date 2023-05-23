using MediatR;

namespace MatrixNotifierApi.Commands;

public class WebhookCommand : IRequest<string>
{
    public WebhookCommand(MatrixNotifier matrixNotifier)
    {
        MatrixNotifier = matrixNotifier;
    }

    public MatrixNotifier MatrixNotifier { get; set; }
}