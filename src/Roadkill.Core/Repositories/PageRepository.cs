using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Marten;
using Roadkill.Core.Models;

namespace Roadkill.Core.Repositories
{
	public interface IPageRepository
	{
		// AddNewPage and AddNewPageContentVersion should be altered so they don't return anything

		Task<PageContentVersion> AddNewPage(Page page, string text);

		Task<PageContentVersion> AddNewPageContentVersion(int pageId, string text, string author, DateTime? dateTime = null);

		// Returns a list of tags for all pages. Each item is a list of tags seperated by, e.g. { "tag1, tag2, tag3", "blah, blah2" }
		Task<IEnumerable<Page>> AllPages();

		Task<IEnumerable<PageContentVersion>> AllPageContents();

		// the raw tags for every page, still comma delimited.
		Task<IEnumerable<string>> AllTags();

		Task DeletePage(Page page);

		//Removes a single version of page contents by its id.
		Task DeletePageContent(PageContentVersion pageContentVersion);

		Task DeleteAllPages();

		Task<IEnumerable<Page>> FindPagesCreatedBy(string username);

		Task<IEnumerable<Page>> FindPagesModifiedBy(string username);

		Task<IEnumerable<Page>> FindPagesContainingTag(string tag);

		Task<IEnumerable<PageContentVersion>> FindPageVersionsByPageId(int pageId);

		Task<IEnumerable<PageContentVersion>> FindPageContentsEditedBy(string username);

		Task<PageContentVersion> GetLatestPageContent(int pageId);

		Task<Page> GetPageById(int id);

		// Case insensitive search by page title
		Task<Page> GetPageByTitle(string title);

		Task<PageContentVersion> GetPageContentById(Guid id);

		Task<PageContentVersion> GetPageContentByPageIdAndVersionNumber(int id, int versionNumber);

		Task<Page> SaveOrUpdatePage(Page page);

		// doesn't add a new version
		Task UpdatePageContent(PageContentVersion contentVersion);
	}

	public class PageRepository : IPageRepository
	{
		private readonly IDocumentStore _store;

		public PageRepository(IDocumentStore store)
		{
			if (store == null)
				throw new ArgumentNullException(nameof(store));

			_store = store;
		}

		public void Wipe()
		{
			try
			{
				_store.Advanced.Clean.DeleteDocumentsFor(typeof(Page));
				_store.Advanced.Clean.DeleteDocumentsFor(typeof(PageContentVersion));
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
		}

		public async Task<PageContentVersion> AddNewPage(Page page, string text)
		{
			using (IDocumentSession session = _store.LightweightSession())
			{
				session.Store(page);

				var pageContent = new PageContentVersion()
				{
					Id = Guid.NewGuid(),
					PageId = page.Id,
					Author = page.CreatedBy,
					DateTime = page.CreatedOn,
					Text = text
				};
				session.Store(pageContent);

				await session.SaveChangesAsync();
				return pageContent;
			}
		}

		public async Task<PageContentVersion> AddNewPageContentVersion(int pageId, string text, string author, DateTime? dateTime = null)
		{
			using (IDocumentSession session = _store.LightweightSession())
			{
				if (dateTime == null)
					dateTime = DateTime.UtcNow;

				var pageContent = new PageContentVersion()
				{
					Id = Guid.NewGuid(),
					Author = author,
					DateTime = dateTime.Value,
					PageId = pageId,
					Text = text
				};
				session.Store(pageContent);

				Page page = await GetPageById(pageId);
				if (page == null)
					throw new InvalidOperationException($"The page id {pageId} does not exist anymore to save content for.");
				page.LastModifiedBy = author;
				page.LastModifiedOn = dateTime.Value;
				session.Store(page);

				await session.SaveChangesAsync();
				return pageContent;
			}
		}

		public async Task<IEnumerable<Page>> AllPages()
		{
			using (IQuerySession session = _store.QuerySession())
			{
				return await session
					.Query<Page>()
					.ToListAsync();
			}
		}

		public async Task<IEnumerable<PageContentVersion>> AllPageContents()
		{
			using (IQuerySession session = _store.QuerySession())
			{
				return await session
					.Query<PageContentVersion>()
					.ToListAsync();
			}
		}

		public async Task<IEnumerable<string>> AllTags()
		{
			using (IQuerySession session = _store.QuerySession())
			{
				return await session
					.Query<Page>()
					.Select(x => x.Tags)
					.ToListAsync();
			}
		}

		public async Task DeletePage(Page page)
		{
			using (IDocumentSession session = _store.LightweightSession())
			{
				session.Delete<Page>(page.Id);
				session.DeleteWhere<PageContentVersion>(x => x.PageId == page.Id);
				await session.SaveChangesAsync();
			}
		}

		public async Task DeletePageContent(PageContentVersion pageContentVersion)
		{
			using (IDocumentSession session = _store.OpenSession())
			{
				session.Delete<PageContentVersion>(pageContentVersion.Id);
				session.DeleteWhere<PageContentVersion>(x => x.Id == pageContentVersion.Id);
				await session.SaveChangesAsync();
			}
		}

		public async Task DeleteAllPages()
		{
			using (IDocumentSession session = _store.LightweightSession())
			{
				session.DeleteWhere<Page>(x => true);
				session.DeleteWhere<PageContentVersion>(x => true);

				await session.SaveChangesAsync();
			}
		}

		public async Task<IEnumerable<Page>> FindPagesCreatedBy(string username)
		{
			using (IQuerySession session = _store.QuerySession())
			{
				return await session
					.Query<Page>()
					.Where(x => x.CreatedBy.Equals(username, StringComparison.CurrentCultureIgnoreCase))
					.ToListAsync();
			}
		}

		public async Task<IEnumerable<Page>> FindPagesModifiedBy(string username)
		{
			using (IQuerySession session = _store.QuerySession())
			{
				return await session
					.Query<Page>()
					.Where(x => x.LastModifiedBy.Equals(username, StringComparison.CurrentCultureIgnoreCase))
					.ToListAsync();
			}
		}

		public async Task<IEnumerable<Page>> FindPagesContainingTag(string tag)
		{
			using (IQuerySession session = _store.QuerySession())
			{
				return await session
					.Query<Page>()
					.Where(x => x.Tags.Contains(tag))
					.ToListAsync();
			}
		}

		public async Task<IEnumerable<PageContentVersion>> FindPageVersionsByPageId(int pageId)
		{
			using (IQuerySession session = _store.QuerySession())
			{
				return await session
					.Query<PageContentVersion>()
					.Where(x => x.PageId == pageId)
					.ToListAsync();
			}
		}

		public async Task<IEnumerable<PageContentVersion>> FindPageContentsEditedBy(string username)
		{
			using (IQuerySession session = _store.QuerySession())
			{
				return await session
					.Query<PageContentVersion>()
					.Where(x => x.Author.Equals(username, StringComparison.CurrentCultureIgnoreCase))
					.ToListAsync();
			}
		}

		public async Task<PageContentVersion> GetLatestPageContent(int pageId)
		{
			using (IQuerySession session = _store.QuerySession())
			{
				return await session
					.Query<PageContentVersion>()
					.OrderByDescending(x => x.DateTime)
					.FirstOrDefaultAsync(x => x.PageId == pageId);
			}
		}

		public async Task<Page> GetPageById(int id)
		{
			using (IQuerySession session = _store.QuerySession())
			{
				return await session
					.Query<Page>()
					.FirstOrDefaultAsync(x => x.Id == id);
			}
		}

		public Task<Page> GetPageByTitle(string title)
		{
			throw new NotImplementedException();
		}

		public async Task<PageContentVersion> GetPageContentById(Guid id)
		{
			using (IQuerySession session = _store.QuerySession())
			{
				return await session
					.Query<PageContentVersion>()
					.FirstOrDefaultAsync(x => x.Id == id);
			}
		}

		public Task<PageContentVersion> GetPageContentByPageIdAndVersionNumber(int id, int versionNumber)
		{
			throw new NotImplementedException();
		}

		public Task<Page> SaveOrUpdatePage(Page page)
		{
			throw new NotImplementedException();
		}

		// updates an existing page contentVersion only
		public async Task UpdatePageContent(PageContentVersion contentVersion)
		{
			using (IDocumentSession session = _store.LightweightSession())
			{
				session.Store(contentVersion);
				await session.SaveChangesAsync();
			}
		}
	}
}