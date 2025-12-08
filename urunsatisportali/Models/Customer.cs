using System.ComponentModel.DataAnnotations;

namespace urunsatisportali.Models
{
    public class Customer
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Müþteri adý gereklidir")]
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

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }

        // Navigation property
        public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();
    }
}

