using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Reactive.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Enterprise.Tests.Reactive.Temp
{
    [TestClass]
    public class TempTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var x = Observable.Merge<int>(Observable.Return(1));
        }

        [TestMethod]
        [TestCategory("Integration")]
        [Timeout(60000)]
        public async Task IOTest1()
        {
            var source = AsyncObservable.Create<KeyValuePair<string, string>>(async (yield, cancellationToken) => 
            {
                var directories = Directory.GetDirectories(@"C:\Git\Fanatics\Wms");

                var files =
                    from directory in directories
                    where !directory.Contains("Wms2")
                    select Directory.GetFiles(directory, "*.cs", SearchOption.AllDirectories);

                foreach (var file in files.SelectMany(x => x))
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    using (var stream = File.OpenRead(file))
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            var text = await reader.ReadToEndAsync();

                            var result = new KeyValuePair<string, string>(file, text);

                            await yield.ReturnAsync(result, cancellationToken);
                        }
                    }
                }
            });

            var query = from item in source where item.Value.Contains("Descrepant") select item.Key;
            await query.ForEachAsync((x, ct) => Console.Out.WriteLineAsync(x), CancellationToken.None);
        }
    }
}
