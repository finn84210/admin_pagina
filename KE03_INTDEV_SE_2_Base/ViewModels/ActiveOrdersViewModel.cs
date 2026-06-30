using DataAccessLayer.Models;

namespace KE03_INTDEV_SE_2_Base.ViewModels
{
    public class ActiveOrdersViewModel
    {
        public IEnumerable<Order> Orders { get; set; } = new List<Order>();

        public string? StatusFilter { get; set; }

        public int TotalActiveOrders { get; set; }

        public int IncomingOrders { get; set; }

        public int PickedOrders { get; set; }

        public int OrdersWithDeliveryPerson { get; set; }
    }
}
