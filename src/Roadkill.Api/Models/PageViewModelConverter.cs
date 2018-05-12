using System.Collections.Generic;
using System.Text.RegularExpressions;
using Roadkill.Core.Models;

namespace Roadkill.Api.Models
{
	public interface IPageViewModelConverter
	{
		PageViewModel Create(Page page);
	}

	public class PageViewModelConverter : IPageViewModelConverter
	{
		public PageViewModel Create(Page page)
		{
			return new PageViewModel()
			{
				Id = page.Id,
				Title = page.Title,
				SeoFriendlyTitle = CreateSeoFriendlyPageTitle(page.Title),
				TagsAsCsv = page.Tags,
				TagList = ParseTags(page.Tags),
				LastModifiedBy = page.LastModifiedBy,
				LastModifiedOn = page.LastModifiedOn,
				CreatedBy = page.CreatedBy,
				CreatedOn = page.CreatedOn,
				IsLocked = page.IsLocked
			};
		}

		private IEnumerable<string> ParseTags(string csvTags)
		{
			List<string> tagList = new List<string>();
			char delimiter = ',';

			if (!string.IsNullOrEmpty(csvTags))
			{
				// For the legacy tag seperator format
				if (csvTags.IndexOf(";") != -1)
					delimiter = ';';

				if (csvTags.IndexOf(delimiter) != -1)
				{
					string[] parts = csvTags.Split(delimiter);
					foreach (string item in parts)
					{
						if (item != "," && !string.IsNullOrWhiteSpace(item))
							tagList.Add(item.Trim());
					}
				}
				else
				{
					tagList.Add(csvTags.TrimEnd());
				}
			}

			return tagList;
		}

		public static string CreateSeoFriendlyPageTitle(string title)
		{
			if (string.IsNullOrEmpty(title))
				return title;

			// Search engine friendly slug routine with help from http://www.intrepidstudios.com/blog/2009/2/10/function-to-generate-a-url-friendly-string.aspx

			// remove invalid characters
			title = Regex.Replace(title, @"[^\w\d\s-]", "");  // this is unicode safe, but may need to revert back to 'a-zA-Z0-9', need to check spec

			// convert multiple spaces/hyphens into one space
			title = Regex.Replace(title, @"[\s-]+", " ").Trim();

			// If it's over 30 chars, take the first 30.
			title = title.Substring(0, title.Length <= 75 ? title.Length : 75).Trim();

			// hyphenate spaces
			title = Regex.Replace(title, @"\s", "-");

			return title;
		}
	}
}