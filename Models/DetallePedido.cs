using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_Restaurante.Models
{
    public class DetallePedido
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PedidoId { get; set; }

        [ForeignKey("PedidoId")]
        public Pedido? Pedido { get; set; }

        [Required]
        public int ProductoId { get; set; }

        [ForeignKey("ProductoId")]
        public Producto? Producto { get; set; }

        [Required]
        [Range(1, 100)]
        public int Cantidad { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioUnitario { get; set; }
    }
}
