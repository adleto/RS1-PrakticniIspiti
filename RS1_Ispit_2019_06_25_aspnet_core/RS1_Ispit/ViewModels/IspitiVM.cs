using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RS1_Ispit_asp.net_core.ViewModels
{
    public class IspitiVM
    {
        public string AkademskaGodina { get; set; }
        public string Predmet { get; set; }
        public string Nastavnik { get; set; }
        public int AngazovanId { get; set; }
        public List<Row> Rows { get; set; }

        public class Row
        {
            public int IspitId { get; set; }
            public string Datum { get; set; }
            public int BrojStudenataKojiNisuPolozili { get; set; }
            public int BrojPrijavljenihStudenata { get; set; }
            public bool Zakljucano { get; set; }
        }
    }
}
