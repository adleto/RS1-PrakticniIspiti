using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RS1_Ispit_asp.net_core.ViewModels
{
    public class IndexVM
    {
        public List<MainRow> Rows { get; set; }

        public class MainRow
        {
            public string NazivPredmeta { get; set; }
            public List<Row> InsideRows { get; set; }

            public class Row
            {
                public int AngazovanId { get; set; }
                public string SkolskaGodina { get; set; }
                public string Nastavnik { get; set; }
                public int OdrzanoCasova { get; set; }
                public int StudenataNaPredmetu { get; set; }
            }
        }
    }
}
