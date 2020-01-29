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
        public Ispit Ispit { get; set; }
        [ForeignKey(nameof(Ispit))]
        public int IspitId { get; set; }
        public SlusaPredmet SlusaPredmet { get; set; }
        [ForeignKey(nameof(SlusaPredmet))]
        public int SlusaPredmetId { get; set; }
        public int Ocjena { get; set; }
        public bool Pristupio { get; set; }
    }
}
