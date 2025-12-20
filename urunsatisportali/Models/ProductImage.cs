using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace urunsatisportali.Models
{
    public class ProductImage
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        [Required]
        public string ImageUrl { get; set; } = string.Empty;

        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }
    }
}
