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

        [StringLength(50)]
        public string Title_Notification { get; set; }

        [StringLength(50)]
        public string Data_Notification { get; set; }

        public int? IDPost { get; set; }

        [StringLength(50)]
        public string ID_User_Seen_noti { get; set; }

        public int Status_Notification { get; set; }

        public virtual Post Post { get; set; }

        public virtual User User { get; set; }

        public virtual User User1 { get; set; }
    }
}
