using System.ComponentModel.DataAnnotations;

namespace Airbnb.DTOs
{
    public class StayUpdateDto
    {
        [Required]
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Type { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public decimal Price { get; set; }

        [MaxLength(500)]
        public string Summary { get; set; } = string.Empty;

        public int Capacity { get; set; }

        //public int HostId { get; set; }

        public string RoomType { get; set; } = string.Empty;

        public int Bathrooms { get; set; }
        public int Bedrooms { get; set; }

        public LocDto Loc { get; set; } = new LocDto();
        public List<String> ImgUrls { get; set; } = new List<String>();
        public List<String> Amenities { get; set; } = new List<String>();
        public List<String> Labels { get; set; } = new List<String>();

        public List<int>? LikedByUsers { get; set; }
        public List<ReviewDto>? Reviews { get; set; }
        public StatReviewsDto? StatReviews { get; set; } = null!;

         
    }
}
