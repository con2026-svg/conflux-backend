using System.ComponentModel.DataAnnotations.Schema;

namespace ConFlux.Model
{
    [Table("UserPriceRequestLog")]
    public class UserPriceRequestLog
    {
        public int Id { get; set; }
     //   public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public decimal M2 { get; set; }
        public int RegionId { get; set; }
        [ForeignKey(nameof(RegionId))]
        public Region? Region { get; set; }

        public int PriceCategoryId { get; set; }
        [ForeignKey(nameof(PriceCategoryId))]
        public Category? PriceCategory { get; set; }

        public int PeriodId { get; set; }
        [ForeignKey(nameof(PeriodId))]
        public QuarterPeriod? Period { get; set; }
        public string Structure { get; set; } = string.Empty;
        public string? Opis { get; set; }
        public string? Napomena { get; set; }
        public string? ParcelData { get; set; }

        public string? AiPrompt { get; set; }
        public string? AiResponse { get; set; }
    }
}
