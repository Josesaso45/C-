using DSW1_T2_JOSE_MONTERO.Api.Models;
using DSW1_T2_JOSE_MONTERO.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace DSW1_T2_JOSE_MONTERO.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CursosController : ControllerBase
{
    private readonly ICursoRepository _cursoRepository;

    public CursosController(ICursoRepository cursoRepository)
    {
        _cursoRepository = cursoRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Curso>>> GetCursos()
    {
        try
        {
            List<Curso> cursos = await _cursoRepository.ListarCursosAsync();
            return Ok(cursos);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ProcesoResponse
            {
                Exito = false,
                Mensaje = $"No se pudo listar los cursos: {ex.Message}"
            });
        }
    }

    [HttpGet("buscar")]
    public async Task<ActionResult<IEnumerable<Curso>>> BuscarCursos([FromQuery] string iniciales)
    {
        if (string.IsNullOrWhiteSpace(iniciales))
        {
            return Ok(new List<Curso>());
        }

        try
        {
            List<Curso> cursos = await _cursoRepository.FiltrarCursosAsync(iniciales);
            return Ok(cursos);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ProcesoResponse
            {
                Exito = false,
                Mensaje = $"No se pudo filtrar los cursos: {ex.Message}"
            });
        }
    }

    [HttpGet("{idCurso:int}")]
    public async Task<ActionResult<Curso>> GetCurso(int idCurso)
    {
        try
        {
            Curso? curso = await _cursoRepository.ObtenerCursoPorIdAsync(idCurso);

            if (curso is null)
            {
                return NotFound(new ProcesoResponse
                {
                    Exito = false,
                    Mensaje = "No se encontro el curso solicitado."
                });
            }

            return Ok(curso);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ProcesoResponse
            {
                Exito = false,
                Mensaje = $"No se pudo obtener el curso: {ex.Message}"
            });
        }
    }

    [HttpPost("dar-baja")]
    public async Task<ActionResult<ProcesoResponse>> DarDeBaja([FromBody] DarBajaCursoRequest request)
    {
        if (request.IdCurso <= 0)
        {
            return BadRequest(new ProcesoResponse
            {
                Exito = false,
                Mensaje = "Debe indicar un curso valido."
            });
        }

        try
        {
            ProcesoResponse response = await _cursoRepository.DarDeBajaAsync(
                request.IdCurso,
                request.CantidadEstudiantes);

            if (!response.Exito)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ProcesoResponse
            {
                Exito = false,
                Mensaje = $"No se pudo procesar la baja: {ex.Message}"
            });
        }
    }
}
