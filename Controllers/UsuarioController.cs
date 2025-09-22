using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using _net_integrador.Models;
using _net_integrador.Repositorios;

namespace _net_integrador.Controllers;

public class UsuarioController : Controller
{
    private readonly ILogger<UsuarioController> _logger;
    private RepositorioUsuario usuario = new RepositorioUsuario();

    public UsuarioController(ILogger<UsuarioController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        var listaUsuarios = usuario.ObtenerUsuarios();
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
        var usuarioSeleccionado = usuario.ObtenerUsuarioId(id);
        return View(usuarioSeleccionado);
    }
    
    public IActionResult Eliminar(int id)
    {
        //[cite_start]
        usuario.EliminarUsuario(id);
        return RedirectToAction("Index");
    }
    
    [HttpPost]
    public IActionResult Editar(Usuario usuarioEditado)
    {
        TempData["Exito"] = "Datos guardados con éxito";
        usuario.ActualizarUsuario(usuarioEditado);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult Agregar(Usuario usuarioNuevo)
    {
        if (ModelState.IsValid)
        {
            usuario.AgregarUsuario(usuarioNuevo);
            TempData["Exito"] = "Usuario agregado con éxito";
            return RedirectToAction("Index");
        }
        return View("Agregar", usuarioNuevo);
    }
}