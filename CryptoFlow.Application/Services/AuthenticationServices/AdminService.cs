using System.Security.Claims;
using CryptoFlow.Application.Exceptions;
using CryptoFlow.Application.Interfaces.IAuthentication;
using CryptoFlow.Application.Interfaces.MongoDb;
using CryptoFlow.Domain.Entities;
using CryptoFlow.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CryptoFlow.Application.Services.AuthenticationServices;

public class AdminService: IAdminService
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;

    public AdminService(UserManager<User> userManager, RoleManager<Role> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }


    public async Task<List<Role>> GetRolesAsync()
    {
        var roles = await _roleManager.Roles.ToListAsync();
        return roles;
    }

    public async Task<Role> GetRoleAsync(Guid id)
    {
        var role = await _roleManager.Roles.SingleOrDefaultAsync(r => r.Id == id);
        if (role == null) throw new NotFoundException("role not found");
        return role;
    }

    public async Task<string> AssignRoleAsync(Guid id, string role)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null) throw new NotFoundException("user not found");
        
        var result = await _userManager.AddToRoleAsync(user, role);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new WrongInputException($"Role assignment failed: {errors}");
        }
        return "role assigned";
    }

    public async Task<string> AssignClaimAsync(Guid id, string type, string value)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if(user == null) throw new NotFoundException("user not found");
        
        var result = await _userManager.AddClaimAsync(user, new Claim(type, value));
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new WrongInputException($"Claim assignment failed: {errors}");
        }
        return "claim assigned";
    }

    public async Task<string> CreateRoleAsync(string roleName)
    {
        
        var roleExists = await _roleManager.RoleExistsAsync(roleName);
        if(roleExists) throw new WrongInputException("Role already exists");
        var result = await _roleManager.CreateAsync(new Role 
        { 
            Name = roleName, 
            NormalizedName = roleName.ToUpper() 
        });
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new WrongInputException($"Role creation failed: {errors}");
        }
        return "role created";
    }

    public async Task<string> DeleteRoleAsync(Guid id)
    {
        var role = await _roleManager.FindByIdAsync(id.ToString());
        if (role == null) throw new NotFoundException("role not found");
        var result = await _roleManager.DeleteAsync(role);
        if (!result.Succeeded) throw new WrongInputException($"Role deletion failed: {result.Errors.First().Description}");
        return "role deleted";
        
    }

    public async Task<string> RemoveClaimAsync(Guid id, string type, string value)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null) throw new NotFoundException("User not found");

        var userClaims = await _userManager.GetClaimsAsync(user);

        var claimToRemove = userClaims.FirstOrDefault(c => c.Type == type && c.Value == value);

        if (claimToRemove == null)
            throw new NotFoundException("This claim does not exist for the user");

        var result = await _userManager.RemoveClaimAsync(user, claimToRemove);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new WrongInputException($"Claim deletion failed: {errors}");
        }

        return "Claim deleted successfully";
    }
    
}