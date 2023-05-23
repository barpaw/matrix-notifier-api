using System.Collections.Concurrent;

namespace MatrixNotifierApi.Services;

public class ConcurrentQueueService : IConcurrentQueueService
{
    public readonly ConcurrentQueue<MatrixNotifier> ConcurrentQueueMatrixNotifier;


    public ConcurrentQueueService()
    {
        ConcurrentQueueMatrixNotifier = new ConcurrentQueue<MatrixNotifier>();
    }
    
    public ConcurrentQueue<MatrixNotifier> InstanceMatrixNotifier()
    {
        return ConcurrentQueueMatrixNotifier;
    }
}