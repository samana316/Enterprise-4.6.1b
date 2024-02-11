using System.Collections.Generic;

namespace Enterprise.Core.Linq
{
    internal sealed class AsyncReadOnlyCollection<T> :
        AsyncEnumerableAdapterBase<T>,
        IReadOnlyCollection<T>
    {
        private readonly IReadOnlyCollection<T> source;

        public AsyncReadOnlyCollection(
            IReadOnlyCollection<T> source)
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

        protected override IEnumerable<T> Source
        {
            get
            {
                return this.source;
            }
        }

        public override AsyncIterator<T> Clone()
        {
            return new AsyncReadOnlyCollection<T>(this.source);
        }
    }
}
