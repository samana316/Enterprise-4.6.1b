using System;
using Enterprise.Core.Reactive.Linq.Implementations;
using Enterprise.Core.Utilities;

namespace Enterprise.Core.Reactive.Linq
{
    partial class AsyncObservable
    {
        public static IAsyncObservable<long> Interval(
            TimeSpan period)
        {
            Check.NotLessThanDefault(period, nameof(period));

            return new Interval(period);
        }
    }
}