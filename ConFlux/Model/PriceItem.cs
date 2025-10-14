using System.ComponentModel.DataAnnotations.Schema;

namespace ConFlux.Model
{
    [Table("PriceItem")]
    public class PriceItem
    {
        public int Id { get; set; }
        public int ObjectTypeId { get; set; }
        public int CategoryId { get; set; }
        public int RegionId { get; set; }
        public int PeriodId { get; set; }
        public int WorkTypeId { get; set; }

        public int PriceTypeId { get; set; }
        public decimal Price { get; set; }

        public WorkType WorkType { get; set; } = null!;


        public ObjectType? ObjectType { get; set; }
        public Category? Category { get; set; }
        public Region? Region { get; set; }
        public QuarterPeriod? Period { get; set; }
        public PriceType? PriceType { get; set; }
    }
}
