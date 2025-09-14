namespace Airbnb.DTOs
{
    public class StayFilterDto
    {
        public int? LikeByUser { get; set; }   
        public string? Place { get; set; }        
        public string? Label { get; set; }      
        public int? HostId { get; set; }
        public bool? IsPetAllowed { get; set; }
    }
}
