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

    public ContratoController(
        ILogger<ContratoController> logger,
        IRepositorioContrato contratoRepo,
        IRepositorioPago pagoRepo,
        IRepositorioInquilino inquilinoRepo,
        IRepositorioInmueble inmuebleRepo)
    {
        _logger = logger;
        _contratoRepo = contratoRepo;
        _pagoRepo = pagoRepo;
        _inquilinoRepo = inquilinoRepo;
        _inmuebleRepo = inmuebleRepo;
    }

    public IActionResult Index()
    {
        var listaContratos = _contratoRepo.ObtenerContratos();
        return View(listaContratos);
    }

    [HttpGet]
    public IActionResult Agregar()
    {
        var inquilinos = _inquilinoRepo.ObtenerInquilinos().Select(i => new SelectListItem { Value = i.id.ToString(), Text = i.NombreCompleto }).ToList();

        ViewBag.Inquilinos = inquilinos;
        ViewBag.InmueblesDisponibles = null;

        return View(new Contrato());
    }

    [HttpGet]
    public IActionResult Detalles(int id)
    {
        var contratoSeleccionado = _contratoRepo.ObtenerContratoId(id);
        var pagosDelContrato = _pagoRepo.ObtenerPagosPorContrato(id);
        ViewBag.Pagos = pagosDelContrato;
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
            .Select(i => new SelectListItem { Value = i.id.ToString(), Text = i.NombreCompleto }).ToList();

        if (actionType == "BuscarInmuebles")
        {
            if (!contrato.fecha_inicio.HasValue || !contrato.fecha_fin.HasValue)

            {
                ModelState.AddModelError("", "Debe ingresar ambas fechas para buscar inmuebles.");
                ViewBag.InmueblesDisponibles = null;
            }
            else if (contrato.fecha_inicio >= contrato.fecha_fin)
            {
                ModelState.AddModelError("", "La fecha de inicio debe ser anterior a la fecha de fin.");
                ViewBag.InmueblesDisponibles = null;
            }
            else
            {
                ViewBag.InmueblesDisponibles = _inmuebleRepo.BuscarDisponiblePorFecha(contrato.fecha_inicio.Value, contrato.fecha_fin.Value);
            }
            return View(contrato);
        }

        if (contrato.id_inmueble == 0 || !ModelState.IsValid)
        {
            ViewBag.InmueblesDisponibles = null;
            return View(contrato);
        }

        contrato.estado = 1;
        if (contrato.id_inmueble.HasValue)
        {
            _contratoRepo.AgregarContrato(contrato);
            _inmuebleRepo.MarcarComoAlquilado(contrato.id_inmueble.Value);
        }


        return RedirectToAction("Index");
    }
    [HttpPost]
    public IActionResult Editar(Contrato contratoEditado)
    {
        if (ModelState.IsValid)
        {
            if (contratoEditado.id_inmueble.HasValue)
            {
                var contratosExistentes = _contratoRepo.ObtenerContratoPorInmueble(contratoEditado.id_inmueble.Value, contratoEditado.id);
                bool haySolapamiento = contratosExistentes.Any(c =>contratoEditado.fecha_inicio <= c.fecha_fin && contratoEditado.fecha_fin >= c.fecha_inicio );

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




    // [HttpPost]
    // public IActionResult TerminarAnticipado(int id)
    // {
    //     var contrato = _contratoRepo.ObtenerContratoId(id);
    //     if (contrato == null)
    //     {
    //         return NotFound();
    //     }

    //     contrato.estado = 0;
    //     _contratoRepo.ActualizarContrato(contrato);
    //     TempData["Exito"] = "Contrato terminado con éxito";

    //     var inmueble = _inmuebleRepo.ObtenerInmuebleId(contrato.id_inmueble);
    //     if (inmueble != null)
    //     {
    //         inmueble.estado = 1;
    //         _inmuebleRepo.ActualizarInmueble(inmueble);
    //     }

    //     return RedirectToAction("Index");
    // }


}
