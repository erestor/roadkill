using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Marten;
using Npgsql;
using Roadkill.Core.Models;
using Roadkill.Core.Repositories;
using Xunit;
using Xunit.Abstractions;

// ReSharper disable PossibleMultipleEnumeration
[assembly: CollectionBehavior(DisableTestParallelization = false)]

namespace Roadkill.Tests.Integration.Repositories
{
	public class PageRepositoryTests
	{
		private readonly ITestOutputHelper _output;
		private readonly Fixture _fixture;

		public PageRepositoryTests(ITestOutputHelper outputHelper)
		{
			_output = outputHelper;
			_fixture = new Fixture();
			IDocumentStore documentStore = DocumentStoreManager.GetMartenDocumentStore(typeof(PageRepositoryTests));

			new PageRepository(documentStore).Wipe();
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
			IDocumentStore documentStore = DocumentStoreManager.GetMartenDocumentStore(typeof(PageRepositoryTests));

			return new PageRepository(documentStore);
		}

		private List<Page> CreateTenPages(PageRepository repository, List<Page> pages = null)
		{
			if (pages == null)
				pages = _fixture.CreateMany<Page>(10).ToList();

			var newPages = new List<Page>();
			foreach (Page page in pages)
			{
				Page newPage = repository.AddNewPage(page).GetAwaiter().GetResult();
				newPages.Add(newPage);
			}

			return newPages;
		}

		private void Sleep500ms()
		{
			// This wait is necessary for slower Postgres instances, e.g Postgres running on Docker.
			//Thread.Sleep(500);
		}

		[Fact]
		public async void AddNewPage_should_add_page_and_increment_id()
		{
			// given
			string createdBy = "lyon";
			DateTime createdOn = DateTime.Today;

			PageRepository repository = CreateRepository();

			Page page = _fixture.Create<Page>();
			page.Id = -1; // should be reset
			page.CreatedBy = createdBy;
			page.CreatedOn = createdOn;
			page.LastModifiedBy = createdBy;
			page.LastModifiedOn = createdOn;

			// when
			await repository.AddNewPage(page);
			Page actualPage = await repository.AddNewPage(page);

			// then
			Assert.NotNull(actualPage);

			Page savedVersion = await repository.GetPageById(actualPage.Id);
			Assert.NotNull(savedVersion);
			Assert.InRange(actualPage.Id, 1, int.MaxValue);
			Assert.Equal(actualPage.CreatedBy, createdBy);
			Assert.Equal(actualPage.CreatedOn, createdOn);
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
		public async void DeletePage_should_delete_specific_page()
		{
			// given
			PageRepository repository = CreateRepository();
			CreateTenPages(repository);
			Sleep500ms();

			var pageToDelete = _fixture.Create<Page>();
			await repository.AddNewPage(pageToDelete);

			// when
			await repository.DeletePage(pageToDelete);

			// then
			var deletedPage = await repository.GetPageById(pageToDelete.Id);
			Assert.Null(deletedPage);
		}

		[Fact]
		public async Task DeleteAllPages_should_clear_pages()
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
		}

		[Fact]
		public async Task FindPagesCreatedBy_should_find_pages_created_by_with_case_insensitive_search()
		{
			// given
			PageRepository repository = CreateRepository();
			CreateTenPages(repository); // add random data

			var page1 = _fixture.Create<Page>();
			var page2 = _fixture.Create<Page>();
			page1.CreatedBy = "myself";
			page2.CreatedBy = "MYSELf";

			await repository.AddNewPage(page1);
			await repository.AddNewPage(page2);

			Sleep500ms();

			// when
			IEnumerable<Page> actualPages = await repository.FindPagesCreatedBy("myself");

			// then
			Assert.Equal(2, actualPages.Count());
			Assert.NotNull(actualPages.First(x => x.Id == page1.Id));
			Assert.NotNull(actualPages.First(x => x.Id == page2.Id));
		}

		[Fact]
		public async Task FindPagesLastModifiedBy_should_find_pages_with_case_insensitive_search()
		{
			// given
			PageRepository repository = CreateRepository();
			CreateTenPages(repository); // add random pages

			var page1 = _fixture.Create<Page>();
			var page2 = _fixture.Create<Page>();
			page1.LastModifiedBy = "THAT guy";
			page2.LastModifiedBy = "That Guy";

			await repository.AddNewPage(page1);
			await repository.AddNewPage(page2);
			Sleep500ms();

			// when
			IEnumerable<Page> actualPages = await repository.FindPagesLastModifiedBy("that guy");

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
			await repository.AddNewPage(pages[0]);
			await repository.AddNewPage(pages[1]);
			await repository.AddNewPage(pages[2]);

			// when
			var actualPages = await repository.FindPagesContainingTag("facebook-data-leak");

			// then
			Assert.Equal(3, actualPages.Count());
			Assert.NotNull(actualPages.First(x => x.Id == pages[0].Id));
			Assert.NotNull(actualPages.First(x => x.Id == pages[1].Id));
			Assert.NotNull(actualPages.First(x => x.Id == pages[2].Id));
		}

		[Fact]
		public async Task GetPageById_should_find_by_id()
		{
			// given
			PageRepository repository = CreateRepository();
			List<Page> pages = CreateTenPages(repository);
			Sleep500ms();

			Page expectedPage = pages[0];

			// when
			Page actualPage = await repository.GetPageById(expectedPage.Id);

			// then
			Assert.NotNull(actualPage);
			AssertExtensions.Equivalent(expectedPage, actualPage);
		}

		[Fact]
		public async Task GetPageByTitle_should_match_by_exact_title()
		{
			// given
			PageRepository repository = CreateRepository();
			List<Page> pages = CreateTenPages(repository);
			Sleep500ms();

			Page expectedPage = pages[0];

			// when
			Page actualPage = await repository.GetPageByTitle(expectedPage.Title);

			// then
			Assert.NotNull(actualPage);
			AssertExtensions.Equivalent(expectedPage, actualPage);
		}

		[Fact]
		public async Task UpdateExisting_should_save_changes()
		{
			// given
			PageRepository repository = CreateRepository();
			List<Page> pages = CreateTenPages(repository);
			Sleep500ms();

			Page expectedPage = pages[0];
			expectedPage.Tags = "new-tags";
			expectedPage.Title = "new title";

			// when
			await repository.UpdateExisting(expectedPage);

			// then
			Page actualPage = await repository.GetPageById(expectedPage.Id);
			AssertExtensions.Equivalent(expectedPage, actualPage);
		}
	}
}