using System;
using Enterprise.Core.Reactive.Linq.Implementations;
using Enterprise.Core.Utilities;

namespace Enterprise.Core.Reactive.Linq
{
    partial class AsyncObservable
    {
        public static IAsyncObservable<long> Timer(
            TimeSpan dueTime)
        {
            Check.NotLessThanDefault(dueTime, nameof(dueTime));

            return new Timer(dueTime);
        }

        public static IAsyncObservable<long> Timer(
            TimeSpan dueTime,
            TimeSpan period)
        {
            Check.NotLessThanDefault(dueTime, nameof(dueTime));
            Check.NotLessThanDefault(period, nameof(period));

            return new Timer(dueTime, period);
        }

        public static IAsyncObservable<long> Timer(
            DateTimeOffset dueTime)
        {
            return new Timer(dueTime);
        }

        public static IAsyncObservable<long> Timer(
            DateTimeOffset dueTime,
            TimeSpan period)
        {
            Check.NotLessThanDefault(period, nameof(period));

            return new Timer(dueTime, period);
        }
    }
}