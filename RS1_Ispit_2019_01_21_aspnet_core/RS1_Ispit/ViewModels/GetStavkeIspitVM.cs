using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RS1_Ispit_asp.net_core.ViewModels
{
    public class GetStavkeIspitVM
    {
        public List<Row> Rows { get; set; }

        public class Row
        {
            public int StavkaId { get; set; }
            public string Ucenik { get; set; }
            public float ProsjekOcjena { get; set; }
            public bool Pristupio { get; set; }
            public int Rezultat { get; set; }
        }
    }
}
