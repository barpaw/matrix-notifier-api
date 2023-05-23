using System.Collections.Concurrent;

namespace MatrixNotifierApi.Services;

public interface IConcurrentQueueService
{
    ConcurrentQueue<MatrixNotifier> InstanceMatrixNotifier();
}