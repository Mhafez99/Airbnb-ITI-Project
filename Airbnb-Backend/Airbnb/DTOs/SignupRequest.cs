namespace Airbnb.DTOs
{
    public class SignupRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Fullname { get; set; } = string.Empty;
        public int PolicyNumber { get; set; }
        public string? ImgUrl { get; set; }= string.Empty;

    }
}
