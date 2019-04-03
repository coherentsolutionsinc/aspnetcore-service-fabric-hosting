using System;
using System.Threading;
using System.Threading.Tasks;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Common;

using Xunit;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Extensions
{
    public class NotifyAsyncExtensionsTests
    {
        private class TestClass
        {
            public EventHandler<NotifyAsyncEventArgs> Event;
        }

        [Fact]
        public async Task Should_await_all_event_handlers_When_event_handlers_are_async()
        {
            // Arrange
            var value = 0L;

            var arrangeClass = new TestClass();
            arrangeClass.Event += async (
                sender,
                args) =>
            {
                try
                {
                    await Task.Delay(1);

                    Interlocked.Increment(ref value);

                    args.Complete();
                }
                catch (Exception e)
                {
                    args.Fail(e);
                }
            };
            arrangeClass.Event += async (
                sender,
                args) =>
            {
                try
                {
                    await Task.Delay(1);

                    Interlocked.Increment(ref value);

                    args.Complete();
                }
                catch (Exception e)
                {
                    args.Fail(e);
                }
            };

            // Act
            await arrangeClass.Event.NotifyAsync(null);

            // Assert
            Assert.Equal(2, value);
        }

        [Fact]
        public async Task Should_await_all_event_handlers_When_event_handlers_are_mix_of_async_and_sync()
        {
            // Arrange
            var value = 0L;

            var arrangeClass = new TestClass();
            arrangeClass.Event += async (
                sender,
                args) =>
            {
                try
                {
                    await Task.Delay(1);

                    Interlocked.Increment(ref value);

                    args.Complete();
                }
                catch (Exception e)
                {
                    args.Fail(e);
                }
            };
            arrangeClass.Event += (
                sender,
                args) =>
            {
                try
                {
                    Thread.Sleep(1);

                    Interlocked.Increment(ref value);

                    args.Complete();
                }
                catch (Exception e)
                {
                    args.Fail(e);
                }
            };

            // Act
            await arrangeClass.Event.NotifyAsync(null);

            // Assert
            Assert.Equal(2, value);
        }

        [Fact]
        public async Task Should_await_all_event_handlers_When_event_handlers_are_sync()
        {
            // Arrange
            var value = 0L;

            var arrangeClass = new TestClass();
            arrangeClass.Event += (
                sender,
                args) =>
            {
                try
                {
                    Thread.Sleep(1);

                    Interlocked.Increment(ref value);

                    args.Complete();
                }
                catch (Exception e)
                {
                    args.Fail(e);
                }
            };
            arrangeClass.Event += (
                sender,
                args) =>
            {
                try
                {
                    Thread.Sleep(1);

                    Interlocked.Increment(ref value);

                    args.Complete();
                }
                catch (Exception e)
                {
                    args.Fail(e);
                }
            };

            // Act
            await arrangeClass.Event.NotifyAsync(null);

            // Assert
            Assert.Equal(2, value);
        }

        [Fact]
        public async Task Should_await_event_handler_When_async_event_handler_throws_exception()
        {
            // Arrange
            var arrangeClass = new TestClass();
            arrangeClass.Event += async (
                sender,
                args) =>
            {
                try
                {
                    await Task.Delay(1);

                    throw new InvalidOperationException();
                }
                catch (Exception e)
                {
                    args.Fail(e);
                }
            };

            // Act, Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                async () => await arrangeClass.Event.NotifyAsync(null));
        }

        [Fact]
        public async Task Should_await_event_handler_When_event_handler_is_async()
        {
            // Arrange
            var executed = false;

            var arrangeClass = new TestClass();
            arrangeClass.Event += async (
                sender,
                args) =>
            {
                try
                {
                    await Task.Delay(1);

                    executed = true;

                    args.Complete();
                }
                catch (Exception e)
                {
                    args.Fail(e);
                }
            };

            // Act
            await arrangeClass.Event.NotifyAsync(null);

            // Assert
            Assert.True(executed);
        }

        [Fact]
        public async Task Should_await_event_handler_When_event_handler_is_sync()
        {
            // Arrange
            var executed = false;

            var arrangeClass = new TestClass();
            arrangeClass.Event += (
                sender,
                args) =>
            {
                try
                {
                    Thread.Sleep(1);

                    executed = true;

                    args.Complete();
                }
                catch (Exception e)
                {
                    args.Fail(e);
                }
            };

            // Act
            await arrangeClass.Event.NotifyAsync(null);

            // Assert
            Assert.True(executed);
        }

        [Fact]
        public async Task Should_await_event_handler_When_sync_event_handler_throws_exception()
        {
            // Arrange
            var arrangeClass = new TestClass();
            arrangeClass.Event += (
                sender,
                args) =>
            {
                try
                {
                    Thread.Sleep(1);

                    throw new InvalidOperationException();
                }
                catch (Exception e)
                {
                    args.Fail(e);
                }
            };

            // Act, Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                async () => await arrangeClass.Event.NotifyAsync(null));
        }
    }
}