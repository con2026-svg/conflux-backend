using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConFlux.Model.Material_price
{

        [Table("MaterialCategory")]
        public class MaterialCategory
        {
            public int Id { get; set; }

            [Required, MaxLength(100)]
            public string Name { get; set; } = string.Empty;

            public ICollection<MaterialItem> Items { get; set; } = new List<MaterialItem>();
        }
    
}
