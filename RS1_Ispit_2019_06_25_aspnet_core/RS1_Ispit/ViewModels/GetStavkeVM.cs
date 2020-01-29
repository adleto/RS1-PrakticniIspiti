using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RS1_Ispit_asp.net_core.ViewModels
{
    public class GetStavkeVM
    {
        public DateTime Datum { get; set; }
        public bool Zakljucan { get; set; }
        public int IspitId { get; set; }
        public List<Row> Rows { get; set; }

        public class Row
        {
            public int StavkaId { get; set; }
            public string Student { get; set; }
            public int Ocjena { get; set; }
            public bool Pristupio { get; set; }
        }
    }
}
