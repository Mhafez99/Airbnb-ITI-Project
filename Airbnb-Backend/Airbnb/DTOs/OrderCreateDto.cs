namespace Airbnb.DTOs
{
    public class OrderCreateDto
    {
        public UserDto Buyer { get; set; } = null!;
        public UserDto Host { get; set; } = null!;
        public StayDto Stay { get; set; } = null!;
        public GuestDto Guests { get; set; } = null!;

        public decimal TotalPrice { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = "pending";
    }
}
