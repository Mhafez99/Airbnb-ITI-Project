namespace Airbnb.DTOs
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public string Txt { get; set; } = string.Empty;
        public int Rate { get; set; }
        public ReviewerDto By { get; set; } = new ReviewerDto();

        public StatReviewsDto StatReviews { get; set; }
    }
}
