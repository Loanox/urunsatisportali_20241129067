using System.ComponentModel.DataAnnotations;

namespace urunsatisportali.Models.ViewModels
{
    public class CheckoutViewModel
    {
        [Required(ErrorMessage = "Lütfen Adınızı giriniz.")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lütfen Soyadınızı giriniz.")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lütfen E-posta adresinizi giriniz.")]
        [EmailAddress(ErrorMessage = "Geçerli bir E-posta adresi giriniz.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lütfen Telefon numaranızı giriniz.")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lütfen Adresinizi giriniz.")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lütfen Ülkenizi giriniz.")]
        public string Country { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lütfen Şehrinizi giriniz.")]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lütfen Posta Kodunu giriniz.")]
        public string ZipCode { get; set; } = string.Empty;

        public string PaymentMethod { get; set; } = "COD"; // Cash on Delivery default

        public CartViewModel Cart { get; set; } = new CartViewModel();
    }
}
