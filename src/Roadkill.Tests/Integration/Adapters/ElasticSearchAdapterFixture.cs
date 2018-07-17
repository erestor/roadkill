using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using Nest;
using Roadkill.Core.Adapters;
using Roadkill.Core.Models;

namespace Roadkill.Tests.Integration.Adapters
{
	public class ElasticSearchAdapterFixture : IDisposable
	{
		public ElasticClient ElasticClient { get; set; }
		public List<SearchablePage> TestPages { get; set; }

		public ElasticSearchAdapterFixture()
		{
			var node = new Uri("http://localhost:9200");
			var connectionSettings = new ConnectionSettings(node);
			ElasticClient = new ElasticClient(connectionSettings);
			ElasticClient.DeleteIndex(ElasticSearchAdapter.PagesIndexName);

			AddDummyData(ElasticClient);
		}

		private void AddDummyData(ElasticClient elasticClient)
		{
			var fixture = new Fixture();
			var adapter = new ElasticSearchAdapter(elasticClient);

			TestPages = fixture.CreateMany<SearchablePage>(10).ToList();
			foreach (SearchablePage page in TestPages)
			{
				adapter.Add(page).GetAwaiter().GetResult();
			}
		}

		public void Dispose()
		{
			ElasticClient.DeleteIndex(ElasticSearchAdapter.PagesIndexName);
		}
	}
}