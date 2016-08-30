using System;
using System.Threading;
using System.Threading.Tasks;

namespace Enterprise.Core.Linq
{
    internal sealed class SkipWhile<TSource> : AsyncEnumerableBase<TSource>
    {
        private readonly IAsyncEnumerable<TSource> source;

        private readonly Func<TSource, bool> predicate;

        private readonly Func<TSource, int, bool> predicateI;

        public SkipWhile(
            IAsyncEnumerable<TSource> source,
            Func<TSource, bool> predicate)
        {
            this.source = source;
            this.predicate = predicate;
        }

        public SkipWhile(
            IAsyncEnumerable<TSource> source,
            Func<TSource, int, bool> predicate)
        {
            this.source = source;
            this.predicateI = predicate;
        }

        public override AsyncIterator<TSource> Clone()
        {
            if (this.predicateI == null)
            {
                return new SkipWhile<TSource>(this.source, this.predicate);
            }

            return new SkipWhile<TSource>(this.source, this.predicateI);
        }

        protected override Task EnumerateAsync(
            IAsyncYield<TSource> yield,
            CancellationToken cancellationToken)
        {
            if (this.predicateI == null)
            {
                return this.SkipWhileImplAsync(yield, cancellationToken);
            }

            return this.SkipWhileImplIAsync(yield, cancellationToken);
        }

        private async Task SkipWhileImplAsync(
            IAsyncYield<TSource> yield,
            CancellationToken cancellationToken)
        {
            using (var iterator = this.source.GetAsyncEnumerator())
            {
                while (await iterator.MoveNextAsync(cancellationToken))
                {
                    var item = iterator.Current;
                    if (!this.predicate(item))
                    {
                        // Stop skipping now, and yield this item
                        await yield.ReturnAsync(item, cancellationToken);
                        break;
                    }
                }
                while (await iterator.MoveNextAsync(cancellationToken))
                {
                    await yield.ReturnAsync(iterator.Current, cancellationToken);
                }
            }
        }

        private async Task SkipWhileImplIAsync(
            IAsyncYield<TSource> yield,
            CancellationToken cancellationToken)
        {
            using (var iterator = this.source.GetAsyncEnumerator())
            {
                int index = 0;
                while (await iterator.MoveNextAsync(cancellationToken))
                {
                    var item = iterator.Current;
                    if (!this.predicateI(item, index))
                    {
                        // Stop skipping now, and yield this item
                        await yield.ReturnAsync(item, cancellationToken);
                        break;
                    }
                    index++;
                }
                while (await iterator.MoveNextAsync(cancellationToken))
                {
                    await yield.ReturnAsync(iterator.Current, cancellationToken);
                }
            }
        }
    }
}
