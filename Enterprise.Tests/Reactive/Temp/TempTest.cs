using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Linq;
using Enterprise.Core.Reactive.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Enterprise.Tests.Reactive.Temp
{
    [TestClass]
    public class TempTest
    {
        [TestMethod]
        [TestCategory("Temp")]
        [Timeout(3000)]
        public async Task TestMethod1()
        {
            var source = Observable.Range(1, 3);
            var observer = Observer.Create<int>(x => Trace.WriteLine(x));

            var subject = System.Reactive.Subjects.Subject.Create<int>(observer, source);
            subject.OnNext(0);
            subject.OnNext(0);
            subject.OnNext(0);
            subject.OnCompleted();

            var list = await subject.ToList();

            foreach (var item in list)
            {
                Trace.WriteLine(list);
            }
        }

        [TestMethod]
        [TestCategory("Temp")]
        [TestCategory("Integration")]
        [Timeout(60000)]
        public async Task FileSystemTextSearch()
        {
            const string basePath = @"C:\Git\Fanatics\Wms";
            const string exclusion = "Wms2.Backup";
            const string searchPattern = "*.cs";
            const string searchText = "PackageInsert";
            
            var paths =
                from directory in Directory.GetDirectories(basePath)
                where !directory.Contains(exclusion)
                let files = Directory.GetFiles(directory, searchPattern, SearchOption.AllDirectories)
                from path in files
                select path;

            var source = AsyncObservable.Create<KeyValuePair<string, string>>(
                new FileSystemTextReader(paths).RunAsync);

            var query = 
                from item in source
                where item.Value.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0
                select item.Key;

            await query.ForEachAsync((x, ct) => Console.Out.WriteLineAsync(x), CancellationToken.None);
        }

        [TestMethod]
        [TestCategory("Temp")]
        [Timeout(3000)]
        public async Task WhenToLeaveOnFriday()
        {
            var period = TimeSpan.FromHours(34.48);
            Trace.WriteLine(period, "Period");
            var clockin = DateTime.Today + TimeSpan.FromHours(11) + TimeSpan.FromMinutes(5);
            Trace.WriteLine(clockin, "Clock In");

            var remaining = TimeSpan.FromHours(40) - period;
            Trace.WriteLine(remaining, "Remaining");

            var clockout = clockin + remaining;
            Trace.WriteLine(clockout, "Clock Out");

            await Task.Yield();
        }

        private sealed class FileSystemTextReader : IAsyncYieldBuilder<KeyValuePair<string, string>>
        {
            private readonly IEnumerable<string> paths;

            public FileSystemTextReader(
                IEnumerable<string> paths)
            {
                if (ReferenceEquals(paths, null))
                {
                    throw new ArgumentNullException(nameof(paths));
                }

                this.paths = paths;
            }

            public async Task RunAsync(
                IAsyncYield<KeyValuePair<string, string>> yield, 
                CancellationToken cancellationToken)
            {
                foreach (var path in paths)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    using (var stream = File.OpenRead(path))
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            var text = await reader.ReadToEndAsync();
                            var result = new KeyValuePair<string, string>(path, text);

                            await yield.ReturnAsync(result, cancellationToken);
                        }
                    }
                }
            }
        }
    }
}
