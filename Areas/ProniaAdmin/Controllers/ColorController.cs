using FronyToBack.Areas.ViewModels.Color;
using FronyToBack.DAL;
using FronyToBack.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FronyToBack.Areas.ProniaAdmin.Controllers
{
    public class ColorController : Controller
    {

        public readonly AppDbContext _context;

        public ColorController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<Color> colors = await _context.Colors

                .Include(x => x.ProductColors)
                .ToListAsync();
            return View(colors);
        }
        public IActionResult Create()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateColorVM colorVM)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            bool result = _context.Colors.Any(x => x.Name == colorVM.Name);

            if (result)
            {
                ModelState.AddModelError("Name", "Bu adda color artiq movcuddur");
                return View();
            }
            Color color = new Color
            {
                Name = colorVM.Name,

            };
            await _context.Colors.AddAsync(color);


            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));


        }
        public async Task<IActionResult> Detail(int id)
        {
            if (id == null) return NotFound();


            Color existed = await _context.Colors.FirstOrDefaultAsync(s => s.Id == id);

            if (existed == null) return NotFound();

            return View(existed);

        }
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) BadRequest();
            Color existed = await _context.Colors.FirstOrDefaultAsync(t => t.Id == id);
            if (existed != null) NotFound();

            UpdateColorVM colorVM = new UpdateColorVM
            {
                Name = existed.Name,


            };
            return View(colorVM);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateColorVM colorVM)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            Color exist = await _context.Colors.FirstOrDefaultAsync(t => t.Id == id);

            if (exist != null) NotFound();

            bool result = await _context.Colors.AnyAsync(t => t.Name == colorVM.Name && t.Id != id);
            if (result)
            {
                ModelState.AddModelError("Name", "This Tag is aviable");
                return View();
            }

            exist.Name = colorVM.Name;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) BadRequest();
            Color color = await _context.Colors.FirstOrDefaultAsync(t => t.Id == id);

            if (color == null) NotFound();

            _context.Colors.Remove(color);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
