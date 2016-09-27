using System;
using Enterprise.Core.Utilities;

namespace Enterprise.Core.Reactive.Linq
{
    partial class AsyncObservable
    {
        public static IAsyncObservable<TResult> Zip<TSource1, TSource2, TSource3, TSource4, TSource5, TResult>(
            this IAsyncObservable<TSource1> source1,
            IObservable<TSource2> source2,
            IObservable<TSource3> source3,
            IObservable<TSource4> source4,
            IObservable<TSource5> source5,
            Func<TSource1, TSource2, TSource3, TSource4, TSource5, TResult> resultSelector)
        {
            Check.NotNull(source1, nameof(source1));
            Check.NotNull(source2, nameof(source2));
            Check.NotNull(source3, nameof(source3));
            Check.NotNull(source4, nameof(source4));
            Check.NotNull(source5, nameof(source5));
            Check.NotNull(resultSelector, nameof(resultSelector));

            return source1
                .Zip(source2, source3, source4, (x1, x2, x3, x4) => new { x1, x2, x3, x4 })
                .Zip(source5, (a, x5) => resultSelector(a.x1, a.x2, a.x3, a.x4, x5));
        }

        public static IAsyncObservable<TResult> Zip<TSource1, TSource2, TSource3, TSource4, TSource5, TSource6, TResult>(
            this IAsyncObservable<TSource1> source1,
            IObservable<TSource2> source2,
            IObservable<TSource3> source3,
            IObservable<TSource4> source4,
            IObservable<TSource5> source5,
            IObservable<TSource6> source6,
            Func<TSource1, TSource2, TSource3, TSource4, TSource5, TSource6, TResult> resultSelector)
        {
            Check.NotNull(source1, nameof(source1));
            Check.NotNull(source2, nameof(source2));
            Check.NotNull(source3, nameof(source3));
            Check.NotNull(source4, nameof(source4));
            Check.NotNull(source5, nameof(source5));
            Check.NotNull(source6, nameof(source6));
            Check.NotNull(resultSelector, nameof(resultSelector));

            return source1
                .Zip(source2, source3, source4, source5, (x1, x2, x3, x4, x5) => new { x1, x2, x3, x4, x5 })
                .Zip(source6, (a, x6) => resultSelector(a.x1, a.x2, a.x3, a.x4, a.x5, x6));
        }

        public static IAsyncObservable<TResult> Zip<TSource1, TSource2, TSource3, TSource4, TSource5, TSource6, TSource7, TResult>(
            this IAsyncObservable<TSource1> source1,
            IObservable<TSource2> source2,
            IObservable<TSource3> source3,
            IObservable<TSource4> source4,
            IObservable<TSource5> source5,
            IObservable<TSource6> source6,
            IObservable<TSource7> source7,
            Func<TSource1, TSource2, TSource3, TSource4, TSource5, TSource6, TSource7, TResult> resultSelector)
        {
            Check.NotNull(source1, nameof(source1));
            Check.NotNull(source2, nameof(source2));
            Check.NotNull(source3, nameof(source3));
            Check.NotNull(source4, nameof(source4));
            Check.NotNull(source5, nameof(source5));
            Check.NotNull(source6, nameof(source6));
            Check.NotNull(source7, nameof(source7));
            Check.NotNull(resultSelector, nameof(resultSelector));

            return source1
                .Zip(source2, source3, source4, source5, source6, (x1, x2, x3, x4, x5, x6) => new { x1, x2, x3, x4, x5, x6 })
                .Zip(source7, (a, x7) => resultSelector(a.x1, a.x2, a.x3, a.x4, a.x5, a.x6, x7));
        }

        public static IAsyncObservable<TResult> Zip<TSource1, TSource2, TSource3, TSource4, TSource5, TSource6, TSource7, TSource8, TResult>(
            this IAsyncObservable<TSource1> source1,
            IObservable<TSource2> source2,
            IObservable<TSource3> source3,
            IObservable<TSource4> source4,
            IObservable<TSource5> source5,
            IObservable<TSource6> source6,
            IObservable<TSource7> source7,
            IObservable<TSource8> source8,
            Func<TSource1, TSource2, TSource3, TSource4, TSource5, TSource6, TSource7, TSource8, TResult> resultSelector)
        {
            Check.NotNull(source1, nameof(source1));
            Check.NotNull(source2, nameof(source2));
            Check.NotNull(source3, nameof(source3));
            Check.NotNull(source4, nameof(source4));
            Check.NotNull(source5, nameof(source5));
            Check.NotNull(source6, nameof(source6));
            Check.NotNull(source7, nameof(source7));
            Check.NotNull(source8, nameof(source8));
            Check.NotNull(resultSelector, nameof(resultSelector));

            return source1
                .Zip(source2, source3, source4, source5, source6, source7, (x1, x2, x3, x4, x5, x6, x7) => new { x1, x2, x3, x4, x5, x6, x7 })
                .Zip(source8, (a, x8) => resultSelector(a.x1, a.x2, a.x3, a.x4, a.x5, a.x6, a.x7, x8));
        }

        public static IAsyncObservable<TResult> Zip<TSource1, TSource2, TSource3, TSource4, TSource5, TSource6, TSource7, TSource8, TSource9, TResult>(
            this IAsyncObservable<TSource1> source1,
            IObservable<TSource2> source2,
            IObservable<TSource3> source3,
            IObservable<TSource4> source4,
            IObservable<TSource5> source5,
            IObservable<TSource6> source6,
            IObservable<TSource7> source7,
            IObservable<TSource8> source8,
            IObservable<TSource9> source9,
            Func<TSource1, TSource2, TSource3, TSource4, TSource5, TSource6, TSource7, TSource8, TSource9, TResult> resultSelector)
        {
            Check.NotNull(source1, nameof(source1));
            Check.NotNull(source2, nameof(source2));
            Check.NotNull(source3, nameof(source3));
            Check.NotNull(source4, nameof(source4));
            Check.NotNull(source5, nameof(source5));
            Check.NotNull(source6, nameof(source6));
            Check.NotNull(source7, nameof(source7));
            Check.NotNull(source8, nameof(source8));
            Check.NotNull(source9, nameof(source9));
            Check.NotNull(resultSelector, nameof(resultSelector));

            return source1
                .Zip(source2, source3, source4, source5, source6, source7, source8, (x1, x2, x3, x4, x5, x6, x7, x8) => new { x1, x2, x3, x4, x5, x6, x7, x8 })
                .Zip(source9, (a, x9) => resultSelector(a.x1, a.x2, a.x3, a.x4, a.x5, a.x6, a.x7, a.x8, x9));
        }

        public static IAsyncObservable<TResult> Zip<TSource1, TSource2, TSource3, TSource4, TSource5, TSource6, TSource7, TSource8, TSource9, TSource10, TResult>(
            this IAsyncObservable<TSource1> source1,
            IObservable<TSource2> source2,
            IObservable<TSource3> source3,
            IObservable<TSource4> source4,
            IObservable<TSource5> source5,
            IObservable<TSource6> source6,
            IObservable<TSource7> source7,
            IObservable<TSource8> source8,
            IObservable<TSource9> source9,
            IObservable<TSource10> source10,
            Func<TSource1, TSource2, TSource3, TSource4, TSource5, TSource6, TSource7, TSource8, TSource9, TSource10, TResult> resultSelector)
        {
            Check.NotNull(source1, nameof(source1));
            Check.NotNull(source2, nameof(source2));
            Check.NotNull(source3, nameof(source3));
            Check.NotNull(source4, nameof(source4));
            Check.NotNull(source5, nameof(source5));
            Check.NotNull(source6, nameof(source6));
            Check.NotNull(source7, nameof(source7));
            Check.NotNull(source8, nameof(source8));
            Check.NotNull(source9, nameof(source9));
            Check.NotNull(source10, nameof(source10));
            Check.NotNull(resultSelector, nameof(resultSelector));

            return source1
                .Zip(source2, source3, source4, source5, source6, source7, source8, source9, (x1, x2, x3, x4, x5, x6, x7, x8, x9) => new { x1, x2, x3, x4, x5, x6, x7, x8, x9 })
                .Zip(source10, (a, x10) => resultSelector(a.x1, a.x2, a.x3, a.x4, a.x5, a.x6, a.x7, a.x8, a.x9, x10));
        }

        public static IAsyncObservable<TResult> Zip<TSource1, TSource2, TSource3, TSource4, TSource5, TSource6, TSource7, TSource8, TSource9, TSource10, TSource11, TResult>(
            this IAsyncObservable<TSource1> source1,
            IObservable<TSource2> source2,
            IObservable<TSource3> source3,
            IObservable<TSource4> source4,
            IObservable<TSource5> source5,
            IObservable<TSource6> source6,
            IObservable<TSource7> source7,
            IObservable<TSource8> source8,
            IObservable<TSource9> source9,
            IObservable<TSource10> source10,
            IObservable<TSource11> source11,
            Func<TSource1, TSource2, TSource3, TSource4, TSource5, TSource6, TSource7, TSource8, TSource9, TSource10, TSource11, TResult> resultSelector)
        {
            Check.NotNull(source1, nameof(source1));
            Check.NotNull(source2, nameof(source2));
            Check.NotNull(source3, nameof(source3));
            Check.NotNull(source4, nameof(source4));
            Check.NotNull(source5, nameof(source5));
            Check.NotNull(source6, nameof(source6));
            Check.NotNull(source7, nameof(source7));
            Check.NotNull(source8, nameof(source8));
            Check.NotNull(source9, nameof(source9));
            Check.NotNull(source10, nameof(source10));
            Check.NotNull(source11, nameof(source11));
            Check.NotNull(resultSelector, nameof(resultSelector));

            return source1
                .Zip(source2, source3, source4, source5, source6, source7, source8, source9, source10, (x1, x2, x3, x4, x5, x6, x7, x8, x9, x10) => new { x1, x2, x3, x4, x5, x6, x7, x8, x9, x10 })
                .Zip(source11, (a, x11) => resultSelector(a.x1, a.x2, a.x3, a.x4, a.x5, a.x6, a.x7, a.x8, a.x9, a.x10, x11));
        }

        public static IAsyncObservable<TResult> Zip<TSource1, TSource2, TSource3, TSource4, TSource5, TSource6, TSource7, TSource8, TSource9, TSource10, TSource11, TSource12, TResult>(
            this IAsyncObservable<TSource1> source1,
            IObservable<TSource2> source2,
            IObservable<TSource3> source3,
            IObservable<TSource4> source4,
            IObservable<TSource5> source5,
            IObservable<TSource6> source6,
            IObservable<TSource7> source7,
            IObservable<TSource8> source8,
            IObservable<TSource9> source9,
            IObservable<TSource10> source10,
            IObservable<TSource11> source11,
            IObservable<TSource12> source12,
            Func<TSource1, TSource2, TSource3, TSource4, TSource5, TSource6, TSource7, TSource8, TSource9, TSource10, TSource11, TSource12, TResult> resultSelector)
        {
            Check.NotNull(source1, nameof(source1));
            Check.NotNull(source2, nameof(source2));
            Check.NotNull(source3, nameof(source3));
            Check.NotNull(source4, nameof(source4));
            Check.NotNull(source5, nameof(source5));
            Check.NotNull(source6, nameof(source6));
            Check.NotNull(source7, nameof(source7));
            Check.NotNull(source8, nameof(source8));
            Check.NotNull(source9, nameof(source9));
            Check.NotNull(source10, nameof(source10));
            Check.NotNull(source11, nameof(source11));
            Check.NotNull(source12, nameof(source12));
            Check.NotNull(resultSelector, nameof(resultSelector));

            return source1
                .Zip(source2, source3, source4, source5, source6, source7, source8, source9, source10, source11, (x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11) => new { x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11 })
                .Zip(source12, (a, x12) => resultSelector(a.x1, a.x2, a.x3, a.x4, a.x5, a.x6, a.x7, a.x8, a.x9, a.x10, a.x11, x12));
        }

        public static IAsyncObservable<TResult> Zip<TSource1, TSource2, TSource3, TSource4, TSource5, TSource6, TSource7, TSource8, TSource9, TSource10, TSource11, TSource12, TSource13, TResult>(
            this IAsyncObservable<TSource1> source1,
            IObservable<TSource2> source2,
            IObservable<TSource3> source3,
            IObservable<TSource4> source4,
            IObservable<TSource5> source5,
            IObservable<TSource6> source6,
            IObservable<TSource7> source7,
            IObservable<TSource8> source8,
            IObservable<TSource9> source9,
            IObservable<TSource10> source10,
            IObservable<TSource11> source11,
            IObservable<TSource12> source12,
            IObservable<TSource13> source13,
            Func<TSource1, TSource2, TSource3, TSource4, TSource5, TSource6, TSource7, TSource8, TSource9, TSource10, TSource11, TSource12, TSource13, TResult> resultSelector)
        {
            Check.NotNull(source1, nameof(source1));
            Check.NotNull(source2, nameof(source2));
            Check.NotNull(source3, nameof(source3));
            Check.NotNull(source4, nameof(source4));
            Check.NotNull(source5, nameof(source5));
            Check.NotNull(source6, nameof(source6));
            Check.NotNull(source7, nameof(source7));
            Check.NotNull(source8, nameof(source8));
            Check.NotNull(source9, nameof(source9));
            Check.NotNull(source10, nameof(source10));
            Check.NotNull(source11, nameof(source11));
            Check.NotNull(source12, nameof(source12));
            Check.NotNull(source13, nameof(source13));
            Check.NotNull(resultSelector, nameof(resultSelector));

            return source1
                .Zip(source2, source3, source4, source5, source6, source7, source8, source9, source10, source11, source12, (x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11, x12) => new { x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11, x12 })
                .Zip(source13, (a, x13) => resultSelector(a.x1, a.x2, a.x3, a.x4, a.x5, a.x6, a.x7, a.x8, a.x9, a.x10, a.x11, a.x12, x13));
        }

        public static IAsyncObservable<TResult> Zip<TSource1, TSource2, TSource3, TSource4, TSource5, TSource6, TSource7, TSource8, TSource9, TSource10, TSource11, TSource12, TSource13, TSource14, TResult>(
            this IAsyncObservable<TSource1> source1,
            IObservable<TSource2> source2,
            IObservable<TSource3> source3,
            IObservable<TSource4> source4,
            IObservable<TSource5> source5,
            IObservable<TSource6> source6,
            IObservable<TSource7> source7,
            IObservable<TSource8> source8,
            IObservable<TSource9> source9,
            IObservable<TSource10> source10,
            IObservable<TSource11> source11,
            IObservable<TSource12> source12,
            IObservable<TSource13> source13,
            IObservable<TSource14> source14,
            Func<TSource1, TSource2, TSource3, TSource4, TSource5, TSource6, TSource7, TSource8, TSource9, TSource10, TSource11, TSource12, TSource13, TSource14, TResult> resultSelector)
        {
            Check.NotNull(source1, nameof(source1));
            Check.NotNull(source2, nameof(source2));
            Check.NotNull(source3, nameof(source3));
            Check.NotNull(source4, nameof(source4));
            Check.NotNull(source5, nameof(source5));
            Check.NotNull(source6, nameof(source6));
            Check.NotNull(source7, nameof(source7));
            Check.NotNull(source8, nameof(source8));
            Check.NotNull(source9, nameof(source9));
            Check.NotNull(source10, nameof(source10));
            Check.NotNull(source11, nameof(source11));
            Check.NotNull(source12, nameof(source12));
            Check.NotNull(source13, nameof(source13));
            Check.NotNull(source14, nameof(source14));
            Check.NotNull(resultSelector, nameof(resultSelector));

            return source1
                .Zip(source2, source3, source4, source5, source6, source7, source8, source9, source10, source11, source12, source13, (x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11, x12, x13) => new { x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11, x12, x13 })
                .Zip(source14, (a, x14) => resultSelector(a.x1, a.x2, a.x3, a.x4, a.x5, a.x6, a.x7, a.x8, a.x9, a.x10, a.x11, a.x12, a.x13, x14));
        }

        public static IAsyncObservable<TResult> Zip<TSource1, TSource2, TSource3, TSource4, TSource5, TSource6, TSource7, TSource8, TSource9, TSource10, TSource11, TSource12, TSource13, TSource14, TSource15, TResult>(
            this IAsyncObservable<TSource1> source1,
            IObservable<TSource2> source2,
            IObservable<TSource3> source3,
            IObservable<TSource4> source4,
            IObservable<TSource5> source5,
            IObservable<TSource6> source6,
            IObservable<TSource7> source7,
            IObservable<TSource8> source8,
            IObservable<TSource9> source9,
            IObservable<TSource10> source10,
            IObservable<TSource11> source11,
            IObservable<TSource12> source12,
            IObservable<TSource13> source13,
            IObservable<TSource14> source14,
            IObservable<TSource15> source15,
            Func<TSource1, TSource2, TSource3, TSource4, TSource5, TSource6, TSource7, TSource8, TSource9, TSource10, TSource11, TSource12, TSource13, TSource14, TSource15, TResult> resultSelector)
        {
            Check.NotNull(source1, nameof(source1));
            Check.NotNull(source2, nameof(source2));
            Check.NotNull(source3, nameof(source3));
            Check.NotNull(source4, nameof(source4));
            Check.NotNull(source5, nameof(source5));
            Check.NotNull(source6, nameof(source6));
            Check.NotNull(source7, nameof(source7));
            Check.NotNull(source8, nameof(source8));
            Check.NotNull(source9, nameof(source9));
            Check.NotNull(source10, nameof(source10));
            Check.NotNull(source11, nameof(source11));
            Check.NotNull(source12, nameof(source12));
            Check.NotNull(source13, nameof(source13));
            Check.NotNull(source14, nameof(source14));
            Check.NotNull(source15, nameof(source15));
            Check.NotNull(resultSelector, nameof(resultSelector));

            return source1
                .Zip(source2, source3, source4, source5, source6, source7, source8, source9, source10, source11, source12, source13, source14, (x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11, x12, x13, x14) => new { x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11, x12, x13, x14 })
                .Zip(source15, (a, x15) => resultSelector(a.x1, a.x2, a.x3, a.x4, a.x5, a.x6, a.x7, a.x8, a.x9, a.x10, a.x11, a.x12, a.x13, a.x14, x15));
        }

        public static IAsyncObservable<TResult> Zip<TSource1, TSource2, TSource3, TSource4, TSource5, TSource6, TSource7, TSource8, TSource9, TSource10, TSource11, TSource12, TSource13, TSource14, TSource15, TSource16, TResult>(
            this IAsyncObservable<TSource1> source1,
            IObservable<TSource2> source2,
            IObservable<TSource3> source3,
            IObservable<TSource4> source4,
            IObservable<TSource5> source5,
            IObservable<TSource6> source6,
            IObservable<TSource7> source7,
            IObservable<TSource8> source8,
            IObservable<TSource9> source9,
            IObservable<TSource10> source10,
            IObservable<TSource11> source11,
            IObservable<TSource12> source12,
            IObservable<TSource13> source13,
            IObservable<TSource14> source14,
            IObservable<TSource15> source15,
            IObservable<TSource16> source16,
            Func<TSource1, TSource2, TSource3, TSource4, TSource5, TSource6, TSource7, TSource8, TSource9, TSource10, TSource11, TSource12, TSource13, TSource14, TSource15, TSource16, TResult> resultSelector)
        {
            Check.NotNull(source1, nameof(source1));
            Check.NotNull(source2, nameof(source2));
            Check.NotNull(source3, nameof(source3));
            Check.NotNull(source4, nameof(source4));
            Check.NotNull(source5, nameof(source5));
            Check.NotNull(source6, nameof(source6));
            Check.NotNull(source7, nameof(source7));
            Check.NotNull(source8, nameof(source8));
            Check.NotNull(source9, nameof(source9));
            Check.NotNull(source10, nameof(source10));
            Check.NotNull(source11, nameof(source11));
            Check.NotNull(source12, nameof(source12));
            Check.NotNull(source13, nameof(source13));
            Check.NotNull(source14, nameof(source14));
            Check.NotNull(source15, nameof(source15));
            Check.NotNull(source16, nameof(source16));
            Check.NotNull(resultSelector, nameof(resultSelector));

            return source1
                .Zip(source2, source3, source4, source5, source6, source7, source8, source9, source10, source11, source12, source13, source14, source15, (x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11, x12, x13, x14, x15) => new { x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11, x12, x13, x14, x15 })
                .Zip(source16, (a, x16) => resultSelector(a.x1, a.x2, a.x3, a.x4, a.x5, a.x6, a.x7, a.x8, a.x9, a.x10, a.x11, a.x12, a.x13, a.x14, a.x15, x16));

        }
    }
}
