using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using _net_integrador.Models;
using _net_integrador.Repositorios;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq.Expressions;
using Org.BouncyCastle.Asn1.Iana;

namespace _net_integrador.Controllers;

public class InmuebleController : Controller
{
    private readonly ILogger<InmuebleController> _logger;
    private readonly IRepositorioInmueble _repositorioInmueble;
    private readonly IRepositorioPropietario _repositorioPropietario;
    private readonly IRepositorioTipoInmueble _repositorioTipoInmueble;

    public InmuebleController(ILogger<InmuebleController> logger, IRepositorioInmueble repositorioInmueble, IRepositorioPropietario repositorioPropietario, IRepositorioTipoInmueble repositorioTipoInmueble)
    {
        _logger = logger;
        _repositorioInmueble = repositorioInmueble;
        _repositorioPropietario = repositorioPropietario;
        _repositorioTipoInmueble = repositorioTipoInmueble;
    }

    public IActionResult Index()
    {
        var listaInmuebles = _repositorioInmueble.ObtenerInmuebles();
        return View(listaInmuebles);
    }
 
    public IActionResult Activar(int id)
    {
        _repositorioInmueble.ActivarOferta(id);
        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult Agregar()
    {
        var propietarios = _repositorioPropietario.ObtenerPropietarios();
        ViewBag.Propietarios = new SelectList(propietarios, "id", "NombreCompleto");
        ViewBag.TiposInmueble = new SelectList(_repositorioTipoInmueble.ObtenerTiposInmueble(), "id", "tipo");
        return View();
    }

    [HttpGet]
    public IActionResult Editar(int id)
    {
        var inmuebleSeleccionado = _repositorioInmueble.ObtenerInmuebleId (id);
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
        if (ModelState.IsValid)
        {
           

            inmuebleNuevo.estado = 1;
            _repositorioInmueble.AgregarInmueble(inmuebleNuevo);
            TempData["Exito"] = "Inmueble agregado con éxito";
            return RedirectToAction("Index");
        }
        if (!ModelState.IsValid)
        {
             var errores = string.Join("; ", ModelState.Values
        .SelectMany(v => v.Errors)
        .Select(e => e.ErrorMessage));
    throw new Exception($"Modelo inválido: {errores}");
            
           
        }

        ViewBag.TiposInmueble = new SelectList(_repositorioTipoInmueble.ObtenerTiposInmueble(), "id", "tipo", inmuebleNuevo.id_tipo);
        ViewBag.Propietarios = new SelectList(_repositorioPropietario.ObtenerPropietarios(), "id", "NombreCompleto", inmuebleNuevo.id_propietario);
        return View("Agregar", inmuebleNuevo);
    }
}
