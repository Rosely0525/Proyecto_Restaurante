using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_Restaurante.Models
{
    public class Producto
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del producto es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El precio es obligatorio.")]
        [Range(0.01, 10000.00, ErrorMessage = "El precio debe ser mayor a 0.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Precio { get; set; }

        [Required(ErrorMessage = "La categoría es obligatoria.")]
        public string Categoria { get; set; } = string.Empty;

        // Permite marcar el producto como agotado/disponible
        public bool Disponible { get; set; } = true;
    }
}
