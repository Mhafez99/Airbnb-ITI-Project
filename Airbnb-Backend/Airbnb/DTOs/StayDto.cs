using Airbnb.Models;

namespace Airbnb.DTOs
{
    public class StayDto
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Summary { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public int HostId { get; set; }
        public string RoomType { get; set; } = string.Empty;
        public int Bathrooms { get; set; }
        public int Bedrooms { get; set; }
        public LocDto Loc { get; set; } = new LocDto();

        public UserDto Host { get; set; } = new UserDto();
        public List<String> Labels { get; set; } = new List<String>();
        public List<String> Amenities { get; set; } = new List<String>();
        public List<int>? LikedByUsers { get; set; }
        public List<ReviewDto>? Reviews { get; set; }
        public StatReviewsDto? StatReviews { get; set; } = null!;

        public List<string> ImgUrls { get; set; } = new List<string>();
    }
}
