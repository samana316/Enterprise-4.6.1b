using System;
using Enterprise.Core.Common.Runtime.CompilerServices;

namespace Enterprise.Core.Reactive
{
    public interface IAsyncSubscription : IAwaitable, IDisposable
    {
    }
}
