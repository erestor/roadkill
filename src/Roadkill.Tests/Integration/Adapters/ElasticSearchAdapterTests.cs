using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Nest;
using Roadkill.Core.Adapters;
using Xunit;
using Xunit.Abstractions;
using Page = Roadkill.Core.Models.Page;

namespace Roadkill.Tests.Integration.Adapters
{
	public class ElasticSearchAdapterTests
	{
		private readonly Fixture _fixture;
		private readonly ITestOutputHelper _console;
		private ElasticClient _elasticClient;

		// https://www.docker.elastic.co/
		// https://www.elastic.co/guide/en/elasticsearch/reference/6.2/docker.html
		// docker run -d -p 9200:9200 -p 9300:9300 -e "discovery.type=single-node" docker.elastic.co/elasticsearch/elasticsearch:6.2.4
		// https://github.com/elastic/elasticsearch-net

		// RESTful api examples:
		// https://github.com/sittinash/elasticsearch-postman
		// http://{{url}}:{{port}}/pages/_search?pretty=true&q=*:*
		// GET /_cat/indices?v

		public ElasticSearchAdapterTests(ITestOutputHelper console)
		{
			_fixture = new Fixture();
			_console = console;

			var node = new Uri("http://localhost:9200");
			var connectionSettings = new ConnectionSettings(node);
			_elasticClient = new ElasticClient(connectionSettings);
		}

		[Fact]
		public async Task Find()
		{
			var adapter = new ElasticSearchAdapter(_elasticClient);
			var results = await adapter.Find("long title");

			_console.WriteLine(results.Count().ToString());
			_console.WriteLine(results.First().Title);
		}

		[Fact]
		public async Task Add()
		{
			var pages = _fixture.CreateMany<Page>(10).ToList();
			pages.Add(new Page() { Id = 900, Title = "Long title to search for" });

			var adapter = new ElasticSearchAdapter(_elasticClient);
			foreach (Page page in pages)
			{
				await adapter.Add(page);
			}
		}
	}
}