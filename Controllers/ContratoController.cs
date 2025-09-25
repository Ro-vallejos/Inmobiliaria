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
        _contratoRepo.AgregarContrato(contrato);
        _inmuebleRepo.MarcarComoAlquilado(contrato.id_inmueble);

        return RedirectToAction("Index");
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
