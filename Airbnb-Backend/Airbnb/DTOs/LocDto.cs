namespace Airbnb.DTOs
{
    public class LocDto
    {
        public string? Country { get; set; } = string.Empty;
        public string? CountryCode { get; set; } = string.Empty;
        public string? City { get; set; } = string.Empty;
        public string? Address { get; set; } = string.Empty;
        public double? Lat { get; set; }
        public double? Lan { get; set; }
    }
}
