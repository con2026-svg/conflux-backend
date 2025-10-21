namespace ConFlux.DTOs
{
    public class ReportDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = "";
        public DateTime CreatedAt { get; set; }

        public decimal M2 { get; set; }
        public string Region { get; set; } = "";
        public string Category { get; set; } = "";
        public string Structure { get; set; } = "";

        public string? UserNote { get; set; }
        public string? AiDescription { get; set; }

        // Ako želiš tabelu sa stavkama:
        public List<(string Label, string Value)> KeyValues { get; set; } = new();
    }
}
