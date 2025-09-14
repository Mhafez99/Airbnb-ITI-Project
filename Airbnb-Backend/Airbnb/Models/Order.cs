using System.ComponentModel.DataAnnotations;

namespace Airbnb.Models
{
    public class Order
    {
        public int Id { get; set; } 

        public int BuyerId { get; set; }
        public User Buyer { get; set; } = null!;

        public decimal TotalPrice { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public Guest Guests { get; set; } = null!;

        public int StayId { get; set; }
        public Stay Stay { get; set; } = null!;

        public int HostId { get; set; }
        //public StayHost Host { get; set; } = null!;
        public User Host { get; set; } = null!;


        [MaxLength(50)]
        public string Status { get; set; } = "pending";

    }
}
