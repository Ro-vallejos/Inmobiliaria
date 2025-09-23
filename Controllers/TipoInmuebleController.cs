using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using _net_integrador.Models;
using _net_integrador.Repositorios;

namespace _net_integrador.Controllers;

public class TipoInmuebleController : Controller
{
    private readonly ILogger<TipoInmuebleController> _logger;
    // Usamos la interfaz del repositorio para habilitar la inyección de dependencias
    private readonly IRepositorioTipoInmueble _tipoInmuebleRepo;

    // El constructor ahora recibe la interfaz del repositorio como dependencia
    public TipoInmuebleController(ILogger<TipoInmuebleController> logger, IRepositorioTipoInmueble tipoInmuebleRepo)
    {
        _logger = logger;
        _tipoInmuebleRepo = tipoInmuebleRepo;
    }

    public IActionResult Index()
    {
        var listaTiposInmueble = _tipoInmuebleRepo.ObtenerTiposInmueble();
        return View(listaTiposInmueble);
    }
    
    [HttpGet]
    public IActionResult Agregar()
    {
        return View();
    }
    
    [HttpGet]
    public IActionResult Editar(int id)
    {
        var tipoSeleccionado = _tipoInmuebleRepo.ObtenerTipoInmuebleId(id);
        return View(tipoSeleccionado);
    }

    public IActionResult Eliminar(int id)
    {
        _tipoInmuebleRepo.EliminarTipoInmueble(id);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult Editar(TipoInmueble tipoEditado)
    {
        TempData["Exito"] = "Datos guardados con éxito";
        _tipoInmuebleRepo.ActualizarTipoInmueble(tipoEditado);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult Agregar(TipoInmueble tipoNuevo)
    {
        if (ModelState.IsValid)
        {
            _tipoInmuebleRepo.AgregarTipoInmueble(tipoNuevo);
            TempData["Exito"] = "Tipo de inmueble agregado con éxito";
            return RedirectToAction("Index");
        }
        return View("Agregar", tipoNuevo);
    }
}
