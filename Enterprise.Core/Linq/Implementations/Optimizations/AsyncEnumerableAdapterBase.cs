using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Enterprise.Core.Linq
{
    internal abstract class AsyncEnumerableAdapterBase : AsyncEnumerableBase<object>
    {
        protected abstract IEnumerable Source { get; }

        protected override sealed async Task EnumerateAsync(
            IAsyncYield<object> yield,
            CancellationToken cancellationToken)
        {
            var enumerator = this.Source.GetEnumerator();
            try
            {
                while (await enumerator.MoveNextAsync(cancellationToken))
                {
                    await yield.ReturnAsync(enumerator.Current, cancellationToken);
                }
            }
            finally
            {
                var disposable = enumerator as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
        }
    }

    internal abstract class AsyncEnumerableAdapterBase<TSource> : AsyncEnumerableBase<TSource>
    {
        protected abstract IEnumerable<TSource> Source { get; }

        protected override sealed async Task EnumerateAsync(
            IAsyncYield<TSource> yield,
            CancellationToken cancellationToken)
        {
            using (var enumerator = this.Source.GetEnumerator())
            {
                while (await enumerator.MoveNextAsync(cancellationToken))
                {
                    await yield.ReturnAsync(enumerator.Current, cancellationToken);
                }
            }
        }
    }
}
