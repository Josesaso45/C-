using DSW1_T2_JOSE_MONTERO.Api.Models;

namespace DSW1_T2_JOSE_MONTERO.Api.Repositories;

public interface ICursoRepository
{
    Task<List<Curso>> ListarCursosAsync();
    Task<List<Curso>> FiltrarCursosAsync(string iniciales);
    Task<Curso?> ObtenerCursoPorIdAsync(int idCurso);
    Task<ProcesoResponse> DarDeBajaAsync(int idCurso, int cantidadEstudiantes);
}
