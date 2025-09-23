using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using _net_integrador.Models;
using _net_integrador.Repositorios;
using Microsoft.Extensions.Configuration; // Necesario para la configuración

namespace _net_integrador.Controllers;

public class ContratoController : Controller
{
    private readonly ILogger<ContratoController> _logger;
    // Ahora usamos las interfaces para las dependencias
    private readonly IRepositorioContrato _contratoRepo;
    private readonly RepositorioPago _pagoRepo;

    // Inyectamos las dependencias a través del constructor
    public ContratoController(ILogger<ContratoController> logger, IRepositorioContrato contratoRepo, RepositorioPago pagoRepo)
    {
        _logger = logger;
        _contratoRepo = contratoRepo;
        _pagoRepo = pagoRepo;
    }

    public IActionResult Index()
    {
        var listaContratos = _contratoRepo.ObtenerContratos();
        return View(listaContratos);
    }
    
    [HttpGet]
    public IActionResult Agregar()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Detalles(int id)
    {
        var contratoSeleccionado = _contratoRepo.ObtenerContratoId(id);
        var pagosDelContrato = _pagoRepo.ObtenerPagosPorContrato(id);
        ViewBag.Pagos = pagosDelContrato;
        return View(contratoSeleccionado);
    }

    public IActionResult TerminarAnticipado(int id)
    {
        var contratoSeleccionado = _contratoRepo.ObtenerContratoId(id);
        return View("TerminarAnticipado", contratoSeleccionado);
    }

    [HttpPost]
    public IActionResult Agregar(Contrato contratoNuevo)
    {
        if (ModelState.IsValid)
        {
            _contratoRepo.AgregarContrato(contratoNuevo);
            TempData["Exito"] = "Contrato agregado con éxito";
            return RedirectToAction("Index");
        }
        return View("Agregar", contratoNuevo);
    }
}
