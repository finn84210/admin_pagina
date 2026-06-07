using DataAccessLayer.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace KE03_INTDEV_SE_2_Base.ViewModels
{
    public class OrderHistoryViewModel
    {
        public IEnumerable<Order> Orders { get; set; } = new List<Order>();

        public IEnumerable<SelectListItem> Customers { get; set; } = new List<SelectListItem>();

        public int? CustomerId { get; set; }

        public string? SearchTerm { get; set; }

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        public int TotalOrders { get; set; }
    }
}
