using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using Enterprise.Core.Linq;
using static System.Data.Common.DbProviderFactories;

namespace Enterprise.Tests.Linq.Helpers.Data
{
    internal static class DbStreamIterator
    {
        public static IAsyncEnumerable<T> Create<T>(
            params T[] items)
        {
            return Create(items as IEnumerable<T>);
        }

        public static IAsyncEnumerable<T> Create<T>(
            IEnumerable<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var settings = GetConnectionString();
            var buffer = source.ToArray();

            return AsyncEnumerable.Create<T>(async (yield, cancellationToken) => 
            {
                using (var connection = new SqlConnection(settings.ConnectionString))
                {
                    await connection.OpenAsync(cancellationToken);

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = string.Empty;

                        for (var i = 0; i < buffer.Length; i++)
                        {
                            if (i > 0)
                            {
                                command.CommandText += " UNION ALL ";
                            }

                            command.CommandText += " SELECT @p" + i;
                            command.Parameters.AddWithValue("@p" + i, buffer[i]);
                        }

                        using (var reader = await command.ExecuteReaderAsync(cancellationToken))
                        {
                            while (await reader.ReadAsync(cancellationToken))
                            {
                                var value = (T)reader[0];

                                await yield.ReturnAsync(value, cancellationToken);
                            }
                        }
                    }
                }
            });
        }

        private static ConnectionStringSettings GetConnectionString()
        {
            const string providerName = "System.Data.SqlClient";
            var provider = GetFactory(providerName);

            var connectionStringBuilder = provider.CreateConnectionStringBuilder();
            connectionStringBuilder.Add("Data Source", @"(LocalDB)\v11.0");
            connectionStringBuilder.Add("Integrated Security", true);

            return new ConnectionStringSettings
            {
                ProviderName = providerName,
                ConnectionString = connectionStringBuilder.ConnectionString
            };
        }
    }
}
