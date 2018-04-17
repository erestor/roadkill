namespace Roadkill.Core.Email
{
	/// <summary>
	/// 
	/// </summary>
	public interface IEmailClient
	{
		string PickupDirectoryLocation { get; set; }
		void Send(IMailMessage message);
		SmtpDeliveryMethod GetDeliveryMethod();
	}
}
