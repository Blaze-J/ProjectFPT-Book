using FPTBook.Data;
using FPTBook.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FPTBook.Controllers
{
    
	public class AccountsController : Controller
	{
        ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AccountsController( ApplicationDbContext context ,UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager) { 
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task<IActionResult> Index([FromQuery] string? search)
        {
            var roles = await _roleManager.Roles.ToListAsync();

     
             List<RoleAndUserModel> usersByRole = new List<RoleAndUserModel>();

            foreach (var role in roles)
            {
                // Get users for the current role
                var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);

                usersByRole.Add(new RoleAndUserModel() { rolename = role.Name, user = usersInRole.ToList() });
            }

            if (!String.IsNullOrEmpty(search))
            {
				usersByRole = usersByRole.Where(x=>x.user.Any(x=>x.UserName.Contains(search))).ToList();

			}

            return View(usersByRole);

        }
       
        public async Task<IActionResult> ChangePass(string id)
        {
                    

            return View(new ChangePass() { Id=id});

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePass([Bind("Id,NewPass,ConfirmNewPass")] ChangePass formChangepass)
        {
            if (ModelState.IsValid)
            {
                Console.Write(formChangepass.Id);
                var user = await _userManager.FindByIdAsync(formChangepass.Id);

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                user.PasswordHash = _userManager.PasswordHasher.HashPassword(user,formChangepass.NewPass);
                var result = await _userManager.UpdateAsync(user);
                
            }
           
            return RedirectToAction(nameof(Index));

        }

    }
}
