using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RS1_Ispit_asp.net_core.ViewModels
{
    public class NastavaIndexVM
    {
        public List<Row> Rows { get; set; }
        

        public class Row
        {
            public int Id { get; set; }
            public string ImePrezime { get; set; }
            public string Skola { get; set; }
        }
    }
}
