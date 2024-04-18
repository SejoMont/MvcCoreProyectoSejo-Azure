using Microsoft.EntityFrameworkCore;
using MvcCoreProyectoSejo.Helpers;
using MvcCoreProyectoSejo.Models;
using MvcCoreProyectoSejo.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor();

builder.Services.AddSession();

// Agrega servicios al contenedor de dependencias.
builder.Services.AddScoped<UploadFilesController>();

builder.Services.AddTransient<HelperMails>();
builder.Services.AddTransient<HelperTools>();
builder.Services.AddTransient<HelperPathProvider>();

builder.Services.AddTransient<EntradasRepository>();
builder.Services.AddTransient<ArtistasEventoRepository>();
builder.Services.AddTransient<EventosRepository>();
builder.Services.AddTransient<UsuariosRepository>();
builder.Services.AddTransient<ProvinciasRepository>();

string connectionString = builder.Configuration.GetConnectionString("SqlServerSejo");
builder.Services.AddDbContext<EventosContext>(options => options.UseSqlServer(connectionString));

var app = builder.Build();

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

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Eventos}/{action=Index}/{id?}");

app.Run();
