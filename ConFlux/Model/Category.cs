using System.ComponentModel.DataAnnotations.Schema;

namespace ConFlux.Model
{
    [Table("Category")]
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal MinArea { get; set; }
        public decimal? MaxArea { get; set; }
    }
}
