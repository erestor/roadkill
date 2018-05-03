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

		private void Wait500ms()
		{
			Thread.Sleep(500);
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
			Wait500ms();

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
			Wait500ms();

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
			Wait500ms();

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

		[Fact]
		public async void DeletePageContent_should_remove_specific_pagecontent_version()
		{
			// given
			PageRepository repository = CreateRepository();
			List<Page> pages = CreateTenPages(repository);
			var expectedPage = pages[0];
			var version2PageContent = await repository.AddNewPageContentVersion(expectedPage, _fixture.Create<string>(), "chris", DateTime.Now, 2);

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
			//PrintPages();
			Wait500ms();

			// when
			var task = repository.DeleteAllPages();
			Task.WaitAll(task);
			//PrintPages();
			Wait500ms();

			// then
			IEnumerable<Page> allPages = await repository.AllPages();
			Assert.Empty(allPages);

			var allPageContents = await repository.AllPageContents();
			Assert.Empty(allPageContents);
		}

		public Task<IEnumerable<Page>> FindPagesCreatedBy(string username)
		{
			throw new NotImplementedException();
		}

		//public Task<IEnumerable<Page>> FindPagesModifiedBy(string username)
		//{
		//    throw new NotImplementedException();
		//}

		// 4

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
		//    throw new NotImplementedException();
		//}

		public void Dispose()
		{
		}
	}
}