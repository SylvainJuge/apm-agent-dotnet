// Licensed to Elasticsearch B.V under
// one or more agreements.
// Elasticsearch B.V licenses this file to you under the Apache 2.0 License.
// See the LICENSE file in the project root for more information

using System.Threading.Tasks;
using Testcontainers.MsSql;
using Xunit;

namespace Elastic.Apm.Profiler.Managed.Tests.AdoNet
{
	[CollectionDefinition("SqlServer")]
	public sealed class SqlServerCollection : ICollectionFixture<SqlServerFixture> { }

	public sealed class SqlServerFixture : IAsyncLifetime
	{
		private readonly MsSqlContainer _container = new MsSqlBuilder().Build();

		public string ConnectionString => _container.GetConnectionString();

		public Task InitializeAsync() => _container.StartAsync();

		public Task DisposeAsync() => _container.DisposeAsync().AsTask();
	}
}
