using ConFlux.DTOs;

namespace ConFlux.Repositories
{
    public interface IReportRepository
    {
        Task<ReportDto?> GetReportAsync(int id);
    }
}
