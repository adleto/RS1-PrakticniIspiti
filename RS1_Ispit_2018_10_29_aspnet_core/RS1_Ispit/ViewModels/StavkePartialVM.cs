using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RS1_Ispit_asp.net_core.ViewModels
{
    public class StavkePartialVM
    {
        public List<Row> Rows { get; set; }

        public class Row
        {
            public string ImePrezime { get; set; }
            public int Ocjena { get; set; }
            public int StavkaId { get; set; }
            public bool Prisutan { get; set; }
            public bool OpravdanoOdsutan { get; set; }
        }
    }
}
