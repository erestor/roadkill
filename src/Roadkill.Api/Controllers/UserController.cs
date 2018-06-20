using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Roadkill.Core.Models;

namespace Roadkill.Api.Controllers
{
	[Route("files")]
	public class UserController : Controller//, IUserService
	{
		private readonly AspNetUserManager<RoadkillUser> _userManager;

		public UserController(AspNetUserManager<RoadkillUser> userManager)
		{
			_userManager = userManager;
		}

		[HttpPost]
		[Route(nameof(Add))]
		public async Task<IdentityResult> Add(string email, string password)
		{
			var newUser = new RoadkillUser()
			{
				UserName = email,
				Email = email,
			};

			return await _userManager.CreateAsync(newUser, password);
		}
	}
}