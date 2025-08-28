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

        // GET: /Inquilino
        public IActionResult Index()
        {
            var listaInquilinos = inquilinoRepo.ObtenerInquilinos();
            return View(listaInquilinos);
        }

        // GET: /Inquilino/Agregar
        [HttpGet]
        public IActionResult Agregar()
        {
            return View();
        }

        // POST: /Inquilino/Agregar
        [HttpPost]
        public IActionResult Agregar(Inquilino inquilinoNuevo)
        {
            if (ModelState.IsValid)
            {
                inquilinoNuevo.estado = 1; // activo por defecto
                inquilinoRepo.AgregarInquilino(inquilinoNuevo);
                TempData["Exito"] = "Inquilino agregado con éxito";
                return RedirectToAction("Index");
            }
            return View(inquilinoNuevo);
        }

        // GET: /Inquilino/Editar/5
        [HttpGet]
        public IActionResult Editar(int id)
        {
            var inquilinoSeleccionado = inquilinoRepo.ObtenerInquilinoId(id);
            if (inquilinoSeleccionado == null) return NotFound();
            return View(inquilinoSeleccionado);
        }

        // POST: /Inquilino/Editar
        [HttpPost]
        public IActionResult Editar(Inquilino inquilino)
        {
            if (!ModelState.IsValid) return View(inquilino);

            inquilinoRepo.ActualizarInquilino(inquilino);
            TempData["Exito"] = "Datos guardados con éxito";
            return RedirectToAction("Index");
        }

        // GET: /Inquilino/Eliminar/5
        public IActionResult Eliminar(int id)
        {
            inquilinoRepo.EliminarInquilino(id);
            return RedirectToAction("Index");
        }
    }
}
