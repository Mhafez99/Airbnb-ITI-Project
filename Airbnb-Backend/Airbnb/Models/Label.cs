namespace Airbnb.Models
{
    public class Label
    {
        public int Id { get; set; }
        public string Value { get; set; } = string.Empty;

        public int StayId { get; set; }
        public Stay Stay { get; set; } = null!;

    }
}
