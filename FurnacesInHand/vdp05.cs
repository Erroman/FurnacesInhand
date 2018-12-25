namespace FurnacesInHand
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("public.vdp05")]
    public partial class vdp05
    {
        public DateTime? dateandtime { get; set; }

        public int? mks { get; set; }

        [StringLength(50)]
        public string tagname { get; set; }

        public double? val { get; set; }

        public int id { get; set; }
    }
}
