namespace API_Noface.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Comment")]
    public partial class Comment
    {
        [Key]
        public int IDCmt { get; set; }

        public int IDPost { get; set; }

        [Required]
        [StringLength(50)]
        public string IDUser { get; set; }

        public string Content { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? Time { get; set; }

        public virtual Post Post { get; set; }

        public virtual User User { get; set; }
    }
}
