using System;
using System.Threading;
using System.Threading.Tasks;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Common;
using Moq;
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
            // Mock
            var mockActionOne = new Mock<Action>();
            mockActionOne
                .Setup(instance => instance()).Verifiable();

            var mockActionTwo = new Mock<Action>();
            mockActionTwo
                .Setup(instance => instance()).Verifiable();

            // Arrange
            var arrangeActionOne = mockActionOne.Object;
            var arrangeActionTwo = mockActionTwo.Object;
            var arrangeClass = new TestClass();
            arrangeClass.Event += async (
                sender,
                args) =>
            {
                try
                {
                    await Task.Delay(1);

                    arrangeActionOne();

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

                    arrangeActionTwo();

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
            mockActionOne.Verify();
            mockActionTwo.Verify();
        }

        [Fact]
        public async Task Should_await_all_event_handlers_When_event_handlers_are_mix_of_async_and_sync()
        {
            // Mock
            var mockActionOne = new Mock<Action>();
            mockActionOne
                .Setup(instance => instance()).Verifiable();

            var mockActionTwo = new Mock<Action>();
            mockActionTwo
                .Setup(instance => instance()).Verifiable();

            // Arrange
            var arrangeActionOne = mockActionOne.Object;
            var arrangeActionTwo = mockActionTwo.Object;
            var arrangeClass = new TestClass();
            arrangeClass.Event += async (
                sender,
                args) =>
            {
                try
                {
                    await Task.Delay(1);

                    arrangeActionOne();

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

                    arrangeActionTwo();

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
            mockActionOne.Verify();
            mockActionTwo.Verify();
        }

        [Fact]
        public async Task Should_await_all_event_handlers_When_event_handlers_are_sync()
        {
            // Mock
            var mockActionOne = new Mock<Action>();
            mockActionOne
                .Setup(instance => instance()).Verifiable();

            var mockActionTwo = new Mock<Action>();
            mockActionTwo
                .Setup(instance => instance()).Verifiable();

            // Arrange
            var arrangeActionOne = mockActionOne.Object;
            var arrangeActionTwo = mockActionTwo.Object;
            var arrangeClass = new TestClass();
            arrangeClass.Event += (
                sender,
                args) =>
            {
                try
                {
                    Thread.Sleep(1);

                    arrangeActionOne();

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

                    arrangeActionTwo();

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
            mockActionOne.Verify();
            mockActionTwo.Verify();
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
            // Mock
            var mockAction = new Mock<Action>();
            mockAction
                .Setup(instance => instance()).Verifiable();

            // Arrange
            var arrangeAction = mockAction.Object;
            var arrangeClass = new TestClass();
            arrangeClass.Event += async (
                sender,
                args) =>
            {
                try
                {
                    await Task.Delay(1);

                    arrangeAction();

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
            mockAction.Verify();
        }

        [Fact]
        public async Task Should_await_event_handler_When_event_handler_is_sync()
        {
            // Mock
            var mockAction = new Mock<Action>();
            mockAction
                .Setup(instance => instance()).Verifiable();

            // Arrange
            var arrangeAction = mockAction.Object;
            var arrangeClass = new TestClass();
            arrangeClass.Event += (
                sender,
                args) =>
            {
                try
                {
                    Thread.Sleep(1);

                    arrangeAction();

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
            mockAction.Verify();
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