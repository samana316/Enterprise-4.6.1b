using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Linq;
using Enterprise.Core.Reactive.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Enterprise.Tests.Reactive.Temp
{
    [TestClass]
    public sealed class CodeGenerationTest
    {
        [TestMethod]
        [TestCategory("Temp")]
        [Timeout(1000)]
        public async Task GenerateZipOverloads()
        {
            var cancellationToken = CancellationToken.None;

            var start = 5;
            var end = 16;

            for (var i = start; i <= end; i++)
            {
                var code = await this.GenerateZipOverloadsAsync(i, cancellationToken);

                Trace.WriteLine(code);
            }
        }

        public async Task<string> GenerateZipOverloadsAsync(
            int sourceNum,
            CancellationToken cancellationToken)
        {
            if (sourceNum > 16 || sourceNum < 3)
            {
                throw new ArgumentOutOfRangeException(nameof(sourceNum));
            }

            var source = AsyncEnumerable.Range(1, sourceNum);

            var builder = new StringBuilder();
            builder.Append("public static IAsyncObservable<TResult> Zip<");

            builder
                .Append(string.Join(", ", source.Select(x => "TSource" + x).Concat(new[] { "TResult" })))
                .Append(">(")
                .AppendLine()
                .Append("\t").Append("this IAsyncObservable<TSource1> source1,")
                .AppendLine();

            await source.Skip(1).ForEachAsync(num => 
            {
                builder
                    .Append("\t")
                    .AppendFormat("IObservable<TSource{0}> source{0},", num)
                    .AppendLine();
            }, cancellationToken);

            builder
                .Append("\t")
                .Append("Func<")
                .Append(string.Join(", ", source.Select(x => "TSource" + x).Concat(new[] { "TResult" })))
                .Append("> resultSelector)")
                .AppendLine()
                .Append("{")
                .AppendLine();

            await source.ForEachAsync(num =>
            {
                builder
                    .Append("\t")
                    .AppendFormat("Check.NotNull(source{0}, nameof(source{0}));", num)
                    .AppendLine();
            }, cancellationToken);

            builder
                .Append("\tCheck.NotNull(resultSelector, nameof(resultSelector));")
                .AppendLine()
                .AppendLine();

            var maxNum = await source.MaxAsync(cancellationToken);
            var previousNums = source.Where(x => x < maxNum);

            builder
                .Append("\t")
                .Append("return source1")
                .AppendLine()
                .Append("\t\t")
                .Append(".Zip(")
                .Append(string.Join(", ", previousNums.Skip(1).Select(x => "source" + x)))
                .Append(", (")
                .Append(string.Join(", ", previousNums.Select(x => "x" + x)))
                .Append(") => new { ")
                .Append(string.Join(", ", previousNums.Select(x => "x" + x)))
                .Append(" })")
                .AppendLine()
                .Append("\t\t")
                .Append(".Zip(");

            builder
                .AppendFormat("source{0}, (a, x{0}) => resultSelector(", maxNum)
                .Append(string.Join(", ", previousNums.Select(x => "a.x" + x)))
                .AppendFormat(", x{0}));", maxNum)
                .AppendLine();

            builder.Append("}").AppendLine();

            return builder.ToString();
        }
    }
}
