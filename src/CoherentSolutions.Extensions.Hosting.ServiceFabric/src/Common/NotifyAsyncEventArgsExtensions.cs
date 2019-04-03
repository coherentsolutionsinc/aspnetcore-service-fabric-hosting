using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Common
{
    public static class NotifyAsyncExtensions
    {
        public static Task NotifyAsync(
            this EventHandler<NotifyAsyncEventArgs> @this,
            object sender,
            CancellationToken cancellationToken = default)
        {
            if (@this == null)
            {
                return Task.CompletedTask;
            }

            return InvokeAsync(
                @this,
                args =>
                {
                    args.method.DynamicInvoke(
                        args.sender,
                        new NotifyAsyncEventArgs(
                            args.cancellationToken,
                            args.completion,
                            args.failure));
                },
                sender,
                cancellationToken);
        }

        public static Task NotifyAsync<TPayload>(
            this EventHandler<NotifyAsyncEventArgs<TPayload>> @this,
            object sender,
            TPayload payload,
            CancellationToken cancellationToken = default)
        {
            if (@this == null)
            {
                return Task.CompletedTask;
            }

            return InvokeAsync(
                @this,
                args =>
                {
                    args.method.DynamicInvoke(
                        args.sender,
                        new NotifyAsyncEventArgs<TPayload>(
                            payload,
                            args.cancellationToken,
                            args.completion,
                            args.failure));
                },
                sender,
                cancellationToken);
        }

        private static Task InvokeAsync(
            Delegate @delegate,
            Action<(Delegate method, object sender, CancellationToken cancellationToken, Action completion, Action<Exception> failure)> invocation,
            object source,
            CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>();
            var methods = @delegate.GetInvocationList();

            var remaining = methods.Length;

            Exception invocationException = null;
            foreach (var method in methods)
            {
                var sync = new object();

                var invoked = false;

                var complete = new Action(
                    () =>
                    {
                        if (invoked)
                        {
                            return;
                        }

                        lock (sync)
                        {
                            if (invoked)
                            {
                                return;
                            }

                            invoked = true;
                        }

                        if (Interlocked.Decrement(ref remaining) == 0)
                        {
                            switch (invocationException)
                            {
                                case null:
                                    tcs.SetResult(true);
                                    break;
                                case OperationCanceledException _:
                                    tcs.SetCanceled();
                                    break;
                                default:
                                    tcs.SetException(invocationException);
                                    break;
                            }
                        }
                    });
                var fail = new Action<Exception>(
                    exception =>
                    {
                        if (invoked)
                        {
                            return;
                        }

                        lock (sync)
                        {
                            if (invoked)
                            {
                                return;
                            }

                            invocationException = exception;
                            invoked = true;
                        }

                        if (Interlocked.Decrement(ref remaining) != 0)
                        {
                            return;
                        }

                        if (exception is OperationCanceledException)
                        {
                            tcs.SetCanceled();
                        }
                        else
                        {
                            tcs.SetException(exception);
                        }
                    });

                try
                {
                    invocation((method, source, cancellationToken, complete, failure: fail));
                }
                catch (TargetInvocationException e) when (e.InnerException != null)
                {
                    fail(e.InnerException);
                }
                catch (Exception e)
                {
                    fail(e);
                }
            }

            return tcs.Task;
        }
    }
}