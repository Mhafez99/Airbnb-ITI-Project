using Airbnb.DTOs;
using System.ComponentModel.DataAnnotations;
using System.Threading;

namespace Airbnb.Models
{
    public class Stay
    {
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Type { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public virtual ICollection<StayImage> ImgUrls { get; set; } = new List<StayImage>();

        public decimal Price { get; set; }

        [MaxLength(500)]
        public string Summary { get; set; } = string.Empty;

        public int Capacity { get; set; }

        public virtual ICollection<Amenity> Amenities { get; set; } = new List<Amenity>();

        public virtual ICollection<Label> Labels { get; set; } = new List<Label>();

        public int HostId { get; set; }
        //public StayHost Host { get; set; } = null!;
        public virtual User Host { get; set; } = null!;

        public Loc Loc { get; set; } = null!;

        public ICollection<Review> Reviews { get; set; } = new List<Review>();

        public virtual ICollection<LikedByUser> LikedByUsers { get; set; } = new List<LikedByUser>();

        [MaxLength(50)]
        public string RoomType { get; set; } = string.Empty;

        public int Bathrooms { get; set; }
        public int Bedrooms { get; set; }

        public virtual ICollection<Order> Orders { get; set; } 

    }
}
