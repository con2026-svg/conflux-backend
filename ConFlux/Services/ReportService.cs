using ConFlux.DTOs;
using ConFlux.Pdf;
using ConFlux.Repositories;
using QuestPDF.Fluent;

namespace ConFlux.Services
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _repo;
        private readonly IWebHostEnvironment _env;

        public ReportService(IReportRepository repo, IWebHostEnvironment env)
        {
            _repo = repo;
            _env = env;
        }

        public async Task<byte[]> GeneratePdfAsync(int id)
        {
            var data = await _repo.GetReportAsync(id)
                       ?? throw new KeyNotFoundException("Nije pronađen podatak za izveštaj.");

            // Učitaj logo iz wwwroot/images/logo.png
            byte[]? logoBytes = null;
            var logoPath = Path.Combine(_env.WebRootPath ?? "", "images", "logo.png");
            if (File.Exists(logoPath))
                logoBytes = await File.ReadAllBytesAsync(logoPath);

            var doc = new ReportDocument(data, logoBytes);
            return doc.GeneratePdf();
        }
    }
}
