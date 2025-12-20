using System.ComponentModel.DataAnnotations;

namespace urunsatisportali.Models
{
    public class Category : BaseEntity
    {
        [Required(ErrorMessage = "Kategori adı gereklidir")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        // Navigation property
        public virtual ICollection<Product> Products { get; set; } = [];
    }
}
