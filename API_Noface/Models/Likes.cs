namespace API_Noface.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Likes
    {
        public int ID { get; set; }

        public int IDPost { get; set; }

        [Required]
        [StringLength(50)]
        public string IDUser { get; set; }

        public virtual Post Post { get; set; }

        public virtual User User { get; set; }
    }
}
