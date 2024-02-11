using System.Collections.Generic;

namespace Enterprise.Core.Linq
{
    internal sealed class AsyncReadOnlyList<T> :
        AsyncEnumerableAdapterBase<T>,
        IReadOnlyList<T>
    {
        private readonly IReadOnlyList<T> source;

        public AsyncReadOnlyList(
            IReadOnlyList<T> source)
        {
            this.source = source;
        }

        public T this[int index]
        {
            get
            {
                return this.source[index];
            }
        }

        public int Count
        {
            get
            {
                return this.source.Count;
            }
        }

        protected override IEnumerable<T> Source
        {
            get
            {
                return this.source;
            }
        }

        public override AsyncIterator<T> Clone()
        {
            return new AsyncReadOnlyList<T>(this.source);
        }
    }
}
