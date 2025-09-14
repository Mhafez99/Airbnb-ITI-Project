using System.ComponentModel.DataAnnotations;

namespace Airbnb.Models
{
    public class Review
    {
        public int Id { get; set; }

        public DateTime At { get; set; }

        [MaxLength(500)]
        public string Txt { get; set; } = string.Empty;

        public int Rate { get; set; }

        public string ReviewerId { get; set; } = string.Empty;
        public string ReviewerFullname { get; set; } = string.Empty;
        public string ReviewerImgUrl { get; set; } = string.Empty;

        public StatReviews? StatReviews { get; set; }

        public int StayId { get; set; }
        public Stay Stay { get; set; } = null!;

    }
}
