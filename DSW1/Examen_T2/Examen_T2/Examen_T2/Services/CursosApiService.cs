using System.Net.Http.Json;
using DSW1_T2_JOSE_MONTERO.Cliente.Models;

namespace DSW1_T2_JOSE_MONTERO.Cliente.Services;

public class CursosApiService
{
    private readonly HttpClient _httpClient;

    public CursosApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<CursoViewModel>> BuscarCursosAsync(string iniciales)
    {
        HttpResponseMessage response =
            await _httpClient.GetAsync($"api/cursos/buscar?iniciales={Uri.EscapeDataString(iniciales)}");

        if (!response.IsSuccessStatusCode)
        {
            string error = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"La API no pudo listar cursos. {error}");
        }

        return await response.Content.ReadFromJsonAsync<List<CursoViewModel>>()
            ?? new List<CursoViewModel>();
    }

    public async Task<CursoBajaViewModel?> ObtenerCursoAsync(int idCurso)
    {
        HttpResponseMessage response = await _httpClient.GetAsync($"api/cursos/{idCurso}");

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }

        if (!response.IsSuccessStatusCode)
        {
            string error = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"La API no pudo obtener el curso. {error}");
        }

        CursoViewModel? curso = await response.Content.ReadFromJsonAsync<CursoViewModel>();

        if (curso is null)
        {
            return null;
        }

        return new CursoBajaViewModel
        {
            IdBaja = curso.IdBaja,
            IdCurso = curso.IdCurso,
            NombreCurso = curso.NombreCurso,
            Aforo = curso.Aforo,
            FechaCreacion = curso.FechaCreacion
        };
    }

    public async Task<ProcesoResponseViewModel> DarDeBajaAsync(DarBajaCursoRequest request)
    {
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync("api/cursos/dar-baja", request);

        ProcesoResponseViewModel? result =
            await response.Content.ReadFromJsonAsync<ProcesoResponseViewModel>();

        if (result is not null)
        {
            return result;
        }

        return new ProcesoResponseViewModel
        {
            Exito = response.IsSuccessStatusCode,
            Mensaje = response.IsSuccessStatusCode
                ? "Proceso ejecutado correctamente."
                : "No se pudo procesar la solicitud en la API."
        };
    }
}
