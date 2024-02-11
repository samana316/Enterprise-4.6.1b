using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Enterprise.Core.Reactive
{
    internal sealed partial class CompositeAsyncObserver<T> : AsyncObserverBase<T>
    {
        private readonly object sink = new object();

        private readonly ICollection<IAsyncObserver<T>> observers = new List<IAsyncObserver<T>>();

        public override void OnCompleted()
        {
            lock (sink)
            {
                foreach (var observer in this.observers.ToArray())
                {
                    if (observer != null)
                    {
                        observer.OnCompleted();
                    }
                }
            }
        }

        public override void OnError(
            Exception error)
        {
            lock (sink)
            {
                foreach (var observer in this.observers)
                {
                    if (observer != null)
                    {
                        observer.OnError(error);
                    }
                }
            }
        }

        public override Task OnNextAsync(
            T value, 
            CancellationToken cancellationToken)
        {
            var tasks =
                from observer in this.observers
                where observer != null
                select observer.OnNextAsync(value, cancellationToken);

            return Task.WhenAll(tasks);
        }

        protected override void Dispose(
            bool disposing)
        {
            lock (sink)
            {
                foreach (var observer in this.observers.ToArray())
                {
                    var disposable = observer as IDisposable;
                    if (disposable == null)
                    {
                        continue;
                    }

                    disposable.Dispose();
                }
            }

            base.Dispose(disposing);
        }
    }

    partial class CompositeAsyncObserver<T> : ICollection<IAsyncObserver<T>>
    {
        public int Count
        {
            get
            {
                return this.observers.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return this.observers.IsReadOnly;
            }
        }

        public void Add(
            IAsyncObserver<T> item)
        {
            this.observers.Add(item);
        }

        public void Clear()
        {
            this.observers.Clear();
        }

        public bool Contains(
            IAsyncObserver<T> item)
        {
            return this.observers.Contains(item);
        }

        public void CopyTo(
            IAsyncObserver<T>[] array, 
            int arrayIndex)
        {
            this.observers.CopyTo(array, arrayIndex);
        }

        public IEnumerator<IAsyncObserver<T>> GetEnumerator()
        {
            return this.observers.GetEnumerator();
        }

        public bool Remove(
            IAsyncObserver<T> item)
        {
            return this.observers.Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }

    partial class CompositeAsyncObserver<T> : IReadOnlyCollection<IAsyncObserver<T>>
    {
    }
}
