using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Roadkill.Api.Models;
using Roadkill.Core.Models;

namespace Roadkill.Api.Interfaces
{
	public interface IUserService
	{
		Task<bool> ActivateUser(string activationKey);

		Task<bool> AddUser(string email, string username, string password, bool isAdmin, bool isEditor);

		Task<bool> Authenticate(string email, string password);

		Task ChangePassword(string email, string newPassword);

		Task<bool> ChangePassword(string email, string oldPassword, string newPassword);

		Task<bool> DeleteUser(string email);

		Task<User> GetUserById(Guid id, bool? isActivated = null);

		Task<User> GetUser(string email, bool? isActivated = null);

		Task<User> GetUserByResetKey(string resetKey);

		Task<bool> IsAdmin(string cookieValue);

		Task<bool> IsEditor(string cookieValue);

		Task<IEnumerable<UserViewModel>> ListAdmins();

		Task<IEnumerable<UserViewModel>> ListEditors();

		Task Logout();

		Task<string> ResetPassword(string email);

		Task<string> Signup(UserViewModel model, Action completed);

		Task ToggleAdmin(string email);

		Task ToggleEditor(string email);

		Task<bool> UpdateUser(UserViewModel model);

		Task<bool> UserExists(string email);

		Task<bool> UserNameExists(string username);

		Task<string> GetLoggedInUserName();

		Task<User> GetLoggedInUser(string cookieValue);
	}
}