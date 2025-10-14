using System.ComponentModel.DataAnnotations.Schema;

namespace ConFlux.Model
{
    [Table("ObjectType")]
    public class ObjectType
    {
        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
    }
}
