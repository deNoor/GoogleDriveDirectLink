using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace GoogleDriveDirectLink;

public class StaThreadProvider : IDisposable
{
    private readonly BlockingCollection<Action> _threadOperations;

    public StaThreadProvider()
    {
        _threadOperations = new();
        var staThread = new Thread(startParams =>
        {
            var threadOperations = (BlockingCollection<Action>) startParams!;
            foreach (var action in threadOperations.GetConsumingEnumerable())
            {
                action();
            }
        }) { IsBackground = true, };
        staThread.SetApartmentState(ApartmentState.STA);
        staThread.Start(_threadOperations);
    }

    public T Run<T>(Func<T> function)
    {
        var tcs = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
        _threadOperations.Add(() =>
        {
            try
            {
                tcs.SetResult(function());
            }
            catch (Exception e)
            {
                tcs.SetException(e);
            }
        });
        return tcs.Task.GetAwaiter().GetResult();
    }

    public void Run(Action action)
    {
        var tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        _threadOperations.Add(() =>
        {
            try
            {
                action();
                tcs.SetResult();
            }
            catch (Exception e)
            {
                tcs.SetException(e);
            }
        });
        tcs.Task.GetAwaiter().GetResult();
    }

    public void Dispose()
    {
        _threadOperations.CompleteAdding();
        _threadOperations.Dispose();
    }
}
