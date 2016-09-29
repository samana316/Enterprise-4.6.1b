using System;

namespace Enterprise.Tests.Reactive.Helpers
{
    internal static class DivideByZero
    {
        public static Action Instance = Impl;

        private static void Impl()
        {
            var x = 1;
            var y = 0;
            var z = x / y;
        }
    }
}
