using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FronyToBack.DAL;
using FronyToBack.Models;

using FronyToBack.ViewModels;


namespace FronyToBack.Areas.ProniaAdmin.Controllers
{
    [Area("ProniaAdmin")]
    [AutoValidateAntiforgeryToken]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;


        public ProductController(AppDbContext context)
        {
            _context = context;

        }
        public async Task<IActionResult> Index()
        {
            List<Product> products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductTags)
                .ThenInclude(pt => pt.Tag)
                .ToListAsync();
            return View(products);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.Tags = await _context.Tags.ToListAsync();
            return View();

        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductVM productVM)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _context.Categories.ToListAsync();
                ViewBag.Tags = await _context.Tags.ToListAsync();
                return View();
            }
            bool result = await _context.Categories.AnyAsync(c => c.Id == productVM.CategoryId);
            if (!result)
            {
                ViewBag.Categories = await _context.Categories.ToListAsync();
                ViewBag.Tags = await _context.Tags.ToListAsync();
                ModelState.AddModelError("CategoryId", "Bu id-li category movcud deyil");
                return View();
            }

            Product product = new Product
            {
                Name = productVM.Name,
                Description = productVM.Description,
                Price = (decimal)productVM.Price,
                SKU = productVM.SKU,
                CategoryId = productVM.CategoryId,
                ProductTags = new List<ProductTag>(),

            };

            foreach (int tagId in productVM.TagIds)
            {
                bool tagResult = await _context.Tags.AnyAsync(t => t.Id == tagId);
                if (!tagResult)
                {
                    ViewBag.Categories = await _context.Categories.ToListAsync();
                    ViewBag.Tags = await _context.Tags.ToListAsync();
                    ModelState.AddModelError("TagIds", $"{tagId} id-li tag movcud deyil");
                    return View();
                }

                ProductTag productTag = new ProductTag
                {
                    TagId = tagId,
                    Product = product,
                };

                product.ProductTags.Add(productTag);
            }



            async Task<IActionResult> Update(int? id)
            {
                if (id == null || id < 1) return BadRequest();
                Product product = await _context.Products.Where(p => p.Id == id).Include(p => p.ProductTags).FirstOrDefaultAsync();
                if (product == null) return NotFound();
                UpdateProductVM productVM = new UpdateProductVM
                {
                    Name = product.Name,
                    Description = product.Description,
                    SKU = product.SKU,
                    Price = (double)product.Price,
                    CategoryId = product.CategoryId,
                    TagIds = product.ProductTags.Select(p => p.TagId).ToList()
                };

                ViewBag.Categories = await _context.Categories.ToListAsync();
                ViewBag.Tags = await _context.Tags.ToListAsync();
                return View(productVM);

            }
            return RedirectToAction(nameof(Index));
        }

    } }
