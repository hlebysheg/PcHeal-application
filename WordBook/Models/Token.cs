using System.ComponentModel.DataAnnotations;

namespace WordBook.Models
{
    public class RefreshToken
    {
        [Key]
        public int JwtId { get; set; }
        public string Token { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime ExpiryData { get; set; }
        public bool Used { get; set; }

        public int StudentId { get; set; }
        public User Student { get; set; }
       

    }
}
