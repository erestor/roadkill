using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Nest;
using Roadkill.Core.Adapters;
using Roadkill.Core.Models;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace Roadkill.Tests.Integration.Adapters
{
	public class ElasticSearchAdapterTests : IClassFixture<ElasticSearchAdapterFixture>
	{
		private readonly Fixture _fixture;
		private readonly ITestOutputHelper _console;
		private readonly ElasticSearchAdapterFixture _classFixture;
		private ElasticClient _elasticClient;

		// These tests need ElasticSearch installed locally, you can do this
		// by running the ElasticSearch Docker image:
		//
		// docker run -d -p 9200:9200 -p 9300:9300 -e "discovery.type=single-node" docker.elastic.co/elasticsearch/elasticsearch:6.2.4
		//
		// ~~~~~~~~~~~~~
		// More details:
		//
		// https://www.docker.elastic.co/
		// https://www.elastic.co/guide/en/elasticsearch/reference/6.2/docker.html
		// https://github.com/elastic/elasticsearch-net
		//
		// RESTful api examples:
		// https://github.com/sittinash/elasticsearch-postman
		// http://{{url}}:{{port}}/pages/_search?pretty=true&q=*:*
		// http://localhost:9200/pages/_search?pretty=true&q=*:*
		// GET /_cat/indices?v

		public ElasticSearchAdapterTests(ITestOutputHelper console, ElasticSearchAdapterFixture classFixture)
		{
			_fixture = new Fixture();
			_console = console;
			_classFixture = classFixture;

			var node = new Uri("http://localhost:9200");
			var connectionSettings = new ConnectionSettings(node);
			_elasticClient = new ElasticClient(connectionSettings);
			Thread.Sleep(500);
		}

		private ElasticSearchAdapter CreateAdapter()
		{
			return new ElasticSearchAdapter(_elasticClient);
		}

		[Fact]
		public async Task AddPage()
		{
			// given
			ElasticSearchAdapter adapter = CreateAdapter();
			string title = "A long example title";
			var page = new SearchablePage() { Id = int.MaxValue, Title = title };
			await adapter.Add(page);

			// when
			var results = await adapter.Find("A long");

			// then
			var firstResult = results.FirstOrDefault();
			firstResult.ShouldNotBeNull();
			firstResult.Title.ShouldBe(title);
			firstResult.Id.ShouldBe(page.Id);
		}

		[Fact]
		public async Task Find()
		{
			// given
			ElasticSearchAdapter adapter = CreateAdapter();
			var page = _classFixture.TestPages.First();
			Thread.Sleep(1000);

			// when
			IEnumerable<SearchablePage> results = await adapter.Find(page.Title);

			// then
			var firstResult = results.FirstOrDefault();
			firstResult.ShouldNotBeNull();
			firstResult.Id.ShouldBe(page.Id);
			firstResult.Text.ShouldBe(page.Text);
			firstResult.Title.ShouldBe(page.Title);
			firstResult.Author.ShouldBe(page.Author);
			firstResult.DateTime.ShouldBe(page.DateTime);
		}
	}

	public class ElasticSearchAdapterFixture
	{
		public ElasticClient ElasticClient { get; set; }
		public List<SearchablePage> TestPages { get; set; }

		public ElasticSearchAdapterFixture()
		{
			var node = new Uri("http://localhost:9200");
			var connectionSettings = new ConnectionSettings(node);
			ElasticClient = new ElasticClient(connectionSettings);

			AddDummyData(ElasticClient);
		}

		private void AddDummyData(ElasticClient elasticClient)
		{
			var fixture = new Fixture();
			var adapter = new ElasticSearchAdapter(elasticClient);
			adapter.DeleteAll().GetAwaiter().GetResult();

			TestPages = fixture.CreateMany<SearchablePage>(10).ToList();
			foreach (SearchablePage page in TestPages)
			{
				adapter.Add(page).GetAwaiter().GetResult();
			}
		}
	}
}