using Microsoft.EntityFrameworkCore;
using MunkasokBeolvasas.Models;

namespace MunkasokBeolvasas.Data
{
    //Osztály a dolgozók adatbázisban való kezeléséhez
    public class WorkerDbContext : DbContext
    {
        // Konstruktor a WorkerDbContext inicializálásához
        public WorkerDbContext(DbContextOptions<WorkerDbContext> options) : base(options) 
        {

        }
        // Workers entitás készlete az adatbázisban
        public DbSet<Worker> Workers { get; set; }
    }
}
