using System.ComponentModel.DataAnnotations.Schema;

namespace ConFlux.Model
{
    [Table("WorkType")]

    public class WorkType
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }
}
