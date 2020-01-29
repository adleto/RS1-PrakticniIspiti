using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RS1_Ispit_asp.net_core.ViewModels
{
    public class EditCasVM
    {
        public int CasId { get; set; }
        public string Datum { get; set; }
        public string Odjeljenje { get; set; }
        public string Sadrzaj { get; set; }
    }
}
