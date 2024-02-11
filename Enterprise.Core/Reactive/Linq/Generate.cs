using System;
using Enterprise.Core.Reactive.Linq.Implementations;
using Enterprise.Core.Utilities;

namespace Enterprise.Core.Reactive.Linq
{
    partial class AsyncObservable
    {
        public static IAsyncObservable<TResult> Generate<TState, TResult>(
            TState initialState, 
            Func<TState, bool> condition, 
            Func<TState, TState> iterate, 
            Func<TState, TResult> resultSelector)
        {
            Check.NotNull(condition, nameof(condition));
            Check.NotNull(iterate, nameof(iterate));
            Check.NotNull(resultSelector, nameof(resultSelector));

            return new Generate<TState, TResult>(
                initialState, condition, iterate, resultSelector);
        }
        
        public static IAsyncObservable<TResult> Generate<TState, TResult>(
            TState initialState, 
            Func<TState, bool> condition, 
            Func<TState, TState> iterate, 
            Func<TState, TResult> resultSelector, 
            Func<TState, DateTimeOffset> timeSelector)
        {
            Check.NotNull(condition, nameof(condition));
            Check.NotNull(iterate, nameof(iterate));
            Check.NotNull(resultSelector, nameof(resultSelector));
            Check.NotNull(timeSelector, nameof(timeSelector));

            return new Generate<TState, TResult>(
                initialState, condition, iterate, resultSelector, null, timeSelector);
        }
        
        public static IAsyncObservable<TResult> Generate<TState, TResult>(
            TState initialState, 
            Func<TState, bool> condition, 
            Func<TState, TState> iterate, 
            Func<TState, TResult> resultSelector, 
            Func<TState, TimeSpan> timeSelector)
        {
            Check.NotNull(condition, nameof(condition));
            Check.NotNull(iterate, nameof(iterate));
            Check.NotNull(resultSelector, nameof(resultSelector));
            Check.NotNull(timeSelector, nameof(timeSelector));

            return new Generate<TState, TResult>(
                initialState, condition, iterate, resultSelector, timeSelector);
        }
    }
}