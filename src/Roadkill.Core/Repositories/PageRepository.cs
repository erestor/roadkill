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

        Task<PageContent> AddNewPage(Page page, string text, string editedBy, DateTime editedOn);

        Task<PageContent> AddNewPageContentVersion(Page page, string text, string editedBy, DateTime editedOn, int version);

        // Returns a list of tags for all pages. Each item is a list of tags seperated by, e.g. { "tag1, tag2, tag3", "blah, blah2" }
        Task<IEnumerable<Page>> AllPages();

        Task<IEnumerable<PageContent>> AllPageContents();

        // the raw tags for every page, still comma delimited.
        Task<IEnumerable<string>> AllTags();

        Task DeletePage(Page page);

        //Removes a single version of page contents by its id.
        Task<Task> DeletePageContent(PageContent pageContent);

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

        public async Task Wipe()
        {
            await DeleteAllPages();
        }

        public async Task<PageContent> AddNewPage(Page page, string text, string editedBy, DateTime editedOn)
        {
            using (IDocumentSession session = _store.LightweightSession())
            {
                session.Store(page);

                var pageContent = new PageContent()
                {
                    Id = Guid.NewGuid(),
                    EditedBy = editedBy,
                    EditedOn = editedOn,
                    Page = page,
                    Text = text,
                    VersionNumber = 1
                };
                session.Store(pageContent);

                await session.SaveChangesAsync();
                return pageContent;
            }
        }

        public async Task<PageContent> AddNewPageContentVersion(Page page, string text, string editedBy, DateTime editedOn, int version)
        {
            PageContent existingVersion = await GetLatestPageContent(page.Id);
            if (existingVersion == null)
            {
                return await AddNewPage(page, text, editedBy, editedOn);
            }

            using (IDocumentSession session = _store.LightweightSession())
            {
                int newVersionNumber = existingVersion.VersionNumber + 1;

                var pageContent = new PageContent()
                {
                    Id = Guid.NewGuid(),
                    EditedBy = editedBy,
                    EditedOn = editedOn,
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

        public Task<Task> DeletePageContent(PageContent pageContent)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteAllPages()
        {
            _store.Advanced.Clean.DeleteDocumentsFor(typeof(Page));
            _store.Advanced.Clean.DeleteDocumentsFor(typeof(PageContent));
            await Task.CompletedTask;
        }

        public Task<IEnumerable<Page>> FindPagesCreatedBy(string username)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Page>> FindPagesModifiedBy(string username)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Page>> FindPagesContainingTag(string tag)
        {
            throw new NotImplementedException();
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

        public Task UpdatePageContent(PageContent content)
        {
            throw new NotImplementedException();
        }
    }
}