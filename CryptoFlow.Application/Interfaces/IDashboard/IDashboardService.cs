using CryptoFlow.Application.Dtos.DashboardDtos;

namespace CryptoFlow.Application.Interfaces.IDashboard;

public interface IDashboardService
{
    public Task <UserDashboardDto>  GetDashboardAsync(Guid userId);
}