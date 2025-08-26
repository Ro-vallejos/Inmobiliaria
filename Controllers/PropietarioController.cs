using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using _net_integrador.Models;
using _net_integrador.Repositorios;

namespace _net_integrador.Controllers;

public class PropietarioController : Controller
{

    private readonly ILogger<PropietarioController> _logger;
    private RepositorioPropietario propietario = new RepositorioPropietario();

    public PropietarioController(ILogger<PropietarioController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        var listaPropietarios = propietario.ObtenerPropietarios();
        return View(listaPropietarios);
    }
    
    [HttpGet]
    public IActionResult Agregar()
    {
        return View();
    }
    [HttpGet]
    public IActionResult Editar(int id)
    {
        var propietarioSeleccionado = propietario.ObtenerPropietarioId(id);
        return View(propietarioSeleccionado);
    }
 
    public IActionResult Eliminar(int id)
    {
        propietario.EliminarPropietario(id);
        return RedirectToAction("Index");
    }
    [HttpPost]
    public IActionResult Editar(Propietario propietario)
    {
        var repo = new RepositorioPropietario();
        TempData["Exito"] = "Datos guardados con éxito";
        repo.ActualizarPropietario(propietario);

        return RedirectToAction("Index");
    }

  [HttpPost]
    public IActionResult Agregar(Propietario propietarioNuevo)
    {
        if (ModelState.IsValid)
        {
            propietarioNuevo.estado = 1; 
            propietario.AgregarPropietario(propietarioNuevo);

            TempData["Exito"] = "Propietario agregado con éxito";
            return RedirectToAction("Index");
        }
        return View("Agregar", propietarioNuevo);
    }



}
