using System;
using System.Collections;
using System.Collections.Generic;
using Enterprise.Core.Linq;

namespace Enterprise.Tests.Linq.Helpers
{
    internal sealed class SemiGenericAsyncCollection<T> : IAsyncEnumerable<T>, IList
    {
        private readonly List<T> source;

        public SemiGenericAsyncCollection()
        {
            this.source = new List<T>();
        }

        public SemiGenericAsyncCollection(
            IEnumerable<T> collection)
        {
            this.source = new List<T>(collection);
        }

        public SemiGenericAsyncCollection(
            params T[] items)
        {
            this.source = new List<T>(items);
        }

        public object this[int index]
        {
            get
            {
                return this.List[index];
            }

            set
            {
                this.List[index] = value;
            }
        }

        public int Count
        {
            get
            {
                return this.List.Count;
            }
        }

        public bool IsFixedSize
        {
            get
            {
                return this.List.IsFixedSize;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return this.List.IsReadOnly;
            }
        }

        public bool IsSynchronized
        {
            get
            {
                return this.List.IsSynchronized;
            }
        }

        public object SyncRoot
        {
            get
            {
                return this.List.SyncRoot;
            }
        }

        private IList List
        {
            get { return this.source; }
        }

        public void Add(
            T item)
        {
            this.source.Add(item);
        }

        int IList.Add(
            object value)
        {
            return this.List.Add(value);
        }

        public void Clear()
        {
            this.List.Clear();
        }

        public bool Contains(
            object value)
        {
            return this.List.Contains(value);
        }

        public void CopyTo(
            Array array, 
            int index)
        {
            this.List.CopyTo(array, index);
        }

        public IAsyncEnumerator<T> GetAsyncEnumerator()
        {
            throw new InvalidOperationException("This is not optimized.");
        }

        public IEnumerator GetEnumerator()
        {
            return this.List.GetEnumerator();
        }

        public int IndexOf(
            object value)
        {
            return this.List.IndexOf(value);
        }

        public void Insert(
            int index, 
            object value)
        {
            this.List.Insert(index, value);
        }

        public void Remove(
            object value)
        {
            this.List.Remove(value);
        }

        public void RemoveAt(
            int index)
        {
            this.List.RemoveAt(index);
        }

        IAsyncEnumerator IAsyncEnumerable.GetAsyncEnumerator()
        {
            return this.GetAsyncEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return this.GetAsyncEnumerator();
        }
    }
}
