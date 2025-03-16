using MaintenanceApp.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


public class AdminController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AdminController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    // List all users
    public IActionResult UsersList()
    {
        var users = _userManager.Users.ToList();
        return View(users);
    }

    [Authorize(Policy = "Admin")]
    // Show user details for role assignment
    [Route("Admin/EditUserRoles/{userId}")]
    public async Task<IActionResult> EditUserRoles(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return NotFound();

        var roles = _roleManager.Roles.ToList();
        var userRoles = await _userManager.GetRolesAsync(user);

        var model = new EditUserRolesViewModel
        {
            UserId = user.Id,
            Email = user.Email,
            AvailableRoles = roles.Select(r => r.Name).ToList(),
            AssignedRoles = userRoles.ToList()
        };

        return View(model);
    }

    [HttpPost]
    [Route("Admin/UpdateUserRoles")]
    public async Task<IActionResult> UpdateUserRoles(string userId, List<string> selectedRoles)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return NotFound();

        var currentRoles = await _userManager.GetRolesAsync(user);
        var result = await _userManager.RemoveFromRolesAsync(user, currentRoles);

        if (!result.Succeeded)
        {
            ModelState.AddModelError("", "Error removing roles");
            return RedirectToAction("EditUserRoles", new { userId });
        }

        result = await _userManager.AddToRolesAsync(user, selectedRoles);
        if (!result.Succeeded)
        {
            ModelState.AddModelError("", "Error adding new roles");
        }

        return RedirectToAction("UsersList");
    }
}
