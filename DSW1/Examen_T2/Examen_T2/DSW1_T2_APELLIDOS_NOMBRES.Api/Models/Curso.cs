namespace DSW1_T2_JOSE_MONTERO.Api.Models;

public class Curso
{
    public int IdBaja { get; set; }
    public int IdCurso { get; set; }
    public string NombreCurso { get; set; } = string.Empty;
    public int Aforo { get; set; }
    public DateTime FechaCreacion { get; set; }
}
