using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using _net_integrador.Models;
using _net_integrador.Repositorios;

namespace _net_integrador.Controllers;

public class UsuarioController : Controller
{
    private readonly ILogger<UsuarioController> _logger;
    private readonly IRepositorioUsuario _usuarioRepo;

    public UsuarioController(ILogger<UsuarioController> logger, IRepositorioUsuario usuarioRepo)
    {
        _logger = logger;
        _usuarioRepo = usuarioRepo;
    }

    public IActionResult Index()
    {
        var listaUsuarios = _usuarioRepo.ObtenerUsuarios();
        return View(listaUsuarios);
    }
    
    [HttpGet]
    public IActionResult Agregar()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Editar(int id)
    {
        var usuarioSeleccionado = _usuarioRepo.ObtenerUsuarioId(id);
        return View(usuarioSeleccionado);
    }
    
    public IActionResult Eliminar(int id)
    {
        _usuarioRepo.EliminarUsuario(id);
        return RedirectToAction("Index");
    }
    
    [HttpPost]
    public IActionResult Editar(Usuario usuarioEditado)
    {
        TempData["Exito"] = "Datos guardados con éxito";
        _usuarioRepo.ActualizarUsuario(usuarioEditado);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult Agregar(Usuario usuarioNuevo)
    {
        if (ModelState.IsValid)
        {
            _usuarioRepo.AgregarUsuario(usuarioNuevo);
            TempData["Exito"] = "Usuario agregado con éxito";
            return RedirectToAction("Index");
        }
        return View("Agregar", usuarioNuevo);
    }
}
