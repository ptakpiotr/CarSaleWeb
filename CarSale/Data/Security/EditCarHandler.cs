using CarSale.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Primitives;
using System.Linq;
using System.Threading.Tasks;

namespace CarSale.Data.Security
{
    public class EditCarHandler : AuthorizationHandler<EditCarRequirement>
    {
        private readonly ICarsRepo _repo;
        private readonly IHttpContextAccessor _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EditCarHandler(ICarsRepo repo, IHttpContextAccessor context, UserManager<ApplicationUser> userManager)
        {
            _repo = repo;
            _context = context;
            _userManager = userManager;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, EditCarRequirement requirement)
        {
            StringValues requestedCar;
            _context.HttpContext.Request.Query.TryGetValue("editId", out requestedCar);

            if (requestedCar.Any())
            {
                string str = requestedCar.ToList().First().ToString();
                var post = _repo.GetSingleCar(str);
                var email = _userManager.FindByNameAsync(context.User.Identity.Name).GetAwaiter().GetResult();
                if (post.UserId == email.Id)
                {
                    context.Succeed(requirement);
                }
                else
                {
                    context.Fail();
                }
            }
            return Task.CompletedTask;
        }
    }
}
