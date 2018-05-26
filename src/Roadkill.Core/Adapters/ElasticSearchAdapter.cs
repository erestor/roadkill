using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nest;
using Page = Roadkill.Core.Models.Page;

namespace Roadkill.Core.Adapters
{
	public class ElasticSearchAdapter
	{
		private readonly IElasticClient _elasticClient;

		public ElasticSearchAdapter(IElasticClient elasticClient)
		{
			_elasticClient = elasticClient;
			var response = _elasticClient.CreateIndexAsync("pages");
		}

		public async Task<IEnumerable<Page>> Find(string title)
		{
			SearchDescriptor<Page> searchDescriptor = CreateSearchDescriptor(title);
			ISearchResponse<Page> response = await _elasticClient.SearchAsync<Page>(searchDescriptor);

			return response.Documents.AsEnumerable();
		}

		private static SearchDescriptor<Page> CreateSearchDescriptor(string title)
		{
			return new SearchDescriptor<Page>()
						.From(0)
						.Size(10)
						.Index("pages")
						.Query(q => q.Match(m => m.Field(f => f.Title).Query(title)));
		}

		public async Task Add(Page page)
		{
			await _elasticClient.IndexAsync(page, idx => idx.Index("pages"));
		}
	}
}