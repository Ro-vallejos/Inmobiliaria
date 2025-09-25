using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using _net_integrador.Models;
using _net_integrador.Repositorios;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

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

    [Authorize] 
    public IActionResult Index()
    {
        var listaUsuarios = _usuarioRepo.ObtenerUsuarios();
        return View(listaUsuarios);
    }
    
    [Authorize(Policy = "AdminOnly")] 
    [HttpGet]
    public IActionResult Agregar()
    {
        return View();
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpGet]
    public IActionResult Editar(int id)
    {
        var usuarioSeleccionado = _usuarioRepo.ObtenerUsuarioId(id);
        return View(usuarioSeleccionado);
    }
    
    [Authorize(Policy = "AdminOnly")]
    public IActionResult Eliminar(int id)
    {
        _usuarioRepo.EliminarUsuario(id);
        TempData["Exito"] = "Usuario desactivado con éxito";
        return RedirectToAction("Index");
    }
    
    [Authorize(Policy = "AdminOnly")]
    public IActionResult Activar(int id)
    {
        _usuarioRepo.ActivarUsuario(id);
        TempData["Exito"] = "Usuario activado con éxito";
        return RedirectToAction("Index");
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public IActionResult Editar(Usuario usuarioEditado)
    {
        if (ModelState.IsValid)
        {
            if (string.IsNullOrEmpty(usuarioEditado.avatar))
            {
                string nombreCompleto = $"{usuarioEditado.nombre} {usuarioEditado.apellido}";
                usuarioEditado.avatar = $"https://ui-avatars.com/api/?name={Uri.EscapeDataString(nombreCompleto)}&background=343a40&color=fff&rounded=true&size=128";
            }
            _usuarioRepo.ActualizarUsuario(usuarioEditado);
            TempData["Exito"] = "Datos guardados con éxito";
            return RedirectToAction("Index");
        }
        return View("Editar", usuarioEditado);
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public IActionResult Agregar(Usuario usuarioNuevo)
    {
        if (ModelState.IsValid)
        {
            string nombreCompleto = $"{usuarioNuevo.nombre} {usuarioNuevo.apellido}";
            usuarioNuevo.avatar = $"https://ui-avatars.com/api/?name={Uri.EscapeDataString(nombreCompleto)}&background=343a40&color=fff&rounded=true&size=128";
            _usuarioRepo.AgregarUsuario(usuarioNuevo);
            TempData["Exito"] = "Usuario agregado con éxito";
            return RedirectToAction("Index");
        }
        return View("Agregar", usuarioNuevo);
    }

    [HttpGet]
    public IActionResult Login()
    {
        if (User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Index");
        }
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string email, string password)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ViewBag.Error = "Debe ingresar el email y la contraseña.";
            return View();
        }

        var usuario = _usuarioRepo.ObtenerUsuarioEmail(email);

        if (usuario == null || usuario.password != password)
        {
            ViewBag.Error = "El email o la contraseña son incorrectos.";
            return View();
        }
        
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, usuario.email),
            new Claim(ClaimTypes.Role, usuario.rol)
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        var authProperties = new AuthenticationProperties
        {
            IsPersistent = false
        };

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

        TempData["Exito"] = "Inicio de sesión exitoso.";
        return RedirectToAction("Index", "Home");
    }
    
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }
}