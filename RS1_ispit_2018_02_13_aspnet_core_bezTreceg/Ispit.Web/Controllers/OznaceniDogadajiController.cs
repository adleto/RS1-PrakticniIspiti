using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eUniverzitet.Web.Helper;
using Ispit.Web.Helper;
using Microsoft.AspNetCore.Mvc;
using Ispit.Data;
using Ispit.Data.EntityModels;
using Microsoft.EntityFrameworkCore;
using Ispit.Web.ViewModels;

namespace Ispit.Web.Controllers
{
    [Autorizacija]
    public class OznaceniDogadajiController : Controller
    {
        private readonly MyContext _context;

        public OznaceniDogadajiController(MyContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var model = new IndexVM
            {
                NeoznaceniRows = _context.Dogadjaj.Select(d=>new IndexVM.NeoznaceniRow { 
                    Datum = d.DatumOdrzavanja,
                    Opis = d.Opis,
                    Nastavnik = d.Nastavnik.ImePrezime,
                    DogadjajId = d.ID
                }).ToList(),
                OznaceniRows = _context.OznacenDogadjaj
                .Where(d=>d.Student.KorisnickiNalogId == Autentifikacija.GetLogiraniKorisnik(Request.HttpContext).Id)
                .Select(d=>new IndexVM.OznaceniRow { 
                    Datum = d.Dogadjaj.DatumOdrzavanja,
                    Nastavnik = d.Dogadjaj.Nastavnik.ImePrezime,
                    Opis = d.Dogadjaj.Opis,
                    DogadjajId = d.DogadjajID,
                    OznaceniId = d.ID
                }).ToList()
            };
            foreach(var n in model.NeoznaceniRows)
            {
                n.BrojObaveza = GetBrojObaveza(n.DogadjajId);
            }
            foreach (var n in model.OznaceniRows)
            {
                n.RealizovanoObaveza = GetRealizovanoObaveza(n.OznaceniId);
            }

            return View(model);
        }

        private float GetRealizovanoObaveza(int oznaceniId)
        {
            var dogadjaj = _context.OznacenDogadjaj
                .Where(o => o.DogadjajID == oznaceniId)
                .FirstOrDefault()
                .Dogadjaj;
            var obavezeNaDogadjajuStanje = _context.StanjeObaveze
                .Where(o => o.OznacenDogadjajID == oznaceniId)
                .ToList();
            var brojObavezaNaDogadjaju = obavezeNaDogadjajuStanje.Count();
            float realizovano = 0;

            foreach(var o in obavezeNaDogadjajuStanje)
            {
                float stanjeOveObaveze = o.IzvrsenoProcentualno;
                realizovano += (stanjeOveObaveze*100 / brojObavezaNaDogadjaju);
            }
            return realizovano;
        }

        private int GetBrojObaveza(int dogadjajId)
        {
            return _context.Obaveza
                .Where(d => d.DogadjajID == dogadjajId)
                .Count();
        }
        public IActionResult OznaciDogadjaj(int dogadjajId)
        {
            var korisnickiNalog = Autentifikacija.GetLogiraniKorisnik(HttpContext);
            var student = _context.Student
                .Where(s => s.KorisnickiNalogId == korisnickiNalog.Id)
                .FirstOrDefault();
            var oznaceniDogadjaj = new OznacenDogadjaj
            {
                DatumDodavanja = DateTime.Now,
                DogadjajID = dogadjajId,
                StudentID = student.ID
            };
            _context.Add(oznaceniDogadjaj);

            var obavezeDogadjaja = _context.Obaveza
                .Where(o => o.DogadjajID == dogadjajId)
                .ToList();
            foreach(var o in obavezeDogadjaja)
            {
                var novoStanje = new StanjeObaveze
                {
                    IsZavrseno = false,
                    IzvrsenoProcentualno = 0,
                    ObavezaID = o.ID,
                    OznacenDogadjaj = oznaceniDogadjaj,
                    NotifikacijaDanaPrije = o.NotifikacijaDanaPrijeDefault,
                    NotifikacijeRekurizivno = o.NotifikacijeRekurizivnoDefault
                };
                _context.Add(novoStanje);
            }
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Detalji(int oznakaId)
        {
            var oznaceni = _context.OznacenDogadjaj
                .Include(o => o.Dogadjaj)
                    .ThenInclude(d=>d.Nastavnik)
                .Where(o => o.ID == oznakaId)
                .FirstOrDefault();
            var model = new DetaljiVM
            {
                DatumDodavanja = oznaceni.DatumDodavanja.ToString("dd.MM.yyyy"),
                DatumDogadjaja = oznaceni.Dogadjaj.DatumOdrzavanja.ToString("dd.MM.yyyy"),
                Natavnik = oznaceni.Dogadjaj.Nastavnik.ImePrezime,
                OznacenoId = oznaceni.ID
            };
            return View(model);
        }
        public IActionResult GetObaveze(int oznaceniId)
        {
            var model = new GetObavezeVM
            {
                Rows = _context.StanjeObaveze
                    .Where(s=>s.OznacenDogadjajID == oznaceniId)
                    .Select(s=>new GetObavezeVM.Row { 
                        IdStanjeObaveze = s.Id,
                        Naziv = s.Obaveza.Naziv,
                        ProcentualnoStanje = (s.IzvrsenoProcentualno*100).ToString("F")+"%",
                        PonavljajNotifikaciju = s.NotifikacijeRekurizivno,
                        SaljiNotifikacijuDana = s.NotifikacijaDanaPrije
                    })
                    .ToList()
            };
            return PartialView("ObavezePartial", model);
        }
        public IActionResult Uredi(int stanjeId)
        {
            var stanje = _context.StanjeObaveze
                .Include(s => s.Obaveza)
                .Where(s => s.Id == stanjeId)
                .FirstOrDefault();
            var model = new UrediVM
            {
                Id = stanjeId,
                Naziv = stanje.Obaveza.Naziv,
                Procentualno = stanje.IzvrsenoProcentualno
            };
            return PartialView("UrediPartial", model);
        }
        [HttpPost]
        public IActionResult Uredi(UrediVM model)
        {
            var stanje = _context.StanjeObaveze
                .Include(s => s.Obaveza)
                .Where(s => s.Id == model.Id)
                .FirstOrDefault();
            stanje.IzvrsenoProcentualno = model.Procentualno / 100;
            _context.SaveChanges();
            return RedirectToAction(nameof(GetObaveze), new { oznaceniId = stanje.OznacenDogadjajID });
        }
    }
}