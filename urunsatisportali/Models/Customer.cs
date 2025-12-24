using System.ComponentModel.DataAnnotations;

namespace urunsatisportali.Models
{
    public class Customer : BaseEntity
    {
        [Required(ErrorMessage = "Müşteri adı gereklidir")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(100)]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
        public string? Email { get; set; }

        [StringLength(20)]
        public string? Phone { get; set; }

        [StringLength(200)]
        public string? Address { get; set; }

        [StringLength(50)]
        public string? City { get; set; }

        [StringLength(20)]
        public string? PostalCode { get; set; }

        [StringLength(50)]
        public string? Country { get; set; }

        // Navigation property
        public virtual ICollection<Sale> Sales { get; set; } = [];
    }
}
