using System.Collections.Generic;

namespace Enterprise.Core.Linq
{
    internal sealed class AsyncCollection<T> : 
        AsyncEnumerableAdapterBase<T>,
        ICollection<T>,
        IReadOnlyCollection<T>
    {
        private readonly ICollection<T> source;

        public AsyncCollection(
            ICollection<T> source)
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
                return this.source.IsReadOnly;
            }
        }

        protected override IEnumerable<T> Source
        {
            get
            {
                return this.source;
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
            return new AsyncCollection<T>(this.source);
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

        public bool Remove(
            T item)
        {
            return this.source.Remove(item);
        }
    }
}
