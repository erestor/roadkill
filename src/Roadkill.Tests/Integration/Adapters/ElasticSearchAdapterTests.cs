using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
		}

		private ElasticSearchAdapter CreateAdapter()
		{
			return new ElasticSearchAdapter(_elasticClient);
		}

		[Fact]
		public async Task Add()
		{
			// given
			ElasticSearchAdapter adapter = CreateAdapter();
			string title = "A long example title";
			var page = new SearchablePage() { Id = int.MaxValue, Title = title };

			// when
			await adapter.Add(page);
			Thread.Sleep(1000);

			// then
			var results = await adapter.Find("A long");
			var firstResult = results.FirstOrDefault();
			firstResult.ShouldNotBeNull();
			firstResult.Title.ShouldBe(title);
			firstResult.Id.ShouldBe(page.Id);
		}

		[Theory]
		[InlineData("Id", "id : {0}")]
		[InlineData("Title", "title: {0}")]
		[InlineData("Text", "text: {0}")]
		[InlineData("Author", "author: {0}")]
		[InlineData("Tags", "tags: {0}")]
		public async Task Find(string property, string query)
		{
			// given
			ElasticSearchAdapter adapter = CreateAdapter();
			var page = _classFixture.TestPages.First();

			var val = typeof(SearchablePage).GetProperty(property).GetValue(page, null);
			query = string.Format(query, val);

			// when
			IEnumerable<SearchablePage> results = await adapter.Find(query);

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
}