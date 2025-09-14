namespace Airbnb.Models
{
    public class LikedByUser
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int StayId { get; set; }
        public Stay Stay { get; set; } = null!;

    }
}
