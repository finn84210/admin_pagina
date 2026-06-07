using DataAccessLayer;
using DataAccessLayer.Interfaces;
using KE03_INTDEV_SE_2_Base.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KE03_INTDEV_SE_2_Base.Controllers
{
    public class AdminController : Controller
    {
        private const int LowStockThreshold = 5;
        private readonly MatrixIncDbContext _context;
        private readonly IProductRepository _productRepository;

        public AdminController(MatrixIncDbContext context, IProductRepository productRepository)
        {
            _context = context;
            _productRepository = productRepository;
        }

        public async Task<IActionResult> Index()
        {
            var products = _productRepository.GetAllProducts().ToList();
            var activeOrders = await _context.Orders
                .Include(order => order.Customer)
                .Include(order => order.Products)
                .Where(order => order.Status != "Afgerond" && order.Status != "Geannuleerd")
                .OrderBy(order => order.SentToDeliveryAt.HasValue)
                .ThenByDescending(order => order.OrderDate)
                .ToListAsync();

            var model = new AdminDashboardViewModel
            {
                TotalProducts = products.Count,
                LowStockProducts = products.Count(product => product.Stock < LowStockThreshold),
                ActiveOrders = activeOrders.Count,
                IncomingOrders = activeOrders.Count(order => order.Source == "Externe website"),
                OrdersWithDeliveryPerson = activeOrders.Count(order => !string.IsNullOrWhiteSpace(order.DeliveryPerson)),
                RecentActiveOrders = activeOrders.Take(5).ToList()
            };

            return View(model);
        }
    }
}
