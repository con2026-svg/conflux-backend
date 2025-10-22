using ConFlux.Data;
using ConFlux.DTOs;
using Microsoft.EntityFrameworkCore;

namespace ConFlux.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly AppDbContext _ctx;
        public ReportRepository(AppDbContext ctx) => _ctx = ctx;

        public async Task<ReportDto?> GetReportAsync(int id)
        {
            // 🔹 Nađi zapis iz UserPriceRequestLog tabele
            var log = await _ctx.UserPriceRequestLogs
                .Include(x => x.Region)
                .Include(x => x.PriceCategory)
                .Include(x => x.Period)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (log == null)
                return null;

            // 🔹 Mapiraj u ReportDto
            var dto = new ReportDto
            {
                Id = log.Id,
                Username = log.Username,
                CreatedAt = log.CreatedAt,
                M2 = log.M2,
                Region = log.Region?.Name ?? "Nepoznato",
                Category = log.PriceCategory?.Name ?? "Nepoznato",
                Structure = log.Structure,
                UserNote = log.Napomena,
                AiDescription = log.AiResponse ?? "Nema AI opisa",
                KeyValues = new List<(string, string)>
                {
                    ("Period", log.Period != null ? $"Q{log.Period.Quarter} {log.Period.Year}" : "-"),
                    ("Tip radova", log.Opis ?? "-"),
                    ("Procena cene", $"{log.M2 * 2150m:0.00} € ukupno (primer)"),
                    ("Površina", $"{log.M2:0.00} m²")
                }
            };

            return dto;
        }
    }
}
