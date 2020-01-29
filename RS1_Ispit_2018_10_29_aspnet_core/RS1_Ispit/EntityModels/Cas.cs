using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace RS1_Ispit_asp.net_core.EntityModels
{
    public class Cas
    {
        public int Id { get; set; }
        public DateTime Datum { get; set; }
        [ForeignKey(nameof(PredajePredmet))]
        public int PredajePredmetId { get; set; }
        public PredajePredmet PredajePredmet { get; set; }
        public string Sadrzaj { get; set; }
    }
}
