namespace MatrixNotifierApi.Services;

public interface IApiService
{
    Task Notify(MatrixNotifier matrixNotifier);
}