using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Index.Memory;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using Roadkill.Core.Configuration;
using Roadkill.Core.Database;
using Roadkill.Core.Database.Repositories;
using Roadkill.Core.Mvc.ViewModels;
using Roadkill.Core.Text.Menu;
using Roadkill.Core.Text.TextMiddleware;

namespace Roadkill.Core.Services
{
	// TODO: NETStandard - make this singleton in IoC
	/// <summary>
	/// Provides searching tasks using a Lucene.net search index.
	/// </summary>
	public class SearchService : ISearchService
	{
		private readonly TextMiddlewareBuilder _textMiddlewareBuilder;
		private static readonly Regex _removeTagsRegex = new Regex("<(.|\n)*?>");
		protected virtual string IndexPath { get; set; }
		private static readonly LuceneVersion LUCENEVERSION = LuceneVersion.LUCENE_48;

		public ApplicationSettings ApplicationSettings { get; set; }
		public ISettingsRepository SettingsRepository { get; set; }
		public IPageRepository PageRepository { get; set; }

		public SearchService(ApplicationSettings settings, ISettingsRepository settingsRepository,
			IPageRepository pageRepository, TextMiddlewareBuilder textMiddlewareBuilder)
		{
			_textMiddlewareBuilder = textMiddlewareBuilder;
			if (settings == null)
				throw new ArgumentNullException(nameof(settings));

			if (settingsRepository == null)
				throw new ArgumentNullException(nameof(settingsRepository));

			if (pageRepository == null)
				throw new ArgumentNullException(nameof(pageRepository));

			IndexPath = settings.SearchIndexPath;

			ApplicationSettings = settings;
			SettingsRepository = settingsRepository;
			PageRepository = pageRepository;
		}

		/// <summary>
		/// Searches the lucene index with the search text.
		/// </summary>
		/// <param name="searchText">The text to search with.</param>
		/// <remarks>Syntax reference: http://lucene.apache.org/java/2_3_2/queryparsersyntax.html#Wildcard</remarks>
		/// <exception cref="SearchException">An error occurred searching the lucene.net index.</exception>
		public virtual IEnumerable<SearchResultViewModel> Search(string searchText)
		{
			List<SearchResultViewModel> list = new List<SearchResultViewModel>();

			if (string.IsNullOrWhiteSpace(searchText))
				return list;

			StandardAnalyzer analyzer = new StandardAnalyzer(LUCENEVERSION);
			MultiFieldQueryParser parser = new MultiFieldQueryParser(LUCENEVERSION, new string[] { "content", "title" }, analyzer);

			Query query = null;
			try
			{
				query = parser.Parse(searchText);
			}
			catch (ParseException)
			{
				// Catch syntax errors in the search and remove them.
				searchText = QueryParserBase.Escape(searchText);
				query = parser.Parse(searchText);
			}

			if (query != null)
			{
				try
				{
					var directory = GetDirectory();
					var reader = DirectoryReader.Open(directory);
					IndexSearcher searcher = new IndexSearcher(reader);
					TopDocs topDocs = searcher.Search(query, 1000);

					foreach (ScoreDoc scoreDoc in topDocs.ScoreDocs)
					{
						Document document = searcher.Doc(scoreDoc.Doc);
						list.Add(new SearchResultViewModel(document, scoreDoc));
					}
				}
				catch (Exception ex)
				{
					throw new SearchException(ex, "An error occurred while searching the index, try rebuilding the search index via the admin tools to fix this.");
				}
			}

			return list;
		}

		private Lucene.Net.Store.Directory GetDirectory()
		{
			return new RAMDirectory();
		}

		/// <summary>
		/// Adds the specified page to the search index.
		/// </summary>
		/// <param name="model">The page to add.</param>
		/// <exception cref="SearchException">An error occurred with the lucene.net IndexWriter while adding the page to the index.</exception>
		public virtual void Add(PageViewModel model)
		{
			try
			{
				StandardAnalyzer analyzer = new StandardAnalyzer(LUCENEVERSION);
				var config = new IndexWriterConfig(LUCENEVERSION, analyzer);
				var directory = GetDirectory();

				using (IndexWriter writer = new IndexWriter(directory, config))
				{
					Document document = new Document();
					document.Add(new StringField("id", model.Id.ToString(), Field.Store.YES));
					document.Add(new StringField("content", model.Content, Field.Store.YES));
					document.Add(new StringField("contentsummary", GetContentSummary(model), Field.Store.YES));
					document.Add(new StringField("title", model.Title, Field.Store.YES));
					document.Add(new StringField("tags", model.SpaceDelimitedTags(), Field.Store.YES));
					document.Add(new StringField("createdby", model.CreatedBy, Field.Store.YES));
					document.Add(new StringField("createdon", model.CreatedOn.ToString("d"), Field.Store.YES));
					document.Add(new StringField("contentlength", model.Content.Length.ToString(), Field.Store.YES));

					writer.AddDocument(document);
				}
			}
			catch (Exception ex)
			{
				if (!ApplicationSettings.IgnoreSearchIndexErrors)
					throw new SearchException(ex, "An error occurred while adding page '{0}' to the search index", model.Title);
			}
		}

		/// <summary>
		/// Deletes the specified page from the search indexs.
		/// </summary>
		/// <param name="model">The page to remove.</param>
		/// <exception cref="SearchException">An error occurred with the lucene.net IndexReader while deleting the page from the index.</exception>
		public virtual void Delete(PageViewModel model)
		{
			try
			{
				StandardAnalyzer analyzer = new StandardAnalyzer(LUCENEVERSION);
				var config = new IndexWriterConfig(LUCENEVERSION, analyzer);
				var directory = GetDirectory();

				using (IndexWriter writer = new IndexWriter(directory, config))
				{
					writer.DeleteDocuments(new Term("id", model.Id.ToString()));
				}
			}
			catch (Exception ex)
			{
				if (!ApplicationSettings.IgnoreSearchIndexErrors)
					throw new SearchException(ex, "An error occurred while deleting page '{0}' from the search index", model.Title);
			}
		}

		/// <summary>
		/// Updates the <see cref="Page"/> in the search index, by removing it and re-adding it.
		/// </summary>
		/// <param name="model">The page to update</param>
		/// <exception cref="SearchException">An error occurred with lucene.net while deleting the page or inserting it back into the index.</exception>
		public virtual void Update(PageViewModel model)
		{
			Delete(model);
			Add(model);
		}

		/// <summary>
		/// Creates the initial search index based on all pages in the system.
		/// </summary>
		/// <exception cref="SearchException">An error occurred with the lucene.net IndexWriter while adding the page to the index.</exception>
		public virtual void CreateIndex()
		{
			try
			{
				StandardAnalyzer analyzer = new StandardAnalyzer(LUCENEVERSION);
				var config = new IndexWriterConfig(LUCENEVERSION, analyzer);
				var directory = GetDirectory();

				using (IndexWriter writer = new IndexWriter(directory, config))
				{
					foreach (Page page in PageRepository.AllPages().ToList())
					{
						PageContent pageContent = PageRepository.GetLatestPageContent(page.Id);
						PageHtml html = _textMiddlewareBuilder.Execute(pageContent.Text);

						PageViewModel model = new PageViewModel(pageContent, html);
						writer.DeleteDocuments(new Term("id", model.Id.ToString()));

						Document document = new Document();
						document.Add(new StringField("id", model.Id.ToString(), Field.Store.YES));
						document.Add(new StringField("content", model.Content, Field.Store.YES));
						document.Add(new StringField("contentsummary", GetContentSummary(model), Field.Store.YES));
						document.Add(new StringField("title", model.Title, Field.Store.YES));
						document.Add(new StringField("tags", model.SpaceDelimitedTags(), Field.Store.YES));
						document.Add(new StringField("createdby", model.CreatedBy, Field.Store.YES));
						document.Add(new StringField("createdon", model.CreatedOn.ToString("d"), Field.Store.YES));
						document.Add(new StringField("contentlength", model.Content.Length.ToString(), Field.Store.YES));

						writer.AddDocument(document);
					}
				}
			}
			catch (Exception ex)
			{
				throw new SearchException(ex, "An error occurred while creating the search index");
			}
		}

		/// <summary>
		/// Converts the page summary to a lucene Document with the relevant searchable fields.
		/// </summary>
		internal string GetContentSummary(PageViewModel model)
		{
			// Turn the contents into HTML, then strip the tags for the mini summary. This needs some works
			string modelHtml = model.Content;
			modelHtml = _textMiddlewareBuilder.Execute(modelHtml);
			modelHtml = _removeTagsRegex.Replace(modelHtml, "");

			if (modelHtml.Length > 150)
				modelHtml = modelHtml.Substring(0, 149);

			return modelHtml;
		}
	}
}