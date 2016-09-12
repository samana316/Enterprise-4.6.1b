using System;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Linq;

namespace Enterprise.Core.Reactive
{
    internal sealed class AnonymousAsyncSubject<TSource> : AsyncSubjectBase<TSource>
    {
        private readonly Func<IAsyncYield<TSource>, CancellationToken, Task> producer;

        public AnonymousAsyncSubject(
            Func<IAsyncYield<TSource>, CancellationToken, Task> producer)
        {
            this.producer = producer;
        }

        public override AsyncIterator<TSource> Clone()
        {
            return new Anonymous<TSource>(this.producer);
        }

        protected override Task ProduceAsync(
            IAsyncYield<TSource> yield,
            CancellationToken cancellationToken)
        {
            return this.producer(yield, cancellationToken);
        }
    }
}
