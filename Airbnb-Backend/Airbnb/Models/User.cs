using System.ComponentModel.DataAnnotations;

namespace Airbnb.Models
{
    public class User
    {
        public int Id { get; set; } 

        [Required, MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Fullname { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Password { get; set; }

        [MaxLength(250)]
        public string ImgUrl { get; set; } = string.Empty;

        public int UserMsg { get; set; }
        public int HostMsg { get; set; }

        public ICollection<Order> Orders { get; set; } = new List<Order>();
        //public StayHost? HostProfile { get; set; }   

        public ICollection<Stay> Stays { get; set; } = new List<Stay>();
        public bool IsSuperhost { get; set; } = false;
        public int PolicyNumber { get; set; }
        [MaxLength(50)]
        public string ResponseTime { get; set; } = string.Empty;
    }
}
