using System;
using System.Reactive.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Enterprise.Tests.Reactive.Temp
{
    //[TestClass]
    public class TempTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var x = Observable.Merge<int>(Observable.Return(1));
        }
    }
}
