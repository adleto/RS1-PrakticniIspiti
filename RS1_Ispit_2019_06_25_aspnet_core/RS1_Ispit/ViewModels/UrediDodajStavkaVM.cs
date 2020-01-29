using RS1_Ispit_asp.net_core.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RS1_Ispit_asp.net_core.ViewModels
{
    public class UrediDodajStavkaVM
    {
        public string Student { get; set; }
        public List<SlusaPredmet> SlusaPredmetList { get; set; }
        public int SlusaPredmetId { get; set; }
        public int IspitId { get; set; }
        public int StavkaId { get; set; }
        public int Ocjena { get; set; }
    }
}
