using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using _net_integrador.Models;
using _net_integrador.Repositorios;

namespace _net_integrador.Controllers;

public class PagoController : Controller
{
    private readonly ILogger<PagoController> _logger;
    private RepositorioPago pago = new RepositorioPago();

    public PagoController(ILogger<PagoController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index(int contratoId)
    {
        var listaPagos = pago.ObtenerPagosPorContrato(contratoId);
        ViewBag.ContratoId = contratoId;
        return View(listaPagos);
    }
    
    [HttpGet]
    public IActionResult Agregar(int contratoId)
    {
        ViewBag.ContratoId = contratoId;
        return View();
    }

    public IActionResult Anular(int id)
    {
        pago.AnularPago(id);
        TempData["Exito"] = "Pago anulado con éxito";
        return RedirectToAction("Index", new { contratoId = TempData["ContratoId"] });
    }

    [HttpPost]
    public IActionResult Agregar(Pago pagoNuevo)
    {
        if (ModelState.IsValid)
        {
            pagoNuevo.estado = 1;
            pago.AgregarPago(pagoNuevo);
            TempData["Exito"] = "Pago agregado con éxito";
            return RedirectToAction("Index", new { contratoId = pagoNuevo.id_contrato });
        }
        return View("Agregar", pagoNuevo);
    }
}