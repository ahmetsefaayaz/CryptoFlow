

namespace CryptoFlow.Application.Dtos.UserDtos;

public class GetUserDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public IEnumerable<GetRoleDto> RoleDto { get; set; }
    public IEnumerable<GetClaimDto> ClaimDto { get; set; }
    public DateTime CreatedAt { get; set; }
}