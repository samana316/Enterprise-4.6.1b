﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Reactive.Linq.Implementations;
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

            return new SelectMany<TSource, TResult>(source, selector);
        }

        public static IAsyncObservable<TResult> SelectMany<TSource, TResult>(
            this IAsyncObservable<TSource> source, 
            Func<TSource, Task<TResult>> selector)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(selector, nameof(selector));

            Func<TSource, CancellationToken, Task<TResult>> overload = (value, cancellationToken) =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                return selector(value);
            };

            return new SelectMany<TSource, TResult>(source, overload);
        }

        public static IAsyncObservable<TResult> SelectMany<TSource, TResult>(
            this IAsyncObservable<TSource> source, 
            Func<TSource, int, Task<TResult>> selector)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(selector, nameof(selector));

            Func<TSource, int, CancellationToken, Task<TResult>> overload = (value, index, cancellationToken) =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                return selector(value, index);
            };

            return new SelectMany<TSource, TResult>(source, overload);
        }

        public static IAsyncObservable<TResult> SelectMany<TSource, TResult>(
            this IAsyncObservable<TSource> source, 
            Func<TSource, CancellationToken, Task<TResult>> selector)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(selector, nameof(selector));

            return new SelectMany<TSource, TResult>(source, selector);
        }

        public static IAsyncObservable<TResult> SelectMany<TSource, TResult>(
            this IAsyncObservable<TSource> source, 
            Func<TSource, int, CancellationToken, Task<TResult>> selector)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(selector, nameof(selector));

            return new SelectMany<TSource, TResult>(source, selector);
        }

        public static IAsyncObservable<TResult> SelectMany<TSource, TResult>(
            this IAsyncObservable<TSource> source, 
            Func<TSource, int, IObservable<TResult>> selector)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(selector, nameof(selector));

            return new SelectMany<TSource, TResult>(source, selector);
        }

        public static IAsyncObservable<TOther> SelectMany<TSource, TOther>(
            this IAsyncObservable<TSource> source, 
            IObservable<TOther> other)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(other, nameof(other));

            return new SelectMany<TSource, TOther>(source, value => other);
        }

        public static IAsyncObservable<TResult> SelectMany<TSource, TResult>(
            this IAsyncObservable<TSource> source, 
            Func<TSource, IEnumerable<TResult>> selector)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(selector, nameof(selector));

            return new SelectMany<TSource, TResult>(source, selector);
        }

        public static IAsyncObservable<TResult> SelectMany<TSource, TResult>(
            this IAsyncObservable<TSource> source, 
            Func<TSource, int, IEnumerable<TResult>> selector)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(selector, nameof(selector));

            return new SelectMany<TSource, TResult>(source, selector);
        }

        public static IAsyncObservable<TResult> SelectMany<TSource, TResult>(
            this IAsyncObservable<TSource> source, 
            Func<TSource, IObservable<TResult>> onNext, 
            Func<Exception, IObservable<TResult>> onError, 
            Func<IObservable<TResult>> onCompleted)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(onNext, nameof(onNext));
            Check.NotNull(onError, nameof(onError));
            Check.NotNull(onCompleted, nameof(onCompleted));

            return new SelectMany<TSource, TResult>(source, onNext, onError, onCompleted);
        }

        public static IAsyncObservable<TResult> SelectMany<TSource, TResult>(
            this IAsyncObservable<TSource> source, 
            Func<TSource, int, IObservable<TResult>> onNext, 
            Func<Exception, IObservable<TResult>> onError, 
            Func<IObservable<TResult>> onCompleted)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(onNext, nameof(onNext));
            Check.NotNull(onError, nameof(onError));
            Check.NotNull(onCompleted, nameof(onCompleted));

            return new SelectMany<TSource, TResult>(source, onNext, onError, onCompleted);
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
