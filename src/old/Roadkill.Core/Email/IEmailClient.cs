using System.Net.Mail;

namespace Roadkill.Core.Email
{
	/// <summary>
	/// 
	/// </summary>
	public interface IEmailClient
	{
		string PickupDirectoryLocation { get; set; }
		void Send(MailMessage message);
		SmtpDeliveryMethod GetDeliveryMethod();
	}
}
