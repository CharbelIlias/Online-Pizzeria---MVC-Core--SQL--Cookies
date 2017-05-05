using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ASPInlämningTvå_KarbelIlias.ViewModels;
using ASPInlämningTvå_KarbelIlias.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ASPInlämningTvå_KarbelIlias.Controllers
{
    public class WebShopController : Controller
    {
        private TomasosContext _context;
        public WebShopController(TomasosContext context)
        {
            _context = context;
        }

        // listar alla Maträtter till sidan
        public IActionResult ViewProducts()
        {
            List<MenuItemsVM> ViewModelList = new List<MenuItemsVM>();

            var dishes = _context.Matratt.ToList();

            foreach (var dish in dishes)

            {
                MenuItemsVM model = new MenuItemsVM();

                model.Matratt = dish;

                model.Produkter = _context.MatrattProdukt.Where(x => x.MatrattId == dish.MatrattId).Select(z => z.Produkt).ToList();

                ViewModelList.Add(model);
            }

            return View(ViewModelList);
        }

        // sparar alla valda maträtter i en sessionsvariabel
        public IActionResult AddToCart(int id)
        {
            var matratt = _context.Matratt.SingleOrDefault(x => x.MatrattId == id);

            Bestallning beställning;

            if (HttpContext.Session.GetString("Matratt") == null)
            {
                beställning = new Bestallning() { BestallningMatratt = new List<BestallningMatratt>() };
            }
            else
            {
                var temp = HttpContext.Session.GetString("Matratt");
                beställning = JsonConvert.DeserializeObject<Bestallning>(temp);
            }

            BestallningMatratt bestallningMatratt = new BestallningMatratt();
            bestallningMatratt.Matratt = matratt;
            bestallningMatratt.Antal = 1;
            bestallningMatratt.MatrattId = matratt.MatrattId;

            if (beställning.BestallningMatratt.Any(x => x.MatrattId == matratt.MatrattId))
            {
                beställning.BestallningMatratt.Where(x => x.MatrattId == matratt.MatrattId).First().Antal++;
            }
            else
            {
                beställning.BestallningMatratt.Add(bestallningMatratt);
            }

            var serializedValue = JsonConvert.SerializeObject(beställning);
            HttpContext.Session.SetString("Matratt", serializedValue);

            return PartialView("_CartViewPartial", beställning.BestallningMatratt);
        }

        // Skapa kund
        public IActionResult CreateCustomer()
        {
            Kund valdKund;

            if (HttpContext.Session.GetString("Login") == null)
            {
                return View();
            }
            else
            {
                var temp = HttpContext.Session.GetString("Login");
                valdKund = JsonConvert.DeserializeObject<Kund>(temp);

                return View(valdKund);
            }
        }

        //Sparar ny kund till databasen
        [HttpPost]
        public IActionResult CreateCustomer(Kund kund)
        {
            if (ModelState.IsValid)
            {
                Kund nyKund = new Kund()
                {
                    Namn = kund.Namn,
                    Gatuadress = kund.Gatuadress,
                    Postnr = kund.Postnr,
                    Postort = kund.Postort,
                    Email = kund.Email,
                    Telefon = kund.Telefon,
                    AnvandarNamn = kund.AnvandarNamn,
                    Losenord = kund.Losenord
                };

                _context.Kund.Add(nyKund);
                _context.SaveChanges();

                return RedirectToAction("ViewProducts");
            }

            return View();
        }

        //Visar "logga in"-sidan
        public IActionResult Login()
        {
            return View();
        }

        // Matchar id med databasen, hämtar kunden och skapar en sessionsvariabel för den. Skickar vidare till "Visa Produkter"
        [HttpPost]
        public IActionResult Login(LoginVM VmKund)
        {
            if (ModelState.IsValid)
            {
                var originalKund = _context.Kund.SingleOrDefault(x => x.AnvandarNamn == VmKund.AnvandarNamn && x.Losenord == VmKund.Losenord);

                var serializedValue = JsonConvert.SerializeObject(originalKund);
                HttpContext.Session.SetString("Login", serializedValue);

                if (originalKund != null)
                {
                    return RedirectToAction("ViewProducts");
                }
                else
                {
                    return View();
                }
            }

            return View();
        }

        // Logga ut
        public IActionResult LogOut()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("ViewProducts");
        }

        //Visar mina uppgifter i tabellform
        public IActionResult MyPage()
        {
            Kund valdKund;

            var temp = HttpContext.Session.GetString("Login");
            valdKund = JsonConvert.DeserializeObject<Kund>(temp);

            var serializedValue = JsonConvert.SerializeObject(valdKund);
            HttpContext.Session.SetString("Login", serializedValue);

            return View(valdKund);
        }
        //Visar mina uppgifter i ett formulär, där man kan ändra uppgifterna
        public IActionResult EditMyPage(int id)
        {
            Kund kund = _context.Kund.FirstOrDefault(x => x.KundId == id);

            return View(kund);
        }

        //Sparar ändringarna till databasen
        [HttpPost]
        public IActionResult UpdateCustomer(Kund kund)
        {
            if (ModelState.IsValid)
            {
                Kund originalKund = _context.Kund.FirstOrDefault(x => x.AnvandarNamn == kund.AnvandarNamn);

                originalKund.Namn = kund.Namn;
                originalKund.Postnr = kund.Postnr;
                originalKund.Postort = kund.Postort;
                originalKund.Telefon = kund.Telefon;
                originalKund.Gatuadress = kund.Gatuadress;
                originalKund.AnvandarNamn = kund.AnvandarNamn;
                originalKund.Losenord = kund.Losenord;
                originalKund.Email = kund.Email;

                _context.Kund.Update(originalKund);
                _context.SaveChanges();

                var temp = HttpContext.Session.GetString("Login");
                var sessionKund = JsonConvert.DeserializeObject<Kund>(temp);

                sessionKund = originalKund;

                var serializedValue = JsonConvert.SerializeObject(sessionKund);
                HttpContext.Session.SetString("Login", serializedValue);

                return RedirectToAction("MyPage", kund);
            }
            return View("EditMyPage", kund);
        }

        //Varukorgen, visar alla maträtter, pris, antal och totalbelopp
        public IActionResult CheckOut()
        {
            var temp = HttpContext.Session.GetString("Matratt");
            var beställning = JsonConvert.DeserializeObject<Bestallning>(temp);

            return View(beställning.BestallningMatratt);
        }
        //Köp knappen. Om man inte är inloggad skickas man till en speciell login annars skickas man till Ordersammanställningen.
        public IActionResult PayForOrder()
        {
            if (HttpContext.Session.GetString("Login") == null)
            {
                return View("LoginFromCheckOut");
            }
            else
            {
                return RedirectToAction("ShowOrder");
            }
        }
        //Hämtar sessionsvariablerna, lägger in all data till objekten och sparar ner till databasen, sedan visar den en vy med en bekräftelse.
        public IActionResult ShowOrder()
        {
            Kund sessionsKund = new Kund();

            var tempMatratt = HttpContext.Session.GetString("Matratt");
            var beställning = JsonConvert.DeserializeObject<Bestallning>(tempMatratt);

            var tempLogin = HttpContext.Session.GetString("Login");
            var Kund = JsonConvert.DeserializeObject<Kund>(tempLogin);

            sessionsKund = Kund;

            Bestallning nyBeställning = new Bestallning();

            BestallningMatratt nyBeställningMaträtt = new BestallningMatratt();

            nyBeställning.BestallningDatum = DateTime.Now;
            nyBeställning.Totalbelopp = beställning.BestallningMatratt.Sum(x => x.Antal * x.Matratt.Pris);
            nyBeställning.KundId = sessionsKund.KundId;

            _context.Bestallning.Add(nyBeställning);
            _context.SaveChanges();

            foreach (var item in beställning.BestallningMatratt)
            {
                nyBeställningMaträtt.MatrattId = item.MatrattId;
                nyBeställningMaträtt.Antal = item.Antal;
                nyBeställningMaträtt.Bestallning = nyBeställning;
                _context.BestallningMatratt.Add(nyBeställningMaträtt);
                _context.SaveChanges();
            }

            OrderVM orderVM = new OrderVM();

            orderVM.kund = sessionsKund;

            orderVM.beställning = beställning;

            return View(orderVM);
        }

        // En ny Login-Vy för att jag vill skicka kunden tillbaka till Kassan efter login.
        public IActionResult LoginFromCheckOut()
        {
            return View();
        }

        [HttpPost]
        public IActionResult LoginFromCheckOut(LoginVM VmKund)
        {
            if (ModelState.IsValid)
            {
                var originalKund = _context.Kund.SingleOrDefault(x => x.AnvandarNamn == VmKund.AnvandarNamn && x.Losenord == VmKund.Losenord);

                var serializedValue = JsonConvert.SerializeObject(originalKund);
                HttpContext.Session.SetString("Login", serializedValue);

                if (originalKund != null)
                {
                    var temp = HttpContext.Session.GetString("Matratt");
                    var beställning = JsonConvert.DeserializeObject<Bestallning>(temp);

                    return RedirectToAction("CheckOut", beställning.BestallningMatratt);
                }
                else
                {
                    return View();
                }
            }

            return View();
        }

    }
}
