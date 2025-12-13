using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class User : BaseEntity
    {
        [Column(TypeName = "citext")]
        public string Email { get; set; }

        public string Phone { get; set; }

        public UserRole Role { get; set; }
    }
}
