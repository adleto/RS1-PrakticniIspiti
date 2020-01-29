using RS1_Ispit_asp.net_core.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RS1_Ispit_asp.net_core.ViewModels
{
    public class DodajIspitVM
    {
        public int SkolaId { get; set; }
        public int PredmetId { get; set; }
        public string SkolskaGodina { get; set; }
        public string NastavnikIme { get; set; }
        public int NastavnikId { get; set; }
        public int SkolskaGodinaId { get; set; }
        public DateTime Datum { get; set; }
        public List<Skola> SkolaList { get; set; }
        public List<Predmet> PredmetList { get; set; }
    }
}
