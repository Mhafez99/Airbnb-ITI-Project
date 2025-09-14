namespace Airbnb.Models
{
    public class StayImage
    {
        public int Id { get; set; }
        public string Url { get; set; } = string.Empty;

        public int StayId { get; set; }
        public Stay Stay { get; set; } = null!;

    }
}
