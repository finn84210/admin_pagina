using DataAccessLayer.Models;

namespace KE03_INTDEV_SE_2_Base.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalProducts { get; set; }

        public int LowStockProducts { get; set; }

        public int ActiveOrders { get; set; }

        public int IncomingOrders { get; set; }

        public int PickedOrders { get; set; }

        public int OrdersWithDeliveryPerson { get; set; }

        public IEnumerable<Order> RecentActiveOrders { get; set; } = new List<Order>();
    }
}
