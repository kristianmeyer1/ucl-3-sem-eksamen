using System.ComponentModel.DataAnnotations;

namespace Danplanner.Domain.Entities
{
    internal class Orderline
    {
        [Key]
        public int OrderlineId { get; set; }
    }
}
