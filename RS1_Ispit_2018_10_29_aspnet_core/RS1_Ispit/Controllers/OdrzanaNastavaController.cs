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
            var model = new NastavaIndexVM
            {
                Rows = _context.Nastavnik.Select(n => new NastavaIndexVM.Row
                {
                    Id = n.Id,
                    ImePrezime = n.Ime + " " + n.Prezime,
                    Skola = n.Skola.Naziv
                }).ToList()
            };
            return View(model);
        }
        public IActionResult Casovi(int nastavnikId)
        {
            var model = new NastavaCasoviVM
            {
                NastavnikId = nastavnikId,
                Rows = _context.Cas
                    .Where(c => c.PredajePredmet.NastavnikID == nastavnikId)
                    .Select(c => new NastavaCasoviVM.Row
                    {
                        CasId = c.Id,
                        DatumCasa = c.Datum.ToString("dd.MM.yyyy"),
                        GodinaOdjeljenje = c.PredajePredmet.Odjeljenje.SkolskaGodina.Naziv + " " + c.PredajePredmet.Odjeljenje.Razred + "-" + c.PredajePredmet.Odjeljenje.Oznaka,
                        Predmet = c.PredajePredmet.Predmet.Naziv
                    })
                    .ToList()
            };
            foreach (var r in model.Rows)
            {
                r.OdsutniUcenici = GetOdsutniUcenici(r.CasId);
            }
            return View(model);
        }

        public IActionResult DodajCas(int nastavnikId)
        {
            var nastavnik = _context.Nastavnik.Find(nastavnikId);
            var model = new DodajCasVM
            {
                Nastavnik = nastavnik.Ime + " " + nastavnik.Prezime,
                PredajePredmetOdjeljenjeList = _context.PredajePredmet
                    .Include(p => p.Odjeljenje)
                    .Include(p => p.Predmet)
                    .Where(p => p.NastavnikID == nastavnikId)
                    .ToList()
            };
            return View(model);
        }
        [HttpPost]
        public IActionResult DodajCas(DodajCasVM model)
        {
            var cas = new Cas
            {
                Datum = model.Datum,
                PredajePredmetId = model.PredajePredmetId,
                Sadrzaj = ""
            };
            _context.Add(cas);

            var predajePredmet = _context.PredajePredmet.Find(model.PredajePredmetId);
            var ucenici = _context.OdjeljenjeStavka
                .Where(o => o.OdjeljenjeId == predajePredmet.OdjeljenjeID)
                .ToList();

            foreach (var u in ucenici)
            {
                var stavka = new CasStavka
                {
                    Cas = cas,
                    Napomena = "",
                    Ocjena = -1,
                    Prisutan = false,
                    OpravdanoOdsutan = false,
                    OdjeljenjeStavkaId = u.Id
                };
                _context.Add(stavka);
            }
            _context.SaveChanges();
            return RedirectToAction(nameof(Casovi), new { nastavnikId = predajePredmet.NastavnikID });
        }

        public IActionResult ObrisiCas(int casId)
        {
            var stavke = _context.CasStavka
                .Where(c => c.CasId == casId)
                .ToList();
            foreach (var s in stavke)
            {
                _context.Remove(s);
            }
            _context.SaveChanges();
            var cas = _context.Cas
                .Include(c => c.PredajePredmet)
                .Where(c => c.Id == casId)
                .FirstOrDefault();
            var nId = cas.PredajePredmet.NastavnikID;
            _context.Remove(cas);
            _context.SaveChanges();
            return RedirectToAction(nameof(Casovi), new { nastavnikId = nId });
        }

        public IActionResult EditCas(int casId)
        {
            var cas = _context.Cas
                .Include(c => c.PredajePredmet)
                    .ThenInclude(p => p.Predmet)
                .Include(c => c.PredajePredmet)
                    .ThenInclude(p => p.Odjeljenje)
                .Where(c => c.Id == casId)
                .FirstOrDefault();
            var model = new EditCasVM
            {
                CasId = cas.Id,
                Datum = cas.Datum.ToString("dd.MM.yyyy"),
                Odjeljenje = cas.PredajePredmet.Odjeljenje.Razred + " " + cas.PredajePredmet.Odjeljenje.Oznaka + " " + cas.PredajePredmet.Predmet.Naziv,
                Sadrzaj = cas.Sadrzaj
            };
            return View(model);
        }
        [HttpPost]
        public IActionResult EditCas(EditCasVM model)
        {
            var cas = _context.Cas.Find(model.CasId);
            cas.Sadrzaj = model.Sadrzaj;
            _context.SaveChanges();
            return RedirectToAction(nameof(EditCas), new { casId = model.CasId });
        }

        public IActionResult GetStavkeCas(int casId)
        {
            var model = new StavkePartialVM
            {
                Rows = _context.CasStavka
                .Where(c=>c.CasId==casId)
                .Select(c=>new StavkePartialVM.Row { 
                    ImePrezime = c.OdjeljenjeStavka.Ucenik.ImePrezime,
                    Ocjena = c.Ocjena,
                    OpravdanoOdsutan = c.OpravdanoOdsutan,
                    Prisutan = c.Prisutan,
                    StavkaId = c.Id
                }).ToList()
            };
            return PartialView("StavkePartial", model);
        }
        public IActionResult TogglePrisutan(int stavkaId)
        {
            var stavka = _context.CasStavka.Find(stavkaId);
            if (stavka.Prisutan)
            {
                stavka.Prisutan = false;
            }
            else
            {
                stavka.Prisutan = true;
            }
            _context.SaveChanges();
            return RedirectToAction(nameof(GetStavkeCas), new { casId = stavka.CasId });
        }
        public IActionResult EditStavka(int stavkaId)
        {
            var stavka = _context.CasStavka
                .Include(c=>c.OdjeljenjeStavka)
                    .ThenInclude(o=>o.Ucenik)
                .Where(c => c.Id == stavkaId)
                .FirstOrDefault();
            var model = new EditStavkaVM
            {
                Napomena = stavka.Napomena,
                Ocjena = stavka.Ocjena,
                Prisutan = stavka.Prisutan,
                OpravdanoOdsutan = stavka.OpravdanoOdsutan,
                StavkaId = stavka.Id,
                UcenikIme = stavka.OdjeljenjeStavka.Ucenik.ImePrezime
            };
            return PartialView("EditStavkaPartial",model);
        }
        [HttpPost]
        public IActionResult EditStavka(EditStavkaVM model)
        {
            var stavka = _context.CasStavka.Find(model.StavkaId);
            stavka.Napomena = model.Napomena;
            stavka.Ocjena = model.Ocjena;
            if (stavka.Ocjena == 0) stavka.Ocjena = -1;
            stavka.OpravdanoOdsutan = model.OpravdanoOdsutan;
            _context.SaveChanges();
            return RedirectToAction(nameof(GetStavkeCas), new { casId = stavka.CasId });
        }

        private List<string> GetOdsutniUcenici(int casId)
        {
            var odsutniUcenici = _context.CasStavka
                .Include(c => c.OdjeljenjeStavka)
                    .ThenInclude(o => o.Ucenik)
                .Where(c => c.CasId == casId && c.Prisutan == false)
                .ToList();
            var list = new List<string>();
            foreach (var o in odsutniUcenici)
            {
                list.Add(o.OdjeljenjeStavka.Ucenik.ImePrezime);
            }
            return list;
        }
    }
}