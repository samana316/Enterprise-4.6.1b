namespace Enterprise.Core.Common.Runtime.CompilerServices
{
    public interface IAwaitable
    {
        IAwaiter GetAwaiter();
    }
}
