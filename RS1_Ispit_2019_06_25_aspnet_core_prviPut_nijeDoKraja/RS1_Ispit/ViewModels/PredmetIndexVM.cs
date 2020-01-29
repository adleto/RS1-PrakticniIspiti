using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RS1_Ispit_asp.net_core.ViewModels
{
    public class PredmetIndexVM
    {
        public List<Row> Rows { get; set; }

        public class Row
        {
            public int Id { get; set; }
            public string NazivPredmeta { get; set; }
            public string SkolaskaGodina { get; set; }
            public string NastavnikImePrezime { get; set; }
            public int BrojOdrzanihCasova { get; set; }
            public int BrojStudenata { get; set; }
        }
    }
}
