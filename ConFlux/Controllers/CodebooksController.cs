using ConFlux.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
    }
}
