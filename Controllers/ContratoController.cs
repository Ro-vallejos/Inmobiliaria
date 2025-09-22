using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using _net_integrador.Models;
using _net_integrador.Repositorios;

namespace _net_integrador.Controllers;

public class ContratoController : Controller
{
    private readonly ILogger<ContratoController> _logger;
    private RepositorioContrato contrato = new RepositorioContrato();
    private RepositorioPago pago = new RepositorioPago();

    public ContratoController(ILogger<ContratoController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        var listaContratos = contrato.ObtenerContratos();
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
        var contratoSeleccionado = contrato.ObtenerContratoId(id);
        var pagosDelContrato = pago.ObtenerPagosPorContrato(id);
        ViewBag.Pagos = pagosDelContrato;
        return View(contratoSeleccionado);
    }

    public IActionResult TerminarAnticipado(int id)
    {
        var contratoSeleccionado = contrato.ObtenerContratoId(id);
        return View("TerminarAnticipado", contratoSeleccionado);
    }

    [HttpPost]
    public IActionResult Agregar(Contrato contratoNuevo)
    {
        if (ModelState.IsValid)
        {
            //[cite_start]
            contrato.AgregarContrato(contratoNuevo);
            TempData["Exito"] = "Contrato agregado con éxito";
            return RedirectToAction("Index");
        }
        return View("Agregar", contratoNuevo);
    }
}