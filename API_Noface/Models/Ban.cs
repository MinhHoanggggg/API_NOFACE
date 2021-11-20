namespace API_Noface.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Ban")]
    public partial class Ban
    {
        [Key]
        public int IDBan { get; set; }

        [Required]
        [StringLength(50)]
        public string IDUser { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime TimeBan { get; set; }

        public virtual User User { get; set; }
    }
}
