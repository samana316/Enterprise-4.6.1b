using System;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Linq;

namespace Enterprise.Core.Reactive.Linq.Implementations
{
    internal sealed class SkipWhile<TSource> : AsyncObservableBase<TSource>
    {
        private readonly IAsyncObservable<TSource> source;

        private readonly Func<TSource, bool> predicate;

        private readonly Func<TSource, int, bool> predicateI;

        public SkipWhile(
            IAsyncObservable<TSource> source,
            Func<TSource, bool> predicate)
        {
            this.source = source;
            this.predicate = predicate;
        }

        public SkipWhile(
            IAsyncObservable<TSource> source,
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

        protected override Task ProduceAsync(
            IAsyncYield<TSource> yield,
            CancellationToken cancellationToken)
        {
            if (this.predicateI == null)
            {
                return this.SkipWhileImplAsync(yield, cancellationToken);
            }

            return this.SkipWhileImplIAsync(yield, cancellationToken);
        }

        private Task SkipWhileImplAsync(
            IAsyncYield<TSource> yield,
            CancellationToken cancellationToken)
        {
            var flag = false;
            return this.source.ForEachAsync(async (current, cancellationToken2) =>
            {
                if (!flag && !predicate(current))
                {
                    flag = true;
                }

                if (flag)
                {
                    await yield.ReturnAsync(current, cancellationToken2);
                }
            }, cancellationToken);
        }

        private Task SkipWhileImplIAsync(
            IAsyncYield<TSource> yield,
            CancellationToken cancellationToken)
        {
            int num = -1;
            bool flag = false;
            return source.ForEachAsync(async (current, cancellationToken2) =>
            {
                int num2 = num;
                num = checked(num2 + 1);
                if (!flag && !this.predicateI(current, num))
                {
                    flag = true;
                }
                if (flag)
                {
                    await yield.ReturnAsync(current, cancellationToken2);
                }
            }, cancellationToken);
        }
    }
}