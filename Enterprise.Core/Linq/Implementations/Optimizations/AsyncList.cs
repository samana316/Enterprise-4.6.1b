using System;
using System.Collections;
using System.Collections.Generic;

namespace Enterprise.Core.Linq
{
    internal sealed class AsyncList : AsyncEnumerableAdapterBase, IList
    {
        private readonly IList source;

        public AsyncList(
            IList source)
        {
            this.source = source;
        }

        public object this[int index]
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

        public bool IsFixedSize
        {
            get
            {
                return this.source.IsFixedSize;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return this.source.IsReadOnly;
            }
        }

        public bool IsSynchronized
        {
            get
            {
                return this.source.IsSynchronized;
            }
        }

        public object SyncRoot
        {
            get
            {
                return this.source.SyncRoot;
            }
        }

        protected override IEnumerable Source
        {
            get
            {
                return this.source;
            }
        }

        public int Add(
            object value)
        {
            return this.source.Add(value);
        }

        public void Clear()
        {
            this.source.Clear();
        }

        public override AsyncIterator<object> Clone()
        {
            return new AsyncList(this.source);
        }

        public bool Contains(
            object value)
        {
            return this.source.Contains(value);
        }

        public void CopyTo(
            Array array, 
            int index)
        {
            this.source.CopyTo(array, index);
        }

        public int IndexOf(
            object value)
        {
            return this.source.IndexOf(value);
        }

        public void Insert(
            int index, 
            object value)
        {
            this.source.Insert(index, value);
        }

        public void Remove(
            object value)
        {
            this.source.Remove(value);
        }

        public void RemoveAt(
            int index)
        {
            this.source.RemoveAt(index);
        }
    }

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
