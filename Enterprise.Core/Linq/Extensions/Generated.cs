﻿//using System;
//using System.Collections.Generic;
//using System.Diagnostics.CodeAnalysis;
//using System.Threading;
//using System.Threading.Tasks;
//using Enterprise.Core.Resources;
//using Enterprise.Core.Utilities;

//namespace Enterprise.Core.Linq
//{
//    partial class AsyncEnumerable
//    {


//        // <summary>
//        // Asynchronously creates a <see cref="List{T}" /> from the <see cref="IAsyncEnumerable" />.
//        // </summary>
//        // <typeparam name="T"> The type that the elements will be cast to. </typeparam>
//        // <returns>
//        // A <see cref="Task" /> containing a <see cref="List{T}" /> that contains elements from the input sequence.
//        // </returns>
//        public static Task<List<T>> ToListAsync<T>(this IAsyncEnumerable source)
//        {
//            Check.NotNull(source, "source");

//            return source.ToListAsync<T>(CancellationToken.None);
//        }

//        // <summary>
//        // Asynchronously creates a <see cref="List{T}" /> from the <see cref="IAsyncEnumerable" />.
//        // </summary>
//        // <typeparam name="T"> The type that the elements will be cast to. </typeparam>
//        // <param name="cancellationToken"> The token to monitor for cancellation requests. </param>
//        // <returns>
//        // A <see cref="Task" /> containing a <see cref="List{T}" /> that contains elements from the input sequence.
//        // </returns>
//        public static async Task<List<T>> ToListAsync<T>(this IAsyncEnumerable source, CancellationToken cancellationToken)
//        {
//            Check.NotNull(source, "source");

//            var list = new List<T>();
//            await source.ForEachAsync(e => list.Add((T)e), cancellationToken).WithCurrentCulture();
//            return list;
//        }

//        // <summary>
//        // Asynchronously creates a <see cref="List{T}" /> from the <see cref="IAsyncEnumerable{T}" />.
//        // </summary>
//        // <returns>
//        // A <see cref="Task" /> containing a <see cref="List{T}" /> that contains elements from the input sequence.
//        // </returns>
//        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
//        public static Task<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> source)
//        {
//            Check.NotNull(source, "source");

//            return source.ToListAsync(CancellationToken.None);
//        }

//        // <summary>
//        // Asynchronously creates a <see cref="List{T}" /> from the <see cref="IAsyncEnumerable{T}" />.
//        // </summary>
//        // <param name="cancellationToken"> The token to monitor for cancellation requests. </param>
//        // <returns>
//        // A <see cref="Task" /> containing a <see cref="List{T}" /> that contains elements from the input sequence.
//        // </returns>
//        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
//        public static Task<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> source, CancellationToken cancellationToken)
//        {
//            Check.NotNull(source, "source");

//            var tcs = new TaskCompletionSource<List<T>>();
//            var list = new List<T>();
//            source.ForEachAsync(list.Add, cancellationToken).ContinueWith(
//                t =>
//                {
//                    if (t.IsFaulted)
//                    {
//                        tcs.TrySetException(t.Exception.InnerExceptions);
//                    }
//                    else if (t.IsCanceled)
//                    {
//                        tcs.TrySetCanceled();
//                    }
//                    else
//                    {
//                        tcs.TrySetResult(list);
//                    }
//                }, TaskContinuationOptions.ExecuteSynchronously);

//            return tcs.Task;
//        }

//        // <summary>
//        // Asynchronously creates a T[] from an <see cref="IAsyncEnumerable{T}" /> by enumerating it asynchronously.
//        // </summary>
//        // <typeparam name="T">
//        // The type of the elements of <paramref name="source" /> .
//        // </typeparam>
//        // <returns>
//        // A <see cref="Task" /> containing a T[] that contains elements from the input sequence.
//        // </returns>
//        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
//        public static Task<T[]> ToArrayAsync<T>(this IAsyncEnumerable<T> source)
//        {
//            Check.NotNull(source, "source");

//            return source.ToArrayAsync(CancellationToken.None);
//        }

//        // <summary>
//        // Asynchronously creates a T[] from an <see cref="IAsyncEnumerable{T}" /> by enumerating it asynchronously.
//        // </summary>
//        // <typeparam name="T">
//        // The type of the elements of <paramref name="source" /> .
//        // </typeparam>
//        // <param name="cancellationToken"> The token to monitor for cancellation requests. </param>
//        // <returns>
//        // A <see cref="Task" /> containing a T[] that contains elements from the input sequence.
//        // </returns>
//        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
//        public static async Task<T[]> ToArrayAsync<T>(this IAsyncEnumerable<T> source, CancellationToken cancellationToken)
//        {
//            Check.NotNull(source, "source");

//            var list = await source.ToListAsync(cancellationToken).WithCurrentCulture();
//            return list.ToArray();
//        }

//        // <summary>
//        // Asynchronously creates a <see cref="Dictionary{TKey, TSource}" /> from an <see cref="IAsyncEnumerable{TSource}" />
//        // by enumerating it asynchronously according to a specified key selector function.
//        // </summary>
//        // <typeparam name="TSource">
//        // The type of the elements of <paramref name="source" /> .
//        // </typeparam>
//        // <typeparam name="TKey">
//        // The type of the key returned by <paramref name="keySelector" /> .
//        // </typeparam>
//        // <param name="keySelector"> A function to extract a key from each element. </param>
//        // <returns>
//        // A <see cref="Task" /> containing a <see cref="Dictionary{TKey, TSource}" /> that contains selected keys and values.
//        // </returns>
//        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
//        public static Task<Dictionary<TKey, TSource>> ToDictionaryAsync<TSource, TKey>(
//            this IAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector)
//        {
//            Check.NotNull(source, "source");
//            Check.NotNull(keySelector, "keySelector");

//            return ToDictionaryAsync(source, keySelector, IdentityFunction<TSource>.Instance, null, CancellationToken.None);
//        }

//        // <summary>
//        // Asynchronously creates a <see cref="Dictionary{TKey, TSource}" /> from an <see cref="IAsyncEnumerable{TSource}" />
//        // by enumerating it asynchronously according to a specified key selector function.
//        // </summary>
//        // <typeparam name="TSource">
//        // The type of the elements of <paramref name="source" /> .
//        // </typeparam>
//        // <typeparam name="TKey">
//        // The type of the key returned by <paramref name="keySelector" /> .
//        // </typeparam>
//        // <param name="keySelector"> A function to extract a key from each element. </param>
//        // <param name="cancellationToken"> The token to monitor for cancellation requests. </param>
//        // <returns>
//        // A <see cref="Task" /> containing a <see cref="Dictionary{TKey, TSource}" /> that contains selected keys and values.
//        // </returns>
//        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
//        public static Task<Dictionary<TKey, TSource>> ToDictionaryAsync<TSource, TKey>(
//            this IAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector, CancellationToken cancellationToken)
//        {
//            Check.NotNull(source, "source");
//            Check.NotNull(keySelector, "keySelector");

//            return ToDictionaryAsync(source, keySelector, IdentityFunction<TSource>.Instance, null, cancellationToken);
//        }

//        // <summary>
//        // Asynchronously creates a <see cref="Dictionary{TKey, TSource}" /> from an <see cref="IAsyncEnumerable{TSource}" />
//        // by enumerating it asynchronously according to a specified key selector function and a comparer.
//        // </summary>
//        // <typeparam name="TSource">
//        // The type of the elements of <paramref name="source" /> .
//        // </typeparam>
//        // <typeparam name="TKey">
//        // The type of the key returned by <paramref name="keySelector" /> .
//        // </typeparam>
//        // <param name="keySelector"> A function to extract a key from each element. </param>
//        // <param name="comparer">
//        // An <see cref="IEqualityComparer{TKey}" /> to compare keys.
//        // </param>
//        // <returns>
//        // A <see cref="Task" /> containing a <see cref="Dictionary{TKey, TSource}" /> that contains selected keys and values.
//        // </returns>
//        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
//        public static Task<Dictionary<TKey, TSource>> ToDictionaryAsync<TSource, TKey>(
//            this IAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
//        {
//            Check.NotNull(source, "source");
//            Check.NotNull(keySelector, "keySelector");

//            return ToDictionaryAsync(source, keySelector, IdentityFunction<TSource>.Instance, comparer, CancellationToken.None);
//        }

//        // <summary>
//        // Asynchronously creates a <see cref="Dictionary{TKey, TSource}" /> from an <see cref="IAsyncEnumerable{TSource}" />
//        // by enumerating it asynchronously according to a specified key selector function and a comparer.
//        // </summary>
//        // <typeparam name="TSource">
//        // The type of the elements of <paramref name="source" /> .
//        // </typeparam>
//        // <typeparam name="TKey">
//        // The type of the key returned by <paramref name="keySelector" /> .
//        // </typeparam>
//        // <param name="keySelector"> A function to extract a key from each element. </param>
//        // <param name="comparer">
//        // An <see cref="IEqualityComparer{TKey}" /> to compare keys.
//        // </param>
//        // <param name="cancellationToken"> The token to monitor for cancellation requests. </param>
//        // <returns>
//        // A <see cref="Task" /> containing a <see cref="Dictionary{TKey, TSource}" /> that contains selected keys and values.
//        // </returns>
//        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
//        public static Task<Dictionary<TKey, TSource>> ToDictionaryAsync<TSource, TKey>(
//            this IAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer,
//            CancellationToken cancellationToken)
//        {
//            Check.NotNull(source, "source");
//            Check.NotNull(keySelector, "keySelector");

//            return ToDictionaryAsync(source, keySelector, IdentityFunction<TSource>.Instance, comparer, cancellationToken);
//        }

//        // <summary>
//        // Asynchronously creates a <see cref="Dictionary{TKey, TElement}" /> from an <see cref="IAsyncEnumerable{TSource}" />
//        // by enumerating it asynchronously according to a specified key selector and an element selector function.
//        // </summary>
//        // <typeparam name="TSource">
//        // The type of the elements of <paramref name="source" /> .
//        // </typeparam>
//        // <typeparam name="TKey">
//        // The type of the key returned by <paramref name="keySelector" /> .
//        // </typeparam>
//        // <typeparam name="TElement">
//        // The type of the value returned by <paramref name="elementSelector" /> .
//        // </typeparam>
//        // <param name="keySelector"> A function to extract a key from each element. </param>
//        // <param name="elementSelector"> A transform function to produce a result element value from each element. </param>
//        // <returns>
//        // A <see cref="Task" /> containing a <see cref="Dictionary{TKey, TElement}" /> that contains values of type
//        // <typeparamref
//        //     name="TElement" />
//        // selected from the input sequence.
//        // </returns>
//        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
//        public static Task<Dictionary<TKey, TElement>> ToDictionaryAsync<TSource, TKey, TElement>(
//            this IAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
//        {
//            Check.NotNull(source, "source");
//            Check.NotNull(keySelector, "keySelector");
//            Check.NotNull(elementSelector, "elementSelector");

//            return ToDictionaryAsync(source, keySelector, elementSelector, null, CancellationToken.None);
//        }

//        // <summary>
//        // Asynchronously creates a <see cref="Dictionary{TKey, TElement}" /> from an <see cref="IAsyncEnumerable{TSource}" />
//        // by enumerating it asynchronously according to a specified key selector and an element selector function.
//        // </summary>
//        // <typeparam name="TSource">
//        // The type of the elements of <paramref name="source" /> .
//        // </typeparam>
//        // <typeparam name="TKey">
//        // The type of the key returned by <paramref name="keySelector" /> .
//        // </typeparam>
//        // <typeparam name="TElement">
//        // The type of the value returned by <paramref name="elementSelector" /> .
//        // </typeparam>
//        // <param name="keySelector"> A function to extract a key from each element. </param>
//        // <param name="elementSelector"> A transform function to produce a result element value from each element. </param>
//        // <param name="cancellationToken"> The token to monitor for cancellation requests. </param>
//        // <returns>
//        // A <see cref="Task" /> containing a <see cref="Dictionary{TKey, TElement}" /> that contains values of type
//        // <typeparamref
//        //     name="TElement" />
//        // selected from the input sequence.
//        // </returns>
//        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
//        public static Task<Dictionary<TKey, TElement>> ToDictionaryAsync<TSource, TKey, TElement>(
//            this IAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector,
//            CancellationToken cancellationToken)
//        {
//            Check.NotNull(source, "source");
//            Check.NotNull(keySelector, "keySelector");
//            Check.NotNull(elementSelector, "elementSelector");

//            return ToDictionaryAsync(source, keySelector, elementSelector, null, cancellationToken);
//        }

//        // <summary>
//        // Asynchronously creates a <see cref="Dictionary{TKey, TElement}" /> from an <see cref="IAsyncEnumerable{TSource}" />
//        // by enumerating it asynchronously according to a specified key selector function, a comparer, and an element selector function.
//        // </summary>
//        // <typeparam name="TSource">
//        // The type of the elements of <paramref name="source" /> .
//        // </typeparam>
//        // <typeparam name="TKey">
//        // The type of the key returned by <paramref name="keySelector" /> .
//        // </typeparam>
//        // <typeparam name="TElement">
//        // The type of the value returned by <paramref name="elementSelector" /> .
//        // </typeparam>
//        // <param name="keySelector"> A function to extract a key from each element. </param>
//        // <param name="elementSelector"> A transform function to produce a result element value from each element. </param>
//        // <param name="comparer">
//        // An <see cref="IEqualityComparer{TKey}" /> to compare keys.
//        // </param>
//        // <returns>
//        // A <see cref="Task" /> containing a <see cref="Dictionary{TKey, TElement}" /> that contains values of type
//        // <typeparamref
//        //     name="TElement" />
//        // selected from the input sequence.
//        // </returns>
//        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
//        public static Task<Dictionary<TKey, TElement>> ToDictionaryAsync<TSource, TKey, TElement>(
//            this IAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector,
//            IEqualityComparer<TKey> comparer)
//        {
//            Check.NotNull(source, "source");
//            Check.NotNull(keySelector, "keySelector");
//            Check.NotNull(elementSelector, "elementSelector");

//            return ToDictionaryAsync(source, keySelector, elementSelector, comparer, CancellationToken.None);
//        }

//        // <summary>
//        // Asynchronously creates a <see cref="Dictionary{TKey, TElement}" /> from an <see cref="IAsyncEnumerable{TSource}" />
//        // by enumerating it asynchronously according to a specified key selector function, a comparer, and an element selector function.
//        // </summary>
//        // <typeparam name="TSource">
//        // The type of the elements of <paramref name="source" /> .
//        // </typeparam>
//        // <typeparam name="TKey">
//        // The type of the key returned by <paramref name="keySelector" /> .
//        // </typeparam>
//        // <typeparam name="TElement">
//        // The type of the value returned by <paramref name="elementSelector" /> .
//        // </typeparam>
//        // <param name="keySelector"> A function to extract a key from each element. </param>
//        // <param name="elementSelector"> A transform function to produce a result element value from each element. </param>
//        // <param name="comparer">
//        // An <see cref="IEqualityComparer{TKey}" /> to compare keys.
//        // </param>
//        // <param name="cancellationToken"> The token to monitor for cancellation requests. </param>
//        // <returns>
//        // A <see cref="Task" /> containing a <see cref="Dictionary{TKey, TElement}" /> that contains values of type
//        // <typeparamref
//        //     name="TElement" />
//        // selected from the input sequence.
//        // </returns>
//        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
//        public static async Task<Dictionary<TKey, TElement>> ToDictionaryAsync<TSource, TKey, TElement>(
//            this IAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector,
//            IEqualityComparer<TKey> comparer, CancellationToken cancellationToken)
//        {
//            Check.NotNull(source, "source");
//            Check.NotNull(keySelector, "keySelector");
//            Check.NotNull(elementSelector, "elementSelector");

//            var d = new Dictionary<TKey, TElement>(comparer);
//            await
//                source.ForEachAsync(element => d.Add(keySelector(element), elementSelector(element)), cancellationToken)
//                    .WithCurrentCulture();
//            return d;
//        }

//        //public static IAsyncEnumerable<TResult> Cast<TResult>(this IAsyncEnumerable source)
//        //{
//        //    Check.NotNull(source, "source");

//        //    return new CastAsyncIterator<TResult>(source);
//        //}

//        public static Task<TSource> FirstAsync<TSource>(this IAsyncEnumerable<TSource> source)
//        {
//            Check.NotNull(source, "source");

//            return source.FirstAsync(CancellationToken.None);
//        }

//        public static Task<TSource> FirstAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, bool> predicate)
//        {
//            Check.NotNull(source, "source");
//            Check.NotNull(predicate, "predicate");

//            return source.FirstAsync(predicate, CancellationToken.None);
//        }

//        public static async Task<TSource> FirstAsync<TSource>(
//            this IAsyncEnumerable<TSource> source, CancellationToken cancellationToken)
//        {
//            Check.NotNull(source, "source");

//            cancellationToken.ThrowIfCancellationRequested();

//            using (var e = source.GetAsyncEnumerator())
//            {
//                if (await e.MoveNextAsync(cancellationToken).WithCurrentCulture())
//                {
//                    return e.Current;
//                }
//            }

//            throw Error.EmptySequence();
//        }

//        public static async Task<TSource> FirstAsync<TSource>(
//            this IAsyncEnumerable<TSource> source, Func<TSource, bool> predicate, CancellationToken cancellationToken)
//        {
//            Check.NotNull(source, "source");
//            Check.NotNull(predicate, "predicate");

//            cancellationToken.ThrowIfCancellationRequested();

//            using (var e = source.GetAsyncEnumerator())
//            {
//                if (await e.MoveNextAsync(cancellationToken).WithCurrentCulture())
//                {
//                    if (predicate(e.Current))
//                    {
//                        return e.Current;
//                    }
//                }
//            }

//            throw Error.NoMatch();
//        }

//        public static Task<TSource> FirstOrDefaultAsync<TSource>(this IAsyncEnumerable<TSource> source)
//        {
//            Check.NotNull(source, "source");

//            return source.FirstOrDefaultAsync(CancellationToken.None);
//        }

//        public static Task<TSource> FirstOrDefaultAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, bool> predicate)
//        {
//            Check.NotNull(source, "source");

//            return source.FirstOrDefaultAsync(predicate, CancellationToken.None);
//        }

//        public static async Task<TSource> FirstOrDefaultAsync<TSource>(
//            this IAsyncEnumerable<TSource> source, CancellationToken cancellationToken)
//        {
//            Check.NotNull(source, "source");

//            cancellationToken.ThrowIfCancellationRequested();

//            using (var e = source.GetAsyncEnumerator())
//            {
//                if (await e.MoveNextAsync(cancellationToken).WithCurrentCulture())
//                {
//                    return e.Current;
//                }
//            }

//            return default(TSource);
//        }

//        public static async Task<TSource> FirstOrDefaultAsync<TSource>(
//            this IAsyncEnumerable<TSource> source, Func<TSource, bool> predicate, CancellationToken cancellationToken)
//        {
//            Check.NotNull(source, "source");

//            cancellationToken.ThrowIfCancellationRequested();

//            using (var e = source.GetAsyncEnumerator())
//            {
//                if (await e.MoveNextAsync(cancellationToken).WithCurrentCulture())
//                {
//                    if (predicate(e.Current))
//                    {
//                        return e.Current;
//                    }
//                }
//            }

//            return default(TSource);
//        }

//        public static Task<TSource> SingleAsync<TSource>(this IAsyncEnumerable<TSource> source)
//        {
//            Check.NotNull(source, "source");

//            return source.SingleAsync(CancellationToken.None);
//        }

//        public static async Task<TSource> SingleAsync<TSource>(
//            this IAsyncEnumerable<TSource> source, CancellationToken cancellationToken)
//        {
//            Check.NotNull(source, "source");

//            cancellationToken.ThrowIfCancellationRequested();

//            using (var e = source.GetAsyncEnumerator())
//            {
//                if (!await e.MoveNextAsync(cancellationToken).WithCurrentCulture())
//                {
//                    throw Error.EmptySequence();
//                }

//                cancellationToken.ThrowIfCancellationRequested();

//                var result = e.Current;

//                if (!await e.MoveNextAsync(cancellationToken).WithCurrentCulture())
//                {
//                    return result;
//                }
//            }

//            throw Error.MoreThanOneElement();
//        }

//        public static Task<TSource> SingleAsync<TSource>(
//            this IAsyncEnumerable<TSource> source,
//            Func<TSource, bool> predicate)
//        {
//            Check.NotNull(source, "source");
//            Check.NotNull(predicate, "predicate");

//            return source.SingleAsync(predicate, CancellationToken.None);
//        }

//        public static async Task<TSource> SingleAsync<TSource>(
//            this IAsyncEnumerable<TSource> source, Func<TSource, bool> predicate, CancellationToken cancellationToken)
//        {
//            Check.NotNull(source, "source");
//            Check.NotNull(predicate, "predicate");

//            cancellationToken.ThrowIfCancellationRequested();

//            var result = default(TSource);
//            long count = 0;
//            using (var e = source.GetAsyncEnumerator())
//            {
//                while (await e.MoveNextAsync(cancellationToken).WithCurrentCulture())
//                {
//                    cancellationToken.ThrowIfCancellationRequested();

//                    if (predicate(e.Current))
//                    {
//                        result = e.Current;
//                        checked
//                        {
//                            count++;
//                        }
//                    }
//                }
//            }

//            switch (count)
//            {
//                case 0:
//                    throw Error.NoMatch();
//                case 1:
//                    return result;
//            }

//            throw Error.MoreThanOneMatch();
//        }

//        public static Task<TSource> SingleOrDefaultAsync<TSource>(this IAsyncEnumerable<TSource> source)
//        {
//            Check.NotNull(source, "source");

//            return source.SingleOrDefaultAsync(CancellationToken.None);
//        }

//        public static async Task<TSource> SingleOrDefaultAsync<TSource>(
//            this IAsyncEnumerable<TSource> source, CancellationToken cancellationToken)
//        {
//            Check.NotNull(source, "source");

//            cancellationToken.ThrowIfCancellationRequested();

//            using (var e = source.GetAsyncEnumerator())
//            {
//                if (!await e.MoveNextAsync(cancellationToken).WithCurrentCulture())
//                {
//                    return default(TSource);
//                }

//                cancellationToken.ThrowIfCancellationRequested();

//                var result = e.Current;

//                if (!await e.MoveNextAsync(cancellationToken).WithCurrentCulture())
//                {
//                    return result;
//                }
//            }

//            throw Error.MoreThanOneElement();
//        }

//        public static Task<TSource> SingleOrDefaultAsync<TSource>(
//            this IAsyncEnumerable<TSource> source,
//            Func<TSource, bool> predicate)
//        {
//            Check.NotNull(source, "source");
//            Check.NotNull(predicate, "predicate");

//            return source.SingleOrDefaultAsync(predicate, CancellationToken.None);
//        }

//        public static async Task<TSource> SingleOrDefaultAsync<TSource>(
//            this IAsyncEnumerable<TSource> source, Func<TSource, bool> predicate, CancellationToken cancellationToken)
//        {
//            Check.NotNull(source, "source");
//            Check.NotNull(predicate, "predicate");

//            cancellationToken.ThrowIfCancellationRequested();

//            var result = default(TSource);
//            long count = 0;
//            using (var e = source.GetAsyncEnumerator())
//            {
//                while (await e.MoveNextAsync(cancellationToken).WithCurrentCulture())
//                {
//                    cancellationToken.ThrowIfCancellationRequested();

//                    if (predicate(e.Current))
//                    {
//                        result = e.Current;
//                        checked
//                        {
//                            count++;
//                        }
//                    }
//                }
//            }

//            if (count < 2)
//            {
//                return result;
//            }

//            throw Error.MoreThanOneMatch();
//        }

//        public static Task<bool> ContainsAsync<TSource>(this IAsyncEnumerable<TSource> source, TSource value)
//        {
//            Check.NotNull(source, "source");

//            return source.ContainsAsync(value, CancellationToken.None);
//        }

//        public static async Task<bool> ContainsAsync<TSource>(
//            this IAsyncEnumerable<TSource> source, TSource value, CancellationToken cancellationToken)
//        {
//            Check.NotNull(source, "source");

//            cancellationToken.ThrowIfCancellationRequested();

//            using (var e = source.GetAsyncEnumerator())
//            {
//                while (await e.MoveNextAsync(cancellationToken).WithCurrentCulture())
//                {
//                    if (EqualityComparer<TSource>.Default.Equals(e.Current, value))
//                    {
//                        return true;
//                    }

//                    cancellationToken.ThrowIfCancellationRequested();
//                }
//            }

//            return false;
//        }

//        public static Task<bool> AnyAsync<TSource>(this IAsyncEnumerable<TSource> source)
//        {
//            Check.NotNull(source, "source");

//            return source.AnyAsync(CancellationToken.None);
//        }

//        public static async Task<bool> AnyAsync<TSource>(this IAsyncEnumerable<TSource> source, CancellationToken cancellationToken)
//        {
//            Check.NotNull(source, "source");

//            cancellationToken.ThrowIfCancellationRequested();

//            using (var e = source.GetAsyncEnumerator())
//            {
//                if (await e.MoveNextAsync(cancellationToken).WithCurrentCulture())
//                {
//                    return true;
//                }
//            }

//            return false;
//        }

//        public static Task<bool> AnyAsync<TSource>(
//            this IAsyncEnumerable<TSource> source, Func<TSource, bool> predicate)
//        {
//            Check.NotNull(source, "source");
//            Check.NotNull(predicate, "predicate");

//            return source.AnyAsync(predicate, CancellationToken.None);
//        }

//        public static async Task<bool> AnyAsync<TSource>(
//            this IAsyncEnumerable<TSource> source, Func<TSource, bool> predicate, CancellationToken cancellationToken)
//        {
//            Check.NotNull(source, "source");
//            Check.NotNull(predicate, "predicate");

//            cancellationToken.ThrowIfCancellationRequested();

//            using (var e = source.GetAsyncEnumerator())
//            {
//                while (await e.MoveNextAsync(cancellationToken).WithCurrentCulture())
//                {
//                    if (predicate(e.Current))
//                    {
//                        return true;
//                    }

//                    cancellationToken.ThrowIfCancellationRequested();
//                }
//            }

//            return false;
//        }

//        public static Task<bool> AllAsync<TSource>(
//            this IAsyncEnumerable<TSource> source, Func<TSource, bool> predicate)
//        {
//            Check.NotNull(source, "source");
//            Check.NotNull(predicate, "predicate");

//            return source.AllAsync(predicate, CancellationToken.None);
//        }

//        public static async Task<bool> AllAsync<TSource>(
//            this IAsyncEnumerable<TSource> source, Func<TSource, bool> predicate, CancellationToken cancellationToken)
//        {
//            Check.NotNull(source, "source");
//            Check.NotNull(predicate, "predicate");

//            cancellationToken.ThrowIfCancellationRequested();

//            using (var e = source.GetAsyncEnumerator())
//            {
//                while (await e.MoveNextAsync(cancellationToken).WithCurrentCulture())
//                {
//                    if (!predicate(e.Current))
//                    {
//                        return false;
//                    }

//                    cancellationToken.ThrowIfCancellationRequested();
//                }
//            }

//            return true;
//        }

//        public static Task<int> CountAsync<TSource>(this IAsyncEnumerable<TSource> source)
//        {
//            Check.NotNull(source, "source");

//            return source.CountAsync(CancellationToken.None);
//        }

//        public static async Task<int> CountAsync<TSource>(this IAsyncEnumerable<TSource> source, CancellationToken cancellationToken)
//        {
//            Check.NotNull(source, "source");

//            cancellationToken.ThrowIfCancellationRequested();

//            var count = 0;

//            using (var e = source.GetAsyncEnumerator())
//            {
//                checked
//                {
//                    while (await e.MoveNextAsync(cancellationToken).WithCurrentCulture())
//                    {
//                        cancellationToken.ThrowIfCancellationRequested();

//                        count++;
//                    }
//                }
//            }

//            return count;
//        }

//        public static Task<int> CountAsync<TSource>(
//            this IAsyncEnumerable<TSource> source, Func<TSource, bool> predicate)
//        {
//            Check.NotNull(source, "source");
//            Check.NotNull(predicate, "predicate");

//            return source.CountAsync(predicate, CancellationToken.None);
//        }

//        public static async Task<int> CountAsync<TSource>(
//            this IAsyncEnumerable<TSource> source, Func<TSource, bool> predicate, CancellationToken cancellationToken)
//        {
//            Check.NotNull(source, "source");
//            Check.NotNull(predicate, "predicate");

//            cancellationToken.ThrowIfCancellationRequested();

//            var count = 0;

//            using (var e = source.GetAsyncEnumerator())
//            {
//                checked
//                {
//                    while (await e.MoveNextAsync(cancellationToken).WithCurrentCulture())
//                    {
//                        cancellationToken.ThrowIfCancellationRequested();

//                        if (predicate(e.Current))
//                        {
//                            count++;
//                        }
//                    }
//                }
//            }

//            return count;
//        }

//        public static Task<long> LongCountAsync<TSource>(this IAsyncEnumerable<TSource> source)
//        {
//            Check.NotNull(source, "source");

//            return source.LongCountAsync(CancellationToken.None);
//        }

//        public static async Task<long> LongCountAsync<TSource>(
//            this IAsyncEnumerable<TSource> source, CancellationToken cancellationToken)
//        {
//            Check.NotNull(source, "source");

//            cancellationToken.ThrowIfCancellationRequested();

//            long count = 0;

//            using (var e = source.GetAsyncEnumerator())
//            {
//                checked
//                {
//                    while (await e.MoveNextAsync(cancellationToken).WithCurrentCulture())
//                    {
//                        cancellationToken.ThrowIfCancellationRequested();

//                        count++;
//                    }
//                }
//            }

//            return count;
//        }

//        public static Task<long> LongCountAsync<TSource>(
//            this IAsyncEnumerable<TSource> source, Func<TSource, bool> predicate)
//        {
//            Check.NotNull(source, "source");
//            Check.NotNull(predicate, "predicate");

//            return source.LongCountAsync(predicate, CancellationToken.None);
//        }

//        public static async Task<long> LongCountAsync<TSource>(
//            this IAsyncEnumerable<TSource> source, Func<TSource, bool> predicate, CancellationToken cancellationToken)
//        {
//            Check.NotNull(source, "source");
//            Check.NotNull(predicate, "predicate");

//            cancellationToken.ThrowIfCancellationRequested();

//            long count = 0;

//            using (var e = source.GetAsyncEnumerator())
//            {
//                checked
//                {
//                    while (await e.MoveNextAsync(cancellationToken).WithCurrentCulture())
//                    {
//                        cancellationToken.ThrowIfCancellationRequested();

//                        if (predicate(e.Current))
//                        {
//                            count++;
//                        }
//                    }
//                }
//            }

//            return count;
//        }

//        public static Task<TSource> MinAsync<TSource>(this IAsyncEnumerable<TSource> source)
//        {
//            Check.NotNull(source, "source");

//            return source.MinAsync(CancellationToken.None);
//        }

//        public static async Task<TSource> MinAsync<TSource>(this IAsyncEnumerable<TSource> source, CancellationToken cancellationToken)
//        {
//            Check.NotNull(source, "source");

//            cancellationToken.ThrowIfCancellationRequested();

//            var comparer = Comparer<TSource>.Default;
//            var value = default(TSource);
//            if (value == null)
//            {
//                using (var e = source.GetAsyncEnumerator())
//                {
//                    while (await e.MoveNextAsync(cancellationToken).WithCurrentCulture())
//                    {
//                        cancellationToken.ThrowIfCancellationRequested();

//                        if (e.Current != null
//                            && (value == null || comparer.Compare(e.Current, value) < 0))
//                        {
//                            value = e.Current;
//                        }
//                    }
//                }

//                return value;
//            }
//            else
//            {
//                var hasValue = false;

//                using (var e = source.GetAsyncEnumerator())
//                {
//                    while (await e.MoveNextAsync(cancellationToken).WithCurrentCulture())
//                    {
//                        cancellationToken.ThrowIfCancellationRequested();

//                        if (hasValue)
//                        {
//                            if (comparer.Compare(e.Current, value) < 0)
//                            {
//                                value = e.Current;
//                            }
//                        }
//                        else
//                        {
//                            value = e.Current;
//                            hasValue = true;
//                        }
//                    }
//                }

//                if (hasValue)
//                {
//                    return value;
//                }
//                throw Error.EmptySequence();
//            }
//        }

//        public static Task<TSource> MaxAsync<TSource>(this IAsyncEnumerable<TSource> source)
//        {
//            Check.NotNull(source, "source");

//            return source.MaxAsync(CancellationToken.None);
//        }

//        public static async Task<TSource> MaxAsync<TSource>(this IAsyncEnumerable<TSource> source, CancellationToken cancellationToken)
//        {
//            Check.NotNull(source, "source");

//            cancellationToken.ThrowIfCancellationRequested();

//            var comparer = Comparer<TSource>.Default;
//            var value = default(TSource);
//            if (value == null)
//            {
//                using (var e = source.GetAsyncEnumerator())
//                {
//                    while (await e.MoveNextAsync(cancellationToken).WithCurrentCulture())
//                    {
//                        cancellationToken.ThrowIfCancellationRequested();

//                        if (e.Current != null
//                            && (value == null || comparer.Compare(e.Current, value) > 0))
//                        {
//                            value = e.Current;
//                        }
//                    }
//                }

//                return value;
//            }
//            else
//            {
//                var hasValue = false;

//                using (var e = source.GetAsyncEnumerator())
//                {
//                    while (await e.MoveNextAsync(cancellationToken).WithCurrentCulture())
//                    {
//                        cancellationToken.ThrowIfCancellationRequested();

//                        if (hasValue)
//                        {
//                            if (comparer.Compare(e.Current, value) > 0)
//                            {
//                                value = e.Current;
//                            }
//                        }
//                        else
//                        {
//                            value = e.Current;
//                            hasValue = true;
//                        }
//                    }
//                }

//                if (hasValue)
//                {
//                    return value;
//                }
//                throw Error.EmptySequence();
//            }
//        }

//        public static Task<int> SumAsync(this IAsyncEnumerable<int> source)
//        {
//            Check.NotNull(source, "source");

//            return source.SumAsync(CancellationToken.None);
//        }

//        public static async Task<int> SumAsync(this IAsyncEnumerable<int> source, CancellationToken cancellationToken)
//        {
//            Check.NotNull(source, "source");

//            cancellationToken.ThrowIfCancellationRequested();

//            long sum = 0;
//            using (var e = source.GetAsyncEnumerator())
//            {
//                while (await e.MoveNextAsync(cancellationToken).WithCurrentCulture())
//                {
//                    cancellationToken.ThrowIfCancellationRequested();

//                    checked
//                    {
//                        sum += e.Current;
//                    }
//                }
//            }

//            return (int)sum;
//        }

//        public static Task<int?> SumAsync(this IAsyncEnumerable<int?> source)
//        {
//            Check.NotNull(source, "source");

//            return source.SumAsync(CancellationToken.None);
//        }

//        public static async Task<int?> SumAsync(this IAsyncEnumerable<int?> source, CancellationToken cancellationToken)
//        {
//            Check.NotNull(source, "source");

//            cancellationToken.ThrowIfCancellationRequested();

//            long sum = 0;
//            using (var e = source.GetAsyncEnumerator())
//            {
//                while (await e.MoveNextAsync(cancellationToken).WithCurrentCulture())
//                {
//                    cancellationToken.ThrowIfCancellationRequested();

//                    checked
//                    {
//                        if (e.Current.HasValue)
//                        {
//                            sum += e.Current.GetValueOrDefault();
//                        }
//                    }
//                }
//            }

//            return (int)sum;
//        }

//        public static Task<long> SumAsync(this IAsyncEnumerable<long> source)
//        {
//            Check.NotNull(source, "source");

//            return source.SumAsync(CancellationToken.None);
//        }

//        public static async Task<long> SumAsync(this IAsyncEnumerable<long> source, CancellationToken cancellationToken)
//        {
//            Check.NotNull(source, "source");

//            cancellationToken.ThrowIfCancellationRequested();

//            long sum = 0;
//            using (var e = source.GetAsyncEnumerator())
//            {
//                while (await e.MoveNextAsync(cancellationToken).WithCurrentCulture())
//                {
//                    cancellationToken.ThrowIfCancellationRequested();

//                    checked
//                    {
//                        sum += e.Current;
//                    }
//                }
//            }

//            return sum;
//        }

//        public static Task<long?> SumAsync(this IAsyncEnumerable<long?> source)
//        {
//            Check.NotNull(source, "source");

//            return source.SumAsync(CancellationToken.None);
//        }

//        public static async Task<long?> SumAsync(this IAsyncEnumerable<long?> source, CancellationToken cancellationToken)
//        {
//            Check.NotNull(source, "source");

//            cancellationToken.ThrowIfCancellationRequested();

//            long sum = 0;
//            using (var e = source.GetAsyncEnumerator())
//            {
//                while (await e.MoveNextAsync(cancellationToken).WithCurrentCulture())
//                {
//                    cancellationToken.ThrowIfCancellationRequested();

//                    checked
//                    {
//                        if (e.Current.HasValue)
//                        {
//                            sum += e.Current.GetValueOrDefault();
//                        }
//                    }
//                }
//            }

//            return sum;
//        }

//        public static Task<float> SumAsync(this IAsyncEnumerable<float> source)
//        {
//            Check.NotNull(source, "source");

//            return source.SumAsync(CancellationToken.None);
//        }

//        public static async Task<float> SumAsync(this IAsyncEnumerable<float> source, CancellationToken cancellationToken)
//        {
//            Check.NotNull(source, "source");

//            cancellationToken.ThrowIfCancellationRequested();

//            double sum = 0;
//            using (var e = source.GetAsyncEnumerator())
//            {
//                while (await e.MoveNextAsync(cancellationToken).WithCurrentCulture())
//                {
//                    cancellationToken.ThrowIfCancellationRequested();

//                    checked
//                    {
//                        sum += e.Current;
//                    }
//                }
//            }

//            return (float)sum;
//        }

//        public static Task<float?> SumAsync(this IAsyncEnumerable<float?> source)
//        {
//            Check.NotNull(source, "source");

//            return source.SumAsync(CancellationToken.None);
//        }

//        public static async Task<float?> SumAsync(this IAsyncEnumerable<float?> source, CancellationToken cancellationToken)
//        {
//            Check.NotNull(source, "source");

//            cancellationToken.ThrowIfCancellationRequested();

//            double sum = 0;
//            using (var e = source.GetAsyncEnumerator())
//            {
//                while (await e.MoveNextAsync(cancellationToken).WithCurrentCulture())
//                {
//                    cancellationToken.ThrowIfCancellationRequested();

//                    checked
//                    {
//                        if (e.Current.HasValue)
//                        {
//                            sum += e.Current.GetValueOrDefault();
//                        }
//                    }
//                }
//            }

//            return (float)sum;
//        }

//        public static Task<double> SumAsync(this IAsyncEnumerable<double> source)
//        {
//            Check.NotNull(source, "source");

//            return source.SumAsync(CancellationToken.None);
//        }

//        public static async Task<double> SumAsync(this IAsyncEnumerable<double> source, CancellationToken cancellationToken)
//        {
//            Check.NotNull(source, "source");

//            cancellationToken.ThrowIfCancellationRequested();

//            double sum = 0;
//            using (var e = source.GetAsyncEnumerator())
//            {
//                while (await e.MoveNextAsync(cancellationToken).WithCurrentCulture())
//                {
//                    cancellationToken.ThrowIfCancellationRequested();

//                    checked
//                    {
//                        sum += e.Current;
//                    }
//                }
//            }

//            return sum;
//        }

//        public static Task<double?> SumAsync(this IAsyncEnumerable<double?> source)
//        {
//            Check.NotNull(source, "source");

//            return source.SumAsync(CancellationToken.None);
//        }

//        public static async Task<double?> SumAsync(this IAsyncEnumerable<double?> source, CancellationToken cancellationToken)
//        {
//            Check.NotNull(source, "source");

//            cancellationToken.ThrowIfCancellationRequested();

//            double sum = 0;
//            using (var e = source.GetAsyncEnumerator())
//            {
//                while (await e.MoveNextAsync(cancellationToken).WithCurrentCulture())
//                {
//                    cancellationToken.ThrowIfCancellationRequested();

//                    checked
//                    {
//                        if (e.Current.HasValue)
//                        {
//                            sum += e.Current.GetValueOrDefault();
//                        }
//                    }
//                }
//            }

//            return sum;
//        }

//        public static Task<decimal> SumAsync(this IAsyncEnumerable<decimal> source)
//        {
//            Check.NotNull(source, "source");

//            return source.SumAsync(CancellationToken.None);
//        }

//        public static async Task<decimal> SumAsync(this IAsyncEnumerable<decimal> source, CancellationToken cancellationToken)
//        {
//            Check.NotNull(source, "source");

//            cancellationToken.ThrowIfCancellationRequested();

//            decimal sum = 0;
//            using (var e = source.GetAsyncEnumerator())
//            {
//                while (await e.MoveNextAsync(cancellationToken).WithCurrentCulture())
//                {
//                    cancellationToken.ThrowIfCancellationRequested();

//                    checked
//                    {
//                        sum += e.Current;
//                    }
//                }
//            }

//            return sum;
//        }

//        public static Task<decimal?> SumAsync(this IAsyncEnumerable<decimal?> source)
//        {
//            Check.NotNull(source, "source");

//            return source.SumAsync(CancellationToken.None);
//        }

//        public static async Task<decimal?> SumAsync(this IAsyncEnumerable<decimal?> source, CancellationToken cancellationToken)
//        {
//            Check.NotNull(source, "source");

//            cancellationToken.ThrowIfCancellationRequested();

//            decimal sum = 0;
//            using (var e = source.GetAsyncEnumerator())
//            {
//                while (await e.MoveNextAsync(cancellationToken).WithCurrentCulture())
//                {
//                    cancellationToken.ThrowIfCancellationRequested();

//                    checked
//                    {
//                        if (e.Current.HasValue)
//                        {
//                            sum += e.Current.GetValueOrDefault();
//                        }
//                    }
//                }
//            }

//            return sum;
//        }

//        public static Task<double> AverageAsync(this IAsyncEnumerable<int> source)
//        {
//            Check.NotNull(source, "source");

//            return source.AverageAsync(CancellationToken.None);
//        }

//        public static async Task<double> AverageAsync(this IAsyncEnumerable<int> source, CancellationToken cancellationToken)
//        {
//            Check.NotNull(source, "source");

//            cancellationToken.ThrowIfCancellationRequested();

//            long sum = 0;
//            long count = 0;
//            using (var e = source.GetAsyncEnumerator())
//            {
//                while (await e.MoveNextAsync(cancellationToken).WithCurrentCulture())
//                {
//                    cancellationToken.ThrowIfCancellationRequested();

//                    checked
//                    {
//                        sum += e.Current;
//                        count++;
//                    }
//                }
//            }

//            if (count > 0)
//            {
//                return (double)sum / count;
//            }
//            throw Error.EmptySequence();
//        }

//        public static Task<double?> AverageAsync(this IAsyncEnumerable<int?> source)
//        {
//            Check.NotNull(source, "source");

//            return source.AverageAsync(CancellationToken.None);
//        }

//        public static async Task<double?> AverageAsync(this IAsyncEnumerable<int?> source, CancellationToken cancellationToken)
//        {
//            Check.NotNull(source, "source");

//            cancellationToken.ThrowIfCancellationRequested();

//            long sum = 0;
//            long count = 0;
//            using (var e = source.GetAsyncEnumerator())
//            {
//                while (await e.MoveNextAsync(cancellationToken).WithCurrentCulture())
//                {
//                    cancellationToken.ThrowIfCancellationRequested();

//                    checked
//                    {
//                        if (e.Current.HasValue)
//                        {
//                            sum += e.Current.GetValueOrDefault();
//                            count++;
//                        }
//                    }
//                }
//            }

//            if (count > 0)
//            {
//                return (double)sum / count;
//            }
//            throw Error.EmptySequence();
//        }

//        public static Task<double> AverageAsync(this IAsyncEnumerable<long> source)
//        {
//            Check.NotNull(source, "source");

//            return source.AverageAsync(CancellationToken.None);
//        }

//        public static async Task<double> AverageAsync(this IAsyncEnumerable<long> source, CancellationToken cancellationToken)
//        {
//            Check.NotNull(source, "source");

//            cancellationToken.ThrowIfCancellationRequested();

//            long sum = 0;
//            long count = 0;
//            using (var e = source.GetAsyncEnumerator())
//            {
//                while (await e.MoveNextAsync(cancellationToken).WithCurrentCulture())
//                {
//                    cancellationToken.ThrowIfCancellationRequested();

//                    checked
//                    {
//                        sum += e.Current;
//                        count++;
//                    }
//                }
//            }

//            if (count > 0)
//            {
//                return (double)sum / count;
//            }
//            throw Error.EmptySequence();
//        }

//        public static Task<double?> AverageAsync(this IAsyncEnumerable<long?> source)
//        {
//            Check.NotNull(source, "source");

//            return source.AverageAsync(CancellationToken.None);
//        }

//        public static async Task<double?> AverageAsync(this IAsyncEnumerable<long?> source, CancellationToken cancellationToken)
//        {
//            Check.NotNull(source, "source");

//            cancellationToken.ThrowIfCancellationRequested();

//            long sum = 0;
//            long count = 0;
//            using (var e = source.GetAsyncEnumerator())
//            {
//                while (await e.MoveNextAsync(cancellationToken).WithCurrentCulture())
//                {
//                    cancellationToken.ThrowIfCancellationRequested();

//                    checked
//                    {
//                        if (e.Current.HasValue)
//                        {
//                            sum += e.Current.GetValueOrDefault();
//                            count++;
//                        }
//                    }
//                }
//            }

//            if (count > 0)
//            {
//                return (double)sum / count;
//            }
//            throw Error.EmptySequence();
//        }

//        public static Task<float> AverageAsync(this IAsyncEnumerable<float> source)
//        {
//            Check.NotNull(source, "source");

//            return source.AverageAsync(CancellationToken.None);
//        }

//        public static async Task<float> AverageAsync(this IAsyncEnumerable<float> source, CancellationToken cancellationToken)
//        {
//            Check.NotNull(source, "source");

//            cancellationToken.ThrowIfCancellationRequested();

//            double sum = 0;
//            long count = 0;
//            using (var e = source.GetAsyncEnumerator())
//            {
//                while (await e.MoveNextAsync(cancellationToken).WithCurrentCulture())
//                {
//                    cancellationToken.ThrowIfCancellationRequested();

//                    checked
//                    {
//                        sum += e.Current;
//                        count++;
//                    }
//                }
//            }

//            if (count > 0)
//            {
//                return (float)(sum / count);
//            }
//            throw Error.EmptySequence();
//        }

//        public static Task<float?> AverageAsync(this IAsyncEnumerable<float?> source)
//        {
//            Check.NotNull(source, "source");

//            return source.AverageAsync(CancellationToken.None);
//        }

//        public static async Task<float?> AverageAsync(this IAsyncEnumerable<float?> source, CancellationToken cancellationToken)
//        {
//            Check.NotNull(source, "source");

//            cancellationToken.ThrowIfCancellationRequested();

//            double sum = 0;
//            long count = 0;
//            using (var e = source.GetAsyncEnumerator())
//            {
//                while (await e.MoveNextAsync(cancellationToken).WithCurrentCulture())
//                {
//                    cancellationToken.ThrowIfCancellationRequested();

//                    checked
//                    {
//                        if (e.Current.HasValue)
//                        {
//                            sum += e.Current.GetValueOrDefault();
//                            count++;
//                        }
//                    }
//                }
//            }

//            if (count > 0)
//            {
//                return (float)(sum / count);
//            }
//            throw Error.EmptySequence();
//        }

//        public static Task<double> AverageAsync(this IAsyncEnumerable<double> source)
//        {
//            Check.NotNull(source, "source");

//            return source.AverageAsync(CancellationToken.None);
//        }

//        public static async Task<double> AverageAsync(this IAsyncEnumerable<double> source, CancellationToken cancellationToken)
//        {
//            Check.NotNull(source, "source");

//            cancellationToken.ThrowIfCancellationRequested();

//            double sum = 0;
//            long count = 0;
//            using (var e = source.GetAsyncEnumerator())
//            {
//                while (await e.MoveNextAsync(cancellationToken).WithCurrentCulture())
//                {
//                    cancellationToken.ThrowIfCancellationRequested();

//                    checked
//                    {
//                        sum += e.Current;
//                        count++;
//                    }
//                }
//            }

//            if (count > 0)
//            {
//                return (float)(sum / count);
//            }
//            throw Error.EmptySequence();
//        }

//        public static Task<double?> AverageAsync(this IAsyncEnumerable<double?> source)
//        {
//            Check.NotNull(source, "source");

//            return source.AverageAsync(CancellationToken.None);
//        }

//        public static async Task<double?> AverageAsync(this IAsyncEnumerable<double?> source, CancellationToken cancellationToken)
//        {
//            Check.NotNull(source, "source");

//            cancellationToken.ThrowIfCancellationRequested();

//            double sum = 0;
//            long count = 0;
//            using (var e = source.GetAsyncEnumerator())
//            {
//                while (await e.MoveNextAsync(cancellationToken).WithCurrentCulture())
//                {
//                    cancellationToken.ThrowIfCancellationRequested();

//                    checked
//                    {
//                        if (e.Current.HasValue)
//                        {
//                            sum += e.Current.GetValueOrDefault();
//                            count++;
//                        }
//                    }
//                }
//            }

//            if (count > 0)
//            {
//                return (float)(sum / count);
//            }
//            throw Error.EmptySequence();
//        }

//        public static Task<decimal> AverageAsync(this IAsyncEnumerable<decimal> source)
//        {
//            Check.NotNull(source, "source");

//            return source.AverageAsync(CancellationToken.None);
//        }

//        public static async Task<decimal> AverageAsync(this IAsyncEnumerable<decimal> source, CancellationToken cancellationToken)
//        {
//            Check.NotNull(source, "source");

//            cancellationToken.ThrowIfCancellationRequested();

//            decimal sum = 0;
//            long count = 0;
//            using (var e = source.GetAsyncEnumerator())
//            {
//                while (await e.MoveNextAsync(cancellationToken).WithCurrentCulture())
//                {
//                    cancellationToken.ThrowIfCancellationRequested();

//                    checked
//                    {
//                        sum += e.Current;
//                        count++;
//                    }
//                }
//            }

//            if (count > 0)
//            {
//                return sum / count;
//            }
//            throw Error.EmptySequence();
//        }

//        public static Task<decimal?> AverageAsync(this IAsyncEnumerable<decimal?> source)
//        {
//            Check.NotNull(source, "source");

//            return source.AverageAsync(CancellationToken.None);
//        }

//        public static async Task<decimal?> AverageAsync(this IAsyncEnumerable<decimal?> source, CancellationToken cancellationToken)
//        {
//            Check.NotNull(source, "source");

//            cancellationToken.ThrowIfCancellationRequested();

//            decimal sum = 0;
//            long count = 0;
//            using (var e = source.GetAsyncEnumerator())
//            {
//                while (await e.MoveNextAsync(cancellationToken).WithCurrentCulture())
//                {
//                    cancellationToken.ThrowIfCancellationRequested();

//                    checked
//                    {
//                        if (e.Current.HasValue)
//                        {
//                            sum += e.Current.GetValueOrDefault();
//                            count++;
//                        }
//                    }
//                }
//            }

//            if (count > 0)
//            {
//                return sum / count;
//            }
//            throw Error.EmptySequence();
//        }

//        #region Nested classes



//        #endregion
//    }
//}
