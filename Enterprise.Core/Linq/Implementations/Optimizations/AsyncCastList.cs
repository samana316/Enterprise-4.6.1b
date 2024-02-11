using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Enterprise.Core.Linq
{
    internal sealed class AsyncCastList<T> : AsyncEnumerableBase<T>, IList<T>
    {
        private readonly IList source;

        public AsyncCastList(
            IList source)
        {
            this.source = source;
        }

        public T this[int index]
        {
            get
            {
                return (T)this.source[index];
            }
            set
            {
                this.source[index] = value;
            }
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
                return this.source.IsReadOnly;
            }
        }

        public void Add(
            T item)
        {
            this.source.Add(item);
        }

        public void Clear()
        {
            this.source.Clear();
        }

        public override AsyncIterator<T> Clone()
        {
            return new AsyncCastList<T>(this.source);
        }

        public bool Contains(
            T item)
        {
            return this.source.Contains(item);
        }

        public void CopyTo(
            T[] array, 
            int arrayIndex)
        {
            this.source.CopyTo(array, arrayIndex);
        }

        public int IndexOf(
            T item)
        {
            return this.source.IndexOf(item);
        }

        public void Insert(
            int index, 
            T item)
        {
            this.source.Insert(index, item);
        }

        public bool Remove(
            T item)
        {
            var casted = this.source as IList<T>;
            if (casted != null)
            {
                return casted.Remove(item);
            }

            var count = this.Count;
            this.source.Remove(item);

            return this.Count > count;
        }

        public void RemoveAt(
            int index)
        {
            this.source.RemoveAt(index);
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
