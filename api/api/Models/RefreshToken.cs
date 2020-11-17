using System;
using System.ComponentModel.DataAnnotations;

namespace api.Models
{
    public class RefreshToken
    {
        [Key]
        public string Token { get; set; }
        public string JwtId { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime Expiry { get; set; }
        public bool Invalidated { get; set; }
        public bool Used { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
    }
}