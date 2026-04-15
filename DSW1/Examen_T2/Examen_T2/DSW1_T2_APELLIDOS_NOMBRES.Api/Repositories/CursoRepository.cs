using System.Data;
using DSW1_T2_JOSE_MONTERO.Api.Models;
using Microsoft.Data.SqlClient;

namespace DSW1_T2_JOSE_MONTERO.Api.Repositories;

public class CursoRepository : ICursoRepository
{
    private readonly string _connectionString;

    public CursoRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("cnBDCURSOS2026API") ?? string.Empty;
    }

    public async Task<List<Curso>> ListarCursosAsync()
    {
        List<Curso> cursos = new();

        await using SqlConnection connection = new(_connectionString);
        await using SqlCommand command = new("usp_listar_cursos_disponibles", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        await connection.OpenAsync();
        await using SqlDataReader reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            cursos.Add(MapearCurso(reader));
        }

        return cursos;
    }

    public async Task<List<Curso>> FiltrarCursosAsync(string iniciales)
    {
        List<Curso> cursos = new();

        await using SqlConnection connection = new(_connectionString);
        await using SqlCommand command = new("usp_filtrar_cursos_por_iniciales", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.AddWithValue("@iniciales", iniciales.Trim());

        await connection.OpenAsync();
        await using SqlDataReader reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            cursos.Add(MapearCurso(reader));
        }

        return cursos;
    }

    public async Task<Curso?> ObtenerCursoPorIdAsync(int idCurso)
    {
        await using SqlConnection connection = new(_connectionString);
        await using SqlCommand command = new("usp_obtener_curso_por_id", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.AddWithValue("@id_curso", idCurso);

        await connection.OpenAsync();
        await using SqlDataReader reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return MapearCurso(reader);
        }

        return null;
    }

    public async Task<ProcesoResponse> DarDeBajaAsync(int idCurso, int cantidadEstudiantes)
    {
        await using SqlConnection connection = new(_connectionString);
        await using SqlCommand command = new("usp_dar_baja_curso", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.AddWithValue("@id_curso", idCurso);
        command.Parameters.AddWithValue("@cantidad_estudiantes", cantidadEstudiantes);

        SqlParameter pExito = new("@exito", SqlDbType.Bit)
        {
            Direction = ParameterDirection.Output
        };
        SqlParameter pMensaje = new("@mensaje", SqlDbType.VarChar, 200)
        {
            Direction = ParameterDirection.Output
        };

        command.Parameters.Add(pExito);
        command.Parameters.Add(pMensaje);

        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();

        return new ProcesoResponse
        {
            Exito = Convert.ToBoolean(pExito.Value),
            Mensaje = Convert.ToString(pMensaje.Value) ?? "Proceso ejecutado."
        };
    }

    private static Curso MapearCurso(SqlDataReader reader)
    {
        return new Curso
        {
            IdBaja = Convert.ToInt32(reader["id_baja"]),
            IdCurso = Convert.ToInt32(reader["id_curso"]),
            NombreCurso = Convert.ToString(reader["nombre_curso"]) ?? string.Empty,
            Aforo = Convert.ToInt32(reader["aforo"]),
            FechaCreacion = Convert.ToDateTime(reader["fecha_creacion"])
        };
    }
}
