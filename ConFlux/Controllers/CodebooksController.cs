using ConFlux.Data;
using ConFlux.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace ConFlux.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CodebooksController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CodebooksController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/sifarnici/objecttype
        [HttpGet("{type}")]
        public async Task<IActionResult> GetAll(string type)
        {
            switch (type.ToLower())
            {
                case "objecttype":
                    return Ok(await _context.ObjectTypes
                        .Select(o => new
                        {
                            o.Id,
                            o.Code,
                            o.Name,
                            ImageUrl = $"/images/objecttypes/{o.Code.ToLower()}.png" // npr. stambeni.jpg
                        })
                        .ToListAsync());

                case "region":
                    return Ok(await _context.Regions
                        .Select(r => new
                        {
                            r.Id,
                            r.Name
                        })
                        .ToListAsync());

                case "worktype":
                    return Ok(await _context.WorkTypes
                        .Select(w => new
                        {
                            w.Id,
                            w.Name
                        })
                        .ToListAsync());

                case "pricetype":
                    return Ok(await _context.PriceTypes
                        .Select(e => new
                        {
                            e.Id,
                            e.Name
                        })
                        .ToListAsync());

                case "category":
                    return Ok(await _context.Categories
                        .Select(c => new
                        {
                            c.Id,
                            c.Name,
                            c.MinArea,
                            c.MaxArea
                        })
                        .ToListAsync());

                case "quarterperiod":
                    return Ok(await _context.QuarterPeriods
                        .OrderByDescending(p => p.Year)
                        .ThenByDescending(p => p.Quarter)
                        .Select(p => new
                        {
                            p.Id,
                            Period = $"Q{p.Quarter} {p.Year}"
                        })
                        .ToListAsync());

                default:
                    return BadRequest($"Nepoznat šifrarnik: {type}");
            }
        }

        // POST api/codebooks/{type}
        [HttpPost("{type}")]
        public async Task<IActionResult> Add(string type, [FromBody] JsonElement data)
        {
            switch (type.ToLower())
            {
                case "region":
                    var newRegion = new Region { Name = data.GetProperty("name").GetString()! };
                    _context.Regions.Add(newRegion);
                    await _context.SaveChangesAsync();
                    return Ok(newRegion);

                case "category":
                    var newCategory = new Category
                    {
                        Name = data.GetProperty("name").GetString()!,
                        MinArea = data.GetProperty("minArea").GetDecimal(),
                        MaxArea = data.GetProperty("maxArea").GetDecimal()
                    };
                    _context.Categories.Add(newCategory);
                    await _context.SaveChangesAsync();
                    return Ok(newCategory);

                default:
                    return BadRequest("Dodavanje nije podržano za ovaj šifrarnik.");
            }
        }

        // PUT api/codebooks/{type}/{id}
        [HttpPut("{type}/{id}")]
        public async Task<IActionResult> Update(string type, int id, [FromBody] JsonElement data)
        {
            switch (type.ToLower())
            {
                case "region":
                    var region = await _context.Regions.FindAsync(id);
                    if (region == null) return NotFound();
                    region.Name = data.GetProperty("name").GetString()!;
                    await _context.SaveChangesAsync();
                    return Ok(region);

                case "category":
                    var cat = await _context.Categories.FindAsync(id);
                    if (cat == null) return NotFound();
                    cat.Name = data.GetProperty("name").GetString()!;
                    cat.MinArea = data.GetProperty("minArea").GetDecimal();
                    cat.MaxArea = data.GetProperty("maxArea").GetDecimal();
                    await _context.SaveChangesAsync();
                    return Ok(cat);

                default:
                    return BadRequest("Izmena nije podržana za ovaj šifrarnik.");
            }
        }

        // DELETE api/codebooks/{type}/{id}
        [HttpDelete("{type}/{id}")]
        public async Task<IActionResult> Delete(string type, int id)
        {
            switch (type.ToLower())
            {
                case "region":
                    var region = await _context.Regions.FindAsync(id);
                    if (region == null) return NotFound();
                    _context.Regions.Remove(region);
                    await _context.SaveChangesAsync();
                    return Ok();

                case "category":
                    var cat = await _context.Categories.FindAsync(id);
                    if (cat == null) return NotFound();
                    _context.Categories.Remove(cat);
                    await _context.SaveChangesAsync();
                    return Ok();

                default:
                    return BadRequest("Brisanje nije podržano za ovaj šifrarnik.");
            }
        }

    }
}
