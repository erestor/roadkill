using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Npgsql;
using Roadkill.Core.Models;
using Roadkill.Core.Repositories;
using Xunit;
using Xunit.Abstractions;

namespace Roadkill.Tests.Integration.Repositories
{
	public class PageVersionRepositoryTests
	{
		private readonly ITestOutputHelper _output;
		private readonly Fixture _fixture;

		public PageVersionRepositoryTests(ITestOutputHelper outputHelper)
		{
			_output = outputHelper;
			_fixture = new Fixture();

			new PageRepository(DocumentStoreManager.MartenDocumentStore).Wipe();
			new PageVersionRepository(DocumentStoreManager.MartenDocumentStore).Wipe();
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

		public PageVersionRepository CreateRepository()
		{
			return new PageVersionRepository(DocumentStoreManager.MartenDocumentStore);
		}

		private List<Page> CreateTenPages(PageVersionRepository repository, List<Page> pages = null)
		{
			if (pages == null)
				pages = _fixture.CreateMany<Page>(10).ToList();

			var pageRepository = new PageRepository(DocumentStoreManager.MartenDocumentStore);
			pages.ForEach(async page =>
			{
				Page newPage = await pageRepository.AddNewPage(page);
				pages.First(x => x.Id == page.Id).Id = newPage.Id;
			});

			pages.ForEach(async page =>
			{
				await repository.AddNewVersion(page.Id, _fixture.Create<string>(), _fixture.Create<string>());
			});
			return pages;
		}

		private void Sleep500ms()
		{
			// This wait is necessary for slower Postgres instances, e.g Postgres running on Docker.
			Thread.Sleep(500);
		}

		[Fact]
		public async void AddNewPageContentVersion_should_save_lastmodified_to_page()
		{
			// given
			PageVersionRepository repository = CreateRepository();
			List<Page> pages = CreateTenPages(repository);
			Page expectedPage = pages.Last();
			await repository.AddNewVersion(expectedPage.Id, "v1 content", "brian");

			// when
			PageVersion secondVersion = await repository.AddNewVersion(expectedPage.Id, "v2 content", "author2");

			// then
			Assert.NotNull(secondVersion);

			PageVersion savedVersions = await repository.GetVersionById(secondVersion.Id);
			Assert.NotNull(savedVersions);
			AssertExtensions.Equivalent(secondVersion, savedVersions);
		}

		[Fact]
		public async void AllPageContents()
		{
			// given
			PageVersionRepository repository = CreateRepository();
			List<Page> pages = CreateTenPages(repository);
			Sleep500ms();

			// when
			IEnumerable<PageVersion> actualPageContents = await repository.AllVersions();

			// then
			Assert.Equal(pages.Count, actualPageContents.Count());
			Assert.NotEmpty(actualPageContents.Last().Text);
		}

		[Fact]
		public async void DeletePageContent_should_remove_specific_pagecontent_version()
		{
			// given
			PageVersionRepository repository = CreateRepository();
			List<Page> pages = CreateTenPages(repository);
			Sleep500ms();

			var expectedPage = pages[0];
			var version1PageContent = await repository.AddNewVersion(expectedPage.Id, "v1", "author2");
			var version2PageContent = await repository.AddNewVersion(expectedPage.Id, "v2", "author2");

			// when
			await repository.DeleteVersion(version2PageContent.Id);

			// then
			var deletedVersion = await repository.GetVersionById(version2PageContent.Id);
			Assert.Null(deletedVersion);

			var latestVersion = await repository.GetVersionById(version1PageContent.Id);
			Assert.NotNull(latestVersion);
		}

		[Fact]
		public async Task FindPageVersionByPageId_should_return_all_pageversions_for_a_page()
		{
			// given
			PageVersionRepository repository = CreateRepository();
			var pages = CreateTenPages(repository);
			Sleep500ms();

			Page expectedPage = pages[0];
			PageVersion latestPageVersion = await repository.AddNewVersion(expectedPage.Id, "v2 text", "author2");

			// when
			IEnumerable<PageVersion> versions = await repository.FindPageVersionsByPageId(expectedPage.Id);

			// then
			Assert.NotNull(versions);
			Assert.NotEmpty(versions);
			Assert.Equal(2, versions.Count());
			AssertExtensions.Equivalent(latestPageVersion, versions.Last());
		}

		[Fact]
		public async Task FindPageVersionAuthoredBy_should_find_using_case_insensitive_search()
		{
			// given
			string editedBy = "shakespeare jr";

			PageVersionRepository repository = CreateRepository();
			CreateTenPages(repository); // add random pages

			var page1 = _fixture.Create<Page>();
			var page2 = _fixture.Create<Page>();

			PageVersion pageContent1 = await repository.AddNewVersion(page1.Id, "v2 text", editedBy);
			PageVersion pageContent2 = await repository.AddNewVersion(page2.Id, "v2 text", editedBy);

			Sleep500ms();

			// when
			IEnumerable<PageVersion> actualPageContents = await repository.FindPageVersionsByAuthor(editedBy);

			// then
			Assert.Equal(2, actualPageContents.Count());
			Assert.Contains(actualPageContents, p => p.Id == pageContent1.Id || p.Id == pageContent2.Id);
		}

		[Fact]
		public async Task GetLatestPageContent()
		{
			throw new NotImplementedException();
		}

		[Fact]
		public async Task GetPageContentById()
		{
			throw new NotImplementedException();
		}

		[Fact]
		public async Task GetPageContentByPageIdAndVersionNumber()
		{
			throw new NotImplementedException();
		}

		[Fact]
		public Task UpdatePageContent()
		{
			//check it saves the page too

			throw new NotImplementedException();
		}
	}
}