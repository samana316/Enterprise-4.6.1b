using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Linq;
using static Enterprise.Core.Linq.AsyncEnumerable;

namespace Enterprise.Tests.Linq.Helpers
{
    internal sealed class ThrowAsyncEnumerable<T> : IAsyncEnumerable<T>
    {
        public IAsyncEnumerator<T> GetAsyncEnumerator()
        {
            return this.InternalGetAsyncEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.InternalGetAsyncEnumerator();
        }

        IAsyncEnumerator IAsyncEnumerable.GetAsyncEnumerator()
        {
            return this.InternalGetAsyncEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.InternalGetAsyncEnumerator();
        }

        private IAsyncEnumerator<T> InternalGetAsyncEnumerator()
        {
            return Create<T>(this.StateMachineIterator).GetAsyncEnumerator();
        }

        private Task StateMachineIterator(
            IAsyncYield<T> yield,
            CancellationToken cancellationToken)
        {
            //await Task.Yield();

            throw new NotImplementedException();
        }
    }
}
