using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace RS1_Ispit_asp.net_core.EntityModels
{
    public class CasStavka
    {
        public int Id { get; set; }
        public Cas Cas { get; set; }
        [ForeignKey(nameof(Cas))]
        public int CasId { get; set; }
        public OdjeljenjeStavka OdjeljenjeStavka { get; set; }
        [ForeignKey(nameof(OdjeljenjeStavka))]
        public int OdjeljenjeStavkaId { get; set; }
        public int Ocjena { get; set; }
        public string Napomena { get; set; }
        public bool Prisutan { get; set; }
        public bool OpravdanoOdsutan { get; set; }
    }
}
