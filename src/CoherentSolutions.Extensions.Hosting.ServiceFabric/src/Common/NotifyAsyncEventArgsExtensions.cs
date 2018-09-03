using System;
using System.Threading;
using System.Threading.Tasks;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Common
{
    public static class NotifyAsyncEventArgsExtensions
    {
        public static Task NotifyAsync(
            this EventHandler<NotifyAsyncEventArgs> @this,
            object sender,
            CancellationToken cancellationToken)
        {
            if (@this == null)
            {
                return Task.CompletedTask;
            }

            var methods = @this.GetInvocationList();
            if (methods.Length == 0)
            {
                return Task.CompletedTask;
            }

            return InvokeAsync(
                args =>
                {
                    foreach (var method in methods)
                    {
                        method.DynamicInvoke(args.sender, new NotifyAsyncEventArgs(args.cancellationToken, args.completion, args.failure));
                    }
                },
                sender,
                cancellationToken);
        }

        public static Task NotifyAsync<TPayload>(
            this EventHandler<NotifyAsyncEventArgs<TPayload>> @this,
            object sender,
            TPayload payload,
            CancellationToken cancellationToken)
        {
            if (@this == null)
            {
                return Task.CompletedTask;
            }

            var methods = @this.GetInvocationList();
            if (methods.Length == 0)
            {
                return Task.CompletedTask;
            }

            return InvokeAsync(
                args =>
                {
                    foreach (var method in methods)
                    {
                        method.DynamicInvoke(args.sender, new NotifyAsyncEventArgs<TPayload>(payload, args.cancellationToken, args.completion, args.failure));
                    }
                },
                sender,
                cancellationToken);
        }

        private static async Task InvokeAsync(
            Action<(object sender, CancellationToken cancellationToken, Action completion, Action<Exception> failure)> action,
            object source,
            CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>();

            action(
                (
                    source,
                    cancellationToken,
                    () =>
                    {
                        tcs.SetResult(true);
                    },
                    exception =>
                    {
                        if (exception is OperationCanceledException)
                        {
                            tcs.SetCanceled();
                        }
                        else
                        {
                            tcs.SetException(exception);
                        }
                    }
                ));

            await tcs.Task;
        }
    }
}