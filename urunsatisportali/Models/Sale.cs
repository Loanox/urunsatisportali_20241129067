using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace urunsatisportali.Models
{
    public class Sale : BaseEntity
    {
        [Required]
        [StringLength(50)]
        public string SaleNumber { get; set; } = string.Empty;

        // Customer is optional
        public int? CustomerId { get; set; }

        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Discount { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Tax { get; set; } = 0;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal FinalAmount { get; set; }

        [StringLength(20)]
        public string Status { get; set; } = "Pending"; // Pending, Completed, Cancelled

        [StringLength(500)]
        public string? Notes { get; set; }

        public DateTime SaleDate { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual Customer? Customer { get; set; }
        public virtual ICollection<SaleItem> SaleItems { get; set; } = [];
    }
}
