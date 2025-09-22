using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using _net_integrador.Models;
using _net_integrador.Repositorios;

namespace _net_integrador.Controllers;

public class TipoInmuebleController : Controller
{
    private readonly ILogger<TipoInmuebleController> _logger;
    private RepositorioTipoInmueble tipoInmueble = new RepositorioTipoInmueble();

    public TipoInmuebleController(ILogger<TipoInmuebleController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        var listaTiposInmueble = tipoInmueble.ObtenerTiposInmueble();
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
        var tipoSeleccionado = tipoInmueble.ObtenerTipoInmuebleId(id);
        return View(tipoSeleccionado);
    }

    public IActionResult Eliminar(int id)
    {
        tipoInmueble.EliminarTipoInmueble(id);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult Editar(TipoInmueble tipoEditado)
    {
        TempData["Exito"] = "Datos guardados con éxito";
        tipoInmueble.ActualizarTipoInmueble(tipoEditado);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult Agregar(TipoInmueble tipoNuevo)
    {
        if (ModelState.IsValid)
        {
            tipoInmueble.AgregarTipoInmueble(tipoNuevo);
            TempData["Exito"] = "Tipo de inmueble agregado con éxito";
            return RedirectToAction("Index");
        }
        return View("Agregar", tipoNuevo);
    }
}