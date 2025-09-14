namespace Airbnb.Models
{
    public class Guest
    {
        public int Id { get; set; }

        public int Adults { get; set; }
        public int Children { get; set; }
        public int Infants { get; set; }
        public int Pets { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;

    }
}
