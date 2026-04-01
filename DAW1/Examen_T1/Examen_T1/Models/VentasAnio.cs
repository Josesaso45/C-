namespace Examen_T1.Models
{
    public class VentasAnio
    {
        public string NumVta { get; set; } = string.Empty;
        public DateTime FecVta { get; set; }
        public string NomCli { get; set; } = string.Empty;
        public string NomVen { get; set; } = string.Empty;
        public decimal TotVta { get; set; }
    }
}
