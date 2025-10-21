
using System.Threading.Tasks;

namespace ConFlux.Services
{
    public interface IReportService
    {
        Task<byte[]> GeneratePdfAsync(int id);
    }
}
