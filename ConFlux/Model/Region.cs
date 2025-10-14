using System.ComponentModel.DataAnnotations.Schema;

namespace ConFlux.Model
{
    [Table("Region")]
    public class Region
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }
}
