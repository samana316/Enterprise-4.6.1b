using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Reactive.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Enterprise.Tests.Reactive.Helpers;
using System.Diagnostics;

namespace Enterprise.Tests.Reactive.Temp
{
    [TestClass]
    public class TempTest
    {
        [TestMethod]
        [Timeout(60000)]
        public async Task TestMethod1()
        {
            //var source = new[]
            //{
            //    Observable.Range(1, 2),
            //    Observable.Range(11, 2),
            //    Observable.Range(21, 2),
            //    Observable.Range(31, 2),
            //}.ToObservable();

            var source = new[]
            {
                Task.FromResult(1),
                Task.Run(async () => { await Task.Delay(100); return 2; }),
                Task.Run(async () => { await Task.Delay(500); return 3; }),
                Task.Run(async () => { await Task.Delay(100); return 4; }),
                Task.FromResult(5),
            }.ToObservable();

            var query = source.Switch();
            var list = await query.ToList();

            foreach (var item in list)
            {
                Trace.WriteLine(item);
            }
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
