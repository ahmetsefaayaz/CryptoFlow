using CryptoFlow.Domain.Entities;
using CryptoFlow.Domain.Enums;

namespace CryptoFlow.Application.Interfaces.IAuthentication;

public interface IAdminService
{
    public Task <List<Role>> GetRolesAsync();
    public Task<Role>  GetRoleAsync(Guid id);
    public Task<string> AssignRoleAsync(Guid id, string role);
    public Task<string> AssignClaimAsync(Guid id, string type, string value);
    public Task<string> CreateRoleAsync(string roleName);
    public Task<string> DeleteRoleAsync(Guid id);
    public Task<string> RemoveClaimAsync(Guid id, string type, string value);
}