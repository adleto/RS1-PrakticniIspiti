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
    public class PredmetController : Controller
    {
        private readonly MojContext _context;

        public PredmetController(MojContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var model = new PredmetIndexVM
            {
                Rows = _context.Angazovan.Select(a => new PredmetIndexVM.Row
                {
                    Id = a.Id,
                    NastavnikImePrezime = a.Nastavnik.Ime + " " + a.Nastavnik.Prezime,
                    SkolaskaGodina = a.AkademskaGodina.Opis,
                    NazivPredmeta = a.Predmet.Naziv
                }).ToList()
            };
            foreach (var r in model.Rows)
            {
                r.BrojOdrzanihCasova = _context.OdrzaniCas
                    .Where(o => o.AngazovaniId == r.Id)
                    .Count();
                r.BrojStudenata = _context.SlusaPredmet
                    .Where(s => s.AngazovanId == r.Id)
                    .Count();
            }
            return View(model);
        }
        public IActionResult Predmet(int predmetId)
        {
            var angazovan = _context.Angazovan
                .Include(a => a.AkademskaGodina)
                .Include(a => a.Nastavnik)
                .Include(a => a.Predmet)
                .Where(a => a.Id == predmetId)
                .FirstOrDefault();
            var model = new PredmetVM
            {
                AkademskaGodina = angazovan.AkademskaGodina.Opis,
                NastavnikImePreziem = angazovan.Nastavnik.Ime + " " + angazovan.Nastavnik.Prezime,
                PredmetNaziv = angazovan.Predmet.Naziv,
                Id = angazovan.Id,
                Rows = _context.Ispit
                    .Where(i => i.AngazovanId == angazovan.Id)
                    .Select(i => new PredmetVM.Row
                    {
                        DatumIspita = i.Datum.ToString("dd.MM.yyyy"),
                        Id = i.Id,
                        Zakljucano = i.Zakljucano
                    }).ToList()
            };
            foreach (var r in model.Rows)
            {
                r.BrojPrijavljenihStudenata = GetBrojPrijavljenih(r.Id);
                r.BrojStudenataKojiNisuPolozili = GetBrojNisuPolozili(r.Id);
            }
            return View(model);
        }
        public IActionResult Zakljucaj(int ispitId)
        {
            var i = _context.Ispit.Find(ispitId);
            i.Zakljucano = true;
            _context.SaveChanges();
            return RedirectToAction(nameof(Predmet), new { predmetId = i.AngazovanId });
        }

        public IActionResult Dodaj(int predmetId)
        {
            var angazovan = _context.Angazovan
                .Include(a => a.AkademskaGodina)
                .Include(a => a.Nastavnik)
                .Include(a => a.Predmet)
                .Where(a => a.Id == predmetId)
                .FirstOrDefault();
            var model = new DodajVM
            {
                AngazovanId = predmetId,
                Nastavnik = angazovan.Nastavnik.Ime + " " + angazovan.Nastavnik.Prezime,
                Predmet = angazovan.Predmet.Naziv,
                SkolskaGodina = angazovan.AkademskaGodina.Opis
            };
            return View(model);
        }
        [HttpPost]
        public IActionResult Dodaj(DodajVM model)
        {
            var ispit = new Ispit
            {
                AngazovanId = model.AngazovanId,
                Datum = model.Datum,
                Zakljucano = false,
                Napomena = model.Napomena
            };
            _context.Add(ispit);
            _context.SaveChanges();
            var studentiKojiSlusajuPredmet = _context.SlusaPredmet
                .Where(s => s.AngazovanId == ispit.AngazovanId)
                .ToList();
            foreach (var s in studentiKojiSlusajuPredmet)
            {
                var stavka = new StudentIspit
                {
                    Ispit = ispit,
                    Ocjena = -1,
                    PristupioIspitu = false,
                    SlusaPredmetId = s.Id
                };
                _context.Add(stavka);
            }
            _context.SaveChanges();
            return RedirectToAction(nameof(Predmet), new { predmetId = model.AngazovanId });
        }
        public IActionResult Detalji(int ispitId)
        {
            var model = new DetaljiVM
            {
                Ispit = _context.Ispit
                    .Include(i => i.Angazovan)
                            .ThenInclude(a => a.Nastavnik)
                    .Include(i => i.Angazovan)
                            .ThenInclude(a => a.Predmet)
                    .Include(i => i.Angazovan)
                            .ThenInclude(a => a.AkademskaGodina)
                    .Where(i => i.Id == ispitId).FirstOrDefault()
            };
            return View(model);
        }
        public IActionResult GetStudentIspit(int ispitId)
        {
            var model = new StudentIspitPartialVM
            {
                StudentIspitList = _context.StudentIspit
                    .Include(si=>si.SlusaPredmet)
                        .ThenInclude(s=>s.UpisGodine)
                            .ThenInclude(g=>g.Student)
                    .Include(si=>si.Ispit)
                    .Where(si => si.IspitId == ispitId)
                    .ToList()
            };
            return PartialView("StudentIspitPartial", model);
        }
        public IActionResult TogglePristupio(int id)
        {
            var stavka = _context.StudentIspit.Find(id);
            if (stavka.PristupioIspitu)
            {
                stavka.PristupioIspitu = false;
            }
            else
            {
                stavka.PristupioIspitu = true;
            }
            _context.SaveChanges();
            return RedirectToAction(nameof(GetStudentIspit), new { ispitId = stavka.IspitId});
        }

        private int GetBrojNisuPolozili(int id)
        {
            return _context.StudentIspit
                .Where(s => s.IspitId == id && s.Ocjena < 6)
                .Count();
        }

        private int GetBrojPrijavljenih(int id)
        {
            return _context.StudentIspit
                .Where(s => s.IspitId == id)
                .Count();
        }
    }
}