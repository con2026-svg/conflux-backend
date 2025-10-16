using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConFlux.Model.Material_price
{
    [Table("MaterialPrice")]
    public class MaterialPrice
    {
        public int Id { get; set; }

        [ForeignKey(nameof(Item))]
        public int ItemId { get; set; }
        public MaterialItem Item { get; set; } = null!;

        public int Year { get; set; }

        [Range(1, 4)]
        public byte Quarter { get; set; }

        [Column(TypeName = "decimal(12,3)")]
        public decimal Price { get; set; }
    }
}
