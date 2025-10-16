using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConFlux.Model.Material_price
{
    [Table("Material")]
    public class Material
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? Unit { get; set; }

        public ICollection<MaterialPrice> Prices { get; set; } = new List<MaterialPrice>();
    }
}
