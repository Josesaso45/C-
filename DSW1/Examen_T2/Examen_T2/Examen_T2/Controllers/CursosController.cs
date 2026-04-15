using DSW1_T2_JOSE_MONTERO.Cliente.Models;
using DSW1_T2_JOSE_MONTERO.Cliente.Services;
using Microsoft.AspNetCore.Mvc;

namespace DSW1_T2_JOSE_MONTERO.Cliente.Controllers;

public class CursosController : Controller
{
    private readonly CursosApiService _cursosApiService;

    public CursosController(CursosApiService cursosApiService)
    {
        _cursosApiService = cursosApiService;
    }

    [HttpGet]
    public async Task<IActionResult> IndexCursos(string? iniciales, int page = 1)
    {
        CursoListadoViewModel model = new()
        {
            Initiales = iniciales?.Trim() ?? string.Empty
        };

        ViewBag.Mensaje = TempData["Mensaje"]?.ToString();
        ViewBag.MensajeError = TempData["MensajeError"]?.ToString();

        if (string.IsNullOrWhiteSpace(model.Initiales))
        {
            return View(model);
        }

        List<CursoViewModel> cursos;
        try
        {
            cursos = await _cursosApiService.BuscarCursosAsync(model.Initiales);
        }
        catch (Exception ex)
        {
            ViewBag.MensajeError = ex.Message;
            return View(model);
        }

        const int pageSize = 4;
        model.TotalRegistros = cursos.Count;
        model.TotalPaginas = (int)Math.Ceiling(model.TotalRegistros / (double)pageSize);

        page = Math.Max(1, page);
        if (model.TotalPaginas > 0)
        {
            page = Math.Min(page, model.TotalPaginas);
        }

        model.Page = page;
        model.Cursos = cursos
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> DarDeBajaCurso(int idCurso, string? iniciales)
    {
        CursoBajaViewModel? curso;
        try
        {
            curso = await _cursosApiService.ObtenerCursoAsync(idCurso);
        }
        catch (Exception ex)
        {
            TempData["MensajeError"] = ex.Message;
            return RedirectToAction(nameof(IndexCursos), new { iniciales });
        }

        if (curso is null)
        {
            TempData["MensajeError"] = "No se encontro el curso solicitado.";
            return RedirectToAction(nameof(IndexCursos), new { iniciales });
        }

        curso.InitialesBusqueda = iniciales?.Trim() ?? string.Empty;
        ViewBag.Mensaje = TempData["Mensaje"]?.ToString();
        return View(curso);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DarDeBajaCurso(CursoBajaViewModel model)
    {
        ProcesoResponseViewModel response;
        try
        {
            response = await _cursosApiService.DarDeBajaAsync(new DarBajaCursoRequest
            {
                IdCurso = model.IdCurso,
                CantidadEstudiantes = model.Aforo
            });
        }
        catch (Exception ex)
        {
            ViewBag.Mensaje = ex.Message;
            return View(model);
        }

        if (response.Exito)
        {
            TempData["Mensaje"] = response.Mensaje;
            return RedirectToAction(nameof(IndexCursos), new { iniciales = model.InitialesBusqueda });
        }

        ViewBag.Mensaje = response.Mensaje;
        return View(model);
    }
}
