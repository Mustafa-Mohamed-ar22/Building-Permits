using Building_Permits.Core.Entities;
using Building_Permits.Core.Repo;
using Building_Permits.Infrastructure;
using Building_Permits.MVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Building_Permits.MVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly AppDbContext _context;
        private readonly ICostRepo costService;

        public AdminController(UserManager<ApplicationUser> userManager, AppDbContext _context, ICostRepo costService)
        {
            this.userManager = userManager;
            this._context = _context;
            this.costService = costService;
        }

        [HttpGet]
        public IActionResult CreateUser()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(RegisterationVM viewmodel)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = new ApplicationUser()
                {
                    PhoneNumber = viewmodel.Phone,
                    isActive = true,
                    UserName = viewmodel.Username,
                    FullName = viewmodel.FullName,
                    CreatedAt = DateTime.Now
                };
                var res = await userManager.CreateAsync(user, viewmodel.Password);
                if (res.Succeeded)
                {
                    var isAdded = await userManager.AddToRoleAsync(user, "Engineer");
                    if (isAdded.Succeeded)
                    {
                        TempData["Register"] = $"تم إضافة المهندس {viewmodel.FullName} بنجاح.";
                        return RedirectToAction("Users");
                    }

                }
            }
            return View(viewmodel);
        }
        [HttpGet]
        public IActionResult Users()
        {
            var userList = _context.Users
            .Select(x => new
            {
                x.UserName,
                x.CreatedAt,
                x.isActive,
                x.FullName,
                x.PhoneNumber,
                x.LastLogin,
                x.Id
            })
            .ToList(); // Materialize the query

            // Step 2: Enrich with additional data
            var users = userList.Select(x => new UserVM
            {
                username = x.UserName,
                createdAt = x.CreatedAt,
                status = x.isActive ? "Active" : "Inactive",
                FullName = x.FullName,
                phone = x.PhoneNumber,
                LastLogin = x.LastLogin,
                NoOfPermits = _context.Permits.Count(y => y.CreatedBy == x.Id),
                Id = x.Id,
                TotalCosts = costService.GetTotalUserCosts(x.Id)
            }).OrderBy(x => x.createdAt).ToList();
            return View(users);
        }
        [HttpGet]
        public async Task<IActionResult> ActivateUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User ID is required");
            }

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found");
            }

            user.isActive = true;
            var result = await userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                TempData["ActivateUser"] = $"User '{user.UserName}' has been activated successfully!";
            }
            else
            {
                TempData["ActivateUser"] = "Failed to activate user";
            }

            return RedirectToAction("Users");
        }

        [HttpGet]
        public async Task<IActionResult> DeactivateUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User ID is required");
            }

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found");
            }

            user.isActive = false;
            var result = await userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                TempData["DeactivateUser"] = $"User '{user.UserName}' has been deactivated successfully!";
            }
            else
            {
                TempData["DeactivateUser"] = "Failed to deactivate user";
            }

            return RedirectToAction("Users");
        }
        [HttpGet]
        public IActionResult searchUsers(string q)
        {
            var userList = _context.Users
            .Select(x => new
            {
                x.UserName,
                x.CreatedAt,
                x.isActive,
                x.FullName,
                x.PhoneNumber,
                x.LastLogin,
                x.Id
            })
            .ToList();
            var users = userList.Select(x => new UserVM
            {
                username = x.UserName,
                createdAt = x.CreatedAt,
                status = x.isActive ? "Active" : "Inactive",
                FullName = x.FullName,
                phone = x.PhoneNumber,
                LastLogin = x.LastLogin,
                NoOfPermits = _context.Permits.Count(y => y.CreatedBy == x.Id),
                Id = x.Id,
                TotalCosts = costService.GetTotalUserCosts(x.Id)
            }).OrderBy(x => x.createdAt).ToList();
            var res = string.IsNullOrEmpty(q)? users:
                users.Where(x=>x.username.ToLower().Contains(q.ToLower())||
                x.status.Contains(q.ToLower())||x.FullName.ToLower().Contains(q.ToLower())||
                x.phone.Contains(q)).ToList();
            return PartialView("_UserTable", res);
        }
    }
}
