using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Marten;
using Newtonsoft.Json;
using Roadkill.Core.Models;
using Roadkill.Core.Repositories;
using Xunit;

// ReSharper disable PossibleMultipleEnumeration

namespace Roadkill.Tests.Integration.Repositories
{
    public class PageRepositoryTests
    {
        private readonly Fixture _fixture;
        public static IDocumentStore MartenDocumentStore { get; }

        static PageRepositoryTests()
        {
            string connectionString = "host=localhost;port=5432;database=roadkill;username=roadkill;password=roadkill;";
            MartenDocumentStore = CreateDocumentStore(connectionString);
        }

        public PageRepositoryTests()
        {
            _fixture = new Fixture();
            new PageRepository(MartenDocumentStore).Wipe().GetAwaiter().GetResult();
        }

        public PageRepository CreateRepository()
        {
            return new PageRepository(MartenDocumentStore);
        }

        internal static IDocumentStore CreateDocumentStore(string connectionString)
        {
            var documentStore = DocumentStore.For(options =>
            {
                options.CreateDatabasesForTenants(c =>
                {
                    c.MaintenanceDatabase(connectionString);
                    c.ForTenant()
                        .CheckAgainstPgDatabase()
                        .WithOwner("roadkill")
                        .WithEncoding("UTF-8")
                        .ConnectionLimit(-1)
                        .OnDatabaseCreated(_ =>
                        {
                            Console.WriteLine("Postgres 'roadkill' database created");
                        });
                });

                options.Connection(connectionString);
                options.Schema.For<User>().Index(x => x.Id);
                options.Schema.For<Page>().Index(x => x.Id);
            });

            return documentStore;
        }

        private void AssertEquivalent(object expected, object actual)
        {
            string expectedJson = JsonConvert.SerializeObject(expected);
            string actualJson = JsonConvert.SerializeObject(actual);

            Assert.Equal(expectedJson, actualJson);
        }

        private List<Page> CreateTenPages(PageRepository repository, List<Page> pages = null)
        {
            if (pages == null)
                pages = _fixture.CreateMany<Page>(10).ToList();

            pages.ForEach(async page =>
            {
                PageContent content = await repository.AddNewPage(page, _fixture.Create<string>(), "edited by", DateTime.UtcNow);
            });
            return pages;
        }

        private void SleepForABit()
        {
            Thread.Sleep(500);// find a better way of doing this with Marten
        }

        [Fact]
        public async void AddNewPage()
        {
            // given
            PageRepository repository = CreateRepository();
            Page expectedPage = _fixture.Create<Page>();
            PageContent dummyContent = _fixture.Create<PageContent>();

            // when
            PageContent actualPageContent = await repository.AddNewPage(expectedPage, dummyContent.EditedBy, dummyContent.EditedBy, dummyContent.EditedOn);

            // then
            Assert.NotNull(actualPageContent);

            PageContent savedContent = await repository.GetPageContentById(actualPageContent.Id);
            Assert.NotNull(savedContent);
            AssertEquivalent(actualPageContent, savedContent);
        }

        [Fact]
        public async void AddNewPageContentVersion_should_increment_version_for_existing_page()
        {
            // given
            PageRepository repository = CreateRepository();
            Page expectedPage = _fixture.Create<Page>();
            PageContent dummyContent = _fixture.Create<PageContent>();

            PageContent firstVersion = await repository.AddNewPage(expectedPage, dummyContent.EditedBy, dummyContent.EditedBy, dummyContent.EditedOn);

            // when
            PageContent secondVersion = await repository.AddNewPageContentVersion(expectedPage, "new content", dummyContent.EditedBy, dummyContent.EditedOn, 2);

            // then
            Assert.NotNull(secondVersion);

            PageContent savedContent = await repository.GetPageContentById(secondVersion.Id);
            Assert.NotNull(savedContent);
            AssertEquivalent(secondVersion, savedContent);

            Assert.Equal(2, secondVersion.VersionNumber);
        }

        [Fact]
        public async void AddNewPageContentVersion_should_addnewpage_when_no_content_exists()
        {
            // given
            PageRepository repository = CreateRepository();
            Page expectedPage = _fixture.Create<Page>();
            PageContent dummyContent = _fixture.Create<PageContent>();

            // when
            PageContent actualPageContent = await repository.AddNewPageContentVersion(expectedPage, "new content", dummyContent.EditedBy, dummyContent.EditedOn, 2);

            // then
            Assert.NotNull(actualPageContent);

            PageContent savedContent = await repository.GetPageContentById(actualPageContent.Id);
            Assert.NotNull(savedContent);
            AssertEquivalent(actualPageContent, savedContent);

            Assert.Equal(1, actualPageContent.VersionNumber);
        }

        [Fact]
        public async void AllPages()
        {
            // given
            PageRepository repository = CreateRepository();
            List<Page> pages = CreateTenPages(repository);
            SleepForABit();

            // when
            var actualPages = await repository.AllPages();

            // then
            Assert.Equal(pages.Count, actualPages.Count());
        }

        [Fact]
        public async void AllPageContents()
        {
            // given
            PageRepository repository = CreateRepository();
            List<Page> pages = CreateTenPages(repository);
            SleepForABit();

            // when
            var actualPageContents = await repository.AllPageContents();

            // then
            Assert.Equal(pages.Count, actualPageContents.Count());
            Assert.NotEmpty(actualPageContents.Last().Text);
        }

        [Fact]
        public async void AllTags_should_return_raw_tags_for_all_pages()
        {
            // given
            PageRepository repository = CreateRepository();
            List<Page> pages = _fixture.CreateMany<Page>(10).ToList();
            pages.ForEach(p => p.Tags = "tag1, tag2, tag3");
            CreateTenPages(repository, pages);
            SleepForABit();

            // when
            IEnumerable<string> actualTags = await repository.AllTags();

            // then
            Assert.Equal(pages.Count, actualTags.Count());
            Assert.Equal("tag1, tag2, tag3", actualTags.First());
        }

        [Fact]
        public async void DeletePage_should_delete_page_and_contents()
        {
            // given
            PageRepository repository = CreateRepository();
            CreateTenPages(repository);

            var pageToDelete = _fixture.Create<Page>();
            await repository.AddNewPage(pageToDelete, _fixture.Create<string>(), "edited by", DateTime.UtcNow);

            // when
            await repository.DeletePage(pageToDelete);

            // then
            var deletedPage = await repository.GetPageById(pageToDelete.Id);
            Assert.Null(deletedPage);

            var pageContents = await repository.FindPageContentsByPageId(pageToDelete.Id);
            Assert.Empty(pageContents);
        }

        public async void DeletePageContent(PageContent pageContent)
        {
            throw new NotImplementedException();
        }

        //public Task DeleteAllPages()
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<IEnumerable<Page>> FindPagesCreatedBy(string username)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<IEnumerable<Page>> FindPagesModifiedBy(string username)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<IEnumerable<Page>> FindPagesContainingTag(string tag)
        //{
        //    throw new NotImplementedException();
        //}

        //public IEnumerable<PageContent> FindPageContentsByPageId(int pageId)
        //{
        //    throw new NotImplementedException();
        //}

        //public IEnumerable<PageContent> FindPageContentsEditedBy(string username)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<PageContent> GetLatestPageContent(int pageId)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<Page> GetPageById(int id)
        //
        //    throw new NotImplementedException();
        //}

        //public Task<Page> GetPageByTitle(string title)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<PageContent> GetPageContentById(Guid id)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<PageContent> GetPageContentByPageIdAndVersionNumber(int id, int versionNumber)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<Page> SaveOrUpdatePage(Page page)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task UpdatePageContent(PageContent content)
        //{
        //    throw new NotImplementedException();
        //}
    }
}