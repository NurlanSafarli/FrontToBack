using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FronyToBack.DAL;
using FronyToBack.Models;
using FronyToBack.Areas.ProniaAdmin.ViewModels.Tag;

namespace Pronia.Areas.ProniaAdmin.Controllers
{
    [Area("ProniaAdmin")]
    public class TagController : Controller
    {
        private readonly AppDbContext _context;

        public TagController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<Tag> Tags = await _context.Tags
                .Include(t => t.ProductTags)
                .ToListAsync();
            return View(Tags);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateTagVM tagVM)
        {
            if (!ModelState.IsValid)
            {
                return View();

            }

            bool isExist = await _context.Tags.AnyAsync(t => t.Name == tagVM.Name);

            if (isExist)
            {
                ModelState.AddModelError("Name", "bele tag var");
                return View();
            }

            await _context.Tags.AddAsync(tagVM);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));


        }

        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Tag tag = await _context.Tags.FirstOrDefaultAsync(t => t.Id == id);
            if (tag is null) return NotFound();
            return View(tag);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int? id, UpdateTagVM tagVM)
        {

            Tag oldTag = await _context.Tags.FirstOrDefaultAsync(t => t.Id == id);
            if (oldTag is null) return NotFound();

            if (!ModelState.IsValid)
            {
                return View(oldTag);
            }
            bool isExistName = await _context.Tags.AnyAsync(t => t.Name.Trim() == tagVM.Name.Trim() && t.Id != id);
            if (isExistName)
            {
                ModelState.AddModelError("Name", "bele tag var");
                return View();
            }


            if (oldTag.Name == tagVM.Name.Trim()) return RedirectToAction(nameof(Index));

            oldTag.Name = tagVM.Name;
            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();

            Tag tag = await _context.Tags.FirstOrDefaultAsync(t => t.Id == id);

            if (tag is null) return NotFound();

            _context.Tags.Remove(tag);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
    }
}
