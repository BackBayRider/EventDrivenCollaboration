using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Paramore.Brighter;

namespace FreightCaptainTests.Test_Doubles
{
    public enum Action
    {
        Send,
        Publish,
        Post
    }

    public class TrackedMessage<T> where T : IRequest
    {
        public Action Action { get; }
        public T Message { get; }

        public TrackedMessage(Action action, T message)
        {
            Action = action;
            Message = message;
        }
    }
    
    
    public class FakeCommandProcessor : IAmACommandProcessor
    {
        public Queue<TrackedMessage<IRequest>> Commands { get; } = new Queue<TrackedMessage<IRequest>>();
        public Queue<TrackedMessage<IRequest>> Events { get; } = new Queue<TrackedMessage<IRequest>>();
        public Queue<TrackedMessage<IRequest>> Messages { get; } = new Queue<TrackedMessage<IRequest>>();


        public void Send<T>(T command) where T : class, IRequest
        {
            Commands.Enqueue(new TrackedMessage<IRequest>(Action.Send, command));
        }

        public Task SendAsync<T>(T command, bool continueOnCapturedContext = false,
            CancellationToken cancellationToken = new CancellationToken()) where T : class, IRequest
        {
            var taskCompletion = new TaskCompletionSource<T>();
            Commands.Enqueue(new TrackedMessage<IRequest>(Action.Send, command));
            taskCompletion.SetResult(command);
            return taskCompletion.Task;
        }

        public void Publish<T>(T @event) where T : class, IRequest
        {
            Events.Enqueue(new TrackedMessage<IRequest>(Action.Publish, @event));
        }

        public Task PublishAsync<T>(T @event, bool continueOnCapturedContext = false,
            CancellationToken cancellationToken = new CancellationToken()) where T : class, IRequest
        {
            var taskCompletion = new TaskCompletionSource<T>();
            Events.Enqueue(new TrackedMessage<IRequest>(Action.Publish, @event));
            taskCompletion.SetResult(@event);
            return taskCompletion.Task;
        }

        public void Post<T>(T request) where T : class, IRequest
        {
            Messages.Enqueue(new TrackedMessage<IRequest>(Action.Post, request));
        }

        public Task PostAsync<T>(T request, bool continueOnCapturedContext = false,
            CancellationToken cancellationToken = new CancellationToken()) where T : class, IRequest
        {
            var taskCompletion = new TaskCompletionSource<T>();
            Messages.Enqueue(new TrackedMessage<IRequest>(Action.Post, request));
            taskCompletion.SetResult(request);
            return taskCompletion.Task;
        }
    }
}