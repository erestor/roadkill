using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Marten;
using Newtonsoft.Json;
using Npgsql;
using Roadkill.Core.Models;
using Roadkill.Core.Repositories;
using Xunit;
using Xunit.Abstractions;

// ReSharper disable PossibleMultipleEnumeration
[assembly: CollectionBehavior(DisableTestParallelization = false)]

namespace Roadkill.Tests.Integration.Repositories
{
	public class PageRepositoryTests : IDisposable
	{
		private readonly ITestOutputHelper _output;
		private readonly Fixture _fixture;

		public PageRepositoryTests(ITestOutputHelper outputHelper)
		{
			_output = outputHelper;
			_fixture = new Fixture();
			new PageRepository(DocumentStoreManager.MartenDocumentStore).Wipe();
		}

		private void PrintPages()
		{
			using (var connection = new NpgsqlConnection(DocumentStoreManager.ConnectionString))
			{
				connection.Open();
				var command = connection.CreateCommand();
				command.CommandText = "delete from public.mt_doc_page";
				//command.ExecuteNonQuery();

				command.CommandText = "delete from public.mt_doc_pagecontent";
				//command.ExecuteNonQuery();

				command.CommandText = "select count(*) from public.mt_doc_page";
				long result = (long)command.ExecuteScalar();
				_output.WriteLine("Pages: {0}", result);

				command.CommandText = "select count(*) from public.mt_doc_pagecontent";
				result = (long)command.ExecuteScalar();
				_output.WriteLine("PageContents: {0}", result);
			}
		}

		public PageRepository CreateRepository()
		{
			return new PageRepository(DocumentStoreManager.MartenDocumentStore);
		}

		private List<Page> CreateTenPages(PageRepository repository, List<Page> pages = null)
		{
			if (pages == null)
				pages = _fixture.CreateMany<Page>(10).ToList();

			pages.ForEach(async page =>
			{
				PageContentVersion contentVersion = await repository.AddNewPage(page, _fixture.Create<string>());
			});
			return pages;
		}

		private void Sleep500ms()
		{
			// This wait is necessary for slower Postgres instances, e.g Postgres running on Docker.
			Thread.Sleep(500);
		}

		[Fact]
		public async void AddNewPage_should_add_page_and_content()
		{
			// given
			string expectedText = _fixture.Create<string>();
			string author = "larcy";
			DateTime createdOn = DateTime.Today;

			PageRepository repository = CreateRepository();

			Page page = _fixture.Create<Page>();
			page.LastModifiedBy = page.CreatedBy = author;
			page.LastModifiedOn = page.CreatedOn = createdOn;

			// when
			PageContentVersion content = await repository.AddNewPage(page, expectedText);

			// then
			Assert.NotNull(content);

			PageContentVersion savedContentVersion = await repository.GetPageContentById(content.Id);
			Assert.NotNull(savedContentVersion);
			Assert.Equal(content.Author, author);
			Assert.Equal(content.DateTime, createdOn);
		}

		[Fact]
		public async void AddNewPageContentVersion_should_save_lastmodified_to_page()
		{
			// given
			PageRepository repository = CreateRepository();
			Page expectedPage = _fixture.Create<Page>();
			await repository.AddNewPage(expectedPage, "v1 content");

			// when
			PageContentVersion secondVersion = await repository.AddNewPageContentVersion(expectedPage.Id, "v2 content", "author2");

			// then
			Assert.NotNull(secondVersion);

			PageContentVersion savedContentVersion = await repository.GetPageContentById(secondVersion.Id);
			Assert.NotNull(savedContentVersion);
			AssertExtensions.Equivalent(secondVersion, savedContentVersion);

			Page latestPage = await repository.GetPageById(expectedPage.Id);
			Assert.Equal(savedContentVersion.Author, latestPage.LastModifiedBy);
			Assert.Equal(savedContentVersion.DateTime, latestPage.LastModifiedOn);
		}

		[Fact]
		public async void AllPages()
		{
			// given
			PageRepository repository = CreateRepository();
			List<Page> pages = CreateTenPages(repository);
			Sleep500ms();

			// when
			IEnumerable<Page> actualPages = await repository.AllPages();

			// then
			Assert.Equal(pages.Count, actualPages.Count());
		}

		[Fact]
		public async void AllPageContents()
		{
			// given
			PageRepository repository = CreateRepository();
			List<Page> pages = CreateTenPages(repository);
			Sleep500ms();

			// when
			IEnumerable<PageContentVersion> actualPageContents = await repository.AllPageContents();

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
			Sleep500ms();

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
			await repository.AddNewPage(pageToDelete, _fixture.Create<string>());

			// when
			await repository.DeletePage(pageToDelete);

			// then
			var deletedPage = await repository.GetPageById(pageToDelete.Id);
			Assert.Null(deletedPage);

			var pageContents = await repository.FindPageVersionsByPageId(pageToDelete.Id);
			Assert.Empty(pageContents);
		}

		[Fact]
		public async void DeletePageContent_should_remove_specific_pagecontent_version()
		{
			// given
			PageRepository repository = CreateRepository();
			List<Page> pages = CreateTenPages(repository);

			var expectedPage = pages[0];
			string text = _fixture.Create<string>();
			var version2PageContent = await repository.AddNewPageContentVersion(expectedPage.Id, text, "author2");

			// when
			await repository.DeletePageContent(version2PageContent);

			// then
			var deletedPage = await repository.GetPageContentById(version2PageContent.Id);
			Assert.Null(deletedPage);

			var pageContents = await repository.GetLatestPageContent(expectedPage.Id);
			Assert.NotNull(pageContents);
		}

		[Fact]
		public async Task DeleteAllPages_should_clear_pages_and_pagecontents()
		{
			// given
			PageRepository repository = CreateRepository();
			CreateTenPages(repository);
			Sleep500ms();

			// when
			await repository.DeleteAllPages();
			Sleep500ms();

			// then
			IEnumerable<Page> allPages = await repository.AllPages();
			Assert.Empty(allPages);

			var allPageContents = await repository.AllPageContents();
			Assert.Empty(allPageContents);
		}

		[Fact]
		public async Task FindPagesCreatedBy_should_find_pages_created_by__with_case_insensitive_search()
		{
			// given
			PageRepository repository = CreateRepository();
			CreateTenPages(repository); // add random data

			var page1 = _fixture.Create<Page>();
			var page2 = _fixture.Create<Page>();
			page1.CreatedBy = "myself";
			page2.CreatedBy = "MYSELf";

			await repository.AddNewPage(page1, "text");
			await repository.AddNewPage(page2, "text");

			Sleep500ms();

			// when
			IEnumerable<Page> actualPages = await repository.FindPagesCreatedBy("myself");

			// then
			Assert.Equal(2, actualPages.Count());
			Assert.NotNull(actualPages.First(x => x.Id == page1.Id));
			Assert.NotNull(actualPages.First(x => x.Id == page2.Id));
		}

		[Fact]
		public async Task FindPagesModifiedBy_should_find_pages_with_case_insensitive_search()
		{
			// given
			PageRepository repository = CreateRepository();
			CreateTenPages(repository); // add random pages

			var page1 = _fixture.Create<Page>();
			var page2 = _fixture.Create<Page>();
			await repository.AddNewPage(page1, "text");
			await repository.AddNewPage(page2, "text");
			await repository.AddNewPageContentVersion(page1.Id, "v2 text", "that guy");
			await repository.AddNewPageContentVersion(page2.Id, "v2 text", "THaT guy");

			Sleep500ms();

			// when
			IEnumerable<Page> actualPages = await repository.FindPagesModifiedBy("that guy");

			// then
			Assert.Equal(2, actualPages.Count());
			Assert.NotNull(actualPages.First(x => x.Id == page1.Id));
			Assert.NotNull(actualPages.First(x => x.Id == page2.Id));
		}

		[Fact]
		public async Task FindPagesContainingTag_should_find_tags_using_case_insensitive_search()
		{
			// given
			PageRepository repository = CreateRepository();
			CreateTenPages(repository);

			List<Page> pages = _fixture.CreateMany<Page>(3).ToList();
			pages.ForEach(p => p.Tags = _fixture.Create<string>() + ", facebook-data-leak");
			await repository.AddNewPage(pages[0], _fixture.Create<string>());
			await repository.AddNewPage(pages[1], _fixture.Create<string>());
			await repository.AddNewPage(pages[2], _fixture.Create<string>());

			// when
			var actualPages = await repository.FindPagesContainingTag("facebook-data-leak");

			// then
			Assert.Equal(3, actualPages.Count());
			Assert.NotNull(actualPages.First(x => x.Id == pages[0].Id));
			Assert.NotNull(actualPages.First(x => x.Id == pages[1].Id));
			Assert.NotNull(actualPages.First(x => x.Id == pages[2].Id));
		}

		[Fact]
		public async Task FindPageVersionsByPageId_should_return_all_pagecontents_for_a_page()
		{
			// given
			PageRepository repository = CreateRepository();
			var pages = CreateTenPages(repository);
			Sleep500ms();

			Page expectedPage = pages[0];
			PageContentVersion latestPageContentVersion = await repository.AddNewPageContentVersion(expectedPage.Id, "v2 text", "author2");

			// when
			IEnumerable<PageContentVersion> actualPageContents = await repository.FindPageVersionsByPageId(expectedPage.Id);

			// then
			Assert.NotNull(actualPageContents);
			Assert.NotEmpty(actualPageContents);
			Assert.Equal(2, actualPageContents.Count());
			AssertExtensions.Equivalent(latestPageContentVersion, actualPageContents.Last());
		}

		[Fact]
		public async Task FindPageContentsEditedBy_should_find_using_case_insensitive_search()
		{
			// given
			string editedBy = "shakespeare jr";

			PageRepository repository = CreateRepository();
			CreateTenPages(repository); // add random pages

			var page1 = _fixture.Create<Page>();
			var page2 = _fixture.Create<Page>();
			await repository.AddNewPage(page1, "text");
			await repository.AddNewPage(page2, "text");

			PageContentVersion pageContent1 = await repository.AddNewPageContentVersion(page1.Id, "v2 text", editedBy);
			PageContentVersion pageContent2 = await repository.AddNewPageContentVersion(page2.Id, "v2 text", editedBy);

			Sleep500ms();

			// when
			IEnumerable<PageContentVersion> actualPageContents = await repository.FindPageContentsEditedBy(editedBy);

			// then
			Assert.Equal(2, actualPageContents.Count());
			Assert.Contains(actualPageContents, p => p.Id == pageContent1.Id || p.Id == pageContent2.Id);
		}

		//public Task<PageContent> GetLatestPageContent(int pageId)
		//{
		//    throw new NotImplementedException();
		//}

		// 4

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

		// 4

		//public Task<Page> SaveOrUpdatePage(Page page)
		//{
		//    throw new NotImplementedException();
		//}

		//public Task UpdatePageContent(PageContent content)
		//{
		// check it saves the page too
		//    throw new NotImplementedException();
		//}

		public void Dispose()
		{
		}
	}
}