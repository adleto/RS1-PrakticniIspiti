using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace RS1_Ispit_asp.net_core.EntityModels
{
    public class IspitStavka
    {
        public int Id { get; set; }
        public MaturskiIspit MaturskiIspit { get; set; }
        [ForeignKey(nameof(MaturskiIspit))]
        public int MaturskiIspitId { get; set; }
        public OdjeljenjeStavka OdjeljenjeStavka { get; set; }

        [ForeignKey(nameof(OdjeljenjeStavka))]
        public int OdjeljenjeStavkaId { get; set; }
        public bool Pristupio { get; set; }
        public int Bodovi { get; set; }
    }
}
