namespace StockWeb.Models
{
    public class RemitoViewModel
    {
        public string TipoMovimiento { get; set; } = "Ingreso";
        public string PersonaRetira { get; set; } = string.Empty;
        public string? Observaciones { get; set; }

        // Lista de productos agregados al remito
        public List<ItemRemito> Items { get; set; } = new List<ItemRemito>();
    }

    public class ItemRemito
    {
        public long ProductoId { get; set; }
        public decimal Cantidad { get; set; }
    }
}