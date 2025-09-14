namespace Airbnb.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string? Fullname { get; set; } = string.Empty;
        public string? ThumbnailUrl { get; set; } = string.Empty;
    }
}
