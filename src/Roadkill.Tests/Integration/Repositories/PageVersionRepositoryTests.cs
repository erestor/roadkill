using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Marten;
using Roadkill.Core.Models;
using Roadkill.Core.Repositories;
using Xunit;
using Xunit.Abstractions;

// ReSharper disable PossibleMultipleEnumeration

namespace Roadkill.Tests.Integration.Repositories
{
	public class PageVersionRepositoryTests
	{
		private readonly Fixture _fixture;

		public PageVersionRepositoryTests()
		{
			_fixture = new Fixture();
			IDocumentStore documentStore = DocumentStoreManager.GetMartenDocumentStore(typeof(PageVersionRepository));

			new PageRepository(documentStore).Wipe();
			new PageVersionRepository(documentStore).Wipe();
		}

		public PageVersionRepository CreateRepository()
		{
			IDocumentStore documentStore = DocumentStoreManager.GetMartenDocumentStore(typeof(PageVersionRepository));

			return new PageVersionRepository(documentStore);
		}

		private List<PageVersion> CreateTenPages(PageVersionRepository repository)
		{
			IDocumentStore documentStore = DocumentStoreManager.GetMartenDocumentStore(typeof(PageVersionRepository));
			var pageRepository = new PageRepository(documentStore);

			List<Page> pages = _fixture.CreateMany<Page>(10).ToList();

			var pageVersions = new List<PageVersion>();
			foreach (Page page in pages)
			{
				string text = _fixture.Create<string>();
				string author = _fixture.Create<string>();
				DateTime dateTime = DateTime.Today;

				Page newPage = pageRepository.AddNewPage(page).GetAwaiter().GetResult();
				PageVersion pageVersion = repository.AddNewVersion(newPage.Id, text, author, dateTime).GetAwaiter().GetResult();
				pageVersions.Add(pageVersion);
			}

			return pageVersions;
		}

		[Fact]
		public async void AddNewVersion()
		{
			// given
			PageVersionRepository repository = CreateRepository();
			List<PageVersion> pages = CreateTenPages(repository);
			PageVersion expectedPage = pages.Last();
			await repository.AddNewVersion(expectedPage.PageId, "v2 text", "brian");

			// when
			PageVersion thirdVersion = await repository.AddNewVersion(expectedPage.PageId, "v3 text", "author2");

			// then
			Assert.NotNull(thirdVersion);

			PageVersion savedVersions = await repository.GetById(thirdVersion.Id);
			Assert.NotNull(savedVersions);
			AssertExtensions.Equivalent(thirdVersion, savedVersions);
		}

		[Fact]
		public async void AllVersions()
		{
			// given
			PageVersionRepository repository = CreateRepository();
			List<PageVersion> pages = CreateTenPages(repository);

			// when
			IEnumerable<PageVersion> allVersions = await repository.AllVersions();

			// then
			Assert.Equal(pages.Count, allVersions.Count());
			Assert.NotEmpty(allVersions.Last().Text);
		}

		[Fact]
		public async void DeleteVersion()
		{
			// given
			PageVersionRepository repository = CreateRepository();
			List<PageVersion> pages = CreateTenPages(repository);

			var expectedPage = pages[0];
			var version2 = await repository.AddNewVersion(expectedPage.PageId, "v2", "author2");
			var version3 = await repository.AddNewVersion(expectedPage.PageId, "v3", "author2");

			// when
			await repository.DeleteVersion(version3.Id);

			// then
			var deletedVersion = await repository.GetById(version3.Id);
			Assert.Null(deletedVersion);

			var latestVersion = await repository.GetById(version2.Id);
			Assert.NotNull(latestVersion);
		}

		[Fact]
		public async Task FindPageVersionsByPageId_should_return_versions_sorted_by_date_desc()
		{
			// given
			PageVersionRepository repository = CreateRepository();
			List<PageVersion> pages = CreateTenPages(repository);

			var expectedPage = pages[0];
			var version2 = await repository.AddNewVersion(expectedPage.PageId, "v2", "author1", DateTime.Today.AddMinutes(10));
			var version3 = await repository.AddNewVersion(expectedPage.PageId, "v3", "author2", DateTime.Today.AddMinutes(20));
			var version4 = await repository.AddNewVersion(expectedPage.PageId, "v4", "author3", DateTime.Today.AddMinutes(30));

			// when
			IEnumerable<PageVersion> versions = await repository.FindPageVersionsByPageId(expectedPage.PageId);

			// then
			Assert.NotNull(versions);
			Assert.NotEmpty(versions);
			Assert.Equal(4, versions.Count());
			AssertExtensions.Equivalent(version4, versions.First());
			AssertExtensions.Equivalent(expectedPage, versions.Last());
		}

		[Fact]
		public async Task FindPageVersionsByAuthor_should_be_case_insensitive_and_return_versions_sorted_by_date_desc()
		{
			// given
			PageVersionRepository repository = CreateRepository();
			List<PageVersion> pageVersions = CreateTenPages(repository);

			string editedBy = "shakespeare jr";
			PageVersion version2 = await repository.AddNewVersion(pageVersions[0].PageId, "v2 text", editedBy);
			PageVersion version3 = await repository.AddNewVersion(pageVersions[1].PageId, "v3 text", editedBy);

			// when
			IEnumerable<PageVersion> actualPageVersions = await repository.FindPageVersionsByAuthor("SHAKESPEARE jr");

			// then
			Assert.Equal(2, actualPageVersions.Count());
			Assert.Contains(actualPageVersions, p => p.Id == version2.Id);
			Assert.Contains(actualPageVersions, p => p.Id == version3.Id);
		}

		[Fact]
		public async Task GetLatestVersions()
		{
			// given
			PageVersionRepository repository = CreateRepository();
			List<PageVersion> pageVersions = CreateTenPages(repository);

			int pageId = pageVersions[0].PageId;
			PageVersion version2 = await repository.AddNewVersion(pageId, "v2 text", "editedBy", DateTime.Today.AddMinutes(10));
			PageVersion version3 = await repository.AddNewVersion(pageId, "v3 text", "editedBy", DateTime.Today.AddMinutes(30));

			// when
			PageVersion latestVersion = await repository.GetLatestVersion(pageId);

			// then
			Assert.NotNull(latestVersion);
			AssertExtensions.Equivalent(version3, latestVersion);
		}

		[Fact]
		public async Task GetById()
		{
			// given
			PageVersionRepository repository = CreateRepository();
			List<PageVersion> pageVersions = CreateTenPages(repository);
			PageVersion pageVersion = pageVersions[0];

			// when
			PageVersion latestVersion = await repository.GetById(pageVersion.Id);

			// then
			Assert.NotNull(latestVersion);
			AssertExtensions.Equivalent(pageVersion, latestVersion);
		}

		[Fact]
		public async Task UpdateExistingVersion()
		{
			// given
			PageVersionRepository repository = CreateRepository();
			List<PageVersion> pageVersions = CreateTenPages(repository);

			PageVersion newVersion = pageVersions[0];
			newVersion.Text = "some new text";
			newVersion.Author = "blake";

			// when
			await repository.UpdateExistingVersion(newVersion);

			// then
			PageVersion savedVersion = await repository.GetById(newVersion.Id);
			Assert.NotNull(savedVersion);
			AssertExtensions.Equivalent(newVersion, savedVersion);
		}
	}
}