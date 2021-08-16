using AutoMapper;
using CarSale.Models;
using CarSale.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarSale.Controllers
{
    [Authorize(Policy = "AdminPolicy")]
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;

        public AdminController(ILogger<AdminController> logger, UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager,
            IMapper mapper)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _mapper = mapper;
        }

        public IActionResult ListUsers()
        {
            var users = _userManager.Users;

            var output = _mapper.Map<List<ReadUserDTO>>(users);

            return View(output);
        }

        [HttpGet]
        [Authorize(Policy = "ManagePolicy")]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return View("Error");
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            var userClaims = (await _userManager.GetClaimsAsync(user)).Select(c => $"{c.Type}:{c.Value}");

            var output = new EditUserModel
            {
                User = _mapper.Map<ReadUserDTO>(user),
                Roles = userRoles.ToList(),
                Claims = userClaims.ToList()
            };

            return View(output);
        }

        [HttpPost]
        [Authorize(Policy = "ManagePolicy")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(string id, EditUserModel model)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return View("Error");
            }

            if (ModelState.IsValid)
            {
                user.Email = model.User.Email;
                user.City = model.User.City;

                var res = await _userManager.UpdateAsync(user);
                if (res.Succeeded)
                {
                    return RedirectToAction("ListUsers", "Admin");
                }
                else
                {
                    foreach (var err in res.Errors)
                    {
                        ModelState.AddModelError("", err.Description);
                    }
                }

            }
            return View(model);
        }

        [HttpGet]
        [Authorize(Policy = "ManagePolicy")]
        public async Task<IActionResult> EditRole(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return View("Error");
            }

            ViewData["UserID"] = id;

            var userRoles = await _userManager.GetRolesAsync(user);
            var allRoles = _roleManager.Roles.Select(r => r.Name);

            var output = new List<EditRoleModel>();

            foreach (var role in allRoles)
            {
                output.Add(new EditRoleModel { Name = role, Chosen = userRoles.Contains(role) });
            }

            return View(output);
        }

        [HttpPost]
        [Authorize(Policy = "ManagePolicy")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRole(string id, List<EditRoleModel> model)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return View("Error");
            }

            if (ModelState.IsValid)
            {
                var chosenRoles = model.Where(m => m.Chosen == true).Select(m => m.Name)
                    .ToList();

                await _userManager.RemoveFromRolesAsync(user, _roleManager.Roles.Select(r => r.Name).ToList());
                await _userManager.AddToRolesAsync(user, chosenRoles);

                return RedirectToAction("ListUsers", "Admin");
            }
            return View(model);
        }


        [HttpPost]
        [Authorize(Policy = "ManagePolicy")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return View("Error");
            }

            var res = await _userManager.DeleteAsync(user);
            if (res.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View("Error");
            }

        }

    }
}
