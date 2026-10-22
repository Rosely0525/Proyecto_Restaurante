using System.ComponentModel.DataAnnotations;

namespace Proyecto_Restaurante.Models
{
    public class Mesa
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El número de mesa es obligatorio.")]
        [Range(1, 100, ErrorMessage = "El número de mesa debe estar entre 1 y 100.")]
        [Display(Name = "Número de Mesa")]
        public int Numero { get; set; }

        [Required(ErrorMessage = "La capacidad es obligatoria.")]
        [Range(1, 20, ErrorMessage = "La capacidad debe ser entre 1 y 20 personas.")]
        [Display(Name = "Capacidad (Personas)")]
        public int Capacidad { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio.")]
        public string Estado { get; set; } = "Disponible"; // Opciones: Disponible, Ocupada, Reservada
    }
}
