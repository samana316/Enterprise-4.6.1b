﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading;
using Enterprise.Core.Linq;
using Enterprise.Core.Reactive.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Enterprise.Tests.Linq
{
    [TestClass]
    public class CompareLinqMethods
    {
        [TestMethod]
        [TestCategory("Linq.Temp")]
        [Timeout(30000)]
        public void CompareEnumerableLinqMethods()
        {
            try
            {
                var linqMethods = this.GetLinqMethods(typeof(Enumerable));
                var asyncLinqMethods = this.GetLinqMethods(typeof(AsyncEnumerable));

                var query = linqMethods.Except(asyncLinqMethods);

                foreach (var item in query)
                {
                    Trace.WriteLine(item);
                }
            }
            catch (Exception exception)
            {
                Trace.WriteLine(exception);

                Assert.Fail(exception.Message);
            }
        }

        [TestMethod]
        [TestCategory("Linq.Temp")]
        [Timeout(30000)]
        public void CompareObservableLinqMethods()
        {
            try
            {
                var linqMethods = this.GetLinqMethods(typeof(Enumerable));
                var asyncLinqMethods = this.GetLinqMethods(typeof(AsyncObservable));

                var query = linqMethods.Except(asyncLinqMethods);

                foreach (var item in query)
                {
                    Trace.WriteLine(item);
                }
            }
            catch (Exception exception)
            {
                Trace.WriteLine(exception);

                Assert.Fail(exception.Message);
            }
        }

        [TestMethod]
        [TestCategory("Linq.Temp")]
        [Timeout(30000)]
        public void CompareReactiveLinqMethods()
        {
            try
            {
                var linqMethods = this.GetLinqMethods(typeof(Observable));
                var asyncLinqMethods = this.GetLinqMethods(typeof(AsyncObservable));

                var query = linqMethods.Except(asyncLinqMethods).Except(this.GetLinqMethods(typeof(Enumerable)));

                foreach (var item in query)
                {
                    Trace.WriteLine(item);
                }
            }
            catch (Exception exception)
            {
                Trace.WriteLine(exception);

                Assert.Fail(exception.Message);
            }
        }

        [TestMethod]
        [TestCategory("Linq.Temp")]
        [Timeout(30000)]
        public void TempLinqTest()
        {
            try
            {
                var source = new int?[] { null };
                Trace.WriteLine(source.Sum(), "Sum");
                Trace.WriteLine(source.Average());
            }
            catch (Exception exception)
            {
                Trace.WriteLine(exception);

                Assert.Fail(exception.Message);
            }
        }

        private IEnumerable<string> GetLinqMethods(
                    Type type)
        {
            var bindingFlags = BindingFlags.Public | BindingFlags.Static;

            var query =
                from method in type.GetMethods(bindingFlags)
                orderby method.Name
                select method.Name.Replace("Async", string.Empty);

            return query.Distinct();
        }
    }
}
