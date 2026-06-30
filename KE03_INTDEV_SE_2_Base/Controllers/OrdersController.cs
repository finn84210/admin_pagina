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
                .Where(order => order.Status != "Afgerond" && order.Status != "Geannuleerd" && order.Status != "Geleverd")
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(statusFilter))
            {
                ordersQuery = ordersQuery.Where(order => order.Status == statusFilter);
            }

            var allActiveOrders = await _context.Orders
                .Where(order => order.Status != "Afgerond" && order.Status != "Geannuleerd" && order.Status != "Geleverd")
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
                PickedOrders = allActiveOrders.Count(order => order.Status == "Gepickt"),
                OrdersWithDeliveryPerson = allActiveOrders.Count(order => !string.IsNullOrWhiteSpace(order.DeliveryPerson))
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Pick(int id)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            if (order.Status == "Afgerond" || order.Status == "Geannuleerd")
            {
                TempData["ErrorMessage"] = $"Order #{order.Id} kan niet meer gepickt worden.";
                return RedirectToAction(nameof(Active));
            }

            order.Status = "Gepickt";
            order.PickedAt = DateTime.Now;
            order.SentToDeliveryAt = DateTime.Now;
            order.DeliveryPerson = null;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Order #{order.Id} is gepickt en doorgestuurd naar de bezorgersapp.";
            return RedirectToAction(nameof(Active));
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

        [HttpGet]
        [Route("api/delivery/orders")]
        public async Task<IActionResult> DeliveryOrders()
        {
            var orders = await _context.Orders
                .Include(order => order.Customer)
                .Include(order => order.Products)
                .Where(order => order.Status == "Gepickt" && string.IsNullOrWhiteSpace(order.DeliveryPerson))
                .OrderBy(order => order.PickedAt)
                .Select(order => new
                {
                    order.Id,
                    order.OrderDate,
                    order.Status,
                    order.Source,
                    order.ExternalReference,
                    order.PickedAt,
                    order.SentToDeliveryAt,
                    Customer = new
                    {
                        order.Customer.Id,
                        order.Customer.Name,
                        order.Customer.Address
                    },
                    Products = order.Products.Select(product => new
                    {
                        product.Id,
                        product.Name,
                        product.Description,
                        product.Category,
                        product.Price
                    })
                })
                .ToListAsync();

            return Ok(orders);
        }

        [HttpPost]
        [Route("api/delivery/orders/{id:int}/claim")]
        public async Task<IActionResult> ClaimDeliveryOrder(int id, [FromBody] DeliveryClaimRequest? request)
        {
            if (request == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            if (order.Status != "Gepickt")
            {
                return BadRequest(new { message = "Deze order staat niet klaar voor de bezorgersapp." });
            }

            order.DeliveryPerson = request.DeliveryPerson.Trim();
            order.Status = "Doorgegeven aan bezorger";

            await _context.SaveChangesAsync();

            return Ok(new
            {
                order.Id,
                order.Status,
                order.DeliveryPerson,
                order.PickedAt,
                order.SentToDeliveryAt
            });
        }

        [HttpPatch]
        [Route("api/delivery/orders/{id:int}/status")]
        public async Task<IActionResult> UpdateDeliveryStatus(int id, [FromBody] DeliveryStatusUpdateRequest? request)
        {
            if (request == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var allowedStatuses = new[] { "Nog te leveren", "Onderweg", "Geleverd" };

            if (!allowedStatuses.Contains(request.Status))
            {
                return BadRequest(new { message = "Deze bezorgstatus is niet geldig." });
            }

            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            order.Status = request.Status;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                order.Id,
                order.Status,
                order.DeliveryPerson,
                order.PickedAt,
                order.SentToDeliveryAt
            });
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
