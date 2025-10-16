using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConFlux.Model.Material_price
{
    [Table("MaterialItem")]
    public class MaterialItem
    {
        public int Id { get; set; }

        [ForeignKey(nameof(Category))]
        public int CategoryId { get; set; }
        public MaterialCategory Category { get; set; } = null!;

        [Required, MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? Unit { get; set; }

        public ICollection<MaterialPrice> Prices { get; set; } = new List<MaterialPrice>();
    }
}
