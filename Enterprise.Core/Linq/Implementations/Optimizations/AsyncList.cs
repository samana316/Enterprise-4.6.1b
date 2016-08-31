using System.Collections.Generic;

namespace Enterprise.Core.Linq
{
    internal sealed class AsyncList<T> : 
        AsyncEnumerableAdapterBase<T>,
        IList<T>,
        IReadOnlyList<T>
    {
        private readonly IList<T> source;

        public AsyncList(
            IList<T> source)
        {
            this.source = source;
        }

        public T this[int index]
        {
            get
            {
                return this.source[index];
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
            return new AsyncList<T>(this.source);
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
            return this.source.Remove(item);
        }

        public void RemoveAt(
            int index)
        {
            this.source.RemoveAt(index);
        }
    }
}
