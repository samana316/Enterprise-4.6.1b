using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Enterprise.Core.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Enterprise.Tests.Poc
{
    [TestClass]
    public class DebuggerTest
    {
        [TestMethod]
        [TestCategory("Poc.Debugger")]
        public void DebuggerTest1()
        {
            var x = new TestAsyncEnumerable<int>();

            Trace.WriteLine(x);
        }

        
        [DebuggerDisplay("{DebuggerView}")]
        private class TestAsyncEnumerable<T> : IAsyncEnumerable<T>
        {
            private readonly IAsyncEnumerable<T> impl = AsyncEnumerable.Repeat(default(T), 1);

            public IAsyncEnumerator<T> GetAsyncEnumerator()
            {
                return this.impl.GetAsyncEnumerator();
            }

            public IEnumerator<T> GetEnumerator()
            {
                return this.GetAsyncEnumerator();
            }

            IAsyncEnumerator IAsyncEnumerable.GetAsyncEnumerator()
            {
                return this.GetAsyncEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            private IEnumerable<T> DebuggerView
            {
                get
                {
                    return this.impl.ToArray();
                }
            }
        }
    }
}
