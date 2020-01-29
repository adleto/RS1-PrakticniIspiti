using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ispit.Web.ViewModels
{
    public class GetObavezeVM
    {
        public List<Row> Rows { get; set; }

        public class Row
        {
            public int IdStanjeObaveze { get; set; }
            public string Naziv { get; set; }
            public string ProcentualnoStanje { get; set; }
            public int SaljiNotifikacijuDana { get; set; }
            public bool PonavljajNotifikaciju { get; set; }
        }
    }
}
