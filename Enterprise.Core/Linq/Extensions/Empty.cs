namespace Enterprise.Core.Linq
{
    partial class AsyncEnumerable
    {
        public static IAsyncEnumerable<TResult> Empty<TResult>()
        {
            return Linq.Empty<TResult>.Instance;
        } 
    }
}
