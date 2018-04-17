using Roadkill.Core.Configuration;
using Roadkill.Core.Mvc.ViewModels;
using Roadkill.Core.Database.Repositories;

namespace Roadkill.Core.Email
{
	/// <summary>
	/// The template for signup emails.
	/// </summary>
	public class SignupEmail : EmailTemplate
	{
		private static string _htmlContent;
		private static string _plainTextContent;

		public SignupEmail(ApplicationSettings applicationSettings, ISettingsRepository settingsRepository, IEmailClient emailClient)
			: base(applicationSettings, settingsRepository, emailClient)
		{
		}

		public override void Send(UserViewModel model)
		{
			// Thread safety should not be an issue here
			if (string.IsNullOrEmpty(_plainTextContent))
				_plainTextContent = ReadTemplateFile("Signup.txt");

			if (string.IsNullOrEmpty(_htmlContent))
				_htmlContent = ReadTemplateFile("Signup.html");

			PlainTextBody = _plainTextContent;
			HtmlBody = _htmlContent;

			base.Send(model);
		}
	}
}
