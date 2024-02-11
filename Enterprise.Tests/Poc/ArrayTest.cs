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
    public sealed class ArrayTest
    {
        private const string CategoryPocArray = "Poc.Array";

        [TestMethod]
        [TestCategory(CategoryPocArray)]
        public unsafe void ClrArrayTest()
        {
            var source = new int[5];
            Assert.IsNotNull(source);

            Trace.WriteLine(source.GetType().FullName);

            this.IsOfType<int>(source);
        }

        [TestMethod]
        [TestCategory(CategoryPocArray)]
        [ExpectedException(typeof(NotSupportedException))]
        public unsafe void UnsafeArrayTest()
        {
            var source = Array.CreateInstance(typeof(int), new int[] { 5 }, new int[] { -5 });
            Assert.IsNotNull(source);
            Trace.WriteLine(source.GetType().FullName);
            this.IsOfType<int>(source);

            var list = source as IList;
            var index = list.IndexOf(0);
            Trace.WriteLine(index);

            list[index] = -5;
            list.Add(1);
        }

        private void IsOfType<T>(
            Array source)
        {
            this.IsOfCollectionType<T[]>(source);
            this.IsOfCollectionType<IEnumerable>(source);
            this.IsOfCollectionType<IEnumerable<T>>(source);
            this.IsOfCollectionType<ICollection>(source);
            this.IsOfCollectionType<ICollection<T>>(source);
            this.IsOfCollectionType<IReadOnlyCollection<T>>(source);
            this.IsOfCollectionType<IList>(source);
            this.IsOfCollectionType<IList<T>>(source);
            this.IsOfCollectionType<IReadOnlyList<T>>(source);
        }

        private void IsOfCollectionType<T>(
            Array array)
        {
            Trace.WriteLine(array is T, typeof(T).FullName);
        }
    }
}
