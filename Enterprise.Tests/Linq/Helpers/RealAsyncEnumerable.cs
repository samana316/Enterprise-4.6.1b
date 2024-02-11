using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;
using Enterprise.Core.Linq;
using Enterprise.Tests.Linq.Helpers.Data;
using static Enterprise.Core.Linq.AsyncEnumerable;

namespace Enterprise.Tests.Linq.Helpers
{
    internal sealed class RealAsyncEnumerable<T> : IAsyncEnumerable<T>
    {
        private readonly IEnumerable<T> source;

        public RealAsyncEnumerable(
            IEnumerable<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            this.source = source;
        }

        public RealAsyncEnumerable(
            params T[] items)
            : this(items as IEnumerable<T>)
        {
        }

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
            var settings = ConfigurationManager.AppSettings["RealIOMode"];

            if (string.Equals("db", settings, StringComparison.OrdinalIgnoreCase))
            {
                return DbStreamIterator.Create(this.source).GetAsyncEnumerator();
            }

            return Create<T>(async (yield, cancellationToken) =>
            {
                Trace.WriteLine("GetAsyncEnumerator");
                cancellationToken.ThrowIfCancellationRequested();
                foreach (var item in this.source)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await Task.Delay(1, cancellationToken);
                    Trace.WriteLine(item, "MoveNextAsync");
                    await yield.ReturnAsync(item, cancellationToken);
                }
                Trace.WriteLine("Dispose");
            }).GetAsyncEnumerator();
        }
    }
}
