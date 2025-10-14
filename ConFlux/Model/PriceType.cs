using System.ComponentModel.DataAnnotations.Schema;

namespace ConFlux.Model
{

    [Table("PriceType")]
    public class PriceType
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
