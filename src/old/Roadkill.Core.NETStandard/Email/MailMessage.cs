namespace Roadkill.Core.Email
{
	public class MailMessage : IMailMessage
	{
		public string To { get; set; }
		public string From { get; set; }
		public string Subject { get; set; }
		public string HtmlBody { get; set; }
		public string PlainTextBody { get; set; }
	}
}