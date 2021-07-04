using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToDo.API.Entities
{
    [Table("todo")]
    public class ToDo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }
        [Required]
        [MaxLength(8)]
        [Column("timestamp")]
        public string Timestamp { get; set; }
        [Required]
        [MaxLength(255)]
        [Column("description")]
        public string Description { get; set; }
        [Column("completed")]
        public bool Completed { get; set; }
        [Column("task_id")]
        [DefaultValue(0)]
        public long TaskId { get; set; }
        [Column("quantity")]
        [DefaultValue(1)]
        public int Quantity { get; set; }
    }
}