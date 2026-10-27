using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_Restaurante.Models
{
    public class Pedido
    {
        [Key] 
        public int Id { get; set; }

        [Required]
        [Display(Name = "Mesa")]
        public int MesaId { get; set; }

        [ForeignKey("MesaId")]
        public Mesa? Mesa { get; set; }

        public DateTime FechaHora { get; set; } = DateTime.Now;

        [Required]
        public string Estado { get; set; } = "Pendiente";

        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; } = 0;

        public List<DetallePedido> Detalles { get; set; } = new List<DetallePedido>();
    }
}
