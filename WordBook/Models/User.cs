using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace WordBook.Models
{
    [Table("Student")]
    public class User
    {   
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(16), MinLength(4)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        //ref
        public int? RoleId { get; set; }
        public Role Role { get; set; }

    }
}
