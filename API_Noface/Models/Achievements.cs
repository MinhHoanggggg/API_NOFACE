namespace API_Noface.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Achievements
    {

        [Key]
        public int ID_Achievements { get; set; }

        [StringLength(50)]
        public string IDUser { get; set; }

        public int? IDMedal { get; set; }

        public virtual Medals Medals { get; set; }

        public virtual User User { get; set; }
    }
}
