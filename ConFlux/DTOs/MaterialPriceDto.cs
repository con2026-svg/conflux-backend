namespace ConFlux.DTOs
{
    public class MaterialPriceDto
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public int Year { get; set; }
        public byte Quarter { get; set; }
        public decimal Price { get; set; }
    }

}