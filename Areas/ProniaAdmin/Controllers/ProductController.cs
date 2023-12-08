using FronyToBack.DAL;
using FronyToBack.Models;
using FronyToBack.Utilities.Enums;
using FronyToBack.Utilities.Extencions;
using FronyToBack.Utilities.Extentions;
using FronyToBack.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FronyToBack.Areas.ProniaAdmin.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> Index()
        {
            List<Product> products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages.Where(pi => pi.IsPrimary == true))
                .ToListAsync();
            return View(products);
        }
        public async Task<IActionResult> Detail(int id)
        {
            if (id == null) return NotFound();


            Product existed = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Include(p => p.ProductTags)
                .ThenInclude(pt => pt.Tag)
                .FirstOrDefaultAsync(s => s.Id == id);

            DetailVM detailVM = new DetailVM
            {
                Product = existed,
                RelatedProducts = await _context.Products.Where(p => p.CategoryId == existed.CategoryId && existed.Id != existed.Id).Take(12).Include(p => p.ProductImages).ToListAsync()
            };

            if (existed == null) return NotFound();

            return View(detailVM);

        }
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Product existed = await _context.Products.Include(p => p.ProductTags)
                .Include(p => p.ProductImages)
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
                ColorIds = existed.ProductColors.Select(p => p.ColorId).ToList(),
                SizeIds = existed.ProductSizes.Select(p => p.SizeId).ToList(),
                Categories = await _context.Categories.ToListAsync(),
                Tags = await _context.Tags.ToListAsync(),
                Colors = await _context.Colors.ToListAsync(),
                Sizes = await _context.Sizes.ToListAsync(),
                ProductImages = existed.ProductImages

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
                 .Include(p => p.ProductSizes)
                .Include(p => p.ProductColors)
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (existeed is null) return NotFound();

            if (productVM.MainPhoto is not null)
            {
                if (!productVM.MainPhoto.FileType.Image)
                {
                    productVM.categories = await _context.Categories.ToListAsync();
                    productVM.Tags = await _context.Tags.ToListAsync();
                    productVM.Colors = await _context.Colors.ToListAsync();
                    productVM.Sizes = await _context.Sizes.ToListAsync();
                    productVM.ProductImages = await _context.ProductImages.ToListAsync();

                    ModelState.AddModelError("MainPhoto", "Sekil tipi uygun deyl");
                    return View(productVM);
                }
                if (!productVM.MainPhoto.ValidateSize(600))
                {
                    productVM.categories = await _context.Categories.ToListAsync();
                    productVM.Tags = await _context.Tags.ToListAsync();
                    productVM.Colors = await _context.Colors.ToListAsync();
                    productVM.Sizes = await _context.Sizes.ToListAsync();
                    productVM.ProductImages = await _context.ProductImages.ToListAsync();

                    ModelState.AddModelError("MainPhoto", "Sekil olcusu uygun deyl");
                    return View(productVM);
                }


            }
            if (!productVM.HoverPhoto.FileType.Image)
            {
                productVM.categories = await _context.Categories.ToListAsync();
                productVM.Tags = await _context.Tags.ToListAsync();
                productVM.Colors = await _context.Colors.ToListAsync();
                productVM.Sizes = await _context.Sizes.ToListAsync();
                productVM.ProductImages = await _context.ProductImages.ToListAsync();

                ModelState.AddModelError("HoverPhoto", "Sekil tipi uygun deyl");
                return View(productVM);
            }
            if (!productVM.HoverPhoto.ValidateSize(600))
            {
                productVM.categories = await _context.Categories.ToListAsync();
                productVM.Tags = await _context.Tags.ToListAsync();
                productVM.Colors = await _context.Colors.ToListAsync();
                productVM.Sizes = await _context.Sizes.ToListAsync();
                productVM.ProductImages = await _context.ProductImages.ToListAsync();

                ModelState.AddModelError("HoverPhoto", "Sekil olcusu uygun deyl");
                return View(productVM);
            }

            bool result = await _context.Products.AnyAsync(c => c.Id == productVM.CategoryId);

            if (!result)
            {
                productVM.Categories = await _context.Categories.ToListAsync();
                productVM.Tags = await _context.Tags.ToListAsync();
                productVM.Colors = await _context.Colors.ToListAsync();
                productVM.Sizes = await _context.Sizes.ToListAsync();
                productVM.ProductImages = await _context.ProductImages.ToListAsync();
                ModelState.AddModelError("CategoryId", "bele categoriya movcud deyl");
                return View();
            }


            existeed.ProductTags.RemoveAll(pt => !productVM.TagIds.Exists(tId => tId == pt.TagId));

            List<int> creatable = productVM.TagIds.Where(tId => !existeed.ProductTags.Exists(pt => pt.TagId == tId)).ToList();


            foreach (var tId in creatable)
            {
                bool tagResult = await _context.Tags.AnyAsync(t => t.Id == tId);
                if (!tagResult)
                {
                    productVM.Categories = await _context.Categories.ToListAsync();
                    productVM.Tags = await _context.Tags.ToListAsync();
                    productVM.Colors = await _context.Colors.ToListAsync();
                    productVM.Sizes = await _context.Sizes.ToListAsync();
                    ModelState.AddModelError("CategoryId", "bele categoriya movcud deyl");
                    return View(productVM);
                }
                existeed.ProductTags.Add(new ProductTag { TagId = tId });
            }

            foreach (var item in existeed.ProductTags)
            {
                if (!productVM.TagIds.Exists(t => t == item.TagId))
                {
                    _context.ProductTags.Remove(item);
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
            foreach (var item in existeed.ProductSizes)
            {
                if (!productVM.SizeIds.Exists(t => t == item.SizeId))
                {
                    _context.ProductSizes.Remove(item);
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
            if (productVM.MainPhoto is not null)
            {

                string fillName = await productVM.MainPhoto.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images");
                ProductImage existedImg = existeed.ProductImages.FirstOrDefault(pi => pi.IsPrimary == true);
                existedImg.Url.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");
                existeed.ProductImages.Remove(existedImg);
                existeed.ProductImages.Add(new ProductImage
                {
                    IsPrimary = true,
                    Alternative = productVM.Name,
                    Url = fillName

                });
            }
            if (productVM.ImageIds is null)
            {
                productVM.ImageIds = new List<int>();
            }
            List<ProductImage> removeable = existeed.ProductImages.Where(pi => !productVM.ImageIds.Exists(imgId => imgId == pi.Id) && pi.IsPrimary == null).ToList();
            foreach (ProductImage removedImg in removeable)
            {
                removedImg.Url.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");
                existeed.ProductImages.Remove(removedImg);
            }

            existeed.Name = productVM.Name;
            existeed.Price = productVM.Price;
            existeed.SKU = productVM.SKU;
            existeed.Description = productVM.Description;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.Tags = await _context.Tags.ToListAsync();
            ViewBag.Sizes = await _context.Sizes.ToListAsync();
            ViewBag.Colors = await _context.Colors.ToListAsync();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateProductVM productVM)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _context.Categories.ToListAsync();
                ViewBag.Tags = await _context.Tags.ToListAsync();
                ViewBag.Sizes = await _context.Sizes.ToListAsync();
                ViewBag.Colors = await _context.Colors.ToListAsync();
                return View();
            }
            bool result = await _context.Categories.AnyAsync(c => c.Id == productVM.CategoryId);
            if (!result)
            {
                ViewBag.Categories = await _context.Categories.ToListAsync();
                ViewBag.Tags = await _context.Tags.ToListAsync();
                ViewBag.Sizes = await _context.Sizes.ToListAsync();
                ViewBag.Colors = await _context.Colors.ToListAsync();

                ModelState.AddModelError("CategoryId", $"Bu Id li categoria movcud deyl");
                return View();
            }
            foreach (int tagId in productVM.TagIds)
            {
                bool tagResult = await _context.Tags.AnyAsync(t => t.Id == tagId);
                if (!tagResult)
                {
                    ViewBag.Categories = await _context.Categories.ToListAsync();
                    ViewBag.Tags = await _context.Tags.ToListAsync();
                    ViewBag.Sizes = await _context.Sizes.ToListAsync();
                    ViewBag.Colors = await _context.Colors.ToListAsync();
                    ModelState.AddModelError("TagId", "tag melumatlari sefdir");
                    return View();

                }
            }
            foreach (int item in productVM.ColorIds)
            {
                bool cresult = await _context.Colors.AnyAsync(t => t.Id == item);
                if (!cresult)
                {
                    ViewBag.Categorys = await _context.Categories.ToListAsync();
                    ViewBag.Sizes = await _context.Sizes.ToListAsync();
                    ViewBag.Colors = await _context.Colors.ToListAsync();
                    ViewBag.Tagids = await _context.Tags.ToListAsync();
                    ModelState.AddModelError("ColorId", "Bu id li color movcud deyil");
                    return View();
                }
            }
            foreach (int item in productVM.SizeIds)
            {
                bool sresult = await _context.Sizes.AnyAsync(t => t.Id == item);
                if (!sresult)
                {
                    ViewBag.Categorys = await _context.Categories.ToListAsync();
                    ViewBag.Sizes = await _context.Sizes.ToListAsync();
                    ViewBag.Colors = await _context.Colors.ToListAsync();
                    ViewBag.Tagids = await _context.Tags.ToListAsync();
                    ModelState.AddModelError("SizeId", "Bu id li size movcud deyil");
                    return View();
                }
            }
            if (!productVM.MainPhoto.FileType.Image)
            {
                ViewBag.Categorys = await _context.Categories.ToListAsync();
                ViewBag.Sizes = await _context.Sizes.ToListAsync();
                ViewBag.Colors = await _context.Colors.ToListAsync();
                ViewBag.Tagids = await _context.Tags.ToListAsync();
                ModelState.AddModelError("MainPhoto", "Sekil tipi uygun deyl");
                return View();
            }
            if (!productVM.MainPhoto.ValidateSize(600))
            {
                ViewBag.Categorys = await _context.Categories.ToListAsync();
                ViewBag.Sizes = await _context.Sizes.ToListAsync();
                ViewBag.Colors = await _context.Colors.ToListAsync();
                ViewBag.Tagids = await _context.Tags.ToListAsync();
                ModelState.AddModelError("MainPhoto", "Sekil olcusu uygun deyl");
                return View();
            }
            if (!productVM.HoverPhoto.FileType.Image)
            {
                ViewBag.Categorys = await _context.Categories.ToListAsync();
                ViewBag.Sizes = await _context.Sizes.ToListAsync();
                ViewBag.Colors = await _context.Colors.ToListAsync();
                ViewBag.Tagids = await _context.Tags.ToListAsync();
                ModelState.AddModelError("HoverPhoto", "Sekil tipi uygun deyl");
                return View();
            }
            if (!productVM.HoverPhoto.ValidateSize(600))
            {
                ViewBag.Categorys = await _context.Categories.ToListAsync();
                ViewBag.Sizes = await _context.Sizes.ToListAsync();
                ViewBag.Colors = await _context.Colors.ToListAsync();
                ViewBag.Tagids = await _context.Tags.ToListAsync();
                ModelState.AddModelError("HoverPhoto", "Sekil olcusu uygun deyl");
                return View();
            }

            ProductImage main = new ProductImage
            {
                IsPrimary = true,
                Url = await productVM.MainPhoto.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images"),
                Alternative = productVM.Name
            };
            ProductImage hover = new ProductImage
            {
                IsPrimary = false,
                Url = await productVM.HoverPhoto.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images"),
                Alternative = productVM.Name
            };



            Product product = new Product
            {
                CategoryId = (int)productVM.CategoryId,
                Name = productVM.Name,
                Price = productVM.Price,
                SKU = productVM.SKU,
                Description = productVM.Description,
                ProductTags = new List<ProductTag>(),
                ProductSizes = new List<ProductSize>(),
                ProductColors = new List<ProductColor>(),
                ProductImages = new List<ProductImage>() { main, hover }


            };
            TempData["Mesage"] = "";
            foreach (IFormFile photo in productVM.Photos ?? new List<IFormFile>())
            {
                if (photo.FileType.Image)
                {
                    TempData["Message"] += $" <p class=\"text-danger\">{photo.FileName} adli file tipi  uygun deyl</p>";
                    continue;
                }
                if (!photo.ValidateSize(600))
                {
                    TempData["Message"] += $"<p class=\"text-danger\">{photo.FileName} adli file olcusu  uygun deyl</p>";
                    continue;
                }
                product.ProductImages.Add(new ProductImage
                {

                    IsPrimary = null,
                    Alternative = productVM.Name,
                    Url = await photo.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images")
                });

            }
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
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();
            Product existed = await _context.Products
                .Include(p => p.ProductImages)

                .FirstOrDefaultAsync(p => p.Id == id);
            if (existed is null) return NotFound();
            foreach (var item in existed.ProductImages ?? new List<ProductImage>())
            {
                item.Url.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");
            };
            _context.Products.Remove(existed);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
