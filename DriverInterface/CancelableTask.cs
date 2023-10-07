using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class CancelableTask<T>
{
    private CancellationTokenSource cancellationTokenSource;

    public CancelableTask(CancellationToken cancellationToken)
    {
        cancellationTokenSource = new CancellationTokenSource();
        cancellationToken.Register(() => cancellationTokenSource.Cancel());
    }

    public Task<T> CreateTask(Func<CancellationToken, T> taskFunction)
    {
        var taskCompletionSource = new TaskCompletionSource<T>();

        Task.Run(() =>
        {
            try
            {
                T result = taskFunction(cancellationTokenSource.Token);
                taskCompletionSource.TrySetResult(result);
            }
            catch (OperationCanceledException)
            {
                taskCompletionSource.TrySetCanceled();
            }
            catch (Exception ex)
            {
                taskCompletionSource.TrySetException(ex);
            }
        });

        return taskCompletionSource.Task;
    }

    public void Cancel()
    {
        cancellationTokenSource.Cancel();
    }
}

