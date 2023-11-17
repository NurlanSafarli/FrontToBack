using FronyToBack.DAL;
using FronyToBack.Models;
using FronyToBack.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FronyToBack.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            List<Slide> sliders = _context.Slides.ToList();
            List<Product> products = _context.Products.OrderBy(p => p.Id).Take(8).ToList();

            HomeVM allClassesInOne = new HomeVM
            {
                Slides = sliders,
                Products = products
            };

            return View(allClassesInOne);
        }
        public IActionResult About()
        {
            return View();
        }
    }
}
