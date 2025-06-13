using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebModels.Models
{
    public class ApplicationRole : IdentityRole<Guid>
    {
        public ApplicationRole() { }
        public ApplicationRole(string name) : base()
        {
            Name = name;
        }

        // Quan hệ 1 - N: 1 Role → N User
        [JsonIgnore]
        public virtual ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
    }
}
