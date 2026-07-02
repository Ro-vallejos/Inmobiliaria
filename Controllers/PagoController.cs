using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using _net_integrador.Models;
using _net_integrador.Repositorios;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace _net_integrador.Controllers;

[Authorize]
public class PagoController : Controller
{
    private readonly ILogger<PagoController> _logger;
    private readonly IRepositorioPago _pagoRepo;
    private readonly IRepositorioAuditoria _auditoriaRepo;
    private readonly IRepositorioContrato _contratoRepo;

    public PagoController(ILogger<PagoController> logger, IRepositorioPago pagoRepo, IRepositorioAuditoria auditoriaRepo, IRepositorioContrato contratoRepo)
    {
        _logger = logger;
        _pagoRepo = pagoRepo;
        _auditoriaRepo = auditoriaRepo;
        _contratoRepo = contratoRepo;
    }

    public IActionResult Index(int contratoId)
    {
        var listaPagos = _pagoRepo.ObtenerPagosPorContrato(contratoId);
        var contrato = _contratoRepo.ObtenerContratoId(contratoId);
        ViewBag.ContratoId = contratoId;
        ViewBag.ContratoEstado = contrato != null ? contrato.estado : 0;
        return View(listaPagos);
    }

    [HttpPost]
    public IActionResult Recibir()
    {
        if (!int.TryParse(HttpContext.Request.Form["id"], out int id))
        {
            TempData["Error"] = "ID de pago inválido.";
            return RedirectToAction("Index", "Contrato");
        }

        var pago = _pagoRepo.ObtenerPagoId(id);
        if (pago == null)
        {
            TempData["Error"] = "Pago no encontrado.";
            return RedirectToAction("Index", "Contrato");
        }

        var estadoAnterior = pago.estado.ToString();
        pago.estado = EstadoPago.recibido;
        pago.fecha_pago = DateTime.Now;
        _pagoRepo.ActualizarPago(pago);

        _auditoriaRepo.InsertarRegistroAuditoria(
            TipoAuditoria.Pago,
            pago.id,
            AccionAuditoria.Recibir,
            User.Identity.Name ?? "Anónimo"
        );

        TempData["Exito"] = "Pago recibido con éxito";
        return RedirectToAction("Index", new { contratoId = pago.id_contrato });
    }

    [HttpPost]
    public IActionResult Anular()
    {
        if (!int.TryParse(HttpContext.Request.Form["id"], out int id))
        {
            TempData["Error"] = "ID de pago inválido.";
            return RedirectToAction("Index", "Contrato");
        }

        var pago = _pagoRepo.ObtenerPagoId(id);
        if (pago == null)
        {
            TempData["Error"] = "Pago no encontrado.";
            return RedirectToAction("Index", "Contrato");
        }

        var estadoAnterior = pago.estado.ToString();

        if (pago.nro_pago == 0)
        {
            pago.estado = EstadoPago.pendiente;
            pago.fecha_pago = null;
            TempData["Exito"] = "El cobro de la multa fue revertido. Ya podés volver a registrar el pago.";
        }
        else
        {
            pago.estado = EstadoPago.anulado;
            pago.fecha_pago = DateTime.Now;
            TempData["Exito"] = "Pago del alquiler anulado con éxito.";
        }

        _pagoRepo.ActualizarPago(pago);

        _auditoriaRepo.InsertarRegistroAuditoria(
            TipoAuditoria.Pago,
            pago.id,
            AccionAuditoria.Anular,
            User.Identity.Name ?? "Anónimo"
        );

        return RedirectToAction("Index", new { contratoId = pago.id_contrato });
    }
    [HttpGet]
    public IActionResult Agregar(int idContrato)
    {
        var contrato = _contratoRepo.ObtenerContratoId(idContrato);
        if (contrato == null)
        {
            TempData["Error"] = "El contrato especificado no existe.";
            return RedirectToAction("Index", "Contrato");
        }
        if (contrato.estado == 0)
        {
            return RedirectToAction("Index", "Contrato");
        }
        List<int> cuotasPagas = _pagoRepo.ContarMesesPagados(idContrato);

        int totalMeses = 0;
        if (contrato.fecha_inicio.HasValue && contrato.fecha_fin.HasValue)
        {
            totalMeses = ((contrato.fecha_fin.Value.Year - contrato.fecha_inicio.Value.Year) * 12)
                        + contrato.fecha_fin.Value.Month - contrato.fecha_inicio.Value.Month;
        }
        var slotsFaltantes = new List<dynamic>();
        DateTime fechaBase = contrato.fecha_inicio ?? DateTime.Now;


        for (int i = 1; i <= totalMeses; i++)
        {
            if (!cuotasPagas.Contains(i))
            {
                DateTime fechaDelMes = fechaBase.AddMonths(i - 1);
                string nombreMes = fechaDelMes.ToString("MMMM yyyy", System.Globalization.CultureInfo.CurrentCulture);
                nombreMes = char.ToUpper(nombreMes[0]) + nombreMes.Substring(1);

                slotsFaltantes.Add(new
                {
                    Numero = i,
                    TextoDisplay = nombreMes
                });
            }
        }


        ViewBag.Contrato = contrato;
        ViewBag.SlotsFaltantes = slotsFaltantes;

        return View(new Pago { id_contrato = idContrato });
    }
    [HttpPost]
    public IActionResult RegistrarPago(int idContrato, List<int> mesesSeleccionados, string detalleAdicional)
    {
        var contrato = _contratoRepo.ObtenerContratoId(idContrato);
        if (contrato != null && contrato.estado == 0) return RedirectToAction("Index", "Contrato");
        var montoMensual = contrato.monto_mensual ?? 0;

        DateTime fechaBase = contrato.fecha_inicio ?? DateTime.Now;

        foreach (var nroMes in mesesSeleccionados.OrderBy(m => m))
        {

            DateTime fechaDelMesAbonado = fechaBase.AddMonths(nroMes - 1);

            string nombreMesText = fechaDelMesAbonado.ToString("MMMM", System.Globalization.CultureInfo.CurrentCulture);
            string anioText = fechaDelMesAbonado.ToString("yyyy");

            nombreMesText = char.ToUpper(nombreMesText[0]) + nombreMesText.Substring(1);

            string conceptoMes = $"Pago N° {nroMes} - Mes de {nombreMesText} {anioText}";

            if (!string.IsNullOrWhiteSpace(detalleAdicional))
            {
                conceptoMes += $" | {detalleAdicional}";
            }

            var nuevoPago = new Pago
            {
                id_contrato = idContrato,
                nro_pago = nroMes,
                fecha_pago = DateTime.Now,
                concepto = conceptoMes,
                estado = EstadoPago.recibido,
                monto = montoMensual
            };

            var pagoId = _pagoRepo.AgregarPago(nuevoPago);
            _auditoriaRepo.InsertarRegistroAuditoria(
            TipoAuditoria.Pago,
            pagoId,
            AccionAuditoria.Recibir,
            User.Identity.Name ?? "Anónimo"
        );
        }

        TempData["Exito"] = "Pagos registrados correctamente.";
        return RedirectToAction("Index", new { ContratoId = idContrato });
    }
    [HttpGet]
    public IActionResult Editar(int id)
    {
        var pago = _pagoRepo.ObtenerPagoId(id);
        if (pago == null)
        {
            TempData["Error"] = "El registro de pago no existe.";
            return RedirectToAction("Index", "Contrato");
        }

        var contrato = _contratoRepo.ObtenerContratoId(pago.id_contrato);
        string conceptoFijo = pago.concepto;
        string detalleEditable = "";

        if (pago.concepto != null && pago.concepto.Contains('|'))
        {
            int indice = pago.concepto.IndexOf('|');
            conceptoFijo = pago.concepto.Substring(0, indice);
            detalleEditable = pago.concepto.Substring(indice + 2);
        }

        string nombreMesDisplay = "Mes Desconocido";
        if (contrato != null && contrato.fecha_inicio.HasValue)
        {
            DateTime fechaDelMes = contrato.fecha_inicio.Value.AddMonths(pago.nro_pago - 1);
            nombreMesDisplay = fechaDelMes.ToString("MMMM yyyy", System.Globalization.CultureInfo.CurrentCulture);
            nombreMesDisplay = char.ToUpper(nombreMesDisplay[0]) + nombreMesDisplay.Substring(1);
        }

        ViewBag.ConceptoFijo = conceptoFijo;
        ViewBag.DetalleEditable = detalleEditable;
        ViewBag.NombreMesDisplay = nombreMesDisplay;
        ViewBag.Contrato = contrato;

        return View(pago);
    }

    [HttpPost]
    public IActionResult Editar(int id, string detalleAdicional)
    {
        var pagoOriginal = _pagoRepo.ObtenerPagoId(id);
        if (pagoOriginal == null) return NotFound();

        string conceptoFijo = pagoOriginal.concepto;
        if (pagoOriginal.concepto != null && pagoOriginal.concepto.Contains("|"))
        {
            int indiceGuion = pagoOriginal.concepto.IndexOf("|");
            conceptoFijo = pagoOriginal.concepto.Substring(0, indiceGuion);
        }
        if (pagoOriginal.nro_pago > 0)
        {
            pagoOriginal.concepto = conceptoFijo;

            if (!string.IsNullOrWhiteSpace(detalleAdicional))
            {
                pagoOriginal.concepto += $" | {detalleAdicional}";
            }
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(detalleAdicional))
            {
                pagoOriginal.concepto = detalleAdicional;
            }
        }

        _pagoRepo.ActualizarPago(pagoOriginal);
        _auditoriaRepo.InsertarRegistroAuditoria(
            TipoAuditoria.Pago,
            pagoOriginal.id,
            AccionAuditoria.Actualizar,
            User.Identity.Name ?? "Anónimo"
        );

        TempData["Exito"] = "Pago actualizado correctamente.";
        return RedirectToAction("Index", new { contratoId = pagoOriginal.id_contrato });
    }

}