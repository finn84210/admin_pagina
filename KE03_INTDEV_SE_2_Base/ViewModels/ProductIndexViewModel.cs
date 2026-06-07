using DataAccessLayer.Models;

namespace KE03_INTDEV_SE_2_Base.ViewModels
{
    public class ProductIndexViewModel
    {
        public IEnumerable<Product> Products { get; set; } = new List<Product>();

        public string? SearchString { get; set; }

        public string? Category { get; set; }

        public IEnumerable<string> Categories { get; set; } = new List<string>();

        public bool LowStockOnly { get; set; }

        public int TotalProducts { get; set; }

        public int LowStockProducts { get; set; }
    }
}
