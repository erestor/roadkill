using Roadkill.Core.Models;

namespace Roadkill.Api.Models
{
	public interface IPageVersionViewModelConverter
	{
		PageVersionViewModel ConvertToViewModel(PageVersion pageVersion);

		PageVersion ConvertToPageVersion(PageVersionViewModel viewModel);
	}

	public class PageVersionViewModelConverter : IPageVersionViewModelConverter
	{
		public PageVersionViewModel ConvertToViewModel(PageVersion pageVersion)
		{
			return new PageVersionViewModel()
			{
				Id = pageVersion.Id,
				Text = pageVersion.Text,
				DateTime = pageVersion.DateTime,
				Author = pageVersion.Author,
				PageId = pageVersion.PageId
			};
		}

		public PageVersion ConvertToPageVersion(PageVersionViewModel viewModel)
		{
			return new PageVersion()
			{
				Id = viewModel.Id,
				Text = viewModel.Text,
				DateTime = viewModel.DateTime,
				Author = viewModel.Author,
				PageId = viewModel.PageId
			};
		}
	}
}