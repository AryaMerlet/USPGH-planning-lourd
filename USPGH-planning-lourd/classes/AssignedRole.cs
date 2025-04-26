using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace USPGH_planning_lourd.classes
{
    [Table("assigned_roles")]
    public class AssignedRole
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("role_id")]
        public int RoleId { get; set; }

        [Column("entity_id")]
        public int EntityId { get; set; } // This is the user ID

        [Column("entity_type")]
        public string EntityType { get; set; } // This should be "App\\Models\\User" for Laravel

        [Column("scope")]
        public int? Scope { get; set; }

        // Navigation properties
        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; }
    }
}