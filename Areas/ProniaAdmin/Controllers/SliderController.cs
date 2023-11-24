using FronyToBack.DAL;
using FronyToBack.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class SliderController : Controller
{
    private readonly AppDbContext _context;
    public SliderController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {

        List<Slide> slides = await _context.Slides.ToListAsync();
        return View(slides);
    }
    public IActionResult Create()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Create(Slide slide)
    {

        if (slide.ImageFile is null)
        {
            ModelState.AddModelError("Photo", "Shekil mutleq secilmelidir");
            return View();
        }

        if (!slide.ImageFile.ContentType.Contains("image/"))
        {
            ModelState.AddModelError("Photo", "File tipi uyqun deyil");
            return View();
        }
        if (slide.ImageFile.Length > 2 * 1024 * 1024)
        {
            ModelState.AddModelError("Photo", "File olcusu 2-mb den boyuk olmamalidir");
            return View();

        }

        FileStream fileStream = new FileStream(@"C:\Users\nurla\OneDrive\Desktop\FrontToBack\wwwroot\admin\images\logo.svg" + slide.ImageFile.FileName, FileMode.Create);
        await slide.ImageFile.CopyToAsync(fileStream);
        slide.Image = slide.ImageFile.FileName;

        return Content(slide.ImageFile.FileName + " " + slide.ImageFile.ContentType + " " + slide.ImageFile.Length);
        await _context.Slides.AddAsync(slide);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> Delete(int? id)
    {
      
            await _context.Delete(id);
            return RedirectToAction(nameof(Index));

    }

    public async Task<IActionResult> Update(int? id)
    {
        
            return View(await _context.GetById(id));
   
    }
    [HttpPost]
    public async Task<IActionResult> Update(int? id, Slide slide)
    {
      
            await _context.Update(slide);
            return RedirectToAction(nameof(Index));

    }
}

