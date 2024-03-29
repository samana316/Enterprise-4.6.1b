﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Utilities;

namespace Enterprise.Core.Linq
{
    internal sealed class AsyncOrderedEnumerable<TElement, TCompositeKey> :
        AsyncEnumerableBase<TElement>,
        IAsyncOrderedEnumerable<TElement>
    {
        private readonly IAsyncEnumerable<TElement> source;

        private readonly Func<TElement, TCompositeKey> compositeSelector;

        private readonly IComparer<TCompositeKey> compositeComparer;

        public AsyncOrderedEnumerable(
            IAsyncEnumerable<TElement> source, 
            Func<TElement, TCompositeKey> selector, 
            IComparer<TCompositeKey> comparer,
            bool descending = false)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(selector, nameof(selector));

            this.source = source;
            this.compositeSelector = selector;

            comparer = comparer ?? Comparer<TCompositeKey>.Default;
            if (descending)
            {
                comparer = new ReverseComparer<TCompositeKey>(comparer);
            }

            this.compositeComparer = comparer;
        }

        public override AsyncIterator<TElement> Clone()
        {
            return new AsyncOrderedEnumerable<TElement, TCompositeKey>(
                this.source, this.compositeSelector, this.compositeComparer);
        }

        public IAsyncOrderedEnumerable<TElement> CreateAsyncOrderedEnumerable<TKey>(
            Func<TElement, TKey> keySelector, 
            IComparer<TKey> comparer, 
            bool descending)
        {
            Check.NotNull(keySelector, nameof(keySelector));

            comparer = comparer ?? Comparer<TKey>.Default;
            if (descending)
            {
                comparer = new ReverseComparer<TKey>(comparer);
            }

            // Copy to a local variable so we don’t need to capture "this"
            var primarySelector = compositeSelector;
            Func<TElement, CompositeKey<TCompositeKey, TKey>> newKeySelector =
                element => new CompositeKey<TCompositeKey, TKey>(primarySelector(element), keySelector(element));

            var newKeyComparer =
                new CompositeComparer<TCompositeKey, TKey>(compositeComparer, comparer);

            return new AsyncOrderedEnumerable<TElement, CompositeKey<TCompositeKey, TKey>>
                (source, newKeySelector, newKeyComparer);
        }

        public IOrderedEnumerable<TElement> CreateOrderedEnumerable<TKey>(
            Func<TElement, TKey> keySelector, 
            IComparer<TKey> comparer, 
            bool descending)
        {
            return this.CreateAsyncOrderedEnumerable(keySelector, comparer, descending);
        }

        protected override async Task EnumerateAsync(
            IAsyncYield<TElement> yield, 
            CancellationToken cancellationToken)
        {
            var buffer = await this.source.ToBufferAsync(cancellationToken);
            var data = buffer.Array;
            var count = buffer.Count;

            var indexes = new int[count];
            for (var i = 0; i < indexes.Length; i++)
            {
                indexes[i] = i;
            }

            var keys = new TCompositeKey[count];
            for (var i = 0; i < keys.Length; i++)
            {
                keys[i] = compositeSelector(data[i]);
            }

            int nextYield = 0;

            var stack = new Stack<LeftRight>();
            stack.Push(new LeftRight(0, count - 1));
            while (stack.Count > 0)
            {
                var leftRight = stack.Pop();
                var left = leftRight.left;
                var right = leftRight.right;
                if (right > left)
                {
                    var pivot = left + (right - left) / 2;
                    var pivotPosition = Partition(indexes, keys, left, right, pivot);
                    // Push the right sublist first, so that we *pop* the
                    // left sublist first
                    stack.Push(new LeftRight(pivotPosition + 1, right));
                    stack.Push(new LeftRight(left, pivotPosition - 1));
                }
                else
                {
                    while (nextYield <= right)
                    {
                        var result = data[indexes[nextYield]];
                        await yield.ReturnAsync(result, cancellationToken);
                        nextYield++;
                    }
                }
            }
        }

        private int Partition(
            int[] indexes, 
            TCompositeKey[] keys, 
            int left, 
            int right, 
            int pivot)
        {
            // Remember the current index (into the keys/elements arrays) of the pivot location
            var pivotIndex = indexes[pivot];
            var pivotKey = keys[pivotIndex];

            // Swap the pivot value to the end
            indexes[pivot] = indexes[right];
            indexes[right] = pivotIndex;
            var storeIndex = left;
            for (var i = left; i < right; i++)
            {
                var candidateIndex = indexes[i];
                var candidateKey = keys[candidateIndex];
                var comparison = compositeComparer.Compare(candidateKey, pivotKey);
                if (comparison < 0 || (comparison == 0 && candidateIndex < pivotIndex))
                {
                    // Swap storeIndex with the current location
                    indexes[i] = indexes[storeIndex];
                    indexes[storeIndex] = candidateIndex;
                    storeIndex++;
                }
            }
            // Move the pivot to its final place
            var tmp = indexes[storeIndex];
            indexes[storeIndex] = indexes[right];
            indexes[right] = tmp;
            return storeIndex;
        }

        private struct CompositeKey<TPrimary, TSecondary>
        {
            private readonly TPrimary primary;

            private readonly TSecondary secondary;

            public CompositeKey(
                TPrimary primary, 
                TSecondary secondary)
            {
                this.primary = primary;
                this.secondary = secondary;
            }

            public TPrimary Primary { get { return this.primary; } }

            public TSecondary Secondary { get { return this.secondary; } }
        }

        private sealed class CompositeComparer<TPrimary, TSecondary> : 
            IComparer<CompositeKey<TPrimary, TSecondary>>
        {
            private readonly IComparer<TPrimary> primaryComparer;

            private readonly IComparer<TSecondary> secondaryComparer;

            public CompositeComparer(
                IComparer<TPrimary> primaryComparer,
                IComparer<TSecondary> secondaryComparer)
            {
                this.primaryComparer = primaryComparer;
                this.secondaryComparer = secondaryComparer;
            }

            public int Compare(
                CompositeKey<TPrimary, TSecondary> x,
                CompositeKey<TPrimary, TSecondary> y)
            {
                int primaryResult = primaryComparer.Compare(x.Primary, y.Primary);
                if (primaryResult != 0)
                {
                    return primaryResult;
                }
                return secondaryComparer.Compare(x.Secondary, y.Secondary);
            }
        }

        private sealed class ReverseComparer<T> : IComparer<T>
        {
            private readonly IComparer<T> forwardComparer;

            internal ReverseComparer(
                IComparer<T> forwardComparer)
            {
                this.forwardComparer = forwardComparer;
            }

            public int Compare(
                T x, 
                T y)
            {
                return forwardComparer.Compare(y, x);
            }
        }

        private struct LeftRight
        {
            internal int left, right;

            public LeftRight(
                int left, 
                int right)
            {
                this.left = left;
                this.right = right;
            }
        }
    }
}
