using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Enterprise.Core.Linq
{
    internal sealed class AsyncCastCollection<T> : AsyncEnumerableBase<T>, ICollection<T>
    {
        private readonly ICollection source;

        public AsyncCastCollection(
            ICollection source)
        {
            this.source = source;
        }

        public int Count
        {
            get
            {
                return this.source.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return this.Source.IsReadOnly;
            }
        }

        private ICollection<T> Source
        {
            get
            {
                var casted = this.source as ICollection<T>;

                if (casted == null)
                {
                    throw new NotSupportedException();
                }

                return casted;
            }
        }

        public void Add(
            T item)
        {
            this.Source.Add(item);
        }

        public void Clear()
        {
            this.Source.Clear();
        }

        public override AsyncIterator<T> Clone()
        {
            return new AsyncCastCollection<T>(this.source);
        }

        public bool Contains(
            T item)
        {
            return this.Source.Contains(item);
        }

        public void CopyTo(
            T[] array, 
            int arrayIndex)
        {
            this.source.CopyTo(array, arrayIndex);
        }

        public bool Remove(
            T item)
        {
            return this.Source.Remove(item);
        }

        protected override async Task EnumerateAsync(
            IAsyncYield<T> yield, 
            CancellationToken cancellationToken)
        {
            var asyncEnumerable = this.source as IAsyncEnumerable;

            if (asyncEnumerable != null)
            {
                await asyncEnumerable.ForEachAsync((item, cancellationToken2) =>
                {
                    return yield.ReturnAsync((T)item, cancellationToken2);
                }, cancellationToken);
            }
            else
            {
                var enumerator = this.source.GetEnumerator();
                try
                {
                    while (await enumerator.MoveNextAsync(cancellationToken))
                    {
                        await yield.ReturnAsync((T)enumerator.Current, cancellationToken);
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
    }
}
