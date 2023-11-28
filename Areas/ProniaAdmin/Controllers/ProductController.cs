using FronyToBack.DAL;
using FronyToBack.Models;
using FronyToBack.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FronyToBack.Areas.ProniaAdmin.Controllers
{
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
                .Include(p => p.ProductImages.Where(pi => pi.IsPrimary == true))
                .Include(p => p.ProductTags)
                .Include(p => p.Tag)
                .ToListAsync();
            return View(products);
        }
        public async Task<IActionResult> Detail(int id)
        {
            if (id == null) return NotFound();


            Product existed = await _context.Products.FirstOrDefaultAsync(s => s.Id == id);

            if (existed == null) return NotFound();

            return View(existed);

        }
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Product existed = await _context.Products.Include(p => p.ProductTags)
                .Include(p => p.ProductColors)
              .Include(p => p.ProductSizes)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (existed is null) return NotFound();


            UpdateProductVM ProductVM = new UpdateProductVM
            {
                Description = existed.Description,
                Name = existed.Name,
                Price = existed.Price,
                SKU = existed.SKU,
                CategoryId = existed.CategoryId,
                TagIds = existed.ProductTags.Select(p => p.TagId).ToList(),
                ColorIds = product.ProductColors.Select(p => p.ColorId).ToList(),
                SizeIds = product.ProductSizes.Select(p => p.SizeId).ToList(),
                Categories = await _context.Categories.ToListAsync(),
                Tags = await _context.Tags.ToListAsync(),
                Colors = await _context.Colors.ToListAsync(),
                Sizes = await _context.Sizes.ToListAsync(),

            };
            return View(ProductVM);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateProductVM productVM)
        {



            if (!ModelState.IsValid)
            {
                productVM.categories = await _context.Categories.ToListAsync();
                productVM.Tags = await _context.Tags.ToListAsync();
                productVM.Colors = await _context.Colors.ToListAsync();
                productVM.Sizes = await _context.Sizes.ToListAsync();
                return View(productVM);
            }

            Product existeed = await _context.Products
                .Include(p => p.ProductTags)




                .FirstOrDefaultAsync(p => p.Id == id);
            if (existeed is null) return NotFound();
            bool result = await _context.Products.AnyAsync(c => c.Id == productVM.CategoryId);

            if (!result)
            {
                productVM.Categories = await _context.Categories.ToListAsync();
                productVM.Tags = await _context.Tags.ToListAsync();
                productVM.Colors = await _context.Colors.ToListAsync();
                productVM.Sizes = await _context.Sizes.ToListAsync();
                ModelState.AddModelError("CategoryId", "bele categoriya movcud deyl");
                return View();
            }
            foreach (ProductTag pTag in existeed.ProductTags)
            {
                if (!productVM.TagIds.Exists(tId => tId == pTag.TagId))
                {
                    _context.ProducsTags.Remove(pTag);
                }

            }
            foreach (int TagId in productVM.TagIds)
            {
                if (!existeed.ProductTags.Any(p => p.TagId == id))
                {


                    existeed.ProductTags.Add(new ProductTag
                    {
                        TagId = TagId
                    });

                }
            }
            foreach (ColorTag cTag in existeed.ColorsTags)
            {
                if (!productVM.ColorIds.Exists(tId => tId == cTag.TagId))
                {
                    _context.ColorTags.Remove(cTag);
                }

            }
            foreach (int ColorId in productVM.ColorIds)
            {
                if (!existeed.ProductColors.Any(p => p.TagId == id))
                {


                    existeed.ProductTags.Add(new ProductTag
                    {
                        TagId = ColorId
                    });

                }
            }
            foreach (ProductTag pTag in existeed.ProductSizes)
            {
                if (!productVM.SizeIds.Exists(tId => tId == pTag.TagId))
                {
                    _context.ProducsTags.Remove(pTag);
                }

            }
            foreach (int SizeId in productVM.SizeIds)
            {
                if (!existeed.ProductTags.Any(p => p.TagId == id))
                {


                    existeed.ProductTags.Add(new ProductTag
                    {
                        TagId = SizeId
                    });

                }
            }


            existeed.Name = productVM.Name;
            existeed.Price = productVM.Price;
            existeed.SKU = productVM.SKU;
            existeed.Description = productVM.Description;

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Create()
        {
            CreateProductVM productVM = new CreateProductVM();
            productVM.Categories = await _context.Categories.ToListAsync();
            productVM.Tags = await _context.Tags.ToListAsync();
            productVM.Sizes = await _context.Sizes.ToListAsync();
            productVM.Colors = await _context.Colors.ToListAsync();
            return View(productVM);
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateProductVM productVM)
        {
            if (!ModelState.IsValid)
            {
                productVM.Categories = await _context.Categories.ToListAsync();
                productVM.Tags = await _context.Tags.ToListAsync();
                productVM.Sizes = await _context.Sizes.ToListAsync();
                productVM.Colors = await _context.Colors.ToListAsync();
                return View(productVM);
            }
            bool result = await _context.Categories.AnyAsync(c => c.Id == productVM.CategoryId);
            if (!result)
            {
                productVM.Categories = await _context.Categories.ToListAsync();
                productVM.Tags = await _context.Tags.ToListAsync();
                productVM.Sizes = await _context.Sizes.ToListAsync();
                productVM.Colors = await _context.Colors.ToListAsync();

                ModelState.AddModelError("CategoryId", $"Bu Id li categoria movcud deyl");
                return View(productVM);
            }
            foreach (int tagId in productVM.TagIds)
            {
                bool tagResult = await _context.Tags.AnyAsync(t => t.Id == tagId);
                if (!tagResult)
                {
                    productVM.Categories = await _context.Categories.ToListAsync();
                    productVM.Tags = await _context.Tags.ToListAsync();
                    productVM.Sizes = await _context.Sizes.ToListAsync();
                    productVM.Colors = await _context.Colors.ToListAsync();
                    ModelState.AddModelError("TagId", "tag melumatlari sefdir");
                    return View(productVM);

                }
            }
            foreach (int item in productVM.ColorIds)
            {
                bool cresult = await _context.Colors.AnyAsync(t => t.Id == item);
                if (!cresult)
                {
                    productVM.Categorys = await _context.Categories.ToListAsync();
                    productVM.Sizes = await _context.Sizes.ToListAsync();
                    productVM.Colors = await _context.Colors.ToListAsync();
                    productVM.Tagids = await _context.Tags.ToListAsync();
                    ModelState.AddModelError("ColorId", "Bu id li color movcud deyil");
                    return View();
                }
            }
            foreach (int item in productVM.SizeIds)
            {
                bool sresult = await _context.Sizes.AnyAsync(t => t.Id == item);
                if (!sresult)
                {
                    productVM.Categorys = await _context.Categories.ToListAsync();
                    productVM.Sizes = await _context.Sizes.ToListAsync();
                    productVM.Colors = await _context.Colors.ToListAsync();
                    productVM.Tagids = await _context.Tags.ToListAsync();
                    ModelState.AddModelError("SizeId", "Bu id li size movcud deyil");
                    return View(productVM);
                }
            }


            Product product = new Product
            {
                CategoryId = (int)productVM.CategoryId,
                Name = productVM.Name,
                Price = productVM.Price,
                SKU = productVM.SKU,
                Description = productVM.Description,
                ProductTags = new List<ProductTag>(),
                ProductSizes = new List<ProductSize>(),
                ProductColors = new List<ProductColor>()


            };
            foreach (var tagId in productVM.TagIds)
            {
                ProductTag productTag = new ProductTag
                {
                    TagId = tagId,


                };
                product.ProductTags.Add(productTag);

            }
            foreach (var ColorId in productVM.ColorIds)
            {
                ProductColor productColor = new ProductColor
                {
                    ColorId = ColorId,
                };
                product.ProductColors.Add(productColor);
            }
            foreach (var SizeId in productVM.SizeIds)
            {
                ProductSize productSize = new ProductSize
                {
                    SizeId = SizeId,
                };
                product.ProductSizes.Add(productSize);
            }

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
