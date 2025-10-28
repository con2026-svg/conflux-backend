using ConFlux.Data;
using ConFlux.DTOs;
using ConFlux.Model.Material_price;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ConFlux.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaterialController : ControllerBase
    {

        private readonly AppDbContext _context;

        public MaterialController(AppDbContext context)
        {
            _context = context;
        }

        // 🔹 1. Sve kategorije
        // GET: api/Material/categories
        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _context.MaterialCategories
                .OrderBy(c => c.Name)
                .Select(c => new
                {
                    c.Id,
                    c.Name,
                    ItemCount = c.Items.Count
                })
                .ToListAsync();

            return Ok(categories);
        }

        // 🔹 2. Stavke (potkategorije) po kategoriji
        // GET: api/Material/category/1/items
        [HttpGet("category/{categoryId}/items")]
        public async Task<IActionResult> GetItemsByCategory(int categoryId)
        {
            var items = await _context.MaterialItems
                .Where(i => i.CategoryId == categoryId)
                .OrderBy(i => i.Name)
                .Select(i => new
                {
                    i.Id,
                    i.Name,
                    i.Unit
                })
                .ToListAsync();

            if (!items.Any())
                return NotFound($"Nema stavki za kategoriju ID={categoryId}");

            return Ok(items);
        }

        // 🔹 3. Cene po kvartalima za konkretnu stavku
        // GET: api/Material/item/5/prices
        [HttpGet("item/{itemId}/prices")]
        public async Task<IActionResult> GetPricesByItem(int itemId)
        {
            var prices = await _context.MaterialPrices
                .Where(p => p.ItemId == itemId)
                .OrderBy(p => p.Year)
                .ThenBy(p => p.Quarter)
                .Select(p => new
                {
                    p.Id,
                    p.ItemId,
                    p.Year,
                    p.Quarter,
                    p.Price
                })
                .ToListAsync();

            if (!prices.Any())
                return NotFound($"Nema cena za stavku ID={itemId}");

            return Ok(prices);
        }


        // 🔹 4. (Opcionalno) Detalj stavke sa kategorijom
        // GET: api/Material/item/5
        [HttpGet("item/{itemId}")]
        public async Task<IActionResult> GetItemDetail(int itemId)
        {
            var item = await _context.MaterialItems
                .Include(i => i.Category)
                .FirstOrDefaultAsync(i => i.Id == itemId);

            if (item == null)
                return NotFound($"Stavka sa ID={itemId} nije pronađena.");

            return Ok(new
            {
                item.Id,
                item.Name,
                item.Unit,
                Category = item.Category.Name
            });
        }


        [HttpPost("price")]
        public async Task<IActionResult> AddPrice([FromBody] MaterialPriceDto dto)
        {
            var price = new MaterialPrice
            {
                ItemId = dto.ItemId,
                Year = dto.Year,
                Quarter = dto.Quarter,
                Price = dto.Price
            };

            try
            {
                _context.MaterialPrices.Add(price);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Cena uspešno dodata.", id = price.Id });
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException is SqlException sqlEx && sqlEx.Number == 2627)
                {
                    // 2627 = Violation of UNIQUE KEY constraint
                    return Conflict(new { message = "❗ Cenovnik za ovaj kvartal već postoji." });
                }

                // ostali SQL errori
                return StatusCode(500, new { message = "Greška prilikom upisa cenovnika.", details = ex.Message });
            }
        }


        // PUT: api/Material/price/5
        [HttpPut("price/{id}")]
        public async Task<IActionResult> UpdatePrice(int id, [FromBody] MaterialPrice model)
        {
            try
            {
                // 👇 ovde EF vidi tačan tip koji stiže iz requesta
                Console.WriteLine(">>> Model type: " + model.GetType().FullName);

                var existing = await _context.MaterialPrices.FirstOrDefaultAsync(p => p.Id == id);
                if (existing == null)
                    return NotFound($"Cena sa ID={id} nije pronađena.");

                existing.ItemId = model.ItemId;
                existing.Year = model.Year;
                existing.Quarter = model.Quarter;
                existing.Price = model.Price;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Cena uspešno ažurirana." });
            }
            catch (Exception ex)
            {
                // 🔹 ako EF i dalje puca zbog MaterialId — uhvati i prikaži detaljnu poruku
                Console.WriteLine(">>> ERROR: " + (ex.InnerException?.Message ?? ex.Message));
                return BadRequest(new { message = ex.InnerException?.Message ?? ex.Message });
            }
        }



        // DELETE: api/Material/price/5
        [HttpDelete("price/{id}")]
        public async Task<IActionResult> DeletePrice(int id)
        {
            var existing = await _context.MaterialPrices.FindAsync(id);
            if (existing == null)
                return NotFound();

            _context.MaterialPrices.Remove(existing);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Cena obrisana" });
        }


        [HttpGet("test-mp")]
        public async Task<IActionResult> TestMP()
        {
            var columns = await _context.Database
                .SqlQueryRaw<string>(
                    "SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'MaterialPrice'")
                .ToListAsync();

            return Ok(columns);
        }
    }
}
