using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Linq;

namespace Enterprise.Core.Reactive.Linq.Implementations
{
    internal sealed class Buffer<TSource> : AsyncObservableBase<IList<TSource>>
    {
        private readonly IAsyncObservable<TSource> source;

        private readonly int count;

        private readonly int skip;

        private readonly TimeSpan timeSpan;

        private readonly TimeSpan timeShift;

        private readonly IAsyncYieldBuilder<IList<TSource>> producer;

        private Buffer(
            Buffer<TSource> parent)
        {
            this.source = parent.source;
            this.count = parent.count;
            this.skip = parent.skip;
            this.timeSpan = parent.timeSpan;
            this.timeShift = parent.timeShift;
            this.producer = parent.producer;
        }

        public Buffer(
            IAsyncObservable<TSource> source, 
            int count,
            int skip)
        {
            this.source = source;
            this.count = count;
            this.skip = skip;
            this.producer = new BufferByCount(this);
        }

        public Buffer(
            IAsyncObservable<TSource> source,
            TimeSpan timeSpan, 
            TimeSpan timeShift)
        {
            this.source = source;
            this.timeSpan = timeSpan;
            this.timeShift = timeShift;
            this.producer = new BufferByTime(this);
        }

        public Buffer(
            IAsyncObservable<TSource> source,
            TimeSpan timeSpan,
            int count)
        {
            this.source = source;
            this.timeSpan = timeSpan;
            this.count = count;
            throw new NotImplementedException();
        }

        public override AsyncIterator<IList<TSource>> Clone()
        {
            return new Buffer<TSource>(this);
        }

        protected override Task ProduceAsync(
            IAsyncYield<IList<TSource>> yield, 
            CancellationToken cancellationToken)
        {
            return this.producer.RunAsync(yield, cancellationToken);
        }

        private sealed class BufferByCount : IAsyncYieldBuilder<IList<TSource>>
        {
            private readonly Buffer<TSource> parent;

            public BufferByCount(
                Buffer<TSource> parent)
            {
                this.parent = parent;
            }

            public async Task RunAsync(
                IAsyncYield<IList<TSource>> yield, 
                CancellationToken cancellationToken)
            {
                var n = 0;
                var queue = new Queue<IList<TSource>>();
                queue.Enqueue(new List<TSource>(this.parent.count));

                await this.parent.source.ForEachAsync(async (value, cancellationToken2) =>
                {
                    foreach (var current in queue)
                    {
                        current.Add(value);
                    }

                    var num = n - this.parent.count + 1;
                    if (num >= 0 && num % this.parent.skip == 0)
                    {
                        var list = queue.Dequeue();
                        if (list.Count > 0)
                        {
                            await yield.ReturnAsync(list, cancellationToken);
                        }
                    }

                    n++;
                    if (n % this.parent.skip == 0)
                    {
                        queue.Enqueue(new List<TSource>(this.parent.count));
                    }

                }, cancellationToken);

                while (queue.Count > 0)
                {
                    var list = queue.Dequeue();
                    if (list.Count > 0)
                    {
                        await yield.ReturnAsync(list, cancellationToken);
                    }
                }
            }
        }

        private sealed class BufferByTime : IAsyncYieldBuilder<IList<TSource>>
        {
            private readonly Buffer<TSource> parent;

            public BufferByTime(
                Buffer<TSource> parent)
            {
                this.parent = parent;
            }

            public async Task RunAsync(
                IAsyncYield<IList<TSource>> yield, 
                CancellationToken cancellationToken)
            {
                if (this.parent.timeShift > this.parent.timeSpan)
                {
                    throw new NotImplementedException();
                }

                var queue = new Queue<IList<TSource>>();
                queue.Enqueue(new List<TSource>(this.parent.count));

                var timestamp = DateTimeOffset.Now;

                await this.parent.source.ForEachAsync(async (value, cancellationToken2) =>
                {
                    foreach (var current in queue)
                    {
                        current.Add(value);
                    }

                    var num = DateTimeOffset.Now - timestamp;
                    if (num >= this.parent.timeSpan)
                    {
                        var list = queue.Dequeue();
                        if (list.Count > 0)
                        {
                            await yield.ReturnAsync(list, cancellationToken);
                        }

                        if (this.parent.timeSpan == this.parent.timeShift)
                        {
                            queue.Enqueue(new List<TSource>(this.parent.count));
                        }

                        timestamp = DateTimeOffset.Now;
                    }

                    if (this.parent.timeSpan > this.parent.timeShift)
                    {
                        queue.Enqueue(new List<TSource>(this.parent.count));
                    }
                }, cancellationToken);

                while (queue.Count > 0)
                {
                    var list = queue.Dequeue();
                    if (list.Count > 0)
                    {
                        await yield.ReturnAsync(list, cancellationToken);
                    }
                }
            }
        }
    }
}
