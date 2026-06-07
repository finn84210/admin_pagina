using DataAccessLayer;
using KE03_INTDEV_SE_2_Base.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace KE03_INTDEV_SE_2_Base.Controllers
{
    public class OrdersController : Controller
    {
        private readonly MatrixIncDbContext _context;

        public OrdersController(MatrixIncDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? customerId, string? searchTerm, DateTime? dateFrom, DateTime? dateTo)
        {
            var ordersQuery = _context.Orders
                .Include(order => order.Customer)
                .Include(order => order.Products)
                .AsQueryable();

            if (customerId.HasValue)
            {
                ordersQuery = ordersQuery.Where(order => order.CustomerId == customerId.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var normalizedSearchTerm = searchTerm.Trim();
                var searchPattern = $"%{normalizedSearchTerm}%";
                var isOrderNumber = int.TryParse(normalizedSearchTerm, out var orderNumber);

                ordersQuery = ordersQuery.Where(order =>
                    (isOrderNumber && order.Id == orderNumber) ||
                    EF.Functions.Like(order.Customer.Name, searchPattern) ||
                    EF.Functions.Like(order.Customer.Address, searchPattern));
            }

            if (dateFrom.HasValue)
            {
                var fromDate = dateFrom.Value.Date;
                ordersQuery = ordersQuery.Where(order => order.OrderDate >= fromDate);
            }

            if (dateTo.HasValue)
            {
                var toDateExclusive = dateTo.Value.Date.AddDays(1);
                ordersQuery = ordersQuery.Where(order => order.OrderDate < toDateExclusive);
            }

            var customers = await _context.Customers
                .OrderBy(customer => customer.Name)
                .Select(customer => new SelectListItem
                {
                    Value = customer.Id.ToString(),
                    Text = customer.Name,
                    Selected = customerId == customer.Id
                })
                .ToListAsync();

            var orders = await ordersQuery
                .OrderByDescending(order => order.OrderDate)
                .ThenByDescending(order => order.Id)
                .ToListAsync();

            var model = new OrderHistoryViewModel
            {
                Orders = orders,
                Customers = customers,
                CustomerId = customerId,
                SearchTerm = searchTerm,
                DateFrom = dateFrom,
                DateTo = dateTo,
                TotalOrders = orders.Count
            };

            return View(model);
        }

        public async Task<IActionResult> Active(string? statusFilter)
        {
            var ordersQuery = _context.Orders
                .Include(order => order.Customer)
                .Include(order => order.Products)
                .Where(order => order.Status != "Afgerond" && order.Status != "Geannuleerd")
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(statusFilter))
            {
                ordersQuery = ordersQuery.Where(order => order.Status == statusFilter);
            }

            var allActiveOrders = await _context.Orders
                .Where(order => order.Status != "Afgerond" && order.Status != "Geannuleerd")
                .ToListAsync();

            var orders = await ordersQuery
                .OrderBy(order => order.SentToDeliveryAt.HasValue)
                .ThenByDescending(order => order.OrderDate)
                .ToListAsync();

            var model = new ActiveOrdersViewModel
            {
                Orders = orders,
                StatusFilter = statusFilter,
                TotalActiveOrders = allActiveOrders.Count,
                IncomingOrders = allActiveOrders.Count(order => order.Source == "Externe website"),
                OrdersWithDeliveryPerson = allActiveOrders.Count(order => !string.IsNullOrWhiteSpace(order.DeliveryPerson))
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignDelivery(AssignDeliveryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Vul een geldige bezorger in.";
                return RedirectToAction(nameof(Active));
            }

            var order = await _context.Orders.FindAsync(model.OrderId);

            if (order == null)
            {
                return NotFound();
            }

            order.DeliveryPerson = model.DeliveryPerson.Trim();
            order.Status = "Doorgegeven aan bezorger";
            order.SentToDeliveryAt = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Order #{order.Id} is doorgegeven aan {order.DeliveryPerson}.";
            return RedirectToAction(nameof(Active));
        }

        [HttpPost]
        [Route("api/orders/incoming")]
        public async Task<IActionResult> Incoming([FromBody] IncomingOrderRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(existingCustomer =>
                    existingCustomer.Name == request.CustomerName &&
                    existingCustomer.Address == request.CustomerAddress);

            if (customer == null)
            {
                customer = new DataAccessLayer.Models.Customer
                {
                    Name = request.CustomerName.Trim(),
                    Address = request.CustomerAddress.Trim(),
                    Active = true
                };

                _context.Customers.Add(customer);
            }

            var order = new DataAccessLayer.Models.Order
            {
                Customer = customer,
                OrderDate = DateTime.Now,
                Status = "Nieuw",
                Source = "Externe website",
                ExternalReference = request.ExternalReference
            };

            if (request.ProductIds.Any())
            {
                var products = await _context.Products
                    .Where(product => request.ProductIds.Contains(product.Id))
                    .ToListAsync();

                foreach (var product in products)
                {
                    order.Products.Add(product);
                }
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Active), new { id = order.Id }, new
            {
                order.Id,
                order.Status,
                order.Source,
                order.ExternalReference,
                order.CustomerId
            });
        }
    }
}
