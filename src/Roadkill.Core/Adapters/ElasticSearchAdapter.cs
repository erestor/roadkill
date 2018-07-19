using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nest;
using Roadkill.Core.Models;

namespace Roadkill.Core.Adapters
{
	public interface IElasticSearchAdapter
	{
		Task Add(SearchablePage page);

		Task Update(SearchablePage page);

		Task DeleteAll();

		Task<IEnumerable<SearchablePage>> Find(string query);
	}

	public class ElasticSearchAdapter : IElasticSearchAdapter
	{
		public const string PagesIndexName = "pages";
		private readonly IElasticClient _elasticClient;

		public ElasticSearchAdapter(IElasticClient elasticClient)
		{
			_elasticClient = elasticClient;
			EnsureIndexesExist();
		}

		private void EnsureIndexesExist()
		{
			if (!_elasticClient.IndexExists(PagesIndexName).Exists)
			{
				_elasticClient.CreateIndexAsync(PagesIndexName);
			}
		}

		public async Task Add(SearchablePage page)
		{
			await _elasticClient.IndexAsync(page, idx => idx.Index(PagesIndexName));
		}

		public async Task DeleteAll()
		{
			await _elasticClient.DeleteIndexAsync(PagesIndexName);
		}

		public Task Update(SearchablePage page)
		{
			throw new System.NotImplementedException();
		}

		public async Task<IEnumerable<SearchablePage>> Find(string query)
		{
			SearchDescriptor<SearchablePage> searchDescriptor = CreateSearchDescriptor(query);
			ISearchResponse<SearchablePage> response = await _elasticClient.SearchAsync<SearchablePage>(searchDescriptor);

			return response.Documents.AsEnumerable();
		}

		private static SearchDescriptor<SearchablePage> CreateSearchDescriptor(string query)
		{
			return new SearchDescriptor<SearchablePage>()
						.From(0)
						.Size(20)
						.Index(PagesIndexName)
						.Query(q => q.QueryString(qs => qs.Query(query)));
		}
	}
}