using System.ComponentModel.DataAnnotations;

namespace Examen_T1.Models
{
    public class Articulo
    {
        [Display(Name = "Codigo")]
        [Required(ErrorMessage = "El codigo es obligatorio")]
        [StringLength(5, MinimumLength = 5, ErrorMessage = "El codigo debe tener 5 caracteres")]
        public string CodArt { get; set; } = string.Empty;

        [Display(Name = "Articulo")]
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(50)]
        public string NomArt { get; set; } = string.Empty;

        [Display(Name = "Unidad Medida")]
        [Required(ErrorMessage = "La unidad de medida es obligatoria")]
        [StringLength(25)]
        public string UniMed { get; set; } = string.Empty;

        [Display(Name = "Precio")]
        [Range(0.01, 99999.99, ErrorMessage = "Ingrese un precio valido")]
        public decimal PreArt { get; set; }

        [Display(Name = "Stock")]
        [Range(0, int.MaxValue, ErrorMessage = "Ingrese un stock valido")]
        public int StkArt { get; set; }

        [Display(Name = "Categoria")]
        [Range(1, int.MaxValue, ErrorMessage = "Seleccione una categoria")]
        public int CodCat { get; set; }

        public string NomCat { get; set; } = string.Empty;
    }
}
