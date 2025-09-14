namespace Airbnb.DTOs
{
    public class FilterOrderDto
    {
        public int? HostId { get; set; }
        public int? BuyerId { get; set; }
        public string? Status { get; set; }
        public string? StayName { get; set; }
        public string? HostName { get; set; }
        public decimal? TotalPrice { get; set; }
        public string? Term { get; set; }
    }
}
