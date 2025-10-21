using ConFlux.Data;
using ConFlux.DTOs;

namespace ConFlux.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly AppDbContext _ctx;
        public ReportRepository(AppDbContext ctx) { _ctx = ctx; }

        public async Task<ReportDto?> GetReportAsync(int id)
        {
            // Ovde mapiraj iz svojih tabela (PriceItem, Log itd.)
            // Primer “na suvo” radi demo-a:
            await Task.CompletedTask;
            return new ReportDto
            {
                Id = id,
                Username = "pera@example.com",
                CreatedAt = DateTime.UtcNow,
                M2 = 72.5m,
                Region = "Beograd",
                Category = "Standard",
                Structure = "2.5 soban",
                UserNote = "Kupatilo renovirano 2024.",
                AiDescription = "Na osnovu unetih parametara procenjena cena je u skladu sa regionalnim trendom...",
                KeyValues = new()
            {
                ("Period", "Q3 2025"),
                ("Tip radova", "Adaptacija"),
                ("Procena cene", "€2,150/m²"),
                ("Ukupno", "€155,375")
            }
            };
        }
    }
}
