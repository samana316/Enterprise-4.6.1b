using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Linq;
using Enterprise.Core.Utilities;

namespace Enterprise.Core.Reactive.Linq
{
    partial class AsyncObservable
    {
        public static IAsyncObservable<TResult> SelectMany<TSource, TResult>(
            this IAsyncObservable<TSource> source, 
            Func<TSource, IObservable<TResult>> selector)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(selector, nameof(selector));
            
            return Create<TResult>(async (yield, cancellationToken) =>
            {
                var tasks = new List<Task>();

                await source.ForEachAsync(async (item, cancellationToken2) =>
                {
                    await Task.Yield();

                    tasks.Add(yield.ReturnAllAsync(selector(item), cancellationToken2));
                }, cancellationToken);

                await Task.WhenAll(tasks);
            });
        }

        public static IAsyncObservable<TResult> SelectMany<TSource, TResult>(
            this IAsyncObservable<TSource> source, 
            Func<TSource, Task<TResult>> selector)
        {
            throw new NotImplementedException();
        }

        public static IAsyncObservable<TResult> SelectMany<TSource, TResult>(
            this IAsyncObservable<TSource> source, 
            Func<TSource, int, Task<TResult>> selector)
        {
            throw new NotImplementedException();
        }

        public static IAsyncObservable<TResult> SelectMany<TSource, TResult>(
            this IAsyncObservable<TSource> source, 
            Func<TSource, CancellationToken, Task<TResult>> selector)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(selector, nameof(selector));

            return Create<TResult>(async (yield, cancellationToken) =>
            {
                var tasks = new List<Task>();

                await source.ForEachAsync(async (item, cancellationToken2) =>
                {
                    await Task.Yield();

                    Func<Task> function = async () => await yield.ReturnAsync(
                        await selector(item, cancellationToken2), cancellationToken2);

                    tasks.Add(function());

                }, cancellationToken);

                await Task.WhenAll(tasks);
            });
        }

        public static IAsyncObservable<TResult> SelectMany<TSource, TResult>(
            this IAsyncObservable<TSource> source, 
            Func<TSource, int, CancellationToken, Task<TResult>> selector)
        {
            throw new NotImplementedException();
        }

        public static IAsyncObservable<TResult> SelectMany<TSource, TResult>(
            this IAsyncObservable<TSource> source, 
            Func<TSource, int, IObservable<TResult>> selector)
        {
            throw new NotImplementedException();
        }

        public static IAsyncObservable<TOther> SelectMany<TSource, TOther>(
            this IAsyncObservable<TSource> source, 
            IObservable<TOther> other)
        {
            throw new NotImplementedException();
        }

        public static IAsyncObservable<TResult> SelectMany<TSource, TResult>(
            this IAsyncObservable<TSource> source, 
            Func<TSource, IEnumerable<TResult>> selector)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(selector, nameof(selector));

            return Create<TResult>((yield, cancellationToken) =>
            {
                return source.ForEachAsync((item, cancellationToken2) =>
                {
                    return yield.ReturnAllAsync(selector(item), cancellationToken2);
                }, cancellationToken);
            });
        }

        public static IAsyncObservable<TResult> SelectMany<TSource, TResult>(
            this IAsyncObservable<TSource> source, 
            Func<TSource, int, IEnumerable<TResult>> selector)
        {
            throw new NotImplementedException();
        }

        public static IAsyncObservable<TResult> SelectMany<TSource, TResult>(
            this IAsyncObservable<TSource> source, 
            Func<TSource, IObservable<TResult>> onNext, 
            Func<Exception, IObservable<TResult>> onError, 
            Func<IObservable<TResult>> onCompleted)
        {
            throw new NotImplementedException();
        }

        public static IAsyncObservable<TResult> SelectMany<TSource, TResult>(
            this IAsyncObservable<TSource> source, 
            Func<TSource, int, IObservable<TResult>> onNext, 
            Func<Exception, IObservable<TResult>> onError, 
            Func<IObservable<TResult>> onCompleted)
        {
            throw new NotImplementedException();
        }

        public static IAsyncObservable<TResult> SelectMany<TSource, TTaskResult, TResult>(
            this IAsyncObservable<TSource> source, 
            Func<TSource, CancellationToken, Task<TTaskResult>> taskSelector, 
            Func<TSource, TTaskResult, TResult> resultSelector)
        {
            throw new NotImplementedException();
        }

        public static IAsyncObservable<TResult> SelectMany<TSource, TTaskResult, TResult>(
            this IAsyncObservable<TSource> source, 
            Func<TSource, int, CancellationToken, Task<TTaskResult>> taskSelector, 
            Func<TSource, int, TTaskResult, TResult> resultSelector)
        {
            throw new NotImplementedException();
        }

        public static IAsyncObservable<TResult> SelectMany<TSource, TCollection, TResult>(
            this IAsyncObservable<TSource> source, 
            Func<TSource, int, IObservable<TCollection>> collectionSelector, 
            Func<TSource, int, TCollection, int, TResult> resultSelector)
        {
            throw new NotImplementedException();
        }

        public static IAsyncObservable<TResult> SelectMany<TSource, TCollection, TResult>(
            this IAsyncObservable<TSource> source, 
            Func<TSource, IObservable<TCollection>> collectionSelector, 
            Func<TSource, TCollection, TResult> resultSelector)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(collectionSelector, nameof(collectionSelector));
            Check.NotNull(resultSelector, nameof(resultSelector));

            return Create<TResult>(async (yield, cancellationToken) =>
            {
                var tasks = new List<Task>();

                await source.ForEachAsync(async (item, cancellationToken2) =>
                {
                    await Task.Yield();
                    
                    var results =
                        from collectionItem in collectionSelector(item).AsAsyncObservable()
                        select resultSelector(item, collectionItem);

                    tasks.Add(yield.ReturnAllAsync(results, cancellationToken2));
                }, cancellationToken);

                await Task.WhenAll(tasks);
            });
        }

        public static IAsyncObservable<TResult> SelectMany<TSource, TCollection, TResult>(
            this IAsyncObservable<TSource> source, 
            Func<TSource, IEnumerable<TCollection>> collectionSelector, 
            Func<TSource, TCollection, TResult> resultSelector)
        {
            throw new NotImplementedException();
        }

        public static IAsyncObservable<TResult> SelectMany<TSource, TTaskResult, TResult>(
            this IAsyncObservable<TSource> source, 
            Func<TSource, int, Task<TTaskResult>> taskSelector, 
            Func<TSource, int, TTaskResult, TResult> resultSelector)
        {
            throw new NotImplementedException();
        }

        public static IAsyncObservable<TResult> SelectMany<TSource, TTaskResult, TResult>(
            this IAsyncObservable<TSource> source, 
            Func<TSource, Task<TTaskResult>> taskSelector, 
            Func<TSource, TTaskResult, TResult> resultSelector)
        {
            throw new NotImplementedException();
        }

        public static IAsyncObservable<TResult> SelectMany<TSource, TCollection, TResult>(
            this IAsyncObservable<TSource> source, 
            Func<TSource, int, IEnumerable<TCollection>> collectionSelector, 
            Func<TSource, int, TCollection, int, TResult> resultSelector)
        {
            throw new NotImplementedException();
        }

    }
}
