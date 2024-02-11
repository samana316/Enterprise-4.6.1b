using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Common.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Enterprise.Tests.Poc
{
    [TestClass]
    public sealed class CustomAsyncTest
    {
        private const string CategoryPocAsync = "Poc.Async";

        [TestMethod]
        [TestCategory(CategoryPocAsync)]
        public async Task CustomCancellationTokenSource()
        {
            var cts = new MyCancellationTokenSource();
            var cancellationToken = cts.Token;

            var cts2 = typeof(CancellationToken)
                .GetField("m_source", BindingFlags.Instance | BindingFlags.NonPublic)
                .GetValue(cancellationToken) as CancellationTokenSource;

            Trace.WriteLine(cts2.GetType().AssemblyQualifiedName);

            await Task.Yield();
        }

        private sealed class MyCancellationTokenSource : CancellationTokenSource
        {
        }
    }
}
