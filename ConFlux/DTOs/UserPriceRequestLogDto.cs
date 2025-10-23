namespace ConFlux.DTOs
{
    public class UserPriceRequestLogDto
    {
        public string Username { get; set; } = string.Empty;
        public decimal M2 { get; set; }
        public int ObjectTypeId { get; set; }
        public int RegionId { get; set; }
        public int PriceCategoryId { get; set; }
        public int PeriodId { get; set; }
        public string Structure { get; set; } = string.Empty;
        public string? Opis { get; set; }
        public string? Napomena { get; set; }
        public string? ParcelData { get; set; }
        public string? AiPrompt { get; set; }
        public string? AiResponse { get; set; }
    }
}
