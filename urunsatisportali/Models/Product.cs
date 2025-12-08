using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace urunsatisportali.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ürün adı zorunludur")]
        [StringLength(200, ErrorMessage = "Ürün adı en fazla 200 karakter olabilir")]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Açıklama en fazla 1000 karakter olabilir")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Fiyat zorunludur")]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Fiyat 0'dan büyük olmalıdır")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Stok miktarı zorunludur")]
        [Range(0, int.MaxValue, ErrorMessage = "Stok miktarı negatif olamaz")]
        public int StockQuantity { get; set; }

        [StringLength(50, ErrorMessage = "SKU en fazla 50 karakter olabilir")]
        public string? SKU { get; set; }

        [StringLength(100, ErrorMessage = "Marka en fazla 100 karakter olabilir")]
        public string? Brand { get; set; }

        [StringLength(50, ErrorMessage = "Birim en fazla 50 karakter olabilir")]
        public string? Unit { get; set; } = "Piece";

        public bool IsActive { get; set; } = true;

        // CategoryId made nullable to make category optional
        public int? CategoryId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public virtual Category? Category { get; set; }
        public virtual ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
    }
}

