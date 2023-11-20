using FronyToBack.DAL;
using FronyToBack.Models;
using FronyToBack.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FronyToBack.Controllers
{




    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Details(int id)
        {
            Product product = _context.Products
                .Include(p => p.Images)
                .Include(p => p.Category)
                .Include(p => p.ProductTags).ThenInclude(pt => pt.Tag)
                .Include(p => p.ProductColors).ThenInclude(pc => pc.Color)
                .Include(p => p.ProductSizes).ThenInclude(ps => ps.Size)
                .FirstOrDefault(p => p.Id == id);
            List<Product> products = _context.Products.Where(p => p.CategoryId == product.CategoryId && p.Id != product.Id).Include(p => p.Images).ToList();

            ProductVM productVM = new ProductVM
            {

                Product = product,
                Products = products

            };
            if (productVM.Product == null) return NotFound();

            return View(productVM);
        }
    }
}
