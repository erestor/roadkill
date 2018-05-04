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

		Task<PageContent> AddNewPage(Page page, string text);

		Task<PageContent> AddNewPageContentVersion(Page page, string text, int version);

		// Returns a list of tags for all pages. Each item is a list of tags seperated by, e.g. { "tag1, tag2, tag3", "blah, blah2" }
		Task<IEnumerable<Page>> AllPages();

		Task<IEnumerable<PageContent>> AllPageContents();

		// the raw tags for every page, still comma delimited.
		Task<IEnumerable<string>> AllTags();

		Task DeletePage(Page page);

		//Removes a single version of page contents by its id.
		Task DeletePageContent(PageContent pageContent);

		Task DeleteAllPages();

		Task<IEnumerable<Page>> FindPagesCreatedBy(string username);

		Task<IEnumerable<Page>> FindPagesModifiedBy(string username);

		Task<IEnumerable<Page>> FindPagesContainingTag(string tag);

		Task<IEnumerable<PageContent>> FindPageContentsByPageId(int pageId);

		Task<IEnumerable<PageContent>> FindPageContentsEditedBy(string username);

		Task<PageContent> GetLatestPageContent(int pageId);

		Task<Page> GetPageById(int id);

		// Case insensitive search by page title
		Task<Page> GetPageByTitle(string title);

		Task<PageContent> GetPageContentById(Guid id);

		Task<PageContent> GetPageContentByPageIdAndVersionNumber(int id, int versionNumber);

		Task<Page> SaveOrUpdatePage(Page page);

		// doesn't add a new version
		Task UpdatePageContent(PageContent content);
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
			_store.Advanced.Clean.DeleteDocumentsFor(typeof(Page));
			_store.Advanced.Clean.DeleteDocumentsFor(typeof(PageContent));
		}

		public async Task<PageContent> AddNewPage(Page page, string text)
		{
			using (IDocumentSession session = _store.LightweightSession())
			{
				session.Store(page);

				var pageContent = new PageContent()
				{
					Id = Guid.NewGuid(),
					EditedBy = page.CreatedBy,
					EditedOn = page.CreatedOn,
					Page = page,
					Text = text,
					VersionNumber = 1
				};
				session.Store(pageContent);

				await session.SaveChangesAsync();
				return pageContent;
			}
		}

		public async Task<PageContent> AddNewPageContentVersion(Page page, string text, int version)
		{
			PageContent existingVersion = await GetLatestPageContent(page.Id);
			if (existingVersion == null)
			{
				return await AddNewPage(page, text);
			}

			using (IDocumentSession session = _store.LightweightSession())
			{
				int newVersionNumber = existingVersion.VersionNumber + 1;

				var pageContent = new PageContent()
				{
					Id = Guid.NewGuid(),
					EditedBy = page.ModifiedBy,
					EditedOn = page.ModifiedOn,
					Page = page,
					Text = text,
					VersionNumber = newVersionNumber
				};
				session.Store(pageContent);

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

		public async Task<IEnumerable<PageContent>> AllPageContents()
		{
			using (IQuerySession session = _store.QuerySession())
			{
				return await session
					.Query<PageContent>()
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
				session.DeleteWhere<PageContent>(x => x.Page.Id == page.Id);
				await session.SaveChangesAsync();
			}
		}

		public async Task DeletePageContent(PageContent pageContent)
		{
			using (IDocumentSession session = _store.OpenSession())
			{
				session.Delete<PageContent>(pageContent.Id);
				session.DeleteWhere<PageContent>(x => x.Id == pageContent.Id);
				await session.SaveChangesAsync();
			}
		}

		public async Task DeleteAllPages()
		{
			using (IDocumentSession session = _store.LightweightSession())
			{
				session.DeleteWhere<Page>(x => true);
				session.DeleteWhere<PageContent>(x => true);

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
					.Where(x => x.ModifiedBy.Equals(username, StringComparison.CurrentCultureIgnoreCase))
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

		public async Task<IEnumerable<PageContent>> FindPageContentsByPageId(int pageId)
		{
			using (IQuerySession session = _store.QuerySession())
			{
				return await session
					.Query<PageContent>()
					.Where(x => x.Page.Id == pageId)
					.ToListAsync();
			}
		}

		public async Task<IEnumerable<PageContent>> FindPageContentsEditedBy(string username)
		{
			throw new NotImplementedException();
		}

		public async Task<PageContent> GetLatestPageContent(int pageId)
		{
			using (IQuerySession session = _store.QuerySession())
			{
				return await session
					.Query<PageContent>()
					.OrderByDescending(x => x.VersionNumber)
					.FirstOrDefaultAsync(x => x.Page.Id == pageId);
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

		public async Task<PageContent> GetPageContentById(Guid id)
		{
			using (IQuerySession session = _store.QuerySession())
			{
				return await session
					.Query<PageContent>()
					.FirstOrDefaultAsync(x => x.Id == id);
			}
		}

		public Task<PageContent> GetPageContentByPageIdAndVersionNumber(int id, int versionNumber)
		{
			throw new NotImplementedException();
		}

		public Task<Page> SaveOrUpdatePage(Page page)
		{
			throw new NotImplementedException();
		}

		public async Task UpdatePageContent(PageContent content)
		{
			PageContent latestPageContent = await GetLatestPageContent(content.Page.Id);

			using (IDocumentSession session = _store.LightweightSession())
			{
				content.Page.ModifiedBy = content.EditedBy;
				content.Page.ModifiedOn = DateTime.UtcNow;

				content.VersionNumber = latestPageContent.VersionNumber + 1;

				session.Store(content);
				session.Store(content.Page);
				await session.SaveChangesAsync();
			}
		}
	}
}