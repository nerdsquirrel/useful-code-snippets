## why is this needed?
Cancellation token is an important feature for controlling long running task. The default `CancellationTokenSource` has one limitation which is, for cancelling the previous long running task we use `cancellationTokenSource.Cancel()` method which might throw two runtime exception. One is null reference and another one is ObjectDisposedException. We can check null reference easily by `cancellationTokenSource?.Cancel()` but there is no way to understand is token source is already disposed or not. That's why this extension has been created which exposes `IsDisposed` property.


```csharp
using System;
using System.Threading;

namespace Infrastructure
{
    /// <summary>
    /// Extension of cancellation token source with dispose property. 
    /// </summary>
    class ApplicationCancellationTokenSource : CancellationTokenSource
    {
        public bool IsDisposed { get; private set; }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            IsDisposed = true;
        }

        public new void Dispose()
        {
            base.Dispose();
            IsDisposed = true;
        }

        public new void Cancel()
        {
            if (IsDisposed) return;
            base.Cancel();
            Dispose(true);
        }

        public new void CancelAfter(TimeSpan delay)
        {
            if (IsDisposed) return;
            base.CancelAfter(delay);
        }

        public new void CancelAfter(int millisecondsDelay)
        {
            if (IsDisposed) return;
            base.CancelAfter(millisecondsDelay);
        }
    }
}
