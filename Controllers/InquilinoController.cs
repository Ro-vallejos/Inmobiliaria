using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using _net_integrador.Models;
using _net_integrador.Repositorios;

namespace _net_integrador.Controllers
{
    public class InquilinoController : Controller
    {
        private readonly ILogger<InquilinoController> _logger;

        private readonly RepositorioInquilino inquilinoRepo = new RepositorioInquilino();

        public InquilinoController(ILogger<InquilinoController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var listaInquilinos = inquilinoRepo.ObtenerInquilinos();
            return View(listaInquilinos);
        }

        [HttpGet]
        public IActionResult Agregar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Agregar(Inquilino inquilinoNuevo)
        {
            if (!ModelState.IsValid)
            {
                return View(inquilinoNuevo);
            }
            else
            {
                try
                 {
                    bool error = false;
                    if (inquilinoRepo.ExisteDni(inquilinoNuevo.dni))
                    {
                        ModelState.AddModelError("dni", "Este DNI ya está registrado");
                        error = true;
                    }

                    if (inquilinoRepo.ExisteEmail(inquilinoNuevo.email))
                    {
                        ModelState.AddModelError("email", "Este email ya está registrado");
                        error = true;
                    }
                    if (error)
                    {
                        return View();
                    }
                    inquilinoNuevo.nombre = inquilinoNuevo.nombre?.ToUpper() ?? "";
                    inquilinoNuevo.apellido = inquilinoNuevo.apellido?.ToUpper() ?? "";
                    inquilinoNuevo.email = inquilinoNuevo.email?.ToLower() ?? "";
                    inquilinoNuevo.estado = 1;
                    inquilinoRepo.AgregarInquilino(inquilinoNuevo);
                    TempData["Exito"] = "Datos guardados con éxito";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);

                }
            }
        }

        [HttpGet]
        public IActionResult Editar(int id)
        {
            var inquilinoSeleccionado = inquilinoRepo.ObtenerInquilinoId(id);
            if (inquilinoSeleccionado == null) return NotFound();
            return View(inquilinoSeleccionado);
        }

        [HttpPost]
        public IActionResult Editar(Inquilino inquilino)
        {
            if (!ModelState.IsValid)
            {
                return View(inquilino);
            }
            else
            {
                try
                {


                    bool error = false;
                    if (inquilinoRepo.ExisteDni(inquilino.dni, inquilino.id))
                    {
                        ModelState.AddModelError("dni", "Este DNI ya está registrado");
                        error = true;
                    }

                    if (inquilinoRepo.ExisteEmail(inquilino.email, inquilino.id))
                    {
                        ModelState.AddModelError("email", "Este email ya está registrado");
                        error = true;
                    }
                    if (error)
                    {
                        return View();
                    }
                    inquilino.nombre = inquilino.nombre?.ToUpper() ?? "";
                    inquilino.apellido = inquilino.apellido?.ToUpper() ?? "";
                    inquilino.email = inquilino.email?.ToLower() ?? "";
                    inquilinoRepo.ActualizarInquilino(inquilino);
                    TempData["Exito"] = "Datos guardados con éxito";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);

                }
            }

        }

        public IActionResult Eliminar(int id)
        {
            inquilinoRepo.EliminarInquilino(id);
            return RedirectToAction("Index");
        }
    }
}
