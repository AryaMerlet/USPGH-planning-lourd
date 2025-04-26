using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace USPGH_planning_lourd.classes
{
    [Table("model_has_roles")]
    public class UserRole
    {
        [Column("role_id")]
        public int RoleId { get; set; }

        [Column("model_type")]
        public string ModelType { get; set; } = string.Empty;

        [Column("model_id")]
        public int UserId { get; set; }

        // Navigation properties
        [ForeignKey("RoleId")]
        public virtual Role? Role { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}