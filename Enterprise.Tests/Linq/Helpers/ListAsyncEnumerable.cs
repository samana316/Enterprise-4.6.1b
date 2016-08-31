using System;
using System.Collections.Generic;
using Enterprise.Core.Linq;

namespace Enterprise.Tests.Linq.Helpers
{
    internal sealed class ListAsyncEnumerable<T> : List<T>, IAsyncEnumerable<T>
    {
        public ListAsyncEnumerable() : base()
        {
        }

        public ListAsyncEnumerable(
            int capacity) : base(capacity)
        {
        }
        public ListAsyncEnumerable(
            IEnumerable<T> collection) : base(collection)
        {
        }

        public IAsyncEnumerator<T> GetAsyncEnumerator()
        {
            throw new InvalidOperationException("This is not optimized.");
        }

        IAsyncEnumerator IAsyncEnumerable.GetAsyncEnumerator()
        {
            return this.GetAsyncEnumerator();
        }
    }
}
