using System.Collections;
using System.Collections.Generic;
using Enterprise.Core.Linq;

namespace Enterprise.Tests.Linq.Helpers
{
    internal sealed class DummyAsyncEnumerable<T> : IAsyncEnumerable<T>
    {
        public IAsyncEnumerator<T> GetAsyncEnumerator()
        {
            return null;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return null;
        }

        IAsyncEnumerator IAsyncEnumerable.GetAsyncEnumerator()
        {
            return null;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return null;
        }
    }
}
