using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RS1_Ispit_asp.net_core.ViewModels
{
    public class PredmetVM
    {
        public string PredmetNaziv { get; set; }
        public int Id { get; set; }
        public string NastavnikImePreziem { get; set; }
        public string AkademskaGodina { get; set; }
        public List<Row> Rows { get; set; }

        public class Row
        {
            public int Id { get; set; }
            public string DatumIspita { get; set; }
            public int BrojStudenataKojiNisuPolozili { get; set; }
            public int BrojPrijavljenihStudenata { get; set; }
            public bool Zakljucano { get; set; }
        }
    }
}
