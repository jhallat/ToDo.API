using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToDo.API.Entities
{

    static class AuditActions
    {
        public const string ADD = "ADD";
        public const string DELETE = "DELETE";
        public const string UPDATE = "UPDATE";
    }
    
    [Table("checklist_audit")]
    public class CheckListAudit
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public long Id { get; set; }
        [Required]
        [Column("checklist_id")]
        public long ChecklistId { get; set; }
        [Required]
        [MaxLength(8)]
        [Column("audit_date")]
        public DateTime AuditDate { get; set; }
        [Column("audit_action")]
        public string AuditAction { get; set; }
        [Column("property")]
        [MaxLength(50)]
        public string Property { get; set; }
        [Column("original_value")]
        [MaxLength(255)]
        public String OriginalValue { get; set; }
        [Column("new_value")]
        [MaxLength(255)]
        public String NewValue { get; set; }
    }
}