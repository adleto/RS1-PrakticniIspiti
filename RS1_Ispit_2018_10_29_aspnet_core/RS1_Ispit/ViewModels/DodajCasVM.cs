using RS1_Ispit_asp.net_core.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RS1_Ispit_asp.net_core.ViewModels
{
    public class DodajCasVM
    {
        public string Nastavnik { get; set; }
        public DateTime Datum { get; set; }
        public int PredajePredmetId { get; set; }
        public List<PredajePredmet> PredajePredmetOdjeljenjeList { get; set; }
    }
}
