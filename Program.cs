using _net_integrador.Repositorios;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddTransient<IRepositorioInmueble, RepositorioInmueble>();

builder.Services.AddTransient<IRepositorioPropietario, RepositorioPropietario>();

builder.Services.AddTransient<IRepositorioInquilino, RepositorioInquilino>();

builder.Services.AddTransient<IRepositorioContrato, RepositorioContrato>();

builder.Services.AddTransient<IRepositorioPago, RepositorioPago>();

builder.Services.AddTransient<IRepositorioTipoInmueble, RepositorioTipoInmueble>();

builder.Services.AddTransient<IRepositorioUsuario, RepositorioUsuario>();

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
