namespace ConFlux.DTOs
{
    public class PriceItemDto
    {
        public int ObjectTypeId { get; set; }
        public int CategoryId { get; set; }
        public int RegionId { get; set; }
        public int PeriodId { get; set; }
        public int WorkTypeId { get; set; }
        public int PriceTypeId { get; set; }
        public decimal Price { get; set; }
    }
}
