using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MunkasokBeolvasas.Data;
using MunkasokBeolvasas.Models;

namespace MunkasokBeolvasas.Controllers
{
    public class WorkerController : Controller
    {
        // A WorkerDbContext a munkások adatainak adatbázisba történő elérését kezeli.
        private readonly WorkerDbContext _context; // Adatbázis kontextus mező hozzáadása

        // Konstruktor a WorkerController osztályhoz
        public WorkerController(WorkerDbContext context)
        {
            this._context = context;
        }

        // A fájl feltöltéséért felelős nézet megjelenítése
        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }

        // A fájl beolvasását végző művelet
        [HttpPost]
        public async Task<IActionResult> ReadFile()
        {
            var file = Request.Form.Files[0]; // A fájl lekérése a HTTP requestből

            // Feltöltött fájl feldolgozása és dolgozók mentése az adatbázisba
            if (file != null && file.Length > 0)
            {
                if (Path.GetExtension(file.FileName) != ".json")
                {
                    // Amennyiben nem json kiterjesztésű, hibát dob a program
                    ViewBag.ErrorMessage = "Érvénytelen fájlformátum. Kérjük, válasszon .json fájlt.";
                    return View("Upload");
                }
                // MemoryStream létrehozása, hogy a fálj tartalmát eltárolhassuk a memóriában
                using (var memoryStream = new MemoryStream())
                {
                    // Fájl másolása a memóriába
                    await file.CopyToAsync(memoryStream);

                    // A stream pozicionálása az elejére
                    memoryStream.Position = 0;

                    //WorkerRepositoryból meghívjuk a ReadFromJson metódust
                    // ezáltal kinyerjük a dolgozókat a feltöltött JSON fáljból 
                    List<Worker> workers = WorkerRepository.ReadFromJson(memoryStream);

                    if (workers == null || workers.Count == 0)
                    {
                        ViewBag.ErrorMessage = "Nincs adat a fájlban.";
                        return View("Upload");
                    }

                    // Munkások mentése az adatbázisba
                    foreach (var worker in workers)
                    {
                        // A program ellenőrzi, hogy amennyiben van már egy dolgozó ugyanazzal a vezetéknévvel
                        // abban az esetben ne mentse az adatbázisba, hogy ne legyen duplikátum
                        var existingWorker = await _context.Workers.FirstOrDefaultAsync(w => w.LastName == worker.LastName);
                        if (existingWorker == null)
                        {
                            _context.Workers.Add(worker);
                        }
                    }
                    await _context.SaveChangesAsync();
                }
            }

            // Átirányítás a List oldalra a dolgozók listázáshoz
            return RedirectToAction("List");
        }

        // Statisztikák megjelenítése
        public async Task<IActionResult> Statistics()
        {
            // Szükséges statisztikák lekérése
            var averageSalary = await _context.Workers.AverageAsync(w => w.Salary);
            var averageAge = await _context.Workers.AverageAsync(w => w.Age);
            var youngestWorker = await _context.Workers.OrderBy(w => w.Age).FirstOrDefaultAsync();
            var oldestWorker = await _context.Workers.OrderByDescending(w => w.Age).FirstOrDefaultAsync();
            var workerWithMostYears = await _context.Workers.OrderByDescending(w => w.EmploymentDuration).FirstOrDefaultAsync();

            // ViewBag feltöltése a statisztikákkal
            ViewBag.AverageSalary = averageSalary;
            ViewBag.AverageAge = averageAge;
            ViewBag.YoungestWorker = youngestWorker;
            ViewBag.OldestWorker = oldestWorker;
            ViewBag.WorkerWithMostYears = workerWithMostYears;

            // Az átlagfizetés feletti munkások lekérése
            var workersAboveAverageSalary = await _context.Workers.Where(w => w.Salary > averageSalary).ToListAsync();

            // Az átlagéletkornál fiatalabb munkások lekérése
            var workersBelowAverageAge = await _context.Workers.Where(w => w.Age < averageAge).ToListAsync();

            // A ViewBaghoz szintén hozzáadjuk ezeket az eredményeket
            ViewBag.WorkersAboveAverageSalary = workersAboveAverageSalary;
            ViewBag.WorkersBelowAverageAge = workersBelowAverageAge;

            return View("Statistics");
        }

        // Dolgozók listájának megjelenítése
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var workers = await _context.Workers.ToListAsync();
            return View(workers); // A dolgozók listájának átadása a nézetnek
        }

        // Munkás szerkesztése
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            // Id alapján keresés
            var worker = await _context.Workers.FindAsync(id);

            return View(worker);
        }

        // Munkás szerkesztése - adatok mentése
        [HttpPost]
        public async Task<IActionResult> Edit(Worker viewModel)
        {
            var worker = await _context.Workers.FindAsync(viewModel.Id);
            //Amennyiben megtalálható a dolgozó id-je, szerkeszthető lesz
            if (worker is not null)
            {
                worker.LastName = viewModel.LastName;
                worker.Age = viewModel.Age;
                worker.Salary = viewModel.Salary;
                worker.EmploymentDuration = viewModel.EmploymentDuration;

                await _context.SaveChangesAsync();
            }
            return RedirectToAction("List", "Worker");
        }

    }
}