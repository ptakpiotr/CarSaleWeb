using AutoMapper;
using CarSale.Models;
using CarSale.Models.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace CarSale.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly IEmailSender _sender;

        public AccountController(ILogger<AccountController> logger, UserManager<ApplicationUser> userManager,
                                SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager,
                                IMapper mapper, IEmailSender sender)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _sender = sender;
        }

        //RegisterACTION
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string returnUrl, RegisterUserDTO model)
        {
            if (ModelState.IsValid)
            {
                var appUser = _mapper.Map<ApplicationUser>(model);
                appUser.UserName = model.Email;

                var user = await _userManager.CreateAsync(appUser, model.Password);

                if (user.Succeeded)
                {
                    var res = await _userManager.AddToRoleAsync(appUser, "User");

                    if (res.Succeeded)
                    {
                        var token = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);

                        var redirUrl = Url.Action("ConfirmEmail", "Account",
                            new { userId = appUser.Id, token = token }, Request.Scheme);

                        await _sender.SendEmailAsync(appUser.Email, "Confirm email", redirUrl);

                        return View("ConfirmationEmailWait");
                    }
                    else
                    {
                        foreach (var error in res.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }

                }
                else
                {
                    foreach (var error in user.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }

            return View(model);
        }


        //LoginACTION
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string returnUrl, LoginUserDTO model)
        {
            if (ModelState.IsValid)
            {

                var res = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

                if (res.Succeeded && (await _userManager.FindByEmailAsync(model.Email)).EmailConfirmed)
                {
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return LocalRedirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Not able to login in!");
                }
            }

            return View(model);
        }


        //LogoutACTION
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            if (User.Identity.IsAuthenticated == true)
            {
                await _signInManager.SignOutAsync();
            }

            return RedirectToAction("Index", "Home");
        }

        //AccessDeniedACTION
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        //ExternalLoginACTION
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", provider, new { returnUrl = returnUrl });

            var props = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

            return new ChallengeResult(provider, props);

        }

        //ExternalLoginCallbackACTION
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl, string remoteError)
        {
            if (string.IsNullOrEmpty(returnUrl))
            {
                returnUrl = "/Home/Index";
            }

            if (!string.IsNullOrEmpty(remoteError))
            {
                return View("Error");
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return View("Error");
            }
            else
            {
                var user = await _userManager.FindByNameAsync(info.Principal.Identity.Name);

                if (user == null)
                {
                    await _userManager.CreateAsync(new ApplicationUser
                    {
                        Email = user.Email,
                        UserName = user.Email
                    });
                }

                await _userManager.AddLoginAsync(user, info);


                var res = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false, true);
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

        //ConfirmEmailACTION
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(token))
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return View("Error");
                }

                var res = await _userManager.ConfirmEmailAsync(user, token);

                return View("ConfirmEmailConfirmed");
            }

            return View("Error");
        }


        //ForgotPasswordACTION
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user == null && user.EmailConfirmed == false)
                {
                    return View("Error");
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                var redirUrl = Url.Action("ChangePassword", "Account"
                    , new { email = model.Email, token = token }, Request.Scheme);

                await _sender.SendEmailAsync(model.Email, "Password Reset", redirUrl);

                ViewData["ForgotSuccess"] = "Yes";
            }
            return View(model);
        }

        //ChangePasswordACTION
        [HttpGet]
        public IActionResult ChangePassword(string email, string token)
        {
            ViewData["Email"] = email;
            ViewData["Token"] = token;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(string email, string token, ChangePasswordModel md)
        {
            if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(token) && ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null || user.EmailConfirmed == false)
                {
                    return View("Error");
                }
                else
                {
                    var res = await _userManager.ResetPasswordAsync(user, token, md.Password);
                    if (res.Succeeded)
                    {
                        return RedirectToAction("Login", "Account");
                    }
                    else
                    {
                        foreach (var err in res.Errors)
                        {
                            ModelState.AddModelError("", err.Description);
                        }
                    }
                }
            }
            ModelState.AddModelError("", "Invalid token/email");

            return View(md);
        }
    }
}
