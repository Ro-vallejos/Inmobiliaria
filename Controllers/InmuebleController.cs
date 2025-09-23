using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using _net_integrador.Models;
using _net_integrador.Repositorios;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace _net_integrador.Controllers;

public class InmuebleController : Controller
{
    private readonly ILogger<InmuebleController> _logger;
    private readonly IRepositorioInmueble _repositorioInmueble;
    private readonly IRepositorioPropietario _repositorioPropietario;

    public InmuebleController(ILogger<InmuebleController> logger, IRepositorioInmueble repositorioInmueble, IRepositorioPropietario repositorioPropietario)
    {
        _logger = logger;
        _repositorioInmueble = repositorioInmueble;
        _repositorioPropietario = repositorioPropietario;
    }

    public IActionResult Index()
    {
        var listaInmuebles = _repositorioInmueble.ObtenerInmuebles();
        return View(listaInmuebles);
    }

    [HttpGet]
    public IActionResult Agregar()
    {
        var propietarios = _repositorioPropietario.ObtenerPropietarios();
        ViewBag.Propietarios = new SelectList(propietarios, "id", "NombreCompleto");
        return View();
    }

    [HttpGet]
    public IActionResult Editar(int id)
    {
        var inmuebleSeleccionado = _repositorioInmueble.ObtenerInmuebleId(id);
        return View(inmuebleSeleccionado);
    }

    public IActionResult Suspender(int id)
    {
        _repositorioInmueble.SuspenderOferta(id);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult Editar(Inmueble inmuebleEditado)
    {
        TempData["Exito"] = "Datos guardados con éxito";
        _repositorioInmueble.ActualizarInmueble(inmuebleEditado);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult Agregar(Inmueble inmuebleNuevo)
    {
        if(ModelState.IsValid)
        {
            inmuebleNuevo.estado = 1;
            _repositorioInmueble.AgregarInmueble(inmuebleNuevo);
            TempData["Exito"] = "Inmueble agregado con éxito";
            return RedirectToAction("Index");
        }
        ViewBag.Propietarios = new SelectList(_repositorioPropietario.ObtenerPropietarios(), "id", "NombreCompleto", inmuebleNuevo.id_propietario);
        return View("Agregar", inmuebleNuevo);
    }
}
