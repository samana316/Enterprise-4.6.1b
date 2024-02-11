using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Utilities;

namespace Enterprise.Core.Linq
{
    partial class AsyncEnumerable
    {
        public static Task<bool> SequenceEqualAsync<TSource>(
           this IAsyncEnumerable<TSource> first,
           IEnumerable<TSource> second)
        {
            return first.SequenceEqualAsync(second, CancellationToken.None);
        }

        public static Task<bool> SequenceEqualAsync<TSource>(
           this IAsyncEnumerable<TSource> first,
           IEnumerable<TSource> second,
           CancellationToken cancellationToken)
        {
            return first.SequenceEqualAsync(second, EqualityComparer<TSource>.Default);
        }

        public static Task<bool> SequenceEqualAsync<TSource>(
            this IAsyncEnumerable<TSource> first,
            IEnumerable<TSource> second,
            IEqualityComparer<TSource> comparer)
        {
            return first.SequenceEqualAsync(second, comparer, CancellationToken.None);
        }

        public static async Task<bool> SequenceEqualAsync<TSource>(
            this IAsyncEnumerable<TSource> first,
            IEnumerable<TSource> second,
            IEqualityComparer<TSource> comparer,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(first, nameof(first));
            Check.NotNull(second, nameof(second));

            if (ReferenceEquals(comparer, null))
            {
                comparer = EqualityComparer<TSource>.Default;
            }

            int count1, count2;
            if (first.TryFastCount(out count1) &&
                second.TryFastCount(out count2) &&
                count1 != count2)
            {
                return false;
            }

            using (IAsyncEnumerator<TSource> 
                iterator1 = first.GetAsyncEnumerator(),
                iterator2 = second.AsAsyncEnumerable().GetAsyncEnumerator())
            {
                while (true)
                {
                    var next1 = await iterator1.MoveNextAsync(cancellationToken);
                    var next2 = await iterator2.MoveNextAsync(cancellationToken);

                    if (next1 != next2)
                    {
                        return false;
                    }

                    if (!next1)
                    {
                        return true;
                    }

                    if (!comparer.Equals(iterator1.Current, iterator2.Current))
                    {
                        return false;
                    }
                }
            }
        }
    }
}