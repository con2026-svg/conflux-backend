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


        [HttpPost]
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
            return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
        }

        // 🔹 READ ALL
        [HttpGet("all")]
      
        public async Task<IActionResult> GetAll()
        {
            var items = await _context.PriceItems
                .Include(p => p.ObjectType)
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

    }
}