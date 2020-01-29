using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ispit.Web.ViewModels
{
    public class IndexVM
    {
        public List<OznaceniRow> OznaceniRows { get; set; }
        public List<NeoznaceniRow> NeoznaceniRows { get; set; }

        public class OznaceniRow
        {
            public DateTime Datum { get; set; }
            public string Nastavnik { get; set; }
            public string Opis { get; set; }
            public float RealizovanoObaveza { get; set; }
            public int DogadjajId { get; set; }
            public int OznaceniId { get; set; }
        }

        public class NeoznaceniRow
        {
            public DateTime Datum { get; set; }
            public string Nastavnik { get; set; }
            public string Opis { get; set; }
            public int BrojObaveza { get; set; }
            public int DogadjajId { get; set; }
        }
    }
}
