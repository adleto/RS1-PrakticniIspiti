using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_Ispit_asp.net_core.EF;
using RS1_Ispit_asp.net_core.EntityModels;
using RS1_Ispit_asp.net_core.ViewModels;

namespace RS1_Ispit_asp.net_core.Controllers
{
    public class OdrzanaNastavaController : Controller
    {
        private readonly MojContext _context;

        public OdrzanaNastavaController(MojContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var model = new IndexVM
            {
                Rows = GetNastavniciUpakovano()
            };
            return View(model);
        }

        private List<IndexVM.Row> GetNastavniciUpakovano()
        {
            var list = new List<IndexVM.Row>();
            var nastavnici = _context.Nastavnik.ToList();
            foreach(var n in nastavnici)
            {
                var skoleNastavnika = GetSkoleNastavnika(n.Id);
                foreach(var s in skoleNastavnika)
                {
                    var item = new IndexVM.Row
                    {
                        Id = n.Id,
                        Nastavnik = n.Ime + " " + n.Prezime,
                        Skola = s
                    };
                    list.Add(item);
                }
            }
            return list;
        }

        private List<string> GetSkoleNastavnika(int id)
        {
            var pp = _context.PredajePredmet
                .Include(p=>p.Odjeljenje)
                    .ThenInclude(o=>o.Skola)
                .Where(p => p.NastavnikID == id)
                .ToList();
            var list = new List<string>();
            foreach(var p in pp)
            {
                if (!list.Contains(p.Odjeljenje.Skola.Naziv)) {
                    list.Add(p.Odjeljenje.Skola.Naziv);
                }
            }
            return list;
        }

        public IActionResult Nastavnik(int nastavnikId)
        {
            var model = new NastavnikVM
            {
                NastavnikId = nastavnikId,
                Rows = _context.MaturskiIspit
                .Where(n=>n.NastavnikId == nastavnikId)
                .Select(n=>new NastavnikVM.Row { 
                        Datum = n.Datum.ToString("dd.MM.yyyy"),
                        IspitId = n.Id,
                        Predmet = n.Predmet.Naziv,
                        Skola = n.Skola.Naziv
                }).ToList()
            };
            foreach(var r in model.Rows)
            {
                r.NisuPristupili = GetUceniciNisuPristupili(r.IspitId);
            }
            return View(model);
        }

        private List<string> GetUceniciNisuPristupili(int ispitId)
        {
            var uceniciNisuPristupili = _context.IspitStavka
                .Include(i => i.OdjeljenjeStavka)
                    .ThenInclude(o => o.Ucenik)
                .Where(i => i.MaturskiIspitId == ispitId && !i.Pristupio)
                .ToList();
            var list = new List<string>();
            foreach(var u in uceniciNisuPristupili)
            {
                list.Add(u.OdjeljenjeStavka.Ucenik.ImePrezime);
            }
            return list;
        }
        public IActionResult DodajIspit(int nastavnikId)
        {
            var aktualenaGodina = _context.SkolskaGodina
                    .Where(s => s.Aktuelna)
                    .FirstOrDefault();
            var nastavnik = _context.Nastavnik.Find(nastavnikId);
            var model = new DodajIspitVM
            {
                NastavnikIme = nastavnik.Ime + " " + nastavnik.Prezime,
                PredmetList = _context.Predmet.ToList(),
                SkolaList = _context.Skola.ToList(),
                SkolskaGodina = aktualenaGodina.Naziv,
                SkolskaGodinaId = aktualenaGodina.Id
            };
            return View(model);
        }
        [HttpPost]
        public IActionResult DodajIspit(DodajIspitVM model)
        {
            var ispit = new MaturskiIspit
            {
                Datum = model.Datum,
                NastavnikId = model.NastavnikId,
                PredmetId = model.PredmetId,
                SkolaId = model.SkolaId,
                SkolskaGodinaId = model.SkolskaGodinaId,
                Napomena = ""
            };
            _context.Add(ispit);

            var uceniciSviIzSkoleCetvrti = _context.OdjeljenjeStavka
                .Include(o=>o.Odjeljenje)
                .Where(o => o.Odjeljenje.Razred == 4 &&
                    o.Odjeljenje.SkolaID == model.SkolaId)
                .ToList();

            var uceniciZadovoljavajuUslove = GetZadovoljavajuciUcenici(uceniciSviIzSkoleCetvrti, model.PredmetId);
            
            foreach(var u in uceniciZadovoljavajuUslove)
            {
                var stavka = new IspitStavka
                {
                    Bodovi = -1,
                    Pristupio = false,
                    MaturskiIspit = ispit,
                    OdjeljenjeStavkaId = u.Id
                };
                _context.Add(stavka);
            }
            _context.SaveChanges();
            return RedirectToAction(nameof(Nastavnik), new { nastavnikId = model.NastavnikId });
        }

        private List<OdjeljenjeStavka> GetZadovoljavajuciUcenici(List<OdjeljenjeStavka> uceniciSviIzSkoleCetvrti, int predmetId)
        {
            var list = new List<OdjeljenjeStavka>();
            foreach(var x in uceniciSviIzSkoleCetvrti)
            {
                if (ZadovoljioUcenik(x, predmetId))
                {
                    list.Add(x);
                }
            }
            return list;
        }

        private bool ZadovoljioUcenik(OdjeljenjeStavka x, int predmetId)
        {
            var predmetiUcenika = _context.DodjeljenPredmet
                    .Where(d => d.OdjeljenjeStavkaId == x.Id)
                    .ToList();
            foreach (var p in predmetiUcenika)
            {
                if (p.ZakljucnoKrajGodine == 1)
                {
                    return false;
                }
            }

            var maturskiIzPredmeta = _context.MaturskiIspit
                .Where(m => m.PredmetId == predmetId)
                .ToList();
            foreach(var m in maturskiIzPredmeta)
            {
                var polaganiIspiti = _context.IspitStavka
                                .Where(i => i.MaturskiIspit == m)
                                .ToList();
                foreach(var polagani in polaganiIspiti)
                {
                    if (polagani.Bodovi > 55)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        public IActionResult UrediIspit(int ispitId)
        {
            var ispit = _context.MaturskiIspit
                .Include(i=>i.Predmet)
                .Where(i => i.Id == ispitId)
                .FirstOrDefault();
            var model = new UrediIspitVM
            {
                Datum = ispit.Datum.ToString("dd.MM.yyyy"),
                IspitId = ispit.Id,
                Napomena = ispit.Napomena,
                Predmet = ispit.Predmet.Naziv
            };
            return View(model);
        }
        [HttpPost]
        public IActionResult UrediIspit(UrediIspitVM model)
        {
            var ispit = _context.MaturskiIspit
                .Find(model.IspitId)
                .Napomena = model.Napomena;
            _context.SaveChanges();
            return RedirectToAction(nameof(UrediIspit), new { ispitId = model.IspitId });
        }
        public IActionResult GetStavkeIspit(int ispitId)
        {
            var model = new GetStavkeIspitVM
            {
                Rows = _context.IspitStavka
                .Where(i=>i.MaturskiIspitId==ispitId)
                .Select(i=>new GetStavkeIspitVM.Row { 
                    Pristupio = i.Pristupio,
                    Rezultat = i.Bodovi,
                    Ucenik = i.OdjeljenjeStavka.Ucenik.ImePrezime,
                    StavkaId = i.Id
                }).ToList()
            };
            foreach(var r in model.Rows)
            {
                r.ProsjekOcjena = GetProsjekUcenika(r.StavkaId);
            }
            return PartialView("StavkeIspitPartial", model);
        }

        private float GetProsjekUcenika(int odjeljenjeStavkaId)
        {
            var dodjeljenjiPredmetiUcenika = _context.DodjeljenPredmet
                .Where(d => d.OdjeljenjeStavkaId == odjeljenjeStavkaId)
                .ToList();
            float prosjek = 0;
            int count = 0;
            foreach(var d in dodjeljenjiPredmetiUcenika)
            {
                prosjek += d.ZakljucnoKrajGodine;
                count++;
            }
            return prosjek / count;
        }
        public IActionResult TogglePristupio(int stavkaId)
        {
            var stavka = _context.IspitStavka.Find(stavkaId);
            if (stavka.Pristupio)
            {
                stavka.Pristupio = false;
            }
            else
            {
                stavka.Pristupio = true;
            }
            _context.SaveChanges();
            return RedirectToAction(nameof(GetStavkeIspit), new { ispitId = stavka.MaturskiIspitId });
        }
        public IActionResult UrediStavku(int stavkaId)
        {
            var stavka = _context.IspitStavka
                .Include(i => i.OdjeljenjeStavka)
                    .ThenInclude(o => o.Ucenik)
                .Where(i => i.Id == stavkaId)
                .FirstOrDefault();
            var model = new UrediStavkuVM
            {
                Rezultat = stavka.Bodovi,
                StavkaId = stavka.Id,
                Ucenik = stavka.OdjeljenjeStavka.Ucenik.ImePrezime,
                IspitId = stavka.MaturskiIspitId
            };
            return PartialView("UrediStavkuPartial", model);
        }
        [HttpPost]
        public IActionResult UrediStavku(UrediStavkuVM model)
        {
            var stavka = _context.IspitStavka.Find(model.StavkaId);
            if (model.Rezultat < 101)
            {
                stavka.Bodovi = model.Rezultat;
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(GetStavkeIspit), new { ispitId = model.IspitId });
        }
        [HttpPost]
        public IActionResult EditBodovi(UrediQuick model)
        {
            var stavka = _context.IspitStavka.Find(model.StavkaId);
            if (model.Bodovi < 101)
            {
                stavka.Bodovi = model.Bodovi;
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(GetStavkeIspit), new { ispitId = stavka.MaturskiIspitId });
        }
    }
}