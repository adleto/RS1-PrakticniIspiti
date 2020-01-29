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
    public class IspitniTerminiController : Controller
    {
        private readonly MojContext _context;

        public IspitniTerminiController(MojContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var model = new IndexVM
            {
                Rows = new List<IndexVM.MainRow>()
            };
            var angazovanList = _context.Angazovan
                .Include(a => a.AkademskaGodina)
                .Include(a => a.Nastavnik)
                .Include(a => a.Predmet)
                .ToList();
            var razlicitiPredmeti = new List<Predmet>();
            foreach(var a in angazovanList)
            {
                if (!razlicitiPredmeti.Contains(a.Predmet))
                {
                    razlicitiPredmeti.Add(a.Predmet);
                }
            }

            foreach(var predmet in razlicitiPredmeti)
            {
                var mainRow = new IndexVM.MainRow
                {
                    NazivPredmeta = predmet.Naziv
                };
                mainRow.InsideRows = new List<IndexVM.MainRow.Row>();
                foreach (var a in angazovanList)
                {
                    if(a.Predmet == predmet)
                    {
                        var insideRow = new IndexVM.MainRow.Row
                        {
                            AngazovanId = a.Id,
                            Nastavnik = a.Nastavnik.Ime + " " + a.Nastavnik.Prezime,
                            SkolskaGodina = a.AkademskaGodina.Opis,
                            OdrzanoCasova = _context.OdrzaniCas
                                .Where(oc=>oc.Angazovani == a)
                                .Count(),
                            StudenataNaPredmetu = _context.SlusaPredmet
                                .Where(sp=>sp.Angazovan == a)
                                .Count()
                        };
                        mainRow.InsideRows.Add(insideRow);
                    }
                }
                model.Rows.Add(mainRow);
            }
            
            return View(model);
        }

        public IActionResult Ispiti(int angazovanId)
        {
            var angazovan = _context.Angazovan
                .Include(a=>a.AkademskaGodina)
                .Include(a=>a.Nastavnik)
                .Include(a=>a.Predmet)
                .Where(a => a.Id == angazovanId)
                .FirstOrDefault();
            var model = new IspitiVM
            {
                AkademskaGodina = angazovan.AkademskaGodina.Opis,
                AngazovanId = angazovanId,
                Nastavnik = angazovan.Nastavnik.Ime +" " + angazovan.Nastavnik.Prezime,
                Predmet = angazovan.Predmet.Naziv,
                Rows = _context.Ispit
                    .Where(i=>i.Angazovan == angazovan)
                    .Select(i=>new IspitiVM.Row { 
                        Datum = i.Datum.ToString("dd.MM.yyyy"),
                        IspitId = i.Id,
                        Zakljucano = i.Zakljucano
                    })
                    .ToList()
            };
            foreach(var r in model.Rows)
            {
                r.BrojPrijavljenihStudenata = _context.IspitStavka
                    .Where(s => s.IspitId == r.IspitId)
                    .Count();
                r.BrojStudenataKojiNisuPolozili = _context.IspitStavka
                    .Where(s => s.IspitId == r.IspitId && s.Ocjena < 6)
                    .Count(); //Ovdje možda treba brojati i sve studente 
                              //koji slusaju predmet a ne samo ove na listi za ispit?

                //Alternativno za BrojStudenataKojiNisuPolozili
                r.BrojStudenataKojiNisuPolozili = GetNisuPolozili(r.IspitId);
            }

            return View(model);
        }

        private int GetNisuPolozili(int ispitId)
        {
            var ispit = _context.Ispit.Find(ispitId);
            var polozili = _context.IspitStavka
                .Where(i => i.IspitId == ispitId
                    && i.Ocjena > 5)
                .Count();
            var ukupnoSlusaPredmet = _context.SlusaPredmet
                .Where(sp => sp.AngazovanId == ispit.AngazovanId)
                .Count();

            //therefor nisu polozili jednako ukupno - oni koji su polozili
            return ukupnoSlusaPredmet - polozili;
        }
        public IActionResult Zakljucaj(int ispitId)
        {
            var ispit = _context.Ispit.Find(ispitId);
            ispit.Zakljucano = true;
            _context.SaveChanges();
            return RedirectToAction(nameof(Ispiti), new { angazovanId = ispit.AngazovanId });
        }
        public IActionResult DodajIspit(int angazovanId)
        {
            var angazovan = _context.Angazovan
                .Include(a => a.AkademskaGodina)
                .Include(a => a.Nastavnik)
                .Include(a => a.Predmet)
                .Where(a => a.Id == angazovanId)
                .FirstOrDefault();
            var model = new DodajIspitVM { 
                AngazovanId = angazovanId,
                Nastavnik = angazovan.Nastavnik.Ime + " " + angazovan.Nastavnik.Prezime,
                Predmet = angazovan.Predmet.Naziv,
                SkolskaGodina = angazovan.AkademskaGodina.Opis
            };
            return View(model);
        }
        [HttpPost]
        public IActionResult DodajIspit(DodajIspitVM model)
        {
            var noviIspit = new Ispit { 
                AngazovanId = model.AngazovanId,
                Datum = model.Datum,
                Napomena = model.Napomena,
                Zakljucano = false
            };
            _context.Add(noviIspit);
            _context.SaveChanges();
            return RedirectToAction(nameof(Ispiti), new { angazovanId = model.AngazovanId });
        }

        public IActionResult Detalji(int ispitId)
        {
            var ispit = _context.Ispit.Find(ispitId);
            var angazovan = _context.Angazovan
                .Include(a => a.AkademskaGodina)
                .Include(a => a.Nastavnik)
                .Include(a => a.Predmet)
                .Where(a => a.Id == ispit.AngazovanId)
                .FirstOrDefault();
            var model = new DodajIspitVM
            {
                AngazovanId = ispit.AngazovanId,
                Nastavnik = angazovan.Nastavnik.Ime + " " + angazovan.Nastavnik.Prezime,
                Predmet = angazovan.Predmet.Naziv,
                SkolskaGodina = angazovan.AkademskaGodina.Opis,
                IspitId = ispitId,
                Datum = ispit.Datum,
                Napomena = ispit.Napomena
            };
            return View(model);
        }
        public IActionResult GetStavke(int ispitId)
        {
            var ispit = _context.Ispit.Find(ispitId);
            var model = new GetStavkeVM
            {
                Datum = ispit.Datum,
                Zakljucan = ispit.Zakljucano,
                IspitId = ispitId,
                Rows = _context.IspitStavka
                    .Where(i=>i.IspitId == ispitId)
                    .Select(i=>new GetStavkeVM.Row { 
                    Ocjena = i.Ocjena,
                    Pristupio = i.Pristupio,
                    StavkaId = i.Id,
                    Student = i.SlusaPredmet.UpisGodine.Student.Ime +" " + i.SlusaPredmet.UpisGodine.Student.Prezime
                }).ToList()
            };
            return PartialView(model);
        }
        public IActionResult PristupioToggle(int stavkaId)
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
            return RedirectToAction(nameof(GetStavke), new { ispitId = stavka.IspitId });
        }
        public IActionResult UrediStavku(int stavkaId)
        {
            var stavka = _context.IspitStavka
                .Include(i=>i.SlusaPredmet)
                    .ThenInclude(s => s.UpisGodine)
                        .ThenInclude(u => u.Student)
                .Where(s=>s.Id == stavkaId)
                .FirstOrDefault();
            var model = new UrediDodajStavkaVM
            {
                Student = stavka.SlusaPredmet.UpisGodine.Student.Ime + " " + stavka.SlusaPredmet.UpisGodine.Student.Prezime,
                StavkaId = stavkaId,
                Ocjena = stavka.Ocjena
            };
            return PartialView(model);
        }
        [HttpPost]
        public IActionResult UrediStavku(UrediDodajStavkaVM model)
        {
            var stavka = _context.IspitStavka.Find(model.StavkaId);
            stavka.Ocjena = model.Ocjena;
            _context.SaveChanges();
            return RedirectToAction(nameof(GetStavke), new { ispitId = stavka.IspitId });
        }
        public IActionResult DodajStavku(int ispitId)
        {
            var ispit = _context.Ispit.Find(ispitId);
            var sviNaPredmetu = _context.SlusaPredmet
                    .Include(s=>s.UpisGodine)
                        .ThenInclude(u=>u.Student)
                    .Where(s => s.AngazovanId == ispit.AngazovanId)
                    .ToList();
            var vecPrijavljeni = _context.IspitStavka
                .Include(i=>i.SlusaPredmet)
                .Where(i => i.IspitId == ispitId)
                .ToList();
            foreach(var p in vecPrijavljeni)
            {
                if (sviNaPredmetu.Contains(p.SlusaPredmet))
                {
                    sviNaPredmetu.Remove(p.SlusaPredmet);
                }
            }
            var model = new UrediDodajStavkaVM
            {
                IspitId = ispitId,
                SlusaPredmetList = sviNaPredmetu
            };
            return PartialView(model);
        }
        [HttpPost]
        public IActionResult DodajStavku(UrediDodajStavkaVM model)
        {
            var novaStavka = new IspitStavka
            {
                IspitId = model.IspitId,
                SlusaPredmetId = model.SlusaPredmetId,
                Pristupio = false,
                Ocjena = -1
            };
            _context.Add(novaStavka);
            _context.SaveChanges();
            return RedirectToAction(nameof(GetStavke), new { ispitId = model.IspitId });
        }
        [HttpPost]
        public IActionResult Ocjena(int StavkaId, int Ocjena)
        {
            var stavka = _context.IspitStavka.Find(StavkaId);
            stavka.Ocjena = Ocjena;
            _context.SaveChanges();
            return RedirectToAction(nameof(GetStavke), new { ispitId = stavka.IspitId });
        }
    }
}