using System.Collections;
using System.Collections.Generic;

namespace Enterprise.Core.Linq
{
    internal sealed class AsAsyncEnumerable : AsyncEnumerableAdapterBase
    {
        private readonly IEnumerable source;

        public AsAsyncEnumerable(
            IEnumerable source)
        {
            this.source = source;
        }

        protected override IEnumerable Source
        {
            get
            {
                return this.source;
            }
        }

        public override AsyncIterator<object> Clone()
        {
            return new AsAsyncEnumerable(this.source);
        }
    }

    internal sealed class AsAsyncEnumerable<TSource> : AsyncEnumerableAdapterBase<TSource>
    {
        private readonly IEnumerable<TSource> source;

        public AsAsyncEnumerable(
            IEnumerable<TSource> source)
        {
            this.source = source;
        }

        protected override IEnumerable<TSource> Source
        {
            get
            {
                return this.source;
            }
        }

        public override AsyncIterator<TSource> Clone()
        {
            return new AsAsyncEnumerable<TSource>(this.source);
        }
    }
}
