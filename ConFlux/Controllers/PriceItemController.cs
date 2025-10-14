using ConFlux.Data;
using ConFlux.DTOs;
using ConFlux.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConFlux.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PriceItemController : ControllerBase
    {

    private readonly AppDbContext _context;

        public PriceItemController(AppDbContext context)
        {
            _context = context;
        }

            [HttpGet("prices")]
        public async Task<IActionResult> GetPrices(
    [FromQuery] int objectTypeId,
    [FromQuery] int regionId,
    [FromQuery] int periodId,
    [FromQuery] decimal area,
    [FromQuery] int priceTypeId)
        {
            // 1️⃣ pronađi kategoriju po površini
            var category = await _context.Categories
                .Where(c => c.MinArea <= area && (c.MaxArea == null || c.MaxArea >= area))
                .FirstOrDefaultAsync();

            if (category == null)
                return NotFound($"Nema definisane kategorije za površinu {area} m².");

            // 2️⃣ proveri da li postoji tip cene
            var priceType = await _context.PriceTypes.FindAsync(priceTypeId);
            if (priceType == null)
                return NotFound($"Nepostojeći tip cene sa ID = {priceTypeId}.");

            // 3️⃣ pronađi cene koje odgovaraju svim filterima
            var prices = await _context.PriceItems
                .Include(p => p.WorkType)
                .Where(p => p.ObjectTypeId == objectTypeId
                         && p.CategoryId == category.Id
                         && p.RegionId == regionId
                         && p.PeriodId == periodId
                         && p.PriceTypeId == priceTypeId)
                .Select(p => new
                {
                    Category = category.Name,
                    WorkType = p.WorkType.Name,
                    PriceType = priceType.Name,
                    Price = p.Price
                })
                .ToListAsync();

            if (!prices.Any())
                return NotFound($"Nema cena za kombinaciju: kategorija '{category.Name}', tip '{priceType.Name}'.");

            return Ok(prices);
        }

           
        [HttpPost("priceentry")]
        public async Task<IActionResult> Create([FromBody] PriceItemDto dto)
        {
            var exists = await _context.PriceItems.AnyAsync(x =>
                x.ObjectTypeId == dto.ObjectTypeId &&
                x.CategoryId == dto.CategoryId &&
                x.RegionId == dto.RegionId &&
                x.PeriodId == dto.PeriodId &&
                x.WorkTypeId == dto.WorkTypeId &&
                x.PriceTypeId == dto.PriceTypeId);

            if (exists)
                return Conflict("Zapis sa ovom kombinacijom već postoji.");

            var item = new PriceItem
            {
                ObjectTypeId = dto.ObjectTypeId,
                CategoryId = dto.CategoryId,
                RegionId = dto.RegionId,
                PeriodId = dto.PeriodId,
                WorkTypeId = dto.WorkTypeId,
                PriceTypeId = dto.PriceTypeId,
                Price = dto.Price
            };

            _context.PriceItems.Add(item);
            await _context.SaveChangesAsync();

            // 🔹 Ponovo pročitaj isti red sa svim Include vezama
            var created = await _context.PriceItems
                .Include(p => p.ObjectType)
                .Include(p => p.Category)
                .Include(p => p.Region)
                .Include(p => p.Period)
                .Include(p => p.WorkType)
                .Include(p => p.PriceType)
                .FirstOrDefaultAsync(p => p.Id == item.Id);

            return Ok(created); // ✅ sada Angular dobija pun objekat sa nazivima
        }


        // 🔹 READ ALL
        [HttpGet("all")]
      
        public async Task<IActionResult> GetAll()
        {
            var items = await _context.PriceItems
                .Include(p => p.ObjectType)
                .Include(p => p.Category)
                .Include(p => p.Region)
                .Include(p => p.Period)
                .Include(p => p.WorkType)
                .Include(p => p.PriceType)
                .ToListAsync();

            return Ok(items);
        }

        // 🔹 READ ONE
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _context.PriceItems
                .Include(p => p.WorkType)
                .Include(p => p.PriceType)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (item == null) return NotFound();
            return Ok(item);
        }

        // 🔹 UPDATE
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PriceItemDto dto)
        {
            var item = await _context.PriceItems.FindAsync(id);
            if (item == null) return NotFound();

            item.ObjectTypeId = dto.ObjectTypeId;
            item.CategoryId = dto.CategoryId;
            item.RegionId = dto.RegionId;
            item.PeriodId = dto.PeriodId;
            item.WorkTypeId = dto.WorkTypeId;
            item.PriceTypeId = dto.PriceTypeId;
            item.Price = dto.Price;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // 🔹 DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.PriceItems.FindAsync(id);
            if (item == null) return NotFound();

            _context.PriceItems.Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }
 
        [HttpPost("savelog")]
        public async Task<IActionResult> CreateLog([FromBody] UserPriceRequestLogDto dto)
        {
            var log = new UserPriceRequestLog
            {
              //  UserId = dto.UserId,
                Username = dto.Username,
                M2 = dto.M2,
                RegionId = dto.RegionId,
                PriceCategoryId = dto.PriceCategoryId,
                PeriodId = dto.PeriodId,
                Structure = dto.Structure,
                Opis = dto.Opis,
                Napomena = dto.Napomena,
                ParcelData = dto.ParcelData,
                AiPrompt = dto.AiPrompt,
                AiResponse = dto.AiResponse
            };

            _context.UserPriceRequestLogs.Add(log);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Log uspešno sačuvan" });
        }


        [HttpGet("userlogs")]

        [HttpGet("logs")]
        public async Task<IActionResult> GetLogs()
        {
            var logs = await _context.UserPriceRequestLogs
                .OrderByDescending(l => l.CreatedAt)
                .Select(l => new
                {
                    l.Id,
                    l.Username,
                    l.CreatedAt,
                    l.M2,
                    l.Structure,
                    l.Opis,
                    l.Napomena,
                    l.ParcelData,
                    l.AiPrompt,
                    l.AiResponse,

                    // 🔹 Dovuci nazive iz drugih tabela:
                    Region = _context.Regions
                        .Where(r => r.Id == l.RegionId)
                        .Select(r => r.Name)
                        .FirstOrDefault(),

                    PriceCategory = _context.Categories
                        .Where(c => c.Id == l.PriceCategoryId)
                        .Select(c => c.Name)
                        .FirstOrDefault(),

                    Period = _context.QuarterPeriods
                        .Where(p => p.Id == l.PeriodId)
                        .Select(p => "Q" + p.Quarter + " " + p.Year)
                        .FirstOrDefault()
                })
                .Take(200)
                .ToListAsync();

            return Ok(logs);
        }



    }
}