using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace GoogleDriveDirectLink;

internal class ClipboardAccessor : IDisposable
{
    private readonly BlockingCollection<Action> _threadOperations;

    public ClipboardAccessor()
    {
        _threadOperations = new();
        var staThread = new Thread(
            startParams =>
            {
                var threadOperations = (BlockingCollection<Action>)startParams!;
                foreach (var action in threadOperations.GetConsumingEnumerable())
                {
                    action();
                }
            })
        { IsBackground = true, };
        staThread.SetApartmentState(ApartmentState.STA);
        staThread.Start(_threadOperations);
    }

    public string GetText()
    {
        var tcs = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
        _threadOperations.Add(
            () =>
            {
                try
                {
                    var input = Clipboard.GetText();
                    tcs.SetResult(input);
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                }
            });
        return tcs.Task.GetAwaiter().GetResult();
    }

    public void SetText(string output)
    {
        var tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        _threadOperations.Add(
            () =>
            {
                try
                {
                    Clipboard.SetText(output);
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
