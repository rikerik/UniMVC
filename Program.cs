
using Microsoft.EntityFrameworkCore;
using MunkasokBeolvasas.Data;

// Az alkalmaz�s builder�nek l�trehoz�sa
var builder = WebApplication.CreateBuilder(args);

// Szolg�ltat�sok hozz�ad�sa a kont�nerhez
builder.Services.AddControllersWithViews();

// Adatb�zis kontextus felv�tele a servicesbe
builder.Services.AddDbContext<WorkerDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MunkasokBeolvasas")));

// Az alkalmaz�s konfigur�ci�j�nak be�ll�t�sa
var app = builder.Build();

// Adatb�zis inicializ�l�sa
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    // WorkerDbContext lek�r�se a szolg�ltat�skont�nerb�l
    var context = services.GetRequiredService<WorkerDbContext>();

    // Adatb�zis inicializ�l� l�trehoz�sa
    var initializer = new DatabaseInitializer(context);

    // Adatb�zis inicializ�l�sa
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
