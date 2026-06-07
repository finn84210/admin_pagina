using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using KE03_INTDEV_SE_2_Base.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace KE03_INTDEV_SE_2_Base.Controllers
{
    public class ProductController : Controller
    {
        private const int LowStockThreshold = 5;
        private readonly IProductRepository _productRepository;

        public ProductController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public IActionResult Index(string? searchString, string? category, bool lowStockOnly = false)
        {
            var allProducts = _productRepository.GetAllProducts().ToList();
            var products = allProducts.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                products = products.Where(product =>
                    product.Name.Contains(searchString.Trim(), StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                products = products.Where(product =>
                    string.Equals(product.Category, category.Trim(), StringComparison.OrdinalIgnoreCase));
            }

            if (lowStockOnly)
            {
                products = products.Where(product => product.Stock < LowStockThreshold);
            }

            var model = new ProductIndexViewModel
            {
                Products = products.OrderBy(product => product.Name).ToList(),
                SearchString = searchString,
                Category = category,
                Categories = allProducts
                    .Select(product => product.Category)
                    .Where(categoryName => !string.IsNullOrWhiteSpace(categoryName))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(categoryName => categoryName)
                    .ToList(),
                LowStockOnly = lowStockOnly,
                TotalProducts = allProducts.Count,
                LowStockProducts = allProducts.Count(product => product.Stock < LowStockThreshold)
            };

            return View(model);
        }

        public IActionResult Details(int id)
        {
            var product = _productRepository.GetProductById(id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        public IActionResult Create()
        {
            return View(new ProductFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProductFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var product = new Product
            {
                Name = model.Name,
                Description = model.Description,
                Category = model.Category.Trim(),
                Price = model.Price,
                Stock = model.Stock
            };

            _productRepository.AddProduct(product);

            TempData["SuccessMessage"] = "Product is toegevoegd.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var product = _productRepository.GetProductById(id);

            if (product == null)
            {
                return NotFound();
            }

            var model = new ProductFormViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Category = product.Category,
                Price = product.Price,
                Stock = product.Stock
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, ProductFormViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var product = _productRepository.GetProductById(id);

            if (product == null)
            {
                return NotFound();
            }

            product.Name = model.Name;
            product.Description = model.Description;
            product.Category = model.Category.Trim();
            product.Price = model.Price;
            product.Stock = model.Stock;

            _productRepository.UpdateProduct(product);

            TempData["SuccessMessage"] = "Product is bijgewerkt.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var product = _productRepository.GetProductById(id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var product = _productRepository.GetProductById(id);

            if (product == null)
            {
                return NotFound();
            }

            _productRepository.DeleteProduct(product);

            TempData["SuccessMessage"] = "Product is verwijderd.";
            return RedirectToAction(nameof(Index));
        }
    }
}
