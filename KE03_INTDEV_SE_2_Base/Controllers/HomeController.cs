using System.Diagnostics;
using DataAccessLayer;
using KE03_INTDEV_SE_2_Base.Models;
using KE03_INTDEV_SE_2_Base.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KE03_INTDEV_SE_2_Base.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MatrixIncDbContext _context;

        public HomeController(ILogger<HomeController> logger, MatrixIncDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var activeStatuses = new[] { "Nieuw", "In behandeling", "Gepickt", "Nog te leveren", "Onderweg", "Doorgegeven aan bezorger" };
            var importantStatuses = new[] { "Nieuw", "In behandeling", "Gepickt", "Nog te leveren", "Onderweg" };
            var today = DateTime.Today;

            var activeOrders = await _context.Orders
                .Include(order => order.Customer)
                .Include(order => order.Products)
                .Where(order => activeStatuses.Contains(order.Status))
                .OrderBy(order => order.Status == "Onderweg" ? 0 : order.Status == "Gepickt" ? 1 : order.Status == "Nieuw" ? 2 : 3)
                .ThenBy(order => order.OrderDate)
                .ToListAsync();

            var importantOrders = activeOrders
                .Where(order => importantStatuses.Contains(order.Status))
                .Take(5)
                .ToList();

            var model = new HomeOrderOverviewViewModel
            {
                ActiveOrders = activeOrders.Count,
                ImportantOrders = activeOrders.Count(order => importantStatuses.Contains(order.Status)),
                NewOrders = activeOrders.Count(order => order.Status == "Nieuw"),
                PickedOrders = activeOrders.Count(order => order.Status == "Gepickt"),
                OrdersOnTheWay = activeOrders.Count(order => order.Status == "Onderweg"),
                DeliveredToday = await _context.Orders.CountAsync(order => order.Status == "Geleverd" && order.SentToDeliveryAt.HasValue && order.SentToDeliveryAt.Value.Date == today),
                ImportantOrderList = importantOrders
            };

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
