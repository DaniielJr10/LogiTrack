
using LogiTrack.Dominio.Entidades.Inventario;

namespace LogiTrack.Dominio.Entidades.Pedidos;

/// <summary>
/// Representa cada producto incluido en un pedido.
/// </summary>
public class DetallePedido
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid PedidoId { get; set; }

    public Pedido? Pedido { get; set; }

    public Guid ProductoId { get; set; }

    public Producto? Producto { get; set; }

    public int Cantidad { get; set; }

    public decimal PrecioUnitario { get; set; }

    public decimal Subtotal => Cantidad * PrecioUnitario;
}