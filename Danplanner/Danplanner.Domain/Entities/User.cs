using System.ComponentModel.DataAnnotations;

namespace Danplanner.Domain.Entities
{
    internal class User
    {
        [Key]
        public int UserId { get; set; }
        [Required]
        public string UserAdress { get; set; }
        public int UserMobile { get; set; }
        [Required]
        public string UserEmail { get; set; }
    }
}
