using Microsoft.EntityFrameworkCore;
using MunkasokBeolvasas.Models;
using Newtonsoft.Json;


namespace MunkasokBeolvasas.Data
{
    // Adatbázis inicializáló osztály, amely a dolgozók adatainak adatbázisba helyezését végzi
    public class DatabaseInitializer
    {
        private readonly WorkerDbContext _context;

        public DatabaseInitializer(WorkerDbContext context)
        {
            _context = context;
        }
        // Alapból törölni fogja a meglévő adatokat a program futtatásakor
        public async Task InitializeAsync(bool clearDatabase = true)
        {
            if (clearDatabase)
            {
                await ClearDatabaseAsync();
            }
            await SeedDatabaseAsync();
        }

        private async Task ClearDatabaseAsync()
        {
            // Meglévő adatok törlése az adatbázisból és az id 0-ra állítása
            _context.Database.ExecuteSqlRaw("TRUNCATE TABLE Workers; DBCC CHECKIDENT('Workers', RESEED, 0)");
            await _context.SaveChangesAsync();
        }

        // Az adatbázis feltöltése az előre megadott Munkasok.json fileból
        private async Task SeedDatabaseAsync()
        {
            try
            {
                // Adat beolvasása
                var jsonFilePath = Path.Combine("App_Data", "Munkasok.json");
                var jsonData = await File.ReadAllTextAsync(jsonFilePath);
                var workers = JsonConvert.DeserializeObject<List<Worker>>(jsonData);

                // Adat feltöltése
                await _context.Workers.AddRangeAsync(workers);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Hibakezelés amennyiben nem töltődik fel az adatbázis
                Console.WriteLine($"Error seeding database: {ex.Message}");
            }
        }
    }
}
