namespace DSW1_T2_JOSE_MONTERO.Cliente.Models;

public class CursoListadoViewModel
{
    public List<CursoViewModel> Cursos { get; set; } = new();
    public string Initiales { get; set; } = string.Empty;
    public int Page { get; set; } = 1;
    public int TotalPaginas { get; set; }
    public int TotalRegistros { get; set; }
}
