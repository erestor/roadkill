using MailKit.Net.Smtp;
using MimeKit;

namespace Roadkill.Core.Email
{
	public class EmailClient : IEmailClient
	{
		public string PickupDirectoryLocation { get; set; }

		public EmailClient()
		{
			// Default it to the SmtpClient's settings, which are read from a .config
			//PickupDirectoryLocation = _smtpClient.PickupDirectoryLocation;
		}

		public void Send(IMailMessage message)
		{
			using (var smtpClient = new SmtpClient())
			{
				// TODO: NETStandard - get auth settings from config, add pickup directory
				//_smtpClient.PickupDirectoryLocation = PickupDirectoryLocation;
				var mimeMessage = new MimeMessage();
				mimeMessage.To.Add(new MailboxAddress(message.To));

				// TODO: Get the from address from config
				mimeMessage.From.Add(new MailboxAddress(message.From));
				mimeMessage.Subject = message.Subject;

				var bodyBuilder = new BodyBuilder();
				bodyBuilder.HtmlBody = message.HtmlBody;
				bodyBuilder.TextBody = message.PlainTextBody;
				mimeMessage.Body = bodyBuilder.ToMessageBody();

				smtpClient.Send(mimeMessage);
			}

		}

		public SmtpDeliveryMethod GetDeliveryMethod()
		{
			// TODO: NETStandard
			return SmtpDeliveryMethod.Default;
		}
	}
}
