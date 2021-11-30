namespace API_Noface.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Notification")]
    public partial class Notification
    {
        [Key]
        public int ID_Notification { get; set; }

        [StringLength(50)]
        public string ID_User { get; set; }

        public string Data_Notification { get; set; }

        public int? IDPost { get; set; }

        public virtual Post Post { get; set; }

        public virtual User User { get; set; }
    }
}
