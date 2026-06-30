using DataAccessLayer.Models;

namespace KE03_INTDEV_SE_2_Base.ViewModels
{
    public class HomeOrderOverviewViewModel
    {
        public int ActiveOrders { get; set; }

        public int ImportantOrders { get; set; }

        public int NewOrders { get; set; }

        public int PickedOrders { get; set; }

        public int OrdersOnTheWay { get; set; }

        public int DeliveredToday { get; set; }

        public IEnumerable<Order> ImportantOrderList { get; set; } = new List<Order>();
    }
}
