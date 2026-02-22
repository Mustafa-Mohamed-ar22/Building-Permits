using Building_Permits.Core.Entities;
using Building_Permits.Infrastructure;
using Building_Permits.MVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.Security.Claims;

namespace Building_Permits.MVC.Controllers
{
    public class AccountController : Controller
    {
        private AppDbContext _context;
        private readonly SignInManager<ApplicationUser> signInManager;

        public AccountController(AppDbContext _context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,SignInManager<ApplicationUser> signInManager)
        {
            this._context = _context;
            UserManager = userManager;
            this.roleManager = roleManager;
            this.signInManager = signInManager;
        }

        public UserManager<ApplicationUser> UserManager { get; }
        public RoleManager<IdentityRole> roleManager { get; }


        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginViewmodel)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await UserManager.FindByNameAsync(loginViewmodel.Username);
                List<Claim> claims = new List<Claim>();
                if (user != null && !user.isActive)
                {
                    ModelState.AddModelError("", "Your account has been deactivated. Please contact an administrator.");
                    return View(loginViewmodel);
                }
                if (user is not null)
                {
                    bool isCorrect = await UserManager.CheckPasswordAsync(user, loginViewmodel.Password);
                    if (isCorrect)
                    {
                        user.isActive = true;
                        user.LastLogin = DateTime.Now;
                        _context.SaveChanges();
                        claims.Add(new Claim("Id",user.Id));
                        claims.Add(new Claim("Name",user.FullName));
                        claims.Add(new Claim("Username",user.UserName));
                        claims.Add(new Claim("IsActive",user.isActive.ToString()));
                        await signInManager.SignInWithClaimsAsync(user, loginViewmodel.rememberMe, claims);
                        return RedirectToAction("Index", "Home");
                    }
                }
                ModelState.AddModelError("", "UserName or Password is incorrect");
            }
            ModelState.AddModelError("", "UserName or Password is incorrect");
            return View(loginViewmodel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();

            Response.Cookies.Delete(".AspNetCore.Identity.Application");
            Response.Cookies.Delete(".AspNetCore.Antiforgery");

            foreach (var cookie in Request.Cookies.Keys)
            {
                if (cookie.StartsWith(".AspNetCore"))
                {
                    Response.Cookies.Delete(cookie);
                }
            }
            return RedirectToAction("Login", "Account");
        }
    }
}