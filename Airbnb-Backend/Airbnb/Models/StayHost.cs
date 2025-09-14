using System.ComponentModel.DataAnnotations;

namespace Airbnb.Models
{
    public class StayHost
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        [Required, MaxLength(100)]
        public string Fullname { get; set; } = string.Empty;

        [MaxLength(250)]
        public string PictureUrl { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public bool IsSuperhost { get; set; }

        public int PolicyNumber { get; set; }

        [MaxLength(50)]
        public string ResponseTime { get; set; } = string.Empty;

        public ICollection<Stay> Stays { get; set; } = new List<Stay>();
    }
}
