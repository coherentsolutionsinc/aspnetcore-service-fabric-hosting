using System;
using System.Threading;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Common
{
    public class NotifyAsyncEventArgs : EventArgs
    {
        private readonly Action completion;

        private readonly Action<Exception> failure;

        public CancellationToken CancellationToken { get; }

        public NotifyAsyncEventArgs(
            CancellationToken cancellationToken,
            Action completion,
            Action<Exception> failure)
        {
            this.completion = completion;
            this.failure = failure;
            this.CancellationToken = cancellationToken;
        }

        public void Completed()
        {
            this.completion();
        }

        public void Failed(
            Exception exception)
        {
            this.failure(exception);
        }
    }

    public class NotifyAsyncEventArgs<TPayload> : NotifyAsyncEventArgs
    {
        public TPayload Payload { get; }

        public NotifyAsyncEventArgs(
            TPayload payload,
            CancellationToken cancellationToken,
            Action completion,
            Action<Exception> failure)
            : base(cancellationToken, completion, failure)
        {
            this.Payload = payload;
        }
    }
}