using System.ComponentModel;

namespace Enterprise.Tests.Reactive.Helpers
{
    internal sealed class TestNotifyPropertyChanged<TSource> : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private TSource state;

        public TSource State
        {
            get
            {
                return this.state;
            }
            set
            {
                this.state = value;
                this.RaisePropertyChanged(nameof(State));
            }
        }

        private void RaisePropertyChanged(
            string propertyName)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
