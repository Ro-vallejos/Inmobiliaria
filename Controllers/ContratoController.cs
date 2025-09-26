using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using _net_integrador.Models;
using _net_integrador.Repositorios;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;


namespace _net_integrador.Controllers;

[Authorize]
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
            .Where(i => i.dni.Contains(dni))
            .Select(i => new { i.id, nombreCompleto = i.NombreCompleto, dni = i.dni })
            .ToList();

        return Json(inquilinos);
    }

    [HttpGet]
    [Authorize(Policy = "Administrador")]
    public IActionResult Detalles(int id)
    {
        var contratoSeleccionado = _contratoRepo.ObtenerContratoId(id);
        if (contratoSeleccionado == null)
        {
            return NotFound();
        }

        // Auditoría de contrato
        var auditoriasContrato = _auditoriaRepo.ObtenerAuditoriasPorTipo(TipoAuditoria.Contrato)
            .Where(a => a.id_registro_afectado == id)
            .OrderByDescending(a => a.fecha_hora)
            .ToList();

        // Auditoría de pagos
        var pagosDelContrato = _pagoRepo.ObtenerPagosPorContrato(id).Select(p => p.id).ToList();
        var auditoriasPagos = _auditoriaRepo.ObtenerAuditoriasPorTipo(TipoAuditoria.Pago)
            .Where(a => pagosDelContrato.Contains(a.id_registro_afectado))
            .OrderByDescending(a => a.fecha_hora)
            .ToList();

        ViewBag.AuditoriasContrato = auditoriasContrato;
        ViewBag.AuditoriasPagos = auditoriasPagos;

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

    // 💡 Lógica Clave: Calcular DuracionEnMeses para precargar en el formulario
    if (contrato.fecha_inicio.HasValue && contrato.fecha_fin.HasValue)
    {
        int meses = ((contrato.fecha_fin.Value.Year - contrato.fecha_inicio.Value.Year) * 12) +
                    contrato.fecha_fin.Value.Month - contrato.fecha_inicio.Value.Month;
        
        // Ajuste simple: si termina el mismo día del mes, es la duración exacta.
        // Si no, podemos redondear o usar la duración original que se guardó.
        // Asumiendo que la duración guardada es la que importa:
        contrato.DuracionEnMeses = meses;
        
        // Si no tienes el campo en la DB y solo existe para el formulario, este es el cálculo.
        // Podrías necesitar un método para obtener la duración original si se guardó en meses.
    } else {
         contrato.DuracionEnMeses = 1; // Default
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
    // 💡 Lógica Clave: Recalcular fecha_fin en base a DuracionEnMeses
    if (contratoEditado.DuracionEnMeses <= 0)
    {
        ModelState.AddModelError("DuracionEnMeses", "La duración en meses debe ser mayor a cero.");
    }
    else if (contratoEditado.fecha_inicio.HasValue)
    {
        // 1. Recalcular la fecha de fin
        contratoEditado.fecha_fin = contratoEditado.fecha_inicio.Value.AddMonths(contratoEditado.DuracionEnMeses);
    }
    // Fin de la Lógica Clave de cálculo
    
    // Si la fecha de inicio no tiene valor, también es un error de modelo.
    if (!contratoEditado.fecha_inicio.HasValue)
    {
         ModelState.AddModelError("fecha_inicio", "La fecha de inicio es requerida.");
    }
    

    if (ModelState.IsValid)
    {
        if (contratoEditado.id_inmueble.HasValue)
        {
            // La validación de solapamiento ahora usa la nueva fecha_fin calculada
            var contratosExistentes = _contratoRepo.ObtenerContratoPorInmueble(contratoEditado.id_inmueble.Value, contratoEditado.id);
            // ... (el resto de tu lógica de solapamiento sigue igual, usando contratoEditado.fecha_fin) ...
            
            bool haySolapamiento = contratosExistentes.Any(c => contratoEditado.fecha_inicio <= c.fecha_fin && contratoEditado.fecha_fin >= c.fecha_inicio);

            if (haySolapamiento)
            {
                ModelState.AddModelError("fecha_fin", "La nueva fecha de finalización se solapa con otro contrato del mismo inmueble.");
                // ... (Recargar Inquilino e Inmueble para el retorno de la vista) ...
                // ... (Es importante recargar el modelo si falla) ...
                return RecargarVistaEditar(contratoEditado);
            }
        }
        
        _contratoRepo.ActualizarContrato(contratoEditado);
        TempData["Exito"] = "Contrato actualizado con éxito.";
        return RedirectToAction("Index");
    }

    // Recargar Inquilino e Inmueble para el retorno de la vista si ModelState es inválido
    return RecargarVistaEditar(contratoEditado);
}

// 💡 Función auxiliar para evitar código repetitivo al retornar la vista con errores
private IActionResult RecargarVistaEditar(Contrato contrato)
{
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
            // ✅ Calcular multa
            decimal multaCalculada = CalcularMulta(contrato, fechaTerminacion);

            contrato.multa = multaCalculada;
            contrato.fecha_terminacion_anticipada = fechaTerminacion;
            contrato.estado = 0; // ❌ estado cambiado a cancelado

            // ✅ Guardar en DB
            _contratoRepo.ActualizarContrato(contrato);

            // ✅ Auditoría correcta
            _auditoriaRepo.InsertarRegistroAuditoria(
                TipoAuditoria.Contrato,
                contrato.id,
                AccionAuditoria.Anular, // ✅ Ahora marca como Anulado
                User.Identity?.Name ?? "Anónimo"
            );

            // ✅ Registrar multa como pago pendiente
            var nuevoPago = new Pago
            {
                id_contrato = contrato.id,
                nro_pago = 999, // marcador especial para multa
                fecha_pago = null,
                estado = EstadoPago.pendiente,
                concepto = "Multa por rescisión anticipada"
            };

            _pagoRepo.AgregarPago(nuevoPago);

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

        var duracionTotalDias = (contrato.fecha_fin.Value - contrato.fecha_inicio.Value).TotalDays;
        var tiempoTranscurridoDias = (fechaTerminacion - contrato.fecha_inicio.Value).TotalDays;

        decimal duracionTotalMeses = (decimal)duracionTotalDias / 30.4375m;
        decimal tiempoTranscurridoMeses = (decimal)tiempoTranscurridoDias / 30.4375m;

        decimal mesesMulta;

        if (tiempoTranscurridoMeses < (duracionTotalMeses / 2m))
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

        try
        {
            // Calcular meses transcurridos desde inicio hasta fecha de terminación
            var mesesTranscurridos = ((fecha.Year - contrato.fecha_inicio.Value.Year) * 12) + fecha.Month - contrato.fecha_inicio.Value.Month;
            if (fecha.Day >= contrato.fecha_inicio.Value.Day)
            {
                mesesTranscurridos += 1;
            }

            int pagosRealizados = _pagoRepo.ContarPagosRealizados(idContrato);
            int mesesAdeudados = mesesTranscurridos - pagosRealizados;
            if (mesesAdeudados < 0)
            {
                mesesAdeudados = 0;
            }

            if (!contrato.monto_mensual.HasValue)
            {
                return Json(new
                {
                    success = false,
                    fechaTerminacionError = "El contrato no tiene un monto mensual definido para calcular la multa."
                });
            }

            decimal montoMensual = contrato.monto_mensual.Value;
            decimal multaCalculada = CalcularMulta(contrato, fecha);
            decimal totalAdeudado = mesesAdeudados * montoMensual;

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
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"EXCEPCIÓN INTERNA: {ex.Message}");
            return Json(new { success = false, error = ex.Message, stack = ex.StackTrace });
        }
    }

    [HttpPost]
    public IActionResult Renovar(Contrato contrato)
    {
        if (contrato.DuracionEnMeses <= 0)
        {
            ModelState.AddModelError("DuracionEnMeses", "La duración en meses debe ser mayor a cero.");
        }
        else if (contrato.fecha_inicio.HasValue)
        {
            contrato.fecha_fin = contrato.fecha_inicio.Value.AddMonths(contrato.DuracionEnMeses);
        }
        if (!contrato.fecha_inicio.HasValue || !contrato.fecha_fin.HasValue)
        {
            ModelState.AddModelError("", "Debe definir la fecha de inicio y la duración.");
        }
        else if (contrato.id_inmueble.HasValue && contrato.id_inmueble.Value > 0)
        {
            var inmueblesDisponibles = _inmuebleRepo.BuscarDisponiblePorFecha(
                contrato.fecha_inicio.Value,
                contrato.fecha_fin.Value
            );

            bool inmuebleOcupado = inmueblesDisponibles
            .Any(i => i.id == contrato.id_inmueble.Value) == false;

            if (inmuebleOcupado)
            {
                ModelState.AddModelError("", "El inmueble seleccionado NO está disponible para las fechas de la renovación.");
                ModelState.AddModelError("fecha_inicio", "El inmueble no está disponible en la fecha ingresada");
            }
        }
        if (contrato.id_inmueble == 0 || !contrato.id_inmueble.HasValue)
            ModelState.AddModelError("id_inmueble", "El inmueble no está definido.");

        if (!contrato.monto_mensual.HasValue)
            ModelState.AddModelError("monto_mensual", "El monto mensual no está definido.");
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

                TempData["Exito"] = "Contrato renovado y creado exitosamente.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al guardar en la base de datos: {ex.Message}");
            }
        }

        var contratoOriginal = _contratoRepo.ObtenerContratoId(contrato.id);

        if (contratoOriginal?.id_inquilino != null)
        {
            ViewBag.InquilinoNombre = _inquilinoRepo
                .ObtenerInquilinoId(contratoOriginal.id_inquilino.Value)?.NombreCompleto ?? "Sin asignar";
        }
        else
        {
            ViewBag.InquilinoNombre = "Sin asignar";
        }

        if (contratoOriginal?.id_inmueble != null)
        {
            ViewBag.InmuebleDireccion = _inmuebleRepo
                .ObtenerInmuebleId(contratoOriginal.id_inmueble.Value)?.direccion ?? "Sin asignar";
        }
        else
        {
            ViewBag.InmuebleDireccion = "Sin asignar";
        }

        return View(contrato);
    }
    [HttpGet]
    public IActionResult Renovar(int id)
    {
        var contratoOriginal = _contratoRepo.ObtenerContratoId(id);

        if (contratoOriginal == null)
        {
            TempData["Error"] = "Contrato original no encontrado.";
            return RedirectToAction("Index");
        }

        var nuevoContrato = new Contrato
        {
            id_inquilino = contratoOriginal.id_inquilino,
            id_inmueble = contratoOriginal.id_inmueble,
            monto_mensual = contratoOriginal.monto_mensual,
            id = contratoOriginal.id,

            DuracionEnMeses = 1,
            fecha_inicio = contratoOriginal.fecha_fin.HasValue ? contratoOriginal.fecha_fin.Value.AddDays(1) : DateTime.Today
        };

        if (contratoOriginal.id_inquilino.HasValue)
        {
            var inquilino = _inquilinoRepo.ObtenerInquilinoId(contratoOriginal.id_inquilino.Value);
            ViewBag.InquilinoNombre = inquilino?.NombreCompleto ?? "Sin asignar";
        }
        else
        {
            ViewBag.InquilinoNombre = "Sin asignar";
        }

        if (contratoOriginal.id_inmueble.HasValue)
        {
            var inmueble = _inmuebleRepo.ObtenerInmuebleId(contratoOriginal.id_inmueble.Value);
            ViewBag.InmuebleDireccion = inmueble?.direccion ?? "Sin asignar";
        }
        else
        {
            ViewBag.InmuebleDireccion = "Sin asignar";
        }

        return View(nuevoContrato);
    }

}
