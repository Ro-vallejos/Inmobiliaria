using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using _net_integrador.Models;
using _net_integrador.Repositorios;

namespace _net_integrador.Controllers;

public class PagoController : Controller
{
    private readonly ILogger<PagoController> _logger;
    private readonly IRepositorioPago _pagoRepo;

    public PagoController(ILogger<PagoController> logger, IRepositorioPago pagoRepo)
    {
        _logger = logger;
        _pagoRepo = pagoRepo;
    }

    public IActionResult Index(int contratoId)
    {
        var listaPagos = _pagoRepo.ObtenerPagosPorContrato(contratoId);
        ViewBag.ContratoId = contratoId;
        return View(listaPagos);
    }
    
    [HttpPost]
    public IActionResult Recibir(int id)
    {
        var pago = _pagoRepo.ObtenerPagoId(id);
        if (pago == null)
        {
            TempData["Error"] = "Pago no encontrado.";
            return RedirectToAction("Index", new { contratoId = TempData["ContratoId"] });
        }
        
        pago.estado = EstadoPago.recibido;
        _pagoRepo.ActualizarPago(pago);
        
        TempData["Exito"] = "Pago recibido con éxito";
        return RedirectToAction("Index", new { contratoId = pago.id_contrato });
    }

    public IActionResult Anular(int id)
    {
        var pago = _pagoRepo.ObtenerPagoId(id);
        if (pago == null)
        {
            TempData["Error"] = "Pago no encontrado.";
            return RedirectToAction("Index", new { contratoId = TempData["ContratoId"] });
        }
        
        pago.estado = EstadoPago.anulado;
        _pagoRepo.ActualizarPago(pago);

        TempData["Exito"] = "Pago anulado con éxito";
        return RedirectToAction("Index", new { contratoId = pago.id_contrato });
    }
}