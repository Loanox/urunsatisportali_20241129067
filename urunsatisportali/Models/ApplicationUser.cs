using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace urunsatisportali.Models
{
    public class ApplicationUser : IdentityUser
    {
        [StringLength(100)]
        public string? FullName { get; set; }

        public bool IsAdmin { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? LastLoginAt { get; set; }
        public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();
    }
}
