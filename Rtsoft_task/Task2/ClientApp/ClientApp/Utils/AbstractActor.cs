using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace ClientApp.Utils
{
    public abstract class AbstractActor<T, Ret>
    {
        private readonly BufferBlock<T> mailbox_;

        public int QueueCount => mailbox_.Count;

        public abstract int ThreadsCount { get; }

        public AbstractActor()
        {
            mailbox_ = new BufferBlock<T>();
            DoWork();
        }

        private void DoWork()
        {
            var workers = new List<Task>();

            Task.Run(async () => {

                while (true)
                {
                    while (workers.Count < ThreadsCount)
                    {
                        workers.Add(Handle());
                    }

                    await Task.WhenAny(workers);
                    workers.RemoveAll(s => s.IsCompleted);
                }
            });
        }

        private async Task Handle()
        {
            var message = await mailbox_.ReceiveAsync();

            try
            {
                await HandleMessage(message);
            }
            catch (Exception ex)
            {
                _ = HandleError(message, ex);
            }
        }

        public abstract Task HandleMessage(T message, Action<Ret>? onComplete = null);

        public abstract Task HandleError(T message, Exception ex);

        public Task SendAsync(T message) => mailbox_.SendAsync(message);

        public void Stop() => mailbox_.TryReceiveAll(out var _);
    }
}
