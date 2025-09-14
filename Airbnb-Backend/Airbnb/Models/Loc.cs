using System.ComponentModel.DataAnnotations;

namespace Airbnb.Models
{
    public class Loc
    {
        public int Id { get; set; } 

        [Required, MaxLength(100)]
        public string Country { get; set; } = string.Empty;

        [MaxLength(10)]
        public string CountryCode { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string? City { get; set; } = string.Empty;

        [MaxLength(250)]
        public string? Address { get; set; } = string.Empty;

        public double? Lat { get; set; }
        public double? Lan { get; set; }

        public int StayId { get; set; }
        public Stay Stay { get; set; } = null!;

    }
}
