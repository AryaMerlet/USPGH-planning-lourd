using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace USPGH_planning_lourd.classes
{
    [Table("users")]
    public class User
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("first_name")]
        public string first_name { get; set; } = string.Empty;

        [Column("last_name")]
        public string last_name { get; set; } = string.Empty;

        [Column("email")]
        public string email { get; set; } = string.Empty;

        [Column("email_verified_at")]
        public DateTime? email_verified_at { get; set; }

        [Column("password")]
        public string password { get; set; } = string.Empty;

        [Column("created_at")]
        public DateTime? created_at { get; set; }

        [Column("updated_at")]
        public DateTime? updated_at { get; set; }

        [NotMapped]
        public bool IsAdmin { get; set; }

        [NotMapped]
        public bool IsSalarie { get; set; }

        [NotMapped]
        public List<Role> Roles { get; set; } = new List<Role>();

        // Check if user has a specific role
        public bool HasRole(string roleName)
        {
            if (roleName.Equals("admin", StringComparison.OrdinalIgnoreCase))
                return IsAdmin;

            if (roleName.Equals("salarie", StringComparison.OrdinalIgnoreCase))
                return IsSalarie;

            return Roles.Any(r => r.Name.Equals(roleName, StringComparison.OrdinalIgnoreCase));
        }

        // Add IsInRole method which was missing
        public bool IsInRole(string roleName)
        {
            return HasRole(roleName);
        }
    }
}