using System.ComponentModel.DataAnnotations.Schema;

namespace ConFlux.Model
{
    [Table("QuarterPeriod")]
    public class QuarterPeriod
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public byte Quarter { get; set; }
    }
}
