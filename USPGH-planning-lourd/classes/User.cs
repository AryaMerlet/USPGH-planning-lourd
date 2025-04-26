using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        // Navigation property for roles
        [NotMapped]
        public List<Role> Roles { get; set; } = new List<Role>();

        // Helper method to check roles
        public bool IsInRole(string roleName)
        {
            return Roles.Exists(r => r.Name.Equals(roleName, StringComparison.OrdinalIgnoreCase));
        }
    }
}