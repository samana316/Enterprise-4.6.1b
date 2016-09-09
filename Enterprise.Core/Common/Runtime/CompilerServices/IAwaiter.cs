using System.Runtime.CompilerServices;

namespace Enterprise.Core.Common.Runtime.CompilerServices
{
    public interface IAwaiter : ICriticalNotifyCompletion, INotifyCompletion
    {
        bool IsCompleted { get; }

        void GetResult();
    }
}
