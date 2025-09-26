using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using _net_integrador.Models;
using _net_integrador.Repositorios;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace _net_integrador.Controllers;

public class ContratoController : Controller
{
    private readonly ILogger<ContratoController> _logger;
    private readonly IRepositorioContrato _contratoRepo;
    private readonly IRepositorioPago _pagoRepo;
    private readonly IRepositorioInquilino _inquilinoRepo;
    private readonly IRepositorioInmueble _inmuebleRepo;
    private readonly IRepositorioAuditoria _auditoriaRepo;

    public ContratoController(
        ILogger<ContratoController> logger,
        IRepositorioContrato contratoRepo,
        IRepositorioPago pagoRepo,
        IRepositorioInquilino inquilinoRepo,
        IRepositorioInmueble inmuebleRepo, 
        IRepositorioAuditoria auditoriaRepo)
    {
        _logger = logger;
        _contratoRepo = contratoRepo;
        _pagoRepo = pagoRepo;
        _inquilinoRepo = inquilinoRepo;
        _inmuebleRepo = inmuebleRepo;
        _auditoriaRepo = auditoriaRepo;
    }

    public IActionResult Index(DateTime? fechaInicio, DateTime? fechaFin, int? diasVencimiento)
    {
        IEnumerable<Contrato> listaContratos = new List<Contrato>();

        ViewBag.FechaInicio = null;
        ViewBag.FechaFin = null;
        ViewBag.DiasVencimiento = diasVencimiento;

        if (fechaInicio.HasValue && fechaFin.HasValue && fechaInicio <= fechaFin)
        {
            ViewBag.FechaInicio = fechaInicio.Value.ToString("yyyy-MM-dd");
            ViewBag.FechaFin = fechaFin.Value.ToString("yyyy-MM-dd");
            ViewBag.DiasVencimiento = null;

            listaContratos = _contratoRepo.ObtenerContratosVigentesPorRango(fechaInicio.Value, fechaFin.Value);
            TempData["Info"] = $"Mostrando contratos activos que se solapan entre {fechaInicio.Value:d} y {fechaFin.Value:d}.";
        }
        else if (diasVencimiento.HasValue && diasVencimiento.Value > 0)
        {
            ViewBag.FechaInicio = null;
            ViewBag.FechaFin = null;

            listaContratos = _contratoRepo.ObtenerContratosPorVencimiento(diasVencimiento.Value);
            TempData["Info"] = $"Mostrando contratos que vencen dentro de {diasVencimiento.Value} días o menos. ⚠️";
        }
        else
        {
            listaContratos = _contratoRepo.ObtenerContratos();
            ViewBag.DiasVencimiento = null;
        }

        return View(listaContratos);
    }

    [HttpGet]
    public IActionResult Agregar()
    {
        var inquilinos = _inquilinoRepo.ObtenerInquilinos()
            .Select(i => new SelectListItem { Value = i.id.ToString(), Text = i.NombreCompleto })
            .ToList();

        ViewBag.Inquilinos = inquilinos;
        ViewBag.InmueblesDisponibles = null;

        var nuevoContrato = new Contrato
        {
            DuracionEnMeses = 1,
            fecha_inicio = DateTime.Today
        };

        return View(nuevoContrato);
    }

    [HttpGet]
    public IActionResult BuscarInquilinoPorDni(string dni)
    {
        if (string.IsNullOrWhiteSpace(dni))
        {
            return Json(new List<object>());
        }

        var inquilinos = _inquilinoRepo.ObtenerInquilinos()
            .Where(i => i.dni.Contains(dni)) // búsqueda parcial
            .Select(i => new { i.id, nombreCompleto = i.NombreCompleto, dni = i.dni })
            .ToList();

        return Json(inquilinos);
    }

    [HttpGet]
    public IActionResult Detalles(int id)
    {
        var contratoSeleccionado = _contratoRepo.ObtenerContratoId(id);
        if (contratoSeleccionado == null)
        {
            return NotFound();
        }

        var auditorias = _auditoriaRepo.ObtenerAuditoriasPorTipo(TipoAuditoria.Contrato)
            .Where(a => a.id_registro_afectado == id)
            .OrderByDescending(a => a.fecha_hora)
            .ToList();

        ViewBag.Auditorias = auditorias;

        return View(contratoSeleccionado);
    }

    [HttpGet]
    public IActionResult Editar(int id)
    {
        var contrato = _contratoRepo.ObtenerContratoId(id);
        if (contrato == null)
        {
            return NotFound();
        }

        if (contrato.id_inquilino.HasValue)
        {
            contrato.Inquilino = _inquilinoRepo.ObtenerInquilinoId(contrato.id_inquilino.Value);
        }
        if (contrato.id_inmueble.HasValue)
        {
            contrato.Inmueble = _inmuebleRepo.ObtenerInmuebleId(contrato.id_inmueble.Value);
        }

        return View(contrato);
    }

    [HttpPost]
    public IActionResult Agregar(Contrato contrato, string actionType)
    {
        ViewBag.Inquilinos = _inquilinoRepo.ObtenerInquilinos()
            .Select(i => new SelectListItem { Value = i.id.ToString(), Text = i.NombreCompleto })
            .ToList();

        if (contrato.DuracionEnMeses <= 0)
        {
            ModelState.AddModelError("DuracionEnMeses", "La duración en meses debe ser mayor a cero.");
        }
        else if (contrato.fecha_inicio.HasValue)
        {
            contrato.fecha_fin = contrato.fecha_inicio.Value.AddMonths(contrato.DuracionEnMeses);
        }

        if (actionType == "BuscarInmuebles")
        {
            if (!contrato.fecha_inicio.HasValue || !contrato.fecha_fin.HasValue || contrato.fecha_inicio >= contrato.fecha_fin)
            {
                if (!ModelState.IsValid) { }
                else
                {
                    ModelState.AddModelError("", "Verifique la fecha de inicio y la duración del contrato.");
                }
                ViewBag.InmueblesDisponibles = null;
            }
            else
            {
                ViewBag.InmueblesDisponibles = _inmuebleRepo.BuscarDisponiblePorFecha(contrato.fecha_inicio.Value, contrato.fecha_fin.Value);
            }
            return View(contrato);
        }

        if (contrato.id_inmueble == 0)
            ModelState.AddModelError("id_inmueble", "Debe seleccionar un inmueble.");

        if (!contrato.monto_mensual.HasValue)
            ModelState.AddModelError("monto_mensual", "Debe ingresar un monto mensual.");

        if (ModelState.IsValid)
        {
            contrato.estado = 1;
            try
            {
                var idContrato = _contratoRepo.AgregarContrato(contrato);
                _auditoriaRepo.InsertarRegistroAuditoria(
                TipoAuditoria.Contrato,
                  idContrato,
                  AccionAuditoria.Crear,
                  User.Identity?.Name ?? "Anónimo"
              );
                if (contrato.id_inmueble.HasValue && contrato.id_inmueble.Value > 0)
                {
                    _inmuebleRepo.MarcarComoAlquilado(contrato.id_inmueble.Value);
                }
                TempData["Exito"] = "Contrato creado exitosamente.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al guardar en la base de datos: {ex.Message}");
            }
        }

        if (contrato.fecha_inicio.HasValue && contrato.fecha_fin.HasValue && contrato.fecha_inicio < contrato.fecha_fin)
        {
            ViewBag.InmueblesDisponibles = _inmuebleRepo.BuscarDisponiblePorFecha(contrato.fecha_inicio.Value, contrato.fecha_fin.Value);
        }
        else
        {
            ViewBag.InmueblesDisponibles = null;
        }

        return View(contrato);
    }

    [HttpGet]
    public IActionResult Cancelar(int id)
    {
        var contrato = _contratoRepo.ObtenerContratoId(id);

        if (contrato == null)
        {
            TempData["Error"] = "El contrato no fue encontrado.";
            return RedirectToAction("Index");
        }

        return View(contrato);
    }

    [HttpPost]
    public IActionResult Editar(Contrato contratoEditado)
    {
        if (ModelState.IsValid)
        {
            if (contratoEditado.id_inmueble.HasValue)
            {
                var contratosExistentes = _contratoRepo.ObtenerContratoPorInmueble(contratoEditado.id_inmueble.Value, contratoEditado.id);
                bool haySolapamiento = contratosExistentes.Any(c => contratoEditado.fecha_inicio <= c.fecha_fin && contratoEditado.fecha_fin >= c.fecha_inicio);

                if (haySolapamiento)
                {
                    ModelState.AddModelError("fecha_fin", "La fecha de finalización se solapa con otro contrato del mismo inmueble.");
                    if (contratoEditado.id_inquilino.HasValue)
                    {
                        contratoEditado.Inquilino = _inquilinoRepo.ObtenerInquilinoId(contratoEditado.id_inquilino.Value);
                    }

                    if (contratoEditado.id_inmueble.HasValue)
                    {
                        contratoEditado.Inmueble = _inmuebleRepo.ObtenerInmuebleId(contratoEditado.id_inmueble.Value);
                    }

                    return View(contratoEditado);
                }
            }
            _contratoRepo.ActualizarContrato(contratoEditado);
            TempData["Exito"] = "Contrato actualizado con éxito.";
            return RedirectToAction("Index");
        }

        if (contratoEditado.id_inquilino.HasValue)
        {
            contratoEditado.Inquilino = _inquilinoRepo.ObtenerInquilinoId(contratoEditado.id_inquilino.Value);
        }

        if (contratoEditado.id_inmueble.HasValue)
        {
            contratoEditado.Inmueble = _inmuebleRepo.ObtenerInmuebleId(contratoEditado.id_inmueble.Value);
        }

        return View(contratoEditado);
    }

    [HttpPost]
    public IActionResult Cancelar(int idContrato, DateTime fechaTerminacion)
    {
        var contrato = _contratoRepo.ObtenerContratoId(idContrato);

        if (contrato == null)
        {
            return Json(new { success = false, fechaTerminacionError = "El contrato no fue encontrado." });
        }
        if (!contrato.fecha_inicio.HasValue || !contrato.fecha_fin.HasValue)
        {
            return Json(new { success = false, fechaTerminacionError = "El contrato no tiene fechas de inicio o fin válidas." });
        }
        if (fechaTerminacion.Date >= contrato.fecha_fin.Value.Date)
        {
            return Json(new { success = false, fechaTerminacionError = "La fecha de terminación debe ser estrictamente anterior a la fecha de finalización original." });
        }

        if (fechaTerminacion.Date <= contrato.fecha_inicio.Value.Date)
        {
            return Json(new { success = false, fechaTerminacionError = "La fecha de terminación debe ser posterior a la fecha de inicio del contrato." });
        }

        try
        {
            decimal multaCalculada = CalcularMulta(contrato, fechaTerminacion);

            contrato.multa = multaCalculada;
            contrato.fecha_terminacion_anticipada = fechaTerminacion;
            contrato.estado = 0;

            _contratoRepo.ActualizarContrato(contrato);

            return Json(new
            {
                success = true,
                multa = contrato.multa.Value.ToString("C", new System.Globalization.CultureInfo("es-AR")),
                multaValor = contrato.multa.Value
            });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, error = $"Error al guardar la cancelación: {ex.Message}" });
        }
    }

    private decimal CalcularMulta(Contrato contrato, DateTime fechaTerminacion)
    {
        if (!contrato.fecha_fin.HasValue || !contrato.fecha_inicio.HasValue || !contrato.monto_mensual.HasValue)
        {
            return 0m;
        }

        var duracionTotalMeses = (contrato.fecha_fin.Value.Year - contrato.fecha_inicio.Value.Year) * 12 +
                                 (contrato.fecha_fin.Value.Month - contrato.fecha_inicio.Value.Month);
        var tiempoTranscurridoMeses = (fechaTerminacion.Year - contrato.fecha_inicio.Value.Year) * 12 +
                                      (fechaTerminacion.Month - contrato.fecha_inicio.Value.Month);

        decimal mesesMulta;
        if (tiempoTranscurridoMeses < (duracionTotalMeses / 2))
        {
            mesesMulta = 2;
        }
        else
        {
            mesesMulta = 1;
        }

        return contrato.monto_mensual.Value * mesesMulta;
    }

    [HttpGet]
    public IActionResult CalcularMultaAjax(int idContrato, string fechaTerminacion)
    {
        var contrato = _contratoRepo.ObtenerContratoId(idContrato);

        if (!DateTime.TryParse(fechaTerminacion, out DateTime fecha))
        {
            return Json(new { success = false, fechaTerminacionError = "Fecha inválida" });
        }

        if (contrato == null)
        {
            return Json(new { success = false, fechaTerminacionError = "Contrato no encontrado" });
        }
        if (!contrato.fecha_inicio.HasValue || !contrato.fecha_fin.HasValue)
        {
            return Json(new { success = false, fechaTerminacionError = "El contrato no tiene fechas de inicio o fin válidas." });
        }
        if (fecha.Date >= contrato.fecha_fin.Value.Date)
        {
            return Json(new { success = false, fechaTerminacionError = "La fecha debe ser ANTERIOR a la fecha de finalización original." });
        }

        if (fecha.Date <= contrato.fecha_inicio.Value.Date)
        {
            return Json(new { success = false, fechaTerminacionError = "La fecha debe ser POSTERIOR a la fecha de inicio del contrato." });
        }

        DateTime? fechaUltimoPago = _pagoRepo.ObtenerFechaUltimoPagoRealizado(idContrato);

        if (fechaUltimoPago.HasValue && fecha.Date <= fechaUltimoPago.Value.Date)
        {
            return Json(new
            {
                success = false,
                fechaTerminacionError = $"La fecha de terminación debe ser POSTERIOR al último pago registrado ({fechaUltimoPago.Value.ToShortDateString()})."
            });
        }

        var pagosPendientes = _pagoRepo.ObtenerPagosPendientes(idContrato);

        int mesesAdeudados = pagosPendientes.Count;
        if (contrato.monto_mensual.HasValue)
        {
            decimal montoMensual = contrato.monto_mensual.Value;
            decimal totalAdeudado = montoMensual * mesesAdeudados;
            decimal multaCalculada = CalcularMulta(contrato, fecha);
            return Json(new
            {
                success = true,
                multaTexto = multaCalculada.ToString("C", new System.Globalization.CultureInfo("es-AR")),
                multaValor = multaCalculada,
                mesesAdeudados = mesesAdeudados,
                totalAdeudadoTexto = totalAdeudado.ToString("C", new System.Globalization.CultureInfo("es-AR")),
                totalAdeudadoValor = totalAdeudado
            });
        }
        else
        {
            return Json(new
            {
                success = false,
                fechaTerminacionError = "El contrato no tiene un monto mensual definido para calcular la multa."
            });
        }
    }
}
