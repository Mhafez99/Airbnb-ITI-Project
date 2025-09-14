namespace Airbnb.Models
{
    public class Amenity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public int StayId { get; set; }
        public Stay Stay { get; set; } = null!;

    }
}
