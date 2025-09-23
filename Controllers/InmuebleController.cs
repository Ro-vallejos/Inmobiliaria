using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using _net_integrador.Models;
using _net_integrador.Repositorios;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace _net_integrador.Controllers;

public class InmuebleController : Controller
{
    private readonly ILogger<InmuebleController> _logger;
    private RepositorioInmueble inmueble = new RepositorioInmueble();
    private RepositorioPropietario propietario = new RepositorioPropietario();

    public InmuebleController(ILogger<InmuebleController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        var listaInmuebles = inmueble.ObtenerInmuebles();
        return View(listaInmuebles);
    }

    [HttpGet]
    public IActionResult Agregar()
    {
         var p = propietario.ObtenerPropietarios();
        ViewBag.Propietarios = new SelectList(p, "id", "NombreCompleto");
        return View();
    }

    [HttpGet]
    public IActionResult Editar(int id)
    {
        var inmuebleSeleccionado = inmueble.ObtenerInmuebleId(id);
        return View(inmuebleSeleccionado);
    }

    public IActionResult Suspender(int id)
    {
        inmueble.SuspenderOferta(id);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult Editar(Inmueble inmuebleEditado)
    {
        TempData["Exito"] = "Datos guardados con éxito";
        inmueble.ActualizarInmueble(inmuebleEditado);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult Agregar(Inmueble inmuebleNuevo)
    {
        
        if(ModelState.IsValid)
        {
           

                inmuebleNuevo.estado = 1;
                inmueble.AgregarInmueble(inmuebleNuevo);
                TempData["Exito"] = "Inmueble agregado con éxito";
                return RedirectToAction("Index");
        }
        ViewBag.Propietarios = new SelectList(propietario.ObtenerPropietarios(), "id", "NombreCompleto", inmuebleNuevo.id_propietario);
        return View("Agregar", inmuebleNuevo);


    }
}