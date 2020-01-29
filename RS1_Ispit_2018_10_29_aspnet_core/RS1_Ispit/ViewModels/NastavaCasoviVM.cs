using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RS1_Ispit_asp.net_core.ViewModels
{
    public class NastavaCasoviVM
    {
        public int NastavnikId { get; set; }
        public List<Row> Rows { get; set; }
        public class Row
        {
            public string DatumCasa { get; set; }
            public string GodinaOdjeljenje { get; set; }
            public string Predmet { get; set; }
            public List<string> OdsutniUcenici { get; set; }
            public int CasId { get; set; }
        }
    }
}
