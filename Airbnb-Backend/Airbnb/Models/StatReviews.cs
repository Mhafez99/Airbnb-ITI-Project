namespace Airbnb.Models
{
    public class StatReviews
    {
        public int Id { get; set; }

        public double Cleanliness { get; set; }
        public double Communication { get; set; }
        public double CheckIn { get; set; }
        public double Accuracy { get; set; }
        public double Location { get; set; }
        public double Value { get; set; }
        public int ReviewId { get; set; }
        public Review Review { get; set; } = null!;

    }
}
