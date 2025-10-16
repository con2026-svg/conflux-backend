using ConFlux.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

    }
}
