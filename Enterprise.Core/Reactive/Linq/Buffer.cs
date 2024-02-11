using System;
using System.Collections.Generic;
using Enterprise.Core.Reactive.Linq.Implementations;
using Enterprise.Core.Utilities;

namespace Enterprise.Core.Reactive.Linq
{
    partial class AsyncObservable
    {
        public static IAsyncObservable<IList<TSource>> Buffer<TSource>(
            this IAsyncObservable<TSource> source, 
            TimeSpan timeSpan)
        {
            return source.Buffer(timeSpan, timeSpan);
        }
        
        public static IAsyncObservable<IList<TSource>> Buffer<TSource>(
            this IAsyncObservable<TSource> source, 
            int count)
        {
            return source.Buffer(count, count);
        }

        public static IAsyncObservable<IList<TSource>> Buffer<TSource>(
            this IAsyncObservable<TSource> source, 
            TimeSpan timeSpan, 
            int count)
        {
            Check.NotNull(source, nameof(source));
            Check.NotLessThanDefault(timeSpan, nameof(timeSpan));
            Check.NotLessThanOrEqualDefault(count, nameof(count));

            return new Buffer<TSource>(source, timeSpan, count);
        }

        public static IAsyncObservable<IList<TSource>> Buffer<TSource>(
            this IAsyncObservable<TSource> source, 
            TimeSpan timeSpan, 
            TimeSpan timeShift)
        {
            Check.NotNull(source, nameof(source));
            Check.NotLessThanDefault(timeSpan, nameof(timeSpan));
            Check.NotLessThanDefault(timeShift, nameof(timeShift));

            return new Buffer<TSource>(source, timeSpan, timeShift);
        }
        
        public static IAsyncObservable<IList<TSource>> Buffer<TSource>(
            this IAsyncObservable<TSource> source, 
            int count, 
            int skip)
        {
            Check.NotNull(source, nameof(source));
            Check.NotLessThanOrEqualDefault(count, nameof(count));
            Check.NotLessThanOrEqualDefault(skip, nameof(skip));

            return new Buffer<TSource>(source, count, skip);
        }
        
        public static IAsyncObservable<IList<TSource>> Buffer<TSource, TBufferClosing>(
            this IAsyncObservable<TSource> source, 
            Func<IAsyncObservable<TBufferClosing>> bufferClosingSelector)
        {
            Check.NotNull(source, nameof(source));

            throw new NotImplementedException();
        }

        public static IAsyncObservable<IList<TSource>> Buffer<TSource, TBufferBoundary>(
            this IAsyncObservable<TSource> source,
            IAsyncObservable<TBufferBoundary> bufferBoundaries)
        {
            Check.NotNull(source, nameof(source));

            throw new NotImplementedException();
        }

        public static IAsyncObservable<IList<TSource>> Buffer<TSource, TBufferOpening, TBufferClosing>(
            this IAsyncObservable<TSource> source,
            IAsyncObservable<TBufferOpening> bufferOpenings, 
            Func<TBufferOpening, IAsyncObservable<TBufferClosing>> bufferClosingSelector)
        {
            Check.NotNull(source, nameof(source));

            throw new NotImplementedException();
        }
    }
}
