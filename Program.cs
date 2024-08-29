
using Microsoft.EntityFrameworkCore;
using MunkasokBeolvasas.Data;

// Az alkalmazás builderének létrehozása
var builder = WebApplication.CreateBuilder(args);

// Szolgáltatások hozzáadása a konténerhez
builder.Services.AddControllersWithViews();

// Adatbázis kontextus felvétele a servicesbe
builder.Services.AddDbContext<WorkerDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MunkasokBeolvasas")));

// Az alkalmazás konfigurációjának beállítása
var app = builder.Build();

// Adatbázis inicializálása
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    // WorkerDbContext lekérése a szolgáltatáskonténerbõl
    var context = services.GetRequiredService<WorkerDbContext>();

    // Adatbázis inicializáló létrehozása
    var initializer = new DatabaseInitializer(context);

    // Adatbázis inicializálása
    await initializer.InitializeAsync();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
