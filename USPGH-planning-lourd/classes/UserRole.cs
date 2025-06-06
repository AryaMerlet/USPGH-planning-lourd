using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace USPGH_planning_lourd.classes
{
    [Table("assigned_roles")]
    public class UserRole
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("role_id")]
        public long RoleId { get; set; }

        [Column("entity_type")]
        public string EntityType { get; set; } = string.Empty;

        [Column("entity_id")]
        public int EntityId { get; set; }

        [Column("restricted_to_id")]
        public long? RestrictedToId { get; set; }

        [Column("restricted_to_type")]
        public string? RestrictedToType { get; set; }

        [Column("scope")]
        public int? Scope { get; set; }

        // Navigation properties
        [ForeignKey("RoleId")]
        public virtual Role? Role { get; set; }

        [ForeignKey("EntityId")]
        public virtual User? User { get; set; }
    }
}