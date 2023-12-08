using FronyToBack.Areas.ProniaAdmin.ViewModels.Color;
using FronyToBack.DAL;
using FronyToBack.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FronyToBack.Areas.ProniaAdmin.Controllers
{
    public class SizeController : Controller
    {
        private readonly AppDbContext _context;
        public SizeController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<Size> tags = await _context.Sizes
                .Include(x => x.ProductSizes)
                .ToListAsync();
            return View(tags);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateColorVM sizeVM)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            bool result = _context.Sizes.Any(x => x.Name == sizeVM.Name);

            if (result)
            {
                ModelState.AddModelError("Name", "Bu adda tag artiq movcuddur");
                return View();
            }
            Size size = new Size
            {
                Name = sizeVM.Name,
            };
            await _context.Sizes.AddAsync(size);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return View();
            Size size = await _context.Sizes.FirstOrDefaultAsync(c => c.Id == id);
            if (size is null) return NotFound();
            //return View(size);
            UpdateColorVM sizeVM = new UpdateColorVM
            {
                Name = size.Name,
            };
            return View(sizeVM);


        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateColorVM sizeVM)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            Size existed = await _context.Sizes.FirstOrDefaultAsync(c => c.Id == id);
            if (existed is null) return NotFound();
            bool result = await _context.Categories.AnyAsync(c => c.Name == sizeVM.Name && c.Id != id);
            if (result)
            {
                ModelState.AddModelError("Name", "bu adda category artiq movcuddur");
                return View();
            }
            existed.Name = sizeVM.Name;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();
            Size existed = await _context.Sizes.FirstOrDefaultAsync(t => t.Id == id);

            if (existed is null) return NotFound();

            _context.Sizes.Remove(existed);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Detail(int id)
        {
            if (id == null) return NotFound();


            Size existed = await _context.Sizes.FirstOrDefaultAsync(s => s.Id == id);

            if (existed == null) return NotFound();

            return View(existed);

        }
    }
}
