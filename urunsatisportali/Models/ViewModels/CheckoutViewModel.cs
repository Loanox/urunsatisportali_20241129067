using System.ComponentModel.DataAnnotations;

namespace urunsatisportali.Models.ViewModels
{
    public class CheckoutViewModel
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Phone { get; set; } = string.Empty;

        [Required]
        public string Address { get; set; } = string.Empty;

        [Required]
        public string Country { get; set; } = string.Empty;

        [Required]
        public string City { get; set; } = string.Empty;

        [Required]
        public string ZipCode { get; set; } = string.Empty;

        public string PaymentMethod { get; set; } = "COD"; // Cash on Delivery default

        // We can include Cart here if we want to bind it, but usually we read form Session
    }
}
