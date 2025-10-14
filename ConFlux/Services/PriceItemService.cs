using ConFlux.Data;
using ConFlux.Model;
using ConFlux.DTOs;
using Microsoft.EntityFrameworkCore;

namespace ConFlux.Services
{
    public class PriceItemService
    {
        private readonly AppDbContext _context;

        public PriceItemService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PriceItem>> GetAllAsync()
        {
            return await _context.PriceItems
                .Include(p => p.ObjectType)
                .Include(p => p.Category)
                .Include(p => p.Region)
                .Include(p => p.Period)
                .Include(p => p.WorkType)
                .Include(p => p.PriceType)
                .ToListAsync();
        }

        public async Task<PriceItem?> GetByIdAsync(int id)
        {
            return await _context.PriceItems
                .Include(p => p.ObjectType)
                .Include(p => p.Category)
                .Include(p => p.Region)
                .Include(p => p.Period)
                .Include(p => p.WorkType)
                .Include(p => p.PriceType)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<PriceItem> CreateAsync(PriceItemDto dto)
        {
            bool exists = await _context.PriceItems.AnyAsync(x =>
                x.ObjectTypeId == dto.ObjectTypeId &&
                x.CategoryId == dto.CategoryId &&
                x.RegionId == dto.RegionId &&
                x.PeriodId == dto.PeriodId &&
                x.WorkTypeId == dto.WorkTypeId &&
                x.PriceTypeId == dto.PriceTypeId);

            if (exists)
                throw new InvalidOperationException("Zapis sa ovom kombinacijom već postoji.");

            var entity = new PriceItem
            {
                ObjectTypeId = dto.ObjectTypeId,
                CategoryId = dto.CategoryId,
                RegionId = dto.RegionId,
                PeriodId = dto.PeriodId,
                WorkTypeId = dto.WorkTypeId,
                PriceTypeId = dto.PriceTypeId,
                Price = dto.Price
            };

            _context.PriceItems.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> UpdateAsync(int id, PriceItemDto dto)
        {
            var entity = await _context.PriceItems.FindAsync(id);
            if (entity == null) return false;

            entity.ObjectTypeId = dto.ObjectTypeId;
            entity.CategoryId = dto.CategoryId;
            entity.RegionId = dto.RegionId;
            entity.PeriodId = dto.PeriodId;
            entity.WorkTypeId = dto.WorkTypeId;
            entity.PriceTypeId = dto.PriceTypeId;
            entity.Price = dto.Price;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.PriceItems.FindAsync(id);
            if (entity == null) return false;

            _context.PriceItems.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
