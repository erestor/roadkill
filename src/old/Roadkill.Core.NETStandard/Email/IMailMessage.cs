namespace Roadkill.Core.Email
{
	public interface IMailMessage
	{
		string To { get; set; }
		string From { get; set; }
		string Subject { get; set; }
		string HtmlBody { get; set; }
		string PlainTextBody { get; set; }
	}
}