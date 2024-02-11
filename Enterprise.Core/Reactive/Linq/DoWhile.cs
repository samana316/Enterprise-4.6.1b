﻿using System;
using Enterprise.Core.Reactive.Linq.Implementations;
using Enterprise.Core.Utilities;

namespace Enterprise.Core.Reactive.Linq
{
    partial class AsyncObservable
    {
        public static IAsyncObservable<TSource> DoWhile<TSource>(
            this IAsyncObservable<TSource> source,
            Func<bool> condition)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(condition, nameof(condition));

            return new DoWhile<TSource>(source, condition);
        }
    }
}
