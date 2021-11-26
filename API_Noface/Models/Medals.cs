namespace API_Noface.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Medals
    {

        public Medals()
        {
            Achievements = new HashSet<Achievements>();
        }

        [Key]
        public int IDMedal { get; set; }

        [StringLength(200)]
        public string MedalName { get; set; }

        public string ImgMedal { get; set; }

        [StringLength(200)]
        public string Description { get; set; }

        public virtual ICollection<Achievements> Achievements { get; set; }
    }
}
