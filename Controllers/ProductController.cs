using FronyToBack.DAL;
using FronyToBack.Models;
using Microsoft.AspNetCore.Mvc;

namespace FronyToBack.Controllers
{




    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Detail(int id)
        {
            if (id<=0) return BadRequest();
          
            Product product= _context.Products.FirstOrDefault(P => P.Id == id);

            if (product==null) return NotFound();

            return View(product);
        }
    }
}
