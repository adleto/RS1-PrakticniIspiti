using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace RS1_Ispit_asp.net_core.EntityModels
{
    public class StudentIspit
    {
        public int Id { get; set; }
        public Ispit Ispit { get; set; }
        [ForeignKey("Ispit")]
        public int IspitId { get; set; }
        public SlusaPredmet SlusaPredmet { get; set; }
        [ForeignKey("SlusaPredmet")]
        public int SlusaPredmetId { get; set; }
        public bool PristupioIspitu { get; set; }
        public int Ocjena { get; set; }
    }
}
