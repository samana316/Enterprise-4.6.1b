using System;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Linq;

namespace Enterprise.Core.Reactive.Linq.Implementations
{
    internal sealed class Generate<TState, TResult> : AsyncObservableBase<TResult>
    {
        private readonly TState initialState;

        private readonly Func<TState, bool> condition;

        private readonly Func<TState, TState> iterate;

        private readonly Func<TState, TResult> resultSelector;

        private readonly Func<TState, TimeSpan> timeSelector;

        private readonly Func<TState, DateTimeOffset> timeSelectorD;

        public Generate(
            TState initialState,
            Func<TState, bool> condition,
            Func<TState, TState> iterate,
            Func<TState, TResult> resultSelector, 
            Func<TState, TimeSpan> timeSelector = null, 
            Func<TState, DateTimeOffset> timeSelectorD = null)
        {
            this.initialState = initialState;
            this.condition = condition;
            this.iterate = iterate;
            this.resultSelector = resultSelector;
            this.timeSelector = timeSelector;
            this.timeSelectorD = timeSelectorD;
        }

        public override AsyncIterator<TResult> Clone()
        {
            return new Generate<TState, TResult>(
                this.initialState, 
                this.condition, 
                this.iterate, 
                this.resultSelector, 
                this.timeSelector, 
                this.timeSelectorD);
        }

        protected override Task ProduceAsync(
            IAsyncYield<TResult> yield, 
            CancellationToken cancellationToken)
        {
            if (this.timeSelectorD != null)
            {
                return this.GenerateImplD(yield, cancellationToken);
            }

            if (this.timeSelector != null)
            {
                return this.GenerateImplT(yield, cancellationToken);
            }

            return this.GenerateImpl(yield, cancellationToken);
        }

        private async Task GenerateImpl(
            IAsyncYield<TResult> yield,
            CancellationToken cancellationToken)
        {
            var next = this.initialState;
            do
            {
                await yield.ReturnAsync(this.resultSelector(next), cancellationToken);
                next = this.iterate(next);
            }
            while (condition(next));
        }

        private async Task GenerateImplT(
            IAsyncYield<TResult> yield,
            CancellationToken cancellationToken)
        {
            var next = this.initialState;
            do
            {
                await Task.Delay(this.timeSelector(next), cancellationToken);
                await yield.ReturnAsync(this.resultSelector(next), cancellationToken);
                next = this.iterate(next);
            }
            while (condition(next));
        }

        private async Task GenerateImplD(
            IAsyncYield<TResult> yield,
            CancellationToken cancellationToken)
        {
            var next = this.initialState;
            do
            {
                var delay = this.timeSelectorD(next) - DateTimeOffset.Now;
                await Task.Delay(delay, cancellationToken);
                await yield.ReturnAsync(this.resultSelector(next), cancellationToken);
                next = this.iterate(next);
            }
            while (condition(next));
        }
    }
}
