using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebModels.Models
{
    public class ApplicationRole : IdentityRole<Guid>
    {
        [Required]
        public DateTime NgayTao { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime NgaySua { get; set; } = DateTime.UtcNow;

        [Required]
        public bool TrangThai { get; set; } = true;

        // Quan hệ 1 - N: 1 Role → N User
        [JsonIgnore]
        public virtual ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
    }
}
