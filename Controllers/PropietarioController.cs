using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using _net_integrador.Models;
using _net_integrador.Repositorios;

namespace _net_integrador.Controllers;

public class PropietarioController : Controller
{

    private readonly ILogger<PropietarioController> _logger;
    private RepositorioPropietario propietario = new RepositorioPropietario();
    private readonly RepositorioPropietario repositorio;


    public PropietarioController(ILogger<PropietarioController> logger)
    {
        this.repositorio = new RepositorioPropietario();
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
        if (propietario == null)
        {
            return NotFound();
        }
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
        if (!ModelState.IsValid)
        {
            return View(propietario);
        }
        else
        {

            try
            {
                bool error = false;
                if (repositorio.ExisteDni(propietario.dni,propietario.id))
                {
                    ModelState.AddModelError("dni", "Este dni ya está registrado");
                    error = true;
                }
                if (repositorio.ExisteEmail(propietario.email,propietario.id))
                {
                    ModelState.AddModelError("email", "Este email ya está registrado");
                    error = true;
                }
                if (error)
                {
                    return View(propietario);
                }
                propietario.nombre = propietario.nombre?.ToUpper() ?? "";
                propietario.apellido = propietario.apellido?.ToUpper() ?? "";
                propietario.email = propietario.email?.ToLower() ?? "";
                var repo = new RepositorioPropietario();
                TempData["Exito"] = "Datos guardados con éxito";
                repo.ActualizarPropietario(propietario);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

    [HttpPost]
    public IActionResult Agregar(Propietario propietarioNuevo)
    {
        if (ModelState.IsValid)
        {
            bool error = false;
            if (repositorio.ExisteDni(propietarioNuevo.dni))
            {
                ModelState.AddModelError("dni", "Este DNI ya está registrado");
                error = true;
            }
            
                if (repositorio.ExisteEmail(propietarioNuevo.email))
                {
                    ModelState.AddModelError("email", "Este email ya está registrado");
                    error = true;
                }
            if (error)
            {
                return View();
            }
             
            propietarioNuevo.estado = 1;
            propietarioNuevo.nombre = propietarioNuevo.nombre.ToUpper();
            propietarioNuevo.apellido = propietarioNuevo.apellido.ToUpper();
            propietarioNuevo.email = propietarioNuevo.email.ToLower();
            propietario.AgregarPropietario(propietarioNuevo);


            TempData["Exito"] = "Propietario agregado con éxito";
            return RedirectToAction("Index");
        }
        return View("Agregar", propietarioNuevo);
    }



}
